using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	public class scenario_spec
	{
		private readonly IList<Action> _givens = new List<Action>();
		private Action _when;

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


		public void Given(Action contextSetupAction)
		{
			_givens.Add(contextSetupAction);
		}

		public void When(Action whenAction)
		{
			if(_when != null)
			{
				throw new InvalidOperationException("'When' action was already specified before. It could be specified only once per scenario.");
			}
			_when = whenAction;
		}


		[SetUp]
		public void TestSetUp()
		{
			PrepareContext();
			RunAction();
		}
		
		private void RunAction()
		{
			if (CouldFail)
			{
				try
				{
					if (_when != null)
					{
						_when();
					}	
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
				if (_when != null)
				{
					_when();
				}
			}
		}

		private void PrepareContext()
		{
			foreach (var contextSetupAction in _givens)
			{
				contextSetupAction();
			}
		}
	}

	
}
