using System;
using System.Collections.Generic;
using DDD.Core.Game.Commands;
using DDD.Core.Game.Entities;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game;

public class Match : AggregateRoot<Guid>
{
    private Player White { get; set; }
    private Player Black { get; set; }
    private List<Piece> Pieces { get; set; } = new();
    private List<Move> Moves { get; set; } = new();

    public Match(Guid id) : base(id) { }
    public Match(Guid id, IEnumerable<DomainEvent> events) : base(id, events) { }

    protected override void When(DomainEvent domainEvent)
    {
        throw new System.NotImplementedException();
    }

    private void AssignPlayers(StartGame command)
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

    public GameStarted Start(StartGame command)
    {
        if (command.MemberOneId == command.MemberTwoId) throw new InvalidOperationException("Member Ids are the same.");

        AssignPlayers(command);

        var startMovement = new Move(Guid.NewGuid()) { Player = White, StartTime = DateTime.UtcNow };
        Moves.Add(startMovement);

        return new GameStarted
        {
            WhiteMemberId = White.MemberId,
            BlackMemberId = Black.MemberId,
            StartTime = startMovement.StartTime
        };
    }

    public void End()
    {

    }

    public void TakeTurn(Guid memberId, Guid pieceId, Square newSquare)
    {

    }

    public void Resign(Guid resigningPlayerId)
    {

    }

    public void ProposeDraw(Guid proposingPlayerId)
    {

    }
}