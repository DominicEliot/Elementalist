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

            var status = options.Value.Activities.ElementAt(rand);

            try
            {
                await client.UpdatePresenceAsync(new PresenceProperties(NetCord.UserStatusType.Online)
                {
                    Activities = [new UserActivityProperties(status, UserActivityType.Custom)]
                }, cancellationToken: stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(options.Value.CycleTimeInMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}

public class ActivityOptions
{
    public int CycleTimeInMinutes { get; set; }
    public IEnumerable<string> Activities { get; set; } = [];
}
