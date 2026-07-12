using System;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[UsedByNativeCode]
	[NativeHeader("Modules/VirtualTexturing/Public/VirtualTexturingSettings.h")]
	[Serializable]
	public struct GPUCacheSetting
	{
		public GraphicsFormat format;

		public uint sizeInMegaBytes;
	}
}
