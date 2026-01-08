using System.Text.RegularExpressions;
using Elementalist.DiscordUi;
using Elementalist.Features.Cards;
using Elementalist.Models;
using MediatR;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace ElementalistBot.DiscordUi;

public partial class CardMessageBotMentionCommand(IMediator mediator,
                                                  ILogger<CardMessageBotMentionCommand> logger,
                                                  CardDisplayService cardDisplayService,
                                                  GatewayClient client) : IMessageCreateGatewayHandler
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<CardMessageBotMentionCommand> _logger = logger;
    private readonly CardDisplayService _cardDisplayService = cardDisplayService;
    private readonly GatewayClient _client = client;

    /// <summary>
    /// This is the message event handler so that we can respond when people @ the bot with cards in the message.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async ValueTask HandleAsync(Message message)
    {
        if (message.Author.Id == _client.Id || message.Author.IsBot || message.Author.IsSystemUser == true)
        {
            return;
        }

        _logger.LogInformation("Parsing for cards from a tagged mention message: {message}", message.Content);
        var cardsToShow = await FindCardsInText(message.Content);
        if (!cardsToShow.Any())
        {
            _logger.LogWarning("No cards found in text '{message}'", message);
            return;
        }

        var responseMessage = await _cardDisplayService.CardInfoMessage(cardsToShow);

        await message.ReplyAsync(new ReplyMessageProperties
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
