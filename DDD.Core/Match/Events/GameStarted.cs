using System;

namespace DDD.Core.Match.Events;

public class MatchStarted: DomainEvent
{
    public Guid WhiteMemberId { get; set; }
    public Guid BlackMemberId { get; set; }
    public DateTime StartTime { get; set; }
}