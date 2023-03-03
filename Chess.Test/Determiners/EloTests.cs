using System;
using Chess.Domain.Determiners;
using Chess.Domain.Events;

namespace Chess.Test.Domain.Determiners;

[TestClass]
public class EloTests
{
    [TestMethod]
    public void CalculateProbability_SumShouldBeOne()
    {
        //Arrange
        var ratingA = 1000;
        var ratingB = 1200;

        //Act
        var pA = Elo.CalculateProbability(ratingA, ratingB);
        var pB = Elo.CalculateProbability(ratingB, ratingA);
        var sum = pA + pB;

        //Assert
        sum.ShouldBe(1);
        Math.Round(pA, 2).ShouldBe(0.76);
        Math.Round(pB, 2).ShouldBe(0.24);
    }

    [TestMethod]
    [DataRow(-1, 1)]
    [DataRow(1, -1)]
    public void CalculateProbability_NegativeRank_ShouldThrowException(float rankingA, float rankingB)
    {
        //Act & Assert
        Should.Throw<ArgumentException>(() => Elo.CalculateProbability(rankingA, rankingB));
    }

    [TestMethod]
    [DataRow(MatchResult.BlackWins)]
    [DataRow(MatchResult.WhiteForfeit)]
    public void Calculate_BlackWins(MatchResult matchResult)
    {
        //Arrange
        var ratingWhite = 1000;
        var ratingBlack = 1200;

        //Act
        var result = Elo.Calculate(ratingWhite, ratingBlack, matchResult);

        //Assert
        Math.Round(result.BlackElo, 0).ShouldBe(1207);
        Math.Round(result.WhiteElo, 0).ShouldBe(993);
    }

    [TestMethod]
    [DataRow(MatchResult.WhiteWins)]
    [DataRow(MatchResult.BlackForfeit)]
    public void Calculate_WhiteWins(MatchResult matchResult)
    {
        //Arrange
        var ratingWhite = 1000;
        var ratingBlack = 1200;

        //Act
        var result = Elo.Calculate(ratingWhite, ratingBlack, matchResult);

        //Assert
        Math.Round(result.BlackElo, 0).ShouldBe(1177);
        Math.Round(result.WhiteElo, 0).ShouldBe(1023);
    }

    [TestMethod]
    public void Calculate_Draw()
    {
        //Arrange
        var ratingWhite = 1000;
        var ratingBlack = 1200;
        var matchResult = MatchResult.Draw;

        //Act
        var result = Elo.Calculate(ratingWhite, ratingBlack, matchResult);

        //Assert
        Math.Round(result.BlackElo, 0).ShouldBe(1192);
        Math.Round(result.WhiteElo, 0).ShouldBe(1008);
    }

    [TestMethod]
    public void Calculate_Stalemate()
    {
        //Arrange
        var ratingWhite = 1000;
        var ratingBlack = 1200;
        var matchResult = MatchResult.Stalemate;

        //Act
        var result = Elo.Calculate(ratingWhite, ratingBlack, matchResult);

        //Assert
        Math.Round(result.BlackElo, 0).ShouldBe(1192);
        Math.Round(result.WhiteElo, 0).ShouldBe(1008);
    }

    [TestMethod]
    [DataRow(null, 1200, MatchResult.WhiteWins)]
    [DataRow(1000, null, MatchResult.WhiteWins)]
    [DataRow(1000, 1200, null)]
    public void Calculate_Draw(int? ratingWhite, int? ratingBlack, MatchResult? result)
    {
        Should.Throw<ArgumentNullException>(() => Elo.Calculate(ratingWhite, ratingBlack, result));
    }
}
