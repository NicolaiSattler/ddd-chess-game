using System.Collections.Generic;
using Chess.Core.BusinessRules;

namespace Chess.Domain.BusinessRules;

public class PieceIsBlocked : BusinessRule
{
    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        throw new NotImplementedException();
    }
}