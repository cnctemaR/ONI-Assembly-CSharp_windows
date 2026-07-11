using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct TextStylePainterParameters
	{
		public static TextStylePainterParameters GetDefault(VisualElement ve, string text)
		{
			IStyle style = ve.style;
			return new TextStylePainterParameters
			{
				rect = ve.contentRect,
				text = text,
				font = style.font,
				fontSize = style.fontSize,
				fontStyle = style.fontStyleAndWeight,
				fontColor = style.color.GetSpecifiedValueOrDefault(Color.black),
				anchor = style.unityTextAlign,
				wordWrap = style.wordWrap,
				wordWrapWidth = ((!style.wordWrap) ? 0f : ve.contentRect.width),
				richText = false,
				clipping = style.textClipping
			};
		}

		public static TextStylePainterParameters GetDefault(TextElement te)
		{
			return TextStylePainterParameters.GetDefault(te, te.text);
		}

		public TextNativeSettings GetTextNativeSettings(float scaling)
		{
			return new TextNativeSettings
			{
				text = this.text,
				font = this.font,
				size = this.fontSize,
				scaling = scaling,
				style = this.fontStyle,
				color = this.fontColor,
				anchor = this.anchor,
				wordWrap = this.wordWrap,
				wordWrapWidth = this.wordWrapWidth,
				richText = this.richText
			};
		}

		public Rect rect;

		public string text;

		public Font font;

		public int fontSize;

		public FontStyle fontStyle;

		public Color fontColor;

		public TextAnchor anchor;

		public bool wordWrap;

		public float wordWrapWidth;

		public bool richText;

		public TextClipping clipping;
	}
}
