using System.Net;
using WebApplication.Helpers;

namespace WebApplication.Checks
{
    public static class Check
    {
        public static void Null<T>(T value, string message, HttpStatusCode code = HttpStatusCode.BadRequest) {
            if (value != null)
            {
                ExceptionHelper.HttpResponseException(message, code);
            }
        }
        public static void NotNull<T>(T value, string message, HttpStatusCode code = HttpStatusCode.BadRequest) {
            if (value == null)
            {
                ExceptionHelper.HttpResponseException(message, code);
            }
        }

        public static void Guard(bool flag, string message, HttpStatusCode code = HttpStatusCode.BadRequest)
        {
            if (!flag)
            {
                ExceptionHelper.HttpResponseException(message, code);
            }
        }
        
    }

}