using System;

namespace UnityEngine
{
	public struct TextureMipmapLimitSettings
	{
		public TextureMipmapLimitBiasMode limitBiasMode { readonly get; set; }

		public int limitBias { readonly get; set; }
	}
}
