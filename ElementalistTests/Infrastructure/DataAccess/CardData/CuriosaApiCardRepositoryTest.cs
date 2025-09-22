using Elementalist.Infrastructure.DataAccess.CardData;
using Xunit;

namespace ElementalistTests.Infrastructure.DataAccess.CardData;

public class CuriosaApiCardRepositoryTest
{

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetCardsTest()
    {
        using var client = new HttpClient();
        var api = new CuriosaApiCardRepository(client);

        var cards = await api.GetCardsMatching(c => c.Name.Contains("Dragonlord", StringComparison.OrdinalIgnoreCase));

        Assert.True(cards.Any());
    }
}
