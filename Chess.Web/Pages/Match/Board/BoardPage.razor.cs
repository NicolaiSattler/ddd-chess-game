using Chess.Application.Models;
using Chess.Web.Model;
using Microsoft.AspNetCore.Components;

using File = Chess.Domain.ValueObjects.File;

namespace Chess.Web.Pages.Match.Board;

public partial class BoardPage: ComponentBase
{
    private const string TakeTurnErroMessage = "An error occurred while updating the board for piece {ActivePieceId} for member {ActiveMemberId}";

    [Inject]
    private ILogger<BoardPage>? Logger { get; set;}

    [Inject]
    private IApplicationService? ApplicationService { get; set; }

    [Parameter]
    public EventCallback<Guid> OnPieceMoved { get; set; }

    [Parameter]
    public Guid AggregateId { get; set; }

    private Guid ActiveMemberId => ActiveColor == Color.White ? White.MemberId : Black.MemberId;
    public IList<Piece> Pieces { get; private set; } = new List<Piece>();
    public List<FieldComponent> Fields { get; } = new();
    public StatusModel Status { get; set; } = StatusModel.Empty();
    public Player White { get; private set; } = new();
    public Player Black { get; private set; } = new();
    public Color ActiveColor { get; private set; }
    public Guid ActivePieceId { get; set; }

    private void SetFieldHighlight(IEnumerable<FieldComponent> fields, bool highlighted)
    {
        foreach (var item in fields) item.Highlight(highlighted);
    }

    public Piece? SelectPiece(int rank, int file) => Pieces?.FirstOrDefault(p => p.Position == new Square((File)file, rank));

    public void HideAvailableMoves() => SetFieldHighlight(Fields, false);

    public void ShowAvailableMoves(Guid pieceId)
    {
        SetFieldHighlight(Fields, false);

        var piece = Pieces.FirstOrDefault(p => p.Id == pieceId);
        var moves = piece?.GetAvailableMoves(Pieces) ?? Enumerable.Empty<Square>();
        var fields = Fields.Where(f => moves.Any(m => m.File == f.File && m.Rank == f.Rank));

        SetFieldHighlight(fields, true);
    }

    public void AddChild(FieldComponent fieldComponent) => Fields.Add(fieldComponent);

    public async Task UpdateBoardAsync(int rank, File file)
    {
        try
        {
            var activePiece = Pieces.FirstOrDefault(p => p.Id == ActivePieceId) ?? throw new InvalidOperationException("Piece could not be found!");
            var endPosition = new Square(file, rank);
            var command = new TakeTurn(ActiveMemberId, activePiece.Position, endPosition, false);

            if (ApplicationService == null) throw new ApplicationException("ApplicationService is null.");

            var turnResult = await ApplicationService.TakeTurnAsync(AggregateId, command);

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

            SetPlayerAtTurnStatus();
        }
    }

    public async Task HandleTurnResultAsync(TurnResult turnResult, Piece activePiece, Square endPosition)
    {
        if (turnResult?.Violations?.Any() ?? true) return;

        activePiece.Position = endPosition;

        var targetPiece = Pieces.FirstOrDefault(p => p.Position == endPosition && p.Color != activePiece.Color);

        if (targetPiece != null) Pieces.Remove(targetPiece);

        await OnPieceMoved.InvokeAsync(ActivePieceId);

        if (turnResult.MatchResult != 0)
        {
            EndMatch(turnResult.MatchResult);
        }
        else
        {
            ActiveColor = ActiveColor == Color.White ? Color.Black : Color.White;

            SetPlayerAtTurnStatus();
        }

        StateHasChanged();
    }

    private void EndMatch(MatchResult result)
    {
        //TODO: ...
    }

    private void SetPlayerAtTurnStatus()
    {
        var content = ActiveColor == Color.Black ? "Black at turn" : "White at turn";
        Status = new(content, StatusType.Information);
    }
}