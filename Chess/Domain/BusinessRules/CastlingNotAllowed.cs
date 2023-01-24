using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Model;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.BusinessRules;

/// <summary>
/// Validating if purposed Castling move is allowd. The following criteria must be met:
/// - Neither the king nor the rook has previously moved during the game.
/// - There are no pieces between the king and the rook.
/// - The king is not in check and does not pass through or land on any square attacked by an enemy piece.
/// </summary>
public class CastlingNotAllowed : BusinessRule
{
    private const string RuleViolationMessage = "The Castling move is not allowed, either the King or Rook has been moved,"
                                              + " the King is in check or a piece is standing between the King and Rook.";
    private readonly TakeTurn? _command;
    private readonly IEnumerable<Turn>? _turns;
    private readonly IEnumerable<Piece>? _pieces;

    public CastlingNotAllowed(TakeTurn? command, IEnumerable<Piece>? pieces, IEnumerable<Turn>? turns)
    {
        Guard.Against.Null<TakeTurn?>(command, nameof(command));
        Guard.Against.Null<IEnumerable<Piece>?>(pieces, nameof(pieces));
        Guard.Against.Null<IEnumerable<Turn>?>(turns, nameof(turns));

        _turns = turns;
        _pieces = pieces;
        _command = command;
    }

    //TODO: unit test required
    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        var movingPiece = _pieces?.FirstOrDefault(p => p.Position == _command?.StartPosition);
        var king = _pieces?.FirstOrDefault(p => p.Type == PieceType.King && p.Color == movingPiece?.Color) as King;
        var rank = movingPiece?.Color == Color.Black ? 8 : 1;
        var file = (int?)_command?.StartPosition?.File < (int?)_command?.EndPosition?.File ? File.A : File.H;

        var rookHasMoved = _turns?.Any(p => p.PieceType == PieceType.Rook && p.StartPosition == new Square(file, rank)) ?? true;
        var kingHasMoved = _turns?.Any(p => p.PieceType == PieceType.King) ?? true;
        var kingIsInCheck = Board.IsCheck(king, _pieces);
        var moveIsBlocked = Board.DirectionIsObstructed(_pieces, _command?.StartPosition, _command?.EndPosition) ?? false;

        if (rookHasMoved || kingHasMoved || moveIsBlocked || kingIsInCheck)
        {
            return new List<BusinessRuleViolation> { new(RuleViolationMessage) };
        }

        return Enumerable.Empty<BusinessRuleViolation>();
    }
}
