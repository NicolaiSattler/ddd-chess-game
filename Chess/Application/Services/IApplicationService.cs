using Chess.Application.Events;
using Chess.Domain.Commands;
using Chess.Domain.Entities;
using Chess.Domain.Entities.Pieces;
using Chess.Domain.Models;
using Chess.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

using MatchEntity = Chess.Infrastructure.Entity.Match;

namespace Chess.Application.Services;

public interface IApplicationService
{
    //TODO: Split in multiple services
    Task StartMatchAsync(StartMatch command);
    Task<TurnResult> TakeTurnAsync(Guid aggregateId, TakeTurn command);
    Task SurrenderAsync(Guid aggregateId, Surrender command);
    Task PurposeDrawAsync(Guid aggregateId, ProposeDraw command);
    Task DrawAsync(Guid aggregateId, Draw command);
    Task ForfeitAsync(Guid aggregateId, Forfeit command);
    Task PromotePawnAsync(Guid aggregateId, Square position, PieceType type);

    Task<List<Piece>> GetPiecesAsync(Guid aggregateId);
    Task<Guid> GetPlayerAtTurnAsync(Guid aggregateId);
    Task<Color> GetColorAtTurnAsync(Guid aggregateId);
    Task<IEnumerable<Turn>> GetTurns(Guid aggregateId);
    Task<IEnumerable<MatchEntity>> GetMatchesAsync();
    Task<Player> GetPlayerAsync(Guid aggregateId, Color color);
}

