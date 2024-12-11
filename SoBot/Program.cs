using System.Reflection.Metadata.Ecma335;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Events;
using SorceryBot.Features.Card.Search;
using SorceryBot.Infrastructure.Config;
using SorceryBot.Infrastructure.Logging;

namespace SorceryBot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddJsonFile("BotToken.Private.json");
        builder.Services.Configure<BotTokenSettings>(builder.Configuration.GetRequiredSection("BotTokenSettings"));

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

        builder.Services.AddSerilog(Log.Logger);

        builder.Services.AddHostedService<BotStartupService>();

        var logLevel = (Log.Logger.IsEnabled(LogEventLevel.Debug)) ? LogSeverity.Debug : LogSeverity.Info;
        var clientConfig = new DiscordSocketConfig { MessageCacheSize = 5, LogLevel = logLevel, GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.GuildMessages | GatewayIntents.Guilds };
        builder.Services.AddSingleton(clientConfig);
        builder.Services.AddSingleton<DiscordSocketClient>();
        builder.Services.AddSingleton(services =>
        {
            var client = services.GetRequiredService<DiscordSocketClient>();
            var interactionServiceConfig = new InteractionServiceConfig() { UseCompiledLambda = true, LogLevel = LogSeverity.Info, AutoServiceScopes = true };
            return new InteractionService(client.Rest, interactionServiceConfig);
        });

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblyContaining<CardSearchQuery>();
            cfg.AddOpenBehavior(typeof(QueryLoggingPipeline<,>));
        });

        var host = builder.Build();

        host.MapDiscord();

        host.Run();
    }
}
