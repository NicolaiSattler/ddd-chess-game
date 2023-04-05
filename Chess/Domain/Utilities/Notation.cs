using System.Text;
using Chess.Domain.Entities.Pieces;

namespace Chess.Domain.Utilities;

public class NotationBuilder
{

    //TODO:
    //Castling
    //  0-0: kingside castle
    //  0-0-0: queenside castle
    //
    //Ambiguous origin both rooks are in same file/rank
    //
    //x: captures
    //+: check
    //#: checkmate
    //

    private readonly StringBuilder _builder;

    public NotationBuilder()
    {
        _builder = new();
    }

    public NotationBuilder HasPiece(Piece piece)
    {
        if (piece is Pawn) return this;

        var pieceNotation = GetPieceNotation(piece);
        _builder.Append(pieceNotation);

        return this;
    }

    public NotationBuilder HasCapturedPiece(Piece piece)
    {
        if (piece is Pawn)
        {
            _builder.Append(piece.Position.File);
        }

        _builder.Append("x");

        return this;
    }

    public NotationBuilder EndsAtPosition(Piece piece)
    {
        _builder.Append(piece.Position.ToString());

        return this;
    }

    public string Build() => _builder.ToString();


    private string GetPieceNotation(Piece piece) => piece.Type switch
    {
        PieceType.King => "K",
        PieceType.Queen => "Q",
        PieceType.Rook => "R",
        PieceType.Bishop => "B",
        PieceType.Knight => "N",
        _ => throw new IndexOutOfRangeException("Unknown PieceType")
    };
}
