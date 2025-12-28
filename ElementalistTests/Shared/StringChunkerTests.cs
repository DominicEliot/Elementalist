using Elementalist.Shared;
using Xunit;

namespace ElementalistTests.Shared;

public class StringChunkerTests
{
    [Fact]
    public void ChunkStringOnWords()
    {
        var testSentence = "The quick brown fox jumped over the lazy dog.";
        var chunked = testSentence.ChunkStringOnWords(12);

        Assert.True(chunked.All(chunk => chunk.Length <= 12));
        Assert.Equal("brown fox", chunked.ElementAt(1));
    }
}
