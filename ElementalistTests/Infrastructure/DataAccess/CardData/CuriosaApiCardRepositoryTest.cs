using Elementalist.Infrastructure.DataAccess.CardData;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace ElementalistTests.Infrastructure.DataAccess.CardData;

public class CuriosaApiCardRepositoryTest
{

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetCardsTest()
    {
        using var client = new HttpClient();
        var options = Options.Create(new DataRefreshOptions { Hours = 48 });

        var api = new CuriosaApiCardRepository(client, options);

        var cards = await api.GetCardsMatching(c => c.Name.Contains("Dragonlord", StringComparison.OrdinalIgnoreCase));

        Assert.True(cards.Any());
    }
}
