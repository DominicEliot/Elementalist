using System.Text.Json;
using NetCord;
using NetCord.Gateway;

namespace Elementalist;

public class BotStartupService : BackgroundService
{
    private readonly GatewayClient _client;
    private readonly Serilog.ILogger _logger;

    // Unfortunately Discord.Net needs the IServiceProvider to make interaction service calls, instead using DI properly
    private readonly IServiceProvider _serviceProvider;

    public BotStartupService(GatewayClient client, Serilog.ILogger logger, IServiceProvider serviceProvider)
    {
        _client = client;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _client.InteractionCreate += _client_InteractionCreated;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            //_client.Log += LogAsync;
            //_client.InteractionCreate += _client_InteractionCreated;

            //await _client.LoginAsync(_options.Value.TokenType, _options.Value.Token);

            //await _client.StartAsync();
            //await _client.UpdatePresenceAsync(new("Opening some packs!"));
            //_logger.Information("Discord client started");

            //Block the thread so that the client stays logged in, at least until the user requests the service to restart/stop
            //await Task.Delay(-1, stoppingToken);
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            _logger.Fatal(ex, "A fatal exception occurred.");
        }
        finally
        {
            //await _client.StopAsync();
        }
    }

    private ValueTask LogAsync(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Error:
                _logger.Error(message.Exception, message.Message);
                break;

            case LogSeverity.Info:
                _logger.Information(message.Message);
                break;

            default:
                _logger.Warning("Unknown log message severity {0}:\nmessage:{}", message.Severity, message.Message);
                break;
        }

        return ValueTask.CompletedTask;
    }

    private ValueTask _client_InteractionCreated(Interaction arg)
    {
        _logger.Information("{user} is executing discord {type} id {Id}. Interaction {data}",
            arg.User.Username,
            arg.Context.GetType().Name,
            arg.Id,
            JsonSerializer.Serialize(arg.Data));

        return ValueTask.CompletedTask;
    }

    private ValueTask clientReady()
    {
        //try
        //{
        //    await _interactionService.RegisterCommandsGloballyAsync();

        //    _client.Ready -= clientReady;
        //}
        //catch (Exception ex)
        //{
        //    _logger.Error(ex, "An error occourred while registering commands");
        //}

        return ValueTask.CompletedTask;
    }
}
