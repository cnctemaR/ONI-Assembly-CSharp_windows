using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Scripting
{
	[VisibleToOtherModules]
	[NativeHeader("Runtime/Scripting/GarbageCollector.h")]
	public static class GarbageCollector
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<GarbageCollector.Mode> GCModeChanged;

		public static GarbageCollector.Mode GCMode
		{
			get
			{
				return GarbageCollector.GetMode();
			}
			set
			{
				if (value != GarbageCollector.GetMode())
				{
					GarbageCollector.SetMode(value);
					if (GarbageCollector.GCModeChanged != null)
					{
						GarbageCollector.GCModeChanged(value);
					}
				}
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMode(GarbageCollector.Mode mode);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GarbageCollector.Mode GetMode();

		public enum Mode
		{
			Disabled,
			Enabled
		}
	}
}
