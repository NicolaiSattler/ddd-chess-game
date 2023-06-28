using Chess.Application.Models;
using Microsoft.AspNetCore.Components;

using MatchEntity = Chess.Infrastructure.Entity.Match;

namespace Chess.Web.Pages.Match.Overview;

public partial class OverviewPage: ComponentBase
{
    [Inject]
    private IApplicationService? ApplicationService { get; set; }

    [Inject]
    private NavigationManager? Navigator { get; set; }

    public List<MatchEntity> Matches { get; set; } = new();

    private void OnViewClick(MatchEntity entity)
    {
        Navigator?.NavigateTo($"/match/board/{entity.AggregateId}");
    }

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationService != null)
        {
            var result = await ApplicationService.GetMatchesAsync();

            Matches = result.ToList();
        }
    }
}