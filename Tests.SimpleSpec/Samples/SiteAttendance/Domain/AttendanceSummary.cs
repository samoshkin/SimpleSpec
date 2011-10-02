using System;

namespace Tests.SimpleSpec.Samples.SiteAttendance.Domain
{
	public class AttendanceSummary
	{
		public AttendanceSummary(DateTime date)
		{
			Date = date;
		}

		public AttendanceSummary(DateTime date, int visits, TimeSpan timeOnSite, int pagesPerVisit, int pageViews)
			: this(date)
		{
			Visits = visits;
			TimeOnSite = timeOnSite;
			PagesPerVisit = pagesPerVisit;
			PageViews = pageViews;
		}

		public DateTime Date { get; set; }
		public int Visits { get; set; }
		public TimeSpan TimeOnSite { get; set; }
		public int PagesPerVisit { get; set; }
		public int PageViews { get; set; }
	}
}