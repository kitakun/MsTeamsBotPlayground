namespace Web.MsBotAttemp.Bot.Chats.Personal.Responses
{
    using System.Collections.Generic;

    using AdaptiveCards;

    using Microsoft.Bot.Schema;

    using Web.MsBotAttemp.Bot.Models.ActionsData;

    public static class PersonalWelcomeCard
    {
        public static Attachment GetCard(string welcomeText)
        {
            var userWelcomeCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                        Text = welcomeText,
                        Wrap = true,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = "todo TakeATourButtonText",
                        Data = new TeamsAdaptiveSubmitActionData
                        {
                            MsTeams = new CardAction
                            {
                              Type = ActionTypes.MessageBack,
                              DisplayText = "todo TakeATourButtonText",
                              Text = PersonalMessages.TakeATour,
                            },
                        },
                    },
                },
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = userWelcomeCard,
            };
        }
    }
}
