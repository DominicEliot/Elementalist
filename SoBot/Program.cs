using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Serilog;
using SorceryBot.Features.Card.Search;
using SorceryBot.Infrastructure.Logging;

namespace SorceryBot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<BotListenerService>();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Services.AddSerilog(Log.Logger);

        builder.Services.AddSingleton<IDiscordClient, DiscordSocketClient>();

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblyContaining<CardSearchQuery>();
            cfg.AddOpenBehavior(typeof(QueryLoggingPipeline<,>));
        });

        var host = builder.Build();
        host.Run();
    }
}
