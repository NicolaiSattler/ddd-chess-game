using System.Collections.Generic;
using System.Linq;
using Chess.Core.BusinessRules;
using Chess.Domain.BusinessRules;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.Factories;

public class RuleFactory
{
    public static IEnumerable<BusinessRule> GetTurnRules(TakeTurn command,
                                                         IEnumerable<Piece>? pieces,
                                                         IEnumerable<Turn>? turns,
                                                         string maxTurnLength = "01:00:00")
    {
        TimeSpan.TryParse(maxTurnLength, out var turnLength);

        var currentTurn = turns?.Last();

        return new List<BusinessRule>
        {
            new PieceInvalidMove(command, pieces, turns),
            new PieceCannotAttackOwnColor(command, pieces),
            new PieceIsBlocked(command, pieces),
            new CastlingNotAllowed(command, pieces, turns),
            new TurnExpired(currentTurn, turnLength)
        };
    }
}