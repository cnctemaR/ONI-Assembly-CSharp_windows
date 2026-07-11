using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.ServiceModel.Internals;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace System.Runtime.Diagnostics
{
	internal sealed class EtwDiagnosticTrace : DiagnosticTraceBase
	{
		[SecurityCritical]
		static EtwDiagnosticTrace()
		{
			if (!PartialTrustHelpers.HasEtwPermissions())
			{
				EtwDiagnosticTrace.defaultEtwProviderId = Guid.Empty;
			}
		}

		[SecurityCritical]
		public EtwDiagnosticTrace(string traceSourceName, Guid etwProviderId)
			: base(traceSourceName)
		{
			try
			{
				this.TraceSourceName = traceSourceName;
				base.EventSourceName = this.TraceSourceName + " " + "4.0.0.0";
				this.CreateTraceSource();
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				new EventLogger(base.EventSourceName, null).LogEvent(TraceEventType.Error, 4, 3221291108U, false, new string[] { ex.ToString() });
			}
			try
			{
				this.CreateEtwProvider(etwProviderId);
			}
			catch (Exception ex2)
			{
				if (Fx.IsFatal(ex2))
				{
					throw;
				}
				this.etwProvider = null;
				new EventLogger(base.EventSourceName, null).LogEvent(TraceEventType.Error, 4, 3221291108U, false, new string[] { ex2.ToString() });
			}
			if (base.TracingEnabled || this.EtwTracingEnabled)
			{
				base.AddDomainEventHandlersForCleanup();
			}
		}

		public static Guid DefaultEtwProviderId
		{
			[SecuritySafeCritical]
			get
			{
				return EtwDiagnosticTrace.defaultEtwProviderId;
			}
			[SecurityCritical]
			set
			{
				EtwDiagnosticTrace.defaultEtwProviderId = value;
			}
		}

		public EtwProvider EtwProvider
		{
			[SecurityCritical]
			get
			{
				return this.etwProvider;
			}
		}

		public bool IsEtwProviderEnabled
		{
			[SecuritySafeCritical]
			get
			{
				return this.EtwTracingEnabled && this.etwProvider.IsEnabled();
			}
		}

		public Action RefreshState
		{
			[SecuritySafeCritical]
			get
			{
				return this.EtwProvider.ControllerCallBack;
			}
			[SecuritySafeCritical]
			set
			{
				this.EtwProvider.ControllerCallBack = value;
			}
		}

		public bool IsEnd2EndActivityTracingEnabled
		{
			[SecuritySafeCritical]
			get
			{
				return this.IsEtwProviderEnabled && this.EtwProvider.IsEnd2EndActivityTracingEnabled;
			}
		}

		private bool EtwTracingEnabled
		{
			[SecuritySafeCritical]
			get
			{
				return this.etwProvider != null;
			}
		}

		[SecuritySafeCritical]
		public void SetEnd2EndActivityTracingEnabled(bool isEnd2EndTracingEnabled)
		{
			this.EtwProvider.SetEnd2EndActivityTracingEnabled(isEnd2EndTracingEnabled);
		}

		public void SetAnnotation(Func<string> annotation)
		{
			EtwDiagnosticTrace.traceAnnotation = annotation;
		}

		public override bool ShouldTrace(TraceEventLevel level)
		{
			return base.ShouldTrace(level) || this.ShouldTraceToEtw(level);
		}

		[SecuritySafeCritical]
		public bool ShouldTraceToEtw(TraceEventLevel level)
		{
			return this.EtwProvider != null && this.EtwProvider.IsEnabled((byte)level, 0L);
		}

		[SecuritySafeCritical]
		public void Event(int eventId, TraceEventLevel traceEventLevel, TraceChannel channel, string description)
		{
			if (base.TracingEnabled)
			{
				EventDescriptor eventDescriptor = EtwDiagnosticTrace.GetEventDescriptor(eventId, channel, traceEventLevel);
				this.Event(ref eventDescriptor, description);
			}
		}

		[SecurityCritical]
		public void Event(ref EventDescriptor eventDescriptor, string description)
		{
			if (base.TracingEnabled)
			{
				TracePayload serializedPayload = this.GetSerializedPayload(null, null, null);
				this.WriteTraceSource(ref eventDescriptor, description, serializedPayload);
			}
		}

		public void SetAndTraceTransfer(Guid newId, bool emitTransfer)
		{
			if (emitTransfer)
			{
				this.TraceTransfer(newId);
			}
			DiagnosticTraceBase.ActivityId = newId;
		}

		[SecuritySafeCritical]
		public void TraceTransfer(Guid newId)
		{
			Guid activityId = DiagnosticTraceBase.ActivityId;
			if (newId != activityId)
			{
				try
				{
					bool haveListeners = base.HaveListeners;
					if (this.IsEtwEventEnabled(ref EtwDiagnosticTrace.transferEventDescriptor, false))
					{
						this.etwProvider.WriteTransferEvent(ref EtwDiagnosticTrace.transferEventDescriptor, new EventTraceActivity(activityId, false), newId, (EtwDiagnosticTrace.traceAnnotation == null) ? string.Empty : EtwDiagnosticTrace.traceAnnotation(), DiagnosticTraceBase.AppDomainFriendlyName);
					}
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					base.LogTraceFailure(null, ex);
				}
			}
		}

		[SecurityCritical]
		public void WriteTraceSource(ref EventDescriptor eventDescriptor, string description, TracePayload payload)
		{
			if (base.TracingEnabled)
			{
				XPathNavigator xpathNavigator = null;
				try
				{
					string text;
					int num;
					EtwDiagnosticTrace.GenerateLegacyTraceCode(ref eventDescriptor, out text, out num);
					string text2 = EtwDiagnosticTrace.BuildTrace(ref eventDescriptor, description, payload, text);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(text2);
					xpathNavigator = xmlDocument.CreateNavigator();
					if (base.CalledShutdown)
					{
						base.TraceSource.Flush();
					}
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					base.LogTraceFailure((xpathNavigator == null) ? string.Empty : xpathNavigator.ToString(), ex);
				}
			}
		}

		[SecurityCritical]
		private static string BuildTrace(ref EventDescriptor eventDescriptor, string description, TracePayload payload, string msdnTraceCode)
		{
			StringBuilder stringBuilder = EtwDiagnosticTrace.StringBuilderPool.Take();
			string text;
			try
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.CurrentCulture))
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						xmlTextWriter.WriteStartElement("TraceRecord");
						xmlTextWriter.WriteAttributeString("xmlns", "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord");
						xmlTextWriter.WriteAttributeString("Severity", TraceLevelHelper.LookupSeverity((TraceEventLevel)eventDescriptor.Level, (TraceEventOpcode)eventDescriptor.Opcode));
						xmlTextWriter.WriteAttributeString("Channel", EtwDiagnosticTrace.LookupChannel((TraceChannel)eventDescriptor.Channel));
						xmlTextWriter.WriteElementString("TraceIdentifier", msdnTraceCode);
						xmlTextWriter.WriteElementString("Description", description);
						xmlTextWriter.WriteElementString("AppDomain", payload.AppDomainFriendlyName);
						if (!string.IsNullOrEmpty(payload.EventSource))
						{
							xmlTextWriter.WriteElementString("Source", payload.EventSource);
						}
						if (!string.IsNullOrEmpty(payload.ExtendedData))
						{
							xmlTextWriter.WriteRaw(payload.ExtendedData);
						}
						if (!string.IsNullOrEmpty(payload.SerializedException))
						{
							xmlTextWriter.WriteRaw(payload.SerializedException);
						}
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.Flush();
						stringWriter.Flush();
						text = stringBuilder.ToString();
					}
				}
			}
			finally
			{
				EtwDiagnosticTrace.StringBuilderPool.Return(stringBuilder);
			}
			return text;
		}

		[SecurityCritical]
		private static void GenerateLegacyTraceCode(ref EventDescriptor eventDescriptor, out string msdnTraceCode, out int legacyEventId)
		{
			switch (eventDescriptor.EventId)
			{
			case 57393:
				msdnTraceCode = EtwDiagnosticTrace.GenerateMsdnTraceCode("System.ServiceModel.Diagnostics", "AppDomainUnload");
				legacyEventId = 131073;
				return;
			case 57394:
			case 57404:
			case 57405:
			case 57406:
				msdnTraceCode = EtwDiagnosticTrace.GenerateMsdnTraceCode("System.ServiceModel.Diagnostics", "TraceHandledException");
				legacyEventId = 131076;
				return;
			case 57396:
			case 57407:
				msdnTraceCode = EtwDiagnosticTrace.GenerateMsdnTraceCode("System.ServiceModel.Diagnostics", "ThrowingException");
				legacyEventId = 131075;
				return;
			case 57397:
				msdnTraceCode = EtwDiagnosticTrace.GenerateMsdnTraceCode("System.ServiceModel.Diagnostics", "UnhandledException");
				legacyEventId = 131077;
				return;
			}
			msdnTraceCode = eventDescriptor.EventId.ToString(CultureInfo.InvariantCulture);
			legacyEventId = eventDescriptor.EventId;
		}

		private static string GenerateMsdnTraceCode(string traceSource, string traceCodeString)
		{
			return string.Format(CultureInfo.InvariantCulture, "http://msdn.microsoft.com/{0}/library/{1}.{2}.aspx", CultureInfo.CurrentCulture.Name, traceSource, traceCodeString);
		}

		private static string LookupChannel(TraceChannel traceChannel)
		{
			string text;
			if (traceChannel != TraceChannel.Application)
			{
				switch (traceChannel)
				{
				case TraceChannel.Admin:
					text = "Admin";
					break;
				case TraceChannel.Operational:
					text = "Operational";
					break;
				case TraceChannel.Analytic:
					text = "Analytic";
					break;
				case TraceChannel.Debug:
					text = "Debug";
					break;
				case TraceChannel.Perf:
					text = "Perf";
					break;
				default:
					text = traceChannel.ToString();
					break;
				}
			}
			else
			{
				text = "Application";
			}
			return text;
		}

		public TracePayload GetSerializedPayload(object source, TraceRecord traceRecord, Exception exception)
		{
			return this.GetSerializedPayload(source, traceRecord, exception, false);
		}

		public TracePayload GetSerializedPayload(object source, TraceRecord traceRecord, Exception exception, bool getServiceReference)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			if (source != null)
			{
				text = DiagnosticTraceBase.CreateSourceString(source);
			}
			if (traceRecord != null)
			{
				StringBuilder stringBuilder = EtwDiagnosticTrace.StringBuilderPool.Take();
				try
				{
					using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.CurrentCulture))
					{
						using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
						{
							xmlTextWriter.WriteStartElement("ExtendedData");
							traceRecord.WriteTo(xmlTextWriter);
							xmlTextWriter.WriteEndElement();
							xmlTextWriter.Flush();
							stringWriter.Flush();
							text2 = stringBuilder.ToString();
						}
					}
				}
				finally
				{
					EtwDiagnosticTrace.StringBuilderPool.Return(stringBuilder);
				}
			}
			if (exception != null)
			{
				text3 = EtwDiagnosticTrace.ExceptionToTraceString(exception, 28672);
			}
			if (getServiceReference && EtwDiagnosticTrace.traceAnnotation != null)
			{
				return new TracePayload(text3, text, DiagnosticTraceBase.AppDomainFriendlyName, text2, EtwDiagnosticTrace.traceAnnotation());
			}
			return new TracePayload(text3, text, DiagnosticTraceBase.AppDomainFriendlyName, text2, string.Empty);
		}

		[SecuritySafeCritical]
		public bool IsEtwEventEnabled(ref EventDescriptor eventDescriptor)
		{
			return this.IsEtwEventEnabled(ref eventDescriptor, true);
		}

		[SecuritySafeCritical]
		public bool IsEtwEventEnabled(ref EventDescriptor eventDescriptor, bool fullCheck)
		{
			if (fullCheck)
			{
				return this.EtwTracingEnabled && this.etwProvider.IsEventEnabled(ref eventDescriptor);
			}
			return this.EtwTracingEnabled && this.etwProvider.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords);
		}

		[SecuritySafeCritical]
		private void CreateTraceSource()
		{
			if (!string.IsNullOrEmpty(this.TraceSourceName))
			{
				base.SetTraceSource(new DiagnosticTraceSource(this.TraceSourceName));
			}
		}

		[SecurityCritical]
		private void CreateEtwProvider(Guid etwProviderId)
		{
			if (etwProviderId != Guid.Empty && EtwDiagnosticTrace.isVistaOrGreater)
			{
				this.etwProvider = (EtwProvider)EtwDiagnosticTrace.etwProviderCache[etwProviderId];
				if (this.etwProvider == null)
				{
					Hashtable hashtable = EtwDiagnosticTrace.etwProviderCache;
					lock (hashtable)
					{
						this.etwProvider = (EtwProvider)EtwDiagnosticTrace.etwProviderCache[etwProviderId];
						if (this.etwProvider == null)
						{
							this.etwProvider = new EtwProvider(etwProviderId);
							EtwDiagnosticTrace.etwProviderCache.Add(etwProviderId, this.etwProvider);
						}
					}
				}
				this.etwProviderId = etwProviderId;
			}
		}

		[SecurityCritical]
		private static EventDescriptor GetEventDescriptor(int eventId, TraceChannel channel, TraceEventLevel traceEventLevel)
		{
			long num = 0L;
			if (channel == TraceChannel.Admin)
			{
				num |= long.MinValue;
			}
			else if (channel == TraceChannel.Operational)
			{
				num |= 4611686018427387904L;
			}
			else if (channel == TraceChannel.Analytic)
			{
				num |= 2305843009213693952L;
			}
			else if (channel == TraceChannel.Debug)
			{
				num |= 72057594037927936L;
			}
			else if (channel == TraceChannel.Perf)
			{
				num |= 576460752303423488L;
			}
			return new EventDescriptor(eventId, 0, (byte)channel, (byte)traceEventLevel, 0, 0, num);
		}

		protected override void OnShutdownTracing()
		{
			this.ShutdownTraceSource();
			this.ShutdownEtwProvider();
		}

		private void ShutdownTraceSource()
		{
			try
			{
				if (TraceCore.AppDomainUnloadIsEnabled(this))
				{
					TraceCore.AppDomainUnload(this, AppDomain.CurrentDomain.FriendlyName, DiagnosticTraceBase.ProcessName, DiagnosticTraceBase.ProcessId.ToString(CultureInfo.CurrentCulture));
				}
				base.TraceSource.Flush();
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				base.LogTraceFailure(null, ex);
			}
		}

		[SecuritySafeCritical]
		private void ShutdownEtwProvider()
		{
			try
			{
				if (this.etwProvider != null)
				{
					this.etwProvider.Dispose();
				}
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				base.LogTraceFailure(null, ex);
			}
		}

		public override bool IsEnabled()
		{
			return TraceCore.TraceCodeEventLogCriticalIsEnabled(this) || TraceCore.TraceCodeEventLogVerboseIsEnabled(this) || TraceCore.TraceCodeEventLogInfoIsEnabled(this) || TraceCore.TraceCodeEventLogWarningIsEnabled(this) || TraceCore.TraceCodeEventLogErrorIsEnabled(this);
		}

		public override void TraceEventLogEvent(TraceEventType type, TraceRecord traceRecord)
		{
			switch (type)
			{
			case TraceEventType.Critical:
				if (TraceCore.TraceCodeEventLogCriticalIsEnabled(this))
				{
					TraceCore.TraceCodeEventLogCritical(this, traceRecord);
					return;
				}
				break;
			case TraceEventType.Error:
				if (TraceCore.TraceCodeEventLogErrorIsEnabled(this))
				{
					TraceCore.TraceCodeEventLogError(this, traceRecord);
				}
				break;
			case (TraceEventType)3:
				break;
			case TraceEventType.Warning:
				if (TraceCore.TraceCodeEventLogWarningIsEnabled(this))
				{
					TraceCore.TraceCodeEventLogWarning(this, traceRecord);
					return;
				}
				break;
			default:
				if (type != TraceEventType.Information)
				{
					if (type != TraceEventType.Verbose)
					{
						return;
					}
					if (TraceCore.TraceCodeEventLogVerboseIsEnabled(this))
					{
						TraceCore.TraceCodeEventLogVerbose(this, traceRecord);
						return;
					}
				}
				else if (TraceCore.TraceCodeEventLogInfoIsEnabled(this))
				{
					TraceCore.TraceCodeEventLogInfo(this, traceRecord);
					return;
				}
				break;
			}
		}

		protected override void OnUnhandledException(Exception exception)
		{
			if (TraceCore.UnhandledExceptionIsEnabled(this))
			{
				TraceCore.UnhandledException(this, (exception != null) ? exception.ToString() : string.Empty, exception);
			}
		}

		internal static string ExceptionToTraceString(Exception exception, int maxTraceStringLength)
		{
			StringBuilder stringBuilder = EtwDiagnosticTrace.StringBuilderPool.Take();
			string text;
			try
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.CurrentCulture))
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						EtwDiagnosticTrace.WriteExceptionToTraceString(xmlTextWriter, exception, maxTraceStringLength, 64);
						xmlTextWriter.Flush();
						stringWriter.Flush();
						text = stringBuilder.ToString();
					}
				}
			}
			finally
			{
				EtwDiagnosticTrace.StringBuilderPool.Return(stringBuilder);
			}
			return text;
		}

		private static void WriteExceptionToTraceString(XmlTextWriter xml, Exception exception, int remainingLength, int remainingAllowedRecursionDepth)
		{
			if (remainingAllowedRecursionDepth < 1)
			{
				return;
			}
			if (!EtwDiagnosticTrace.WriteStartElement(xml, "Exception", ref remainingLength))
			{
				return;
			}
			try
			{
				IList<Tuple<string, string>> list = new List<Tuple<string, string>>
				{
					new Tuple<string, string>("ExceptionType", DiagnosticTraceBase.XmlEncode(exception.GetType().AssemblyQualifiedName)),
					new Tuple<string, string>("Message", DiagnosticTraceBase.XmlEncode(exception.Message)),
					new Tuple<string, string>("StackTrace", DiagnosticTraceBase.XmlEncode(DiagnosticTraceBase.StackTraceString(exception))),
					new Tuple<string, string>("ExceptionString", DiagnosticTraceBase.XmlEncode(exception.ToString()))
				};
				Win32Exception ex = exception as Win32Exception;
				if (ex != null)
				{
					list.Add(new Tuple<string, string>("NativeErrorCode", ex.NativeErrorCode.ToString("X", CultureInfo.InvariantCulture)));
				}
				foreach (Tuple<string, string> tuple in list)
				{
					if (!EtwDiagnosticTrace.WriteXmlElementString(xml, tuple.Item1, tuple.Item2, ref remainingLength))
					{
						return;
					}
				}
				if (exception.Data != null && exception.Data.Count > 0)
				{
					string exceptionData = EtwDiagnosticTrace.GetExceptionData(exception);
					if (exceptionData.Length < remainingLength)
					{
						xml.WriteRaw(exceptionData);
						remainingLength -= exceptionData.Length;
					}
				}
				if (exception.InnerException != null)
				{
					string innerException = EtwDiagnosticTrace.GetInnerException(exception, remainingLength, remainingAllowedRecursionDepth - 1);
					if (!string.IsNullOrEmpty(innerException) && innerException.Length < remainingLength)
					{
						xml.WriteRaw(innerException);
					}
				}
			}
			finally
			{
				xml.WriteEndElement();
			}
		}

		private static string GetInnerException(Exception exception, int remainingLength, int remainingAllowedRecursionDepth)
		{
			if (remainingAllowedRecursionDepth < 1)
			{
				return null;
			}
			StringBuilder stringBuilder = EtwDiagnosticTrace.StringBuilderPool.Take();
			string text;
			try
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.CurrentCulture))
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						if (!EtwDiagnosticTrace.WriteStartElement(xmlTextWriter, "InnerException", ref remainingLength))
						{
							text = null;
						}
						else
						{
							EtwDiagnosticTrace.WriteExceptionToTraceString(xmlTextWriter, exception.InnerException, remainingLength, remainingAllowedRecursionDepth);
							xmlTextWriter.WriteEndElement();
							xmlTextWriter.Flush();
							stringWriter.Flush();
							text = stringBuilder.ToString();
						}
					}
				}
			}
			finally
			{
				EtwDiagnosticTrace.StringBuilderPool.Return(stringBuilder);
			}
			return text;
		}

		private static string GetExceptionData(Exception exception)
		{
			StringBuilder stringBuilder = EtwDiagnosticTrace.StringBuilderPool.Take();
			string text;
			try
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.CurrentCulture))
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						xmlTextWriter.WriteStartElement("DataItems");
						foreach (object obj in exception.Data.Keys)
						{
							xmlTextWriter.WriteStartElement("Data");
							xmlTextWriter.WriteElementString("Key", DiagnosticTraceBase.XmlEncode(obj.ToString()));
							if (exception.Data[obj] == null)
							{
								xmlTextWriter.WriteElementString("Value", string.Empty);
							}
							else
							{
								xmlTextWriter.WriteElementString("Value", DiagnosticTraceBase.XmlEncode(exception.Data[obj].ToString()));
							}
							xmlTextWriter.WriteEndElement();
						}
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.Flush();
						stringWriter.Flush();
						text = stringBuilder.ToString();
					}
				}
			}
			finally
			{
				EtwDiagnosticTrace.StringBuilderPool.Return(stringBuilder);
			}
			return text;
		}

		private static bool WriteStartElement(XmlTextWriter xml, string localName, ref int remainingLength)
		{
			int num = localName.Length * 2 + 5;
			if (num <= remainingLength)
			{
				xml.WriteStartElement(localName);
				remainingLength -= num;
				return true;
			}
			return false;
		}

		private static bool WriteXmlElementString(XmlTextWriter xml, string localName, string value, ref int remainingLength)
		{
			int num;
			if (string.IsNullOrEmpty(value) && !LocalAppContextSwitches.IncludeNullExceptionMessageInETWTrace)
			{
				num = localName.Length + 4;
			}
			else
			{
				num = localName.Length * 2 + 5 + value.Length;
			}
			if (num <= remainingLength)
			{
				xml.WriteElementString(localName, value);
				remainingLength -= num;
				return true;
			}
			return false;
		}

		private const int WindowsVistaMajorNumber = 6;

		private const string EventSourceVersion = "4.0.0.0";

		private const ushort TracingEventLogCategory = 4;

		private const int MaxExceptionStringLength = 28672;

		private const int MaxExceptionDepth = 64;

		private const string DiagnosticTraceSource = "System.ServiceModel.Diagnostics";

		private const int XmlBracketsLength = 5;

		private const int XmlBracketsLengthForNullValue = 4;

		public static readonly Guid ImmutableDefaultEtwProviderId = new Guid("{c651f5f6-1c0d-492e-8ae1-b4efd7c9d503}");

		[SecurityCritical]
		private static Guid defaultEtwProviderId = EtwDiagnosticTrace.ImmutableDefaultEtwProviderId;

		private static Hashtable etwProviderCache = new Hashtable();

		private static bool isVistaOrGreater = Environment.OSVersion.Version.Major >= 6;

		private static Func<string> traceAnnotation;

		[SecurityCritical]
		private EtwProvider etwProvider;

		private Guid etwProviderId;

		[SecurityCritical]
		private static EventDescriptor transferEventDescriptor = new EventDescriptor(499, 0, 18, 0, 0, 0, 2305843009215397989L);

		private static class TraceCodes
		{
			public const string AppDomainUnload = "AppDomainUnload";

			public const string TraceHandledException = "TraceHandledException";

			public const string ThrowingException = "ThrowingException";

			public const string UnhandledException = "UnhandledException";
		}

		private static class EventIdsWithMsdnTraceCode
		{
			public const int AppDomainUnload = 57393;

			public const int ThrowingExceptionWarning = 57396;

			public const int ThrowingExceptionVerbose = 57407;

			public const int HandledExceptionInfo = 57394;

			public const int HandledExceptionWarning = 57404;

			public const int HandledExceptionError = 57405;

			public const int HandledExceptionVerbose = 57406;

			public const int UnhandledException = 57397;
		}

		private static class LegacyTraceEventIds
		{
			public const int Diagnostics = 131072;

			public const int AppDomainUnload = 131073;

			public const int EventLog = 131074;

			public const int ThrowingException = 131075;

			public const int TraceHandledException = 131076;

			public const int UnhandledException = 131077;
		}

		private static class StringBuilderPool
		{
			public static StringBuilder Take()
			{
				StringBuilder stringBuilder = null;
				if (EtwDiagnosticTrace.StringBuilderPool.freeStringBuilders.TryDequeue(out stringBuilder))
				{
					return stringBuilder;
				}
				return new StringBuilder();
			}

			public static void Return(StringBuilder sb)
			{
				if (EtwDiagnosticTrace.StringBuilderPool.freeStringBuilders.Count <= 64)
				{
					sb.Clear();
					EtwDiagnosticTrace.StringBuilderPool.freeStringBuilders.Enqueue(sb);
				}
			}

			private const int maxPooledStringBuilders = 64;

			private static readonly ConcurrentQueue<StringBuilder> freeStringBuilders = new ConcurrentQueue<StringBuilder>();
		}
	}
}
