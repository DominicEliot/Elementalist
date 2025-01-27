using SorceryBot.Infrastructure.DataAccess.CardData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SorceryBot.Infrastructure.DataAccess.CardData.Tests;

public class FaqRepoistoryTests
{
    [Fact]
    public async Task FaqRepoistoryTestAsync()
    {
        var repo = new FaqRepoistory();
        await repo.LoadAsync();
    }
}
