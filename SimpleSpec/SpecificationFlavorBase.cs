using System;

namespace SimpleSpec.NUnit
{
	internal abstract class SpecificationFlavorBase : ISpecificationFlavor
	{
		public abstract void Given(Action setupContext);
		public abstract void When(Action action);
		public abstract void Then(Action behaviorSpecification);
		public abstract void Setup();
		public abstract void Verify();

		private Type ExpectedFailureType { get; set; }
		public specification SpecificationHost { get; set; }

		public virtual void CouldFail(Type failureType)
		{
			NotAllowedOnVerification("expected failure");
			ExpectedFailureType = failureType;
		}

		protected void RunAction(Action action)
		{
			if (ExpectedFailureType != null)
			{
				try
				{
					action();
				}
				catch (Exception failure)
				{
					if (failure.GetType().IsAssignableFrom(ExpectedFailureType))
					{
						SpecificationHost.ReportFailure(failure);
					}
					else
					{
						throw;
					}
				}
			}
			else
			{
				action();
			}
		}

		protected void NotAllowedOnSetup(string subject)
		{
			if(SpecificationHost.InSetupPhase)
			{
				throw new InvalidOperationException("Specification of '{0}' is not allowed on setup phase.");
			}
		}

		protected void NotAllowedOnVerification(string subject)
		{
			if (!SpecificationHost.InSetupPhase)
			{
				throw new InvalidOperationException("Specification of '{0}' is not allowed on verification phase.");
			}
		}
	}
}