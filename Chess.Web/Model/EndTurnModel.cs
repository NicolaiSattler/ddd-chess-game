namespace Chess.Web.Model;

public record EndTurnModel
{
    public Guid ActiveMemberId { get; init; }
    public string? Notation { get; init; }
    public TurnResult Result { get; init; } = new();
}