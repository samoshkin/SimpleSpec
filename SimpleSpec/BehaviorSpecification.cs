using System;
using System.Collections.Generic;

namespace SimpleSpec.NUnit
{
	internal class BehaviorSpecification : SpecificationFlavorBase
	{
		private readonly IList<Action> _givens = new List<Action>();
		private readonly IList<Action> _behaviorSpecifications = new List<Action>();
		
		public override void Given(Action setupContext)
		{
			if (SpecificationHost.InSetupPhase)
			{
				_givens.Add(setupContext);
			}
			else
			{
				setupContext();
			}
		}

		public override void When(Action action)
		{
			NotAllowedOnSetup("action");
			RunAction(action);
		}

		public override void Then(Action behaviorSpecification)
		{
			if(SpecificationHost.InSetupPhase)
			{
				_behaviorSpecifications.Add(behaviorSpecification);	
			}
			else
			{
				behaviorSpecification();
			}
		}

		public override void Setup()
		{
			foreach (var setupContext in _givens)
			{
				setupContext();
			}
		}

		public override void Verify()
		{
			foreach (var behaviorSpecification in _behaviorSpecifications)
			{
				behaviorSpecification();
			}
		}
	}
}