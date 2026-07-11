using System;
using System.Diagnostics;

namespace System.Runtime
{
	internal class TraceLevelHelper
	{
		internal static TraceEventType GetTraceEventType(byte level, byte opcode)
		{
			if (opcode <= 2)
			{
				if (opcode == 1)
				{
					return TraceEventType.Start;
				}
				if (opcode == 2)
				{
					return TraceEventType.Stop;
				}
			}
			else
			{
				if (opcode == 7)
				{
					return TraceEventType.Resume;
				}
				if (opcode == 8)
				{
					return TraceEventType.Suspend;
				}
			}
			return TraceLevelHelper.EtwLevelToTraceEventType[(int)level];
		}

		internal static TraceEventType GetTraceEventType(TraceEventLevel level)
		{
			return TraceLevelHelper.EtwLevelToTraceEventType[(int)level];
		}

		internal static TraceEventType GetTraceEventType(byte level)
		{
			return TraceLevelHelper.EtwLevelToTraceEventType[(int)level];
		}

		internal static string LookupSeverity(TraceEventLevel level, TraceEventOpcode opcode)
		{
			if (opcode <= TraceEventOpcode.Stop)
			{
				if (opcode == TraceEventOpcode.Start)
				{
					return "Start";
				}
				if (opcode == TraceEventOpcode.Stop)
				{
					return "Stop";
				}
			}
			else
			{
				if (opcode == TraceEventOpcode.Resume)
				{
					return "Resume";
				}
				if (opcode == TraceEventOpcode.Suspend)
				{
					return "Suspend";
				}
			}
			string text;
			switch (level)
			{
			case TraceEventLevel.Critical:
				text = "Critical";
				break;
			case TraceEventLevel.Error:
				text = "Error";
				break;
			case TraceEventLevel.Warning:
				text = "Warning";
				break;
			case TraceEventLevel.Informational:
				text = "Information";
				break;
			case TraceEventLevel.Verbose:
				text = "Verbose";
				break;
			default:
				text = level.ToString();
				break;
			}
			return text;
		}

		private static TraceEventType[] EtwLevelToTraceEventType = new TraceEventType[]
		{
			TraceEventType.Critical,
			TraceEventType.Critical,
			TraceEventType.Error,
			TraceEventType.Warning,
			TraceEventType.Information,
			TraceEventType.Verbose
		};
	}
}
