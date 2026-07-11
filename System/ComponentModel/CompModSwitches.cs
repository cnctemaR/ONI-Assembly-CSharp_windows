using System;
using System.Diagnostics;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	internal static class CompModSwitches
	{
		public static BooleanSwitch CommonDesignerServices
		{
			get
			{
				if (CompModSwitches.commonDesignerServices == null)
				{
					CompModSwitches.commonDesignerServices = new BooleanSwitch("CommonDesignerServices", "Assert if any common designer service is not found.");
				}
				return CompModSwitches.commonDesignerServices;
			}
		}

		public static TraceSwitch EventLog
		{
			get
			{
				if (CompModSwitches.eventLog == null)
				{
					CompModSwitches.eventLog = new TraceSwitch("EventLog", "Enable tracing for the EventLog component.");
				}
				return CompModSwitches.eventLog;
			}
		}

		private static volatile BooleanSwitch commonDesignerServices;

		private static volatile TraceSwitch eventLog;
	}
}
