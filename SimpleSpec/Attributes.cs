using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	// for behavior specifications
	public class BehaviorAttribute : TestFixtureAttribute
	{ }

	public class WhenAttribute : TestAttribute
	{ }

	//for scenario specifications
	public class ThenAttribute : TestAttribute
	{}

	public class ScenarioAttribute : TestFixtureAttribute
	{ }
}