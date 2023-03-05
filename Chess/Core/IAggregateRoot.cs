using System.Collections.Generic;


namespace Chess.Core
{
    public interface IAggregateRoot : IEntity
    {
        int Version { get; }
        int OriginalVersion { get; }
        IEnumerable<DomainEvent> Events { get; }

        void ClearEvents();
    }
}

