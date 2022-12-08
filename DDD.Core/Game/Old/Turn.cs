using System;
using DDD.Core.Game.Entities;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game;

public class Turn : Entity<Guid>
{
    public Player Player { get; init; }
    public Piece Piece { get; init; }
    public Position Start { get; init; }
    public Position End { get; private set; }

    public Turn(Guid id, Player player, Position start, Piece piece) : base(id)
    {
        Player = player;
        Piece = piece;
    }
}