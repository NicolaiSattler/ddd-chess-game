using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.BusinessRules;

public class PieceCannotAttackOwnColor : BusinessRule
{
    private readonly TakeTurn? _command;
    private readonly IEnumerable<Piece>? _pieces;

    public PieceCannotAttackOwnColor(TakeTurn? command, IEnumerable<Piece>? pieces)
    {
        Guard.Against.Null<TakeTurn?>(command, nameof(command));
        Guard.Against.Null<IEnumerable<Piece>?>(pieces, nameof(pieces));

        _command = command;
        _pieces = pieces;
    }

    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        var result = new List<BusinessRuleViolation>();
        var movingPiece = _pieces?.FirstOrDefault(p => p.Position == _command?.StartPosition)
            ?? throw new InvalidOperationException($"No piece was found at {_command?.StartPosition}");

        var availableMoves = movingPiece.GetAttackRange();
        var isValidSquare = availableMoves.Any(m => m == _command?.EndPosition);
        var targetPiece = _pieces?.FirstOrDefault(p => p.Position == _command?.EndPosition);

        if (targetPiece?.Color == movingPiece.Color)
        {
            return new List<BusinessRuleViolation> { new("Cannot target piece of same color.")};
        }
        else return Enumerable.Empty<BusinessRuleViolation>();
    }
}