using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Entities;

namespace Chess.Domain.BusinessRules;

public class TurnExpired : BusinessRule
{
    private const string RuleViolationMessage = "Maximum time of turn has succeeded";
    private readonly Turn _currentTurn;
    private readonly TimeSpan _maxTurnLength;

    public TurnExpired(Turn currentTurn, TimeSpan maxTurnLength)
    {
        Guard.Against.Null<Turn>(currentTurn, nameof(currentTurn));
        Guard.Against.Zero(maxTurnLength, nameof(maxTurnLength));

        _currentTurn = currentTurn;
        _maxTurnLength = maxTurnLength;
    }

    //TODO: unit tests
    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        var duration = DateTime.Now.Subtract(_currentTurn.StartTime);

        if (duration > _maxTurnLength)
        {
            return new List<BusinessRuleViolation> { new(RuleViolationMessage) };
        }

        return Enumerable.Empty<BusinessRuleViolation>();
    }
}