using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[StaticAccessor("VirtualTexturing::System", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
	public static class System
	{
		internal static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Update();

		[NativeThrows]
		internal static void SetDebugFlag(Guid guid, bool enabled)
		{
			UnityEngine.Rendering.VirtualTexturing.System.SetDebugFlagInteger(guid.ToByteArray(), enabled ? 1L : 0L);
		}

		[NativeThrows]
		internal static void SetDebugFlagInteger(Guid guid, long value)
		{
			UnityEngine.Rendering.VirtualTexturing.System.SetDebugFlagInteger(guid.ToByteArray(), value);
		}

		[NativeThrows]
		internal static void SetDebugFlagDouble(Guid guid, double value)
		{
			UnityEngine.Rendering.VirtualTexturing.System.SetDebugFlagDouble(guid.ToByteArray(), value);
		}

		[NativeThrows]
		private unsafe static void SetDebugFlagInteger(byte[] guid, long value)
		{
			Span<byte> span = new Span<byte>(guid);
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				UnityEngine.Rendering.VirtualTexturing.System.SetDebugFlagInteger_Injected(ref managedSpanWrapper, value);
			}
		}

		[NativeThrows]
		private unsafe static void SetDebugFlagDouble(byte[] guid, double value)
		{
			Span<byte> span = new Span<byte>(guid);
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				UnityEngine.Rendering.VirtualTexturing.System.SetDebugFlagDouble_Injected(ref managedSpanWrapper, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDebugFlagInteger_Injected(ref ManagedSpanWrapper guid, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDebugFlagDouble_Injected(ref ManagedSpanWrapper guid, double value);

		public const int AllMips = 2147483647;
	}
}
