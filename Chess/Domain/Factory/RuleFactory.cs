using System.Collections.Generic;
using Chess.Core.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.BusinessRules;

public class RuleFactory
{
    public static IEnumerable<BusinessRule> GetTurnRules(TakeTurn command, IEnumerable<Piece>? pieces)
    {
        return new List<BusinessRule>
        {
            new PieceInvalidMove(command, pieces),
            new PieceCannotAttackOwnColor(command, pieces),
            new PieceIsBlocked(command, pieces)
        };
    }
}