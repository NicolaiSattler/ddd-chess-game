using System;
using DDD.Core.Game.ValueObjects;

namespace DDD.Core.Game.Entities;

public class Piece : Entity<Guid>
{
    public Color Color {get; init;}
    public string Type { get; init; }
    public Square Position { get; set; }

    public Piece(Guid id) : base(id) { }
}