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
        if (matchingRules.Count() == 1)
        {
            var singleEntry = matchingRules.First();
            var codexProperties = CreateCodexDiscordEntities(singleEntry, keywords);

            message.Embeds = [codexProperties.Item1];
            message.WithComponents(codexProperties.Item2.Take(25));
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
        var contentHighlighted = rule.Content;
        var regexString = @$"\b({string.Join("|", keywords.Where(word => word != rule.Title))})\b";
        var regex = new Regex(regexString, RegexOptions.IgnoreCase);

        var nextMatch = regex.Match(contentHighlighted); //todo: use matches with a control loop for performance?
        while (nextMatch.Success)
        {
            contentHighlighted = regex.Replace(contentHighlighted, "_$1_", 1);

            regexString = regexString.Replace(nextMatch.Groups[1].Value + "|", "");
            regex = new Regex(regexString, RegexOptions.IgnoreCase);
            nextMatch = regex.Match(contentHighlighted);
        }

        Serilog.Log.Debug("Generated highlighted content for {rule}: {content}", rule.Title, contentHighlighted);

        contentHighlighted = contentHighlighted
            .Replace("[[", "**").Replace("]]", "**")
            .Replace("((", "_").Replace("))", "_");

        return contentHighlighted;
    }

    private static List<IMessageComponentProperties> CreateCodexComponents(string content)
    {
        var components = new List<IMessageComponentProperties>();
        var stringMenu = new StringMenuProperties("referenceSelect");

        foreach (Match cardMatch in CardMentionsRegex().Matches(content).DistinctBy(m => m.Groups[2].Value))
        {
            stringMenu.Add(new StringMenuSelectOptionProperties(cardMatch.Groups[2].Value, $"card:{cardMatch.Groups[2].Value}"));
        }

        foreach (Match codexMatch in CodexMentionsRegex().Matches(content).DistinctBy(m => m.Groups[2].Value))
        {
            stringMenu.Add(new StringMenuSelectOptionProperties(codexMatch.Groups[2].Value, $"codex:{codexMatch.Groups[2].Value}"));
        }

        if (stringMenu.Any())
        {
            components.Add(stringMenu);
        }

        return components;
    }

    [GeneratedRegex(@"([[*]{2})([^()[\]*@$%^&_+={}|\/<>]*?)[\]*]{2}")]
    private static partial Regex CardMentionsRegex();

    [GeneratedRegex(@"(\(\(|_)([^()[\]*@$%^&_+={}|\/<>]*?)(\)\)|_)")]
    private static partial Regex CodexMentionsRegex();

}
