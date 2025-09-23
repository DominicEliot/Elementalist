using Elementalist.Infrastructure.DataAccess.CardData;
using Microsoft.Extensions.Options;
using Serilog;
using static Elementalist.Features.Cards.Prices;

namespace Elementalist;

public class CardDataService(ICardRepository cardRepository, IOptions<DataRefreshOptions> refreshOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("Fetching card data.");
            if (cardRepository is CuriosaApiCardRepository apiRepo)
            {
                await apiRepo.RefreshData();
            }

            var cards = await cardRepository.GetCards();
            Log.Information("Loaded {count} cards.", cards.Count());

            try
            {
                var delayTime = TimeSpan.FromHours(refreshOptions.Value.Hours);

                if (delayTime <= TimeSpan.FromSeconds(10))
                {
                    Log.Warning("Card data refresh time cannot be less than 10 seconds. Data will not be refreshed.");
                    return;
                }

                await Task.Delay(delayTime, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
