using Elementalist.Infrastructure.DataAccess.CardData;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class FaqUi(FaqRepoistory faqRepository) : ComponentInteractionModule<ButtonInteractionContext>
{
    private readonly FaqRepoistory _faqRepository = faqRepository;

    [ComponentInteraction("faq")]
    public async Task ShowFaq(string cardName)
    {
        await RespondWithFaq(cardName);
    }

    [SlashCommand("faq", "Shows any FAQs for the input card.")]
    public async Task CardSearchByName([SlashCommandParameter(AutocompleteProviderType = typeof(CardAutoCompleteHandler))] string cardName, bool privateMessage = false)
    {
        await RespondWithFaq(cardName, privateMessage);
    }

    private async Task RespondWithFaq(string cardName, bool privateMessage = false)
    {
        var message = new InteractionMessageProperties();
        if (privateMessage) message.Flags = NetCord.MessageFlags.Ephemeral;

        var faqs = await _faqRepository.GetFaqs();
        if (faqs.TryGetValue(cardName, out var cardFaqs) is not true)
        {
            message.Content = $"No FAQ entries found for {cardName}";
            message.Flags = NetCord.MessageFlags.Ephemeral;
            await RespondAsync(InteractionCallback.Message(message));
        }

        var faqEmbed = new EmbedProperties().WithTitle($"{cardName} FAQs");

        foreach (var faq in cardFaqs)
        {
            faqEmbed.AddFields(new EmbedFieldProperties().WithName(faq.QuestionText).WithValue(faq.AnswerText));
        }
        message.Embeds = [faqEmbed];
        await RespondAsync(InteractionCallback.Message(message));
    }
}
