using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ElementalistTests;

internal class TestHelpers
{
    public static IMemoryCache MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
}
