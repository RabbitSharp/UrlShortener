using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Domain
{
    public class StatisticService
    {
        private readonly ClickStatisticRepository _statRepository;

        public StatisticService(ClickStatisticRepository statisticRepository)
        {
            _statRepository = statisticRepository ?? throw new ArgumentNullException(nameof(statisticRepository));
        }

        public async Task<IEnumerable<ClickStatistic>> GetAllByTail(string tail)
        {
            throw new NotImplementedException();
        }

        public async Task<ClickStatistic> Update(Url url)
        {
            var statistic = new ClickStatistic(url.RowKey);
            return await _statRepository.Save(statistic);
        }
    }
}