namespace Web.MsBotAttemp.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;

    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;

        public BotController(
            IBotFrameworkHttpAdapter adapter,
            IBot bot)
        {
            _adapter = adapter ?? throw new System.ArgumentNullException(nameof(adapter));
            _bot = bot ?? throw new System.ArgumentNullException(nameof(bot));
        }

        // Delegate request to bot
        [HttpPost]
        public Task PostAsync() =>
            _adapter.ProcessAsync(Request, Response, _bot);
    }
}
