using Microsoft.AspNetCore.Components;

namespace Chess.Web.Components;

public partial class ExampleComponent
{
    [Parameter]
    public int Count { get; set; }

    [Parameter, EditorRequired]
    public int Incrementation { get; set; }

    [Parameter]
    public EventCallback<int> CountChanged { get; set; }

    private async Task IncrementAsync()
    {
        Count += Incrementation;
        await CountChanged.InvokeAsync(Count);
    }
}