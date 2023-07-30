using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Determiners;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.BusinessRules;

/// <summary>
/// Validating if purposed Castling move is allowed. The following criteria must be met:
/// - Neither the king nor the rook has previously moved during the game.
/// - There are no pieces between the king and the rook.
/// - The king is not in check and does not pass through or land on any square attacked by an enemy piece.
/// </summary>
public class CastlingNotAllowed : BusinessRule
{
    private const string CastlingNotAllowedError = "The Castling move is not allowed, either the King or Rook has been moved,"
                                              + " the King is in check or a piece is standing between the King and Rook.";
    private readonly TakeTurn _command;
    private readonly IEnumerable<Turn> _turns;
    private readonly IEnumerable<Piece> _pieces;

    public CastlingNotAllowed(TakeTurn command, IEnumerable<Piece> pieces, IEnumerable<Turn> turns)
    {
        _command = Guard.Against.Null<TakeTurn>(command, nameof(command));
        _pieces = Guard.Against.Null<IEnumerable<Piece>>(pieces, nameof(pieces));
        _turns = Guard.Against.Null<IEnumerable<Turn>>(turns, nameof(turns));
    }

    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        var movingPiece = _pieces.FirstOrDefault(p => p.Position == _command.StartPosition)
                          ?? throw new InvalidOperationException("Moving piece cannot be found!");
        var king = (_pieces.FirstOrDefault(p => p.Type == PieceType.King && p.Color == movingPiece.Color) as King)
                   ?? throw new InvalidOperationException("King cannot be found!");

        var rank = movingPiece.Color == Color.Black ? 8 : 1;
        var file = _command.StartPosition.File < _command.EndPosition.File ? File.A : File.H;

        var castlingType = SpecialMoves.IsCastling(_command.StartPosition, _command.EndPosition, _pieces);
        var isCastling = castlingType != CastlingType.Undefined;

        if (!isCastling) return Enumerable.Empty<BusinessRuleViolation>();

        var rookHasMoved = _turns.Any(p => p.PieceType == PieceType.Rook && p.StartPosition == new Square(file, rank));
        var kingHasMoved = _turns.Any(p => p.PieceType == PieceType.King);

        var kingIsInCheck = Board.IsCheck(king, _pieces);
        var moveIsBlocked = Board.DirectionIsObstructed(_pieces, _command.StartPosition, _command.EndPosition);
        var moveThroughCheck = MoveThroughCheck(king, castlingType);

        if (rookHasMoved || kingHasMoved || moveIsBlocked || kingIsInCheck || moveThroughCheck)
        {
            return new List<BusinessRuleViolation> { new(CastlingNotAllowedError) };
        }

        return Enumerable.Empty<BusinessRuleViolation>();
    }

    private bool MoveThroughCheck(King king, CastlingType type)
    {
        var rank = king.Position.Rank;
        var passingSquares = Array.Empty<Square>();

        if (type == CastlingType.Undefined)
        {
            return false;
        }
        else if (type == CastlingType.KingSide)
        {
            passingSquares = new Square[] { new(File.F, rank), new(File.G, rank), new(File.H, rank) };
        }
        else if (type == CastlingType.QueenSide)
        {
            passingSquares = new Square[] { new(File.A, rank),  new(File.B, rank),  new(File.C, rank), new(File.D, rank),};
        }

        return passingSquares.Any(s => Board.IsCheck(new() { Position = s }, _pieces));
    }
}
