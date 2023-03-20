using Chess.Domain.ValueObjects;

namespace Chess.Domain.Commands;

public record TakeTurn(Guid MemberId, Square StartPosition, Square EndPosition, bool IsCastling);
