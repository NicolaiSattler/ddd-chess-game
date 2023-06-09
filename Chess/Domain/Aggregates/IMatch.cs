using Chess.Core;
using Chess.Domain.Models;
using Chess.Domain.Commands;
using Chess.Domain.ValueObjects;
using Chess.Domain.Configuration;
using Chess.Domain.Entities.Pieces;
using System.Collections.Generic;
using Chess.Domain.Entities;

namespace Chess.Domain.Aggregates;

public interface IMatch : IAggregateRoot
{
    MatchOptions Options { get; }
    Player White { get; }
    Player Black { get; }
    List<Piece> Pieces { get; }
    List<Turn> Turns { get; }

    void Start(StartMatch command);
    void Draw(Draw command);
    void Resign(Resign command);
    void Forfeit(Forfeit command);
    TurnResult TakeTurn(TakeTurn command);
}

