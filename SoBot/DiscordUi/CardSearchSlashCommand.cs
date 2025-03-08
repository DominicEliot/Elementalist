using Discord;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.Options;
using SorceryBot.Features.Card;
using SorceryBot.Infrastructure.Config;
using SorceryBot.Models;

namespace SorceryBot.DiscordUi;

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
        var cards = await _mediator.Send(new GetCardsQuery() { CardNameContains = cardName });

        await SendDiscordResponse(cardName, cards, ephemeral);
    }

    [SlashCommand("search-by-text", "Searches for and returns any matching sorcery cards")]
    public async Task CardSearchByRulesText(string cardText, string? element = null, bool ephemeral = false)
    {
        var query = new GetCardsQuery()
        {
            TextContains = cardText,
            ElementsContain = element
        };
        var cards = await _mediator.Send(query);

        await SendDiscordResponse(cardText, cards, ephemeral: ephemeral, query);
    }

    private async Task SendDiscordResponse(string cardName, IEnumerable<Card> cards, bool ephemeral, GetCardsQuery? query = null)
    {
        if (!cards.Any())
        {
            await RespondAsync($"Couldn't find match for '{cardName}'", ephemeral: true);
            return;
        }

        if (cards.Count() > _config.MaxCardEmbedsPerMessage)
        {
            await RespondAsync($"Too many matches for {cardName}", ephemeral: true);
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
}

internal class EmbedCardDetailAdapter : EmbedBuilder
{
    public EmbedCardDetailAdapter(Card card, SetVariant? setVariant = null)
    {
        setVariant ??= CardLookups.GetDefaultVariant(card);

        var cardCostSymbols = DiscordHelpers.GetManaEmojis(card);
        var thresholdSymbols = DiscordHelpers.GetThresholdEmojis(card.Guardian.Thresholds);

        WithTitle($"{card.Name} {cardCostSymbols} {thresholdSymbols}");
        WithColor(DiscordHelpers.GetCardColor(card.Elements));
        WithThumbnailUrl(CardArt.GetUrl(setVariant));
        WithDescription(setVariant.Variant.TypeText);

        var powerText = (card.Guardian.Attack > 0) ? $"Attack: {card.Guardian.Attack} " : string.Empty;
        var defenseText = (card.Guardian.Defence > 0 && card.Guardian.Defence != card.Guardian.Attack) ? $"Defence: {card.Guardian.Defence} " : string.Empty;
        var rulesTextField = $"{powerText}{defenseText}\n{DiscordHelpers.ReplaceManaTokensWithEmojis(card.Guardian.RulesText)}".Trim();
        AddField(card.SubTypes, rulesTextField);
    }
}

public class SetVariant
{
    public Set Set { get; set; }
    public Variant Variant { get; set; }
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
        return $"http://wrexial.com/images/{escapedSet}/{cardSlug}.png";
    }
}

internal static class CardLookups
{
    internal static SetVariant GetDefaultVariant(Models.Card card)
    {
        var sets = card.Sets.OrderByDescending(s => s.ReleasedAt);

        SetVariant setVariant = new();

        foreach (var set in sets)
        {
            var variant = set.Variants.FirstOrDefault(s => s.Finish == "Standard");
            if (variant?.Product == "Booster") return new SetVariant { Variant = variant, Set = set };

            setVariant.Variant ??= variant;
            setVariant.Set = set;
        }

        if (setVariant.Variant == null)
        {
            setVariant.Variant = sets.First().Variants.First();
            setVariant.Set = sets.First();
        }

        return setVariant;
    }
}
