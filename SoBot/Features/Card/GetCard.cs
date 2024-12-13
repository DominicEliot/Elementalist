using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

using Discord.Interactions;

using MediatR;
using Microsoft.Extensions.Options;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Models;
using SorceryBot.Shared;
using static SorceryBot.Features.Card.GetCard;

namespace SorceryBot.Features.Card;

public static class GetCard
{
    public record CardSearchQuery(string CardQuery) : IQuery<IEnumerable<Models.Card>>;
    public record CardByFullNameQuery(string CardFullName) : IQuery<Models.Card>;

    public class CardSearchQueryHandler(ICardRepository cardRepository) : IRequestHandler<CardSearchQuery, IEnumerable<Models.Card>>
    {
        private readonly ICardRepository _cardRepository = cardRepository;

        public async Task<IEnumerable<Models.Card>> Handle(CardSearchQuery request, CancellationToken cancellationToken)
        {
            var cards = await _cardRepository.GetCards();
            var matches = _cardRepository.GetCardsMatching(c => c.Name.Contains(request.CardQuery, StringComparison.OrdinalIgnoreCase));

            return await matches.ToListAsync();
        }
    }

    public class CardSearchSlashCommand(IMediator mediator, IOptions<BotConfig> config) : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator = mediator;
        private readonly BotConfig _config = config.Value;

        [SlashCommand("search", "Searches for and returns any matching sorcery cards")]
        public async Task CardSearch(string cardName)
        {
            var cards = await _mediator.Send(new CardSearchQuery(cardName));

            if (cards.Count() > _config.MaxCardEmbedsPerMessage)
            {
                await RespondAsync($"Too many matches for {cardName}");
                return;
            }

            var embeds = new List<Embed>();
            foreach (var card in cards)
            {
                embeds.Add(new EmbedCardAdapter(card).Build());
            }
        }
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
        {
            return new Color(0xb28950);
        }

        if (elements.Contains("Fire"))
        {
            return new Color(0xfb671d);
        }

        if (elements.Contains("Air"))
        {
            return new Color(0x959cb8);
        }

        if (elements.Contains("Earth"))
        {
            return new Color(0x909090);
        }

        if (elements.Contains("Water"))
        {
            return new Color(0x19cce3);
        }

        // Colorless
        return new Color(0xdcdcdc);
    }
}
