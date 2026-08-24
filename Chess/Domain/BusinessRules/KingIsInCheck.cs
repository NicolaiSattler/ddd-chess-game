using System.Collections.Generic;
using System.Linq;

using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Determiners;
using Chess.Domain.Entities.Pieces;

using FluentResults;

namespace Chess.Domain.BusinessRules;

public class KingIsInCheck(TakeTurn command, IEnumerable<Piece> pieces) : BusinessRule
{
    private readonly TakeTurn _command = command;
    private readonly IEnumerable<Piece> _pieces = pieces;

    public override Result CheckRule()
    {
        var movingPiece = _pieces.FirstOrDefault(p => p.Position == _command.StartPosition);

        if (movingPiece == null)
        {
            return new MovingPieceNotFoundError();
        }

        var king = _pieces.First(p => p?.Type == PieceType.King && p.Color == movingPiece?.Color);

        return WouldKingBeInCheckAfterMove((King)king, movingPiece)
            ? new KingIsInCheckError()
            : Result.Ok();
    }

    private bool WouldKingBeInCheckAfterMove(King king, Piece movingPiece)
    {
        var testBoard = _pieces.ToList();
        var testMovingPiece = testBoard.First(p => p.Id == movingPiece.Id);
        testMovingPiece.MoveTo(_command.EndPosition);

        var kingForTest = movingPiece.Id == king.Id ? (King)testMovingPiece : king;

        return Board.IsCheck(kingForTest, testBoard);
    }
}