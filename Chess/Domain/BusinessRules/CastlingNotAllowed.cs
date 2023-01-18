using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.BusinessRules;

public class CastlingNotAllowed : BusinessRule
{
    private readonly TakeTurn? _command;
    private readonly IEnumerable<Turn>? _turns;
    private readonly IEnumerable<Piece>? _pieces;

    //Neither the king nor the rook has previously moved during the game.
    //There are no pieces between the king and the rook.
    //The king is not in check and does not pass through or land on any square attacked by an enemy piece.
    public CastlingNotAllowed(TakeTurn? command, IEnumerable<Piece>? pieces, IEnumerable<Turn>? turns)
    {
        Guard.Against.Null<TakeTurn?>(command, nameof(command));
        Guard.Against.Null<IEnumerable<Piece>?>(pieces, nameof(pieces));
        Guard.Against.Null<IEnumerable<Turn>?>(turns, nameof(turns));

        _turns = turns;
        _pieces = pieces;
        _command = command;
    }

    public override IEnumerable<BusinessRuleViolation> CheckRule()
    {
        var movingPiece = _pieces?.FirstOrDefault(p => p.Position == _command?.StartPosition);

        throw new NotImplementedException();
    }
}