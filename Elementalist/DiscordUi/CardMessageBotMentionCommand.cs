using System.Text.RegularExpressions;
using Elementalist.DiscordUi;
using Elementalist.Features.Cards;
using Elementalist.Models;
using MediatR;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace ElementalistBot.DiscordUi;

public partial class CardMessageBotMentionCommand(IMediator mediator, CardArtService cardArtService) : IMessageCreateGatewayHandler
{
    private readonly IMediator _mediator = mediator;
    private readonly CardArtService _cardArtService = cardArtService;

    /// <summary>
    /// This is the message event handler so that we can respond when people @ the bot with cards in the message.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async ValueTask HandleAsync(Message message)
    {
        var cardsToShow = await FindCardsInText(message.Content);
        if (!cardsToShow.Any())
        {            
            return;
        }

        var responseMessage = CardDisplay.CardInfoMessage(cardsToShow, _cardArtService);

        await message.ReplyAsync(new()
        {
            Components = responseMessage.Components,
            Content = responseMessage.Content,
            Flags = responseMessage.Flags,
            Embeds = responseMessage.Embeds
        });
    }

    private async Task<IEnumerable<Card>> FindCardsInText(string textToSearch)
    {
        var cardsToShow = new List<Card>();
        foreach (Match match in CardNameInTextRegex().Matches(textToSearch))
        {
            var cardName = match.Groups[1].Value;

            var query = new GetCardsQuery() { CardNameContains = cardName };
            var cards = await _mediator.Send(query);

            cardsToShow.AddRange(cards);
        }

        return cardsToShow;
    }

    [GeneratedRegex(@"[([]+(.*?)[)\]]+")]
    private static partial Regex CardNameInTextRegex();
}
