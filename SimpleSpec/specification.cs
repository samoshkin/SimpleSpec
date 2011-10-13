using System;
using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	public class specification
	{
		public specification()
		{
			InSetupPhase = true;
			InnerSpec = ResolveSpecificationFlavor();
			SetupScenarioOnlyOnce = InnerSpec is ScenarioSpecification;
		}

		public Exception				Failure { get; private set; }

		private ISpecificationFlavor	InnerSpec { get; set; }
		
		internal bool					InSetupPhase { get; private set; }
		private bool					SetupScenarioOnlyOnce { get; set; }
		

		public specification CouldFailWith<TException>()
			where TException : Exception
		{
			InnerSpec.CouldFail(typeof(TException));
			return this;
		}

		public specification Given(Action contextSetup)
		{
			InnerSpec.Given(contextSetup);
			return this;
		}

		public specification When(Action action)
		{
			InnerSpec.When(action);
			return this;
		}

		public specification Then(Action behavior)
		{
			InnerSpec.Then(behavior);
			return this;
		}

		public specification Then(BehavioralAspect behavior)
		{
			InnerSpec.Then(() => behavior.Specify(this));
			return this;
		}



		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			InSetupPhase = false;
			if(SetupScenarioOnlyOnce)
			{
				InnerSpec.Setup();
			}
		}

		[SetUp]
		public void TestSetUp()
		{
			InSetupPhase = false;
			if(!SetupScenarioOnlyOnce)
			{
				InnerSpec.Setup();
			}
		}

		[TearDown]
		public void TearDown()
		{
			InnerSpec.Verify();
			if(!SetupScenarioOnlyOnce)
			{
				OnScenarioCleanUp();	
			}
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			if (SetupScenarioOnlyOnce)
			{
				OnScenarioCleanUp();
			}
		}



		protected virtual void OnScenarioCleanUp()
		{}

		internal void ReportFailure(Exception failure)
		{
			Failure = failure;
		}

		private ISpecificationFlavor ResolveSpecificationFlavor()
		{
			ISpecificationFlavor supposedFlavor;
			var specificationType = GetType();
			if (Attribute.IsDefined(specificationType, typeof(Scenario.Spec)))
			{
				supposedFlavor = new ScenarioSpecification();
			}
			else if (Attribute.IsDefined(specificationType, typeof(Behavior.Spec)))
			{
				supposedFlavor = new BehaviorSpecification();
			}
			else
			{
				throw new Exception(String.Format(
					"Fail to resolve specification flavor. Neither '{0}' or '{1}' are applied on specification class.",
					typeof(Scenario.Spec),
					typeof(Behavior.Spec)));
			}
			supposedFlavor.SpecificationHost = this;
			return supposedFlavor;
		}
	}

	
}
