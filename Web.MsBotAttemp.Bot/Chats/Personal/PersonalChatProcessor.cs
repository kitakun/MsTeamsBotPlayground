namespace Web.MsBotAttemp.Bot.Chats.Personal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json.Linq;

    using Web.MsBotAttemp.Bot.Chats.Personal.Responses;
    using Web.MsBotAttemp.Bot.Models.ActionsData;
    using Web.MsBotAttemp.Bot.Models.ChatProcessorTypes;

    public class PersonalChatProcessor : IChatProcessor<PersonalChatType>
    {
        private readonly ILogger<PersonalChatProcessor> _logger;
        private readonly string _appBaseUri;

        public PersonalChatProcessor(
            ILogger<PersonalChatProcessor> logger
            )
        {
            // todo get from config
            _appBaseUri = string.Empty;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// </inheritdoc>
        public Task OnMembersAddedToPersonalChatAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext)
        {
            var activity = turnContext.Activity;

            if (membersAdded.Any(channelAccount => channelAccount.Id == activity.Recipient.Id))
            {
                // User started chat with the bot in personal scope, for the first time.
                _logger.LogInformation($"Bot added to 1:1 chat {activity.Conversation.Id}");

                var welcomeText = "Welcome text!";

                var userWelcomeCardAttachment = PersonalWelcomeCard.GetCard(welcomeText);

                return turnContext.SendActivityAsync(MessageFactory.Attachment(userWelcomeCardAttachment));
            }

            return Task.CompletedTask;
        }

        /// </inheritdoc>
        public async Task OnMessageActivityInPersonalChatAsync(
            IMessageActivity message,
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            if (message.Value != null && message.Value is JObject jResponse && jResponse.HasValues)
            {
                _logger.LogInformation("Card submit in 1:1 chat");

                await OnAdaptiveCardSubmitInPersonalChatAsync(jResponse, turnContext, cancellationToken);
            }
            else
            {
                string rawTextInputFromUser = message.Text?.ToLower()?.Trim() ?? string.Empty;

                await ProceedTextAsync(rawTextInputFromUser, turnContext, cancellationToken);
            }
        }

        /// <summary>User user action button from bot interface</summary>
        private Task OnAdaptiveCardSubmitInPersonalChatAsync(
            JObject message,
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var adaptiveActionModel = message.ToObject<TeamsAdaptiveSubmitActionData>();
            var recievedText = adaptiveActionModel?.MsTeams?.Text;
            if (!string.IsNullOrEmpty(recievedText))
            {
                return ProceedTextAsync(recievedText, turnContext, cancellationToken);
            }
            else
            {
                // todo say something
                return Task.CompletedTask;
            }
        }

        private Task ProceedTextAsync(string text, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            switch (text)
            {
                case PersonalMessages.TakeATour:
                    _logger.LogInformation($"Call {nameof(PersonalTakeTourCarousel.GetUserTourCards)}");
                    var userTourCards = PersonalTakeTourCarousel.GetUserTourCards(_appBaseUri);
                    return turnContext.SendActivityAsync(MessageFactory.Carousel(userTourCards));

                // initial feedback request
                case PersonalMessages.ShareFeedback:
                    _logger.LogInformation("Sending user feedback card");
                    var feedbackCard = ShareFeedbackCard.GetCard();
                    return turnContext.SendActivityAsync(MessageFactory.Attachment(feedbackCard));

                //feedback with response
                case ShareFeedbackCard.ShareFeedbackSubmitText:
                    _logger.LogInformation("Received app feedback");
                    if (turnContext.Activity.Value is JObject jResp)
                    {
                        var feedbackModel = jResp.ToObject<ShareFeedbackCardPayload>();
                        // todo proceed feedback data
                        var saveFeedbackData = Task.CompletedTask;
                        var sendUserThankYouMessage = turnContext.SendActivityAsync(MessageFactory.Text("todo ThankYouTextContent"));

                        return Task.WhenAll(saveFeedbackData, sendUserThankYouMessage);
                    }
                    else
                    {
                        return turnContext.SendActivityAsync(MessageFactory.Text("todo somethingWentWrong"));
                    }

                default:
                    _logger.LogInformation($"Call echo test");
                    return turnContext.SendActivityAsync($"You said '{text}'");
            }
        }
    }
}
