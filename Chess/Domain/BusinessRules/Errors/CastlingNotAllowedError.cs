using FluentResults;

namespace Chess.Domain.BusinessRules;

public class CastlingNotAllowedError: Error
{
    private const string Error = "The Castling move is not allowed, either the King or Rook has been moved,"
                               + " the King is in check or a piece is standing between the King and Rook.";

    public CastlingNotAllowedError() : base(Error) { }
}


