using Elementalist.Shared;
using ElementalistBot.Infrastructure.DataAccess.Rules;
using ElementalistBot.Models;
using MediatR;

namespace ElementalistBot.Features.Rules;
public record GetRulesQuery(string RuleName) : IQuery<IEnumerable<CodexEntry>>;

public class GetRulesQueryHandler(CodexCsvRulesRepository rulesRepository) : IRequestHandler<GetRulesQuery, IEnumerable<CodexEntry>>
{
    public async Task<IEnumerable<CodexEntry>> Handle(GetRulesQuery request, CancellationToken cancellationToken)
    {
        return (await rulesRepository.GetRules())
            .Where(r => r.Title.Equals(request.RuleName, StringComparison.OrdinalIgnoreCase));
    }
}
