using Chess.Application.Models;
using Chess.Web.Dialogs.Promotion;
using Chess.Web.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Chess.Web.Pages.Match.Board;

public partial class BoardPage: ComponentBase
{
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
        var content = ActiveColor == Domain.ValueObjects.Color.Black ? "Black is at turn" : "White is at turn";
        Status = new(content, StatusType.Information);
    }

    private async Task OpenDialogAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = false, DisableBackdropClick = true };

        if (DialogService == null) return;

        var dialog = await DialogService.ShowAsync<PromotionDialog>("Select piece promotion", options);
        var result = await dialog.Result;

        Console.Write(result.Data);
    }
}