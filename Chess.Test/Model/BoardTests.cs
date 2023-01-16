using System;
using System.Collections.Generic;
using Chess.Core.Match.Events;
using Chess.Domain.Model;

namespace Chess.Test.Domain.Model
{
    [TestClass]
    public class BoardTests
    {

        [TestMethod]
        public void PawnC7_MovesToC8_IsPromoted()
        {
            //Arrange
            var command = new TurnTaken(Guid.NewGuid(), new(File.C, 7), new(File.C, 8));
            var pieces = new List<Piece>()
            {
                new Pawn
                {
                    Color = Color.White,
                    Position = new(File.C, 7)
                }
            };


            //Act
            var result = Board.PawnIsPromoted(command, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void PawnC7_MovesToD8_Captures_BishopD8()
        {
            //Arrange
            var command = new TurnTaken(Guid.NewGuid(), new(File.C, 7), new(File.D, 8));
            var pieces = new List<Piece>()
            {
                new Pawn { Color = Color.White, Position = new(File.C, 7) },
                new Bishop { Color = Color.Black, Position = new(File.D, 8)}
            };

            //Act
            var result = Board.PieceIsCaptured(command, pieces);

            //Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void PawnC7_MovesToD8_DoesNotCapture_BishopD8()
        {
            //Arrange
            var command = new TurnTaken(Guid.NewGuid(), new(File.C, 7), new(File.D, 8));
            var pieces = new List<Piece>()
            {
                new Pawn { Color = Color.White, Position = new(File.C, 7) },
                new Bishop { Color = Color.Black, Position = new(File.D, 8)}
            };

            //Act
            var result = Board.PieceIsCaptured(command, pieces);

            //Assert
            result.ShouldBeTrue();
        }
    }
}
