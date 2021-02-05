namespace Web.MsBotAttemp.Bot.Chats
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    using Web.MsBotAttemp.Bot.Models;

    /// <summary>Processor who can deal with 1:1, channels, etc</summary>
    public interface IChatProcessor<TChatType> where TChatType : BaseChatMessage
    {
        /// <summary>
        /// When we get new users
        /// </summary>
        Task OnMembersAddedToPersonalChatAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext);

        /// <summary>
        /// When user push some action to the server
        /// </summary>
        Task OnMessageActivityInPersonalChatAsync(
            IMessageActivity message,
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken);
    }
}
