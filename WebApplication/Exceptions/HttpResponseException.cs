using System;
using System.Net;

namespace WebApplication.Exceptions
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException()
        {
        }

        public HttpResponseException(string message) : base(message)
        {
        }

        public HttpResponseException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.InternalServerError;
    }
}