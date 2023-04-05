using Chess.Web.Components.Field;

namespace Chess.Web.Pages.Test;

public partial class Example
{
    public void bla()
    {

        var i = 0;
        var j = 0;
        var colour = (j % 2) > 0
                   ? (i % 2) > 0
                        ?  BackgroundColour.Light : BackgroundColour.Dark
                   :  BackgroundColour.Dark;
    }
    private int Count { get; set; }
}