using System;
using System.Collections.Generic;

namespace Tests.SimpleSpec.Samples.SiteAttendance.Domain
{
	public interface IAttendanceStatisticsProvider
	{
		IEnumerable<AttendanceSummary> FetchStatistics(Resource resource, DateTime startDate, DateTime endDate);
	}
}