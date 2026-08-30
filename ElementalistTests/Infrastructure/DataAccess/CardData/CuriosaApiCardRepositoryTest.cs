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

        // using var client = new HttpClient();
        // var refreshOptions = Options.Create(new DataRefreshOptions { Hours = 48 });
        // var apiOptions = Options.Create(new CardApiOptions { Url = "https://api.sorcerytcg.com/api/cards"});
        //
        // var repo = new CuriosaApiCardRepository(client, refreshOptions, apiOptions);
        var repo = new FileCardRepository();

        var cards = await repo.GetCardsMatching(c => c.Name.Contains("Dragonlord", StringComparison.OrdinalIgnoreCase));

        Assert.True(cards.Any());
    }
}
