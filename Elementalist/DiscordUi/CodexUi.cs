using System.Data;
using System.Text.RegularExpressions;
using Elementalist.Infrastructure.DataAccess.Rules;
using Elementalist.Models;
using Elementalist.Shared;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Elementalist.DiscordUi.Rules;

public class CodexSlashCommand(ICodexMessageService codexMessageService, PlainTextCodexMessageService textCodexMessageService)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("codex", "Shows any Rules/Codex entries for the provided input.")]
    public async Task CodexSearchByTitle(
        [SlashCommandParameter(AutocompleteProviderType = typeof(RulesAutoCompleteHandler))]
        string codexName,
        bool privateMessage = false)
    {
        var service = codexMessageService;

        if (Context.Channel is IInteractionChannel interactionChannel)
        {
            var allowedToPostEmbeds = (interactionChannel.Permissions & Permissions.EmbedLinks) != 0;
            if (!allowedToPostEmbeds)
            {
                service = textCodexMessageService;
            }
        }
        var message = await service.CreateCodexMessageAsync(codexName, CancellationToken.None);

        if (privateMessage)
        {
            message.Flags = MessageFlags.Ephemeral & message.Flags ?? 0;
        }

        await RespondAsync(InteractionCallback.Message(message));
    }
}

public class PlainTextCodexMessageService(IRulesRepository codexRepository) : ICodexMessageService
{
    public async Task<CodexDiscordMessage> CreateCodexMessageAsync(string codexName, CancellationToken cancellationToken)
    {
        var codex = (await codexRepository.GetRules(cancellationToken))
            .Where(c => c.Title.Equals(codexName, StringComparison.OrdinalIgnoreCase) ||
                        c.Subcodexes.Any(s => s.Title.Equals(codexName, StringComparison.OrdinalIgnoreCase)));

        if (codex.Count() == 1)
        {
            return await CreateCodexMessageAsync(codex.First(), cancellationToken);
        }

        if (!codex.Any())
        {
            return new CodexDiscordMessage()
            {
                Flags = MessageFlags.Ephemeral, Content = $"Couldn't find any codex entries with name '{codexName}'"
            };
        }

        return new CodexDiscordMessage()
        {
            Flags = MessageFlags.Ephemeral,
            Content = $"Too many results: {string.Join(", ", codex.Select(c => c.Title))}"
        };
    }

    public async Task<CodexDiscordMessage> CreateCodexMessageAsync(CodexEntry codex, CancellationToken cancellationToken)
    {
        var contentHighlighted = await CodexMessageService.GetDiscordDescription(codexRepository, codex, cancellationToken);

        var message = $"# {codex.Title}\n{contentHighlighted}\n{codex.Url}";

        if (message.Length > 2000)
        {
            var lengthToEnd = 1996 - codex.Url?.Length ?? 0;
            message = $"{message[..lengthToEnd]}...\n{codex.Url}";
        }

        return new CodexDiscordMessage()
        {
            Content =  message
        };
    }
}

public partial class CodexMessageService(IRulesRepository codexRepository) : ICodexMessageService
{
    public async Task<CodexDiscordMessage> CreateCodexMessageAsync(string codexName,
        CancellationToken cancellationToken)
    {
        var codex = (await codexRepository.GetRules(cancellationToken))
            .Where(c => c.Title.Equals(codexName, StringComparison.OrdinalIgnoreCase) ||
                        c.Subcodexes.Any(s => s.Title.Equals(codexName, StringComparison.OrdinalIgnoreCase)));

        if (codex.Count() == 1)
        {
            return await CreateCodexMessageAsync(codex.First(), cancellationToken);
        }

        if (!codex.Any())
        {
            return new CodexDiscordMessage()
            {
                Flags = MessageFlags.Ephemeral, Content = $"Couldn't find any codex entries with name '{codexName}'"
            };
        }

        return new CodexDiscordMessage()
        {
            Flags = MessageFlags.Ephemeral,
            Content = $"Too many results: {string.Join(", ", codex.Select(c => c.Title))}"
        };
    }

    public async Task<CodexDiscordMessage> CreateCodexMessageAsync(CodexEntry codex,
        CancellationToken cancellationToken)
    {
        var contentHighlighted = await GetDiscordDescription(codexRepository, codex, cancellationToken);

        if (contentHighlighted.Length > 2000)
        {
            contentHighlighted = contentHighlighted[..1996] + "...";
        }

        var embed = new CodexEmbed()
            .WithUrl(codex.Url)
            .WithTitle(codex.Title + " - Codex")
            .WithDescription(contentHighlighted);

        foreach (var subCodex in codex.Subcodexes)
        {
            var continued = string.Empty;
            var content = await GetDiscordDescription(codexRepository, subCodex, cancellationToken);

            foreach (var fieldContentChunk in content.ChunkStringOnWords(1024)) //1024 is discord's max field length
            {
                embed.AddFields(new EmbedFieldProperties()
                    .WithName(subCodex.Title + continued)
                    .WithValue(fieldContentChunk)
                );

                continued = " - continued";
            }
        }

        var component = await CreateCodexComponents(codex, cancellationToken);

        return new CodexDiscordMessage()
        {
            Embeds = [embed],
            Components = component is not null ? [component] : null,
        };
    }

    private static EmojiProperties cardEmoji = EmojiProperties.Standard("🃏");
    private static EmojiProperties codexEmoji = EmojiProperties.Standard("📖");

    private async Task<CodexSelectComponent?> CreateCodexComponents(CodexEntry codex,
        CancellationToken cancellationToken)
    {
        var stringMenu = new CodexSelectComponent();
        var content = await GetDiscordDescription(codexRepository, codex, cancellationToken);

        foreach (Match codexMatch in CodexMentionsRegex().Matches(content).DistinctBy(m => m.Groups[2].Value))
        {
            var ruleName = codexMatch.Groups[2].Value;
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                continue;
            }

            if (stringMenu.Count() >= 25)
            {
                break;
            }

            stringMenu.Add(new StringMenuSelectOptionProperties(ruleName, $"codex:{ruleName}").WithEmoji(codexEmoji));
        }

        foreach (Match cardMatch in CardMentionsRegex().Matches(content).DistinctBy(m => m.Groups[2].Value))
        {
            if (stringMenu.Count() >= 25)
            {
                break;
            }

            var cardName = cardMatch.Groups[2].Value;
            stringMenu.Add(new StringMenuSelectOptionProperties(cardName, $"card:{cardName}").WithEmoji(cardEmoji));
        }

        foreach (var subcodex in codex.Subcodexes)
        {
            var components = await CreateCodexComponents(subcodex, cancellationToken);
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

    [GeneratedRegex(@"([[_]{2})([^()[\]*@$%^&_+={}|\/<>]*?)[\]_]{2}")]
    private static partial Regex CardMentionsRegex();

    [GeneratedRegex(@"(\(\(|\*\*\*)([^()[\]*@$%^&_+={}|\/<>]*?)(\)\)|\*\*\*)")]
    private static partial Regex CodexMentionsRegex();

    public static async Task<string> GetDiscordDescription(IRulesRepository rulesRepository, CodexEntry rule,
        CancellationToken cancellationToken)
    {
        var keywords = await rulesRepository.GetKeywords(cancellationToken);
        var contentHighlighted = rule.Content;
        var wordsToMatch = keywords.Where(word => !word.Equals(rule.Title, StringComparison.CurrentCultureIgnoreCase)
                                                  && !word.Equals("you", StringComparison.CurrentCultureIgnoreCase));
        var regexString = @$"\b({string.Join("|", wordsToMatch)})\b";

        var regex = new Regex(regexString, RegexOptions.IgnoreCase);

        var regexMatches = regex.Matches(contentHighlighted);

        contentHighlighted = contentHighlighted
            .Replace("[[", "__").Replace("]]", "__")
            .Replace("((", "***").Replace("))", "***");

        foreach (var keyword in regexMatches.Select(r => r.Value).Distinct(StringComparer.OrdinalIgnoreCase).OrderByDescending(k => k.Length))
        {
            if (keyword.Equals("Codex", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var replacementRegex = new Regex(@$"(?<!\*)\b({keyword})\b(?![,.]?\*)", RegexOptions.IgnoreCase);
            contentHighlighted = replacementRegex.Replace(contentHighlighted, "***$1***", 1);
        }

        return contentHighlighted;
    }
}

public interface ICodexMessageService
{
    Task<CodexDiscordMessage> CreateCodexMessageAsync(string codexName, CancellationToken cancellationToken);
    Task<CodexDiscordMessage> CreateCodexMessageAsync(CodexEntry codex, CancellationToken cancellationToken);
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
