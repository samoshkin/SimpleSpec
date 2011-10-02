using System;
using System.Collections.Generic;

namespace SimpleSpec.NUnit
{
	public class BehaviorSpecification : ISpecificationFlavor
	{
		private readonly IList<Action> _givens = new List<Action>();
		private readonly IList<Action> _behaviorSpecifications = new List<Action>();

		public specification SpecificationHost { get; set; }

		
		public void Given(Action setupContext)
		{
			if (SpecificationHost.OnConstruction)
			{
				_givens.Add(setupContext);
			}
			else
			{
				setupContext();
			}
		}

		public void When(Action action)
		{
			if (SpecificationHost.OnConstruction)
			{
				throw new InvalidOperationException("'When' action cannot be specified on construction step for behavior specification.");
			}
			action();
		}

		public void Then(Action behaviorSpecification)
		{
			if(SpecificationHost.OnConstruction)
			{
				_behaviorSpecifications.Add(behaviorSpecification);	
			}
			else
			{
				behaviorSpecification();
			}
		}

		public void SetupContext()
		{
			foreach (var setupContext in _givens)
			{
				setupContext();
			}
		}

		public void RunAction()
		{
			// Do nothing. Not applicable at this step.
		}

		public void VerifyBehavior()
		{
			foreach (var behaviorSpecification in _behaviorSpecifications)
			{
				behaviorSpecification();
			}
		}
	}
}