using System;
using System.Collections.Generic;

namespace SimpleSpec.NUnit
{
	public class ScenarioSpecification : ISpecificationFlavor
	{
		private readonly IList<Action> _givens = new List<Action>();
		private Action _when;

		public specification SpecificationHost { get; set; }

		public void Given(Action setupContext)
		{
			if(!SpecificationHost.OnConstruction)
			{
				throw new InvalidOperationException("Scenario context must be setup only on construction step.");
			}
			_givens.Add(setupContext);
		}

		public void When(Action action)
		{
			if(SpecificationHost.OnConstruction)
			{
				if (_when != null)
				{
					throw new InvalidOperationException("Scenario could have only zero or one 'When' action.");
				}
				_when = action;	
			}
			else
			{
				action();
			}
		}

		public void Then(Action behaviorSpecification)
		{
			if (SpecificationHost.OnConstruction)
			{
				throw new InvalidOperationException("Scenario behavior could not be specified on construction step.");
			}
			behaviorSpecification();
		}


		public void SetupContext()
		{
			foreach (var contextSetupAction in _givens)
			{
				contextSetupAction();
			}
		}

		public void RunAction()
		{
			if (_when != null)
			{
				_when();
			}
		}

		public void VerifyBehavior()
		{
			// Do nothing. Behavior verification step is not applicable in scenario specification.
		}
	}
}