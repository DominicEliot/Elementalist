using Serilog;
using static Elementalist.Features.Cards.Prices;

namespace Elementalist;

public class CardPriceService(TcgPlayerDataProvider tcgPlayerData) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("Fetching card price data.");
            var cards = await tcgPlayerData.GetTcgPlayerCards(stoppingToken);
            Log.Information("Loaded {count} card prices.", cards.Count);

            try
            {
                await Task.Delay(TimeSpan.FromHours(12) + TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
