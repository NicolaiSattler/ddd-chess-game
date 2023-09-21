using Chess.Application.Models;
using Chess.Web.Dialogs.Surrender;
using Chess.Web.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Chess.Web.Pages.Match.Board;

public partial class BoardPage: ComponentBase
{
    private const string BlackAtTurn = "Black is at turn";
    private const string WhiteAtTurn = "White is at turn";
    private const string SurrenderDialogTitle = "Surrender";

    [Inject]
    private IApplicationService? ApplicationService { get; set; }
    [Inject]
    private IDialogService? DialogService { get; set; }

    [Parameter]
    public Guid AggregateId { get; set; }
    public StatusModel Status { get; set; } = StatusModel.Empty();
    public List<NotationModel> Notations { get; set; } = new();
    public Domain.ValueObjects.Color ActiveColor { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationService != null)
        {
            ActiveColor =  await ApplicationService.GetColorAtTurnAsync(AggregateId);
            Notations = (await ApplicationService.GetTurns(AggregateId))
                                                 .Select(m => new NotationModel(m.Notation, m.StartTime.GetVerbalTimeDisplay()))
                                                 .ToList();

            SetPlayerAtTurnStatus();
        }
    }

    public async Task OnTurnResultAsync(EndTurnModel endTurnModel)
    {
        if (endTurnModel == null) return;

        if (endTurnModel.Result!.MatchResult != MatchResult.Undefined)
        {
            EndMatch(endTurnModel.Result.MatchResult);
        }
        else
        {
            var turns = await ApplicationService!.GetTurns(AggregateId);
            Notations = turns.Select(m => new NotationModel(m.Notation, m.StartTime.GetVerbalTimeDisplay()))
                             .ToList();

            ActiveColor = ActiveColor == Domain.ValueObjects.Color.White
                        ? Domain.ValueObjects.Color.Black
                        : Domain.ValueObjects.Color.White;

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
        var content = ActiveColor == Domain.ValueObjects.Color.Black ? BlackAtTurn : WhiteAtTurn;
        Status = new(content, StatusType.Information);
    }

    private async Task OpenForfeitDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        if (DialogService == null) return;

        var dialog = await DialogService.ShowAsync<SurrenderDialog>(SurrenderDialogTitle, options);
        var result = await dialog.Result;

        if (result.Data != null && (bool)result.Data && ApplicationService != null)
        {
            var memberId = await ApplicationService.GetPlayerAtTurnAsync(AggregateId);
            var command = new Surrender { MemberId = memberId };

            await ApplicationService.SurrenderAsync(AggregateId, command);

            //TODO: Set match result to the opposite of the player who surrendered.
        }
    }
}