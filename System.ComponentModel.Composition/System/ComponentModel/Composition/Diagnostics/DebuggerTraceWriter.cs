using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.ComponentModel.Composition.Diagnostics
{
	internal sealed class DebuggerTraceWriter : TraceWriter
	{
		public override bool CanWriteInformation
		{
			get
			{
				return false;
			}
		}

		public override bool CanWriteWarning
		{
			get
			{
				return Debugger.IsLogging();
			}
		}

		public override bool CanWriteError
		{
			get
			{
				return Debugger.IsLogging();
			}
		}

		public override void WriteInformation(CompositionTraceId traceId, string format, params object[] arguments)
		{
			DebuggerTraceWriter.WriteEvent(DebuggerTraceWriter.TraceEventType.Information, traceId, format, arguments);
		}

		public override void WriteWarning(CompositionTraceId traceId, string format, params object[] arguments)
		{
			DebuggerTraceWriter.WriteEvent(DebuggerTraceWriter.TraceEventType.Warning, traceId, format, arguments);
		}

		public override void WriteError(CompositionTraceId traceId, string format, params object[] arguments)
		{
			DebuggerTraceWriter.WriteEvent(DebuggerTraceWriter.TraceEventType.Error, traceId, format, arguments);
		}

		private static void WriteEvent(DebuggerTraceWriter.TraceEventType eventType, CompositionTraceId traceId, string format, params object[] arguments)
		{
			if (!Debugger.IsLogging())
			{
				return;
			}
			string text = DebuggerTraceWriter.CreateLogMessage(eventType, traceId, format, arguments);
			Debugger.Log(0, null, text);
		}

		internal static string CreateLogMessage(DebuggerTraceWriter.TraceEventType eventType, CompositionTraceId traceId, string format, params object[] arguments)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}: {2} : ", DebuggerTraceWriter.SourceName, eventType.ToString(), (int)traceId);
			if (arguments == null)
			{
				stringBuilder.Append(format);
			}
			else
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, format, arguments);
			}
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		private static readonly string SourceName = "System.ComponentModel.Composition";

		internal enum TraceEventType
		{
			Error = 2,
			Warning = 4,
			Information = 8
		}
	}
}
