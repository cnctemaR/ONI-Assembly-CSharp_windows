using System;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal static class TextUtilities
	{
		internal static Vector2 MeasureVisualElementTextSize(TextElement te, string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			float num = float.NaN;
			float num2 = float.NaN;
			bool flag = textToMeasure == null || !TextUtilities.IsFontAssigned(te);
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(num, num2);
			}
			else
			{
				float scaledPixelsPerPoint = te.scaledPixelsPerPoint;
				bool flag2 = scaledPixelsPerPoint <= 0f;
				if (flag2)
				{
					vector = Vector2.zero;
				}
				else
				{
					bool flag3 = widthMode == VisualElement.MeasureMode.Exactly;
					if (flag3)
					{
						num = width;
					}
					else
					{
						num = te.uitkTextHandle.ComputeTextWidth(textToMeasure, false, width, height);
						bool flag4 = widthMode == VisualElement.MeasureMode.AtMost;
						if (flag4)
						{
							num = Mathf.Min(num, width);
						}
					}
					bool flag5 = heightMode == VisualElement.MeasureMode.Exactly;
					if (flag5)
					{
						num2 = height;
					}
					else
					{
						num2 = te.uitkTextHandle.ComputeTextHeight(textToMeasure, width, height);
						bool flag6 = heightMode == VisualElement.MeasureMode.AtMost;
						if (flag6)
						{
							num2 = Mathf.Min(num2, height);
						}
					}
					float num3 = AlignmentUtils.CeilToPixelGrid(num, scaledPixelsPerPoint, 0f);
					float num4 = AlignmentUtils.CeilToPixelGrid(num2, scaledPixelsPerPoint, 0f);
					Vector2 vector2 = new Vector2(num3, num4);
					te.uitkTextHandle.MeasuredSizes = new Vector2(num, num2);
					te.uitkTextHandle.RoundedSizes = vector2;
					vector = vector2;
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
				PanelTextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(ve);
				bool flag2 = ve.computedStyle.unityFontDefinition.font != null;
				if (flag2)
				{
					fontAsset = textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFontDefinition.font);
				}
				else
				{
					bool flag3 = ve.computedStyle.unityFont != null;
					if (flag3)
					{
						fontAsset = textSettingsFrom.GetCachedFontAsset(ve.computedStyle.unityFont);
					}
					else
					{
						fontAsset = null;
					}
				}
			}
			return fontAsset;
		}

		internal unsafe static Font GetFont(VisualElement ve)
		{
			ComputedStyle computedStyle = *ve.computedStyle;
			bool flag = computedStyle.unityFontDefinition.font != null;
			Font font;
			if (flag)
			{
				font = computedStyle.unityFontDefinition.font;
			}
			else
			{
				bool flag2 = computedStyle.unityFont != null;
				if (flag2)
				{
					font = computedStyle.unityFont;
				}
				else
				{
					FontAsset fontAsset = computedStyle.unityFontDefinition.fontAsset;
					font = ((fontAsset != null) ? fontAsset.sourceFontFile : null);
				}
			}
			return font;
		}

		internal static bool IsFontAssigned(VisualElement ve)
		{
			return ve.computedStyle.unityFont != null || !ve.computedStyle.unityFontDefinition.IsEmpty();
		}

		internal static PanelTextSettings GetTextSettingsFrom(VisualElement ve)
		{
			RuntimePanel runtimePanel = ve.panel as RuntimePanel;
			bool flag = runtimePanel != null;
			PanelTextSettings panelTextSettings;
			if (flag)
			{
				panelTextSettings = runtimePanel.panelSettings.textSettings ?? PanelTextSettings.defaultPanelTextSettings;
			}
			else
			{
				panelTextSettings = PanelTextSettings.defaultPanelTextSettings;
			}
			return panelTextSettings;
		}

		internal static float ConvertPixelUnitsToTextCoreRelativeUnits(VisualElement ve, FontAsset fontAsset)
		{
			float num = 1f / (float)fontAsset.atlasPadding;
			float num2 = (float)fontAsset.faceInfo.pointSize / ve.computedStyle.fontSize.value;
			return num * num2;
		}

		internal unsafe static TextCoreSettings GetTextCoreSettingsForElement(VisualElement ve)
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
				float num = TextUtilities.ConvertPixelUnitsToTextCoreRelativeUnits(ve, fontAsset);
				float num2 = Mathf.Clamp(resolvedStyle.unityTextOutlineWidth * num, 0f, 1f);
				float num3 = Mathf.Clamp(computedStyle.textShadow.blurRadius * num, 0f, 1f);
				float num4 = ((computedStyle.textShadow.offset.x < 0f) ? Mathf.Max(computedStyle.textShadow.offset.x * num, -1f) : Mathf.Min(computedStyle.textShadow.offset.x * num, 1f));
				float num5 = ((computedStyle.textShadow.offset.y < 0f) ? Mathf.Max(computedStyle.textShadow.offset.y * num, -1f) : Mathf.Min(computedStyle.textShadow.offset.y * num, 1f));
				Vector2 vector = new Vector2(num4, num5);
				Color color = resolvedStyle.color;
				Color unityTextOutlineColor = resolvedStyle.unityTextOutlineColor;
				bool flag2 = num2 < 1E-30f;
				if (flag2)
				{
					unityTextOutlineColor.a = 0f;
				}
				textCoreSettings = new TextCoreSettings
				{
					faceColor = color,
					outlineColor = unityTextOutlineColor,
					outlineWidth = num2,
					underlayColor = computedStyle.textShadow.color,
					underlayOffset = vector,
					underlaySoftness = num3
				};
			}
			return textCoreSettings;
		}
	}
}
