using System;
using FluentAssertions;
using SimpleSpec.NUnit;
using Tests.SimpleSpec.Samples.SiteAttendance.Domain;

namespace Tests.SimpleSpec.Samples.SiteAttendance
{
	[Behavior.Spec]
	public class attendance_data_should_not_pass_validation : with_site_attendance_analyzer
	{
		public attendance_data_should_not_pass_validation()
		{
			Given(attendance_statistics_analyzer);
			CouldFailWith<ValidationException>();

			Then(() => Failure.Should().NotBeNull());
		}

		[Scenario]
		public void when_visit_variation_threshold_is_exceeded()
		{
			Given(() => resource_attendance_statistics(
				new AttendanceSummary(new DateTime(2011, 7, 1), 10, TimeSpan.FromMinutes(1), 2, 13),
				new AttendanceSummary(new DateTime(2011, 7, 2), 15, TimeSpan.FromMinutes(2), 3, 15),
				new AttendanceSummary(new DateTime(2011, 7, 3), 16, TimeSpan.FromMinutes(3), 4, 16)));
			Given(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 1, 1), new DateTime(2012, 1, 1)));
			When(() => analyzer.Validate(0.4));
		}

		[Scenario]
		public void when_statistics_is_not_contiguous_with_some_days_missing()
		{
			Given(() => resource_attendance_statistics(
				new AttendanceSummary(new DateTime(2011, 7, 1), 10, TimeSpan.FromMinutes(1), 2, 13),
				new AttendanceSummary(new DateTime(2011, 7, 3), 11, TimeSpan.FromMinutes(2), 3, 15),
				new AttendanceSummary(new DateTime(2011, 7, 4), 12, TimeSpan.FromMinutes(3), 4, 16)));
			Given(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 1, 1), new DateTime(2012, 1, 1)));
			When(() => analyzer.Validate(0.4));
		}
	}
}