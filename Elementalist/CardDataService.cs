using Elementalist.Infrastructure.DataAccess.CardData;
using Microsoft.Extensions.Options;
using Serilog;
using static Elementalist.Features.Cards.Prices;

namespace Elementalist;

public class CardDataService(ICardRepository tcgPlayerData, IOptions<DataRefreshOptions> refreshOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("Fetching card data.");
            var cards = await tcgPlayerData.GetCards();
            Log.Information("Loaded {count} cards.", cards.Count());

            try
            {
                var delayTime = TimeSpan.FromHours(refreshOptions.Value.Hours).Add(TimeSpan.FromSeconds(-5));

                if (delayTime <= TimeSpan.Zero)
                    delayTime = TimeSpan.FromMinutes(1);

                await Task.Delay(delayTime, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
