using Newtonsoft.Json;
using System.Collections.Generic;

namespace LeagueClientApi.CollectionsEndpoint
{
    public class RuneBook
    {
        /// <summary>
        /// The list of all associated runepages of this runebook
        /// </summary>
        [JsonProperty("pages")]
        public List<RunePage> Pages { get; set; }

        /// <summary>
        /// The SummonerId of the owner from this runebook
        /// </summary>
        [JsonProperty("summonerId")]
        public int SummonerId { get; set; }
    }
}
