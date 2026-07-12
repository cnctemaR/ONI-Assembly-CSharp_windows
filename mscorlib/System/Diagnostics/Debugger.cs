using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	public sealed class Debugger
	{
		public static bool IsAttached
		{
			get
			{
				return Debugger.IsAttached_internal();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAttached_internal();

		public static void Break()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsLogging();

		public static bool Launch()
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Log_icall(int level, ref string category, ref string message);

		public static void Log(int level, string category, string message)
		{
			Debugger.Log_icall(level, ref category, ref message);
		}

		public static void NotifyOfCrossThreadDependency()
		{
		}

		[Obsolete("Call the static methods directly on this type", true)]
		public Debugger()
		{
		}

		public static readonly string DefaultCategory = "";
	}
}
