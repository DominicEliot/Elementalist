using System.Text.Json;
using NetCord;
using NetCord.Gateway;

namespace Elementalist;

public class BotStartupService : BackgroundService
{
    private readonly GatewayClient _client;
    private readonly Serilog.ILogger _logger;

    public BotStartupService(GatewayClient client, Serilog.ILogger logger)
    {
        _client = client;
        _logger = logger;
        _client.InteractionCreate += _client_InteractionCreated;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private ValueTask _client_InteractionCreated(Interaction arg)
    {
        string data = arg.Data switch
        {
            ButtonInteractionData b => $"{b.ComponentType}:{b.CustomId}",
            StringMenuInteractionData stringMenu => $"{stringMenu.ComponentType}:{stringMenu.CustomId}",
            AutocompleteInteractionData autoCompleteData => $"{autoCompleteData.Type}:{autoCompleteData.Name}",
            SlashCommandInteractionData slashCommandData => $"{slashCommandData.Type}:{slashCommandData.Name} {JsonSerializer.Serialize(slashCommandData.Options)}",
            ModalInteractionData modalData => $"{nameof(ModalInteraction)}:{modalData.CustomId}",
            _ => $"unknown interaction type {arg.Data.GetType().Name}"
        };

        _logger.Information("{user} is executing discord {type} id {Id}. Interaction {data}",
            arg.User.Username,
            arg.Context.GetType().Name,
            arg.Id,
            data);

        return ValueTask.CompletedTask;
    }
}
