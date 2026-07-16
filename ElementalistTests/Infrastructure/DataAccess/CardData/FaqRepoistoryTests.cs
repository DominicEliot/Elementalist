using ElementalistTests;
using Xunit;

namespace Elementalist.Infrastructure.DataAccess.CardData.Tests;

public class FaqRepoistoryTests
{
    [Fact]
    public async Task FaqRepoistoryTestAsync()
    {
        var repo = new CsvFaqRepository(TestHelpers.MemoryCache, TestHelpers.RefreshOptions);
        var faqs = await repo.GetFaqs(CancellationToken.None);

        var wallOfIceFaq = faqs["Wall of Ice"];

        Assert.Equal(2, wallOfIceFaq.Count());

        foreach (var faq in wallOfIceFaq)
        {
            Console.WriteLine(faq);
        }
    }
}
