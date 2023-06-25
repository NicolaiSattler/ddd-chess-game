namespace Chess.Test.Pieces;

[TestClass]
public class BishopTests
{
    private Bishop _sut;

    [TestMethod]
    public void WhiteAttackRange_C1_ShouldBeValid()
    {
        //Arrange
        _sut = new()
        {
            Color = Color.White,
            Position = new(File.C, 1)
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.ShouldContain(m => m == new Square(File.A, 3));
        availableMoves.ShouldContain(m => m == new Square(File.B, 2));
        availableMoves.ShouldContain(m => m == new Square(File.D, 2));
        availableMoves.ShouldContain(m => m == new Square(File.H, 6));
    }

    [TestMethod]
    public void BlackAttackRange_C8_ShouldBeValid()
    {
        //Arrange
        _sut = new()
        {
            Color = Color.White,
            Position = new(File.C, 8)
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        availableMoves.ShouldContain(m => m == new Square(File.A, 6));
        availableMoves.ShouldContain(m => m == new Square(File.B, 7));
        availableMoves.ShouldContain(m => m == new Square(File.D, 7));
        availableMoves.ShouldContain(m => m == new Square(File.H, 3));
    }
}