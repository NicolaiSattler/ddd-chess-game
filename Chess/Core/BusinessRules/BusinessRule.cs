using System.Collections.Generic;

namespace Chess.Core.BusinessRules;

public abstract class BusinessRule
{
    public abstract IEnumerable<BusinessRuleViolation> CheckRule();
}
