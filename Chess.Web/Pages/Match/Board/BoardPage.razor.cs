using Chess.Application.Events;
using Chess.Application.Services;
using Chess.Domain.Entities;
using Chess.Web.Dialogs.Draw;
using Chess.Web.Dialogs.Surrender;
using Chess.Web.Dialogs.TimerExceeded;
using Chess.Web.Hubs;
using Chess.Web.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chess.Web.Pages.Match.Board;

public partial class BoardPage: ComponentBase, IAsyncDisposable
{
    private HubConnection? _hubConnection;

    [Inject] private IPlayerActionService? ActionService { get; set; }
    [Inject] private IMatchInfoService? MatchInfoService { get; set; }
    [Inject] private IMatchDataService? MatchDataService { get; set; }
    [Inject] private ITurnTimerInfoService? TimerInfoService { get; set; }
    [Inject] private ITimerService? TimerService { get; set;}
    [Inject] private MudBlazor.IDialogService? DialogService { get; set; }
    [Inject] private NavigationManager? Navigator { get; set; }
    [Inject] private IHubContext<MatchHub>? HubContext { get; set; }

    [Parameter] public Guid AggregateId { get; set; }
    public StatusModel Status { get; set; } = StatusModel.Empty();
    public List<NotationModel> Notations { get; set; } = new();
    public Color ActiveColor { get; private set; }
    public bool HubIsConnected => _hubConnection?.State == HubConnectionState.Connected;

    protected override async Task OnInitializedAsync()
    {
        await InitializeHub();

        ActiveColor = await MatchInfoService!.GetColorAtTurnAsync(AggregateId);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var options = await MatchInfoService!.GetMatchOptionsAsync(AggregateId);
            var matchResult = await MatchInfoService!.GetMatchResult(AggregateId);
            var turns = await MatchDataService!.GetTurns(AggregateId);

            if (options.UseTurnTimer && matchResult == MatchResult.Undefined)
            {
                await SetupTurnTimer(turns);
            }

            SetMatchState(matchResult, turns);
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

        await InvokeAsync(StateHasChanged);
    }

    private async Task InitializeHub()
    {
        var url = Navigator!.ToAbsoluteUri(MatchHub.HubUrl);

        _hubConnection = new HubConnectionBuilder().WithUrl(url)
                                                   .Build();

        _hubConnection.On<string>("DrawPurposed", (memberId) => {
            InvokeAsync(async() => await OpenDrawDialogAsync(false));
        });

        await _hubConnection.StartAsync();
    }

    private void SetMatchState(IEnumerable<Turn> turns)
    {
        SetMatchNotations(turns);

        var content = ActiveColor == Color.Black ? Constants.BlackAtTurn : Constants.WhiteAtTurn;
        Status = new(content, StatusType.Information);
    }

    private void SetMatchState(MatchResult matchResult, IEnumerable<Turn> turns)
    {
        SetMatchNotations(turns);

        var content = GetStatusText(matchResult);
        Status = new(content, StatusType.Information);
    }

    private void SetMatchNotations(IEnumerable<Turn> turns) =>
        Notations = turns.Reverse()
                         .Select(m => new NotationModel(m.Notation, m.StartTime.GetVerbalTimeDisplay()))
                         .ToList();

    private string GetStatusText(MatchResult matchResult) => matchResult switch
    {
        MatchResult.Undefined => ActiveColor == Color.Black
                              ? Constants.BlackAtTurn
                              : Constants.WhiteAtTurn,
        MatchResult.WhiteWins or
        MatchResult.BlackSurrenders => Constants.WhiteWon,
        MatchResult.BlackWins or
        MatchResult.WhiteSurrenders => Constants.BlackWon,
        MatchResult.Stalemate => Constants.Stalemate,
        MatchResult.Draw => Constants.Draw,
        _ => string.Empty
    };

    private async Task SetupTurnTimer(IEnumerable<Turn> turns)
    {
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

    private async void TurnExpiredEventHandler(object sender, TurnExpiredEventArgs args) =>
        await HandleTurnTimeExpired(args.MemberId);

    private async Task HandleTurnTimeExpired(Guid memberId)
    {
        var options = new MudBlazor.DialogOptions { CloseOnEscapeKey = false };
        var dialog = await DialogService!.ShowAsync<TimerExceededDialog>(Constants.TimerExceededDialogTitle, options);
        var result = await dialog.Result;
        var command = new Forfeit { MemberId = memberId};

        await ActionService!.ForfeitAsync(AggregateId, command);
    }

    private async Task OpenForfeitDialogAsync()
    {
        var options = new MudBlazor.DialogOptions { CloseOnEscapeKey = true };

        if (DialogService == null) return;

        var dialog = await DialogService.ShowAsync<SurrenderDialog>(Constants.SurrenderDialogTitle, options);
        var result = await dialog.Result;

        if (result.Data != null && (bool)result.Data)
        {
            var memberId = await MatchInfoService!.GetPlayerAtTurnAsync(AggregateId);
            var command = new Surrender { MemberId = memberId };

            await ActionService!.SurrenderAsync(AggregateId, command);
        }
    }

    private async Task OpenDrawDialogAsync(bool proposing)
    {
        if (DialogService == null) return;

        var options = new MudBlazor.DialogOptions { CloseOnEscapeKey = true };
        var parameters = new MudBlazor.DialogParameters<DrawDialog>
        {
            {
                m => m.ContentText,
                proposing ? DrawDialog.ConfirmDrawDialogQuestion
                          : DrawDialog.DrawDialogQuestion
            }
        };

        var dialog = await DialogService.ShowAsync<DrawDialog>(Constants.DrawDialogTitle, parameters, options);
        var result = await dialog.Result;

        if (result?.Data is bool accepted)
        {
            if (proposing)
            {
                var memberId = await MatchInfoService!.GetPlayerAtTurnAsync(AggregateId);
                await _hubConnection!.SendAsync("PurposeDraw", memberId, AggregateId);
            }
            else await ActionService!.DrawAsync(AggregateId);
        }
    }
}