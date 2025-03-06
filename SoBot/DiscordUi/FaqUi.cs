using Discord;
using Discord.Interactions;
using MediatR;
using SorceryBot.Infrastructure.DataAccess.CardData;

namespace SorceryBot.DiscordUi;

public class FaqUi(IMediator mediator, FaqRepoistory faqRepository) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator = mediator;
    private readonly FaqRepoistory _faqRepository = faqRepository;

    [ComponentInteraction("faq-*")]
    public async Task ShowFaq(string cardName)
    {
        var faqs = await _faqRepository.GetFaqs();
        if (faqs.TryGetValue(cardName, out var cardFaqs) is not true)
        {
            await RespondAsync($"No FAQ entries found for {cardName}");
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
