using System.Net.Http.Json;
using System.Text.Json;
using SorceryBot.Models;
using SorceryBot.Shared;

namespace SorceryBot.Infrastructure.DataAccess.CardData;

public interface ICardRepository
{
    Task<IEnumerable<Card>> GetCards();
    Task<IEnumerable<Card>> GetCardsMatching(Func<Card, bool> predicate);
}

public class CuriosaApiCardRepository(HttpClient httpClient) : ICardRepository
{
    private readonly HttpClient _httpClient = httpClient;
    private List<Card> _cards = [];
    private DateTimeOffset _lastUpdated;

    public async Task<IEnumerable<Card>> GetCards()
    {
        if (_lastUpdated < SystemClock.Now.AddHours(48))
        {
            var cardsFromApi = await _httpClient.GetAsync("https://api.sorcerytcg.com/api/cards");
            var cardResults = await cardsFromApi.Content.ReadFromJsonAsync<List<Card>>();
            _cards = cardResults ?? [];
            _lastUpdated = SystemClock.Now;
        }

        return _cards;
    }

    public async Task<IEnumerable<Card>> GetCardsMatching(Func<Card, bool> predicate)
    {
        IEnumerable<Card> filteredCards = (await GetCards()).Where(predicate);
        return filteredCards;
    }
}

public class FileCardRepository() : ICardRepository
{
    private List<Card> _cards = [];

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
            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            var cards = JsonSerializer.Deserialize<List<Card>>(json, jsonOptions);
            _cards = cards ?? [];
        }
    }

    public async Task<IEnumerable<Card>> GetCardsMatching(Func<Card, bool> predicate)
    {
        await PopulateRepository();

        return _cards.Where(predicate);
    }
}
