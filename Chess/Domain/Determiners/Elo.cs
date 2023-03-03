using Ardalis.GuardClauses;
using Chess.Domain.Events;
using Chess.Domain.ValueObjects;

namespace Chess.Domain.Determiners;

public class Elo
{
    /// <summary>
    /// The K-Factor determines how strongly a result affects the rating change.
    /// </summary>
    private const int K = 30;

    /// <summary>
    /// Calculate the Elo of the player
    /// </summary>
    public static EloResult Calculate(float? ratingWhite, float? ratingBlack, MatchResult? matchResult)
    {
        var rA = Guard.Against.Null<float?>(ratingWhite, nameof(ratingWhite))!.Value;
        var rB = Guard.Against.Null<float?>(ratingBlack, nameof(ratingBlack))!.Value;
        var result = Guard.Against.Null<MatchResult?>(matchResult, nameof(matchResult))!.Value;

        Guard.Against.Negative(rA, nameof(ratingWhite));
        Guard.Against.Negative(rB, nameof(ratingBlack));

        var probabilityBlack = CalculateProbability(rA, rB);
        var probabilityWhite = CalculateProbability(rB, rA);

        return result switch
        {
            MatchResult.WhiteWins or MatchResult.BlackForfeit => new()
            {
                WhiteElo = CalculateEloResult(rA, probabilityWhite, 1),
                BlackElo = CalculateEloResult(rB, probabilityBlack, 0)
            },
            MatchResult.BlackWins or MatchResult.WhiteForfeit => new()
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
        var rA = Guard.Against.Null<float?>(ratingA, nameof(ratingA))!.Value;
        var rB = Guard.Against.Null<float?>(ratingB, nameof(ratingB))!.Value;

        Guard.Against.Negative(rA, nameof(ratingA));
        Guard.Against.Negative(rB, nameof(ratingB));

        return 1.0f * 1.0f
                / (1 + 1.0f
                   * (float)(Math.Pow(10, 1.0f * (rA - rB) / 400)));
    }

    private static Func<float, float, float, float> CalculateEloResult = (ranking, probability, matchResult) =>
    {
        return ranking + (K * (matchResult - probability));
    };
}
