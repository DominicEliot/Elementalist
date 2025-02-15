using Discord.Interactions;
using Discord;
using MediatR;
using Microsoft.Extensions.Options;
using SorceryBot.Features.Card;
using SorceryBot.Infrastructure.Config;
using SorceryBot.Models;
using SorceryBot.Infrastructure.DataAccess.CardData;
using System.Reflection.Emit;

namespace SorceryBot.DiscordUi;

public static class CardDisplay
{
    internal static ComponentBuilder CardComponentBuilder(Models.Card card)
    {
        var builder = new ComponentBuilder();

        if (card.Sets.Count() > 1 || card.Sets.Any(s => s.Variants.Count() > 1))
        {
            AddVariantsMenu(card, builder);
        }

        builder.WithButton("Art", $"art-{card.Name}");
        builder.WithButton("Faq", $"faq-{card.Name}");
        builder.WithButton("Price", $"price-{card.Sets.First().Variants.First().Slug}");

        return builder;
    }

    private static void AddVariantsMenu(Card card, ComponentBuilder builder)
    {
        var menuBuilder = new SelectMenuBuilder();
        menuBuilder.WithCustomId($"variantSelect:{card.Name}");

        foreach (var set in card.Sets)
        {
            foreach (var variant in set.Variants)
            {
                menuBuilder.AddOption($"{set.Name} - {variant.Product} - {variant.Finish}", $"variant:{variant.Slug}");
            }
        }

        builder.WithSelectMenu(menuBuilder);
    }
}

public class CardAutoCompleteHandler(ICardRepository cardRepository) : AutocompleteHandler
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var value = autocompleteInteraction.Data.Current.Value as string;

        if (string.IsNullOrWhiteSpace(value))
        {
            return AutocompletionResult.FromSuccess();
        }

        var suggestions = await _cardRepository.GetCardsMatching(c => c.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
        return AutocompletionResult.FromSuccess(suggestions.Take(25).Select(c => new AutocompleteResult(c.Name, c.Name)));
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

        await SendDiscordResponse(cardText, cards, ephemeral: ephemeral);
    }

    private async Task SendDiscordResponse(string cardName, IEnumerable<Card> cards, bool ephemeral)
    {
        if (!cards.Any())
        {
            await RespondAsync($"Couldn't find match for '{cardName}'", ephemeral: true);
            return;
        }

        if (cards.Count() > _config.MaxCardEmbedsPerMessage)
        {
            await RespondAsync($"Too many matches for {cardName}", ephemeral: ephemeral);
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

        await RespondAsync(embeds: embeds.ToArray(), components: components);
    }
}

internal class EmbedCardDetailAdapter : EmbedBuilder
{
    private Card _card;

    public EmbedCardDetailAdapter(Card card, SetVariant? setVariant = null)
    {
        _card = card;
        setVariant ??= GetDefaultSetVariant(_card);

        //sample style: https://message.style/app/editor/share/KYfJ50a5
        WithAuthor(_card.Name);
        WithColor(DiscordLookups.GetCardColor(_card.Elements));
        WithThumbnailUrl(CardArt.GetUrl(setVariant));
        WithDescription(setVariant.Variant.TypeText);
        AddField("----", _card.Guardian.RulesText);
    }

    private SetVariant GetDefaultSetVariant(Card card)
    {
        var sets = _card.Sets.OrderByDescending(s => s.ReleasedAt);

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

public class SetVariant
{
    public Set Set { get; set; }
    public Variant Variant { get; set; }
}

public static class CardArt
{
    public static string GetUrl(SetVariant setVariant)
    {
        var cardSlug = setVariant.Variant.Slug.Substring(4); //slugs are in the format set_image-slug, for now...
        return $"https://github.com/XenotropicDev/SorceryBot/raw/refs/heads/main/SoBot/Images/{setVariant.Set.Name}/{cardSlug}.png";
    }
}

internal class EmbedCardArtAdapter : EmbedBuilder
{
    public EmbedCardArtAdapter(Models.Card card, SetVariant? setVariant = null)
    {
        setVariant ??= CardLookups.GetDefaultVariant(card);

        //sample style: https://message.style/app/editor/share/KYfJ50a5
        WithAuthor(card.Name);
        WithColor(DiscordLookups.GetCardColor(card.Elements));
        WithThumbnailUrl(CardArt.GetUrl(setVariant));
        WithFooter($"Art @ {setVariant.Variant.Artist}");
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
