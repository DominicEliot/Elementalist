using System.Text;

namespace Elementalist.Shared;

public static class StringChunker
{
    public static IEnumerable<string> ChunkStringOnWords(this string value, int maxLength)
    {
        if (value.Length <= maxLength)
        {
            return [value];
        }

        var words =  value.Split([' ']);
        var sb = new StringBuilder();
        int currentLength = 0;

        var chunkedList = new List<string>();

        foreach (var word in words)
        {
            if (currentLength + word.Length > maxLength)
            {
                var chunk = sb.ToString().Trim();
                chunkedList.Add(chunk);
                sb.Clear();
                currentLength = 0;
            }

            sb.Append(word).Append(' ');
            currentLength += word.Length + 1;
        }

        return chunkedList;
    }
}
