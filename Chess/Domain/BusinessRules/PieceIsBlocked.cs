using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Model;

namespace Chess.Domain.BusinessRules;

public class PieceIsBlocked : BusinessRule
{
    private const string PieceIsBlockedViolation = "Piece is blocked!";
    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece>? _pieces;

    public PieceIsBlocked(TakeTurn command, IEnumerable<Piece>? pieces)
    {
        _command = command;
        _pieces = pieces;
    }

    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        _ = _command.StartPosition ?? throw new InvalidOperationException("Start position cannot be null");
        _ = _command.EndPosition ?? throw new InvalidOperationException("End position cannot be null");

        var movingPiece = _pieces?.FirstOrDefault(p => p.Position == _command.StartPosition)
                        ?? throw new InvalidOperationException($"No piece was found at {_command.StartPosition}");

        var pieceIsBlocked = EndPositionIsBlocked(movingPiece)
            || Board.DirectionIsObstructed(_pieces, _command.StartPosition, _command.EndPosition);

        return pieceIsBlocked
            ? new List<BusinessRuleViolation>() { new(PieceIsBlockedViolation) }
            : Enumerable.Empty<BusinessRuleViolation>();
    }

    private bool EndPositionIsBlocked(Piece movingPiece) =>
        _pieces?.Any(p => p.Position == _command.EndPosition && p.Color == movingPiece.Color) ?? false;
}