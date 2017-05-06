using LeagueClientApi.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace LeagueClientApi
{
    public partial class LeagueClient
    {
        /// <summary>
        /// Retrieves the SummonerId of the currently logged in user account
        /// </summary>
        /// <returns>Returns an SummonerId</returns>
        /// <exception cref="LeagueClientRequestException">Occurs when the response StatusCode is not OK</exception>
        /// <exception cref="JsonReaderException">Occurs when the response contains an invalid JSON</exception>
        /// <exception cref="KeyNotFoundException">Occurs when the JSON-response does not contains the requested key</exception>
        public int GetSummonerId()
        {
            HttpWebRequest request = CreateRequest("lol-login/v1/session", "GET");
            HttpWebResponse response = SubmitRequest(request, out string content);

            // Expected StatusCode is OK, so throw Exception if it differs
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new LeagueClientRequestException("An error occurred while fetching SummonerId", content, response.StatusCode);
            }
            else
            {
                JObject jObject = JObject.Parse(content);
                JToken jToken = jObject.GetValue("summonerId");

                if (jToken == null)
                {
                    throw new KeyNotFoundException("The summonerId does not exist in the JSON.");
                }
                else
                {
                    return jToken.Value<int>();
                }
            }
        }
    }
}
