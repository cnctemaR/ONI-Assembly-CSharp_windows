using System;
using Unity.Collections;

namespace UnityEngine.Profiling.Experimental
{
	public struct DebugScreenCapture
	{
		public NativeArray<byte> rawImageDataReference { readonly get; set; }

		public TextureFormat imageFormat { readonly get; set; }

		public int width { readonly get; set; }

		public int height { readonly get; set; }
	}
}
