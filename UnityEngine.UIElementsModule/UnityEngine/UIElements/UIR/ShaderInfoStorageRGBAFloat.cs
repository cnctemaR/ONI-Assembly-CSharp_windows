using System;

namespace UnityEngine.UIElements.UIR
{
	internal class ShaderInfoStorageRGBAFloat : ShaderInfoStorage<Color>
	{
		public ShaderInfoStorageRGBAFloat(int initialSize = 64, int maxSize = 4096)
			: base(TextureFormat.RGBAFloat, ShaderInfoStorageRGBAFloat.s_Convert, initialSize, maxSize)
		{
		}

		private static readonly Func<Color, Color> s_Convert = (Color c) => c;
	}
}
