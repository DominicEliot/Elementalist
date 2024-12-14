using Discord;
using MediatR;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Models;
using SorceryBot.Shared;
using static SorceryBot.Features.Card.GetCards;

namespace SorceryBot.Features.Card;

public record CardByFullNameQuery(string CardFullName) : IQuery<Models.Card>;

public class CardSearchQueryHandler(ICardRepository cardRepository) : IRequestHandler<CardByFullNameQuery, Models.Card>
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public async Task<Models.Card> Handle(CardByFullNameQuery request, CancellationToken cancellationToken)
    {
        var card = await _cardRepository.GetCardsMatching(c => c.Name == request.CardFullName).FirstOrDefaultAsync();

        return card;
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
