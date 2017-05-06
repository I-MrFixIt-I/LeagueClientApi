using Newtonsoft.Json;
using System.Collections.Generic;

namespace LeagueClientApi.CollectionsEndpoint
{
    public class RunePage
    {
        /// <summary>
        /// Determines whether this runepage is currently set as activ in the runebook
        /// </summary>
        [JsonProperty("current")]
        public bool Current { get; set; }

        /// <summary>
        /// The Id of this runepage
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The name of this runepage
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The list of all selected runes of this page
        /// </summary>
        [JsonProperty("runes")]
        public List<Rune> Runes { get; set; }
    }
}
