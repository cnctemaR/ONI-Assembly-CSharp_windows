using System;

namespace System.Diagnostics.Tracing
{
	[EventSource(Name = "Microsoft.Tasks.Nuget")]
	internal class TplEtwProvider : EventSource
	{
		public bool Debug
		{
			get
			{
				return base.IsEnabled(EventLevel.Verbose, (EventKeywords)1L);
			}
		}

		public void DebugFacilityMessage(string Facility, string Message)
		{
			base.WriteEvent(1, Facility, Message);
		}

		public void DebugFacilityMessage1(string Facility, string Message, string Arg)
		{
			base.WriteEvent(2, Facility, Message, Arg);
		}

		public void SetActivityId(Guid Id)
		{
			base.WriteEvent(3, new object[] { Id });
		}

		public static TplEtwProvider Log = new TplEtwProvider();

		public class Keywords
		{
			public const EventKeywords Debug = (EventKeywords)1L;
		}
	}
}
