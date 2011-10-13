using System;
using System.Collections.Generic;

namespace SimpleSpec.NUnit
{
	internal class ScenarioSpecification : SpecificationFlavorBase
	{
		private readonly IList<Action> _givens = new List<Action>();
		private Action _when;

		public override void Given(Action setupContext)
		{
			NotAllowedOnVerification("scenario context");
			_givens.Add(setupContext);
		}

		public override void When(Action action)
		{
			if(SpecificationHost.InSetupPhase)
			{
				if (_when != null)
				{
					throw new InvalidOperationException("When action could be specified only once per scenario.");
				}
				_when = action;	
			}
			else
			{
				RunAction(action);
			}
		}

		public override void Then(Action behaviorSpecification)
		{
			NotAllowedOnSetup("behavior");
			behaviorSpecification();
		}
		
		public override void Setup()
		{
			foreach (var contextSetupAction in _givens)
			{
				contextSetupAction();
			}
			if (_when != null)
			{
				RunAction(_when);
			}
		}

		public override void Verify()
		{
			// Do nothing. 
			// Behavior verification is performed directly from user code, or by calling Then() with behavior verification delegate.
		}
	}
}