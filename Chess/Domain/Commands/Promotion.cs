using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Commands;

public record Promotion
{
    public Square PawnPosition  { get; init; } = new(File.Undefined, 0);
    public PieceType PromotionType { get; init; }
}