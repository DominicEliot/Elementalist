using System.Text.RegularExpressions;
using Elementalist.Features.Cards;
using Elementalist.Infrastructure.Config;
using Elementalist.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Elementalist.DiscordUi;

public class CardDisplayService(PriceEnabledService priceEnabledService, CardArtService cardArtService)
{
    public async Task<InteractionMessageProperties> CardInfoMessage(IEnumerable<Card> cards, SetVariant? variant, ulong GuildId)
    {
        var message = new InteractionMessageProperties();
        var embeds = new List<EmbedProperties>();
        foreach (var card in cards)
        {
            embeds.Add(new EmbedCardDetailAdapter(card, cardArtService, variant));
        }
        message.Embeds = embeds;

        if (cards.Count() == 1)
        {
            message.Components = await CardComponentBuilder(cards.First(), variant, GuildId);
        }

        return message;
    }

    internal async Task<List<IMessageComponentProperties>> CardComponentBuilder(Card card, SetVariant? variant, ulong guildId)
    {
        var components = new List<IMessageComponentProperties>();
        var buttonRow = new ActionRowProperties();

        if (card.Sets.Count() > 1 || card.Sets.Any(s => s.Variants.Count() > 1))
        {
            AddVariantsMenu(card, components, variant);
        }

        buttonRow.AddComponents(new ButtonProperties($"art:{card.Name}", "Art", NetCord.ButtonStyle.Primary),
                           new ButtonProperties($"faq:{card.Name}", "Faq", NetCord.ButtonStyle.Primary));

        if (await priceEnabledService.IsPriceEnabledOnServer(guildId))
        {
            buttonRow.AddComponents(new ButtonProperties($"price:{card.Name}", "Price", NetCord.ButtonStyle.Primary));
        }

        components.Add(buttonRow);

        return components;
    }

    private static void AddVariantsMenu(Card card, List<IMessageComponentProperties> componentsList, SetVariant? defaultVariant)
    {
        var menuBuilder = new StringMenuProperties("variantSelect");

        defaultVariant ??= CardLookups.GetDefaultVariant(card);

        var selectMenuOptions = new List<StringMenuSelectOptionProperties>();
        foreach (var set in card.Sets)
        {
            foreach (var variant in set.Variants)
            {
                var isDefault = (defaultVariant.Variant == variant && defaultVariant.Set == set);
                var uniqueCardId = new UniqueCardIdentifier(card.Name, set.Name, variant.Product, variant.Finish);
                selectMenuOptions.Add(new(uniqueCardId.ToNamelessString(), uniqueCardId.ToJson()) { Default = isDefault });
            }
        }

        menuBuilder.AddOptions(selectMenuOptions);

        componentsList.Add(menuBuilder);
    }
}

public class CardSearchSlashCommand(IMediator mediator,
                                    IOptions<BotConfig> config,
                                    CardDisplayService cardDisplayService) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly BotConfig _config = config.Value;
    private readonly IMediator _mediator = mediator;
    private readonly CardDisplayService _cardDisplayService = cardDisplayService;

    [SlashCommand("name", "Searches for and returns any matching sorcery cards")]
    public async Task CardSearchByName([SlashCommandParameter(AutocompleteProviderType = typeof(CardAutoCompleteHandler))] string cardName, bool ephemeral = false)
    {
        var query = new GetCardsQuery() { CardNameContains = cardName };
        var cards = await _mediator.Send(query);

        await SendDiscordResponse(cardName, cards, ephemeral, query);
    }

    [SlashCommand("text", "Searches for and returns any matching sorcery cards")]
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
        var message = new InteractionMessageProperties();

        if (!cards.Any())
        {
            message.WithContent($"Couldn't find match for '{cardNameOrText ?? query.ToString()}'")
                .WithFlags(NetCord.MessageFlags.Ephemeral);
            await RespondAsync(InteractionCallback.Message(message));
            return;
        }

        if (cards.Count() > _config.MaxCardEmbedsPerMessage)
        {
            message.WithContent($"Too many matches to display your search results,\nplease see {GetRealmsAppUrl(query)}");

            await RespondAsync(InteractionCallback.Message(message));
            return;
        }

        message = await _cardDisplayService.CardInfoMessage(cards, null, Context.GetGuildId() ?? 0);
        if (ephemeral) message.WithFlags(NetCord.MessageFlags.Ephemeral);

        if (cards.Count() > 1 || string.IsNullOrWhiteSpace(query.CardNameContains))
        {
            var searchParamters = (query is not null) ? $"Search Criteria: " : string.Empty;
            if (query?.CardNameContains is not null) searchParamters += $"Name: {query.CardNameContains} ";
            if (query?.TextContains is not null) searchParamters += $"Card Text: {query.TextContains} ";
            if (query?.ElementsContain is not null) searchParamters += $"Element: {query.ElementsContain}";

            message.Content = searchParamters;
        }

        await RespondAsync(InteractionCallback.Message(message));
    }

    //Todo: this should maybe be its own service
    private static readonly string[] _hiddenCardTypes = ["Minion", "Artifact"];

    public static Uri GetRealmsAppUrl(GetCardsQuery query)
    {
        if (query is null) throw new NullReferenceException($"{nameof(query)} cannot be null");

        List<string> queryParams = query.TextContains?.Replace(",", "").Split(' ').Select(t => $"x:{t}").ToList() ?? [];
        queryParams.AddRange(query.CardNameContains?.Replace(",", "").Split(' ').Select(n => $"n:{n}") ?? []);
        queryParams.AddRange(query.ElementsContain?.Replace(",", "").Split(' ').Select(e => $"e:{e}") ?? []);

        foreach (var cardType in query.TypeContains?.Replace(",", "").Split(' ') ?? [])
        {
            var cardTypeSyntaxOption = _hiddenCardTypes.Contains(cardType, StringComparer.OrdinalIgnoreCase) ? "t:" : "l:";
            queryParams.Add($"{cardTypeSyntaxOption}{cardType}");
        }

        var uri = new UriBuilder($"https://www.realmsapp.com/sorcery_tcg/cards")
        {
            Query = $"?query={string.Join("+", queryParams)}"
        };
        return uri.Uri;
    }
}

internal class EmbedCardDetailAdapter : EmbedProperties
{
    public EmbedCardDetailAdapter(Card card, CardArtService cardArtService, SetVariant? setVariant = null)
    {
        setVariant ??= CardLookups.GetDefaultVariant(card);

        var cardCostSymbols = DiscordHelpers.GetManaEmojis(card);
        var thresholdSymbols = DiscordHelpers.GetThresholdEmojis(card.Guardian.Thresholds);

        WithTitle($"{card.Name} {cardCostSymbols} {thresholdSymbols}");
        WithUrl($"https://curiosa.io/cards/{card.Name.ToLower().Replace(' ', '_')}");
        WithColor(DiscordHelpers.GetCardColor(card.Elements));
        WithThumbnail(new(cardArtService.GetUrl(setVariant)));
        WithDescription(setVariant.Variant.TypeText);

        var powerText = (card.Guardian.Attack > 0) ? $"Attack: {card.Guardian.Attack} " : string.Empty;
        var defenseText = (card.Guardian.Defence > 0 && card.Guardian.Defence != card.Guardian.Attack) ? $"Defence: {card.Guardian.Defence} " : string.Empty;
        var rulesTextField = $"{powerText}{defenseText}\n{DiscordHelpers.ReplaceManaTokensWithEmojis(card.Guardian.RulesText)}".Trim();

        var subtypeText = (!string.IsNullOrEmpty(card.SubTypes)) ? $" - {card.SubTypes}" : string.Empty;

        AddFields(new EmbedFieldProperties().WithName(card.Guardian.Type + subtypeText).WithValue(rulesTextField));
    }
}

public partial class CardArtService(IOptions<CardImageOptions> imageOptions)
{
    public string GetUrl(SetVariant setVariant)
    {
        return GetUrl(setVariant.Variant.Slug, setVariant.Set.Name);
    }

    public string GetUrl(string cardSlug, string setName)
    {
        if (imageOptions.Value.RemoveSpacesInSetName)
        {
            setName = setName.Replace(" ", string.Empty);
        }
        var escapedSet = Uri.EscapeDataString(setName);

        var imageSlug = cardSlug.Substring(4); //slugs are in the format set_image-slug, for now...
        var productSlugMatch = ProdcutSlugRegex().Match(cardSlug);
        if (productSlugMatch.Success)
        {
            var productSlug = productSlugMatch.Groups[2].Value;
            return string.Format(imageOptions.Value.UrlFormat, cardSlug, escapedSet, productSlug, imageSlug);
        }

        return string.Format(imageOptions.Value.UrlFormat, cardSlug, escapedSet, imageSlug);
    }

    [GeneratedRegex(@"^(\w{3})-.*_(\w+_\w+)$")]
    private static partial Regex ProdcutSlugRegex();
}

public class CardImageOptions
{
    public required string UrlFormat { get; init; }
    public bool RemoveSpacesInSetName { get; init; } = false;
}

public static class CardLookups
{
    public static SetVariant GetDefaultVariant(Models.Card card)
    {
        var sets = card.Sets.OrderByDescending(s => s.ReleasedAt);

        Set? foundSet = null;
        Models.Variant? foundVariant = null;

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
