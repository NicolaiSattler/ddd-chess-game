namespace Chess.Core
{
    /// <summary>
    /// Represents an Entity in the domain (DDD).
    /// </summary>
    /// <typeparam name="TId">The type of the Id of the entity.</typeparam>
    public abstract class Entity : IEntity
    {
        public Guid Id { get; }

        public Entity(Guid id)
        {
            Id = id;
        }
    }
}
