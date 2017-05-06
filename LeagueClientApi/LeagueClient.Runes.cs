using LeagueClientApi.CollectionsEndpoint;
using LeagueClientApi.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace LeagueClientApi
{
    public partial class LeagueClient
    {
        /// <summary>
        /// Retrieves the RuneBook of the currently logged in user account
        /// </summary>
        /// <returns>Returns the RuneBook</returns>
        /// <exception cref="LeagueClientRequestException">Occurs when the response StatusCode is not OK</exception>
        /// <exception cref="JsonReaderException">Occurs when the response contains an invalid JSON or can not be converted to a RuneBook object</exception>
        public RuneBook GetRuneBook()
        {
            HttpWebRequest request = CreateRequest(string.Format("lol-collections/v1/inventories/{0}/rune-book", GetSummonerId()), "GET");
            HttpWebResponse response = SubmitRequest(request, out string content);

            // Expected StatusCode is OK, so throw Exception if it differs
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new LeagueClientRequestException("An error occurred while fetching RuneBook", content, response.StatusCode);
            }
            else
            {
                return JsonConvert.DeserializeObject<RuneBook>(content);
            }
        }

        /// <summary>
        /// Updates the RuneBook for the currently logged in user account
        /// </summary>
        /// <exception cref="LeagueClientRequestException">Occurs when the response StatusCode is not Created</exception>
        public void SetRuneBook(RuneBook book)
        {
            HttpWebRequest request = CreateRequest(string.Format("lol-collections/v1/inventories/{0}/rune-book", GetSummonerId()), "PUT");
            HttpWebResponse response = SubmitRequest(request, JsonConvert.SerializeObject(book), out string content);

            // Expected StatusCode is Created, so throw Exception if it differs
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new LeagueClientRequestException("An error occurred while saving RuneBook", content, response.StatusCode);
            }
        }
    }
}
