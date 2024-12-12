using NUnit.Framework;
using SorceryBot.Infrastructure.DataAccess.CardData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SorceryBot.Infrastructure.DataAccess.CardData.Tests;

[TestFixture()]
public class FaqRepoistoryTests
{
    [Test()]
    public async Task FaqRepoistoryTestAsync()
    {
        var repo = new FaqRepoistory();
        await repo.LoadAsync();
    }
}
