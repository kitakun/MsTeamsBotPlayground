namespace Web.MsBotAttemp.Bot
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;

    using Web.MsBotAttemp.Bot.Chats;
    using Web.MsBotAttemp.Bot.Models.ChatProcessorTypes;

    public class RootBootProcessor : TeamsActivityHandler
    {
        private readonly IChatProcessor<PersonalChatType> _personalProcessor;
        private readonly ILogger<RootBootProcessor> _logger;

        public RootBootProcessor(
            IChatProcessor<PersonalChatType> personalProcessor,
            ILogger<RootBootProcessor> logger
            )
        {
            _personalProcessor = personalProcessor ?? throw new ArgumentNullException(nameof(personalProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            try
            {
                TryApplyCulture(turnContext);

                return base.OnTurnAsync(turnContext, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error at ${nameof(OnTurnAsync)}()");
                return base.OnTurnAsync(turnContext, cancellationToken);
            }
        }

        protected override async Task OnConversationUpdateActivityAsync(
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            try
            {
                var activity = turnContext?.Activity;
                _logger.LogInformation("Received conversationUpdate activity");
                _logger.LogInformation($"conversationType: {activity.Conversation.ConversationType}, membersAdded: {activity.MembersAdded?.Count}, membersRemoved: {activity.MembersRemoved?.Count}");

                if (activity?.MembersAdded?.Count == 0)
                {
                    _logger.LogInformation("Ignoring conversationUpdate that was not a membersAdded event");
                    return;
                }

                switch (activity.Conversation?.ConversationType?.ToLower() ?? ConversationTypes.ConversationTypePersonal)
                {
                    case ConversationTypes.ConversationTypePersonal:
                        await _personalProcessor.OnMembersAddedToPersonalChatAsync(activity.MembersAdded, turnContext);
                        return;

                    default:
                        _logger.LogInformation($"Ignoring event from conversation type {activity.Conversation.ConversationType}");
                        return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing conversationUpdate: {ex.Message}");
                throw;
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                await SendTypingIndicatorAsync(turnContext);

                await Task.Delay(2000);

                switch (turnContext.Activity.Conversation.ConversationType?.ToLower() ?? ConversationTypes.ConversationTypePersonal)
                {
                    case ConversationTypes.ConversationTypePersonal:
                        await _personalProcessor.OnMessageActivityInPersonalChatAsync(
                            turnContext.Activity,
                            turnContext,
                            cancellationToken);
                        break;


                    default:
                        _logger.LogWarning($"Received unexpected conversationType {turnContext.Activity.Conversation.ConversationType}");
                        break;
                }

                await base.OnMessageActivityAsync(turnContext, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error at ${nameof(OnMessageActivityAsync)}()");
                await base.OnMessageActivityAsync(turnContext, cancellationToken);
            }
        }

        // todo to extensions
        private Task SendTypingIndicatorAsync(ITurnContext turnContext)
        {
            try
            {
                var typingActivity = turnContext.Activity.CreateReply();
                typingActivity.Type = ActivityTypes.Typing;
                return turnContext.SendActivityAsync(typingActivity);
            }
            catch (Exception ex)
            {
                // Do not fail on errors sending the typing indicator
                _logger.LogWarning(ex, "Failed to send a typing indicator");
                return Task.CompletedTask;
            }
        }

        private void TryApplyCulture(ITurnContext turnContext)
        {
            var recievedLocale = string.Empty;

            if (turnContext is TurnContext botContext
                && !string.IsNullOrEmpty(botContext.Locale))
            {
                recievedLocale = botContext.Locale;
            }
            else
            {
                // from ms repo
                recievedLocale = turnContext?
                    .Activity
                    .Entities?
                    .FirstOrDefault(entity => entity.Type == "clientInfo")?
                    .Properties["locale"]?
                    .ToString();
            }

            if (!string.IsNullOrEmpty(recievedLocale))
            {
                CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(recievedLocale);
            }
        }
    }
}
