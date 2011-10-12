using System;
using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	public class specification
	{
		public specification()
		{
			OnConstruction = true;
			InnerSpec = ResolveSpecificationFlavor();
			SetupScenarioOnlyOnce = InnerSpec is ScenarioSpecification;
		}

		private ISpecificationFlavor	InnerSpec { get; set; }
		private bool					CouldFail { get { return ExpectedFailureType != null; } }
		private Type					ExpectedFailureType { get; set; }
		private bool                    SetupScenarioOnlyOnce { get; set; }
		
		public Exception				Failure { get; private set; }
		internal bool					OnConstruction { get; private set; }
		
		public specification CouldFailWith<TException>()
			where TException : Exception
		{
			ExpectedFailureType = typeof(TException);
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
			OnConstruction = false;
			if(SetupScenarioOnlyOnce)
			{
				InnerSpec.SetupContext();
				RunAction();    
			}
		}

		[SetUp]
		public void TestSetUp()
		{
			OnConstruction = false;
			if(!SetupScenarioOnlyOnce)
			{
				InnerSpec.SetupContext();
				RunAction();    
			}
		}

		[TearDown]
		public void TearDown()
		{
			InnerSpec.VerifyBehavior();
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

		private void RunAction()
		{
			if (CouldFail)
			{
				try
				{
					InnerSpec.RunAction();	
				}
				catch(Exception failure)
				{
					if(failure.GetType().IsAssignableFrom(ExpectedFailureType))
					{
						Failure = failure;
					}
					else
					{
						throw;
					}
				}
			}
			else
			{
				InnerSpec.RunAction();
			}
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
				supposedFlavor = new ScenarioSpecification();
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
