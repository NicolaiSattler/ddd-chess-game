using System.Collections.Generic;

namespace Chess.Core;

/// <summary>
/// Represents an aggregate-root of a domain aggregate (DDD). An aggregate-root is always an entity.
/// </summary>
/// <typeparam name="TId">The type of the Id of the entity.</typeparam>
public interface IAggregateRoot : IEntity
{
    /// <summary>
    /// The current version after handling any commands.
    /// </summary>
    int Version { get; }
    /// <summary>
    /// The original version of the aggregate after replaying all events in the event-store.
    /// </summary>
    int OriginalVersion { get; }
    IEnumerable<DomainEvent> Events { get; }

    void ClearEvents();
}

public abstract class AggregateRoot : Entity, IAggregateRoot
{
    private readonly List<DomainEvent> _events;

    protected bool IsReplaying { get; private set; } = false;

    public int Version { get; private set; }
    public int OriginalVersion { get; private set; }
    public IEnumerable<DomainEvent> Events => _events;

    public AggregateRoot(Guid id) : base(id)
    {
        OriginalVersion = 0;
        Version = 0;
        _events = new List<DomainEvent>();
    }

    public AggregateRoot(Guid id, IEnumerable<DomainEvent?>? events) : this(id)
    {
        IsReplaying = true;

        if (events != null)
        {
            foreach (var evt in events)
            {
                When(evt);
                OriginalVersion++;
                Version++;
            }
        }

        IsReplaying = false;
    }

    protected void RaiseEvent(DomainEvent domainEvent)
    {
        When(domainEvent);

        _events.Add(domainEvent);

        Version += 1;
    }
    public void ClearEvents() => _events.Clear();

    protected abstract void When(DomainEvent? domainEvent);
}
