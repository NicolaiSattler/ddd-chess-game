using FluentResults;

namespace Chess.Domain.BusinessRules;

public class KingNotFoundError : Error
{
    private const string Error = "King cannot be found!";

    public KingNotFoundError() : base(Error) { }
}


