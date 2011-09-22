using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	public class behavior_spec
	{
		private readonly IList<Action> _behaviorSpecifications = new List<Action>();
		private readonly IList<Action> _commonGivens = new List<Action>();

		private bool IsVerifying { get; set; }

		private bool CouldFail
		{
			get { return ExpectedFailureType != null; }
		}
		private Type ExpectedFailureType { get; set; }
		public Exception Failure { get; private set; }

		public void CouldFailWith<TException>()
			where TException : Exception
		{
			ExpectedFailureType = typeof(TException);
		}

		public behavior_spec()
		{
			IsVerifying = false;
		}


		public void When(Action whenAction)
		{
			if(!IsVerifying)
			{
				throw new InvalidOperationException("Cannot setup or run 'When' action when constructing specification. Is valid only on verification step.");
			}
			if(CouldFail)
			{
				try
				{
					whenAction();		
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
				whenAction();
			}
		}

		public void Given(Action setupContextAction)
		{
			// TODO: Do not allow specifying given after when action is run
			if (IsVerifying)
			{
				setupContextAction();
			}
			else
			{
				_commonGivens.Add(setupContextAction);
			}
		}

		public void Should(Action behaviorSpecification)
		{
			_behaviorSpecifications.Add(behaviorSpecification);
		}


		[TearDown]
		public void TearDown()
		{
			VerifySpecification();
			IsVerifying = false;
		}

		[SetUp]
		public void SetUp()
		{
			IsVerifying = true;
			PrepareCommonContext();
		}

		private void PrepareCommonContext()
		{
			foreach (var contextSetupAction in _commonGivens)
			{
				contextSetupAction();
			}
		}

		private void VerifySpecification()
		{
			foreach (var behaviorSpecification in _behaviorSpecifications)
			{
				behaviorSpecification();
			}
		}
	}
}