namespace Tests.SimpleSpec.Samples.BillingStatistics.Domain
{
	public class Currency
	{
		private static readonly Currency _usd =
			new Currency("USD", "$", "Dollar");
		private static readonly Currency _euro =
			new Currency("EUR", "€", "Euro");
		private static readonly Currency _poundSterling =
			new Currency("GBP", "£", "Pound Sterling");
		private static readonly Currency _yena =
			new Currency("JPY", "¥", "Yena");

		private Currency()
		{}

		public Currency(string name, string symbol, string description)
		{
			Name = name;
			Symbol = symbol;
			Description = description;
		}
		

		public static Currency USD { get { return _usd; } }
		public static Currency EUR { get { return _euro; } }
		public static Currency GBP { get { return _poundSterling; } }
		public static Currency JPY { get { return _yena; } }

		public string Name { get; private set; }
		public string Symbol { get; private set; }
		public string Description { get; private set; }

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public bool Equals(Currency other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Name, Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Currency)) return false;
			return Equals((Currency) obj);
		}

		public static bool operator ==(Currency left, Currency right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Currency left, Currency right)
		{
			return !(left == right);
		}
	}
}