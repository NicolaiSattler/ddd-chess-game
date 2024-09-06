using FluentResults;

namespace Chess.Domain.BusinessRules;

public class MovingPieceNotFoundError : Error
{
    private const string Error = "Moving piece cannot be found!";

    public MovingPieceNotFoundError() : base(Error) { }
}
