namespace Chess.Core;

public interface IEntity
{
    Guid Id { get; }
}

public abstract class Entity : IEntity
{
    public Guid Id { get; set; }

    public Entity(Guid id)
    {
        Id = id;
    }
}
