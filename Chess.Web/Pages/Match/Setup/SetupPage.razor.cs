using Chess.Web.Model;
using Microsoft.AspNetCore.Components;

namespace Chess.Web.Pages.Match.Setup;

public partial class SetupPage: ComponentBase
{
    private SetupModel? SetupModel { get; set; }

    protected override void OnInitialized()
    {
        SetupModel = new();
    }
}