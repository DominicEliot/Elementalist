using System.Net.Http.Json;
using System.Text.Json;
using Elementalist.Models;
using Elementalist.Shared;
using Microsoft.Extensions.Options;

namespace Elementalist.Infrastructure.DataAccess.CardData;

public interface ICardRepository
{
    Task<IEnumerable<Card>> GetCards();

    Task<IEnumerable<Card>> GetCardsMatching(Func<Card, bool> predicate);
}

public class CuriosaApiCardRepository(HttpClient httpClient, IOptions<DataRefreshOptions> dataRefreshOptions, ILogger<CuriosaApiCardRepository> logger) : ICardRepository
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IOptions<DataRefreshOptions> _dataRefreshOptions = dataRefreshOptions;
    private List<Card> _cards = [];

    public async Task RefreshData()
    {
        //todo move this into an IOptions
        var cardsFromApi = await _httpClient.GetAsync("https://api.sorcerytcg.com/api/cards");
        if (!cardsFromApi.IsSuccessStatusCode)
        {
            if (_cards.Count == 0)
            {
                _cards = await GetCardsFromBackupSource();
                logger.LogWarning("Tried to refresh card data but https://api.sorcerytcg.com/api/cards returned with a status code of {httpStatus}. Loaded {cardsCount} cards from file instead",  cardsFromApi.StatusCode, _cards.Count);
                return;
            }

            logger.LogWarning("Tried to refresh card data but https://api.sorcerytcg.com/api/cards returned with a status code of {httpStatus}. There are currently {cardsCount} cards cached from the last successful fetch.",  cardsFromApi.StatusCode, _cards.Count);
            return;
        }

        var cardResults = await cardsFromApi.Content.ReadFromJsonAsync<List<Card>>();
        _cards = cardResults ?? await GetCardsFromBackupSource();
    }

    private async Task<List<Card>> GetCardsFromBackupSource()
    {
        var fileRepo = new FileCardRepository();
        var cards = await fileRepo.GetCards();
        return new List<Card>(cards);
    }

    public async Task<IEnumerable<Card>> GetCards()
    {
        if (_cards.Count == 0)
        {
            await RefreshData();
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
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IEnumerable<Card>> GetCards()
    {
        await PopulateRepository();

        return _cards;
    }

    private async Task PopulateRepository()
    {
        if (_cards.Count == 0)
        {
            var json = await File.ReadAllTextAsync(Path.Combine("Infrastructure", "DataAccess", "CardData", "cards.json"));

            var cards = JsonSerializer.Deserialize<List<Card>>(json, _jsonOptions);
            _cards = cards ?? [];
        }
    }

    public async Task<IEnumerable<Card>> GetCardsMatching(Func<Card, bool> predicate)
    {
        await PopulateRepository();

        return _cards.Where(predicate);
    }
}
