using Elementalist.Infrastructure.DataAccess.CardData;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Elementalist.DiscordUi;

public class CardAutoCompleteHandler(ICardRepository cardRepository) : IAutocompleteProvider<AutocompleteInteractionContext>
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        var value = option.Value;

        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        var searchParams = value.Split(' ', '-', '&');

        var suggestions = (await _cardRepository.GetCardsMatching(c => c.Name.StartsWith(value.Trim(), StringComparison.OrdinalIgnoreCase))).ToList();
        suggestions.AddRange(await _cardRepository.GetCardsMatching(c => c.Name.Contains(value.Trim(), StringComparison.OrdinalIgnoreCase)));

        suggestions.AddRange(await _cardRepository.GetCardsMatching(c => searchParams.Any(sp => sp.Trim().Length > 2 && c.Name.Contains(sp, StringComparison.OrdinalIgnoreCase))));
        return suggestions.Distinct().Take(25).Select(c => new ApplicationCommandOptionChoiceProperties(c.Name, c.Name));
    }
}
