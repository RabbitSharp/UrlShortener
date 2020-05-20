using System;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Domain.Models;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure;

namespace UrlShortener.Domain
{
    public class UrlService
    {
        private readonly UrlRepository _urlRepository;
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";

        public UrlService()
        {
            var loc = IServiceLocator.Instance;
            _urlRepository = loc.GetService<UrlRepository>();
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

        private async Task<string> CreateTail()
        {
            var newKey = await _urlRepository.GetNextTableId();
            return string.Join(string.Empty, Encode(newKey)); //sinnvoll?
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