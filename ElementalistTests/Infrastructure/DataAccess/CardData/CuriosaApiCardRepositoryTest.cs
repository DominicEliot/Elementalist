using Elementalist.Infrastructure.DataAccess.CardData;
using Microsoft.Extensions.Options;
using Xunit;

namespace ElementalistTests.Infrastructure.DataAccess.CardData;

public class CuriosaApiCardRepositoryTest
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetCardsTest()
    {
        var skip = Environment.OSVersion.Platform == PlatformID.Win32NT; //This is a hack so that this test doesn't run in an environment where I can't access curiosa
        if (skip)
        {
            Assert.True(true);
            return; 
        }

        using var client = new HttpClient();
        var options = Options.Create(new DataRefreshOptions { Hours = 48 });

        var api = new CuriosaApiCardRepository(client, options);

        var cards = await api.GetCardsMatching(c => c.Name.Contains("Dragonlord", StringComparison.OrdinalIgnoreCase));

        Assert.True(cards.Any());
    }
}
