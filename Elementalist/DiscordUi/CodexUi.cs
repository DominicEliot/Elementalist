using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Elementalist.Models;
using Elementalist.Shared;
using ElementalistBot.Infrastructure.DataAccess.Rules;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Elementalist.DiscordUi.Rules;

public class CodexSlashCommand(IRulesRepository faqRepository) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IRulesRepository _faqRepository = faqRepository;

    [SlashCommand("codex", "Shows any Rules/Codex entries for the provided input.")]
    public async Task CodexSearchByTitle([SlashCommandParameter(AutocompleteProviderType = typeof(RulesAutoCompleteHandler))] string codexName, bool privateMessage = false)
    {
        var message = await CodexUiHelper.CreateCodexMessage(codexName, _faqRepository, privateMessage);

        await RespondAsync(InteractionCallback.Message(message));
    }
}

public static partial class CodexUiHelper
{
    internal static async Task<InteractionMessageProperties> CreateCodexMessage(string ruleToCreate, IRulesRepository faqRepository, bool privateMessage = false)
    {
        var message = new InteractionMessageProperties();
        if (privateMessage) message.Flags = NetCord.MessageFlags.Ephemeral;

        var codex = await faqRepository.GetRules();
        if (!codex.Any(r => r.Title.Contains(ruleToCreate, StringComparison.OrdinalIgnoreCase)))
        {
            message.Content = $"No Rules/Codex entries found for {ruleToCreate}";
            message.Flags = NetCord.MessageFlags.Ephemeral;
            return message;
        }

        var singleEntry = codex.SingleOrDefault(r => r.Title.Equals(ruleToCreate, StringComparison.OrdinalIgnoreCase));
        if (singleEntry is not null)
        {
            var codexEmbed = CreateCodexRuleEmbed(singleEntry);

            message.Embeds = [codexEmbed];
            message.WithComponents(CreateCodexComponents(singleEntry));
            return message;
        }

        var embeds = new List<EmbedProperties>();
        foreach (var rule in codex.Where(r => r.Title.Contains(ruleToCreate, StringComparison.OrdinalIgnoreCase)))
        {
            var codexEmbed = CreateCodexRuleEmbed(rule);

            embeds.Add(codexEmbed);
        }
        message.Embeds = embeds;
        return message;
    }

    private static EmbedProperties CreateCodexRuleEmbed(CodexEntry rule)
    {
        var codexEmbed = new EmbedProperties()
            .WithTitle($"{rule.Title} Codex/Rules")
            .WithDescription(rule.Content
                .Replace("[[", "**").Replace("]]", "**")
                .Replace("((", "_").Replace("))", "_"));

        foreach (var subCodex in rule.Subcodexes)
        {
            var continued = string.Empty;
            foreach (var fieldContentChunk in subCodex.Content.ChunkStringOnWords(1024)) //discord's max field length
            {
                codexEmbed.AddFields(new EmbedFieldProperties()
                    .WithName(subCodex.Title + continued)
                    .WithValue(new string(fieldContentChunk))
                );

                continued = " - continued";
            }
        }

        return codexEmbed;
    }

    private static List<IMessageComponentProperties> CreateCodexComponents(CodexEntry singleEntry)
    {
        var components = new List<IMessageComponentProperties>();
        var stringMenu = new StringMenuProperties("referenceSelect");

        foreach (Match cardMatch in CardMentionsRegex().Matches(singleEntry.Content).DistinctBy(m => m.Groups[1].Value))
        {
            stringMenu.Add(new StringMenuSelectOptionProperties(cardMatch.Groups[1].Value, $"card:{cardMatch.Groups[1].Value}"));
        }

        foreach (Match codexMatch in CodexMentionsRegex().Matches(singleEntry.Content).DistinctBy(m => m.Groups[1].Value))
        {
            stringMenu.Add(new StringMenuSelectOptionProperties(codexMatch.Groups[1].Value, $"codex:{codexMatch.Groups[1].Value}"));
        }

        if (stringMenu.Any())
        {
            components.Add(stringMenu);
        }

        return components;
    }

    [GeneratedRegex(@"\[\[(.*?)\]\]")]
    private static partial Regex CardMentionsRegex();

    [GeneratedRegex(@"\(\((.*?)\)\)")]
    private static partial Regex CodexMentionsRegex();
}
