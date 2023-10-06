using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Determiners;

namespace Chess.Domain.Models;

public record TurnResult
{
    public bool IsEnPassant { get; init; }
    public CastlingType CastlingType { get; init; }
    public bool IsPromotion { get; init; }
    public IEnumerable<BusinessRuleViolation> Violations { get; init; } = Enumerable.Empty<BusinessRuleViolation>();
}
