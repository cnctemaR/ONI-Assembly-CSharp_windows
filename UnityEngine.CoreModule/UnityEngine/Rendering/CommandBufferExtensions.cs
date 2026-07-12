using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Export/Graphics/RenderingCommandBufferExtensions.bindings.h")]
	public static class CommandBufferExtensions
	{
		[FreeFunction("RenderingCommandBufferExtensions_Bindings::Internal_SwitchIntoFastMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SwitchIntoFastMemory([NotNull("NullExceptionObject")] CommandBuffer cmd, ref RenderTargetIdentifier rt, FastMemoryFlags fastMemoryFlags, float residency, bool copyContents);

		[FreeFunction("RenderingCommandBufferExtensions_Bindings::Internal_SwitchOutOfFastMemory")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SwitchOutOfFastMemory([NotNull("NullExceptionObject")] CommandBuffer cmd, ref RenderTargetIdentifier rt, bool copyContents);

		[NativeConditional("UNITY_XBOXONE || UNITY_GAMECORE_XBOXONE")]
		public static void SwitchIntoFastMemory(this CommandBuffer cmd, RenderTargetIdentifier rid, FastMemoryFlags fastMemoryFlags, float residency, bool copyContents)
		{
			CommandBufferExtensions.Internal_SwitchIntoFastMemory(cmd, ref rid, fastMemoryFlags, residency, copyContents);
		}

		[NativeConditional("UNITY_XBOXONE || UNITY_GAMECORE_XBOXONE")]
		public static void SwitchOutOfFastMemory(this CommandBuffer cmd, RenderTargetIdentifier rid, bool copyContents)
		{
			CommandBufferExtensions.Internal_SwitchOutOfFastMemory(cmd, ref rid, copyContents);
		}
	}
}
