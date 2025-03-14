using Discord;
using Discord.Interactions;
using MediatR;
using Elementalist.Infrastructure.DataAccess.CardData;

namespace Elementalist.DiscordUi;

public class FaqUi(FaqRepoistory faqRepository) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly FaqRepoistory _faqRepository = faqRepository;

    [ComponentInteraction("faq-*")]
    public async Task ShowFaq(string cardName)
    {
        var faqs = await _faqRepository.GetFaqs();
        if (faqs.TryGetValue(cardName, out var cardFaqs) is not true)
        {
            await RespondAsync($"No FAQ entries found for {cardName}", ephemeral: true);
            return;
        }

        var faqEmbed = new EmbedBuilder().WithTitle($"{cardName} FAQs");

        foreach (var faq in cardFaqs)
        {
            faqEmbed.AddField(faq.QuestionText, faq.AnswerText);
        }

        await RespondAsync(embed: faqEmbed.Build());
    }
}
