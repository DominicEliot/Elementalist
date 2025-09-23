using Elementalist.Infrastructure.DataAccess.CardData;
using Serilog;
using static Elementalist.Features.Cards.Prices;

namespace Elementalist;

public class CardDataService(ICardRepository tcgPlayerData) : BackgroundService
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
                await Task.Delay(TimeSpan.FromHours(47.5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
