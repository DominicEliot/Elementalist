using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBotTests.Features.Card;

internal class MockCardRepository : ICardRepository
{
    private List<Models.Card> _cards = [
        new() { Name = "Pudge Butcher"}
        ];

    public Task<IEnumerable<Models.Card>> GetCards()
    {
        return Task.FromResult(_cards.AsEnumerable());
    }

    public Task<IEnumerable<Models.Card>> GetCardsMatching(Func<Models.Card, bool> predicate)
    {
        return Task.FromResult(_cards.Where(predicate));
    }
}
