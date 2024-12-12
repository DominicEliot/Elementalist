using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;

using Discord.Interactions;

using MediatR;
using SorceryBot.Infrastructure.DataAccess.CardData;
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

    public class CardSearchSlashCommand(IMediator mediator) : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IMediator _mediator = mediator;

        [SlashCommand("search", "Searches for and returns any matching sorcery cards")]
        public async Task CardSearch(string cardName)
        {
            var card = await _mediator.Send(new CardSearchQuery(cardName));
            await RespondAsync($"{cardName}'s info should go here.");
        }
    }
}
