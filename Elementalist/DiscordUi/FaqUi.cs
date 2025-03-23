using Discord;
using Discord.Interactions;
using MediatR;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Features.Card;

namespace Elementalist.DiscordUi;

public class FaqUi(FaqRepoistory faqRepository) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly FaqRepoistory _faqRepository = faqRepository;

    [ComponentInteraction("faq-*")]
    public async Task ShowFaq(string cardName)
    {
        await RespondWithFaq(cardName);
    }

    [SlashCommand("faq", "Shows any FAQs for the input card.")]
    public async Task CardSearchByName([Autocomplete<CardAutoCompleteHandler>()] string cardName, bool privateMessage = false)
    {
        await RespondWithFaq(cardName, privateMessage);
    }

    private async Task RespondWithFaq(string cardName, bool privateMessage = false)
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

        await RespondAsync(embed: faqEmbed.Build(), ephemeral: privateMessage);
    }
}
