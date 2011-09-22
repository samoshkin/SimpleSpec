using System;
using NUnit.Framework;

namespace SimpleSpec.NUnit
{
	[Scenario]
	public class when_calculating_sum_of_two_positive_numbers : scenario_spec
	{
		int a, b, c;

		public when_calculating_sum_of_two_positive_numbers()
		{
			Given(() =>
			{
				a = 1;
				b = 2;
			});
			When(() => c = a + b);
		}

		[Then]
		public void should_produce_positive_number_too()
		{
			Assert.That(c, Is.GreaterThan(0));
		}
	}

	[Scenario]
	public class when_dividing_on_zero : scenario_spec
	{
		int a, b, c;

		public when_dividing_on_zero()
		{
			Given(() =>
			{
				a = 1;
				b = 0;
			});
			When(() => c = a / b);
			CouldFailWith<DivideByZeroException>();
		}

		[Then]
		public void should_fail()
		{
			Assert.That(Failure, Is.Not.Null);
		}
	}

	[Behavior]
	public class operation_should_fail : behavior_spec
	{
		int a = 1;

		public operation_should_fail()
		{
			Given(() => { a = 1; });
			CouldFailWith<DivideByZeroException>();

			Should(() => Assert.That(Failure, Is.Not.Null));
		}

		[When]
		public void when_divide_by_zero()
		{
			int b = 0;
			Given(() => { b = 0; });
			When(() => Console.WriteLine(a / b));
		}
	}
}