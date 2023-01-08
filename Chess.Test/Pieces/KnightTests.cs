using System.Linq;

namespace Chess.Test.Pieces;

[TestClass]
public class KnightTests
{
    private Knight _sut;

    [TestMethod]
    public void  WhiteAttackRange_B1_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Color = Color.White, Position = new(File.B, 1) };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.A && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 2);
    }

    [TestMethod]
    public void  BlackAttackRange_B8_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Color = Color.Black, Position = new(File.B, 8) };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(3);
        availableMoves.ShouldContain(m => m.File == File.A && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 7);
    }

    [TestMethod]
    public void  BlackAttackRange_D5_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Color = Color.Black, Position = new(File.D, 5) };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(8);
        availableMoves.ShouldContain(m => m.File == File.B && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 7);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 7);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 6);
        availableMoves.ShouldContain(m => m.File == File.B && m.Rank == 4);
        availableMoves.ShouldContain(m => m.File == File.C && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 4);
    }
}