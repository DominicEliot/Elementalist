using Elementalist.Infrastructure.DataAccess.CardData;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

namespace Elementalist.DiscordUi;

public class FaqUi(IFaqRepository faqRepository) : ComponentInteractionModule<ButtonInteractionContext>
{
    private readonly IFaqRepository _faqRepository = faqRepository;

    [ComponentInteraction("faq")]
    public async Task ShowFaq(string cardName)
    {
        var message = await FaqUiHelper.CreateFaqMessage(cardName, _faqRepository, CancellationToken.None);
        await RespondAsync(InteractionCallback.Message(message));
    }
}

public class FaqSlashCommand(IFaqRepository faqRepository) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IFaqRepository _faqRepository = faqRepository;

    [SlashCommand("faq", "Shows any FAQs for the input card.")]
    public async Task CardSearchByName([SlashCommandParameter(AutocompleteProviderType = typeof(CardAutoCompleteHandler))] string cardName, bool privateMessage = false)
    {
        var message = await FaqUiHelper.CreateFaqMessage(cardName, _faqRepository, CancellationToken.None, privateMessage);
        await RespondAsync(InteractionCallback.Message(message));
    }
}

public static class FaqUiHelper
{
    internal static async Task<InteractionMessageProperties> CreateFaqMessage(string cardName, IFaqRepository faqRepository, CancellationToken ct, bool privateMessage = false)
    {
        var message = new InteractionMessageProperties();
        if (privateMessage) message.Flags = NetCord.MessageFlags.Ephemeral;

        var faqs = await faqRepository.GetFaqs(ct);
        if (faqs.TryGetValue(cardName, out var cardFaqs) is not true)
        {
            message.Content = $"No FAQ entries found for {cardName}";
            message.Flags = NetCord.MessageFlags.Ephemeral;
            return message;
        }

        var faqEmbed = new EmbedProperties().WithTitle($"{cardName} FAQs");

        foreach (var faq in cardFaqs)
        {
            faqEmbed.AddFields(new EmbedFieldProperties().WithName(faq.QuestionText).WithValue(faq.AnswerText));
        }
        message.Embeds = [faqEmbed];
        return message;
    }
}
