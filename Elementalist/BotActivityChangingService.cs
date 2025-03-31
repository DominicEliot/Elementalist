using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Elementalist;

public class BotActivityChangingService(DiscordSocketClient client, IOptions<ActivityOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (client.LoginState != Discord.LoginState.LoggedIn)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            var rand = Random.Shared.Next(0, options.Value.Activities.Count());

            await client.SetCustomStatusAsync(options.Value.Activities.ElementAt(rand));

            await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
        }
    }
}

public class ActivityOptions
{
    public IEnumerable<string> Activities { get; set; } = [];
}
