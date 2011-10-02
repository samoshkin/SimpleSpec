using FluentAssertions;
using SimpleSpec.NUnit;

namespace Tests.SimpleSpec
{
	public class ScenarioShouldFail : BehavioralAspect
	{
		private readonly string _containedMessage;

		public ScenarioShouldFail(string containedMessage)
		{
			_containedMessage = containedMessage;
		}

		public ScenarioShouldFail()
		{}

		public override void Specify(specification specification)
		{
			specification.Failure.Should().NotBeNull();
			if(_containedMessage != null)
			{
				specification.Failure.Message.Should().Contain(_containedMessage);
			}
		}
	}
}