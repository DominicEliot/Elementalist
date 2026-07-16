using Elementalist.Infrastructure.DataAccess.CardData;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ElementalistTests;

internal class TestHelpers
{
    public static IMemoryCache MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
    public static IOptions<DataRefreshOptions> RefreshOptions = Options.Create(new DataRefreshOptions() { Hours = 99999 });
}
