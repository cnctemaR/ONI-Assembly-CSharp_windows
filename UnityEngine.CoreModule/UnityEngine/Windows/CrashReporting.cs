using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Windows
{
	public static class CrashReporting
	{
		public static string crashReportFolder
		{
			[NativeHeader("PlatformDependent/WinPlayer/Bindings/CrashReportingBindings.h")]
			[ThreadSafe]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					CrashReporting.get_crashReportFolder_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_crashReportFolder_Injected(out ManagedSpanWrapper ret);
	}
}
