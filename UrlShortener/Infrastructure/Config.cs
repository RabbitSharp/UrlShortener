using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace UrlShortener.Infrastructure
{
    public class Config
    {
        private readonly ExecutionContext _executionContext;

        public Config()
        {
            var locator = IServiceLocator.Instance;
            _executionContext = locator.GetService<ExecutionContext>();
            BuildConfig();
        }

        public string StorageConnectionString { get; set; }

        public void BuildConfig()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(_executionContext.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            StorageConnectionString = config["STORAGE_CONNECTION_STRING"];
        }
    }
}