using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.SimpleSpec.Samples.BillingStatistics.Domain
{
	public class BillingStatisticsAnalyzer
	{
		private SortedList<DateTime, List<InvoiceSummary>> _billingRuns;
		private readonly IBillingDataProvider _billingDataProvider;

		public BillingStatisticsAnalyzer(IBillingDataProvider billingDataProvider)
		{
			_billingDataProvider = billingDataProvider;
		}

		public Client Client { get; private set; }
		public IEnumerable<InvoiceSummary> Invoices 
		{
			get { return _billingRuns.Values.SelectMany(invoices => invoices); } 
		}

		public virtual void LoadClientStatistics(Client client)
		{
			Client = client;
			_billingRuns = new SortedList<DateTime, List<InvoiceSummary>>(
				_billingDataProvider.FetchClientStatistics(client)
					.GroupBy(inv => inv.Date)
					.ToDictionary(gr => gr.Key, gr => gr.ToList()));
		}

		public virtual IEnumerable<InvoiceSummary> GetInvoiceSummary(DateTime billingRunDate)
		{
			return _billingRuns.ContainsKey(billingRunDate)
				? _billingRuns[billingRunDate]
				: Enumerable.Empty<InvoiceSummary>();
		}

		public virtual bool WasBilledOn(DateTime billingRunDate)
		{
			return _billingRuns.ContainsKey(billingRunDate);
		}

		public virtual bool IsFirstBillingRun(DateTime billingRunDate)
		{
			if(!_billingRuns.ContainsKey(billingRunDate))
			{
				throw new ArgumentException(String.Format("Client was not billed on {0}", billingRunDate));
			}
			return _billingRuns.Keys.First() == billingRunDate;
		}

		public virtual double CalculateRevenueVariationBetweenPreviousInvoice(DateTime billingRunDate)
		{
			var indexOfPrevBillingRun = _billingRuns.Keys.IndexOf(billingRunDate) - 1;
			if (indexOfPrevBillingRun < 0)
			{
				throw new ArgumentOutOfRangeException("billingRunDate");
			}
			var prevBillingRun = _billingRuns.Values[indexOfPrevBillingRun];
			var billingRun = _billingRuns[billingRunDate];
			double variation = 0.0;
			var currencies = prevBillingRun.Select(inv => inv.TotalAmount.Currency)
				.Concat(billingRun.Select(inv => inv.TotalAmount.Currency))
				.Distinct()
				.ToList();
			foreach (var currency in currencies)
			{
				var prevBillingRunAmount = prevBillingRun
					.Where(inv => inv.TotalAmount.Currency == currency)
					.Sum(inv => inv.TotalAmount.Amount);
				var curBillingRunAmount = billingRun
					.Where(inv => inv.TotalAmount.Currency == currency)
					.Sum(inv => inv.TotalAmount.Amount);
				if (prevBillingRunAmount == 0.0m && curBillingRunAmount == 0.0m)
				{
					continue;
				}
				prevBillingRunAmount = prevBillingRunAmount == 0.0m ? 1.0m : prevBillingRunAmount;
				variation += (double)((curBillingRunAmount - prevBillingRunAmount) / (prevBillingRunAmount * currencies.Count));
			}
			return variation;
		}

		public virtual IEnumerable<DateTime> GetDatesOfAllBillingRuns()
		{
			return _billingRuns.Keys;
		}
	}
}
