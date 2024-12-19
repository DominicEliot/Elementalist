using MediatR;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Shared;

namespace SorceryBot.Features.Card;

public record GetCardsQuery(string CardQuery) : IQuery<IEnumerable<Models.Card>>;

public class GetCardsQueryHandler(ICardRepository cardRepository) : IRequestHandler<GetCardsQuery, IEnumerable<Models.Card>>
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public async Task<IEnumerable<Models.Card>> Handle(GetCardsQuery request, CancellationToken cancellationToken)
    {
        return await _cardRepository
            .GetCardsMatching(c => c.Name.Contains(request.CardQuery, StringComparison.OrdinalIgnoreCase));
    }
}
