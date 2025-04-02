using System.Text.Json;
using Elementalist.DiscordUi;
using Xunit;

namespace SorceryBotTests.DiscordUi;
public class UniqueCardIdTests
{
    [Fact]
    public void TestSerialization()
    {
        var uniqueCard = new UniqueCardIdentifier("Caerleon-Upon-Usk", "Arthurian Legends", "Preconstructed_Deck", "Normal");

        var json = uniqueCard.ToJson();

        var uniqueCardDeserialized = JsonSerializer.Deserialize<UniqueCardIdentifier>(json);

        Assert.True(json.Length <= 100);
        Assert.Equal(uniqueCard.Name, uniqueCardDeserialized!.Name);
    }
}
