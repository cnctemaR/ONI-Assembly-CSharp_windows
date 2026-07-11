using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct TextureStylePainterParameters
	{
		public Rect rect;

		public Rect uv;

		public Color color;

		public Texture texture;

		public ScaleMode scaleMode;

		public BorderParameters border;

		public int sliceLeft;

		public int sliceTop;

		public int sliceRight;

		public int sliceBottom;

		public bool usePremultiplyAlpha;
	}
}
