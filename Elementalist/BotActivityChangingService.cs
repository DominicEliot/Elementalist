using Microsoft.Extensions.Options;
using NetCord.Gateway;

namespace Elementalist;

public class BotActivityChangingService(GatewayClient client, IOptions<ActivityOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var rand = Random.Shared.Next(0, options.Value.Activities.Count());

            //Todo - Set the status
            var status = options.Value.Activities.ElementAt(rand);

             await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
        }
    }
}

public class ActivityOptions
{
    public IEnumerable<string> Activities { get; set; } = [];
}
