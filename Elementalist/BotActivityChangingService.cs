using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Gateway;

namespace Elementalist;

public class BotActivityChangingService : BackgroundService
{
    private readonly GatewayClient _client;
    private readonly IOptions<ActivityOptions> _options;
    private bool _connected = false;

    public BotActivityChangingService(GatewayClient client, IOptions<ActivityOptions> options)
    {
        _client = client;
        _options = options;
        _client.Connect += _client_Connect;
    }

    private ValueTask _client_Connect()
    {
        _connected = true;
        _client.Connect -= _client_Connect;
        _client.Disconnect += _client_Disconnect;
        return ValueTask.CompletedTask;
    }

    private ValueTask _client_Disconnect(bool arg)
    {
        _connected = false;
        _client.Connect += _client_Connect;
        _client.Disconnect -= _client_Disconnect;
        return ValueTask.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_connected)
            {
                await Task.Delay(1000, default);
            }

            var rand = Random.Shared.Next(0, _options.Value.Activities.Count());

            try
            {
                await _client.UpdatePresenceAsync(new PresenceProperties(UserStatusType.Online)
                {
                    Activities = [
                            new("The Elementalist", UserActivityType.Custom)
                            {
                                Name = "The Elementalist",
                                Details = "A bot for helping discussions about sorcery cards in Discord.",
                                State = _options.Value.Activities.ElementAt(rand),
                            }
                        ]
                },
                cancellationToken: stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(_options.Value.CycleTimeInMinutes), stoppingToken);
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
