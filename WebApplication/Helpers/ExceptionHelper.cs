using System.Net;
using WebApplication.Exceptions;

namespace WebApplication.Helpers
{
    public static class ExceptionHelper
    {
        public static void HttpResponseException(string message, HttpStatusCode code)
        {
            throw new HttpResponseException(message, code);
        }
        
    }
}