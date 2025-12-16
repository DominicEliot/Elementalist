using Elementalist.Shared;
using ElementalistBot.Infrastructure.DataAccess.Rules;
using ElementalistBot.Models;
using MediatR;

namespace ElementalistBot.Features.Rules;
public record GetRulesQuery(string RuleName) : IQuery<IEnumerable<CodexEntry>>;

public class GetCardsQueryHandler(CodexCsvRulesRepository rulesRepository) : IRequestHandler<GetRulesQuery, IEnumerable<CodexEntry>>
{
    public async Task<IEnumerable<CodexEntry>> Handle(GetRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = (await rulesRepository.GetRules()).Where(r => r.Title.Equals(request.RuleName, StringComparison.OrdinalIgnoreCase));

        return rules;
    }
}
