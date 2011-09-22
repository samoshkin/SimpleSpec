using System;
using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	// TODO: Consider renaming this class to avoid misunderstanding between ScenarioSpecification, BehaviorSpecification
	public class specification
	{
		public specification()
		{
			OnConstruction = true;
		}

		private ISpecificationFlavor	InnerSpec { get; set; }
		private bool					CouldFail { get { return ExpectedFailureType != null; } }
		private Type					ExpectedFailureType { get; set; }
		
		public Exception				Failure { get; private set; }
		internal bool					OnConstruction { get; private set; }

		
		public specification IsA<TSpecificationFlavor>()
			where TSpecificationFlavor : ISpecificationFlavor
		{
			InnerSpec = Activator.CreateInstance<TSpecificationFlavor>();
			InnerSpec.SpecificationHost = this;
			return this;
		}

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

		public specification Verify(Action behavior)
		{
			InnerSpec.Verify(behavior);
			return this;
		}


		[SetUp]
		public void TestSetUp()
		{
			OnConstruction = false;
			InnerSpec.SetupContext();
			RunAction();
		}

		[TearDown]
		public void TearDown()
		{
			InnerSpec.VerifyBehavior();
		}
	

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
	}

	
}
