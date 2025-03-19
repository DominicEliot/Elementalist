using FluentValidation;
using MediatR;
using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Shared;

namespace Elementalist.Features.Card;

public record GetCardsQuery : IQuery<IEnumerable<Models.Card>>
{
    public string? CardNameContains { get; set; }
    public string? TextContains { get; set; }
    public string? ElementsContain { get; set; }
}

public class GetCardsQueryValidator : AbstractValidator<GetCardsQuery>
{
    public GetCardsQueryValidator()
    {
        RuleFor(q => q)
            .Must(q => q.CardNameContains is not null || q.TextContains is not null || q.ElementsContain is not null)
            .WithMessage("You must pass at least one value to the query");

        RuleFor(q => q.CardNameContains)
            .NotEmpty()
            .When(q => q.CardNameContains != null);

        RuleFor(q => q.TextContains)
            .NotEmpty()
            .When(q => q.TextContains != null);

        RuleFor(q => q.ElementsContain)
            .NotEmpty()
            .When(q => q.ElementsContain != null);
    }
}

public class GetCardsQueryHandler(ICardRepository cardRepository) : IRequestHandler<GetCardsQuery, IEnumerable<Models.Card>>
{
    private readonly ICardRepository _cardRepository = cardRepository;

    public async Task<IEnumerable<Models.Card>> Handle(GetCardsQuery request, CancellationToken cancellationToken)
    {
        var cards = await _cardRepository.GetCards();

        if (request.CardNameContains != null)
        {
            cards = cards.Where(c => c.Name.Contains(request.CardNameContains, StringComparison.OrdinalIgnoreCase));
        }

        if (request.ElementsContain != null)
        {
            cards = cards.Where(c => c.Elements.Contains(request.ElementsContain, StringComparison.OrdinalIgnoreCase));
        }

        if (request.TextContains != null)
        {
            cards = cards.Where(c =>
                c.Guardian.RulesText.Contains(request.TextContains, StringComparison.OrdinalIgnoreCase) ||
                c.SubTypes.Contains(request.TextContains, StringComparison.OrdinalIgnoreCase) ||
                c.Guardian.Type.Contains(request.TextContains, StringComparison.OrdinalIgnoreCase)
            );
        }

        return cards;
    }
}
