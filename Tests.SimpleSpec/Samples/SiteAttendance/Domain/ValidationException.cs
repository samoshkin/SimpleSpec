using System;
using System.Runtime.Serialization;

namespace Tests.SimpleSpec.Samples.SiteAttendance.Domain
{
	[Serializable] 
	public class ValidationException : Exception
	{
		public ValidationException()
		{}

		public ValidationException(string message) : base(message)
		{}

		public ValidationException(string message, Exception inner) : base(message, inner)
		{}

		protected ValidationException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{}
	}
}