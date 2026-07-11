using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.CrashReportHandler
{
	/// <summary>
	///   <para>Engine API for CrashReporting Service.</para>
	/// </summary>
	[NativeHeader("Modules/CrashReporting/CrashReportHandler.h")]
	public class CrashReportHandler
	{
		private CrashReportHandler()
		{
		}

		/// <summary>
		///   <para>This Boolean field will cause CrashReportHandler to capture exceptions when set to true. By default enable capture exceptions is true.</para>
		/// </summary>
		public static extern bool enableCaptureExceptions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
