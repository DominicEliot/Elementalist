using Discord;
using Discord.Interactions;

using MediatR;
using Microsoft.Extensions.Options;
using SorceryBot.Infrastructure.Config;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Shared;

namespace SorceryBot.Features.Card;

public class GetCards
{
    public record CardSearchQuery(string CardQuery) : IQuery<IEnumerable<Models.Card>>;

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

            MessageComponent? components = null;
            if (cards.Count() == 1)
            {
                components = CardDisplay.CardComponentBuilder(cards.First()).Build();
            }

            await RespondAsync(embeds: embeds.ToArray(), components: components);
        }
    }
}
