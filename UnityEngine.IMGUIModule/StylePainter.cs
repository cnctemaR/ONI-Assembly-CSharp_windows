using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Utilities/TextUtil.h")]
	[NativeHeader("Modules/IMGUI/StylePainter.h")]
	[NativeHeader("Modules/TextRendering/Public/Font.h")]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[StructLayout(LayoutKind.Sequential)]
	internal class StylePainter : IStylePainter
	{
		public StylePainter()
		{
			this.m_Ptr = StylePainter.Internal_Create();
		}

		public StylePainter(Vector2 pos)
			: this()
		{
			this.mousePosition = pos;
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr self);

		internal void DrawRect(Rect screenRect, Color color, Vector4 borderWidths, Vector4 borderRadiuses)
		{
			this.DrawRect_Injected(ref screenRect, ref color, ref borderWidths, ref borderRadiuses);
		}

		internal void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, Color color, Vector4 borderWidths, Vector4 borderRadiuses, int leftBorder, int topBorder, int rightBorder, int bottomBorder, bool usePremultiplyAlpha)
		{
			this.DrawTexture_Injected(ref screenRect, texture, ref sourceRect, ref color, ref borderWidths, ref borderRadiuses, leftBorder, topBorder, rightBorder, bottomBorder, usePremultiplyAlpha);
		}

		internal void DrawText(Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping textClipping)
		{
			this.DrawText_Injected(ref screenRect, text, font, fontSize, fontStyle, ref fontColor, anchor, wordWrap, wordWrapWidth, richText, textClipping);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float ComputeTextWidth(string text, float width, bool wordWrap, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, bool richText);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float ComputeTextHeight(string text, float width, bool wordWrap, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, bool richText);

		public Vector2 GetCursorPosition(string text, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, float wordWrapWidth, bool richText, Rect screenRect, int cursorPosition)
		{
			Vector2 vector;
			this.GetCursorPosition_Injected(text, font, fontSize, fontStyle, anchor, wordWrapWidth, richText, ref screenRect, cursorPosition, out vector);
			return vector;
		}

		~StylePainter()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				StylePainter.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public void DrawRect(RectStylePainterParameters painterParams)
		{
			Rect rect = painterParams.rect;
			Color color = painterParams.color;
			Vector4 widths = painterParams.border.GetWidths();
			Vector4 radiuses = painterParams.border.GetRadiuses();
			this.DrawRect(rect, color * this.m_OpacityColor, widths, radiuses);
		}

		public void DrawTexture(TextureStylePainterParameters painterParams)
		{
			Rect rect = painterParams.rect;
			Rect rect2 = ((!(painterParams.uv != Rect.zero)) ? new Rect(0f, 0f, 1f, 1f) : painterParams.uv);
			Texture texture = painterParams.texture;
			Color color = painterParams.color;
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
			this.DrawTexture(rect3, texture, rect2, color * this.m_OpacityColor, widths, radiuses, sliceLeft, sliceTop, sliceRight, sliceBottom, usePremultiplyAlpha);
		}

		public void DrawText(TextStylePainterParameters painterParams)
		{
			Rect rect = painterParams.rect;
			string text = painterParams.text;
			Font font = painterParams.font;
			int fontSize = painterParams.fontSize;
			FontStyle fontStyle = painterParams.fontStyle;
			Color fontColor = painterParams.fontColor;
			TextAnchor anchor = painterParams.anchor;
			bool wordWrap = painterParams.wordWrap;
			float wordWrapWidth = painterParams.wordWrapWidth;
			bool richText = painterParams.richText;
			TextClipping clipping = painterParams.clipping;
			this.DrawText(rect, text, font, fontSize, fontStyle, fontColor * this.m_OpacityColor, anchor, wordWrap, wordWrapWidth, richText, clipping);
		}

		public Vector2 GetCursorPosition(CursorPositionStylePainterParameters painterParams)
		{
			Font font = painterParams.font;
			Vector2 vector;
			if (font == null)
			{
				Debug.LogError("StylePainter: Can't process a null font.");
				vector = Vector2.zero;
			}
			else
			{
				string text = painterParams.text;
				int fontSize = painterParams.fontSize;
				FontStyle fontStyle = painterParams.fontStyle;
				TextAnchor anchor = painterParams.anchor;
				float wordWrapWidth = painterParams.wordWrapWidth;
				bool richText = painterParams.richText;
				Rect rect = painterParams.rect;
				int cursorIndex = painterParams.cursorIndex;
				vector = this.GetCursorPosition(text, font, fontSize, fontStyle, anchor, wordWrapWidth, richText, rect, cursorIndex);
			}
			return vector;
		}

		public float ComputeTextWidth(TextStylePainterParameters painterParams)
		{
			string text = painterParams.text;
			float wordWrapWidth = painterParams.wordWrapWidth;
			bool wordWrap = painterParams.wordWrap;
			Font font = painterParams.font;
			int fontSize = painterParams.fontSize;
			FontStyle fontStyle = painterParams.fontStyle;
			TextAnchor anchor = painterParams.anchor;
			bool richText = painterParams.richText;
			return this.ComputeTextWidth(text, wordWrapWidth, wordWrap, font, fontSize, fontStyle, anchor, richText);
		}

		public float ComputeTextHeight(TextStylePainterParameters painterParams)
		{
			string text = painterParams.text;
			float wordWrapWidth = painterParams.wordWrapWidth;
			bool wordWrap = painterParams.wordWrap;
			Font font = painterParams.font;
			int fontSize = painterParams.fontSize;
			FontStyle fontStyle = painterParams.fontStyle;
			TextAnchor anchor = painterParams.anchor;
			bool richText = painterParams.richText;
			return this.ComputeTextHeight(text, wordWrapWidth, wordWrap, font, fontSize, fontStyle, anchor, richText);
		}

		public Matrix4x4 currentTransform { get; set; }

		public Vector2 mousePosition { get; set; }

		public Rect currentWorldClip { get; set; }

		public Event repaintEvent { get; set; }

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
		private extern void DrawRect_Injected(ref Rect screenRect, ref Color color, ref Vector4 borderWidths, ref Vector4 borderRadiuses);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DrawTexture_Injected(ref Rect screenRect, Texture texture, ref Rect sourceRect, ref Color color, ref Vector4 borderWidths, ref Vector4 borderRadiuses, int leftBorder, int topBorder, int rightBorder, int bottomBorder, bool usePremultiplyAlpha);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DrawText_Injected(ref Rect screenRect, string text, Font font, int fontSize, FontStyle fontStyle, ref Color fontColor, TextAnchor anchor, bool wordWrap, float wordWrapWidth, bool richText, TextClipping textClipping);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetCursorPosition_Injected(string text, Font font, int fontSize, FontStyle fontStyle, TextAnchor anchor, float wordWrapWidth, bool richText, ref Rect screenRect, int cursorPosition, out Vector2 ret);

		[NonSerialized]
		internal IntPtr m_Ptr;

		private Color m_OpacityColor = Color.white;
	}
}
