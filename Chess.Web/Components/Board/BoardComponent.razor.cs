using Microsoft.AspNetCore.Components;
using Chess.Application.Models;
using Chess.Web.Model;

using File = Chess.Domain.ValueObjects.File;
using PieceEntity = Chess.Domain.Entities.Pieces.Piece;
using Chess.Domain.Determiners;
using Chess.Domain.Entities;

namespace Chess.Web.Components.Board;

public partial class BoardComponent: ComponentBase
{
    private const string TakeTurnErroMessage = "An error occurred while updating the board for piece {ActivePieceId} for member {ActiveMemberId}";

    [Inject]
    private ILogger<BoardComponent>? Logger { get; set;}
    [Inject]
    private IApplicationService? ApplicationService { get; set; }
    private Guid ActiveMemberId => ActiveColor == Color.White ? White.MemberId : Black.MemberId;

    [Parameter, EditorRequired]
    public Guid AggregateId { get; set; }
    [Parameter, EditorRequired]
    public EventCallback<EndTurnModel> OnTurnEnded { get; set; }

    public Player White { get; private set; } = new();
    public Player Black { get; private set; } = new();
    public Color ActiveColor { get; private set; }
    public Guid ActivePieceId { get; set; }
    public List<FieldComponent> Fields { get; } = new();
    public List<PieceEntity> Pieces { get; private set; } = new();

    public PieceEntity? SelectPiece(int rank, int file) => Pieces?.Find(p => p.Position == new Square((File)file, rank));
    public void AddChild(FieldComponent fieldComponent) => Fields.Add(fieldComponent);
    public void HideAvailableMoves() => SetFieldHighlight(Fields, false);
    public async Task ShowAvailableMovesAsync(Guid pieceId)
    {
        SetFieldHighlight(Fields, false);

        var piece = Pieces.Find(p => p.Id == pieceId);
        var moves = piece?.GetAvailableMoves(Pieces) ?? Enumerable.Empty<Square>();

        if (piece is King kingPiece) moves = moves.Concat(GetCastlingMoves(kingPiece));
        else if (piece is Pawn pawn)
        {
            var pawnMoves = await GetEnPassantMoves(pawn);
            moves = moves.Concat(pawnMoves);
        }

        var fields = Fields.Where(f => moves.Any(m => m.File == f.File && m.Rank == f.Rank));

        SetFieldHighlight(fields, true);
    }
    public async Task UpdateBoardAsync(int rank, File file)
    {
        try
        {
            var activePiece = Pieces.First(p => p.Id == ActivePieceId);
            var endPosition = new Square(file, rank);
            var turnResult = await ExecuteTurnCommand(activePiece, endPosition);

            await HandleTurnResultAsync(turnResult, activePiece, endPosition);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, TakeTurnErroMessage, ActivePieceId, ActiveMemberId);
        }
        finally
        {
            ActivePieceId = Guid.Empty;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationService != null)
        {
            Pieces = await ApplicationService.GetPiecesAsync(AggregateId);
            ActiveColor = await ApplicationService.GetColorAtTurnAsync(AggregateId);
        }
    }

    private async Task<TurnResult> ExecuteTurnCommand(PieceEntity activePiece, Square endPosition)
    {
        var command = new TakeTurn() { MemberId = ActiveMemberId, StartPosition = activePiece.Position, EndPosition = endPosition };

        return await ApplicationService!.TakeTurnAsync(AggregateId, command);
    }

    private async Task HandleTurnResultAsync(TurnResult turnResult, PieceEntity activePiece, Square endPosition)
    {
        if (turnResult?.Violations?.Any() ?? true) return;

        var targetPiece = Pieces.FirstOrDefault(p => p.Position == endPosition && p.Color != activePiece.Color);

        activePiece.Position = endPosition;

        if (targetPiece != null) Pieces.Remove(targetPiece);

        if (turnResult.CastlingType != CastlingType.Undefined)
        {
            HandleCastlingMove(activePiece, turnResult.CastlingType);
        }

        if (turnResult.IsEnPassant) await HandleEnPessantMoveAsync(activePiece);

        if (turnResult.MatchResult == MatchResult.Undefined)
        {
            ActiveColor = ActiveColor == Color.White ? Color.Black : Color.White;

            await OnTurnEnded.InvokeAsync(new() { ActiveMemberId = ActiveMemberId, Result = turnResult });
        }

        StateHasChanged();
    }


    private static void SetFieldHighlight(IEnumerable<FieldComponent> fields, bool highlighted)
    {
        foreach (var item in fields) item.Highlight(highlighted);
    }

    private IEnumerable<Square> GetCastlingMoves(King king)
    {
        var result = new List<Square>();
        var rank = king.Position.Rank;

        if (!Pieces.Any(p => p.Position == new Square(File.G, rank) || p.Position == new Square(File.F, rank)))
        {
            result.Add(new(File.G, rank));
        }

        if (!Pieces.Any(p => p.Position == new Square(File.B, rank)
                            || p.Position == new Square(File.C, rank)
                            || p.Position == new Square(File.B, rank)))
        {
            result.Add(new(File.B, rank));
        }

        return result;
    }

    private async Task<IEnumerable<Square>> GetEnPassantMoves(Pawn pawn)
    {
        if (ApplicationService == null) return Enumerable.Empty<Square>();

        var turns =  await ApplicationService.GetTurns(AggregateId);
        var turnCount = turns.Count();

        if (turnCount == 1) return Enumerable.Empty<Square>();

        var opponentTurn = turns?.ElementAt(turnCount - 2);
        var isPawnMove = opponentTurn?.PieceType == PieceType.Pawn;
        var rankDiff = opponentTurn?.StartPosition?.Rank - opponentTurn?.EndPosition?.Rank;
        var isTwoRankMove = rankDiff > 1 || rankDiff < -1;
        var sameRank = pawn.Position.Rank == opponentTurn?.EndPosition?.Rank;

        if (!isPawnMove || !isTwoRankMove || !sameRank) return Enumerable.Empty<Square>();

        var file = opponentTurn?.StartPosition?.File ?? File.Undefined;
        var rank = rankDiff > 0
                   ? pawn.Position.Rank + 1
                   : pawn.Position.Rank - 1;

        return new Square[] { new(file, rank)};
    }

    //TODO: should be refactored: Aggregate contains similair logic?
    private void HandleCastlingMove(PieceEntity activePiece, CastlingType castlingType)
    {
        if (castlingType == CastlingType.Undefined) return;

        var oldFile = castlingType == CastlingType.KingSide ? File.H : File.A;
        var newFile = castlingType == CastlingType.KingSide ? File.F : File.D;
        var rook = Pieces.Find(p => p.Type == PieceType.Rook
                                        && p.Color == activePiece.Color
                                        && p.Position.File == oldFile);

        if (rook != null)
        {
            var newField = Fields.Find(m => m.File == newFile && m.Rank == activePiece.Position.Rank);
            var oldField = Fields.Find(m => m.File == rook.Position.File && m.Rank == rook.Position.Rank);

            rook.Position = new(newFile, rook.Position.Rank);

            oldField?.RemoveChild();
            newField?.AddChild(rook);
        }
    }

    private async Task HandleEnPessantMoveAsync(PieceEntity activePiece)
    {
        if (ApplicationService == null) return;
        if (activePiece is Pawn pawn && pawn == null) return;

        var turns = await ApplicationService.GetTurns(AggregateId) ?? Enumerable.Empty<Turn>();
        var turnCount = turns.Count();
        var lastOppenentMove = turns.ElementAt(turnCount - 3);
        var field = lastOppenentMove?.EndPosition;

        var targetPiece = Pieces?.Find(p => p.Position == field);
        if (targetPiece != null) Pieces?.Remove(targetPiece);

        var pawnField = Fields.Find(m => m.File == field?.File && m.Rank == field?.Rank);
        pawnField?.RemoveChild();

        StateHasChanged();
    }

    private void HandlePromotion()
    {

    }
}