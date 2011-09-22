namespace Tests.SimpleSpec.Samples.BillingStatistics.Domain
{
	public class Money
	{
		public Money(decimal amount, Currency currency)
		{
			Amount = amount;
			Currency = currency;
		}

		public Currency Currency { get; private set; }
		public decimal Amount { get; private set; }
	}
}