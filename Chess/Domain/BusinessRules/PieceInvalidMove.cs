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

public class PieceInvalidMove : BusinessRule
{
    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece> _pieces;
    private readonly IEnumerable<Turn> _turns;

    public PieceInvalidMove(TakeTurn command, IEnumerable<Piece> pieces, IEnumerable<Turn> turns)
    {
        _command = Guard.Against.Null(command, nameof(command));
        _pieces = Guard.Against.Null(pieces, nameof(pieces));
        _turns = Guard.Against.Null(turns, nameof(turns));
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

    private BusinessRuleViolation? ValidateMovement(Piece piece) => piece.Type switch
    {
        PieceType.Pawn when !IsValidMove((Pawn)piece)
                        => new("A pawn must attack a filled square."),
        PieceType.King when !IsValidMove((King)piece)
                        => new("King cannot set itself check."),
        _ when !PieceMovesToValidSquare(piece)
                       => new("Piece must move to designated squares."),
        _ => null
    };

    private bool PieceMovesToValidSquare(Piece piece)
    {
        var availableMoves = piece.GetAttackRange().ToList();

        if (piece is King king && KingIsAtStartPosition(king))
        {
            availableMoves.Add(new(File.B, king.Position.Rank));
            availableMoves.Add(new(File.G, king.Position.Rank));
        }

        return availableMoves?.Any(square => square == _command.EndPosition) ?? false;
    }

    private bool IsValidMove(King king)
    {
        //Move results in check.
        var opponentPieces = _pieces?.Where(p => p.Color != king?.Color);
        var positionIsSafe = !Board.GetPiecesThatCanReachPosition(_command.EndPosition, _pieces!, opponentPieces!).Any();

        return positionIsSafe;
    }

    private bool IsValidMove(Pawn pawn)
    {
        Guard.Against.Null(pawn, nameof(pawn));

        if (!PieceMovesToValidSquare(pawn)) return false;

        var availableMoves = pawn.GetAttackRange();
        var attackMoves = availableMoves?.Where(m => m.File != pawn?.Position?.File);
        var attack = attackMoves?.FirstOrDefault(m => m == _command.EndPosition);

        if (attack != null)
        {
            return SpecialMoves.IsEnPassant(pawn, _turns) || (_pieces?.Any(p => p.Position == attack) ?? false);
        }

        return true;
    }

    private static bool KingIsAtStartPosition(King king) =>
        king.Color == Color.Black
        ? king.Position == new Square(File.E, 8)
        : king.Position == new Square(File.E, 1);

}
