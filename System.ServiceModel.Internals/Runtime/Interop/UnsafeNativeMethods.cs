using System;
using System.ComponentModel;
using System.Runtime.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.Runtime.Interop
{
	[SuppressUnmanagedCodeSecurity]
	internal static class UnsafeNativeMethods
	{
		[SecurityCritical]
		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto)]
		public static extern SafeWaitHandle CreateWaitableTimer(IntPtr mustBeZero, bool manualReset, string timerName);

		[SecurityCritical]
		[DllImport("kernel32.dll", ExactSpelling = true)]
		public static extern bool SetWaitableTimer(SafeWaitHandle handle, ref long dueTime, int period, IntPtr mustBeZero, IntPtr mustBeZeroAlso, bool resume);

		[SecurityCritical]
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int QueryPerformanceCounter(out long time);

		[SecurityCritical]
		[DllImport("kernel32.dll")]
		public static extern uint GetSystemTimeAdjustment(out int adjustment, out uint increment, out uint adjustmentDisabled);

		[SecurityCritical]
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern void GetSystemTimeAsFileTime(out global::System.Runtime.InteropServices.ComTypes.FILETIME time);

		[SecurityCritical]
		public static void GetSystemTimeAsFileTime(out long time)
		{
			global::System.Runtime.InteropServices.ComTypes.FILETIME filetime;
			UnsafeNativeMethods.GetSystemTimeAsFileTime(out filetime);
			time = 0L;
			time |= (long)((ulong)filetime.dwHighDateTime);
			time <<= 32;
			time |= (long)((ulong)filetime.dwLowDateTime);
		}

		[SecurityCritical]
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetComputerNameEx([In] ComputerNameFormat nameType, [MarshalAs(UnmanagedType.LPTStr)] [In] [Out] StringBuilder lpBuffer, [In] [Out] ref int size);

		[SecurityCritical]
		internal static string GetComputerName(ComputerNameFormat nameType)
		{
			int num = 0;
			if (!UnsafeNativeMethods.GetComputerNameEx(nameType, null, ref num))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 234)
				{
					throw Fx.Exception.AsError(new Win32Exception(lastWin32Error));
				}
			}
			if (num < 0)
			{
				Fx.AssertAndThrow("GetComputerName returned an invalid length: " + num.ToString());
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			if (!UnsafeNativeMethods.GetComputerNameEx(nameType, stringBuilder, ref num))
			{
				int lastWin32Error2 = Marshal.GetLastWin32Error();
				throw Fx.Exception.AsError(new Win32Exception(lastWin32Error2));
			}
			return stringBuilder.ToString();
		}

		[SecurityCritical]
		[DllImport("kernel32.dll")]
		internal static extern bool IsDebuggerPresent();

		[SecurityCritical]
		[DllImport("kernel32.dll")]
		internal static extern void DebugBreak();

		[SecurityCritical]
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		internal static extern void OutputDebugString(string lpOutputString);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal unsafe static extern uint EventRegister([In] ref Guid providerId, [In] UnsafeNativeMethods.EtwEnableCallback enableCallback, [In] void* callbackContext, [In] [Out] ref long registrationHandle);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern uint EventUnregister([In] long registrationHandle);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern bool EventEnabled([In] long registrationHandle, [In] ref global::System.Runtime.Diagnostics.EventDescriptor eventDescriptor);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal unsafe static extern uint EventWrite([In] long registrationHandle, [In] ref global::System.Runtime.Diagnostics.EventDescriptor eventDescriptor, [In] uint userDataCount, [In] UnsafeNativeMethods.EventData* userData);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal unsafe static extern uint EventWriteTransfer([In] long registrationHandle, [In] ref global::System.Runtime.Diagnostics.EventDescriptor eventDescriptor, [In] ref Guid activityId, [In] ref Guid relatedActivityId, [In] uint userDataCount, [In] UnsafeNativeMethods.EventData* userData);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal unsafe static extern uint EventWriteString([In] long registrationHandle, [In] byte level, [In] long keywords, [In] char* message);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern uint EventActivityIdControl([In] int ControlCode, [In] [Out] ref Guid ActivityId);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool ReportEvent(SafeHandle hEventLog, ushort type, ushort category, uint eventID, byte[] userSID, ushort numStrings, uint dataLen, HandleRef strings, byte[] rawData);

		[SecurityCritical]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern SafeEventLogWriteHandle RegisterEventSource(string uncServerName, string sourceName);

		public const string KERNEL32 = "kernel32.dll";

		public const string ADVAPI32 = "advapi32.dll";

		public const int ERROR_INVALID_HANDLE = 6;

		public const int ERROR_MORE_DATA = 234;

		public const int ERROR_ARITHMETIC_OVERFLOW = 534;

		public const int ERROR_NOT_ENOUGH_MEMORY = 8;

		[StructLayout(LayoutKind.Explicit, Size = 16)]
		public struct EventData
		{
			[FieldOffset(0)]
			internal ulong DataPointer;

			[FieldOffset(8)]
			internal uint Size;

			[FieldOffset(12)]
			internal int Reserved;
		}

		[SecurityCritical]
		internal unsafe delegate void EtwEnableCallback([In] ref Guid sourceId, [In] int isEnabled, [In] byte level, [In] long matchAnyKeywords, [In] long matchAllKeywords, [In] void* filterData, [In] void* callbackContext);
	}
}
