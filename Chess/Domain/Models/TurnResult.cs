using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Events;

namespace Chess.Domain.Models;

public record TurnResult
{
    public IEnumerable<BusinessRuleViolation> Violations { get; init; } = Enumerable.Empty<BusinessRuleViolation>();
    public MatchResult MatchResult { get; init; }
}
