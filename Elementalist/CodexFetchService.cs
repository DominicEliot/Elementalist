using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Infrastructure.DataAccess.Rules;
using Microsoft.Extensions.Options;
using Serilog;

namespace Elementalist;

public class CodexFetchService(IRulesRepository rulesRepo, IOptions<DataRefreshOptions> refreshOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("Fetching curiosa codex data from GitHub.");
            var rules = await rulesRepo.GetRules(stoppingToken);
            Log.Information("Loaded {count} codex entries.", rules.Count());

            try
            {
                await Task.Delay(TimeSpan.FromHours(refreshOptions.Value.Hours) + TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}

public class FaqFetchService(IFaqRepository faqRepo, IOptions<DataRefreshOptions> refreshOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("Fetching curiosa faq data from Curiosa.Io");
            var faqs = await faqRepo.GetFaqs(stoppingToken);
            Log.Information("Loaded {count} faq entries.", faqs.Count());

            try
            {
                await Task.Delay(TimeSpan.FromHours(refreshOptions.Value.Hours) + TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
