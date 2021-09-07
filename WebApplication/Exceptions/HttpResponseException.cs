using System;
using System.Net;

namespace WebApplication.Exceptions
{
    /// <summary>
    /// Exception with status code, status code is extracted in Exception middleware and used as a status response
    /// </summary>
    public class HttpResponseException : Exception
    {
        public HttpResponseException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}