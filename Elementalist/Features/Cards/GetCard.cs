using Discord;
using MediatR;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Models;
using Elementalist.Shared;

namespace Elementalist.Features.Card;

public record CardByFullNameQuery : IQuery<Models.Card?>
{
    public string CardFullName { get; set; }
}

public class CardSearchQueryHandler(ICardRepository cardRepository) : IRequestHandler<CardByFullNameQuery, Models.Card?>
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public async Task<Models.Card?> Handle(CardByFullNameQuery request, CancellationToken cancellationToken)
    {
        var cards = await _cardRepository.GetCardsMatching(c => c.Name == request.CardFullName);

        return cards.FirstOrDefault();
    }
}
