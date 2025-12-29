using Elementalist.Shared;
using Xunit;

namespace ElementalistTests.Shared;

public class StringChunkerTests
{
    [Fact]
    public void ChunkStringOnWordsTest()
    {
        var testSentence = "The quick brown fox jumps over the lazy dog.";
        var chunked = testSentence.ChunkStringOnWords(12);

        Assert.True(chunked.All(chunk => chunk.Length <= 12));
        Assert.Equal("brown fox", chunked.ElementAt(1));
    }

    [Fact]
    public void ChunkStringLargeChunkTest()
    {
        var testSentence = "The quick brown fox jumps over the lazy dog.";
        var chunked = testSentence.ChunkStringOnWords(int.MaxValue);

        Assert.Equal(testSentence, chunked.ElementAt(0));
    }
}
