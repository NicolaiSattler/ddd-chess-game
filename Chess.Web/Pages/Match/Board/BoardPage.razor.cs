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
    public string? NotationSummary { get; set; }
    public Color ActiveColor { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationService != null)
        {
            ActiveColor =  await ApplicationService.GetColorAtTurnAsync(AggregateId);
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
            NotationSummary = await ApplicationService!.GetNotations(AggregateId);
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