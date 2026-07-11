using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct TextureStylePainterParameters
	{
		public static TextureStylePainterParameters GetDefault(VisualElement ve)
		{
			IStyle style = ve.style;
			TextureStylePainterParameters textureStylePainterParameters = new TextureStylePainterParameters
			{
				rect = GUIUtility.AlignRectToDevice(ve.rect),
				uv = new Rect(0f, 0f, 1f, 1f),
				color = Color.white,
				texture = style.backgroundImage,
				scaleMode = style.backgroundScaleMode,
				sliceLeft = style.sliceLeft,
				sliceTop = style.sliceTop,
				sliceRight = style.sliceRight,
				sliceBottom = style.sliceBottom
			};
			BorderParameters.SetFromStyle(ref textureStylePainterParameters.border, style);
			return textureStylePainterParameters;
		}

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
