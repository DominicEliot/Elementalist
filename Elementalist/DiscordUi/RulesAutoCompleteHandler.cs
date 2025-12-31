using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Infrastructure.DataAccess.Rules;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Elementalist.DiscordUi;

public class RulesAutoCompleteHandler(IRulesRepository rulesRepository) : IAutocompleteProvider<AutocompleteInteractionContext>
{
    private readonly IRulesRepository _rulesRepository = rulesRepository;

    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        var value = option.Value;

        if (string.IsNullOrWhiteSpace(value))
        {
            return [new("Airborne", "Airborne"), new("Fight", "Fight"), new("Disabled", "Disabled")];
        }

        var searchParams = value.Split(' ', '-', '&');

        var suggestions = (await _rulesRepository.GetRules()).Where(rule => searchParams.Any(sp => rule.Title.Contains(sp, StringComparison.OrdinalIgnoreCase)));
        return suggestions.Take(25).Select(c => new ApplicationCommandOptionChoiceProperties(c.Title, c.Title));
    }
}
