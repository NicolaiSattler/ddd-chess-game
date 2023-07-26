using System;
using Chess.Domain.Determiners;
using Chess.Domain.Utilities;

namespace Chess.Test.Unitilies;

[TestClass]
public class NotationBuilderTests
{
    private NotationBuilder _sut;

    [TestInitialize]
    public void Initialize()
    {
        _sut = new();
    }

    [TestMethod]
    [DataRow(PieceType.Bishop, "B")]
    [DataRow(PieceType.King, "K")]
    [DataRow(PieceType.Queen, "Q")]
    [DataRow(PieceType.Rook, "R")]
    [DataRow(PieceType.Knight, "N")]
    public void  HasPiece_ShouldReturnTheCorrectLetter(PieceType pieceType, string expectedLetter)
    {
        //Arrange & Act
        var result = _sut.HasPiece(pieceType).Build();

        //Assert
        result.ShouldBe(expectedLetter);
    }

    [TestMethod]
    [DataRow(typeof(King), "x")]
    [DataRow(typeof(Queen), "x")]
    [DataRow(typeof(Bishop), "x")]
    [DataRow(typeof(Knight), "x")]
    [DataRow(typeof(Rook), "x")]
    [DataRow(typeof(Pawn), "Ex")]
    public void  HasCapturedPiece_ShouldReturn_x(Type pieceType, string expectedLetter)
    {
        //Arrange
        var piece = Activator.CreateInstance(pieceType) as Piece;
        piece.Position = new(File.E, 8);

        //Act
        var result = _sut.HasCapturedPiece(piece).Build();

        //Assert
        result.ShouldBe(expectedLetter);
    }

    [TestMethod]
    public void  IsCheck_ShouldReturn_Plus()
    {
        //Arrange & act
        var result = _sut.IsCheck().Build();

        //Assert
        result.ShouldBe("+");
    }

    [TestMethod]
    public void  IsCheckMate_ShouldReturn_Hash()
    {
        //Arrange & Act
        var result = _sut.IsCheckMate().Build();

        //Assert
        result.ShouldBe("#");
    }

    [TestMethod]
    [DataRow(CastlingType.Undefined, "")]
    [DataRow(CastlingType.KingSide, "0-0")]
    [DataRow(CastlingType.QueenSide, "0-0-0")]
    public void  Castling_ShouldReturnCorrectNotationForEachSide(CastlingType type, string expectedResult)
    {
        //Arrange & Act
        var result = _sut.IsCastling(type).Build();

        //Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow(PieceType.Queen, "=Q")]
    [DataRow(PieceType.Bishop, "=B")]
    [DataRow(PieceType.Rook, "=R")]
    [DataRow(PieceType.Knight, "=N")]
    public void Promotion_ShouldReturnTheCorrectLetter(PieceType pieceType, string expectedLetter)
    {
        //Arrange & Act
        var result = _sut.IsPromotion(pieceType)
                         .Build();

        //Assert
        result.ShouldBe(expectedLetter);
    }

    [TestMethod]
    public void EndPosition_ShouldReturn_FileAndRank()
    {
        //Arrange
        var pawn = new Pawn()
        {
            Position = new Square(File.G, 2)
        };

        //Act
        var result = _sut.EndsAtPosition(pawn).Build();

        //Assert
        result.ShouldBe("G2");
    }
}