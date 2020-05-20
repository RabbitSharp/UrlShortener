using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Infrastructure
{
    /*
     * Anti-Pattern
     */
    public interface IServiceLocator
    {
        T GetService<T>() where T : class;
        void RegisterService<T>(T instance) where T : class;
        static IServiceLocator Instance { get; } = ServiceLocator.Instance;
        void RegisterServices(HttpRequest req, ILogger log, ExecutionContext context);
    }

    public sealed class ServiceLocator : IServiceLocator
    {
        public Dictionary<object, object> Container;

        private ServiceLocator()
        {
            Container = new Dictionary<object, object>();
        }

        public static IServiceLocator Instance { get; } = new ServiceLocator();

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
            try
            {
                Container.Add(typeof(T), instance);
            }
            catch (Exception e)
            {
                throw new Exception("Service is already registered.", e);
            }
        }

        public void RegisterServices(HttpRequest req, ILogger log, ExecutionContext context)
        {
            Container.Clear();
            RegisterService(req);
            RegisterService(log);
            RegisterService(context);
            RegisterService(new Config());
            RegisterService(new StorageTableHelper());
            RegisterService(new UrlRepository());
        }
    }
}