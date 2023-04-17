using Chess.Domain.Entities.Pieces;
using Chess.Domain.ValueObjects;
using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Pages.Test;

public partial class Board
{
    public IEnumerable<Piece> Pieces { get; set; } = new List<Piece>
    {
        new King() { Position = new(File.E, 8), Color = Color.Black },
        new King() { Position = new(File.E, 1), Color = Color.White }
    };

    public Piece? SelectPiece(int rank, int file)
        => Pieces.FirstOrDefault(p => p.Position == new Square((File)file, rank));

    public IEnumerable<Square> ShowAvailableMoves(Guid pieceId)
    {
        throw new NotImplementedException();
    }

    public void TakeTurn(Guid pieceId, Square endPosition)
    {
        throw new NotImplementedException();
    }
}