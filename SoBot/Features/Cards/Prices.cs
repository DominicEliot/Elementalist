using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SorceryBot.Shared;

namespace SorceryBot.Features.Cards;

public static class Prices
{
    public record CardPriceQuery(string cardSlug) : IQuery<Result<PriceData>>;

    public class CardPriceHandler(TcgPlayerDataProvider priceProvider) : IRequestHandler<CardPriceQuery, Result<PriceData>>
    {
        private readonly TcgPlayerDataProvider _priceProvider = priceProvider;

        public async Task<Result<PriceData>> Handle(CardPriceQuery request, CancellationToken cancellationToken)
        {
            var prices = await _priceProvider.GetPriceData(request.cardSlug);

            return prices;
        }
    }

    public class TcgPlayerDataProvider(IOptions<TcgPlayerSettings> settings, IMemoryCache cache, HttpClient httpClient)
    {
        private readonly IOptions<TcgPlayerSettings> _settings = settings;
        private readonly IMemoryCache _cache = cache;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<PriceData>> GetPriceData(string cardSlug)
        {
            var cardId = (await GetCardIds()).FirstOrDefault(c => c.CardSlug == cardSlug);

            if (cardId == null)
            {
                return new Result<PriceData>(new NotFoundError());
            }

            var result = await _httpClient.GetAsync(_settings.Value.GetPriceUrl(cardId.TcgPlayerCardId));

            var tcgPlayerPriceData = JsonSerializer.Deserialize<PriceData>(await result.Content.ReadAsStringAsync());

            return new Result<PriceData>(tcgPlayerPriceData);
        }

        public async Task<Dictionary<string, int>> GetSetsIdsAsync()
        {
            var sets = await _cache.GetOrCreateAsync<Dictionary<string, int>>("tcgPlayerSets", async cacheItem =>
            {
                var reply = await _httpClient.GetAsync(_settings.Value.FormattedSetsUrl);

                var items = JsonSerializer.Deserialize<Dictionary<string, int>>(await reply.Content.ReadAsStringAsync());

                cacheItem.SetAbsoluteExpiration(TimeSpan.FromHours(12))
                .SetValue(items);

                return items!;
            });
            return sets;
        }

        public async Task<List<TcgPriceListItem>> GetCardIds()
        {
            List<TcgPriceListItem>? cards = await _cache.GetOrCreateAsync("cardIds", async cacheItem =>
            {
                var cardsList = new List<TcgPriceListItem>();
                foreach (var set in await GetSetsIdsAsync())
                {
                    var cardIdsResult = await _httpClient.GetAsync(string.Format(_settings.Value.CardIdsUrl, set));

                    var cardIds = JsonSerializer.Deserialize<List<TcgPriceListItem>>(await cardIdsResult.Content.ReadAsStringAsync());

                    cardsList.AddRange(cardIds ?? []);
                }

                cacheItem.SetAbsoluteExpiration(TimeSpan.FromHours(12))
                .SetValue(cardsList);

                return cardsList;
            });

            return cards!;
        }
    }

    public record PriceData
    {
        public Double Mid { get; set; }
        public Double Low { get; set; }
    }
}

public class TcgPriceListItem
{
    public int TcgPlayerCardId { get; set; }
    public string CardName { get; set; }
    public string CardSlug { get; set; }
}

public class TcgPlayerSettings
{
    public int SorceryProductId { get; set; }
    public string SorceryUrlValue { get; set; }
    private string CardPriceUrl { get; set; }
    public string SetsUrl { get; set; }
    public string CardIdsUrl { get; set; }

    public string GetPriceGuideUrl(int setId) => string.Format(CardIdsUrl, setId);
    public string GetPriceUrl(int cardId) => string.Format(CardIdsUrl, cardId);
    public string FormattedSetsUrl => string.Format(SetsUrl, SorceryProductId);
}
