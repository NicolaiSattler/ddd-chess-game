using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Determiners;

namespace Chess.Domain.BusinessRules;

public class PieceIsBlocked : BusinessRule
{
    private const string PieceIsBlockedViolation = "Piece is blocked!";

    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece> _pieces;

    public PieceIsBlocked(TakeTurn command, IEnumerable<Piece> pieces)
    {
        _command = Guard.Against.Null<TakeTurn>(command, nameof(command));
        _pieces = Guard.Against.Null<IEnumerable<Piece>>(pieces, nameof(pieces));

        Guard.Against.InvalidInput<TakeTurn>(command, nameof(command), c => c.StartPosition != null, "Start postion cannot be null");
        Guard.Against.InvalidInput<TakeTurn>(command, nameof(command), c => c.EndPosition != null, "End postion cannot be null");
    }

    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        var movingPiece = _pieces.FirstOrDefault(p => p.Position == _command.StartPosition)
            ?? throw new InvalidOperationException($"No piece was found at {_command.StartPosition}");

        var pieceIsBlocked = EndPositionIsBlocked(movingPiece)
            || (Board.DirectionIsObstructed(_pieces, _command.StartPosition, _command.EndPosition));

        return pieceIsBlocked
            ? new List<BusinessRuleViolation>() { new(PieceIsBlockedViolation) }
            : Enumerable.Empty<BusinessRuleViolation>();
    }

    private bool EndPositionIsBlocked(Piece movingPiece) =>
        _pieces.Any(p => p.Position == _command.EndPosition && p.Color == movingPiece.Color);
}
