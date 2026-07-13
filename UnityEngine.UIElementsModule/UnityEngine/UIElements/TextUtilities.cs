using System;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal static class TextUtilities
	{
		public static TextSettings textSettings
		{
			get
			{
				bool flag = TextUtilities.s_TextSettings == null;
				if (flag)
				{
					TextUtilities.s_TextSettings = TextUtilities.getEditorTextSettings();
				}
				return TextUtilities.s_TextSettings;
			}
		}

		private static Vector2 PostProcessMeasuredSize(TextElement te, Vector2 measuredSize, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float pixelsPerPoint)
		{
			float num = measuredSize.x;
			float num2 = measuredSize.y;
			bool flag = widthMode == VisualElement.MeasureMode.Exactly;
			if (flag)
			{
				num = width;
			}
			else
			{
				bool flag2 = widthMode == VisualElement.MeasureMode.AtMost;
				if (flag2)
				{
					num = Mathf.Min(num, width);
				}
			}
			bool flag3 = heightMode == VisualElement.MeasureMode.Exactly;
			if (flag3)
			{
				num2 = height;
			}
			else
			{
				bool flag4 = heightMode == VisualElement.MeasureMode.AtMost;
				if (flag4)
				{
					num2 = Mathf.Min(num2, height);
				}
			}
			float num3 = AlignmentUtils.CeilToPixelGrid(num, pixelsPerPoint, 0f);
			float num4 = AlignmentUtils.CeilToPixelGrid(num2, pixelsPerPoint, 0f);
			Vector2 vector = new Vector2(num3, num4);
			bool flag5 = TextUtilities.IsAdvancedTextEnabledForElement(te);
			if (flag5)
			{
				te.uitkTextHandle.ATGMeasuredWidth = new float?(num);
				te.uitkTextHandle.ATGRoundedWidth = num3;
				te.uitkTextHandle.LastPixelPerPoint = pixelsPerPoint;
			}
			else
			{
				te.uitkTextHandle.MeasuredWidth = new float?(num);
				te.uitkTextHandle.RoundedWidth = num3;
				te.uitkTextHandle.LastPixelPerPoint = pixelsPerPoint;
			}
			return vector;
		}

		internal static Vector2 MeasureVisualElementTextSize(TextElement te, string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
		{
			bool flag = !TextUtilities.IsFontAssigned(te);
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(float.NaN, float.NaN);
			}
			else
			{
				IPanel panel = te.panel;
				float num = ((panel != null) ? panel.scaledPixelsPerPoint : 1f);
				bool flag2 = num <= 0f;
				if (flag2)
				{
					vector = Vector2.zero;
				}
				else
				{
					Vector2 vector2 = Vector2.zero;
					bool flag3 = widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly;
					if (flag3)
					{
						vector2 = te.uitkTextHandle.ComputeTextSize(textToMeasure, width, widthMode, height, heightMode, fontsize);
					}
					vector = TextUtilities.PostProcessMeasuredSize(te, vector2, width, widthMode, height, heightMode, num);
				}
			}
			return vector;
		}

		internal static Vector2 MeasureVisualElementTextSize(TextElement te, in RenderedText textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
		{
			bool flag = !TextUtilities.IsFontAssigned(te);
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(float.NaN, float.NaN);
			}
			else
			{
				IPanel panel = te.panel;
				float num = ((panel != null) ? panel.scaledPixelsPerPoint : 1f);
				bool flag2 = num <= 0f;
				if (flag2)
				{
					vector = Vector2.zero;
				}
				else
				{
					Vector2 vector2 = Vector2.zero;
					bool flag3 = widthMode != VisualElement.MeasureMode.Exactly || heightMode != VisualElement.MeasureMode.Exactly;
					if (flag3)
					{
						vector2 = te.uitkTextHandle.ComputeTextSize(in textToMeasure, width, height, fontsize);
					}
					vector = TextUtilities.PostProcessMeasuredSize(te, vector2, width, widthMode, height, heightMode, num);
				}
			}
			return vector;
		}

		internal static FontAsset GetFontAsset(VisualElement ve)
		{
			bool flag = ve.computedStyle.unityFontDefinition.fontAsset != null;
			FontAsset fontAsset;
			if (flag)
			{
				fontAsset = ve.computedStyle.unityFontDefinition.fontAsset;
			}
			else
			{
				TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(ve);
				bool flag2 = !object.Equals(ve.computedStyle.unityFontDefinition.font, null);
				if (flag2)
				{
					fontAsset = textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFontDefinition.font);
				}
				else
				{
					bool flag3 = !object.Equals(ve.computedStyle.unityFont, null);
					if (flag3)
					{
						fontAsset = textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFont);
					}
					else
					{
						bool flag4 = !object.Equals(textSettingsFrom, null);
						if (flag4)
						{
							fontAsset = textSettingsFrom.defaultFontAsset;
						}
						else
						{
							fontAsset = null;
						}
					}
				}
			}
			return fontAsset;
		}

		internal static bool IsFontAssigned(VisualElement ve)
		{
			return ve.computedStyle.unityFont != null || !ve.computedStyle.unityFontDefinition.IsEmpty();
		}

		internal static TextSettings GetTextSettingsFrom(VisualElement ve)
		{
			RuntimePanel runtimePanel = ve.panel as RuntimePanel;
			bool flag = runtimePanel != null;
			TextSettings textSettings;
			if (flag)
			{
				textSettings = runtimePanel.panelSettings.textSettings ?? PanelTextSettings.defaultPanelTextSettings;
			}
			else
			{
				textSettings = PanelTextSettings.defaultPanelTextSettings;
			}
			return textSettings;
		}

		internal static bool IsAdvancedTextEnabledForPanel(IPanel panel)
		{
			bool flag = false;
			RuntimePanel runtimePanel = panel as RuntimePanel;
			bool flag2 = runtimePanel != null;
			if (flag2)
			{
				PanelSettings panelSettings = runtimePanel.panelSettings;
				flag = ((panelSettings != null) ? panelSettings.m_ICUDataAsset : null) != null;
			}
			return flag;
		}

		internal static bool IsAdvancedTextEnabledForElement(VisualElement ve)
		{
			bool flag = ve == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = ve.computedStyle.unityTextGenerator == TextGeneratorType.Advanced;
				bool flag4 = flag3 && TextUtilities.IsAdvancedTextEnabledForPanel(ve.panel);
				flag2 = flag3 && flag4;
			}
			return flag2;
		}

		internal unsafe static TextCoreSettings GetTextCoreSettingsForElement(VisualElement ve, bool ignoreColors)
		{
			FontAsset fontAsset = TextUtilities.GetFontAsset(ve);
			bool flag = fontAsset == null;
			TextCoreSettings textCoreSettings;
			if (flag)
			{
				textCoreSettings = default(TextCoreSettings);
			}
			else
			{
				IResolvedStyle resolvedStyle = ve.resolvedStyle;
				ComputedStyle computedStyle = *ve.computedStyle;
				TextShadow textShadow = computedStyle.textShadow;
				float num = TextHandle.ConvertPixelUnitsToTextCoreRelativeUnits(computedStyle.fontSize.value, fontAsset);
				float num2 = Mathf.Clamp(resolvedStyle.unityTextOutlineWidth * num, 0f, 1f);
				float num3 = Mathf.Clamp(textShadow.blurRadius * num, 0f, 1f);
				float num4 = ((textShadow.offset.x < 0f) ? Mathf.Max(textShadow.offset.x * num, -1f) : Mathf.Min(textShadow.offset.x * num, 1f));
				float num5 = ((textShadow.offset.y < 0f) ? Mathf.Max(textShadow.offset.y * num, -1f) : Mathf.Min(textShadow.offset.y * num, 1f));
				Vector2 vector = new Vector2(num4, num5);
				Color color;
				Color color3;
				if (ignoreColors)
				{
					color = Color.white;
					Color color2 = Color.white;
					color3 = Color.white;
				}
				else
				{
					bool flag2 = ((Texture2D)fontAsset.material.mainTexture).format != TextureFormat.Alpha8;
					color = resolvedStyle.color;
					color3 = resolvedStyle.unityTextOutlineColor;
					bool flag3 = num2 < 1E-30f;
					if (flag3)
					{
						color3.a = 0f;
					}
					Color color2 = textShadow.color;
					bool flag4 = flag2;
					if (flag4)
					{
						color = new Color(1f, 1f, 1f, color.a);
					}
					else
					{
						color2.r *= color.a;
						color2.g *= color.a;
						color2.b *= color.a;
						color3.r *= color3.a;
						color3.g *= color3.a;
						color3.b *= color3.a;
					}
				}
				textCoreSettings = new TextCoreSettings
				{
					faceColor = color,
					outlineColor = color3,
					outlineWidth = num2,
					underlayColor = textShadow.color,
					underlayOffset = vector,
					underlaySoftness = num3
				};
			}
			return textCoreSettings;
		}

		public static TextWrappingMode toTextWrappingMode(this WhiteSpace whiteSpace, bool isSingleLineInputField)
		{
			TextWrappingMode textWrappingMode2;
			if (isSingleLineInputField)
			{
				if (!true)
				{
				}
				TextWrappingMode textWrappingMode;
				if (whiteSpace > WhiteSpace.NoWrap)
				{
					if (whiteSpace - WhiteSpace.Pre > 1)
					{
						textWrappingMode = TextWrappingMode.NoWrap;
					}
					else
					{
						textWrappingMode = TextWrappingMode.PreserveWhitespaceNoWrap;
					}
				}
				else
				{
					textWrappingMode = TextWrappingMode.NoWrap;
				}
				if (!true)
				{
				}
				textWrappingMode2 = textWrappingMode;
			}
			else
			{
				if (!true)
				{
				}
				TextWrappingMode textWrappingMode;
				switch (whiteSpace)
				{
				case WhiteSpace.Normal:
					textWrappingMode = TextWrappingMode.Normal;
					break;
				case WhiteSpace.NoWrap:
					textWrappingMode = TextWrappingMode.NoWrap;
					break;
				case WhiteSpace.Pre:
					textWrappingMode = TextWrappingMode.PreserveWhitespaceNoWrap;
					break;
				case WhiteSpace.PreWrap:
					textWrappingMode = TextWrappingMode.PreserveWhitespace;
					break;
				default:
					textWrappingMode = TextWrappingMode.Normal;
					break;
				}
				if (!true)
				{
				}
				textWrappingMode2 = textWrappingMode;
			}
			return textWrappingMode2;
		}

		public static TextOverflow toTextCore(this TextOverflow textOverflow, OverflowInternal overflow, TextOverflowPosition position)
		{
			bool flag = position > TextOverflowPosition.End;
			TextOverflow textOverflow2;
			if (flag)
			{
				textOverflow2 = TextOverflow.Clip;
			}
			else
			{
				if (!true)
				{
				}
				TextOverflow textOverflow3;
				if (textOverflow == TextOverflow.Ellipsis)
				{
					if (overflow == OverflowInternal.Hidden)
					{
						textOverflow3 = TextOverflow.Ellipsis;
						goto IL_0027;
					}
				}
				textOverflow3 = TextOverflow.Clip;
				IL_0027:
				if (!true)
				{
				}
				textOverflow2 = textOverflow3;
			}
			return textOverflow2;
		}

		public static Func<TextSettings> getEditorTextSettings;

		internal static Func<bool> IsAdvancedTextEnabled;

		private static TextSettings s_TextSettings;
	}
}
