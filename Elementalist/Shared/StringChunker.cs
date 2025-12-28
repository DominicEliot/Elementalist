using System.Text;

namespace Elementalist.Shared;

public static class StringChunker
{
    public static IEnumerable<string> ChunkStringOnWords(this string value, int maxLength)
    {
        var words =  value.Split([' ']);
        var sb = new StringBuilder();
        int currentLength = 0;

        foreach (var word in words)
        {
            if (currentLength + word.Length < maxLength)
            {
                sb.Append(word).Append(' ');
                currentLength += word.Length + 1;
                continue;
            }

            var wordChunk = sb.ToString().Trim();
            sb = new StringBuilder();
            sb.Append(word).Append(' ');
            currentLength = word.Length + 1;

            yield return wordChunk;
        }
    }
}
