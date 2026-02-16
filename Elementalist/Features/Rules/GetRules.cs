using Elementalist.Infrastructure.DataAccess.Rules;
using Elementalist.Models;
using Elementalist.Shared;
using MediatR;

namespace Elementalist.Features.Rules;
public record GetRulesQuery(string RuleName) : IQuery<IEnumerable<CodexEntry>>;

public class GetRulesQueryHandler(IRulesRepository rulesRepository) : IRequestHandler<GetRulesQuery, IEnumerable<CodexEntry>>
{
    public async Task<IEnumerable<CodexEntry>> Handle(GetRulesQuery request, CancellationToken cancellationToken)
    {
        return (await rulesRepository.GetRules(cancellationToken))
            .Where(r => r.Title.Equals(request.RuleName, StringComparison.OrdinalIgnoreCase));
    }
}
