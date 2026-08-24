using Bunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Web.Test;

/// <summary>
/// Test context wrapper for bUnit.
/// Read more about using <see cref="BunitTestContext"/> <seealso href="https://bunit.dev/docs/getting-started/writing-tests.html#remove-boilerplate-code-from-tests">here</seealso>.
/// </summary>
public abstract class BunitTestContext : BunitContext
{
	[TestCleanup]
	public void TearDown() => Dispose();
}
