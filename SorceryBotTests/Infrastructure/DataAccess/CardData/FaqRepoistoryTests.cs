using Xunit;

namespace SorceryBot.Infrastructure.DataAccess.CardData.Tests;

public class FaqRepoistoryTests
{
    [Fact]
    public async Task FaqRepoistoryTestAsync()
    {
        var repo = new FaqRepoistory();
        var faqs = await repo.GetFaqs();

        var wallOfIceFaq = faqs["Wall of Ice"];

        Assert.Equal(4, wallOfIceFaq.Count());

        foreach (var faq in wallOfIceFaq)
        {
            Console.WriteLine(faq);
        }
    }
}
