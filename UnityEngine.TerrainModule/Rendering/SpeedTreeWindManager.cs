using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Modules/Terrain/Public/SpeedTreeWindManager.h")]
	[StaticAccessor("GetSpeedTreeWindManager()", StaticAccessorType.Dot)]
	internal static class SpeedTreeWindManager
	{
		public unsafe static void UpdateWindAndWriteBufferWindParams(ReadOnlySpan<int> renderersID, SpeedTreeWindParamsBufferIterator windParams, bool history)
		{
			ReadOnlySpan<int> readOnlySpan = renderersID;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				SpeedTreeWindManager.UpdateWindAndWriteBufferWindParams_Injected(ref managedSpanWrapper, ref windParams, history);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateWindAndWriteBufferWindParams_Injected(ref ManagedSpanWrapper renderersID, [In] ref SpeedTreeWindParamsBufferIterator windParams, bool history);
	}
}
