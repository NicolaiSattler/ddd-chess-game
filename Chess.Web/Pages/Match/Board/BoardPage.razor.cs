using Chess.Application.Models;
using Chess.Web.Model;
using Microsoft.AspNetCore.Components;

namespace Chess.Web.Pages.Match.Board;

public partial class BoardPage: ComponentBase
{
    [Inject]
    private IApplicationService? ApplicationService { get; set; }

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
            Notations = (await ApplicationService.GetTurns(AggregateId))
                                                 .Select(m => new NotationModel(m.Notation, m.StartTime.GetVerbalTimeDisplay()))
                                                 .ToList();
            Notations.Reverse();

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
            Notations = turns.Select(m => new NotationModel(m.Notation, m.StartTime.GetVerbalTimeDisplay())).ToList();
            Notations.Reverse();

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
        var content = ActiveColor == Color.Black ? "Black is at turn" : "White is at turn";
        Status = new(content, StatusType.Information);
    }
}