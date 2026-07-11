using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Interop;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace System.Runtime.Diagnostics
{
	internal sealed class EventLogger
	{
		private EventLogger()
		{
			this.isInPartialTrust = this.IsInPartialTrust();
		}

		[Obsolete("For System.Runtime.dll use only. Call FxTrace.EventLog instead")]
		public EventLogger(string eventLogSourceName, DiagnosticTraceBase diagnosticTrace)
		{
			try
			{
				this.diagnosticTrace = diagnosticTrace;
				if (EventLogger.canLogEvent)
				{
					this.SafeSetLogSourceName(eventLogSourceName);
				}
			}
			catch (SecurityException)
			{
				EventLogger.canLogEvent = false;
			}
		}

		[SecurityCritical]
		public static EventLogger UnsafeCreateEventLogger(string eventLogSourceName, DiagnosticTraceBase diagnosticTrace)
		{
			EventLogger eventLogger = new EventLogger();
			eventLogger.SetLogSourceName(eventLogSourceName, diagnosticTrace);
			return eventLogger;
		}

		[SecurityCritical]
		public void UnsafeLogEvent(TraceEventType type, ushort eventLogCategory, uint eventId, bool shouldTrace, params string[] values)
		{
			if (EventLogger.logCountForPT < 5)
			{
				try
				{
					int num = 0;
					string[] array = new string[values.Length + 2];
					for (int i = 0; i < values.Length; i++)
					{
						string text = values[i];
						if (!string.IsNullOrEmpty(text))
						{
							text = EventLogger.NormalizeEventLogParameter(text);
						}
						else
						{
							text = string.Empty;
						}
						array[i] = text;
						num += text.Length + 1;
					}
					string text2 = EventLogger.NormalizeEventLogParameter(this.UnsafeGetProcessName());
					array[array.Length - 2] = text2;
					num += text2.Length + 1;
					string text3 = this.UnsafeGetProcessId().ToString(CultureInfo.InvariantCulture);
					array[array.Length - 1] = text3;
					num += text3.Length + 1;
					if (num > 25600)
					{
						int num2 = 25600 / array.Length - 1;
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j].Length > num2)
							{
								array[j] = array[j].Substring(0, num2);
							}
						}
					}
					SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
					byte[] array2 = new byte[user.BinaryLength];
					user.GetBinaryForm(array2, 0);
					IntPtr[] array3 = new IntPtr[array.Length];
					GCHandle gchandle = default(GCHandle);
					GCHandle[] array4 = null;
					try
					{
						gchandle = GCHandle.Alloc(array3, GCHandleType.Pinned);
						array4 = new GCHandle[array.Length];
						for (int k = 0; k < array.Length; k++)
						{
							array4[k] = GCHandle.Alloc(array[k], GCHandleType.Pinned);
							array3[k] = array4[k].AddrOfPinnedObject();
						}
						this.UnsafeWriteEventLog(type, eventLogCategory, eventId, array, array2, gchandle);
					}
					finally
					{
						if (gchandle.AddrOfPinnedObject() != IntPtr.Zero)
						{
							gchandle.Free();
						}
						if (array4 != null)
						{
							foreach (GCHandle gchandle2 in array4)
							{
								gchandle2.Free();
							}
						}
					}
					if (shouldTrace && this.diagnosticTrace != null && this.diagnosticTrace.IsEnabled())
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length + 4);
						dictionary["CategoryID.Name"] = "EventLogCategory";
						dictionary["CategoryID.Value"] = eventLogCategory.ToString(CultureInfo.InvariantCulture);
						dictionary["InstanceID.Name"] = "EventId";
						dictionary["InstanceID.Value"] = eventId.ToString(CultureInfo.InvariantCulture);
						for (int m = 0; m < values.Length; m++)
						{
							dictionary.Add("Value" + m.ToString(CultureInfo.InvariantCulture), (values[m] == null) ? string.Empty : DiagnosticTraceBase.XmlEncode(values[m]));
						}
						this.diagnosticTrace.TraceEventLogEvent(type, new DictionaryTraceRecord(dictionary));
					}
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
				}
				if (this.isInPartialTrust)
				{
					EventLogger.logCountForPT++;
				}
			}
		}

		public void LogEvent(TraceEventType type, ushort eventLogCategory, uint eventId, bool shouldTrace, params string[] values)
		{
			if (EventLogger.canLogEvent)
			{
				try
				{
					this.SafeLogEvent(type, eventLogCategory, eventId, shouldTrace, values);
				}
				catch (SecurityException ex)
				{
					EventLogger.canLogEvent = false;
					if (shouldTrace)
					{
						Fx.Exception.TraceHandledException(ex, TraceEventType.Information);
					}
				}
			}
		}

		public void LogEvent(TraceEventType type, ushort eventLogCategory, uint eventId, params string[] values)
		{
			this.LogEvent(type, eventLogCategory, eventId, true, values);
		}

		private static EventLogEntryType EventLogEntryTypeFromEventType(TraceEventType type)
		{
			EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
			if (type - TraceEventType.Critical > 1)
			{
				if (type == TraceEventType.Warning)
				{
					eventLogEntryType = EventLogEntryType.Warning;
				}
			}
			else
			{
				eventLogEntryType = EventLogEntryType.Error;
			}
			return eventLogEntryType;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		private void SafeLogEvent(TraceEventType type, ushort eventLogCategory, uint eventId, bool shouldTrace, params string[] values)
		{
			this.UnsafeLogEvent(type, eventLogCategory, eventId, shouldTrace, values);
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		private void SafeSetLogSourceName(string eventLogSourceName)
		{
			this.eventLogSourceName = eventLogSourceName;
		}

		[SecurityCritical]
		private void SetLogSourceName(string eventLogSourceName, DiagnosticTraceBase diagnosticTrace)
		{
			this.eventLogSourceName = eventLogSourceName;
			this.diagnosticTrace = diagnosticTrace;
		}

		[SecuritySafeCritical]
		private bool IsInPartialTrust()
		{
			bool flag = false;
			try
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					flag = string.IsNullOrEmpty(currentProcess.ProcessName);
				}
			}
			catch (SecurityException)
			{
				flag = true;
			}
			return flag;
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		private void UnsafeWriteEventLog(TraceEventType type, ushort eventLogCategory, uint eventId, string[] logValues, byte[] sidBA, GCHandle stringsRootHandle)
		{
			using (SafeEventLogWriteHandle safeEventLogWriteHandle = SafeEventLogWriteHandle.RegisterEventSource(null, this.eventLogSourceName))
			{
				if (safeEventLogWriteHandle != null)
				{
					HandleRef handleRef = new HandleRef(safeEventLogWriteHandle, stringsRootHandle.AddrOfPinnedObject());
					UnsafeNativeMethods.ReportEvent(safeEventLogWriteHandle, (ushort)EventLogger.EventLogEntryTypeFromEventType(type), eventLogCategory, eventId, sidBA, (ushort)logValues.Length, 0U, handleRef, null);
				}
			}
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private string UnsafeGetProcessName()
		{
			string text = null;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				text = currentProcess.ProcessName;
			}
			return text;
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private int UnsafeGetProcessId()
		{
			int num = -1;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				num = currentProcess.Id;
			}
			return num;
		}

		internal static string NormalizeEventLogParameter(string eventLogParameter)
		{
			if (eventLogParameter.IndexOf('%') < 0)
			{
				return eventLogParameter;
			}
			StringBuilder stringBuilder = null;
			int length = eventLogParameter.Length;
			for (int i = 0; i < length; i++)
			{
				char c = eventLogParameter[i];
				if (c != '%')
				{
					if (stringBuilder != null)
					{
						stringBuilder.Append(c);
					}
				}
				else if (i + 1 >= length)
				{
					if (stringBuilder != null)
					{
						stringBuilder.Append(c);
					}
				}
				else if (eventLogParameter[i + 1] < '0' || eventLogParameter[i + 1] > '9')
				{
					if (stringBuilder != null)
					{
						stringBuilder.Append(c);
					}
				}
				else
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(length + 2);
						for (int j = 0; j < i; j++)
						{
							stringBuilder.Append(eventLogParameter[j]);
						}
					}
					stringBuilder.Append(c);
					stringBuilder.Append(' ');
				}
			}
			if (stringBuilder == null)
			{
				return eventLogParameter;
			}
			return stringBuilder.ToString();
		}

		private const int MaxEventLogsInPT = 5;

		[SecurityCritical]
		private static int logCountForPT;

		private static bool canLogEvent = true;

		private DiagnosticTraceBase diagnosticTrace;

		[SecurityCritical]
		private string eventLogSourceName;

		private bool isInPartialTrust;
	}
}
