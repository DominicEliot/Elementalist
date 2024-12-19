using Discord;
using MediatR;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Models;
using SorceryBot.Shared;

namespace SorceryBot.Features.Card;

public record CardByFullNameQuery(string CardFullName) : IQuery<Models.Card?>;

public class CardSearchQueryHandler(ICardRepository cardRepository) : IRequestHandler<CardByFullNameQuery, Models.Card?>
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public async Task<Models.Card?> Handle(CardByFullNameQuery request, CancellationToken cancellationToken)
    {
        var cards = await _cardRepository.GetCardsMatching(c => c.Name == request.CardFullName);

        return cards.FirstOrDefault();
    }
}
