using Chess.Web.Model;
using Microsoft.AspNetCore.Components;

namespace Chess.Web.Components.Status;

public partial class StatusComponent : ComponentBase
{
    private const string CalloutClass = "callout";

    [Parameter]
    public StatusModel Model { get; set; } = StatusModel.Empty();

    private string? HtmlClass { get; set; }

    private static string GetCalloutTypeClass(StatusType status) => status switch
    {
        StatusType.Information => "Color.Info",
        StatusType.Warning => "Color.Warning",
        StatusType.Error => "Color.Error",
        _ => string.Empty
    };

    protected override void OnInitialized()
    {
        var typeClass = GetCalloutTypeClass(Model.Type);
        HtmlClass = $"{CalloutClass} {typeClass}";
    }
}