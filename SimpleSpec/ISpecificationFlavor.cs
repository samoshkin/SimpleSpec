using System;

namespace SimpleSpec.NUnit
{
	public interface ISpecificationFlavor
	{
		specification SpecificationHost { get; set; }

		void Given(Action setupContext);
		void When(Action action);
		void Verify(Action behaviorSpecification);

		void SetupContext();
		void RunAction();
		void VerifyBehavior();
	}
}