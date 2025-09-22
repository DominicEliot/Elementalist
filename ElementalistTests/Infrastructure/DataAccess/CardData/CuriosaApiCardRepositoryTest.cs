using Elementalist.Infrastructure.DataAccess.CardData;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ElementalistTests.Infrastructure.DataAccess.CardData;

public class CuriosaApiCardRepositoryTest
{

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetCardsTest()
    {
        using var client = new HttpClient();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        var api = new CuriosaApiCardRepository(client, loggerFactory.CreateLogger<ICardRepository>());

        var cards = await api.GetCardsMatching(c => c.Name.Contains("Dragonlord", StringComparison.OrdinalIgnoreCase));

        Assert.True(cards.Any());
    }
}
