using Chess.Domain.Determiners;
using FluentResults;
using System.Linq;

namespace Chess.Domain.Models;

public record TurnResult
{
    public bool IsEnPassant { get; init; }
    public CastlingType CastlingType { get; init; }
    public bool IsPromotion { get; init; }
    public Result TurnValidation { get; init; } = Result.Ok();

    public bool IsValid => TurnValidation.IsSuccess;
    public string? ErrorMessage => TurnValidation.Errors.FirstOrDefault()?.Message;
}
