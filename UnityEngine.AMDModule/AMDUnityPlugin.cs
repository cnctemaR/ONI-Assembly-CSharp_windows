using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.AMD
{
	[NativeHeader("Modules/AMD/AMDPlugins.h")]
	public static class AMDUnityPlugin
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Load();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsLoaded();
	}
}
