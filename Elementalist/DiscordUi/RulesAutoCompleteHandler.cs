using Elementalist.Infrastructure.DataAccess.CardData;
using ElementalistBot.Infrastructure.DataAccess.Rules;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Elementalist.DiscordUi;

public class RulesAutoCompleteHandler(RulesRepository rulesRepository) : IAutocompleteProvider<AutocompleteInteractionContext>
{
    private readonly RulesRepository rulesRepository = rulesRepository;

    public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context)
    {
        var value = option.Value;

        if (string.IsNullOrWhiteSpace(value))
        {
            return [new("Airborne", "Airborne"), new("Immobile", "Immobile/Immobilised"), new("Disabled", "Disable/Disabled")];
        }

        var searchParams = value.Split(' ', '-', '&');

        var suggestions = (await rulesRepository.GetRules()).Where(rule => searchParams.Any(sp => rule.Name.Contains(sp, StringComparison.OrdinalIgnoreCase)));
        return suggestions.Take(25).Select(c => new ApplicationCommandOptionChoiceProperties(c.Name, c.Name));
    }
}
