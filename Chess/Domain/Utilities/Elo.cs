using Ardalis.GuardClauses;
using Chess.Domain.Events;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Utilities;

public class Elo
{
    /// <summary>
    /// The K-Factor determines how strongly a result affects the rating change.
    /// </summary>
    private const int K = 30;

    private static readonly Func<float, float, float, float> CalculateEloResult = (ranking, probability, matchResult)
        => ranking + (K * (matchResult - probability));

    /// <summary>
    /// Calculate the Elo of the player
    /// </summary>
    public static EloResult Calculate(float? ratingWhite, float? ratingBlack, MatchResult? matchResult)
    {
        var rA = Guard.Against.Null(ratingWhite, nameof(ratingWhite));
        var rB = Guard.Against.Null(ratingBlack, nameof(ratingBlack));
        var result = Guard.Against.Null(matchResult, nameof(matchResult));

        Guard.Against.Negative(rA, nameof(ratingWhite));
        Guard.Against.Negative(rB, nameof(ratingBlack));

        var probabilityBlack = CalculateProbability(rA, rB);
        var probabilityWhite = CalculateProbability(rB, rA);

        return result switch
        {
            MatchResult.WhiteWins or MatchResult.BlackSurrenders => new()
            {
                WhiteElo = CalculateEloResult(rA, probabilityWhite, 1),
                BlackElo = CalculateEloResult(rB, probabilityBlack, 0)
            },
            MatchResult.BlackWins or MatchResult.WhiteSurrenders => new()
            {
                WhiteElo = CalculateEloResult(rA, probabilityWhite, 0),
                BlackElo = CalculateEloResult(rB, probabilityBlack, 1)
            },
            MatchResult.Draw or MatchResult.Stalemate => new()
            {
                WhiteElo = CalculateEloResult(rA, probabilityWhite, 0.5f),
                BlackElo = CalculateEloResult(rB, probabilityBlack, 0.5f)
            },
            _ => throw new InvalidOperationException("Unknown match result")
        };
    }

    /// <summary>
    /// Calculate the probability of winning for the player with ratingA
    /// 400 is an arbitrary number for the ranking range.
    /// </summary>
    public static float CalculateProbability(float? ratingA, float? ratingB)
    {
        var rA = Guard.Against.Null(ratingA, nameof(ratingA));
        var rB = Guard.Against.Null(ratingB, nameof(ratingB));

        Guard.Against.Negative(rA, nameof(ratingA));
        Guard.Against.Negative(rB, nameof(ratingB));

        return 1.0f * 1.0f
                / (1 + 1.0f
                   * (float)Math.Pow(10, 1.0f * (rA - rB) / 400));
    }

}
