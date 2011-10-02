using System;
using FluentAssertions;
using FluentAssertions.Assertions;
using SimpleSpec.NUnit;
using Tests.SimpleSpec.Samples.SiteAttendance.Domain;

namespace Tests.SimpleSpec.Samples.SiteAttendance
{
	[Scenario.Spec]
	public class when_calculating_visit_count_variation : with_site_attendance_analyzer
	{
		double visitVariation;

		public when_calculating_visit_count_variation()
		{
			Given(attendance_statistics_analyzer);
			Given(() => resource_attendance_statistics(
				new AttendanceSummary(new DateTime(2011, 6, 1), 6, TimeSpan.FromMinutes(1), 2, 12),
				new AttendanceSummary(new DateTime(2011, 6, 2), 4, TimeSpan.FromMinutes(2), 10, 123),
				new AttendanceSummary(new DateTime(2011, 6, 3), 5, TimeSpan.FromSeconds(50), 4, 6)));
			Given(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 6, 1), new DateTime(2011, 6, 3)));
			When(() => visitVariation = analyzer.CalculateVisitVariation(new DateTime(2011, 6, 2)));
		}

		[Behavior]
		public void should_calculate_variation_comparing_current_day_visit_count_to_previous_one()
		{
			visitVariation.Should().BeApproximately(-0.333, 0.001);
		}
	}

	[Scenario.Spec]
	public class when_calculating_visit_count_variation_and_no_attendance_data_for_previous_day : with_site_attendance_analyzer
	{
		double visitVariation;

		public when_calculating_visit_count_variation_and_no_attendance_data_for_previous_day()
		{
			Given(attendance_statistics_analyzer);
			Given(() => resource_attendance_statistics(
				new AttendanceSummary(new DateTime(2011, 6, 1), 4, TimeSpan.FromMinutes(1), 2, 12),
				new AttendanceSummary(new DateTime(2011, 6, 3), 8, TimeSpan.FromMinutes(2), 10, 123),
				new AttendanceSummary(new DateTime(2011, 6, 4), 5, TimeSpan.FromSeconds(50), 4, 6)));
			Given(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 6, 1), new DateTime(2011, 6, 3)));
			When(() => visitVariation = analyzer.CalculateVisitVariation(new DateTime(2011, 6, 3)));
		}

		[Behavior]
		public void should_calculate_variation_comparing_current_day_visit_count_to_the_most_recent_date_with_data_available()
		{
			visitVariation.Should().BeApproximately(1.0, 0.001);
		}
	}

	[Scenario.Spec]
	public class when_calculating_visit_count_variation_for_the_first_day_of_the_statistic_period : with_site_attendance_analyzer
	{
		public when_calculating_visit_count_variation_for_the_first_day_of_the_statistic_period()
		{
			Given(attendance_statistics_analyzer);
			Given(() => resource_attendance_statistics(
				new AttendanceSummary(new DateTime(2011, 6, 1), 4, TimeSpan.FromMinutes(1), 2, 12),
				new AttendanceSummary(new DateTime(2011, 6, 3), 8, TimeSpan.FromMinutes(2), 10, 123),
				new AttendanceSummary(new DateTime(2011, 6, 4), 5, TimeSpan.FromSeconds(50), 4, 6)));
			Given(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 6, 1), new DateTime(2011, 6, 3)));
			When(() => analyzer.CalculateVisitVariation(new DateTime(2011, 6, 1)));
			CouldFailWith<ArgumentException>();
		}

		[Behavior]
		public void should_fail()
		{
			Failure.Message.Should().Contain("is the first date in the period");
		}
	}

	[Scenario.Spec]
	public class when_calculating_visit_count_variation_for_a_day_with_no_attendance_data : with_site_attendance_analyzer
	{
		public when_calculating_visit_count_variation_for_a_day_with_no_attendance_data()
		{
			Given(attendance_statistics_analyzer);
			Given(() => resource_attendance_statistics(
				new AttendanceSummary(new DateTime(2011, 6, 1), 4, TimeSpan.FromMinutes(1), 2, 12),
				new AttendanceSummary(new DateTime(2011, 6, 3), 8, TimeSpan.FromMinutes(2), 10, 123),
				new AttendanceSummary(new DateTime(2011, 6, 4), 5, TimeSpan.FromSeconds(50), 4, 6)));
			Given(() => analyzer.LoadClientStatistics(resource, new DateTime(2011, 6, 1), new DateTime(2011, 6, 3)));
			When(() => analyzer.CalculateVisitVariation(new DateTime(2011, 6, 2)));
			CouldFailWith<ArgumentException>();
		}

		[Behavior]
		public void should_fail()
		{
			Failure.Message.Should().Contain("does not have attendance statistiscs");
		}
	}
}