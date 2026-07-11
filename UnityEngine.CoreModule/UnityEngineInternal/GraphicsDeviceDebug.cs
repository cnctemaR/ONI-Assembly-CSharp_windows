using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngineInternal
{
	[StaticAccessor("GraphicsDeviceDebug", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Export/Graphics/GraphicsDeviceDebug.bindings.h")]
	internal static class GraphicsDeviceDebug
	{
		internal static GraphicsDeviceDebugSettings settings
		{
			get
			{
				GraphicsDeviceDebugSettings graphicsDeviceDebugSettings;
				GraphicsDeviceDebug.get_settings_Injected(out graphicsDeviceDebugSettings);
				return graphicsDeviceDebugSettings;
			}
			set
			{
				GraphicsDeviceDebug.set_settings_Injected(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_settings_Injected(out GraphicsDeviceDebugSettings ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_settings_Injected(ref GraphicsDeviceDebugSettings value);
	}
}
