using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.UIElements
{
	[NativeHeader("Modules/UIElements/ImmediateStylePainter.h")]
	[StructLayout(LayoutKind.Sequential)]
	internal class ImmediateStylePainter : IStylePainterInternal, IStylePainter
	{
		internal static void DrawRect(Rect screenRect, Color color, Vector4 borderWidths, Vector4 borderRadiuses)
		{
			ImmediateStylePainter.DrawRect_Injected(ref screenRect, ref color, ref borderWidths, ref borderRadiuses);
		}

		internal static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, Color color, Vector4 borderWidths, Vector4 borderRadiuses, int leftBorder, int topBorder, int rightBorder, int bottomBorder, bool usePremultiplyAlpha)
		{
			ImmediateStylePainter.DrawTexture_Injected(ref screenRect, texture, ref sourceRect, ref color, ref borderWidths, ref borderRadiuses, leftBorder, topBorder, rightBorder, bottomBorder, usePremultiplyAlpha);
		}

		internal static void DrawText(Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping textClipping)
		{
			ImmediateStylePainter.DrawText_Injected(ref screenRect, text, font, fontSize, fontStyle, ref fontColor, anchor, wordWrap, wordWrapWidth, richText, textClipping);
		}

		public VisualElement currentElement { get; set; }

		public void DrawRect(RectStylePainterParameters painterParams)
		{
			Rect rect = painterParams.rect;
			Color color = painterParams.color * UIElementsUtility.editorPlayModeTintColor;
			Vector4 widths = painterParams.border.GetWidths();
			Vector4 radiuses = painterParams.border.GetRadiuses();
			ImmediateStylePainter.DrawRect(rect, color * this.m_OpacityColor, widths, radiuses);
		}

		public void DrawTexture(TextureStylePainterParameters painterParams)
		{
			Rect rect = painterParams.rect;
			Rect rect2 = ((!(painterParams.uv != Rect.zero)) ? new Rect(0f, 0f, 1f, 1f) : painterParams.uv);
			Texture texture = painterParams.texture;
			Color color = painterParams.color * UIElementsUtility.editorPlayModeTintColor;
			ScaleMode scaleMode = painterParams.scaleMode;
			int sliceLeft = painterParams.sliceLeft;
			int sliceTop = painterParams.sliceTop;
			int sliceRight = painterParams.sliceRight;
			int sliceBottom = painterParams.sliceBottom;
			bool usePremultiplyAlpha = painterParams.usePremultiplyAlpha;
			Rect rect3 = rect;
			float num = (float)texture.width * rect2.width / ((float)texture.height * rect2.height);
			float num2 = rect.width / rect.height;
			if (scaleMode != ScaleMode.StretchToFill)
			{
				if (scaleMode != ScaleMode.ScaleAndCrop)
				{
					if (scaleMode == ScaleMode.ScaleToFit)
					{
						if (num2 > num)
						{
							float num3 = num / num2;
							rect3 = new Rect(rect.xMin + rect.width * (1f - num3) * 0.5f, rect.yMin, num3 * rect.width, rect.height);
						}
						else
						{
							float num4 = num2 / num;
							rect3 = new Rect(rect.xMin, rect.yMin + rect.height * (1f - num4) * 0.5f, rect.width, num4 * rect.height);
						}
					}
				}
				else if (num2 > num)
				{
					float num5 = rect2.height * (num / num2);
					float num6 = (rect2.height - num5) * 0.5f;
					rect2 = new Rect(rect2.x, rect2.y + num6, rect2.width, num5);
				}
				else
				{
					float num7 = rect2.width * (num2 / num);
					float num8 = (rect2.width - num7) * 0.5f;
					rect2 = new Rect(rect2.x + num8, rect2.y, num7, rect2.height);
				}
			}
			Vector4 widths = painterParams.border.GetWidths();
			Vector4 radiuses = painterParams.border.GetRadiuses();
			ImmediateStylePainter.DrawTexture(rect3, texture, rect2, color * this.m_OpacityColor, widths, radiuses, sliceLeft, sliceTop, sliceRight, sliceBottom, usePremultiplyAlpha);
		}

		public void DrawText(TextStylePainterParameters painterParams)
		{
			Rect rect = painterParams.rect;
			string text = painterParams.text;
			Font font = painterParams.font;
			int fontSize = painterParams.fontSize;
			FontStyle fontStyle = painterParams.fontStyle;
			Color color = painterParams.fontColor * UIElementsUtility.editorPlayModeTintColor;
			TextAnchor anchor = painterParams.anchor;
			bool wordWrap = painterParams.wordWrap;
			float wordWrapWidth = painterParams.wordWrapWidth;
			bool richText = painterParams.richText;
			TextClipping clipping = painterParams.clipping;
			ImmediateStylePainter.DrawText(rect, text, font, fontSize, fontStyle, color * this.m_OpacityColor, anchor, wordWrap, wordWrapWidth, richText, clipping);
		}

		public void DrawMesh(MeshStylePainterParameters painterParams)
		{
			Mesh mesh = painterParams.mesh;
			Material material = painterParams.material;
			int pass = painterParams.pass;
			material.SetPass(pass);
			Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
		}

		public void DrawImmediate(Action callback)
		{
			callback();
		}

		public void DrawBackground()
		{
			IStyle style = this.currentElement.style;
			if (style.backgroundColor != Color.clear)
			{
				RectStylePainterParameters @default = RectStylePainterParameters.GetDefault(this.currentElement);
				@default.border.SetWidth(0f);
				this.DrawRect(@default);
			}
			if (style.backgroundImage.value != null)
			{
				TextureStylePainterParameters default2 = TextureStylePainterParameters.GetDefault(this.currentElement);
				default2.border.SetWidth(0f);
				this.DrawTexture(default2);
			}
		}

		public void DrawBorder()
		{
			IStyle style = this.currentElement.style;
			if (style.borderColor != Color.clear && (style.borderLeftWidth > 0f || style.borderTopWidth > 0f || style.borderRightWidth > 0f || style.borderBottomWidth > 0f))
			{
				RectStylePainterParameters @default = RectStylePainterParameters.GetDefault(this.currentElement);
				@default.color = style.borderColor;
				this.DrawRect(@default);
			}
		}

		public void DrawText(string text)
		{
			if (!string.IsNullOrEmpty(text) && this.currentElement.contentRect.width > 0f && this.currentElement.contentRect.height > 0f)
			{
				this.DrawText(TextStylePainterParameters.GetDefault(this.currentElement, text));
			}
		}

		public float opacity
		{
			get
			{
				return this.m_OpacityColor.a;
			}
			set
			{
				this.m_OpacityColor.a = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawRect_Injected(ref Rect screenRect, ref Color color, ref Vector4 borderWidths, ref Vector4 borderRadiuses);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawTexture_Injected(ref Rect screenRect, Texture texture, ref Rect sourceRect, ref Color color, ref Vector4 borderWidths, ref Vector4 borderRadiuses, int leftBorder, int topBorder, int rightBorder, int bottomBorder, bool usePremultiplyAlpha);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawText_Injected(ref Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, ref Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping textClipping);

		private Color m_OpacityColor = Color.white;
	}
}
