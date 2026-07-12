using System;
using System.Globalization;
using System.Resources;
using System.Runtime.Diagnostics;
using System.Security;

namespace System.Runtime.Serialization.Diagnostics.Application
{
	internal class TD
	{
		private TD()
		{
		}

		private static ResourceManager ResourceManager
		{
			get
			{
				if (TD.resourceManager == null)
				{
					TD.resourceManager = new ResourceManager("System.Runtime.Serialization.Diagnostics.Application.TD", typeof(TD).Assembly);
				}
				return TD.resourceManager;
			}
		}

		internal static CultureInfo Culture
		{
			get
			{
				return TD.resourceCulture;
			}
			set
			{
				TD.resourceCulture = value;
			}
		}

		internal static bool ReaderQuotaExceededIsEnabled()
		{
			return FxTrace.ShouldTraceError && TD.IsEtwEventEnabled(0);
		}

		internal static void ReaderQuotaExceeded(string param0)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(0))
			{
				TD.WriteEtwEvent(0, null, param0, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCSerializeWithSurrogateStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(1);
		}

		internal static void DCSerializeWithSurrogateStart(string SurrogateType)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(1))
			{
				TD.WriteEtwEvent(1, null, SurrogateType, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCSerializeWithSurrogateStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(2);
		}

		internal static void DCSerializeWithSurrogateStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(2))
			{
				TD.WriteEtwEvent(2, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCDeserializeWithSurrogateStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(3);
		}

		internal static void DCDeserializeWithSurrogateStart(string SurrogateType)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(3))
			{
				TD.WriteEtwEvent(3, null, SurrogateType, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCDeserializeWithSurrogateStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(4);
		}

		internal static void DCDeserializeWithSurrogateStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(4))
			{
				TD.WriteEtwEvent(4, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool ImportKnownTypesStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(5);
		}

		internal static void ImportKnownTypesStart()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(5))
			{
				TD.WriteEtwEvent(5, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool ImportKnownTypesStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(6);
		}

		internal static void ImportKnownTypesStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(6))
			{
				TD.WriteEtwEvent(6, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCResolverResolveIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(7);
		}

		internal static void DCResolverResolve(string TypeName)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(7))
			{
				TD.WriteEtwEvent(7, null, TypeName, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCGenWriterStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(8);
		}

		internal static void DCGenWriterStart(string Kind, string TypeName)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(8))
			{
				TD.WriteEtwEvent(8, null, Kind, TypeName, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCGenWriterStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(9);
		}

		internal static void DCGenWriterStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(9))
			{
				TD.WriteEtwEvent(9, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCGenReaderStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(10);
		}

		internal static void DCGenReaderStart(string Kind, string TypeName)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(10))
			{
				TD.WriteEtwEvent(10, null, Kind, TypeName, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCGenReaderStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(11);
		}

		internal static void DCGenReaderStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(11))
			{
				TD.WriteEtwEvent(11, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCJsonGenReaderStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(12);
		}

		internal static void DCJsonGenReaderStart(string Kind, string TypeName)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(12))
			{
				TD.WriteEtwEvent(12, null, Kind, TypeName, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCJsonGenReaderStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(13);
		}

		internal static void DCJsonGenReaderStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(13))
			{
				TD.WriteEtwEvent(13, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCJsonGenWriterStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(14);
		}

		internal static void DCJsonGenWriterStart(string Kind, string TypeName)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(14))
			{
				TD.WriteEtwEvent(14, null, Kind, TypeName, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool DCJsonGenWriterStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(15);
		}

		internal static void DCJsonGenWriterStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(15))
			{
				TD.WriteEtwEvent(15, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool GenXmlSerializableStartIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(16);
		}

		internal static void GenXmlSerializableStart(string DCType)
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(16))
			{
				TD.WriteEtwEvent(16, null, DCType, serializedPayload.AppDomainFriendlyName);
			}
		}

		internal static bool GenXmlSerializableStopIsEnabled()
		{
			return FxTrace.ShouldTraceVerbose && TD.IsEtwEventEnabled(17);
		}

		internal static void GenXmlSerializableStop()
		{
			TracePayload serializedPayload = FxTrace.Trace.GetSerializedPayload(null, null, null);
			if (TD.IsEtwEventEnabled(17))
			{
				TD.WriteEtwEvent(17, null, serializedPayload.AppDomainFriendlyName);
			}
		}

		[SecuritySafeCritical]
		private static void CreateEventDescriptors()
		{
			EventDescriptor[] array = new EventDescriptor[]
			{
				new EventDescriptor(1420, 0, 18, 2, 0, 2560, 2305843009217888256L),
				new EventDescriptor(5001, 0, 19, 5, 1, 2592, 1152921504606846978L),
				new EventDescriptor(5002, 0, 19, 5, 2, 2592, 1152921504606846978L),
				new EventDescriptor(5003, 0, 19, 5, 1, 2591, 1152921504606846978L),
				new EventDescriptor(5004, 0, 19, 5, 2, 2591, 1152921504606846978L),
				new EventDescriptor(5005, 0, 19, 5, 1, 2547, 1152921504606846978L),
				new EventDescriptor(5006, 0, 19, 5, 2, 2547, 1152921504606846978L),
				new EventDescriptor(5007, 0, 19, 5, 1, 2528, 1152921504606846978L),
				new EventDescriptor(5008, 0, 19, 5, 1, 2544, 1152921504606846978L),
				new EventDescriptor(5009, 0, 19, 5, 2, 2544, 1152921504606846978L),
				new EventDescriptor(5010, 0, 19, 5, 1, 2543, 1152921504606846978L),
				new EventDescriptor(5011, 0, 19, 5, 2, 2543, 1152921504606846978L),
				new EventDescriptor(5012, 0, 19, 5, 1, 2543, 1152921504606846978L),
				new EventDescriptor(5013, 0, 19, 5, 2, 2543, 1152921504606846978L),
				new EventDescriptor(5014, 0, 19, 5, 1, 2544, 1152921504606846978L),
				new EventDescriptor(5015, 0, 19, 5, 2, 2544, 1152921504606846978L),
				new EventDescriptor(5016, 0, 19, 5, 1, 2545, 1152921504606846978L),
				new EventDescriptor(5017, 0, 19, 5, 2, 2545, 1152921504606846978L)
			};
			ushort[] array2 = new ushort[0];
			FxTrace.UpdateEventDefinitions(array, array2);
			TD.eventDescriptors = array;
		}

		private static void EnsureEventDescriptors()
		{
			if (TD.eventDescriptorsCreated)
			{
				return;
			}
			lock (TD.syncLock)
			{
				if (!TD.eventDescriptorsCreated)
				{
					TD.CreateEventDescriptors();
					TD.eventDescriptorsCreated = true;
				}
			}
		}

		private static bool IsEtwEventEnabled(int eventIndex)
		{
			if (FxTrace.Trace.IsEtwProviderEnabled)
			{
				TD.EnsureEventDescriptors();
				return FxTrace.IsEventEnabled(eventIndex);
			}
			return false;
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2)
		{
			TD.EnsureEventDescriptors();
			return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2);
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1)
		{
			TD.EnsureEventDescriptors();
			return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1);
		}

		[SecuritySafeCritical]
		private static bool WriteEtwEvent(int eventIndex, EventTraceActivity eventParam0, string eventParam1, string eventParam2, string eventParam3)
		{
			TD.EnsureEventDescriptors();
			return FxTrace.Trace.EtwProvider.WriteEvent(ref TD.eventDescriptors[eventIndex], eventParam0, eventParam1, eventParam2, eventParam3);
		}

		private static ResourceManager resourceManager;

		private static CultureInfo resourceCulture;

		[SecurityCritical]
		private static EventDescriptor[] eventDescriptors;

		private static object syncLock = new object();

		private static volatile bool eventDescriptorsCreated;
	}
}
