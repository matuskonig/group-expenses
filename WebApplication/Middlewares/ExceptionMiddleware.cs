using System;
using System.Threading.Tasks;
using Entities.Dto.General;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using WebApplication.Exceptions;

namespace WebApplication.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment hostEnvironment)
        {
            _next = next;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Middleware pipeline processor, top-level catch block
        /// </summary>
        /// <param name="context">Request context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleThrownException(context, exception);
            }
        }

        /// <summary>
        /// Exception handler, writes ProblemDetail with exception detail as the API response
        /// </summary>
        /// <param name="context">Request context</param>
        /// <param name="exception">Exception thrown down the pipeline</param>
        private async Task HandleThrownException(HttpContext context, Exception exception)
        {
            var statusCode = exception is HttpResponseException responseException
                ? (int)responseException.StatusCode
                : 500;
            var response = new ProblemDetail
            {
                Title = exception.Message,
                Detail = _hostEnvironment.IsDevelopment() ? exception.StackTrace : null,
                Status = statusCode
            };
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}