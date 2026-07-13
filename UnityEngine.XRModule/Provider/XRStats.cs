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

		[StaticAccessor("XRStats::Get()", StaticAccessorType.Dot)]
		[NativeConditional("ENABLE_XR")]
		[NativeHeader("Modules/XR/Stats/XRStats.h")]
		[NativeMethod("TryGetStatByName_Internal")]
		private unsafe static bool TryGetStat_Internal(IntPtr ptr, string tag, out float value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr2 = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr2, readOnlySpan.Length);
					}
				}
				flag = XRStats.TryGetStat_Internal_Injected(ptr, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr2 = null;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetStat_Internal_Injected(IntPtr ptr, ref ManagedSpanWrapper tag, out float value);
	}
}
