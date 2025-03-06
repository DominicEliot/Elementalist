using Xunit;

namespace SorceryBot.Infrastructure.DataAccess.CardData.Tests;

public class FaqRepoistoryTests
{
    [Fact]
    public async Task FaqRepoistoryTestAsync()
    {
        var repo = new FaqRepoistory();
        var faqs = await repo.GetFaqs();

        var abundanceFaqs = faqs["Abundance"];

        Assert.Equal(3, abundanceFaqs.Count());

        foreach (var faq in abundanceFaqs)
        {
            Console.WriteLine(faq);
        }
    }
}
