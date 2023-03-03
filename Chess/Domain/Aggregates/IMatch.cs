using Chess.Domain.Models;
using Chess.Domain.Commands;

namespace Chess.Domain.Aggregates;

public interface IMatch
{
    void Draw(Draw command);
    void Resign(Resign command);
    void Start(StartMatch command);
    TurnResult TakeTurn(TakeTurn command);
}

