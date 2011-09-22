using System;

namespace Tests.SimpleSpec.Samples.BillingStatistics.Domain
{
	public class InvoiceSummary
	{
		public InvoiceSummary(Client client, int invoiceID, DateTime date, int unitCount, int orderCount, Money totalAmount)
		{
			Client = client;
			InvoiceID = invoiceID;
			Date = date;
			UnitCount = unitCount;
			OrderCount = orderCount;
			TotalAmount = totalAmount;
		}

		public InvoiceSummary(Client client)
		{
			Client = client;
		}

		public Client Client { get; private set; }

		public int InvoiceID { get; set; }
		public DateTime Date { get; set; }
		public int UnitCount { get; set; }
		public int OrderCount { get; set; }
		public Money TotalAmount { get; set; }
	}
}