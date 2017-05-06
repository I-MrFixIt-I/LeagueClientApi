using System;
using System.Net;

namespace LeagueClientApi.Exceptions
{
    /// <summary>
    /// LeagueClientRequest exception. Throws if the error is related to an request
    /// </summary>
    public class LeagueClientRequestException : Exception
    {
        /// <summary>
        /// HTTP error code returned by the League Client, causing this exception.
        /// </summary>
        public readonly HttpStatusCode HttpStatusCode;

        /// <summary>
        /// Response returned by the League Client, causing this exception. 
        /// </summary>
        public readonly string HttpResponse;

        public LeagueClientRequestException(string message, string httpResponse, HttpStatusCode httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
            HttpResponse = httpResponse;
        }
    }
}
