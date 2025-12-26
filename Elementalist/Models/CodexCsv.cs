using CsvHelper.Configuration.Attributes;

namespace Elementalist.Models;

public record CodexCsv
{
    [Name("title")]
    public required string title { get; init; }

    [Name("content")]
    public required string content { get; init; }

    [Name("subcodexes")]
    public string? subcodexes { get; init; }
}

public record CodexEntry
{
    [Name("title")]
    public required string Title { get; init; }

    [Name("content")]
    public required string Content { get; init; }

    public List<CodexEntry> Subcodexes { get; init; } = [];
}
