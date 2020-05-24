using System;
using Microsoft.Extensions.Configuration;

namespace UrlShortener.Infrastructure
{
    public class Config
    {
        public Config(IConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config["STORAGE_CONNECTION_STRING"]))
            {
                throw new ArgumentException("Configuration is invalid.", StorageConnectionString);
            }

            StorageConnectionString = config["STORAGE_CONNECTION_STRING"];
        }

        public string StorageConnectionString { get; set; }
    }
}