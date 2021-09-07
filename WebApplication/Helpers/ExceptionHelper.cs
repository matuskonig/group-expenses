using System.Net;
using WebApplication.Exceptions;

namespace WebApplication.Helpers
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Throws HttpResponseException, making the message and status code required as part of contract
        /// </summary>
        /// <param name="message">Message, which will be passed to exception and eventually be shown in serialized message</param>
        /// <param name="code">Http status code, which will be returned as a result of API call</param>
        /// <exception cref="HttpResponseException">Exception, which will be caught by middleware</exception>
        public static void HttpResponseException(string message, HttpStatusCode code)
        {
            throw new HttpResponseException(message, code);
        }
    }
}