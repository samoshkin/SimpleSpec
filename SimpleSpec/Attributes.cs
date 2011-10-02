using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	public class Scenario : TestAttribute
	{
		public class Spec : TestFixtureAttribute
		{}
	}

	public class Behavior : TestAttribute
	{
		public class Spec : TestFixtureAttribute
		{ }
	}
}