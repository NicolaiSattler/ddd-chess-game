namespace Chess.Application.Models;

public interface ITurnTimer
{
    void Start(Guid aggregateId, Guid memberId, int maxTurnLengthInSecods);
    void Stop();
}

