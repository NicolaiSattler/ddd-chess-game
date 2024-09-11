using System.Collections.Generic;
using Ardalis.GuardClauses;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using FluentResults;

namespace Chess.Domain.Factories;

public class RuleFactory
{
    public static Result GetTurnRules(TakeTurn command,
                                      IEnumerable<Piece> pieces,
                                      IEnumerable<Turn> turns)
    {
        Guard.Against.Null(command, nameof(command));
        Guard.Against.Null(pieces, nameof(pieces));
        Guard.Against.Null(turns, nameof(turns));
    
        return new PieceInvalidMove(command, pieces, turns).CheckRule()
            .Bind(() => new PieceCannotAttackOwnColor(command, pieces).CheckRule())
            .Bind(() => new PieceIsBlocked(command, pieces).CheckRule())
            .Bind(() => new CastlingNotAllowed(command, pieces, turns).CheckRule())
            .Bind(() => new KingIsInCheck(command, pieces).CheckRule());
    }
}
