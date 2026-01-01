using System.Data;
using System.Text.RegularExpressions;
using Elementalist.Infrastructure.DataAccess.Rules;
using Elementalist.Models;
using Elementalist.Shared;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Elementalist.DiscordUi.Rules;

public class CodexSlashCommand(ICodexMessageService codexMessageService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("codex", "Shows any Rules/Codex entries for the provided input.")]
    public async Task CodexSearchByTitle([SlashCommandParameter(AutocompleteProviderType = typeof(RulesAutoCompleteHandler))] string codexName, bool privateMessage = false)
    {
        var message = await codexMessageService.CreateCodexMessageAsync(codexName);

        if (privateMessage)
        {
            message.Flags = MessageFlags.Ephemeral & message.Flags ?? 0;
        }

        await RespondAsync(InteractionCallback.Message(message));
    }
}

public partial class CodexMessageService(IRulesRepository codexRepository) : ICodexMessageService
{
    public async Task<CodexDiscordMessage> CreateCodexMessageAsync(string codexName)
    {
        var codex = (await codexRepository.GetRules())
            .Where(c => c.Title.Equals(codexName, StringComparison.OrdinalIgnoreCase) || c.Subcodexes.Any(s => s.Title.Equals(codexName, StringComparison.OrdinalIgnoreCase)));

        if (codex.Count() == 1)
        {
            return await CreateCodexMessageAsync(codex.First());
        }

        if (!codex.Any())
        {
            return new CodexDiscordMessage()
            {
                Flags = MessageFlags.Ephemeral,
                Content = $"Couldn't find any codex entries with name '{codexName}'"
            };
        }

        return new CodexDiscordMessage()
        {
            Flags = MessageFlags.Ephemeral,
            Content = $"Too many results: {string.Join(", ", codex.Select(c => c.Title))}"
        };
    }

    public async Task<CodexDiscordMessage> CreateCodexMessageAsync(CodexEntry codex)
    {
        var keywords = await codexRepository.GetKeywords();

        var contentHighlighted = await GetDiscordDescription(codex);

        var embed = new CodexEmbed()
            .WithTitle($"{codex.Title} Codex/Rules")
            .WithDescription(contentHighlighted);

        foreach (var subCodex in codex.Subcodexes)
        {
            var continued = string.Empty;
            var content = await GetDiscordDescription(subCodex);

            foreach (var fieldContentChunk in content.ChunkStringOnWords(1024)) //1024 is discord's max field length
            {
                embed.AddFields(new EmbedFieldProperties()
                    .WithName(subCodex.Title + continued)
                    .WithValue(fieldContentChunk)
                );

                continued = " - continued";
            }
        }

        var component = await CreateCodexComponents(codex);

        return new CodexDiscordMessage()
        {
            Embeds = [embed],
            Components = component is not null ? [component] : null,
        };
    }

    private static EmojiProperties cardEmoji = EmojiProperties.Standard("🃏");
    private static EmojiProperties codexEmoji = EmojiProperties.Standard("📖");

    private async Task<CodexSelectComponent?> CreateCodexComponents(CodexEntry codex)
    {
        var stringMenu = new CodexSelectComponent();
        var content = await GetDiscordDescription(codex);

        foreach (Match cardMatch in CardMentionsRegex().Matches(content).DistinctBy(m => m.Groups[2].Value))
        {
            var cardName = cardMatch.Groups[2].Value;
            stringMenu.Add(new StringMenuSelectOptionProperties(cardName, $"card:{cardName}").WithEmoji(cardEmoji));
        }

        foreach (Match codexMatch in CodexMentionsRegex().Matches(content).DistinctBy(m => m.Groups[2].Value))
        {
            var ruleName = codexMatch.Groups[2].Value;
            if (string.IsNullOrEmpty(ruleName))
            {
                continue;
            }
            if (stringMenu.Count() >= 25)
            {
                break;
            }
            stringMenu.Add(new StringMenuSelectOptionProperties(ruleName, $"codex:{ruleName}").WithEmoji(codexEmoji));
        }

        foreach (var subcodex in codex.Subcodexes)
        {
            var components = await CreateCodexComponents(subcodex);
            if (components == null)
            {
                continue;
            }

            foreach (var component in components.Where(c => !stringMenu.Any(m => m.Label == c.Label)))
            {
                if (stringMenu.Count() >= 25)
                {
                    break;
                }
                stringMenu.Add(component);
            }
        }

        if (stringMenu.Any())
        {
            return stringMenu;
        }

        return null;
    }

    [GeneratedRegex(@"([[*]{2})([^()[\]*@$%^&_+={}|\/<>]*?)[\]*]{2}")]
    private static partial Regex CardMentionsRegex();

    [GeneratedRegex(@"(\(\(|_)([^()[\]*@$%^&_+={}|\/<>]*?)(\)\)|_)")]
    private static partial Regex CodexMentionsRegex();

    private async Task<string> GetDiscordDescription(CodexEntry rule)
    {
        var keywords = await codexRepository.GetKeywords();
        var contentHighlighted = rule.Content;
        var regexString = @$"\b({string.Join("|", keywords.Where(word => word != rule.Title))})\b";
        var regex = new Regex(regexString, RegexOptions.IgnoreCase);

        var regexMatches = regex.Matches(contentHighlighted);

        foreach(var keyword in regexMatches.Select(r => r.Value).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var replacementRegex = new Regex(@$"\b({keyword})\b", RegexOptions.IgnoreCase);
            contentHighlighted = replacementRegex.Replace(contentHighlighted, "_$1_", 1);
        }

        contentHighlighted = contentHighlighted
            .Replace("[[", "**").Replace("]]", "**")
            .Replace("((", "_").Replace("))", "_");

        return contentHighlighted;
    }
}

public interface ICodexMessageService
{
    Task<CodexDiscordMessage> CreateCodexMessageAsync(string codexName);
    Task<CodexDiscordMessage> CreateCodexMessageAsync(CodexEntry codex);
}

public class CodexDiscordMessage : InteractionMessageProperties
{

}

public class CodexEmbed : EmbedProperties
{

}

public class CodexSelectComponent : StringMenuProperties
{
    public CodexSelectComponent() : base("referenceSelect")
    {
        
    }
}
