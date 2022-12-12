using System.Collections.Generic;
using DDD.Core.Match.Commands;
using DDD.Core.Match.Entities;
using DDD.Core.Match.Events;
using DDD.Core.Match.ValueObjects;

namespace DDD.Core.Match;

public class Match : AggregateRoot<Guid>
{
    private Player White { get; set; }
    private Player Black { get; set; }
    private List<Piece> Pieces { get; set; }
    private List<Move> Moves { get; set; }

    public Match(Guid id) : base(id) { }
    public Match(Guid id, IEnumerable<DomainEvent> events) : base(id, events) { }

    protected override void When(DomainEvent domainEvent)
    {
        if (domainEvent is MatchStarted gameStarted) Handle(gameStarted);
    }

    public void Start(StartMatch command)
    {
        if (command.MemberOneId == command.MemberTwoId) throw new InvalidOperationException("Member Ids are the same.");

        AssignPlayers(command);

        var gameStarted = new MatchStarted
        {
            WhiteMemberId = White.MemberId,
            BlackMemberId = Black.MemberId,
            StartTime = DateTime.UtcNow
        };

        RaiseEvent(gameStarted);
    }

    public void End(MatchEnded command)
    {
        throw new NotImplementedException();
    }

    public void TakeTurn(TakeTurn command)
    {
        throw new NotImplementedException();
    }

    public void Resign(Guid resigningPlayerId)
    {
        throw new NotImplementedException();
    }

    public void ProposeDraw(Guid proposingPlayerId)
    {
        throw new NotImplementedException();
    }

    private void Handle(MatchStarted @event)
    {
        Pieces = new();
        Moves = new()
        {
            new() { Player = White, StartTime = @event.StartTime }
        };

        //TODO: Generate pieces for Chess game.

    }

    private void AssignPlayers(StartMatch command)
    {
        var colorPicker = new Random(1);

        if (colorPicker.Next() == 0)
        {
            White = new Player { MemberId = command.MemberOneId };
            Black = new Player { MemberId = command.MemberTwoId };

        }
        else
        {
            White = new Player { MemberId = command.MemberTwoId };
            Black = new Player { MemberId = command.MemberOneId };
        }
    }
}
