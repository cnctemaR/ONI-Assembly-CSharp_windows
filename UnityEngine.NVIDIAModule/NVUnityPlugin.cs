using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.NVIDIA
{
	[NativeHeader("Modules/NVIDIA/NVPlugins.h")]
	public static class NVUnityPlugin
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Load();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsLoaded();
	}
}
