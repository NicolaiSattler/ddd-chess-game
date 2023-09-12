using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Determiners;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.BusinessRules;

//TODO: Unit test.
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
        var piece = _pieces.First(p => p?.Position == _command.StartPosition);

        try
        {
            var king = _pieces.First(p => p?.Type == PieceType.King && p.Color == piece?.Color);

            if (piece == null || king == null) return Enumerable.Empty<BusinessRuleViolation>();

            piece.Position = _command.EndPosition;

            var kingIsInCheck = Board.IsCheck((King)king, _pieces);

            if (kingIsInCheck)
            {
                return new List<BusinessRuleViolation>()
                {
                    new("King is in check, move is not allowed!")
                };
            }

            return Enumerable.Empty<BusinessRuleViolation>();

        }
        finally
        {
            piece!.Position = _command.StartPosition;
        }
    }
}