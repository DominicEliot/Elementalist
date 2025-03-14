using Elementalist.Infrastructure.DataAccess.CardData;

namespace SorceryBotTests.Features.Card;

internal class MockCardRepository : ICardRepository
{
    private List<Elementalist.Models.Card> _cards = [
        new() { Name = "Pudge Butcher"}
        ];

    public Task<IEnumerable<Elementalist.Models.Card>> GetCards()
    {
        return Task.FromResult(_cards.AsEnumerable());
    }

    public Task<IEnumerable<Elementalist.Models.Card>> GetCardsMatching(Func<Elementalist.Models.Card, bool> predicate)
    {
        return Task.FromResult(_cards.Where(predicate));
    }
}
