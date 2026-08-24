using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Chess.Web.Test;

/// <summary>
/// These tests are written entirely in C#.
/// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
/// </summary>
[TestClass]
public class CounterCSharpTests : BunitTestContext
{
	[TestMethod]
	public void CounterStartsAtZero()
	{
		var cut = Render<CounterComponent>();

		cut.Find("p").MarkupMatches("<p>Current count: 0</p>");
	}

	[TestMethod]
	public void ClickingButtonIncrementsCounter()
	{
		var cut = Render<CounterComponent>();

		cut.Find("button").Click();

		cut.Find("p").MarkupMatches("<p>Current count: 1</p>");
	}

	private sealed class CounterComponent : ComponentBase
	{
		private int _currentCount;

		private void IncrementCount() => _currentCount++;

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "p");
			builder.AddContent(1, $"Current count: {_currentCount}");
			builder.CloseElement();

			builder.OpenElement(2, "button");
			builder.AddAttribute(3, "type", "button");
			builder.AddAttribute(4, "onclick", EventCallback.Factory.Create(this, IncrementCount));
			builder.AddContent(5, "Click me");
			builder.CloseElement();
		}
	}
}
