using System.Linq;

namespace Chess.Test.Pieces;

[TestClass]
public class KingTests
{
    private King _sut;

    [TestMethod]
    public void  WhiteAttackRange_E2_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Color = Color.White, Position = new(File.E, 1) };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(5);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 2);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 1);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 2);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 2);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 1);
    }

    [TestMethod]
    public void  BlackAttackRange_E8_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Color = Color.White, Position = new(File.E, 8) };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(5);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 8);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 7);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 7);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 7);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 8);
    }

    [TestMethod]
    public void  BlackAttackRange_E4_ShouldBeValid()
    {
        //Arrange
        _sut = new() { Color = Color.White, Position = new(File.E, 4) };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.Count().ShouldBe(8);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 4);
        availableMoves.ShouldContain(m => m.File == File.D && m.Rank == 5);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.E && m.Rank == 5);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 3);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 4);
        availableMoves.ShouldContain(m => m.File == File.F && m.Rank == 5);
    }
}