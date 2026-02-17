using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using NetCord.Services;

namespace Elementalist;

public class CheckForDisabledPriceServer<TContext> : PreconditionAttribute<TContext> where TContext : IGuildContext
{
    public override async ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        var priceFeatureService = serviceProvider!.GetRequiredService<PriceEnabledService>();
        if (!await priceFeatureService.IsPriceEnabledOnServer(context.Guild?.Id ?? 0))
        {
            return PreconditionResult.Fail("Prices are disabled on this server.");
        }

        return PreconditionResult.Success;
    }
}

public class PriceEnabledService(IFeatureManager featureManager, IOptions<PerServerConfig> options)
{
    public async Task<bool> IsPriceEnabledOnServer(ulong guildId)
    {
        if (!await featureManager.IsEnabledAsync("prices"))
        {
            return false;
        }

        return options?.Value.PricesDisabled.Contains(guildId) == false;
    }
}
