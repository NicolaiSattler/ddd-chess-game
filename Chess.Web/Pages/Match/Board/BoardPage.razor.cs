using Chess.Application.Events;
using Chess.Application.Services;
using Chess.Domain.Entities;
using Chess.Web.Dialogs.Surrender;
using Chess.Web.Dialogs.TimerExceeded;
using Chess.Web.Model;
using Microsoft.AspNetCore.Components;

namespace Chess.Web.Pages.Match.Board;

public partial class BoardPage: ComponentBase, IDisposable
{
    private const string BlackAtTurn = "Black is at turn";
    private const string WhiteAtTurn = "White is at turn";
    private const string SurrenderDialogTitle = "Surrender";
    private const string TimerExceededDialogTitle = "Turn time exceeded";

    [Inject]
    private IApplicationService? ApplicationService { get; set; }
    [Inject]
    private ITurnTimerInfoService? TimerInfoService { get; set; }
    [Inject]
    private ITimerService? TimerService { get; set;}
    [Inject]
    private MudBlazor.IDialogService? DialogService { get; set; }

    [Parameter]
    public Guid AggregateId { get; set; }
    public StatusModel Status { get; set; } = StatusModel.Empty();
    public List<NotationModel> Notations { get; set; } = new();
    public Color ActiveColor { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationService != null)
        {
            ActiveColor =  await ApplicationService.GetColorAtTurnAsync(AggregateId);

            var turns = await ApplicationService.GetTurns(AggregateId);
            //TODO: check, match ended etc..
            SetMatchState(turns);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender && ApplicationService != null && TimerInfoService != null && TimerService != null)
        {
            var turns = await ApplicationService.GetTurns(AggregateId);
            var lastTurn = turns.Last();
            var remainingTimeInSeconds = await TimerInfoService.GetRemainingTimeAsync(AggregateId);
            var turnExpired = remainingTimeInSeconds < 0;

            if (turnExpired)
            {
                await HandleTurnTimeExpired(lastTurn.Player.MemberId);
            }
            else
            {
                TimerService.Start(AggregateId, lastTurn.Player.MemberId, remainingTimeInSeconds);
                TimerService.TurnExpired += TurnExpiredEventHandler;
            }
        }
    }

    public void Dispose()
    {
        if (TimerService != null)
        {
            TimerService!.TurnExpired -= TurnExpiredEventHandler;
        }
    }

    public async Task OnTurnResultAsync(EndTurnModel endTurnModel)
    {
        if (endTurnModel == null) return;

        ActiveColor = ActiveColor == Color.White ? Color.Black : Color.White;

        var turns = await ApplicationService!.GetTurns(AggregateId);
        SetMatchState(turns);
        StateHasChanged();
    }

    //TODO: set state of game, e.g. check, checkmate, draw, etc.
    private void SetMatchState(IEnumerable<Turn> turns)
    {
        Notations = turns.Reverse()
                         .Select(m => new NotationModel(m.Notation, m.StartTime.GetVerbalTimeDisplay()))
                         .ToList();

        var content = ActiveColor == Color.Black ? BlackAtTurn : WhiteAtTurn;
        Status = new(content, StatusType.Information);
    }

    private async Task OpenForfeitDialogAsync()
    {
        var options = new MudBlazor.DialogOptions { CloseOnEscapeKey = true };

        if (DialogService == null) return;

        var dialog = await DialogService.ShowAsync<SurrenderDialog>(SurrenderDialogTitle, options);
        var result = await dialog.Result;

        if (result.Data != null && (bool)result.Data && ApplicationService != null)
        {
            var memberId = await ApplicationService.GetPlayerAtTurnAsync(AggregateId);
            var command = new Surrender { MemberId = memberId };

            await ApplicationService.SurrenderAsync(AggregateId, command);
        }
    }

    private async void TurnExpiredEventHandler(object sender, TurnExpiredEventArgs args)
    {
        await HandleTurnTimeExpired(args.MemberId);
    }
    private async Task HandleTurnTimeExpired(Guid memberId)
    {
        var options = new MudBlazor.DialogOptions { CloseOnEscapeKey = false };
        var dialog = await DialogService!.ShowAsync<TimerExceededDialog>(TimerExceededDialogTitle, options);
        var result = await dialog.Result;
        var command = new Forfeit { MemberId = memberId};

        await ApplicationService!.ForfeitAsync(AggregateId, command);
    }
}