using Discord.Interactions;
using Discord;
using MediatR;
using Microsoft.Extensions.Options;
using SorceryBot.Features.Card;
using SorceryBot.Infrastructure.Config;
using SorceryBot.Models;
using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBot.DiscordUi;

public class CardSearchSlashCommand(IMediator mediator, IOptions<BotConfig> config) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly BotConfig _config = config.Value;
    private readonly IMediator _mediator = mediator;

    [SlashCommand("search", "Searches for and returns any matching sorcery cards")]
    public async Task CardSearch([Autocomplete<CardAutoCompleteHandler>()]string cardName)
    {
        var cards = await _mediator.Send(new GetCardsQuery(cardName));

        if (cards.Count() > _config.MaxCardEmbedsPerMessage)
        {
            await RespondAsync($"Too many matches for {cardName}", ephemeral: true);
            return;
        }

        if (cards.Count() == 0)
        {
            await RespondAsync($"Unknown card {cardName}", ephemeral: true);
            return;
        }

        var embeds = new List<Embed>();
        foreach (var card in cards)
        {
            embeds.Add(new EmbedCardAdapter(card).Build());
        }

        MessageComponent? components = null;
        if (cards.Count() == 1)
        {
            components = CardDisplay.CardComponentBuilder(cards.First()).Build();
        }

        await RespondAsync(embeds: embeds.ToArray(), components: components);
    }
}

public class CardAutoCompleteHandler(ICardRepository cardRepository) : AutocompleteHandler
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var value = autocompleteInteraction.Data.Current.Value.ToString();

        if (string.IsNullOrWhiteSpace(value))
        {
            return AutocompletionResult.FromSuccess();
        }

        var suggestions = await _cardRepository.GetCardsMatching(c => c.Name.Contains(value));
        return AutocompletionResult.FromSuccess(suggestions.Take(25).Select(c => new AutocompleteResult(c.Name, c.Name.ToLower())));
    }
}

public static class CardDisplay
{
    internal static ComponentBuilder CardComponentBuilder(Models.Card card)
    {
        var builder = new ComponentBuilder();

        var variants = card.Sets.SelectMany(s => s.Variants);
        if (variants.Count() > 1)
        {
            var menuBuilder = new SelectMenuBuilder();

            foreach (var variant in variants)
            {
                menuBuilder.AddOption(variant.Product, $"variant-{variant.Slug}");
            }

            builder.WithSelectMenu(menuBuilder);
        }

        builder.WithButton("Faq", $"faq-{card.Name}");
        builder.WithButton("Price", $"price-{card.Name}");

        return builder;
    }
}

internal class EmbedCardAdapter : EmbedBuilder
{
    private Models.Card _card;

    public EmbedCardAdapter(Models.Card card, Variant? variant = null)
    {
        _card = card;
        variant ??= GetDefaultVariant(_card);

        //sample style: https://message.style/app/editor/share/KYfJ50a5
        WithAuthor(_card.Name);
        WithColor(GetDiscordColor(_card.Elements));
        WithDescription(variant.TypeText);
        AddField("----", _card.Guardian.RulesText);
        //WithFooter($"Art @ {variant.Artist}");
    }

    private Variant GetDefaultVariant(Models.Card card)
    {
        var sets = _card.Sets.OrderByDescending(s => s.ReleasedAt);

        Variant? bestFitVariant = null;
        foreach (var set in sets)
        {
            var variant = set.Variants.FirstOrDefault(s => s.Finish == "Standard");
            if (variant?.Product == "Booster") return variant;

            bestFitVariant ??= variant;
        }

        return bestFitVariant ?? sets.First().Variants.First();
    }

    private Color GetDiscordColor(string elements)
    {
        if (elements.Contains(","))
            return new Color(0xb28950);

        if (elements.Contains("Fire"))
            return new Color(0xfb671d);

        if (elements.Contains("Air"))
            return new Color(0x959cb8);

        if (elements.Contains("Earth"))
            return new Color(0x909090);

        if (elements.Contains("Water"))
            return new Color(0x19cce3);

        // Colorless
        return new Color(0xdcdcdc);
    }
}
