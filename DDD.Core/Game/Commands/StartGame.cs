using System;

namespace DDD.Core.Game.Commands;

public record StartGame
{
    public Guid MemberOneId { get; set;}
    public Guid MemberTwoId { get; set;}
}