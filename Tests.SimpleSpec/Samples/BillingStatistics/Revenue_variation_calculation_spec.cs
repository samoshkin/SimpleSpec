using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using SimpleSpec.NUnit;
using Tests.SimpleSpec.Samples.BillingStatistics.Domain;

namespace Tests.SimpleSpec.Samples.BillingStatistics.RevenueVariationCalculation
{
	[Scenario]
	public class when_calculating_revenue_variation_for_first_billing_run : with_billing_statistics_analyzer
	{
		public when_calculating_revenue_variation_for_first_billing_run()
		{
			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
			                                   new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(3.0m, Currency.USD)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(10.0m, Currency.USD)),
				new InvoiceSummary(client, 3, new DateTime(2011, 9, 1), 10, 2, new Money(4.0m, Currency.USD))));
			Given(() => analyzer.LoadClientStatistics(client));
			When(() => analyzer.CalculateRevenueVariationBetweenPreviousInvoice(new DateTime(2011, 7, 1)))
				.CouldFailWith<ArgumentOutOfRangeException>();
		}

		[Then]
		public void should_fail_because_no_previous_invoice_to_compare_with()
		{
			Failure.Should().NotBeNull();
		}
	}

	[Scenario]
	public class when_calculating_revenue_variation : with_billing_statistics_analyzer
	{
		double revenueVariation;

		public when_calculating_revenue_variation()
		{
			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(3.0m, Currency.USD)),
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(4.0m, Currency.USD)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(10.0m, Currency.USD)),
				new InvoiceSummary(client, 3, new DateTime(2011, 9, 1), 10, 2, new Money(4.0m, Currency.USD))));
			Given(() => analyzer.LoadClientStatistics(client));
			When(() => revenueVariation = analyzer.CalculateRevenueVariationBetweenPreviousInvoice(new DateTime(2011, 8, 1)));
		}

		[Then]
		public void variation_should_be_calculated_upon_total_amount_of_all_invoices_between_subsequent_billing_runs()
		{
			// (10USD - 7 USD)/7USD
			revenueVariation.Should().BeApproximately(0.4285, 0.0001);
		}
	}

	[Scenario]
	public class when_calculating_revenue_variation_between_billing_runs_with_different_currencies : with_billing_statistics_analyzer
	{
		double revenueVariation;

		public when_calculating_revenue_variation_between_billing_runs_with_different_currencies()
		{
			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(10.0m, Currency.USD)),
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(10.0m, Currency.USD)),
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(2.0m, Currency.EUR)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(3.0m, Currency.USD)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(4.0m, Currency.EUR)),
				new InvoiceSummary(client, 3, new DateTime(2011, 9, 1), 10, 2, new Money(4.0m, Currency.USD))));
			Given(() => analyzer.LoadClientStatistics(client));
			When(() => revenueVariation = analyzer.CalculateRevenueVariationBetweenPreviousInvoice(new DateTime(2011, 8, 1)));
		}

		[Then]
		public void variation_should_be_calculated_separately_for_each_currency_and_average_used_as_a_result()
		{
			// (3 USD - 20USD)/2(average per two currencies) * 20USD + (4EUR - 2EUR)/ 2(average per two currencies) * 2EUR = -0.85 + 1 = +0.15
			revenueVariation.Should().BeApproximately(0.075, 0.0001);
		}
	}

	[Scenario]
	public class when_calculating_revenue_variation_and_previous_billing_run_has_zero_amount_invoice : with_billing_statistics_analyzer
	{
		double revenueVariation;

		public when_calculating_revenue_variation_and_previous_billing_run_has_zero_amount_invoice()
		{
			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(1.0m, Currency.EUR)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(0.0m, Currency.EUR)),
				new InvoiceSummary(client, 3, new DateTime(2011, 9, 1), 10, 2, new Money(4.0m, Currency.EUR))));
			Given(() => analyzer.LoadClientStatistics(client));
			When(() => revenueVariation = analyzer.CalculateRevenueVariationBetweenPreviousInvoice(new DateTime(2011, 9, 1)));
		}

		[Then]
		public void should_substitute_zero_amount_of_prev_billing_run_with_one_and_calculate_revenue()
		{
			// 4EUR - 1EUR(substituted instead of zero)/1EUR
			revenueVariation.Should().BeApproximately(3.0, 0.0001);
		}
	}

	[Scenario]
	public class when_calculating_revenue_variation_and_current_billing_run_has_zero_amount_invoice : with_billing_statistics_analyzer
	{
		double revenueVariation;

		public when_calculating_revenue_variation_and_current_billing_run_has_zero_amount_invoice()
		{
			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(1.0m, Currency.EUR)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(0.0m, Currency.EUR)),
				new InvoiceSummary(client, 3, new DateTime(2011, 9, 1), 10, 2, new Money(4.0m, Currency.EUR))));
			Given(() => analyzer.LoadClientStatistics(client));
			When(() => revenueVariation = analyzer.CalculateRevenueVariationBetweenPreviousInvoice(new DateTime(2011, 8, 1)));
		}

		[Then]
		public void should_produce_minu_one_hundred_percents()
		{
			// 0 EUR - 1EUR / 1EUR = -1.0 
			revenueVariation.Should().BeApproximately(-1.0, 0.0001);
		}
	}

	[Scenario]
	public class when_calculating_revenue_variation_and_both_current_and_prev_billing_run_has_zero_amount_invoice : with_billing_statistics_analyzer
	{
		double revenueVariation;

		public when_calculating_revenue_variation_and_both_current_and_prev_billing_run_has_zero_amount_invoice()
		{
			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(1.0m, Currency.EUR)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(0.0m, Currency.EUR)),
				new InvoiceSummary(client, 3, new DateTime(2011, 9, 1), 10, 2, new Money(0.0m, Currency.EUR))));
			Given(() => analyzer.LoadClientStatistics(client));
			When(() => revenueVariation = analyzer.CalculateRevenueVariationBetweenPreviousInvoice(new DateTime(2011, 9, 1)));
		}

		[Then]
		public void should_produce_zero_percent_variation()
		{
			revenueVariation.Should().BeApproximately(0.0, 0.0001);
		}
	}

	[Scenario]
	public class when_calculating_revenue_variation_with_cur_invoice_in_EUR_and_prev_in_USD : with_billing_statistics_analyzer
	{
		double revenueVariation;

		public when_calculating_revenue_variation_with_cur_invoice_in_EUR_and_prev_in_USD()
		{
			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(3.0m, Currency.USD)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(5.0m, Currency.EUR)),
				new InvoiceSummary(client, 3, new DateTime(2011, 9, 1), 10, 2, new Money(0.0m, Currency.EUR))));
			Given(() => analyzer.LoadClientStatistics(client));
			When(() => revenueVariation = analyzer.CalculateRevenueVariationBetweenPreviousInvoice(new DateTime(2011, 8, 1)));
		}

		[Then]
		public void variation_should_be_calculated_separately_for_each_currency_and_average_used_as_a_result()
		{
			// ((0 USD - 3 USD)/ * 3 USD + (5 EUR - 1 EUR (substitute instead of zero))/ 1 EUR)/2(average per two currencies) = 1.5
			revenueVariation.Should().BeApproximately(1.5, 0.0001);
		}
	}
}