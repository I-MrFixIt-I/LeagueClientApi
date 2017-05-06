using Newtonsoft.Json;

namespace LeagueClientApi.CollectionsEndpoint
{
    public class Rune
    {
        /// <summary>
        /// The Id of the rune
        /// </summary>
        [JsonProperty("runeId")]
        public int RuneId { get; set; }

        /// <summary>
        /// The slotId of the rune (1-30)
        /// </summary>
        [JsonProperty("runeSlotId")]
        public int RuneSlotId { get; set; }
    }
}
