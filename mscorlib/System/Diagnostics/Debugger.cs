using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[MonoTODO("The Debugger class is not functional")]
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

		[MonoTODO("Not implemented")]
		public static bool Launch()
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Log(int level, string category, string message);

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
