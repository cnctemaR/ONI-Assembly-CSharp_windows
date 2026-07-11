using System;

namespace UnityEngine.Experimental.UIElements
{
	internal static class StylePainterExtensionMethods
	{
		internal static TextureStylePainterParameters GetDefaultTextureParameters(this IStylePainter painter, VisualElement ve)
		{
			IStyle style = ve.style;
			TextureStylePainterParameters textureStylePainterParameters = new TextureStylePainterParameters
			{
				rect = GUIUtility.AlignRectToDevice(ve.rect),
				uv = new Rect(0f, 0f, 1f, 1f),
				color = Color.white,
				texture = style.backgroundImage,
				scaleMode = style.backgroundSize,
				sliceLeft = style.sliceLeft,
				sliceTop = style.sliceTop,
				sliceRight = style.sliceRight,
				sliceBottom = style.sliceBottom
			};
			painter.SetBorderFromStyle(ref textureStylePainterParameters.border, style);
			return textureStylePainterParameters;
		}

		internal static RectStylePainterParameters GetDefaultRectParameters(this IStylePainter painter, VisualElement ve)
		{
			IStyle style = ve.style;
			RectStylePainterParameters rectStylePainterParameters = new RectStylePainterParameters
			{
				rect = GUIUtility.AlignRectToDevice(ve.rect),
				color = style.backgroundColor
			};
			painter.SetBorderFromStyle(ref rectStylePainterParameters.border, style);
			return rectStylePainterParameters;
		}

		internal static TextStylePainterParameters GetDefaultTextParameters(this IStylePainter painter, BaseTextElement te)
		{
			IStyle style = te.style;
			return new TextStylePainterParameters
			{
				rect = te.contentRect,
				text = te.text,
				font = style.font,
				fontSize = style.fontSize,
				fontStyle = style.fontStyle,
				fontColor = style.textColor.GetSpecifiedValueOrDefault(Color.black),
				anchor = style.textAlignment,
				wordWrap = style.wordWrap,
				wordWrapWidth = ((!style.wordWrap) ? 0f : te.contentRect.width),
				richText = false,
				clipping = style.textClipping
			};
		}

		internal static CursorPositionStylePainterParameters GetDefaultCursorPositionParameters(this IStylePainter painter, BaseTextElement te)
		{
			IStyle style = te.style;
			return new CursorPositionStylePainterParameters
			{
				rect = te.contentRect,
				text = te.text,
				font = style.font,
				fontSize = style.fontSize,
				fontStyle = style.fontStyle,
				anchor = style.textAlignment,
				wordWrapWidth = ((!style.wordWrap) ? 0f : te.contentRect.width),
				richText = false,
				cursorIndex = 0
			};
		}

		internal static void DrawBackground(this IStylePainter painter, VisualElement ve)
		{
			IStyle style = ve.style;
			if (style.backgroundColor != Color.clear)
			{
				RectStylePainterParameters defaultRectParameters = painter.GetDefaultRectParameters(ve);
				defaultRectParameters.border.SetWidth(0f);
				painter.DrawRect(defaultRectParameters);
			}
			if (style.backgroundImage.value != null)
			{
				TextureStylePainterParameters defaultTextureParameters = painter.GetDefaultTextureParameters(ve);
				defaultTextureParameters.border.SetWidth(0f);
				painter.DrawTexture(defaultTextureParameters);
			}
		}

		internal static void DrawBorder(this IStylePainter painter, VisualElement ve)
		{
			IStyle style = ve.style;
			if (style.borderColor != Color.clear && (style.borderLeftWidth > 0f || style.borderTopWidth > 0f || style.borderRightWidth > 0f || style.borderBottomWidth > 0f))
			{
				RectStylePainterParameters defaultRectParameters = painter.GetDefaultRectParameters(ve);
				defaultRectParameters.color = style.borderColor;
				painter.DrawRect(defaultRectParameters);
			}
		}

		internal static void DrawText(this IStylePainter painter, BaseTextElement te)
		{
			if (!string.IsNullOrEmpty(te.text) && te.contentRect.width > 0f && te.contentRect.height > 0f)
			{
				painter.DrawText(painter.GetDefaultTextParameters(te));
			}
		}

		internal static void SetBorderFromStyle(this IStylePainter painter, ref BorderParameters border, IStyle style)
		{
			border.SetWidth(style.borderTopWidth, style.borderRightWidth, style.borderBottomWidth, style.borderLeftWidth);
			border.SetRadius(style.borderTopLeftRadius, style.borderTopRightRadius, style.borderBottomRightRadius, style.borderBottomLeftRadius);
		}
	}
}
