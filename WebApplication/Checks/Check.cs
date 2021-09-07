using System.Net;
using WebApplication.Helpers;

namespace WebApplication.Checks
{
    /// <summary>
    /// Class used for checks, which failing results in HttpResponseException
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Ensures, that the value is null, else throws exception
        /// </summary>
        public static void Null<T>(T value, string message, HttpStatusCode code = HttpStatusCode.BadRequest) {
            if (value != null)
            {
                ExceptionHelper.HttpResponseException(message, code);
            }
        }
        /// <summary>
        /// Ensures, that the value is not null, else throws exception
        /// </summary>
        public static void NotNull<T>(T value, string message, HttpStatusCode code = HttpStatusCode.BadRequest) {
            if (value == null)
            {
                ExceptionHelper.HttpResponseException(message, code);
            }
        }

        /// <summary>
        /// Ensures, that the condition holds, otherwise throws exception
        /// </summary>
        public static void Guard(bool flag, string message, HttpStatusCode code = HttpStatusCode.BadRequest)
        {
            if (!flag)
            {
                ExceptionHelper.HttpResponseException(message, code);
            }
        }
        
    }

}