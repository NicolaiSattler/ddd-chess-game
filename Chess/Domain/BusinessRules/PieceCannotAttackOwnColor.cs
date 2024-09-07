using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using FluentResults;

namespace Chess.Domain.BusinessRules;

public class PieceCannotAttackOwnColor : BusinessRule
{
    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece> _pieces;

    public PieceCannotAttackOwnColor(TakeTurn command, IEnumerable<Piece> pieces)
    {
        _command = Guard.Against.Null<TakeTurn>(command, nameof(command));
        _pieces = Guard.Against.Null<IEnumerable<Piece>>(pieces, nameof(pieces));
    }

    public override Result CheckRule()
    {
        return ValidateMovingPiece()
                    .Bind(movingPiece =>
                    {
                        var availableMoves = movingPiece.GetAttackRange();
                        var isValidSquare = availableMoves.Any(m => m == _command.EndPosition);
                        var targetPiece = _pieces.FirstOrDefault(p => p.Position == _command.EndPosition);

                        if (targetPiece?.Color == movingPiece.Color) return new PieceCannotAttackOwnColorError();
                        
                        return Result.Ok();
                    });
    }

    private Result<Piece> ValidateMovingPiece()
    {
        var movingPiece = _pieces.FirstOrDefault(p => p.Position == _command.StartPosition);

        if (movingPiece == null)
        {
            return new MovingPieceNotFoundError();
        }

        return Result.Ok(movingPiece);
    }
}
