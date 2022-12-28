using System.Collections.Generic;
using System.Linq;

namespace Chess.Core.BusinessRules;

[Serializable]
public class BusinessRuleViolationException : Exception
{
    public IEnumerable<BusinessRuleViolation> Violations { get; }

    public BusinessRuleViolationException()
    {
        Violations = new List<BusinessRuleViolation>();
    }

    public BusinessRuleViolationException(IEnumerable<BusinessRuleViolation> violations)
        : base ($"Rule Violations: {violations.Count()} violations have been detected.")
    {
        Violations = violations;
    }

    public BusinessRuleViolationException(IEnumerable<BusinessRuleViolation> violations, Exception innerException)
        : base($"Rule Violations: {violations.Count()} violations have been detected.", innerException)
    {
        Violations = violations;
    }
}