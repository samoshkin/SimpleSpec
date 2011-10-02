using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using Moq;
using SimpleSpec.NUnit;
using Tests.SimpleSpec.Samples.SiteAttendance.Domain;

namespace Tests.SimpleSpec.Samples.SiteAttendance
{
	public class with_site_attendance_analyzer : specification
	{
		public SiteAttendanceAnalyzer analyzer;
		public Mock<IAttendanceStatisticsProvider> billingDataProvider;
		public Resource resource;

		protected void attendance_statistics_analyzer()
		{
			resource = new Resource { Uri = new Uri("http://bdd.com/simplespect") };
			billingDataProvider = new Mock<IAttendanceStatisticsProvider>();
			analyzer = new SiteAttendanceAnalyzer(billingDataProvider.Object);
		}

		protected void resource_attendance_statistics(params AttendanceSummary[] attendances)
		{
			billingDataProvider
				.Setup(provider => provider.FetchStatistics(resource, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
				.Returns(attendances);
		}

		protected void resource_attendance_statistics(DateTime startDate, DateTime endDate, params AttendanceSummary[] attendances)
		{
			billingDataProvider
				.Setup(provider => provider.FetchStatistics(resource, startDate, endDate))
				.Returns(attendances);
		}
	}

	[Scenario.Spec]
	public class when_attendance_statistics_is_being_loaded_for_the_client : with_site_attendance_analyzer
	{
		AttendanceSummary _june1Attendance, _june2Attendance, _june3Attendance;

		public when_attendance_statistics_is_being_loaded_for_the_client()
		{
			_june1Attendance = new AttendanceSummary(new DateTime(2011, 6, 1), 6, TimeSpan.FromMinutes(1), 2, 12);
			_june2Attendance = new AttendanceSummary(new DateTime(2011, 6, 2), 4, TimeSpan.FromMinutes(2), 10, 123);
			_june3Attendance = new AttendanceSummary(new DateTime(2011, 6, 3), 5, TimeSpan.FromSeconds(50), 4, 6);

			Given(attendance_statistics_analyzer);
			Given(() => resource_attendance_statistics(
				new DateTime(2011, 6, 1), 
				new DateTime(2011, 6, 3),
				_june1Attendance, _june2Attendance, _june3Attendance));
			When(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 6, 1), new DateTime(2011, 6, 3)));
		}

		[Behavior]
		public void should_get_statistics_data_from_attendance_data_provider()
		{
			billingDataProvider.Verify();
			analyzer.Statistics.Should().BeEquivalentTo(_june1Attendance, _june2Attendance, _june3Attendance);
		}
	}

	[Behavior.Spec]
	public class attendance_analysis_behavior : with_site_attendance_analyzer
	{
		public attendance_analysis_behavior()
		{
			Given(attendance_statistics_analyzer);
			Given(() => resource_attendance_statistics(
			    new AttendanceSummary(new DateTime(2011, 7, 1), 10, TimeSpan.FromMinutes(1), 2, 13),
				new AttendanceSummary(new DateTime(2011, 7, 3), 15, TimeSpan.FromMinutes(2), 3, 15),
				new AttendanceSummary(new DateTime(2011, 7, 4), 20, TimeSpan.FromMinutes(3), 4, 16)));
			Given(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 7, 1), new DateTime(2011, 7, 3)));
		}

		[Behavior]
		public void should_get_attendance_info_for_given_date()
		{
			analyzer.GetAttendanceOn(new DateTime(2011, 7, 1)).Visits.Should().Be(10);
			analyzer.GetAttendanceOn(new DateTime(2011, 7, 3)).Visits.Should().Be(15);
			analyzer.GetAttendanceOn(new DateTime(2011, 7, 4)).Visits.Should().Be(20);
		}

		[Behavior]
		public void when_getting_attendance_info_for_date_with_no_statistics_should_fail()
		{
			analyzer.Invoking(a => a.GetAttendanceOn(new DateTime(2011, 7, 2)))
				.ShouldThrow<ArgumentException>()
				.WithMessage("No attendance information on", ComparisonMode.Substring);
		}

		[Behavior]
		public void should_tell_whether_date_belongs_to_statistics_period()
		{
			analyzer.BelongsToPeriod(new DateTime(2011, 6, 30)).Should().BeFalse();
			analyzer.BelongsToPeriod(new DateTime(2011, 7, 1)).Should().BeTrue();
			analyzer.BelongsToPeriod(new DateTime(2011, 7, 2)).Should().BeTrue();
			analyzer.BelongsToPeriod(new DateTime(2011, 7, 3)).Should().BeTrue();
			analyzer.BelongsToPeriod(new DateTime(2011, 7, 4)).Should().BeFalse();
		}

		[Behavior]
		public void should_tell_whether_has_attendace_date_for_given_date()
		{
			analyzer.HasAttendanceDataOn(new DateTime(2011, 6, 30)).Should().BeFalse();
			analyzer.HasAttendanceDataOn(new DateTime(2011, 7, 1)).Should().BeTrue();
			analyzer.HasAttendanceDataOn(new DateTime(2011, 7, 2)).Should().BeFalse();
			analyzer.HasAttendanceDataOn(new DateTime(2011, 7, 3)).Should().BeTrue();
			analyzer.HasAttendanceDataOn(new DateTime(2011, 7, 4)).Should().BeTrue();
		}
	}
}