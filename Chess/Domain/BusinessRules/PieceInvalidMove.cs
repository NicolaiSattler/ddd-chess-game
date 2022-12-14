using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.BusinessRules;

public class PieceInvalidMove : BusinessRule
{
    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece>? _pieces;

    public PieceInvalidMove(TakeTurn command, IEnumerable<Piece>? pieces)
    {
        _command = command;
        _pieces = pieces;
    }

    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        var movingPiece = _pieces?.FirstOrDefault(p => p.Position == _command.StartPosition)
            ?? throw new InvalidOperationException($"No piece was found on position {_command.StartPosition}");
        var result = ValidateMovement(movingPiece);

        if (result != null)
        {
            return new List<BusinessRuleViolation> { result };
        }

        return Enumerable.Empty<BusinessRuleViolation>();
    }

    private BusinessRuleViolation? ValidateMovement(Piece? piece) => (piece?.Type) switch
    {
        PieceType.Pawn when !IsValidMove((Pawn)piece) => new("A pawn must attack a filled square."),
        _ when !PieceMovesToValidSquare(piece) => new("Piece must move to designated squares."),
        _ => null
    };

    private bool PieceMovesToValidSquare(Piece? piece)
    {
        var availableMoves = piece?.GetAttackRange();
        return availableMoves?.Any(square => square == _command.EndPosition) ?? false;
    }

    private bool IsValidMove(Pawn? pawn)
    {
        if (pawn == null) throw new ArgumentNullException(nameof(pawn));

        var availableMoves = pawn?.GetAttackRange();
        var attackMoves = availableMoves?.Where(m => m.File != pawn?.Position?.File);
        var attack = attackMoves?.FirstOrDefault(m => m == _command.EndPosition);

        if (attack != null)
        {
            return _pieces?.Any(p => p.Position == attack) ?? false;
        }

        return true;
    }
}