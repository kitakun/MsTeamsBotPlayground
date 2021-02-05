namespace Web.MsBotAttemp.Bot.Chats.Personal.Responses
{
    using System.Collections.Generic;

    using Microsoft.Bot.Schema;

    public static class PersonalTakeTourCarousel
    {
        public static IEnumerable<Attachment> GetUserTourCards(string appBaseUri)
        {
            return new List<Attachment>()
            {
                GetCard("FunctionCardText1", "FunctionCardText2", $"{appBaseUri}/content/Askaquestion.png"),
                GetCard("AskAnExpertTitleText", "AskAnExpertText2", $"{appBaseUri}/content/Expertinquiry.png"),
                GetCard("ShareFeedbackTitleText", "FeedbackText1", $"{appBaseUri}/content/Sharefeedback.png"),
            };
        }

        private static Attachment GetCard(string title, string text, string imageUri)
        {
            var tourCarouselCard = new HeroCard()
            {
                Title = title,
                Text = text,
                Images = new List<CardImage>()
                {
                    new CardImage(imageUri),
                },
            };

            return tourCarouselCard.ToAttachment();
        }
    }
}
