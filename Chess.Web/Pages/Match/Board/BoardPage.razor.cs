using Chess.Application.Events;
using Chess.Application.Services;
using Chess.Domain.Entities;
using Chess.Web.Dialogs.Surrender;
using Chess.Web.Dialogs.TimerExceeded;
using Chess.Web.Hubs;
using Chess.Web.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chess.Web.Pages.Match.Board;

public partial class BoardPage: ComponentBase, IAsyncDisposable
{
    private const string BlackAtTurn = "Black is at turn";
    private const string WhiteAtTurn = "White is at turn";
    private const string SurrenderDialogTitle = "Surrender";
    private const string TimerExceededDialogTitle = "Turn time exceeded";

    private HubConnection? _hubConnection;

    [Inject]
    private IPlayerActionService? ActionService { get; set; }
    [Inject]
    private IMatchInfoService? MatchInfoService { get; set; }
    [Inject]
    private IMatchDataService? MatchDataService { get; set; }
    [Inject]
    private ITurnTimerInfoService? TimerInfoService { get; set; }
    [Inject]
    private ITimerService? TimerService { get; set;}
    [Inject]
    private MudBlazor.IDialogService? DialogService { get; set; }
    [Inject]
    private NavigationManager? Navigator { get; set; }

    [Parameter]
    public Guid AggregateId { get; set; }
    public StatusModel Status { get; set; } = StatusModel.Empty();
    public List<NotationModel> Notations { get; set; } = new();
    public Color ActiveColor { get; private set; }
    public bool HubIsConnected => _hubConnection?.State == HubConnectionState.Connected;

    protected override async Task OnInitializedAsync()
    {
        await InitializeHub();

        ActiveColor = await MatchInfoService!.GetColorAtTurnAsync(AggregateId);

        var turns = await MatchDataService!.GetTurns(AggregateId);
        //TODO: check, match ended etc..
        SetMatchState(turns);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var turns = await MatchDataService!.GetTurns(AggregateId);
            var lastTurn = turns.Last();
            var remainingTimeInSeconds = await TimerInfoService!.GetRemainingTimeAsync(AggregateId);
            var turnExpired = remainingTimeInSeconds < 0;

            if (turnExpired)
            {
                await HandleTurnTimeExpired(lastTurn.Player.MemberId);
            }
            else
            {
                TimerService!.Start(AggregateId, lastTurn.Player.MemberId, remainingTimeInSeconds);
                TimerService.TurnExpired += TurnExpiredEventHandler;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (TimerService is not null)
        {
            TimerService!.TurnExpired -= TurnExpiredEventHandler;
        }

        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    public async Task OnTurnResultAsync(EndTurnModel endTurnModel)
    {
        if (endTurnModel == null) return;

        ActiveColor = ActiveColor == Color.White ? Color.Black : Color.White;

        var turns = await MatchDataService!.GetTurns(AggregateId);
        SetMatchState(turns);
        StateHasChanged();
    }
    private async Task InitializeHub()
    {
        var url = Navigator!.ToAbsoluteUri(MatchHub.HubUrl);

        _hubConnection = new HubConnectionBuilder()
                            .WithUrl(url)
                            .Build();

        _hubConnection.On<string, string>("DrawPurposed", (memberId, aggregateId) => {
            //TODO: Show draw dialog.
            InvokeAsync(StateHasChanged);
        });

        await _hubConnection.StartAsync();
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

        if (result.Data != null && (bool)result.Data)
        {
            var memberId = await MatchInfoService!.GetPlayerAtTurnAsync(AggregateId);
            var command = new Surrender { MemberId = memberId };

            await ActionService!.SurrenderAsync(AggregateId, command);
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

        await ActionService!.ForfeitAsync(AggregateId, command);
    }
}