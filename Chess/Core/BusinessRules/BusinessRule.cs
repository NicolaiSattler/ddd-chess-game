using System.Collections.Generic;

namespace Chess.Core.BusinessRules;

public abstract class BusinessRule
{
    public abstract IEnumerable<BusinessRuleViolation> CheckRule();

    public BusinessRule And(BusinessRule rule)
    {
        return new AndBusinessRule(this, rule);
    }
    public static BusinessRule operator &(BusinessRule firstRule, BusinessRule secondRule)
    {
        return new AndBusinessRule(firstRule, secondRule);
    }
}
