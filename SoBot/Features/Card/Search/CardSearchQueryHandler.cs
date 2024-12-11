using MediatR;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Models;

namespace SorceryBot.Features.Card.Search;

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
