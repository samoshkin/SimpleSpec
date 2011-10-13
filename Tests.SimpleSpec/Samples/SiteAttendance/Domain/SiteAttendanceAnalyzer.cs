using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.SimpleSpec.Samples.SiteAttendance.Domain
{
	public class SiteAttendanceAnalyzer
	{
		private SortedList<DateTime, AttendanceSummary> _dayAttendanceStatistics;
		private readonly IAttendanceStatisticsProvider _attendanceStatisticsProvider;
		private DateTime _startDate;
		private DateTime _endDate;

		public SiteAttendanceAnalyzer(IAttendanceStatisticsProvider attendanceStatisticsProvider)
		{
			_attendanceStatisticsProvider = attendanceStatisticsProvider;
		}

		public IEnumerable<AttendanceSummary> Statistics 
		{
			get { return _dayAttendanceStatistics.Values; } 
		}

		public virtual void LoadClientStatistics(Resource resource, DateTime startDate, DateTime endDate)
		{
			_dayAttendanceStatistics = new SortedList<DateTime, AttendanceSummary>(_attendanceStatisticsProvider
				.FetchStatistics(resource, startDate, endDate)
				.ToDictionary(gr => gr.Date.Date));
			_startDate = startDate;
			_endDate = endDate;
		}

		public bool BelongsToPeriod(DateTime date)
		{
			return _startDate <= date && date <= _endDate;
		}

		public bool HasAttendanceDataOn(DateTime date)
		{
			return _dayAttendanceStatistics.ContainsKey(date);
		}

		public virtual AttendanceSummary GetAttendanceOn(DateTime dateTime)
		{
			var startOfTheDay = dateTime.Date;
			if (!_dayAttendanceStatistics.ContainsKey(startOfTheDay))
			{
				throw new ArgumentException(String.Format("No attendance information on {0}", dateTime));
			}
			return _dayAttendanceStatistics[startOfTheDay];
		}

		public void Validate(double variationThreshold)
		{
			var firstDay = _dayAttendanceStatistics.Keys.First();
			var lastDay = _dayAttendanceStatistics.Keys.Last();
			var totalDayCount = _dayAttendanceStatistics.Count;

			if(lastDay.Subtract(firstDay).TotalDays + 1 != totalDayCount)
			{
				throw new ValidationException("Statistics is not contiguous. Data is missing on some days.");
			}

			var violatesThreshold = _dayAttendanceStatistics.Keys
				.Skip(1)
				.Select(CalculateVisitVariation)
				.Where(variation => Math.Abs(variation) > variationThreshold)
				.Any();
			if(violatesThreshold)
			{
				throw new ValidationException("Actual visit variation violates given threshold.");
			}
		}
		
		public virtual double CalculateVisitVariation(DateTime day)
		{
			var date = day.Date;
			if(!_dayAttendanceStatistics.ContainsKey(day))
			{
				throw new ArgumentException(String.Format("Day specified {0} does not have attendance statistiscs.", date));
			}
			var indexOfADay = _dayAttendanceStatistics.Keys.IndexOf(date);
			if(indexOfADay -1 < 0)
			{
				throw new ArgumentException(String.Format("Day specified {0} is the first date in the period.", date));
			}

			var prevDayAttendanceInfo = _dayAttendanceStatistics.Values[indexOfADay - 1];
			var currDayAttendanceInfo = _dayAttendanceStatistics.Values[indexOfADay];
			var prevDayVisits = prevDayAttendanceInfo.Visits;
			var curDayVisits = currDayAttendanceInfo.Visits;
			if(prevDayVisits == 0 && curDayVisits == 0)
			{
				return 0;
			}
			prevDayVisits = prevDayVisits == 0 ? 1 : prevDayVisits;
			return (double)(curDayVisits - prevDayVisits) / prevDayVisits;
		}
	}
}
