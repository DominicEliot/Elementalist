using System.Net.Http.Json;
using System.Text.Json;
using SorceryBot.Models;
using SorceryBot.Shared;

namespace SorceryBot.Infrastructure.DataAccess.CardData;

public interface ICardRepository
{
    Task<IEnumerable<Card>> GetCards();
    IAsyncEnumerable<Card> GetCardsMatching(Func<Card, bool> predicate);

}

public class CuriosaApiCardRepository(HttpClient httpClient) : ICardRepository
{
    private readonly HttpClient _httpClient = httpClient;
    private List<Card> _cards = [];
    private DateTimeOffset _lastUpdated;


    public async Task<IEnumerable<Card>> GetCards()
    {
        if (_lastUpdated < SystemClock.Now.AddHours(24))
        {
            var cardsFromApi = await _httpClient.GetAsync("https://api.sorcerytcg.com/api/cards");
            var cardResults = await cardsFromApi.Content.ReadFromJsonAsync<List<Card>>();
            _cards = cardResults ?? [];
            _lastUpdated = SystemClock.Now;
        }

        return _cards;
    }

    public async Task<IEnumerable<Card>> GetCards(Func<IEnumerable<Card>, IEnumerable<Card>> filter)
    {
        return filter.Invoke(await GetCards());
    }

    public async IAsyncEnumerable<Card> GetCardsMatching(Func<Card, bool> predicate)
    {
        foreach (var card in await GetCards())
        {
            if (predicate(card))
            {
                yield return card;
            }
        }
    }
}

public class FileCardRepository() : ICardRepository
{
    List<Card> _cards = [];

    public async Task<IEnumerable<Card>> GetCards()
    {
        await PopulateRepository();

        return _cards;
    }

    private async Task PopulateRepository()
    {
        if (_cards.Count == 0)
        {
            var json = await File.ReadAllTextAsync(@"Infrastructure\DataAccess\CardData\cards.json");
            var cards = JsonSerializer.Deserialize<List<Card>>(json);
            _cards = cards ?? [];
        }
    }

    public async IAsyncEnumerable<Card> GetCardsMatching(Func<Card, bool> predicate)
    {
        await PopulateRepository();

        foreach (var card in _cards)
        {
            if (predicate(card)) yield return card;
        }
    }
}
