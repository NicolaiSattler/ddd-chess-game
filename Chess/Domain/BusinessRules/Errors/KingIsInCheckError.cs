using FluentResults;

namespace Chess.Domain.BusinessRules;

public class KingIsInCheckError: Error
{
    private const string ErrorMessage = "King is in check, move is not allowed!";

    public KingIsInCheckError(): base(ErrorMessage) {}
}

