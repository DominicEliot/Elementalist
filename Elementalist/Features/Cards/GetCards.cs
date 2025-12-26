using Elementalist.Infrastructure.DataAccess.CardData;
using Elementalist.Shared;
using FluentValidation;
using MediatR;

namespace Elementalist.Features.Cards;

public record GetCardsQuery : IQuery<IEnumerable<Models.Card>>
{
    public string? CardNameContains { get; init; }
    public string? TextContains { get; init; }
    public string? TypeContains { get; init; }
    public string? ElementsContain { get; init; }

    public override string ToString() => $"{CardNameContains} {TextContains} {TypeContains} {ElementsContain}".Trim();
}

public class GetCardsQueryValidator : AbstractValidator<GetCardsQuery>
{
    public GetCardsQueryValidator()
    {
        RuleFor(q => q)
            .Must(q => q.CardNameContains is not null || q.TextContains is not null || q.ElementsContain is not null || q.TypeContains is not null)
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
            var splitValues = request.CardNameContains.Replace(",", "").Replace(".", "").Split(['-', ' ']);

            var exactMatch = cards.SingleOrDefault(c => c.Name.Equals(request.CardNameContains, StringComparison.OrdinalIgnoreCase));

            cards = (exactMatch is not null) ? [exactMatch] : cards.Where(c => splitValues.All(s => c.Name.Contains(s, StringComparison.OrdinalIgnoreCase)));
        }

        if (request.ElementsContain != null)
        {
            cards = cards.Where(c => c.Elements.Contains(request.ElementsContain, StringComparison.OrdinalIgnoreCase));
        }

        if (request.TextContains != null)
        {
            var searchTerms = request.TextContains.Split(' ');

            cards = cards.Where(c => DoesCardHaveAllTerms(c, searchTerms));
        }

        if (request.TypeContains != null)
        {
            var searchTerms = request.TypeContains.Split(" ");

            cards = cards.Where(c => DoesCardHaveAllTypes(c, searchTerms));
        }

        return cards;
    }

    private static bool DoesCardHaveAllTerms(Models.Card card, string[] searchTerms)
    {
        var matchCount = searchTerms.Count(singleWord => card.Guardian?.RulesText.Contains(singleWord, StringComparison.OrdinalIgnoreCase) == true);

        return matchCount >= searchTerms.Length;
    }

    private static bool DoesCardHaveAllTypes(Models.Card card, string[] searchTerms)
    {
        var matchCount = searchTerms.Count(singleWord => card.Guardian?.Type?.Contains(singleWord, StringComparison.OrdinalIgnoreCase) == true)
            + searchTerms.Count(singleWord => card.SubTypes?.Contains(singleWord, StringComparison.OrdinalIgnoreCase) == true);

        return matchCount >= searchTerms.Length;
    }
}
