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
			UnityEngine.Rendering.VirtualTexturing.System.SetDebugFlag(guid.ToByteArray(), enabled);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDebugFlag(byte[] guid, bool enabled);

		public const int AllMips = 2147483647;
	}
}
