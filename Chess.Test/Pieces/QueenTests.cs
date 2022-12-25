namespace Chess.Test.Pieces;

[TestClass]
public class QueenTests
{
    private Queen _sut;

    [TestInitialize]
    public void Initialize()
    {
    }

    [TestMethod]
    public void  WhiteAttackRange_D1_ShouldBeValid()
    {
        //Arrange
        _sut = new()
        {
            Position = new Square(File.D, 1),
            Color = Color.White
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        //Vertical
        availableMoves.ShouldContain(p => p == new Square(File.D, 4));
        availableMoves.ShouldContain(p => p == new Square(File.D, 8));
        //Horizonal
        availableMoves.ShouldContain(p => p == new Square(File.A, 1));
        availableMoves.ShouldContain(p => p == new Square(File.H, 1));
        //Left Diagonal
        availableMoves.ShouldContain(p => p == new Square(File.A, 4));
        //Right Diagonal
        availableMoves.ShouldContain(p => p == new Square(File.H, 5));
    }

    [TestMethod]
    public void  BlackAttackRange_D8_ShouldBeValid()
    {
        //Arrange
        _sut = new()
        {
            Position = new Square(File.D, 8),
            Color = Color.Black
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        //Vertical
        availableMoves.ShouldContain(p => p == new Square(File.D, 1));
        availableMoves.ShouldContain(p => p == new Square(File.D, 7));
        //Horizonal
        availableMoves.ShouldContain(p => p == new Square(File.A, 8));
        availableMoves.ShouldContain(p => p == new Square(File.H, 8));
        //Left Diagonal
        availableMoves.ShouldContain(p => p == new Square(File.A, 5));
        //Right Diagonal
        availableMoves.ShouldContain(p => p == new Square(File.H, 4));
    }

    [TestMethod]
    public void  WhiteAttackRange_E4_ShouldBeValid()
    {
        //Arrange
        _sut = new()
        {
            Position = new Square(File.E, 4),
            Color = Color.White
        };

        //Act
        var availableMoves = _sut.GetAttackRange();

        //Assert
        //Vertical
        availableMoves.ShouldContain(p => p == new Square(File.E, 1));
        availableMoves.ShouldContain(p => p == new Square(File.E, 7));
        //Horizonal
        availableMoves.ShouldContain(p => p == new Square(File.A, 4));
        availableMoves.ShouldContain(p => p == new Square(File.H, 4));
        //Left Diagonal
        availableMoves.ShouldContain(p => p == new Square(File.A, 8));
        availableMoves.ShouldContain(p => p == new Square(File.H, 1));
        //Right Diagonal
        availableMoves.ShouldContain(p => p == new Square(File.B, 1));
        availableMoves.ShouldContain(p => p == new Square(File.H, 7));
    }
}