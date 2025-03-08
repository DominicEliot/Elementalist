using Discord;
using Discord.Interactions;
using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBot.DiscordUi;

public class CardAutoCompleteHandler(ICardRepository cardRepository) : AutocompleteHandler
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var value = autocompleteInteraction.Data.Current.Value as string;

        if (string.IsNullOrWhiteSpace(value))
        {
            return AutocompletionResult.FromSuccess();
        }

        var suggestions = await _cardRepository.GetCardsMatching(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
        return AutocompletionResult.FromSuccess(suggestions.Take(25).Select(c => new AutocompleteResult(c.Name, c.Name)));
    }
}
