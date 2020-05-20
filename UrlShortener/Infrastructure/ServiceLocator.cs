using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace UrlShortener.Infrastructure
{
    public interface IServiceLocator
    {
        T GetService<T>() where T : class;
        void RegisterService<T>(T instance) where T : class;
        static IServiceLocator GetInstance { get; } = ServiceLocator.GetInstance;
        void RegisterExternalServices(HttpRequest req, ILogger log, ExecutionContext context);
    }

    public sealed class ServiceLocator : IServiceLocator
    {
        public Dictionary<object, object> Container;

        private ServiceLocator()
        {
            Container = new Dictionary<object, object>();
        }

        public static IServiceLocator GetInstance { get; } = new ServiceLocator();

        public T GetService<T>()
            where T : class
        {
            try
            {
                return (T)Container[typeof(T)];
            }
            catch (Exception)
            {
                throw new NotImplementedException("Service not available.");
            }
        }

        public void RegisterService<T>(T instance)
            where T : class
        {
            Container.Add(typeof(T), instance);
        }

        public void RegisterExternalServices(HttpRequest req, ILogger log, ExecutionContext context)
        {
            RegisterService(req);
            RegisterService(log);
            RegisterService(context);
            RegisterInternalServices();
        }

        private void RegisterInternalServices()
        {
            RegisterService(new Config());
            RegisterService(new StorageTableHelper());
        }
    }
}