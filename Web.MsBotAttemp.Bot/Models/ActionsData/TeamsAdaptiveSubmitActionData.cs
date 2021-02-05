namespace Web.MsBotAttemp.Bot.Models.ActionsData
{
    using Microsoft.Bot.Schema;

    using Newtonsoft.Json;

    public class TeamsAdaptiveSubmitActionData
    {
        /// <summary>
        /// Gets or sets the Teams-specific action.
        /// </summary>
        [JsonProperty("msteams")]
        public CardAction MsTeams { get; set; }
    }
}
