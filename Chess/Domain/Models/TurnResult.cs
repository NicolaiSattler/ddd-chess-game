using Chess.Domain.Determiners;

namespace Chess.Domain.Models;

public record TurnResult
{
    public bool IsEnPassant { get; init; }
    public CastlingType CastlingType { get; init; }
    public bool IsPromotion { get; init; }
    public string Violation { get; init; } = string.Empty;
}
