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
    public static EloResult Calculate(float ratingWhite, float ratingBlack, MatchResult matchResult)
    {
        var probabilityWhite = CalculateProbability(ratingWhite, ratingBlack);
        var probabilityBlack = CalculateProbability(ratingBlack, ratingWhite);

        return matchResult switch
        {
            MatchResult.White => new()
            {
                WhiteElo = CalculateEloResult(ratingWhite, probabilityWhite, 1),
                BlackElo = CalculateEloResult(ratingBlack, probabilityBlack, 0)
            },
            MatchResult.Black => new()
            {
                WhiteElo = CalculateEloResult(ratingWhite, probabilityWhite, 0),
                BlackElo = CalculateEloResult(ratingBlack, probabilityBlack, 1)
            },
            MatchResult.Draw => new()
            {
                WhiteElo = CalculateEloResult(ratingWhite, probabilityWhite, 0.5f),
                BlackElo = CalculateEloResult(ratingBlack, probabilityBlack, 0.5f)
            },
            _ => throw new InvalidOperationException("Unknown match result")
        };
    }

    /// <summary>
    /// Calculate the probability of winning for the player with ratingA
    /// 400 range of ranking.
    /// </summary>
    public static float CalculateProbability(float ratingA, float ratingB)
        => 1.0f * 1.0f
            / (1 + 1.0f
                   * (float)(Math.Pow(10, 1.0f * (ratingA - ratingB) / 400)));


    private static Func<float, float, float, float> CalculateEloResult = (ranking, probability, matchResult) =>
    {
        return ranking * K * (matchResult - probability);
    };

}
