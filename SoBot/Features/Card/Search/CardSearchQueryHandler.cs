using MediatR;

namespace SorceryBot.Features.Card.Search;

public class CardSearchQueryHandler : IRequestHandler<CardSearchQuery, IEnumerable<CardDto>>
{
    public Task<IEnumerable<CardDto>> Handle(CardSearchQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
