using System;

namespace SimpleSpec.NUnit
{
	internal interface ISpecificationFlavor
	{
		specification SpecificationHost { get; set; }

		void Given(Action setupContext);
		void When(Action action);
		void Then(Action behaviorSpecification);
		void CouldFail(Type failureType);

		void Setup();
		void Verify();
	}
}