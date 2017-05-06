using System;

namespace LeagueClientApi.Exceptions
{
    /// <summary>
    /// LeagueClientBase exception. Throws if the error is not related to an request
    /// </summary>
    public class LeagueClientBaseException : Exception
    {
        public LeagueClientBaseException(string message) : base(message) { }
    }
}
