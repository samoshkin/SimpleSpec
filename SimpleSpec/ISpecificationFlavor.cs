using System;

namespace SimpleSpec.NUnit
{
	public interface ISpecificationFlavor
	{
		specification SpecificationHost { get; set; }

		void Given(Action setupContext);
		void When(Action action);
		void Then(Action behaviorSpecification);

		void SetupContext();
		void RunAction();
		void VerifyBehavior();
	}
}