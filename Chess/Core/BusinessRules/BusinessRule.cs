using FluentResults;

namespace Chess.Core.BusinessRules;

public abstract class BusinessRule
{
    public abstract Result  CheckRule();
}
