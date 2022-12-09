using System;
using DDD.Core;

public class GameStarted: DomainEvent
{
    public Guid WhiteMemberId { get; set; }
    public Guid BlackMemberId { get; set; }

    public DateTime StartTime { get; set; }
}