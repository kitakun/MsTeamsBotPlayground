namespace Web.MsBotAttemp.Bot.Chats.Personal.Responses
{
    using System;
    using System.Collections.Generic;

    using AdaptiveCards;

    using Microsoft.Bot.Schema;

    using Web.MsBotAttemp.Bot.Models;
    using Web.MsBotAttemp.Bot.Models.ActionsData;

    public static class ShareFeedbackCard
    {
        public const string ShareFeedbackSubmitText = "ShareFeedback";

        public static Attachment GetCard() =>
            GetCard(new ShareFeedbackCardPayload(), showValidationErrors: false);

        public static Attachment GetCard(ShareFeedbackCardPayload payload) =>
            payload != null
                ? GetCard(payload, showValidationErrors: true)
                : null;

        private static Attachment GetCard(ShareFeedbackCardPayload data, bool showValidationErrors)
        {
            var shareFeedbackCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Text = !string.IsNullOrWhiteSpace(data.UserQuestion)
                            ? "todo ResultsFeedbackText"
                            : "todo ShareFeedbackTitleText",
                        Size = AdaptiveTextSize.Large,
                        Wrap = true,
                    },
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>
                        {
                            new AdaptiveColumn
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = !string.IsNullOrWhiteSpace(data.UserQuestion)
                                            ? "todo FeedbackRatingRequired"
                                            : "todo ShareAppFeedbackRating",
                                        Wrap = true,
                                    },
                                },
                            },
                            new AdaptiveColumn
                            {
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock
                                    {
                                        Text = (showValidationErrors && !Enum.TryParse(data.Rating, out FeedbackRating rating))
                                            ? "todo RatingMandatoryText"
                                            : string.Empty,
                                        Color = AdaptiveTextColor.Attention,
                                        HorizontalAlignment = AdaptiveHorizontalAlignment.Right,
                                        Wrap = true,
                                    },
                                },
                            },
                        },
                    },
                    new AdaptiveChoiceSetInput
                    {
                        Id = nameof(ShareFeedbackCardPayload.Rating),
                        IsMultiSelect = false,
                        Style = AdaptiveChoiceInputStyle.Compact,
                        Choices = new List<AdaptiveChoice>
                        {
                            new AdaptiveChoice
                            {
                                Title = "todo HelpfulRatingText",
                                Value = nameof(FeedbackRating.Helpful),
                            },
                            new AdaptiveChoice
                            {
                                Title = "todo NeedsImprovementRatingText",
                                Value = nameof(FeedbackRating.NeedsImprovement),
                            },
                            new AdaptiveChoice
                            {
                                Title = "todo NotHelpfulRatingText",
                                Value = nameof(FeedbackRating.NotHelpful),
                            },
                        },
                    },
                    new AdaptiveTextBlock
                    {
                        Text = "todo DescriptionText",
                        Wrap = true,
                    },
                    new AdaptiveTextInput
                    {
                        Spacing = AdaptiveSpacing.Small,
                        Id = nameof(ShareFeedbackCardPayload.Description),
                        Placeholder = !string.IsNullOrWhiteSpace(data.UserQuestion)
                            ? "todo FeedbackDescriptionPlaceholderText"
                            : "todo AppFeedbackDescriptionPlaceholderText",
                        IsMultiline = true,
                        Value = data.Description,
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = "todo ShareFeedbackButtonText",
                        Data = new ShareFeedbackCardPayload
                        {
                            MsTeams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                DisplayText = "todo ShareFeedbackDisplayText",
                                Text = ShareFeedbackSubmitText,
                            },
                            UserQuestion = data.UserQuestion,
                            KnowledgeBaseAnswer = data.KnowledgeBaseAnswer,
                        },
                    },
                },
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = shareFeedbackCard,
            };
        }
    }
}
