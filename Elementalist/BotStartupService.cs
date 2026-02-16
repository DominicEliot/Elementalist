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
            ButtonInteractionData b => $"button => {b.ComponentType}:{b.CustomId}",
            StringMenuInteractionData stringMenu => $"stringMenu => {stringMenu.ComponentType}:{stringMenu.CustomId} => {string.Join(",", stringMenu.SelectedValues)}",
            AutocompleteInteractionData autoCompleteData => $"autoComplete => {autoCompleteData.Type}:{autoCompleteData.Name}",
            SlashCommandInteractionData slashCommandData => $"slashCommand => {slashCommandData.Type}:{slashCommandData.Name} {JsonSerializer.Serialize(slashCommandData.Options)}",
            ModalInteractionData modalData => $"modalInteraction => {nameof(ModalInteraction)}:{modalData.CustomId}",
            MessageCommandInteractionData messageData => $"messageCommand => {messageData.Type}:{messageData.Name}",
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
