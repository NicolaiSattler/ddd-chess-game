using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Determiners;
using Chess.Domain.ValueObjects;
using FluentResults;

namespace Chess.Domain.BusinessRules;

public class PieceInvalidMove : BusinessRule
{
    private const string InvalidPawnAttackError = "A pawn must attack a filled square.";
    private const string KingMoveError = "King cannot set itself check.";
    private const string IlligalMoveError = "Piece must move to designated squares.";

    private readonly TakeTurn _command;
    private readonly IEnumerable<Piece> _pieces;
    private readonly IEnumerable<Turn> _turns;

    public PieceInvalidMove(TakeTurn command, IEnumerable<Piece> pieces, IEnumerable<Turn> turns)
    {
        _command = Guard.Against.Null(command, nameof(command));
        _pieces = Guard.Against.Null(pieces, nameof(pieces));
        _turns = Guard.Against.Null(turns, nameof(turns));
    }

    public override Result CheckRule()
    {
        return ValidateMovingPiece()
            .Bind(movingPiece => ValidateMovement(movingPiece));
    }

    private Result<Piece> ValidateMovingPiece()
    {
        var movingPiece = _pieces?.FirstOrDefault(p => p.Position == _command.StartPosition);

        if (movingPiece == null)
        {
            return new MovingPieceNotFoundError();
        }

        return movingPiece;
    }

    private Result ValidateMovement(Piece piece) => piece.Type switch
    {
        PieceType.Pawn when !IsValidMove((Pawn)piece)
                        => new InvalidMoveError(InvalidPawnAttackError),
        PieceType.King when !IsValidMove((King)piece)
                        => new InvalidMoveError(KingMoveError),
        _ when !PieceMovesToValidSquare(piece)
                       => new InvalidMoveError(IlligalMoveError),
        _ => Result.Ok()
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
