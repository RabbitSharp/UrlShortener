using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Exceptions;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Domain.Models;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Domain
{
    public class UrlService
    {
        private readonly UrlRepository _urlRepository;
        private readonly ClickStatisticRepository _statRepository;
        private readonly ILogger<UrlService> _logger;
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";

        public UrlService(UrlRepository urlRepository, ClickStatisticRepository statRepository, ILogger<UrlService> logger)
        {
            _urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
            _statRepository = statRepository ?? throw new ArgumentNullException(nameof(statRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Url> Get(string shortUrl)
        {
            var storedUrl = await _urlRepository.GetEntity(new Url(shortUrl));
            if (storedUrl == null)
            {
                throw new EntityNotFoundException($"Url for '{shortUrl}' not found.");
            }

            _logger.LogInformation($"Found: {storedUrl.LongUrl}");
            storedUrl.ClickCount++;
            var statistic = new ClickStatistic(storedUrl.RowKey);
            await _statRepository.Save(statistic);
            await _urlRepository.Save(storedUrl);
            return storedUrl;
        }

        public async Task<Url> Add(string sourceUrl, string tail, string desc)
        {
            sourceUrl = sourceUrl.Trim();
            tail = string.IsNullOrWhiteSpace(tail) ? await CreateTail() : tail.Trim();
            desc = string.IsNullOrWhiteSpace(desc) ? $"Entry added at {DateTime.UtcNow} UTC" : desc.Trim();

            var newUrl = new Url(sourceUrl, tail, desc);
            if (await _urlRepository.ExistAsync(newUrl))
            {
                throw new ConflictException("This Short URL already exist.");
            }

            return await _urlRepository.Save(newUrl);
        }

        public async Task<Url> Update(string sourceUrl, string tail, string desc)
        {
            if (string.IsNullOrWhiteSpace(tail))
            {
                throw new ValidationException("Tail has to be set.");
            }
            
            var updateEntity = new Url(sourceUrl.Trim(), tail.Trim(), desc.Trim());
            var originalEntity = await _urlRepository.GetEntity(updateEntity);
            if (originalEntity == null)
            {
                throw new NotFoundException($"URL with '{updateEntity.RowKey}' tail could not be found.");
            }

            originalEntity.LongUrl = updateEntity.LongUrl;
            originalEntity.Description = updateEntity.Description;
            
            return await _urlRepository.Save(originalEntity);
        }

        private async Task<string> CreateTail()
        {
            var newKey = await _urlRepository.GetNextTableId();
            return Encode(newKey);
        }

        private string Encode(int i)
        {
            if (i == 0)
            {
                return Alphabet[0].ToString();
            }
                
            var s = string.Empty;
            while (i > 0)
            {
                s += Alphabet[i % Alphabet.Length];
                i /= Alphabet.Length;
            }

            return string.Join(string.Empty, s.Reverse());
        }
    }
}