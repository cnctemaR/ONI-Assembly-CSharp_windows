using System;
using System.Runtime.CompilerServices;

namespace System.Threading
{
	internal static class MonitorEx
	{
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Enter(object obj, ref bool lockTaken)
		{
			Monitor.Enter(obj, ref lockTaken);
		}
	}
}
