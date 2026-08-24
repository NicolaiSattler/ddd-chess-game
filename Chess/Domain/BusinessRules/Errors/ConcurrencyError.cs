using FluentResults;

namespace Chess.Domain.BusinessRules;

public class ConcurrencyError : Error
{
    private const string ErrorMessage = "Aggregate version mismatch. The aggregate has been modified by another request.";

    public ConcurrencyError() : base(ErrorMessage) { }
}
