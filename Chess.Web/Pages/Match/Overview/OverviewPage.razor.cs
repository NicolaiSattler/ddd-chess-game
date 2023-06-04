using Chess.Application.Models;
using Microsoft.AspNetCore.Components;

using MatchEntity = Chess.Infrastructure.Entity.Match;

namespace Chess.Web.Pages.Match.Overview;

public partial class OverviewPage: ComponentBase
{
    [Inject]
    private IApplicationService? ApplicationService { get; set; }

    public List<MatchEntity> Matches { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationService != null)
        {
            var result = await ApplicationService.GetMatchesAsync();

            Matches = result.ToList();
        }
    }
}