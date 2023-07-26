using Microsoft.AspNetCore.Components;
using Chess.Application.Models;
using Chess.Web.Model;

using File = Chess.Domain.ValueObjects.File;
using PieceEntity = Chess.Domain.Entities.Pieces.Piece;

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

    public PieceEntity? SelectPiece(int rank, int file) => Pieces?.FirstOrDefault(p => p.Position == new Square((File)file, rank));
    public void AddChild(FieldComponent fieldComponent) => Fields.Add(fieldComponent);
    public void HideAvailableMoves() => SetFieldHighlight(Fields, false);
    public void ShowAvailableMoves(Guid pieceId)
    {
        SetFieldHighlight(Fields, false);

        var piece = Pieces.FirstOrDefault(p => p.Id == pieceId);
        var moves = piece?.GetAvailableMoves(Pieces) ?? Enumerable.Empty<Square>();
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

    //TODO: check if move is castling!
    private async Task<TurnResult> ExecuteTurnCommand(PieceEntity activePiece, Square endPosition)
    {
        var command = new TakeTurn() { MemberId = ActiveMemberId, StartPosition = activePiece.Position, EndPosition = endPosition };

        return await ApplicationService!.TakeTurnAsync(AggregateId, command);
    }

    private async Task HandleTurnResultAsync(TurnResult turnResult, PieceEntity activePiece, Square endPosition)
    {
        if (turnResult?.Violations?.Any() ?? true) return;

        activePiece.Position = endPosition;

        var targetPiece = Pieces.FirstOrDefault(p => p.Position == endPosition && p.Color != activePiece.Color);

        if (targetPiece != null) Pieces.Remove(targetPiece);

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
}