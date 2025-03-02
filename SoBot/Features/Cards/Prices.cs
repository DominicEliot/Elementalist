using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SorceryBot.Models;
using SorceryBot.Shared;

namespace SorceryBot.Features.Cards;

public static class Prices
{
    public record CardPriceQuery(string CardName, string Set, string CardFinish) : IQuery<Result<PriceData>>;

    public class CardPriceHandler(TcgPlayerDataProvider priceProvider) : IRequestHandler<CardPriceQuery, Result<PriceData>>
    {
        private readonly TcgPlayerDataProvider _priceProvider = priceProvider;

        public async Task<Result<PriceData>> Handle(CardPriceQuery request, CancellationToken cancellationToken)
        {
            var prices = await _priceProvider.GetPriceData(request.CardName, request.Set, request.CardFinish);

            return prices;
        }
    }

    public class TcgPlayerDataProvider(IOptions<TcgPlayerSettings> settings, IMemoryCache cache, HttpClient httpClient)
    {
        private readonly IOptions<TcgPlayerSettings> _settings = settings;
        private readonly IMemoryCache _cache = cache;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<PriceData>> GetPriceData(string cardName, string set, string cardFinish)
        {
            var tcgPlayerCardData = (await GetTcgPlayerCards()).FirstOrDefault(c => c.ProductName.Contains(cardName) && c.Set == set && c.Printing == cardFinish);

            if (tcgPlayerCardData == null)
            {
                return new Result<PriceData>(new NotFoundError());
            }

            var priceData = new PriceData() { Low = tcgPlayerCardData.LowPrice, Mid = tcgPlayerCardData.MarketPrice };

            return new Result<PriceData>(priceData);
        }

        public async Task<List<TcgPlayerSetResult>> GetSetsIdsAsync()
        {
            var sets = await _cache.GetOrCreateAsync<List<TcgPlayerSetResult>>("tcgPlayerSets", async cacheItem =>
            {
                var reply = await _httpClient.GetAsync(_settings.Value.FormattedSetsUrl);

                var items = JsonSerializer.Deserialize<TcgPlayerGameSets>(await reply.Content.ReadAsStringAsync(), new JsonSerializerOptions(JsonSerializerDefaults.Web));

                cacheItem.SetAbsoluteExpiration(TimeSpan.FromHours(12))
                .SetValue(items.Results);

                return items.Results!;
            });
            return sets;
        }

        public async Task<List<TcgPlayerCard>> GetTcgPlayerCards()
        {
            List<TcgPlayerCard>? cards = await _cache.GetOrCreateAsync("cardIds", async cacheItem =>
            {
                var cardsList = new List<TcgPlayerCard>();
                foreach (var set in await GetSetsIdsAsync())
                {
                    var cardIdsResult = await _httpClient.GetAsync(string.Format(_settings.Value.CardIdsUrl, set.SetNameId));

                    var tcgPlayerSet = JsonSerializer.Deserialize<TcgPlayerSet>(await cardIdsResult.Content.ReadAsStringAsync(), new JsonSerializerOptions(JsonSerializerDefaults.Web));

                    cardsList.AddRange(tcgPlayerSet?.Result ?? []);
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
    public int SorceryProductId { get; set; } = 77;
    public string SorceryUrlValue { get; set; } = "sorcery-contested-realm";
    private string CardPriceUrl { get; set; } = "https://mpapi.tcgplayer.com/v2/product/{0}/pricepoints"; //0 = CardId
    public string SetsUrl { get; set; } = "https://mpapi.tcgplayer.com/v2/Catalog/SetNames?categoryId={0}&active=true&mpfev=3118"; //0 = SorceryProductId
    public string CardIdsUrl { get; set; } = "https://infinite-api.tcgplayer.com/priceguide/set/{0}/cards/?rows=5000&productTypeID=128"; //0 = SetId
    public string GetPriceGuideUrl(int setId) => string.Format(CardIdsUrl, setId);
    public string GetPriceUrl(int cardId) => string.Format(CardPriceUrl, cardId);
    public string FormattedSetsUrl => string.Format(SetsUrl, SorceryProductId.ToString());
}
