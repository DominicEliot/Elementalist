using NUnit.Framework;
using SorceryBot.Features.Card.Search;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static SorceryBot.Features.Card.GetCard;

namespace SorceryBot.Features.Card.Search.Tests;

[TestFixture()]
public class CardSearchQueryHandlerTests
{
    [Test()]
    public async Task HandleTestAsync()
    {
        var query = new CardSearchQuery("Pudge");
        var handler = new CardSearchQueryHandler(new FileCardRepository());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
    }
}
