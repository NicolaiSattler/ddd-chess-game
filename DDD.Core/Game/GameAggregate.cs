using System;
using System.Collections.Generic;
using DDD.Core.Game.Entities;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game;

public class Game : AggregateRoot<Guid>
{
    private Player White { get; set; }
    private Player Black { get; set; }
    private List<Piece> Pieces { get; set; }
    private List<Move> Moves { get; set; }

    public Game(Guid id) : base(id) { }

    public Game(Guid id, IEnumerable<DomainEvent> events) : base(id, events)
    {
    }

    protected override void When(DomainEvent domainEvent)
    {
        throw new System.NotImplementedException();
    }

    public void Start(Guid memberOneId, Guid memberTwoId)
    {
        //TODO:
        // Create players and assign colors
        // Random color assignment
        // Create turn for white player
    }

    public void End()
    {

    }

    public void TakeTurn(Player player, Piece piece)
    {

    }

    public void Resign(Player resigningPlayer)
    {

    }

    public void ProposeDraw(Player proposingDraw)
    {

    }
}