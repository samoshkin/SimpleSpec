using System.Collections.Generic;

namespace Tests.SimpleSpec.Samples.BillingStatistics.Domain
{
	public interface IBillingDataProvider
	{
		IEnumerable<InvoiceSummary> FetchClientStatistics(Client client);
	}
}