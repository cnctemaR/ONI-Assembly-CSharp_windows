using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Windows
{
	/// <summary>
	///   <para>Exposes useful information related to crash reporting on Windows platforms.</para>
	/// </summary>
	public static class CrashReporting
	{
		/// <summary>
		///   <para>Returns the path to the crash report folder on Windows.</para>
		/// </summary>
		public static extern string crashReportFolder
		{
			[ThreadSafe]
			[NativeHeader("PlatformDependent/WinPlayer/Bindings/CrashReportingBindings.h")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
