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

/// <summary>
/// Validating if purposed Castling move is allowed. The following criteria must be met:
/// - Neither the king nor the rook has previously moved during the game.
/// - There are no pieces between the king and the rook.
/// - The king is not in check and does not pass through or land on any square attacked by an enemy piece.
/// </summary>
public class CastlingNotAllowed : BusinessRule
{
    private readonly TakeTurn _command;
    private readonly IEnumerable<Turn> _turns;
    private readonly IEnumerable<Piece> _pieces;

    public CastlingNotAllowed(TakeTurn command, IEnumerable<Piece> pieces, IEnumerable<Turn> turns)
    {
        _command = Guard.Against.Null(command, nameof(command));
        _pieces = Guard.Against.Null(pieces, nameof(pieces));
        _turns = Guard.Against.Null(turns, nameof(turns));
    }

    public override Result CheckRule()
    {
        return ValidateMovingPiece()
            .Bind((movingPiece) => ValidateKing(movingPiece))
            .Bind((result) =>
            {
               var rank = result.Item1.Color == Color.Black ? 8 : 1;
               var file = _command.StartPosition.File < _command.EndPosition.File ? File.A : File.H;

               var castlingType = SpecialMoves.IsCastling(_command.StartPosition, _command.EndPosition, _pieces);
               var isCastling = castlingType != CastlingType.Undefined;

               if (isCastling)
               {
                    var rookHasMoved = _turns.Any(p => p.PieceType == PieceType.Rook && p.StartPosition == new Square(file, rank));
                    var kingHasMoved = _turns.Any(p => p.PieceType == PieceType.King && p.Player.MemberId == _command.MemberId);
                    var kingIsInCheck = Board.IsCheck(result.Item2, _pieces);
                    var moveIsBlocked = Board.DirectionIsObstructed(_pieces, _command.StartPosition, _command.EndPosition);
                    var moveThroughCheck = MoveThroughCheck(result.Item2, castlingType);

                    if (rookHasMoved || kingHasMoved || moveIsBlocked || kingIsInCheck || moveThroughCheck)
                    {
                        return new CastlingNotAllowedError();
                    }
                }

                return Result.Ok();
            });
    }

    private Result<Piece> ValidateMovingPiece()
    {
        var movingPiece = _pieces.FirstOrDefault(p => p.Position == _command.StartPosition);

        return movingPiece == null
            ? new MovingPieceNotFoundError()
            : Result.Ok(movingPiece);
    }

    private Result<(Piece, King)> ValidateKing(Piece movingPiece)
    {
        var king = _pieces.FirstOrDefault(p => p.Type == PieceType.King && p.Color == movingPiece.Color) as King;

        return king == null
            ? new KingNotFoundError()
            : (movingPiece, king);
    }

    private bool ActionIsCastlingMove(Piece movingPiece)
    {
        var rank = movingPiece.Color == Color.Black ? 8 : 1;
        var file = _command.StartPosition.File < _command.EndPosition.File ? File.A : File.H;
        var castlingType = SpecialMoves.IsCastling(_command.StartPosition, _command.EndPosition, _pieces);
        return castlingType != CastlingType.Undefined;
    }

    private bool MoveThroughCheck(King king, CastlingType type)
    {
        var rank = king.Position.Rank;
        var passingSquares = Array.Empty<Square>();
        var opponentPieces = _pieces.Where(p => p.Color != king.Color);

        if (type == CastlingType.Undefined)
        {
            return false;
        }
        else if (type == CastlingType.KingSide)
        {
            passingSquares = new Square[] { new(File.F, rank), new(File.G, rank) };
        }
        else if (type == CastlingType.QueenSide)
        {
            passingSquares = new Square[] { new(File.C, rank), new(File.D, rank),};
        }

        return passingSquares.Any(s => Board.IsCheck(new() { Position = s }, opponentPieces));
    }

}
