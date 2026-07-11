using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct CursorPositionStylePainterParameters
	{
		public static CursorPositionStylePainterParameters GetDefault(VisualElement ve, string text)
		{
			IStyle style = ve.style;
			return new CursorPositionStylePainterParameters
			{
				rect = ve.contentRect,
				text = text,
				font = style.font,
				fontSize = style.fontSize,
				fontStyle = style.fontStyleAndWeight,
				anchor = style.unityTextAlign,
				wordWrapWidth = ((!style.wordWrap) ? 0f : ve.contentRect.width),
				richText = false,
				cursorIndex = 0
			};
		}

		internal TextNativeSettings GetTextNativeSettings(float scaling)
		{
			return new TextNativeSettings
			{
				text = this.text,
				font = this.font,
				size = this.fontSize,
				scaling = scaling,
				style = this.fontStyle,
				color = Color.white,
				anchor = this.anchor,
				wordWrap = true,
				wordWrapWidth = this.wordWrapWidth,
				richText = this.richText
			};
		}

		public Rect rect;

		public string text;

		public Font font;

		public int fontSize;

		public FontStyle fontStyle;

		public TextAnchor anchor;

		public float wordWrapWidth;

		public bool richText;

		public int cursorIndex;
	}
}
