using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Elementalist.Models;

namespace Elementalist.Infrastructure.DataAccess.Rules;

internal partial class CodexMarkdownRulesRepository : IRulesRepository
{
    public Task<IEnumerable<string>> GetKeywords()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CodexEntry>> GetRules()
    {
        var markDown = await File.ReadAllTextAsync("Full Codex.md");
        var markdownDirectory = new DirectoryInfo("");
        foreach(var file in markdownDirectory.EnumerateFiles(""))
        {

        }

        var splits = LineStartsWithH1Regex().Split(markDown);

        var codex = new List<CodexEntry>();
        foreach (var singleMarkdown in splits)
        {
            if (string.IsNullOrWhiteSpace(singleMarkdown))
            {
                continue;
            }

            var title = GroupedCodexContentRegex().Match(singleMarkdown);
            var entry = new CodexEntry { Content = singleMarkdown, Title = };
        }
    }

    [GeneratedRegex(@"^(?=# )")]
    private static partial Regex LineStartsWithH1Regex();
    [GeneratedRegex(@"^# (.*)\n(.*)")]
    private static partial Regex GroupedCodexContentRegex();
}
