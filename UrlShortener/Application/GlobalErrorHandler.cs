using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrlShortener.Domain.Exceptions;
using ApplicationException = UrlShortener.Application.Exceptions.ApplicationException;

namespace UrlShortener.Application
{
    internal static class GlobalErrorHandler
    {
        internal static async Task<IActionResult> HandleExceptionAsync<T>(Func<Task<IActionResult>> action, ILogger<T> logger)
        {
            try
            {
                return await action();
            }
            catch (DomainException de)
            {
                logger.LogError(de, "An domain error encountered.");
                return de.HttpResult;
            }
            catch (ApplicationException ae)
            {
                logger.LogError(ae, "An validation error encountered.");
                return ae.HttpResult;
            }
            catch (Exception e)
            {
                logger.LogError(e, "An unexpected error was encountered.");
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}