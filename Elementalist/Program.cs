using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Elementalist.Features.Cards;
using Elementalist.Infrastructure.Config;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Infrastructure.Logging;
using Serilog;
using Serilog.Events;
using static Elementalist.Features.Cards.Prices;

namespace Elementalist;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

        try
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.Configure<BotTokenSettings>(builder.Configuration);
            builder.Services.Configure<TcgPlayerSettings>(builder.Configuration.GetRequiredSection("TcgPlayer"));
            builder.Services.Configure<ActivityOptions>(builder.Configuration.GetRequiredSection("ActivityOptions"));

            builder.Services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.Services.AddHttpClient();
            builder.Services.AddMemoryCache();

            builder.Services.AddHostedService<BotStartupService>();
            builder.Services.AddHostedService<CardPriceService>();
            builder.Services.AddHostedService<BotActivityChangingService>();
            builder.Services.AddSingleton<ICardRepository, FileCardRepository>();
            builder.Services.AddSingleton<TcgPlayerDataProvider>();

            var logLevel = (Log.Logger.IsEnabled(LogEventLevel.Debug)) ? LogSeverity.Debug : LogSeverity.Info;
            var clientConfig = new DiscordSocketConfig { MessageCacheSize = 5, LogLevel = logLevel, GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.GuildMessages | GatewayIntents.Guilds };
            builder.Services.AddSingleton(clientConfig);
            builder.Services.AddSingleton<DiscordSocketClient>();
            builder.Services.AddSingleton(new InteractionServiceConfig() { LogLevel = LogSeverity.Info, AutoServiceScopes = true });
            builder.Services.AddSingleton<FaqRepoistory>();

            builder.Services.AddSingleton(services =>
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                var interactionServiceConfig = new InteractionServiceConfig() { UseCompiledLambda = true, LogLevel = LogSeverity.Info, AutoServiceScopes = true };
                return new InteractionService(client, interactionServiceConfig);
            });

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<Program>();
                cfg.AddOpenBehavior(typeof(QueryLoggingPipeline<,>));
            });

            var host = builder.Build();

            host.MapDiscord();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "An unhandled error occurred.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
