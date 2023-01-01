using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

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

        var movingPiece = _pieces.FirstOrDefault(p => p.Position == _command.StartPosition)
                        ?? throw new InvalidOperationException($"No piece was found at {_command.StartPosition}");
        var direction = GetMoveDirection();

        return EndPositionIsBlocked(movingPiece) || MoveIsObstructed(_pieces.ToArray(), direction, _command.StartPosition, _command.EndPosition)
            ? new List<BusinessRuleViolation>() { new(PieceIsBlockedViolation) }
            : Enumerable.Empty<BusinessRuleViolation>();
    }

    private bool EndPositionIsBlocked(Piece movingPiece) =>
        _pieces.Any(p => p.Position == _command.EndPosition && p.Color == movingPiece.Color);

    private DirectionType GetVerticalDirection(DirectionType type) => _command.StartPosition?.Rank < _command.EndPosition?.Rank
                                            ? type |= DirectionType.Up
                                            : type |= DirectionType.Down;

    private DirectionType GetHorizontalDirection(DirectionType type) => _command.StartPosition?.File < _command.EndPosition?.File
                                            ? type |=  DirectionType.Right
                                            : type |= DirectionType.Left;

    private DirectionType GetMoveDirection()
    {
        _ = _command.StartPosition ?? throw new InvalidOperationException("Start position is null");
        _ = _command.EndPosition ?? throw new InvalidOperationException("End position is null");

        var result = DirectionType.Undefined;

        if (_command.StartPosition.File == _command.EndPosition.File)
            return GetVerticalDirection(result);

        if (_command.StartPosition.Rank == _command.EndPosition.Rank)
            return GetHorizontalDirection(result);

        result = GetVerticalDirection(result);
        result = GetHorizontalDirection(result);

        return result;
    }

    private bool MoveIsObstructed(Piece[] pieces, DirectionType direction, Square start, Square end) => (direction) switch
    {
        DirectionType.Left => pieces.Any(p => (int?)p.Position?.File < (int)start.File && (int?)p.Position?.File > (int)end.File),
        DirectionType.Right => pieces.Any(p => (int?)p.Position?.File > (int)start.File && (int?)p.Position?.File < (int)end.File),
        DirectionType.Up => pieces.Any(p => p.Position?.Rank > start.Rank && p.Position?.Rank < end.Rank),
        DirectionType.Down => pieces.Any(p => p.Position?.Rank < start.Rank && p.Position?.Rank > end.Rank),
        DirectionType.Left | DirectionType.Up => ValidateDiagonalLeftUpObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Up => ValidateDiagonalRightUpObstruction(pieces, start, end),
        DirectionType.Left | DirectionType.Down => ValidateDiagonalLeftDownObstruction(pieces, start, end),
        DirectionType.Right | DirectionType.Down => ValidateDiagonalRightDownObstruction(pieces, start, end),
        _ => throw new InvalidOperationException("Oeps")
    };

    private bool ValidateDiagonalRightDownObstruction(Piece[] pieces, Square start, Square end)
    {
        for (var x = end.Rank; x > start.Rank; x--)
            for (var y = (int)start.File; y < (int)end.File; y++)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }

    private bool ValidateDiagonalLeftDownObstruction(Piece[] pieces, Square start, Square end)
    {
        for (var x = end.Rank; x > start.Rank; x--)
            for (var y = (int)start.File; y > (int)end.File; y--)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }

    private bool ValidateDiagonalRightUpObstruction(Piece[] pieces, Square start, Square end)
    {
        for (var x = start.Rank; x < end.Rank; x++)
            for (var y = (int)start.File; y < (int)end.File; y++)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }

    private bool ValidateDiagonalLeftUpObstruction(Piece[] pieces, Square start, Square end)
    {
        for (var x = start.Rank; x < end.Rank; x++)
            for (var y = (int)start.File; y > (int)end.File; y--)
                if (pieces.Any(p => p.Position == new Square((File)y, x)))
                    return true;
        return false;
    }
}