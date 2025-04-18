using Elementalist.Shared;
using ElementalistBot.Infrastructure.DataAccess.Rules;
using ElementalistBot.Models;
using MediatR;

namespace ElementalistBot.Features.Rules;
public record GetRulesQuery(string RuleName) : IQuery<IEnumerable<Rule>>;

public class GetCardsQueryHandler(RulesRepository rulesRepository) : IRequestHandler<GetRulesQuery, IEnumerable<Rule>>
{
    public async Task<IEnumerable<Rule>> Handle(GetRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = (await rulesRepository.GetRules()).Where(r => r.Name.Equals(request.RuleName, StringComparison.OrdinalIgnoreCase));

        return rules;
    }
}
