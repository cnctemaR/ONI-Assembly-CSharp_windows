using System;

namespace UnityEngine.NVIDIA
{
	public struct DLSSTextureTable
	{
		public Texture colorInput { readonly get; set; }

		public Texture colorOutput { readonly get; set; }

		public Texture depth { readonly get; set; }

		public Texture motionVectors { readonly get; set; }

		public Texture transparencyMask { readonly get; set; }

		public Texture exposureTexture { readonly get; set; }

		public Texture biasColorMask { readonly get; set; }
	}
}
