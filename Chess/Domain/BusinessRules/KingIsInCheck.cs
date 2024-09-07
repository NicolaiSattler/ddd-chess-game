using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Determiners;
using Chess.Domain.Entities.Pieces;
using FluentResults;

namespace Chess.Domain.BusinessRules;

public class KingIsInCheck : BusinessRule
{
    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece> _pieces;

    public KingIsInCheck(TakeTurn command, IEnumerable<Piece> pieces)
    {
        _command = command;
        _pieces = pieces;
    }

    public override Result CheckRule()
    {
        //Copy collection to test the move
        var pieces = _pieces.ToList();

        return ValidateMovingPiece(pieces).Bind(movingPiece => 
        {
            var king = pieces.First(p => p?.Type == PieceType.King && p.Color == movingPiece?.Color);
    
            movingPiece.Position = _command.EndPosition;
    
            return Board.IsCheck((King)king, pieces)
                ?  new KingIsInCheckError()
                : Result.Ok();
        });
    }

    private Result<Piece> ValidateMovingPiece(IEnumerable<Piece> pieces)
    {
        var movingPiece = pieces.FirstOrDefault(p => p.Position == _command.StartPosition);

        if (movingPiece == null)
        {
            return new MovingPieceNotFoundError();
        }

        return Result.Ok(movingPiece);
    }
}
