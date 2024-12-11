using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SorceryBot.Features.Card.Search;
public record CardSearchQuery : IRequest<IEnumerable<CardDto>>
{
}
