using System;
using Unity.Collections;
using UnityEngine;

namespace Unity.Profiling
{
	public struct DebugScreenCapture
	{
		public NativeArray<byte> RawImageDataReference { readonly get; set; }

		public TextureFormat ImageFormat { readonly get; set; }

		public int Width { readonly get; set; }

		public int Height { readonly get; set; }
	}
}
