using FluentResults;

namespace Chess.Domain.BusinessRules;

public class PieceCannotAttackOwnColorError: Error
{
    private const string ErrorMessage = "Piece cannot attack it's own color";

    public PieceCannotAttackOwnColorError(): base(ErrorMessage) { }
}

