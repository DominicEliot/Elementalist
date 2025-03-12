using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBotTests.Features.Card;

internal class MockCardRepository : ICardRepository
{
    private List<SorceryBot.Models.Card> _cards = [
        new() { Name = "Pudge Butcher"}
        ];

    public Task<IEnumerable<SorceryBot.Models.Card>> GetCards()
    {
        return Task.FromResult(_cards.AsEnumerable());
    }

    public Task<IEnumerable<SorceryBot.Models.Card>> GetCardsMatching(Func<SorceryBot.Models.Card, bool> predicate)
    {
        return Task.FromResult(_cards.Where(predicate));
    }
}
