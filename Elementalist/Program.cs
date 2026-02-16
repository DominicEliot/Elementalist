using Elementalist.DiscordUi;
using Elementalist.DiscordUi.Rules;
using Elementalist.Features.Cards;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Infrastructure.DataAccess.Rules;
using Elementalist.Infrastructure.Logging;
using Microsoft.FeatureManagement;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;
using Serilog;
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
            builder.Services.AddFeatureManagement();

            builder.Services.Configure<TcgPlayerSettings>(builder.Configuration.GetRequiredSection("TcgPlayer"));
            builder.Services.Configure<ActivityOptions>(builder.Configuration.GetRequiredSection("ActivityOptions"));
            builder.Services.Configure<CardImageOptions>(builder.Configuration.GetRequiredSection("CardImageOptions"));
            builder.Services.Configure<DataRefreshOptions>(builder.Configuration.GetRequiredSection("DataRefreshOptions"));

            builder.Services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            Log.Logger.Information("Starting Elementalist version {v}", typeof(Program).Assembly.GetName().Version);

            builder.Services.AddHttpClient();
            builder.Services.AddMemoryCache();

            builder.Services.AddHostedService<CardDataService>();
            builder.Services.AddHostedService<CardPriceService>();
            builder.Services.AddHostedService<BotActivityChangingService>();
            builder.Services.AddHostedService<BotStartupService>();
            builder.Services.AddSingleton<ICardRepository, CuriosaApiCardRepository>();
            builder.Services.AddSingleton<TcgPlayerDataProvider>();
            builder.Services.AddSingleton<IFaqRepository, CsvFaqRepository>();
            builder.Services.AddSingleton<CardArtService>();
            builder.Services.AddSingleton<CardDisplayService>();
            builder.Services.AddSingleton<IRulesRepository, CodexMarkdownRulesRepository>();
            builder.Services.AddSingleton<PlainTextCodexMessageService>();
            builder.Services.AddSingleton<ICodexMessageService, CodexMessageService>();

            builder.Services
                .AddDiscordGateway(options =>
                {
                    options.Intents = GatewayIntents.AllNonPrivileged;
                    options.Presence = new PresenceProperties(UserStatusType.Online)
                    {
                        Activities = [
                            new("The Elementalist", UserActivityType.Playing)
                            {
                                Name = "Shuffling a spicy brew"
                            }
                        ],
                    };
                })
                .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
                .AddComponentInteractions<StringMenuInteraction, StringMenuInteractionContext>()
                //.AddComponentInteractions<ModalInteraction, ModalInteractionContext>()
                .AddApplicationCommands()
                .AddGatewayHandlers(typeof(Program).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<Program>();
                cfg.AddOpenBehavior(typeof(QueryLoggingPipeline<,>));
            });

            var host = builder.Build();

            host.AddModules(typeof(Program).Assembly);

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
