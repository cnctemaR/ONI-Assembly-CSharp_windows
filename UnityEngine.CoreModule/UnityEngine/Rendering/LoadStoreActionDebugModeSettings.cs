using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	public static class LoadStoreActionDebugModeSettings
	{
		[StaticAccessor("GetGfxDevice()", StaticAccessorType.Dot)]
		public static extern bool LoadStoreDebugModeEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
