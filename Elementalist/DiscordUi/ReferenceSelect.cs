using Elementalist.DiscordUi.Rules;
using Elementalist.Infrastructure.DataAccess.CardData;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class ReferenceSelect(ICodexMessageService codexService, ICardRepository cardRepository, CardDisplayService cardDisplayService) : ComponentInteractionModule<StringMenuInteractionContext>
{
    private readonly ICodexMessageService _codexService = codexService;
    private readonly ICardRepository _cardRepository = cardRepository;
    private readonly CardDisplayService _cardDisplayService = cardDisplayService;

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

        var cardDisplay = await _cardDisplayService.CardInfoMessage([card]);

        cardDisplay.Flags = (cardDisplay.Flags ?? 0) | MessageFlags.Ephemeral;

        await RespondAsync(InteractionCallback.Message(cardDisplay));
    }

    private async Task respondWithCodex(string codexName)
    {
        var codexMessage = await _codexService.CreateCodexMessageAsync(codexName);

        codexMessage.Flags = (codexMessage.Flags ?? 0) | MessageFlags.Ephemeral;

        await RespondAsync(InteractionCallback.Message(codexMessage));
    }
}
