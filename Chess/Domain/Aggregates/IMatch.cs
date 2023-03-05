using Chess.Domain.Models;
using Chess.Domain.Commands;
using Chess.Core;
using Chess.Domain.ValueObjects;
using Chess.Domain.Configuration;

namespace Chess.Domain.Aggregates;

public interface IMatch : IAggregateRoot
{
    MatchOptions? Options { get; }
    Player? White { get; }
    Player? Black { get; }

    void Start(StartMatch command);
    void Draw(Draw command);
    void Resign(Resign command);
    void Forfeit(ForfeitCommand command);
    TurnResult TakeTurn(TakeTurn command);
}

