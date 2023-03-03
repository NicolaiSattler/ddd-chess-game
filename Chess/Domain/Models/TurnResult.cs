using System.Collections.Generic;
using Chess.Core.BusinessRules;
using Chess.Domain.Events;

namespace Chess.Domain.Models;

public record TurnResult
{
    public IEnumerable<BusinessRuleViolation>? Violations { get; init; }
    public MatchResult MatchResult { get; init; }
}
