using FluentResults;

namespace Chess.Domain.BusinessRules;

public class PieceIsBlockedError: Error
{
    private const string PieceIsBlockedMessage = "Piece is blocked!";

    public PieceIsBlockedError() : base(PieceIsBlockedMessage) { }
}

