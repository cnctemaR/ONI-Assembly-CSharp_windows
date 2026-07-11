using System;
using System.Collections.Generic;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleSheetCache
	{
		internal static void ClearCaches()
		{
			StyleSheetCache.s_EnumToIntCache.Clear();
			StyleSheetCache.s_RulePropertyIDsCache.Clear();
		}

		internal static int GetEnumValue<T>(StyleSheet sheet, StyleValueHandle handle)
		{
			Debug.Assert(handle.valueType == StyleValueType.Enum);
			StyleSheetCache.SheetHandleKey sheetHandleKey = new StyleSheetCache.SheetHandleKey(sheet, handle.valueIndex);
			int num;
			if (!StyleSheetCache.s_EnumToIntCache.TryGetValue(sheetHandleKey, out num))
			{
				string text = sheet.ReadEnum(handle).Replace("-", string.Empty);
				object obj = Enum.Parse(typeof(T), text, true);
				num = (int)obj;
				StyleSheetCache.s_EnumToIntCache.Add(sheetHandleKey, num);
			}
			Debug.Assert(Enum.GetName(typeof(T), num) != null);
			return num;
		}

		internal static StylePropertyID[] GetPropertyIDs(StyleSheet sheet, int ruleIndex)
		{
			StyleSheetCache.SheetHandleKey sheetHandleKey = new StyleSheetCache.SheetHandleKey(sheet, ruleIndex);
			StylePropertyID[] array;
			if (!StyleSheetCache.s_RulePropertyIDsCache.TryGetValue(sheetHandleKey, out array))
			{
				StyleRule styleRule = sheet.rules[ruleIndex];
				array = new StylePropertyID[styleRule.properties.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = StyleSheetCache.GetPropertyID(sheet, styleRule, i);
				}
				StyleSheetCache.s_RulePropertyIDsCache.Add(sheetHandleKey, array);
			}
			return array;
		}

		private static string MapDeprecatedPropertyName(string name, string styleSheetName, int line)
		{
			string text;
			StyleSheetCache.s_DeprecatedNames.TryGetValue(name, out text);
			return text ?? name;
		}

		private static StylePropertyID GetPropertyID(StyleSheet sheet, StyleRule rule, int index)
		{
			string text = rule.properties[index].name;
			text = StyleSheetCache.MapDeprecatedPropertyName(text, sheet.name, rule.line);
			StylePropertyID stylePropertyID;
			if (!StyleSheetCache.s_NameToIDCache.TryGetValue(text, out stylePropertyID))
			{
				stylePropertyID = StylePropertyID.Custom;
			}
			return stylePropertyID;
		}

		private static StyleSheetCache.SheetHandleKeyComparer s_Comparer = new StyleSheetCache.SheetHandleKeyComparer();

		private static Dictionary<StyleSheetCache.SheetHandleKey, int> s_EnumToIntCache = new Dictionary<StyleSheetCache.SheetHandleKey, int>(StyleSheetCache.s_Comparer);

		private static Dictionary<StyleSheetCache.SheetHandleKey, StylePropertyID[]> s_RulePropertyIDsCache = new Dictionary<StyleSheetCache.SheetHandleKey, StylePropertyID[]>(StyleSheetCache.s_Comparer);

		private static Dictionary<string, StylePropertyID> s_NameToIDCache = new Dictionary<string, StylePropertyID>
		{
			{
				"width",
				StylePropertyID.Width
			},
			{
				"height",
				StylePropertyID.Height
			},
			{
				"max-width",
				StylePropertyID.MaxWidth
			},
			{
				"max-height",
				StylePropertyID.MaxHeight
			},
			{
				"min-width",
				StylePropertyID.MinWidth
			},
			{
				"min-height",
				StylePropertyID.MinHeight
			},
			{
				"flex",
				StylePropertyID.Flex
			},
			{
				"flex-wrap",
				StylePropertyID.FlexWrap
			},
			{
				"flex-basis",
				StylePropertyID.FlexBasis
			},
			{
				"flex-grow",
				StylePropertyID.FlexGrow
			},
			{
				"flex-shrink",
				StylePropertyID.FlexShrink
			},
			{
				"overflow",
				StylePropertyID.Overflow
			},
			{
				"left",
				StylePropertyID.PositionLeft
			},
			{
				"top",
				StylePropertyID.PositionTop
			},
			{
				"right",
				StylePropertyID.PositionRight
			},
			{
				"bottom",
				StylePropertyID.PositionBottom
			},
			{
				"margin-left",
				StylePropertyID.MarginLeft
			},
			{
				"margin-top",
				StylePropertyID.MarginTop
			},
			{
				"margin-right",
				StylePropertyID.MarginRight
			},
			{
				"margin-bottom",
				StylePropertyID.MarginBottom
			},
			{
				"padding-left",
				StylePropertyID.PaddingLeft
			},
			{
				"padding-top",
				StylePropertyID.PaddingTop
			},
			{
				"padding-right",
				StylePropertyID.PaddingRight
			},
			{
				"padding-bottom",
				StylePropertyID.PaddingBottom
			},
			{
				"position",
				StylePropertyID.Position
			},
			{
				"-unity-position",
				StylePropertyID.PositionType
			},
			{
				"align-self",
				StylePropertyID.AlignSelf
			},
			{
				"-unity-text-align",
				StylePropertyID.UnityTextAlign
			},
			{
				"-unity-font-style",
				StylePropertyID.FontStyleAndWeight
			},
			{
				"-unity-clipping",
				StylePropertyID.TextClipping
			},
			{
				"-unity-font",
				StylePropertyID.Font
			},
			{
				"font-size",
				StylePropertyID.FontSize
			},
			{
				"-unity-word-wrap",
				StylePropertyID.WordWrap
			},
			{
				"color",
				StylePropertyID.Color
			},
			{
				"flex-direction",
				StylePropertyID.FlexDirection
			},
			{
				"background-color",
				StylePropertyID.BackgroundColor
			},
			{
				"border-color",
				StylePropertyID.BorderColor
			},
			{
				"background-image",
				StylePropertyID.BackgroundImage
			},
			{
				"-unity-background-scale-mode",
				StylePropertyID.BackgroundScaleMode
			},
			{
				"align-items",
				StylePropertyID.AlignItems
			},
			{
				"align-content",
				StylePropertyID.AlignContent
			},
			{
				"justify-content",
				StylePropertyID.JustifyContent
			},
			{
				"border-left-width",
				StylePropertyID.BorderLeftWidth
			},
			{
				"border-top-width",
				StylePropertyID.BorderTopWidth
			},
			{
				"border-right-width",
				StylePropertyID.BorderRightWidth
			},
			{
				"border-bottom-width",
				StylePropertyID.BorderBottomWidth
			},
			{
				"border-radius",
				StylePropertyID.BorderRadius
			},
			{
				"border-top-left-radius",
				StylePropertyID.BorderTopLeftRadius
			},
			{
				"border-top-right-radius",
				StylePropertyID.BorderTopRightRadius
			},
			{
				"border-bottom-right-radius",
				StylePropertyID.BorderBottomRightRadius
			},
			{
				"border-bottom-left-radius",
				StylePropertyID.BorderBottomLeftRadius
			},
			{
				"-unity-slice-left",
				StylePropertyID.SliceLeft
			},
			{
				"-unity-slice-top",
				StylePropertyID.SliceTop
			},
			{
				"-unity-slice-right",
				StylePropertyID.SliceRight
			},
			{
				"-unity-slice-bottom",
				StylePropertyID.SliceBottom
			},
			{
				"opacity",
				StylePropertyID.Opacity
			},
			{
				"cursor",
				StylePropertyID.Cursor
			},
			{
				"visibility",
				StylePropertyID.Visibility
			}
		};

		private static Dictionary<string, string> s_DeprecatedNames = new Dictionary<string, string>
		{
			{ "position-left", "left" },
			{ "position-top", "top" },
			{ "position-right", "right" },
			{ "position-bottom", "bottom" },
			{ "text-color", "color" },
			{ "slice-left", "-unity-slice-left" },
			{ "slice-top", "-unity-slice-top" },
			{ "slice-right", "-unity-slice-right" },
			{ "slice-bottom", "-unity-slice-bottom" },
			{ "text-alignment", "-unity-text-align" },
			{ "word-wrap", "-unity-word-wrap" },
			{ "font", "-unity-font" },
			{ "background-size", "-unity-background-scale-mode" },
			{ "font-style", "-unity-font-style" },
			{ "position-type", "-unity-position" },
			{ "text-clipping", "-unity-clipping" },
			{ "border-left", "border-left-width" },
			{ "border-top", "border-top-width" },
			{ "border-right", "border-right-width" },
			{ "border-bottom", "border-bottom-width" }
		};

		private struct SheetHandleKey
		{
			public SheetHandleKey(StyleSheet sheet, int index)
			{
				this.sheetInstanceID = sheet.GetInstanceID();
				this.index = index;
			}

			public readonly int sheetInstanceID;

			public readonly int index;
		}

		private class SheetHandleKeyComparer : IEqualityComparer<StyleSheetCache.SheetHandleKey>
		{
			public bool Equals(StyleSheetCache.SheetHandleKey x, StyleSheetCache.SheetHandleKey y)
			{
				return x.sheetInstanceID == y.sheetInstanceID && x.index == y.index;
			}

			public int GetHashCode(StyleSheetCache.SheetHandleKey key)
			{
				return key.sheetInstanceID.GetHashCode() ^ key.index.GetHashCode();
			}
		}
	}
}
