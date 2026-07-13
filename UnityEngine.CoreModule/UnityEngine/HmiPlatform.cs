using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Hmi/HmiPlatform.bindings.h")]
	public class HmiPlatform
	{
		[ExcludeFromDocs]
		public unsafe static void LogStartupTiming(string tag)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				HmiPlatform.LogStartupTiming_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LogStartupTiming_Injected(ref ManagedSpanWrapper tag);
	}
}
