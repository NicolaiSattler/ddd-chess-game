using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Determiners;
using FluentResults;

namespace Chess.Domain.BusinessRules;

public class PieceIsBlocked : BusinessRule
{
    private const string StartPositionIsNull = "Start postion cannot be null";
    private const string EndPositionIsNull = "End postion cannot be null";

    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece> _pieces;

    public PieceIsBlocked(TakeTurn command, IEnumerable<Piece> pieces)
    {
        _command = Guard.Against.Null<TakeTurn>(command, nameof(command));
        _pieces = Guard.Against.Null<IEnumerable<Piece>>(pieces, nameof(pieces));

        Guard.Against.InvalidInput<TakeTurn>(command, nameof(command), c => c.StartPosition != null, StartPositionIsNull);
        Guard.Against.InvalidInput<TakeTurn>(command, nameof(command), c => c.EndPosition != null, EndPositionIsNull);
    }

    public override Result CheckRule()
    {
        return ValidateMovingPiece().Bind((movingPiece) => {

            var pieceIsBlocked = EndPositionIsBlocked(movingPiece)
            || (Board.DirectionIsObstructed(_pieces, _command.StartPosition, _command.EndPosition));

            if (pieceIsBlocked) return new PieceIsBlockedError();

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

    private bool EndPositionIsBlocked(Piece movingPiece) =>
        _pieces.Any(p => p.Position == _command.EndPosition && p.Color == movingPiece.Color);
}
