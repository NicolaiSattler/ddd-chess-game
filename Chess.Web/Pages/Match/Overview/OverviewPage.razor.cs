using Chess.Application.Models;
using Microsoft.AspNetCore.Components;

using MatchEntity = Chess.Infrastructure.Entity.Match;

namespace Chess.Web.Pages.Match;

public partial class OverviewPage: ComponentBase
{
    [Inject]
    private ApplicationService? ApplicationService { get; set; }

    private List<MatchEntity>? Matches { get; set;}

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationService != null)
        {
            var result = await ApplicationService.GetMatchesAsync();

            Matches = result.ToList();
        }
    }
}