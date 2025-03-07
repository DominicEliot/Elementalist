using SorceryBot.DiscordUi;
using Xunit;

namespace SorceryBotTests.DiscordUi;
public class UniqueCardIdTests
{
    [Fact]
    public void TestSerialization()
    {
        var uniqueCard = new UniqueCardIdentifier("Caerleon-Upon-Usk", "Arthurian Legends", "Preconstructed_Deck", "Normal");

        var json = uniqueCard.ToJson();

        Assert.True(json.Length <= 100);
    }
}
