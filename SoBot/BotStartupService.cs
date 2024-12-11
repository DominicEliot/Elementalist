using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Serilog;
using SorceryBot.Infrastructure.Config;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SorceryBot;

public class BotStartupService : BackgroundService
{
    private readonly DiscordSocketClient _client;
    private readonly IOptions<BotTokenSettings> _options;
    private readonly InteractionService _interactionService;
    private readonly Serilog.ILogger _logger;

    // Unforunatley Discord.Net needs the IServiceProvider to make interaction service calls, instead using DI properly
    private readonly IServiceProvider _serviceProvider;

    public BotStartupService(DiscordSocketClient client, IOptions<BotTokenSettings> options, InteractionService interactionService, Serilog.ILogger logger, IServiceProvider serviceProvider)
    {
        _client = client;
        _options = options;
        _interactionService = interactionService;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _client.Log += LogAsync;
            _interactionService.Log += LogAsync;
            _client.Ready += clientReady;
            _client.InteractionCreated += interactionCreated;
            _client.SlashCommandExecuted += slashCommandExecuted;

            await _client.LoginAsync(_options.Value.TokenType, _options.Value.Token);

            await _client.StartAsync();
            _logger.Information("Discord client started");


            //Block the thread so that the client stays logged in, at least until the user requests the service to restart/stop
            await Task.Delay(-1, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex, "A fatal exception occourred.");
        }
        finally
        {
            await _client.StopAsync();
        }
    }

    private async Task interactionCreated(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(_client, interaction);
        await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
    }

    private async Task LogAsync(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                _logger.Fatal(message.Exception, message.ToString());
                break;
            case LogSeverity.Error:
                _logger.Error(message.Exception, message.ToString());
                break;
            case LogSeverity.Warning:
                _logger.Warning(message.ToString());
                break;
            case LogSeverity.Info:
                _logger.Information(message.ToString());
                break;
            case LogSeverity.Verbose:
                _logger.Debug(message.ToString());
                break;
            case LogSeverity.Debug:
                _logger.Debug(message.ToString());
                break;
            default:
                break;
        }
    }

    private async Task slashCommandExecuted(SocketSlashCommand arg)
    {
        var ctx = new SocketInteractionContext<SocketSlashCommand>(_client, arg);
        try
        {
            var withSubOptions = arg.Data.Options?.SelectMany(o => o.Options).Select(o => o.Value.ToString());
            var options = withSubOptions?.Count() > 0 ? withSubOptions : arg.Data.Options?.Select(o => o.Value?.ToString()) ?? new List<string>();
            _logger.Information($"{arg.User.Username} is executing Slash Command {arg.Data.Name} with value(s): '{string.Join(", ", options)}'.");
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Slash command {CommandName} failed", arg.CommandName);

            if (!ctx.Interaction.HasResponded)
            {
                await ctx.Interaction.RespondAsync($"An error occoured: {ex.Message}", ephemeral: true);
            }
        }
    }

    private async Task clientReady()
    {
        try
        {
            //await _interactionService.RegisterCommandsGloballyAsync();

            _client.Ready -= clientReady;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occourred while registering commands");
        }
    }
}
