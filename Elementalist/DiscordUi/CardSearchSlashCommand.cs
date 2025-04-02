using System.Runtime.CompilerServices;
using Discord;
using Discord.Interactions;
using Elementalist.Features.Card;
using Elementalist.Infrastructure.Config;
using Elementalist.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace Elementalist.DiscordUi;

public static class CardDisplay
{
    internal static ComponentBuilder CardComponentBuilder(Card card)
    {
        var builder = new ComponentBuilder();

        if (card.Sets.Count() > 1 || card.Sets.Any(s => s.Variants.Count() > 1))
        {
            AddVariantsMenu(card, builder);
        }

        builder.WithButton("Art", $"art-{card.Name}");
        builder.WithButton("Faq", $"faq-{card.Name}");
        builder.WithButton("Price", $"price-{card.Name}");

        return builder;
    }

    private static void AddVariantsMenu(Card card, ComponentBuilder builder)
    {
        var menuBuilder = new SelectMenuBuilder();
        menuBuilder.WithCustomId($"variantSelect");

        var defaultVariant = CardLookups.GetDefaultVariant(card);

        foreach (var set in card.Sets)
        {
            foreach (var variant in set.Variants)
            {
                var isDefault = (defaultVariant.Variant == variant && defaultVariant.Set == set);
                var uniqueCardId = new UniqueCardIdentifier(card.Name, set.Name, variant.Product, variant.Finish);
                menuBuilder.AddOption(uniqueCardId.ToNamelessString(), uniqueCardId.ToJson(), isDefault: isDefault);
            }
        }

        builder.WithSelectMenu(menuBuilder);
    }
}

public class CardSearchSlashCommand(IMediator mediator, IOptions<BotConfig> config) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly BotConfig _config = config.Value;
    private readonly IMediator _mediator = mediator;

    [SlashCommand("search-by-name", "Searches for and returns any matching sorcery cards")]
    public async Task CardSearchByName([Autocomplete<CardAutoCompleteHandler>()] string cardName, bool ephemeral = false)
    {
        var query = new GetCardsQuery() { CardNameContains = cardName };
        var cards = await _mediator.Send(query);

        await SendDiscordResponse(cardName, cards, ephemeral, query);
    }

    [SlashCommand("search-by-text", "Searches for and returns any matching sorcery cards")]
    public async Task CardSearchByRulesText(string? cardText = null, string? element = null, string? cardTypes = null, bool ephemeral = false)
    {
        var query = new GetCardsQuery()
        {
            TextContains = cardText,
            ElementsContain = element,
            TypeContains = cardTypes,
        };
        var cards = await _mediator.Send(query);

        await SendDiscordResponse(cardText, cards, ephemeral: ephemeral, query);
    }

    private async Task SendDiscordResponse(string? cardNameOrText, IEnumerable<Card> cards, bool ephemeral, GetCardsQuery query)
    {
        if (!cards.Any())
        {
            await RespondAsync($"Couldn't find match for '{cardNameOrText ?? query.ToString()}'", ephemeral: true);
            return;
        }

        if (cards.Count() > _config.MaxCardEmbedsPerMessage)
        {
            string responseText = $"Too many matches to display your search results,\nplease see {GetRealmsAppUrl(query)}";

            await base.RespondAsync(responseText, ephemeral: ephemeral);
            return;
        }

        var embeds = new List<Embed>();
        foreach (var card in cards)
        {
            embeds.Add(new EmbedCardDetailAdapter(card).Build());
        }

        MessageComponent? components = null;
        if (cards.Count() == 1)
        {
            components = CardDisplay.CardComponentBuilder(cards.First()).Build();
        }

        var searchParamters = (query is not null) ? $"Search Criteria: " : string.Empty;
        if (query?.CardNameContains is not null) searchParamters += $"Name: {query.CardNameContains} ";
        if (query?.TextContains is not null) searchParamters += $"Card Text: {query.TextContains} ";
        if (query?.ElementsContain is not null) searchParamters += $"Element: {query.ElementsContain}";

        await RespondAsync(searchParamters, embeds: embeds.ToArray(), components: components, ephemeral: ephemeral);
    }

    //Todo: this should maybe be its own service
    private static string[] hiddenCardTypes = ["Minion", "Artifact"];

    public static Uri GetRealmsAppUrl(GetCardsQuery query)
    {
        if (query is null) throw new NullReferenceException($"{nameof(query)} cannot be null");

        List<string> queryParams = query.TextContains?.Replace(",", "").Split(' ').Select(t => $"x:{t}").ToList() ?? [];
        queryParams.AddRange(query.CardNameContains?.Replace(",", "").Split(' ').Select(n => $"n:{n}") ?? []);
        queryParams.AddRange(query.ElementsContain?.Replace(",", "").Split(' ').Select(e => $"e:{e}") ?? []);

        foreach (var cardType in query.TypeContains?.Replace(",", "").Split(' ') ?? [])
        {
            var cardTypeSyntaxOption = hiddenCardTypes.Contains(cardType, StringComparer.OrdinalIgnoreCase) ? "t:" : "l:";
            queryParams.Add($"{cardTypeSyntaxOption}{cardType}");
        }

        var uri = new UriBuilder($"https://www.realmsapp.com/sorcery_tcg/cards");
        uri.Query = $"?query={string.Join("+", queryParams)}";
        return uri.Uri;
    }
}

internal class EmbedCardDetailAdapter : EmbedBuilder
{
    public EmbedCardDetailAdapter(Card card, SetVariant? setVariant = null)
    {
        setVariant ??= CardLookups.GetDefaultVariant(card);

        var cardCostSymbols = DiscordHelpers.GetManaEmojis(card);
        var thresholdSymbols = DiscordHelpers.GetThresholdEmojis(card.Guardian.Thresholds);

        WithTitle($"{card.Name} {cardCostSymbols} {thresholdSymbols}");
        WithUrl($"https://curiosa.io/cards/{card.Name.ToLower().Replace(' ', '_')}");
        WithColor(DiscordHelpers.GetCardColor(card.Elements));
        WithThumbnailUrl(CardArt.GetUrl(setVariant));
        WithDescription(setVariant.Variant.TypeText);

        var powerText = (card.Guardian.Attack > 0) ? $"Attack: {card.Guardian.Attack} " : string.Empty;
        var defenseText = (card.Guardian.Defence > 0 && card.Guardian.Defence != card.Guardian.Attack) ? $"Defence: {card.Guardian.Defence} " : string.Empty;
        var rulesTextField = $"{powerText}{defenseText}\n{DiscordHelpers.ReplaceManaTokensWithEmojis(card.Guardian.RulesText)}".Trim();

        var subtypeText = (!string.IsNullOrEmpty(card.SubTypes)) ? $" - {card.SubTypes}" : string.Empty;

        AddField(card.Guardian.Type + subtypeText, rulesTextField);
    }
}

public class SetVariant
{
    public required Set Set { get; init; }
    public required Variant Variant { get; init; }
}

public static class CardArt
{
    public static string GetUrl(SetVariant setVariant)
    {
        return GetUrl(setVariant.Variant.Slug, setVariant.Set.Name);
    }

    public static string GetUrl(string cardSlug, string setName)
    {
        cardSlug = cardSlug.Substring(4); //slugs are in the format set_image-slug, for now...
        var escapedSet = Uri.EscapeDataString(setName);
        return $"https://content.wrexial.com/SorceryImages/{escapedSet}/{cardSlug}.png";
    }
}

internal static class CardLookups
{
    internal static SetVariant GetDefaultVariant(Models.Card card)
    {
        var sets = card.Sets.OrderByDescending(s => s.ReleasedAt);

        Set? foundSet = null;
        Variant? foundVariant = null;

        foreach (var set in sets)
        {
            var variant = set.Variants.FirstOrDefault(s => s.Finish == "Standard");
            if (variant?.Product == "Booster") return new SetVariant { Variant = variant, Set = set };

            foundVariant ??= variant;
            foundSet = set;
        }

        if (foundSet is null || foundVariant is null)
        {
            foundVariant = sets.First().Variants.First();
            foundSet = sets.First();
        }

        return new SetVariant { Set = foundSet, Variant = foundVariant };
    }
}
