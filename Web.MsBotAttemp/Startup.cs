namespace Web.MsBotAttemp
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Web.MsBotAttemp.Bot;
    using Web.MsBotAttemp.Bot.Chats;
    using Web.MsBotAttemp.Bot.Chats.Personal;
    using Web.MsBotAttemp.Bot.Models.ChatProcessorTypes;

    public class Startup
    {
        public IConfiguration Configuration { get; init; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
            services.AddTransient<IBot, RootBootProcessor>();
            // todo move to Web.MsBotAttemp.Bot namespace
            services.AddTransient<IChatProcessor<PersonalChatType>, PersonalChatProcessor>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
