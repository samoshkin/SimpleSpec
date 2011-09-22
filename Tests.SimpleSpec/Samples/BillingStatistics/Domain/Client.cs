using System;

namespace Tests.SimpleSpec.Samples.BillingStatistics.Domain
{
	public class Client
	{
		public Client(int id, string code, DateTime launchDate)
		{
			ID = id;
			Code = code;
			LaunchDate = launchDate;
		}

		public int ID { get; set; }
		public string Code { get; set; }
		public DateTime LaunchDate { get; set; }
	}
}