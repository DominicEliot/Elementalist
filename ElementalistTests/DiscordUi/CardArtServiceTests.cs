using Elementalist.DiscordUi;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Models;
using Microsoft.Extensions.Options;
using SorceryBotTests.Features.Card;
using Xunit;

namespace SorceryBotTests.DiscordUi;

public class CardArtServiceTests
{
    [Fact]
    public async Task UrlTestAsync()
    {
        var options = Options.Create(new CardImageOptions
        {
            UrlFormat = @"https://dominiceliot.github.io/sorcery-image-gallery/media/original/{0}/{1}/{2}.png"
        });
        var service = new CardArtService(options);
        var repo = new MockCardRepository();
        var cards = await repo.GetCardsMatching(c => c.Name == "Hounds of Ondaros");
        var card = cards.FirstOrDefault();

        var variant = CardLookups.GetDefaultVariant(card!);

        var url = service.GetUrl(variant);

        Assert.Equal("https://dominiceliot.github.io/sorcery-image-gallery/media/original/Beta/b_s/hounds_of_ondaros_b_s.png", url);
    }
}
