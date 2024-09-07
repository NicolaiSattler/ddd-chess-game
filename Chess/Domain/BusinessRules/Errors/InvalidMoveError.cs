using FluentResults;

namespace Chess.Domain.BusinessRules;

public class InvalidMoveError: Error 
{ 
    public InvalidMoveError(string error) : base(error) { }
}

