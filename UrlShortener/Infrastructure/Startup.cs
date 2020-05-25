using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Domain;
using UrlShortener.Domain.Repositories;

[assembly: FunctionsStartup(typeof(UrlShortener.Infrastructure.Startup))]
namespace UrlShortener.Infrastructure
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<StorageTableHelper>();
            builder.Services.AddScoped<Config>();
            builder.Services.AddScoped<UrlRepository>();
            builder.Services.AddScoped<ClickStatisticRepository>();
            builder.Services.AddScoped<UrlService>();
            builder.Services.AddScoped<StatisticService>();
            builder.Services.AddLogging();
        }
    }
}