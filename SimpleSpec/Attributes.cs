using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	public class ScenarioAttribute : TestFixtureAttribute
	{ }

	public class BehaviorAttribute : TestFixtureAttribute
	{ }

	public class WhenAttribute : TestAttribute
	{ }

	public class ThenAttribute : TestAttribute
	{}
}