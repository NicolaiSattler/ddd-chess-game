using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Determiners;
using Chess.Domain.Entities.Pieces;

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

    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        //Copy collection to test the move
        var pieces = _pieces.ToList();
        var piece = pieces.First(p => p?.Position == _command.StartPosition);
        var king = pieces.First(p => p?.Type == PieceType.King && p.Color == piece?.Color);

        if (piece == null || king == null) return Enumerable.Empty<BusinessRuleViolation>();

        piece.Position = _command.EndPosition;

        var kingIsInCheck = Board.IsCheck((King)king, pieces);

        if (kingIsInCheck)
        {
            return new List<BusinessRuleViolation>()
            {
                new("King is in check, move is not allowed!")
            };
        }

        return Enumerable.Empty<BusinessRuleViolation>();
    }
}
