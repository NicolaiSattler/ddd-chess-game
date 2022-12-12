using System;

namespace DDD.Core.Match.Commands;

public record StartMatch
{
    public Guid MemberOneId { get; set;}
    public Guid MemberTwoId { get; set;}
}