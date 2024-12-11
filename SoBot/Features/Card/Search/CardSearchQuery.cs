using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SorceryBot.Shared;

namespace SorceryBot.Features.Card.Search;
public record CardSearchQuery : IQuery<IEnumerable<CardDto>>
{
}
