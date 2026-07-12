using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.Provider
{
	public static class XRStats
	{
		public static bool TryGetStat(IntegratedSubsystem xrSubsystem, string tag, out float value)
		{
			return XRStats.TryGetStat_Internal(xrSubsystem.m_Ptr, tag, out value);
		}

		[NativeMethod("TryGetStatByName_Internal")]
		[StaticAccessor("XRStats::Get()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_XR")]
		[NativeHeader("Modules/XR/Stats/XRStats.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetStat_Internal(IntPtr ptr, string tag, out float value);
	}
}
