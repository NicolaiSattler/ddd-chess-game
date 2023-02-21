using Chess.Domain.Commands;

namespace Chess.Domain.Aggregates;

public interface IMatch
{
    void Draw(Draw command);
    void ProposeDraw(ProposeDraw command);
    void Resign(Resign command);
    void Start(StartMatch command);
    void TakeTurn(TakeTurn command);
}

