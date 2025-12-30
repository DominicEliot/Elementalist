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
        if (privateMessage) message.Flags = MessageFlags.Ephemeral;

        var codex = await faqRepository.GetRules();
        var matchingRules = codex.Where(r => r.Title.Contains(ruleToCreate, StringComparison.OrdinalIgnoreCase) || r.Subcodexes.Any(s => s.Title.Contains(ruleToCreate, StringComparison.OrdinalIgnoreCase)));
        if (!matchingRules.Any())
        {
            message.Content = $"No Rules/Codex entries found for {ruleToCreate}";
            message.Flags = MessageFlags.Ephemeral;
            return message;
        }

        var keywords = await faqRepository.GetKeywords();
        var singleEntry = matchingRules.SingleOrDefault();
        if (singleEntry is not null)
        {
            var codexProperties = CreateCodexDiscordEntities(singleEntry, keywords);

            message.Embeds = [codexProperties.Item1];
            message.WithComponents(codexProperties.Item2);
            return message;
        }

        var embeds = new List<EmbedProperties>();
        foreach (var rule in matchingRules)
        {
            var codexEmbed = CreateCodexDiscordEntities(rule, keywords);

            embeds.Add(codexEmbed.Item1);
        }
        message.Embeds = embeds;
        return message;
    }

    private static Tuple<EmbedProperties, IEnumerable<IMessageComponentProperties>> CreateCodexDiscordEntities(CodexEntry rule, IEnumerable<string> keywords)
    {
        var contentHighlighted = GetDiscordDescription(rule, keywords);

        var codexEmbed = new EmbedProperties()
            .WithTitle($"{rule.Title} Codex/Rules")
            .WithDescription(contentHighlighted);

        var components = new List<IMessageComponentProperties>();
        components.AddRange(CreateCodexComponents(contentHighlighted));

        foreach (var subCodex in rule.Subcodexes)
        {
            var continued = string.Empty;
            var content = GetDiscordDescription(subCodex, keywords);

            components.AddRange(CreateCodexComponents(content));

            foreach (var fieldContentChunk in content.ChunkStringOnWords(1024)) //1024 is discord's max field length
            {
                codexEmbed.AddFields(new EmbedFieldProperties()
                    .WithName(subCodex.Title + continued)
                    .WithValue(fieldContentChunk)
                );

                continued = " - continued";
            }
        }

        return new(codexEmbed, components.OfType<StringMenuProperties>().DistinctBy(c => c.CustomId));
    }

    private static string GetDiscordDescription(CodexEntry rule, IEnumerable<string> keywords)
    {
        var regex = @$"\b{string.Join("|", keywords)}\b";
        var contentHighlighted = Regex.Replace(rule.Content, regex, "_$1_", RegexOptions.IgnoreCase);

        contentHighlighted = contentHighlighted
            .Replace("[[", "**").Replace("]]", "**")
            .Replace("((", "_").Replace("))", "_");

        return contentHighlighted;
    }

    private static List<IMessageComponentProperties> CreateCodexComponents(string content)
    {
        var components = new List<IMessageComponentProperties>();
        var stringMenu = new StringMenuProperties("referenceSelect");

        foreach (Match cardMatch in CardMentionsRegex().Matches(content).DistinctBy(m => m.Groups[1].Value))
        {
            stringMenu.Add(new StringMenuSelectOptionProperties(cardMatch.Groups[1].Value, $"card:{cardMatch.Groups[1].Value}"));
        }

        foreach (Match codexMatch in CodexMentionsRegex().Matches(content).DistinctBy(m => m.Groups[1].Value))
        {
            stringMenu.Add(new StringMenuSelectOptionProperties(codexMatch.Groups[1].Value, $"codex:{codexMatch.Groups[1].Value}"));
        }

        if (stringMenu.Any())
        {
            components.Add(stringMenu);
        }

        return components;
    }

    [GeneratedRegex(@"[[*]{2}(.*?)[\]*]{2}")]
    private static partial Regex CardMentionsRegex();

    [GeneratedRegex(@"(?<=\(\(|_)(.*?)(\)\)|_)")]
    private static partial Regex CodexMentionsRegex();
}
