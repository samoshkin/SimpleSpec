using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Assertions;
using Moq;
using SimpleSpec.NUnit;
using Tests.SimpleSpec.Samples.BillingStatistics.Domain;

namespace Tests.SimpleSpec.Samples.BillingStatistics
{
	public class with_billing_statistics_analyzer : specification
	{
		public BillingStatisticsAnalyzer analyzer;
		public Mock<IBillingDataProvider> billingDataProvider;
		public Client client;

		protected void billing_statistics_analyzer()
		{
			client = new Client(1, "me", new DateTime(2011, 1, 1));
			billingDataProvider = new Mock<IBillingDataProvider>();
			analyzer = new BillingStatisticsAnalyzer(billingDataProvider.Object);
		}

		protected void client_was_billed_with(params InvoiceSummary[] invoices)
		{
			billingDataProvider
				.Setup(provider => provider.FetchClientStatistics(client))
				.Returns(invoices);
		}
	}

	[Scenario]
	public class when_statistics_is_being_loaded_for_the_client : with_billing_statistics_analyzer
	{
		InvoiceSummary juneInvoice, julyInvoice, augustInvoice;

		public when_statistics_is_being_loaded_for_the_client()
		{
			juneInvoice = new InvoiceSummary(client, 1, new DateTime(2011, 6, 1), 6, 2, new Money(2.0m, Currency.USD));
			julyInvoice = new InvoiceSummary(client, 2, new DateTime(2011, 7, 1), 10, 1, new Money(3.0m, Currency.USD));
			augustInvoice = new InvoiceSummary(client, 3, new DateTime(2011, 8, 1), 8, 4, new Money(4.0m, Currency.USD));

			IsA<ScenarioSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(juneInvoice, julyInvoice, augustInvoice));
			When(() => analyzer.LoadClientStatistics(client));
		}

		[Then]
		public void should_get_statistics_data_from_billing_data_provider()
		{
			billingDataProvider.Verify();
			analyzer.Invoices.Should().BeEquivalentTo(juneInvoice, julyInvoice, augustInvoice);
		}
	}

	[Behavior]
	public class statistics_analysis_behavior : with_billing_statistics_analyzer
	{
		public statistics_analysis_behavior()
		{
			IsA<BehaviorSpecification>();
			Given(billing_statistics_analyzer);
			Given(() => client_was_billed_with(
				new InvoiceSummary(client, 1, new DateTime(2011, 7, 1), 10, 1, new Money(3.0m, Currency.USD)),
				new InvoiceSummary(client, 2, new DateTime(2011, 8, 1), 10, 1, new Money(3.0m, Currency.USD)),
				new InvoiceSummary(client, 3, new DateTime(2011, 8, 1), 10, 2, new Money(2.0m, Currency.EUR))));
			Given(() => analyzer.LoadClientStatistics(client));
		}

		[Then]
		public void for_specified_date_should_tell_whether_it_is_a_date_of_a_first_billing_run()
		{
			analyzer.IsFirstBillingRun(new DateTime(2011, 7, 1)).Should().BeTrue();
			analyzer.IsFirstBillingRun(new DateTime(2011, 8, 1)).Should().BeFalse();
		}

		[Then]
		public void when_determine_first_billing_run_and_no_billing_runs_occured_on_specified_date_should_fail()
		{
			analyzer.Invoking(a => a.IsFirstBillingRun(new DateTime(2011, 6, 1)))
				.ShouldThrow<ArgumentException>()
				.WithMessage("Client was not billed on", ComparisonMode.Substring);
		}

		[Then]
		public void for_specified_date_should_tell_whether_client_was_billed_on_it()
		{
			analyzer.WasBilledOn(new DateTime(2011, 6, 1)).Should().BeFalse();
			analyzer.WasBilledOn(new DateTime(2011, 7, 1)).Should().BeTrue();
			analyzer.WasBilledOn(new DateTime(2011, 8, 1)).Should().BeTrue();
			analyzer.WasBilledOn(new DateTime(2011, 9, 1)).Should().BeFalse();
		}

		[Then]
		public void should_get_invoices_of_specified_billing_run()
		{
			analyzer.GetInvoiceSummary(new DateTime(2011, 7, 1)).Select(inv => inv.InvoiceID)
				.Should().BeEquivalentTo(1);
			analyzer.GetInvoiceSummary(new DateTime(2011, 8, 1)).Select(inv => inv.InvoiceID)
				.Should().BeEquivalentTo(2, 3);
		}

		[Then]
		public void when_getting_invoice_on_date_of_nonexistent_billing_run_should_return_empty_collection()
		{
			analyzer.GetInvoiceSummary(new DateTime(2011, 6, 1)).Should().BeEmpty();
		}

		[Then]
		public void should_get_dates_of_all_billing_runs()
		{
			analyzer.GetDatesOfAllBillingRuns().Should().BeEquivalentTo(new DateTime(2011, 7, 1), new DateTime(2011, 8, 1));
		}
	}
}