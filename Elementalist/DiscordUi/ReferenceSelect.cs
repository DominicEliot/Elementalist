using System.Text.Json;
using Elementalist.DiscordUi.Rules;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Models;
using ElementalistBot.Infrastructure.DataAccess.Rules;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class ReferenceSelect(IRulesRepository rulesRepository, ICardRepository cardRepository, CardArtService cardArtService) : ComponentInteractionModule<StringMenuInteractionContext>
{
    private readonly IRulesRepository _rulesRepository = rulesRepository;
    private readonly ICardRepository _cardRepository = cardRepository;

    [ComponentInteraction("referenceSelect")]
    public async Task SelectReference()
    {
        if (Context.SelectedValues[0].StartsWith("card:"))
        {
            await respondWithCard(Context.SelectedValues[0].Replace("card:", ""));
            return;
        }

        if (Context.SelectedValues[0].StartsWith("codex:"))
        {
            await respondWithCodex(Context.SelectedValues[0].Replace("codex:", ""));
            return;
        }

        await RespondAsync(InteractionCallback.Message(new() { Content = $"Unknown interaction {Context.SelectedValues[0]}", Flags = MessageFlags.Ephemeral }));
    }

    private async Task respondWithCard(string cardName)
    {
        var card = (await _cardRepository.GetCardsMatching(c => c.Name == cardName)).FirstOrDefault();

        if (card is null)
        {
            await RespondAsync(InteractionCallback.Message(new() { Content = $"Unknown card {cardName}", Flags = MessageFlags.Ephemeral }));
            return;
        }

        var cardDisplay = CardDisplay.CardInfoMessage([card], cardArtService).WithFlags(MessageFlags.Ephemeral);
        await RespondAsync(InteractionCallback.Message(cardDisplay));
    }

    private async Task respondWithCodex(string codexName)
    {
        var codex = (await _rulesRepository.GetRules()).FirstOrDefault(c => c.Title == codexName || c.Subcodexes.Any(s => s.Title == codexName));

        if (codex is null)
        {
            await RespondAsync(InteractionCallback.Message(new() { Content = $"Unknown codex/rule {codexName}", Flags = MessageFlags.Ephemeral }));
            return;
        }

        var codexMessage = await CodexUiHelper.CreateCodexMessage(codexName, _rulesRepository, privateMessage: true);
        await RespondAsync(InteractionCallback.Message(codexMessage));
    }
}
