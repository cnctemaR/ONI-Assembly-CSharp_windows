using System;
using System.Globalization;
using System.Resources;
using System.Runtime.Diagnostics;
using System.Security;

namespace System.Runtime
{
	internal class TraceCore
	{
		private TraceCore()
		{
		}

		private static ResourceManager ResourceManager
		{
			get
			{
				if (TraceCore.resourceManager == null)
				{
					TraceCore.resourceManager = new ResourceManager("System.Runtime.TraceCore", typeof(TraceCore).Assembly);
				}
				return TraceCore.resourceManager;
			}
		}

		internal static CultureInfo Culture
		{
			get
			{
				return TraceCore.resourceCulture;
			}
			set
			{
				TraceCore.resourceCulture = value;
			}
		}

		internal static bool AppDomainUnloadIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Informational) || TraceCore.IsEtwEventEnabled(trace, 0);
		}

		internal static void AppDomainUnload(EtwDiagnosticTrace trace, string appdomainName, string processName, string processId)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, null);
			if (TraceCore.IsEtwEventEnabled(trace, 0))
			{
				TraceCore.WriteEtwEvent(trace, 0, null, appdomainName, processName, processId, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Informational))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("AppDomainUnload", TraceCore.Culture), appdomainName, processName, processId);
				TraceCore.WriteTraceSource(trace, 0, text, serializedPayload);
			}
		}

		internal static bool HandledExceptionIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Informational) || TraceCore.IsEtwEventEnabled(trace, 1);
		}

		internal static void HandledException(EtwDiagnosticTrace trace, string param0, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 1))
			{
				TraceCore.WriteEtwEvent(trace, 1, null, param0, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Informational))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("HandledException", TraceCore.Culture), param0);
				TraceCore.WriteTraceSource(trace, 1, text, serializedPayload);
			}
		}

		internal static bool ShipAssertExceptionMessageIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Error) || TraceCore.IsEtwEventEnabled(trace, 2);
		}

		internal static void ShipAssertExceptionMessage(EtwDiagnosticTrace trace, string param0)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, null);
			if (TraceCore.IsEtwEventEnabled(trace, 2))
			{
				TraceCore.WriteEtwEvent(trace, 2, null, param0, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Error))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("ShipAssertExceptionMessage", TraceCore.Culture), param0);
				TraceCore.WriteTraceSource(trace, 2, text, serializedPayload);
			}
		}

		internal static bool ThrowingExceptionIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Warning) || TraceCore.IsEtwEventEnabled(trace, 3);
		}

		internal static void ThrowingException(EtwDiagnosticTrace trace, string param0, string param1, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 3))
			{
				TraceCore.WriteEtwEvent(trace, 3, null, param0, param1, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Warning))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("ThrowingException", TraceCore.Culture), param0, param1);
				TraceCore.WriteTraceSource(trace, 3, text, serializedPayload);
			}
		}

		internal static bool UnhandledExceptionIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Critical) || TraceCore.IsEtwEventEnabled(trace, 4);
		}

		internal static void UnhandledException(EtwDiagnosticTrace trace, string param0, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 4))
			{
				TraceCore.WriteEtwEvent(trace, 4, null, param0, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Critical))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("UnhandledException", TraceCore.Culture), param0);
				TraceCore.WriteTraceSource(trace, 4, text, serializedPayload);
			}
		}

		internal static bool TraceCodeEventLogCriticalIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Critical) || TraceCore.IsEtwEventEnabled(trace, 5);
		}

		internal static void TraceCodeEventLogCritical(EtwDiagnosticTrace trace, TraceRecord traceRecord)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, traceRecord, null);
			if (TraceCore.IsEtwEventEnabled(trace, 5))
			{
				TraceCore.WriteEtwEvent(trace, 5, null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Critical))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("TraceCodeEventLogCritical", TraceCore.Culture), Array.Empty<object>());
				TraceCore.WriteTraceSource(trace, 5, text, serializedPayload);
			}
		}

		internal static bool TraceCodeEventLogErrorIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Error) || TraceCore.IsEtwEventEnabled(trace, 6);
		}

		internal static void TraceCodeEventLogError(EtwDiagnosticTrace trace, TraceRecord traceRecord)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, traceRecord, null);
			if (TraceCore.IsEtwEventEnabled(trace, 6))
			{
				TraceCore.WriteEtwEvent(trace, 6, null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Error))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("TraceCodeEventLogError", TraceCore.Culture), Array.Empty<object>());
				TraceCore.WriteTraceSource(trace, 6, text, serializedPayload);
			}
		}

		internal static bool TraceCodeEventLogInfoIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Informational) || TraceCore.IsEtwEventEnabled(trace, 7);
		}

		internal static void TraceCodeEventLogInfo(EtwDiagnosticTrace trace, TraceRecord traceRecord)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, traceRecord, null);
			if (TraceCore.IsEtwEventEnabled(trace, 7))
			{
				TraceCore.WriteEtwEvent(trace, 7, null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Informational))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("TraceCodeEventLogInfo", TraceCore.Culture), Array.Empty<object>());
				TraceCore.WriteTraceSource(trace, 7, text, serializedPayload);
			}
		}

		internal static bool TraceCodeEventLogVerboseIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Verbose) || TraceCore.IsEtwEventEnabled(trace, 8);
		}

		internal static void TraceCodeEventLogVerbose(EtwDiagnosticTrace trace, TraceRecord traceRecord)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, traceRecord, null);
			if (TraceCore.IsEtwEventEnabled(trace, 8))
			{
				TraceCore.WriteEtwEvent(trace, 8, null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Verbose))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("TraceCodeEventLogVerbose", TraceCore.Culture), Array.Empty<object>());
				TraceCore.WriteTraceSource(trace, 8, text, serializedPayload);
			}
		}

		internal static bool TraceCodeEventLogWarningIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Warning) || TraceCore.IsEtwEventEnabled(trace, 9);
		}

		internal static void TraceCodeEventLogWarning(EtwDiagnosticTrace trace, TraceRecord traceRecord)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, traceRecord, null);
			if (TraceCore.IsEtwEventEnabled(trace, 9))
			{
				TraceCore.WriteEtwEvent(trace, 9, null, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Warning))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("TraceCodeEventLogWarning", TraceCore.Culture), Array.Empty<object>());
				TraceCore.WriteTraceSource(trace, 9, text, serializedPayload);
			}
		}

		internal static bool HandledExceptionWarningIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Warning) || TraceCore.IsEtwEventEnabled(trace, 10);
		}

		internal static void HandledExceptionWarning(EtwDiagnosticTrace trace, string param0, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 10))
			{
				TraceCore.WriteEtwEvent(trace, 10, null, param0, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Warning))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("HandledExceptionWarning", TraceCore.Culture), param0);
				TraceCore.WriteTraceSource(trace, 10, text, serializedPayload);
			}
		}

		internal static bool BufferPoolAllocationIsEnabled(EtwDiagnosticTrace trace)
		{
			return TraceCore.IsEtwEventEnabled(trace, 11);
		}

		internal static void BufferPoolAllocation(EtwDiagnosticTrace trace, int Size)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, null);
			if (TraceCore.IsEtwEventEnabled(trace, 11))
			{
				TraceCore.WriteEtwEvent(trace, 11, null, Size, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool BufferPoolChangeQuotaIsEnabled(EtwDiagnosticTrace trace)
		{
			return TraceCore.IsEtwEventEnabled(trace, 12);
		}

		internal static void BufferPoolChangeQuota(EtwDiagnosticTrace trace, int PoolSize, int Delta)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, null);
			if (TraceCore.IsEtwEventEnabled(trace, 12))
			{
				TraceCore.WriteEtwEvent(trace, 12, null, PoolSize, Delta, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool ActionItemScheduledIsEnabled(EtwDiagnosticTrace trace)
		{
			return TraceCore.IsEtwEventEnabled(trace, 13);
		}

		internal static void ActionItemScheduled(EtwDiagnosticTrace trace, EventTraceActivity eventTraceActivity)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, null);
			if (TraceCore.IsEtwEventEnabled(trace, 13))
			{
				TraceCore.WriteEtwEvent(trace, 13, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool ActionItemCallbackInvokedIsEnabled(EtwDiagnosticTrace trace)
		{
			return TraceCore.IsEtwEventEnabled(trace, 14);
		}

		internal static void ActionItemCallbackInvoked(EtwDiagnosticTrace trace, EventTraceActivity eventTraceActivity)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, null);
			if (TraceCore.IsEtwEventEnabled(trace, 14))
			{
				TraceCore.WriteEtwEvent(trace, 14, eventTraceActivity, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool HandledExceptionErrorIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Error) || TraceCore.IsEtwEventEnabled(trace, 15);
		}

		internal static void HandledExceptionError(EtwDiagnosticTrace trace, string param0, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 15))
			{
				TraceCore.WriteEtwEvent(trace, 15, null, param0, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Error))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("HandledExceptionError", TraceCore.Culture), param0);
				TraceCore.WriteTraceSource(trace, 15, text, serializedPayload);
			}
		}

		internal static bool HandledExceptionVerboseIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Verbose) || TraceCore.IsEtwEventEnabled(trace, 16);
		}

		internal static void HandledExceptionVerbose(EtwDiagnosticTrace trace, string param0, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 16))
			{
				TraceCore.WriteEtwEvent(trace, 16, null, param0, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Verbose))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("HandledExceptionVerbose", TraceCore.Culture), param0);
				TraceCore.WriteTraceSource(trace, 16, text, serializedPayload);
			}
		}

		internal static bool EtwUnhandledExceptionIsEnabled(EtwDiagnosticTrace trace)
		{
			return TraceCore.IsEtwEventEnabled(trace, 17);
		}

		internal static void EtwUnhandledException(EtwDiagnosticTrace trace, string param0, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 17))
			{
				TraceCore.WriteEtwEvent(trace, 17, null, param0, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool ThrowingEtwExceptionIsEnabled(EtwDiagnosticTrace trace)
		{
			return TraceCore.IsEtwEventEnabled(trace, 18);
		}

		internal static void ThrowingEtwException(EtwDiagnosticTrace trace, string param0, string param1, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 18))
			{
				TraceCore.WriteEtwEvent(trace, 18, null, param0, param1, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool ThrowingEtwExceptionVerboseIsEnabled(EtwDiagnosticTrace trace)
		{
			return TraceCore.IsEtwEventEnabled(trace, 19);
		}

		internal static void ThrowingEtwExceptionVerbose(EtwDiagnosticTrace trace, string param0, string param1, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 19))
			{
				TraceCore.WriteEtwEvent(trace, 19, null, param0, param1, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool ThrowingExceptionVerboseIsEnabled(EtwDiagnosticTrace trace)
		{
			return trace.ShouldTrace(TraceEventLevel.Verbose) || TraceCore.IsEtwEventEnabled(trace, 20);
		}

		internal static void ThrowingExceptionVerbose(EtwDiagnosticTrace trace, string param0, string param1, Exception exception)
		{
			TracePayload serializedPayload = trace.GetSerializedPayload(null, null, exception);
			if (TraceCore.IsEtwEventEnabled(trace, 20))
			{
				TraceCore.WriteEtwEvent(trace, 20, null, param0, param1, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
			}
			if (trace.ShouldTraceToTraceSource(TraceEventLevel.Verbose))
			{
				string text = string.Format(TraceCore.Culture, TraceCore.ResourceManager.GetString("ThrowingExceptionVerbose", TraceCore.Culture), param0, param1);
				TraceCore.WriteTraceSource(trace, 20, text, serializedPayload);
			}
		}

		[SecuritySafeCritical]
		private static void CreateEventDescriptors()
		{
			TraceCore.eventDescriptors = new EventDescriptor[]
			{
				new EventDescriptor(57393, 0, 19, 4, 0, 0, 1152921504606912512L),
				new EventDescriptor(57394, 0, 18, 4, 0, 0, 2305843009213759488L),
				new EventDescriptor(57395, 0, 18, 2, 0, 0, 2305843009213759488L),
				new EventDescriptor(57396, 0, 18, 3, 0, 0, 2305843009213759488L),
				new EventDescriptor(57397, 0, 17, 1, 0, 0, 4611686018427453440L),
				new EventDescriptor(57399, 0, 19, 1, 0, 0, 1152921504606912512L),
				new EventDescriptor(57400, 0, 19, 2, 0, 0, 1152921504606912512L),
				new EventDescriptor(57401, 0, 19, 4, 0, 0, 1152921504606912512L),
				new EventDescriptor(57402, 0, 19, 5, 0, 0, 1152921504606912512L),
				new EventDescriptor(57403, 0, 19, 3, 0, 0, 1152921504606912512L),
				new EventDescriptor(57404, 0, 18, 3, 0, 0, 2305843009213759488L),
				new EventDescriptor(131, 0, 19, 5, 12, 2509, 1152921504606912512L),
				new EventDescriptor(132, 0, 19, 5, 13, 2509, 1152921504606912512L),
				new EventDescriptor(133, 0, 19, 5, 1, 2593, 1152921504608944128L),
				new EventDescriptor(134, 0, 19, 5, 2, 2593, 1152921504608944128L),
				new EventDescriptor(57405, 0, 17, 2, 0, 0, 4611686018427453440L),
				new EventDescriptor(57406, 0, 18, 5, 0, 0, 2305843009213759488L),
				new EventDescriptor(57408, 0, 17, 1, 0, 0, 4611686018427453440L),
				new EventDescriptor(57410, 0, 18, 3, 0, 0, 2305843009213759488L),
				new EventDescriptor(57409, 0, 18, 5, 0, 0, 2305843009213759488L),
				new EventDescriptor(57407, 0, 18, 5, 0, 0, 2305843009213759488L)
			};
		}

		private static void EnsureEventDescriptors()
		{
			if (TraceCore.eventDescriptorsCreated)
			{
				return;
			}
			lock (TraceCore.syncLock)
			{
				if (!TraceCore.eventDescriptorsCreated)
				{
					TraceCore.CreateEventDescriptors();
					TraceCore.eventDescriptorsCreated = true;
				}
			}
		}

		[SecuritySafeCritical]
		private static bool IsEtwEventEnabled(EtwDiagnosticTrace trace, int eventIndex)
		{
			if (trace.IsEtwProviderEnabled)
			{
				TraceCore.EnsureEventDescriptors();
				return trace.IsEtwEventEnabled(ref TraceCore.eventDescriptors[eventIndex], false);
			}
			return false;
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(EtwDiagnosticTrace trace, int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2, string eventParam3, string eventParam4)
		{
			TraceCore.EnsureEventDescriptors();
			return trace.EtwProvider.WriteEvent(ref TraceCore.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3, eventParam4);
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(EtwDiagnosticTrace trace, int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2, string eventParam3)
		{
			TraceCore.EnsureEventDescriptors();
			return trace.EtwProvider.WriteEvent(ref TraceCore.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3);
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(EtwDiagnosticTrace trace, int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2)
		{
			TraceCore.EnsureEventDescriptors();
			return trace.EtwProvider.WriteEvent(ref TraceCore.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2);
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(EtwDiagnosticTrace trace, int eventIndex, EventTraceActivity eventParam0, int eventParam1, string eventParam2)
		{
			TraceCore.EnsureEventDescriptors();
			return trace.EtwProvider.WriteEvent(ref TraceCore.eventDescriptors[eventIndex], eventParam0, new object[] { eventParam1, eventParam2 });
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(EtwDiagnosticTrace trace, int eventIndex, EventTraceActivity eventParam0, int eventParam1, int eventParam2, string eventParam3)
		{
			TraceCore.EnsureEventDescriptors();
			return trace.EtwProvider.WriteEvent(ref TraceCore.eventDescriptors[eventIndex], eventParam0, new object[] { eventParam1, eventParam2, eventParam3 });
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(EtwDiagnosticTrace trace, int eventIndex, EventTraceActivity eventParam0, string eventParam1)
		{
			TraceCore.EnsureEventDescriptors();
			return trace.EtwProvider.WriteEvent(ref TraceCore.eventDescriptors[eventIndex], eventParam0, eventParam1);
		}

		[SecuritySafeCritical]
		private static void WriteTraceSource(EtwDiagnosticTrace trace, int eventIndex, string description, TracePayload payload)
		{
			TraceCore.EnsureEventDescriptors();
			trace.WriteTraceSource(ref TraceCore.eventDescriptors[eventIndex], description, payload);
		}

		private static ResourceManager resourceManager;

		private static CultureInfo resourceCulture;

		[SecurityCritical]
		private static EventDescriptor[] eventDescriptors;

		private static object syncLock = new object();

		private static volatile bool eventDescriptorsCreated;
	}
}
