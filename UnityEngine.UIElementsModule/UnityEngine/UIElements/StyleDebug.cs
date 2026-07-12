using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal static class StyleDebug
	{
		public static string[] GetStylePropertyNames()
		{
			List<string> list = StylePropertyUtil.s_NameToId.Keys.ToList<string>();
			list.Sort();
			return list.ToArray();
		}

		public static string[] GetLonghandPropertyNames(string shorthandName)
		{
			StylePropertyId stylePropertyId;
			bool flag = StylePropertyUtil.s_NameToId.TryGetValue(shorthandName, out stylePropertyId);
			if (flag)
			{
				bool flag2 = StyleDebug.IsShorthandProperty(stylePropertyId);
				if (flag2)
				{
					return StyleDebug.GetLonghandPropertyNames(stylePropertyId);
				}
			}
			return null;
		}

		public static StylePropertyId GetStylePropertyIdFromName(string name)
		{
			StylePropertyId stylePropertyId;
			bool flag = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
			StylePropertyId stylePropertyId2;
			if (flag)
			{
				stylePropertyId2 = stylePropertyId;
			}
			else
			{
				stylePropertyId2 = StylePropertyId.Unknown;
			}
			return stylePropertyId2;
		}

		public static object GetComputedStyleValue(ComputedStyle computedStyle, string name)
		{
			StylePropertyId stylePropertyId;
			bool flag = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
			object obj;
			if (flag)
			{
				obj = StyleDebug.GetComputedStyleValue(computedStyle, stylePropertyId);
			}
			else
			{
				obj = null;
			}
			return obj;
		}

		public static object GetInlineStyleValue(IStyle style, string name)
		{
			StylePropertyId stylePropertyId;
			bool flag = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
			object obj;
			if (flag)
			{
				obj = StyleDebug.GetInlineStyleValue(style, stylePropertyId);
			}
			else
			{
				obj = null;
			}
			return obj;
		}

		public static void SetInlineStyleValue(IStyle style, string name, object value)
		{
			StylePropertyId stylePropertyId;
			bool flag = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
			if (flag)
			{
				StyleDebug.SetInlineStyleValue(style, stylePropertyId, value);
			}
		}

		public static Type GetComputedStyleType(string name)
		{
			StylePropertyId stylePropertyId;
			bool flag = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
			if (flag)
			{
				bool flag2 = !StyleDebug.IsShorthandProperty(stylePropertyId);
				if (flag2)
				{
					return StyleDebug.GetComputedStyleType(stylePropertyId);
				}
			}
			return null;
		}

		public static void FindSpecifiedStyles(ComputedStyle computedStyle, IEnumerable<SelectorMatchRecord> matchRecords, Dictionary<StylePropertyId, int> result)
		{
			result.Clear();
			bool flag = computedStyle == null;
			if (!flag)
			{
				foreach (SelectorMatchRecord selectorMatchRecord in matchRecords)
				{
					int num = selectorMatchRecord.complexSelector.specificity;
					bool isUnityStyleSheet = selectorMatchRecord.sheet.isUnityStyleSheet;
					if (isUnityStyleSheet)
					{
						num = -1;
					}
					StyleProperty[] properties = selectorMatchRecord.complexSelector.rule.properties;
					foreach (StyleProperty styleProperty in properties)
					{
						StylePropertyId stylePropertyId;
						bool flag2 = StylePropertyUtil.s_NameToId.TryGetValue(styleProperty.name, out stylePropertyId);
						if (flag2)
						{
							bool flag3 = StyleDebug.IsShorthandProperty(stylePropertyId);
							if (flag3)
							{
								string[] longhandPropertyNames = StyleDebug.GetLonghandPropertyNames(stylePropertyId);
								foreach (string text in longhandPropertyNames)
								{
									StylePropertyId stylePropertyIdFromName = StyleDebug.GetStylePropertyIdFromName(text);
									result[stylePropertyIdFromName] = num;
								}
							}
							else
							{
								result[stylePropertyId] = num;
							}
						}
					}
				}
				StylePropertyId[] inheritedProperties = StyleDebug.GetInheritedProperties();
				foreach (StylePropertyId stylePropertyId2 in inheritedProperties)
				{
					bool flag4 = result.ContainsKey(stylePropertyId2);
					if (!flag4)
					{
						object computedStyleValue = StyleDebug.GetComputedStyleValue(computedStyle, stylePropertyId2);
						object computedStyleValue2 = StyleDebug.GetComputedStyleValue(InitialStyle.Get(), stylePropertyId2);
						bool flag5 = !computedStyleValue.Equals(computedStyleValue2);
						if (flag5)
						{
							result[stylePropertyId2] = 2147483646;
						}
					}
				}
			}
		}

		public static object GetComputedStyleValue(ComputedStyle computedStyle, StylePropertyId id)
		{
			object obj;
			switch (id)
			{
			case StylePropertyId.Color:
				obj = computedStyle.color;
				break;
			case StylePropertyId.FontSize:
				obj = computedStyle.fontSize;
				break;
			case StylePropertyId.UnityFont:
				obj = computedStyle.unityFont;
				break;
			case StylePropertyId.UnityFontStyleAndWeight:
				obj = computedStyle.unityFontStyleAndWeight;
				break;
			case StylePropertyId.UnityTextAlign:
				obj = computedStyle.unityTextAlign;
				break;
			case StylePropertyId.Visibility:
				obj = computedStyle.visibility;
				break;
			case StylePropertyId.WhiteSpace:
				obj = computedStyle.whiteSpace;
				break;
			default:
				switch (id)
				{
				case StylePropertyId.AlignContent:
					obj = computedStyle.alignContent;
					break;
				case StylePropertyId.AlignItems:
					obj = computedStyle.alignItems;
					break;
				case StylePropertyId.AlignSelf:
					obj = computedStyle.alignSelf;
					break;
				case StylePropertyId.BackgroundColor:
					obj = computedStyle.backgroundColor;
					break;
				case StylePropertyId.BackgroundImage:
					obj = computedStyle.backgroundImage;
					break;
				case StylePropertyId.BorderBottomColor:
					obj = computedStyle.borderBottomColor;
					break;
				case StylePropertyId.BorderBottomLeftRadius:
					obj = computedStyle.borderBottomLeftRadius;
					break;
				case StylePropertyId.BorderBottomRightRadius:
					obj = computedStyle.borderBottomRightRadius;
					break;
				case StylePropertyId.BorderBottomWidth:
					obj = computedStyle.borderBottomWidth;
					break;
				case StylePropertyId.BorderLeftColor:
					obj = computedStyle.borderLeftColor;
					break;
				case StylePropertyId.BorderLeftWidth:
					obj = computedStyle.borderLeftWidth;
					break;
				case StylePropertyId.BorderRightColor:
					obj = computedStyle.borderRightColor;
					break;
				case StylePropertyId.BorderRightWidth:
					obj = computedStyle.borderRightWidth;
					break;
				case StylePropertyId.BorderTopColor:
					obj = computedStyle.borderTopColor;
					break;
				case StylePropertyId.BorderTopLeftRadius:
					obj = computedStyle.borderTopLeftRadius;
					break;
				case StylePropertyId.BorderTopRightRadius:
					obj = computedStyle.borderTopRightRadius;
					break;
				case StylePropertyId.BorderTopWidth:
					obj = computedStyle.borderTopWidth;
					break;
				case StylePropertyId.Bottom:
					obj = computedStyle.bottom;
					break;
				case StylePropertyId.Cursor:
					obj = computedStyle.cursor;
					break;
				case StylePropertyId.Display:
					obj = computedStyle.display;
					break;
				case StylePropertyId.FlexBasis:
					obj = computedStyle.flexBasis;
					break;
				case StylePropertyId.FlexDirection:
					obj = computedStyle.flexDirection;
					break;
				case StylePropertyId.FlexGrow:
					obj = computedStyle.flexGrow;
					break;
				case StylePropertyId.FlexShrink:
					obj = computedStyle.flexShrink;
					break;
				case StylePropertyId.FlexWrap:
					obj = computedStyle.flexWrap;
					break;
				case StylePropertyId.Height:
					obj = computedStyle.height;
					break;
				case StylePropertyId.JustifyContent:
					obj = computedStyle.justifyContent;
					break;
				case StylePropertyId.Left:
					obj = computedStyle.left;
					break;
				case StylePropertyId.MarginBottom:
					obj = computedStyle.marginBottom;
					break;
				case StylePropertyId.MarginLeft:
					obj = computedStyle.marginLeft;
					break;
				case StylePropertyId.MarginRight:
					obj = computedStyle.marginRight;
					break;
				case StylePropertyId.MarginTop:
					obj = computedStyle.marginTop;
					break;
				case StylePropertyId.MaxHeight:
					obj = computedStyle.maxHeight;
					break;
				case StylePropertyId.MaxWidth:
					obj = computedStyle.maxWidth;
					break;
				case StylePropertyId.MinHeight:
					obj = computedStyle.minHeight;
					break;
				case StylePropertyId.MinWidth:
					obj = computedStyle.minWidth;
					break;
				case StylePropertyId.Opacity:
					obj = computedStyle.opacity;
					break;
				case StylePropertyId.Overflow:
					obj = computedStyle.overflow;
					break;
				case StylePropertyId.PaddingBottom:
					obj = computedStyle.paddingBottom;
					break;
				case StylePropertyId.PaddingLeft:
					obj = computedStyle.paddingLeft;
					break;
				case StylePropertyId.PaddingRight:
					obj = computedStyle.paddingRight;
					break;
				case StylePropertyId.PaddingTop:
					obj = computedStyle.paddingTop;
					break;
				case StylePropertyId.Position:
					obj = computedStyle.position;
					break;
				case StylePropertyId.Right:
					obj = computedStyle.right;
					break;
				case StylePropertyId.TextOverflow:
					obj = computedStyle.textOverflow;
					break;
				case StylePropertyId.Top:
					obj = computedStyle.top;
					break;
				case StylePropertyId.UnityBackgroundImageTintColor:
					obj = computedStyle.unityBackgroundImageTintColor;
					break;
				case StylePropertyId.UnityBackgroundScaleMode:
					obj = computedStyle.unityBackgroundScaleMode;
					break;
				case StylePropertyId.UnityOverflowClipBox:
					obj = computedStyle.unityOverflowClipBox;
					break;
				case StylePropertyId.UnitySliceBottom:
					obj = computedStyle.unitySliceBottom;
					break;
				case StylePropertyId.UnitySliceLeft:
					obj = computedStyle.unitySliceLeft;
					break;
				case StylePropertyId.UnitySliceRight:
					obj = computedStyle.unitySliceRight;
					break;
				case StylePropertyId.UnitySliceTop:
					obj = computedStyle.unitySliceTop;
					break;
				case StylePropertyId.UnityTextOverflowPosition:
					obj = computedStyle.unityTextOverflowPosition;
					break;
				case StylePropertyId.Width:
					obj = computedStyle.width;
					break;
				default:
					Debug.LogAssertion(string.Format("Cannot get computed style value for property id {0}", id));
					obj = null;
					break;
				}
				break;
			}
			return obj;
		}

		public static object GetInlineStyleValue(IStyle style, StylePropertyId id)
		{
			object obj;
			switch (id)
			{
			case StylePropertyId.Color:
				obj = style.color;
				break;
			case StylePropertyId.FontSize:
				obj = style.fontSize;
				break;
			case StylePropertyId.UnityFont:
				obj = style.unityFont;
				break;
			case StylePropertyId.UnityFontStyleAndWeight:
				obj = style.unityFontStyleAndWeight;
				break;
			case StylePropertyId.UnityTextAlign:
				obj = style.unityTextAlign;
				break;
			case StylePropertyId.Visibility:
				obj = style.visibility;
				break;
			case StylePropertyId.WhiteSpace:
				obj = style.whiteSpace;
				break;
			default:
				switch (id)
				{
				case StylePropertyId.AlignContent:
					obj = style.alignContent;
					break;
				case StylePropertyId.AlignItems:
					obj = style.alignItems;
					break;
				case StylePropertyId.AlignSelf:
					obj = style.alignSelf;
					break;
				case StylePropertyId.BackgroundColor:
					obj = style.backgroundColor;
					break;
				case StylePropertyId.BackgroundImage:
					obj = style.backgroundImage;
					break;
				case StylePropertyId.BorderBottomColor:
					obj = style.borderBottomColor;
					break;
				case StylePropertyId.BorderBottomLeftRadius:
					obj = style.borderBottomLeftRadius;
					break;
				case StylePropertyId.BorderBottomRightRadius:
					obj = style.borderBottomRightRadius;
					break;
				case StylePropertyId.BorderBottomWidth:
					obj = style.borderBottomWidth;
					break;
				case StylePropertyId.BorderLeftColor:
					obj = style.borderLeftColor;
					break;
				case StylePropertyId.BorderLeftWidth:
					obj = style.borderLeftWidth;
					break;
				case StylePropertyId.BorderRightColor:
					obj = style.borderRightColor;
					break;
				case StylePropertyId.BorderRightWidth:
					obj = style.borderRightWidth;
					break;
				case StylePropertyId.BorderTopColor:
					obj = style.borderTopColor;
					break;
				case StylePropertyId.BorderTopLeftRadius:
					obj = style.borderTopLeftRadius;
					break;
				case StylePropertyId.BorderTopRightRadius:
					obj = style.borderTopRightRadius;
					break;
				case StylePropertyId.BorderTopWidth:
					obj = style.borderTopWidth;
					break;
				case StylePropertyId.Bottom:
					obj = style.bottom;
					break;
				case StylePropertyId.Cursor:
					obj = style.cursor;
					break;
				case StylePropertyId.Display:
					obj = style.display;
					break;
				case StylePropertyId.FlexBasis:
					obj = style.flexBasis;
					break;
				case StylePropertyId.FlexDirection:
					obj = style.flexDirection;
					break;
				case StylePropertyId.FlexGrow:
					obj = style.flexGrow;
					break;
				case StylePropertyId.FlexShrink:
					obj = style.flexShrink;
					break;
				case StylePropertyId.FlexWrap:
					obj = style.flexWrap;
					break;
				case StylePropertyId.Height:
					obj = style.height;
					break;
				case StylePropertyId.JustifyContent:
					obj = style.justifyContent;
					break;
				case StylePropertyId.Left:
					obj = style.left;
					break;
				case StylePropertyId.MarginBottom:
					obj = style.marginBottom;
					break;
				case StylePropertyId.MarginLeft:
					obj = style.marginLeft;
					break;
				case StylePropertyId.MarginRight:
					obj = style.marginRight;
					break;
				case StylePropertyId.MarginTop:
					obj = style.marginTop;
					break;
				case StylePropertyId.MaxHeight:
					obj = style.maxHeight;
					break;
				case StylePropertyId.MaxWidth:
					obj = style.maxWidth;
					break;
				case StylePropertyId.MinHeight:
					obj = style.minHeight;
					break;
				case StylePropertyId.MinWidth:
					obj = style.minWidth;
					break;
				case StylePropertyId.Opacity:
					obj = style.opacity;
					break;
				case StylePropertyId.Overflow:
					obj = style.overflow;
					break;
				case StylePropertyId.PaddingBottom:
					obj = style.paddingBottom;
					break;
				case StylePropertyId.PaddingLeft:
					obj = style.paddingLeft;
					break;
				case StylePropertyId.PaddingRight:
					obj = style.paddingRight;
					break;
				case StylePropertyId.PaddingTop:
					obj = style.paddingTop;
					break;
				case StylePropertyId.Position:
					obj = style.position;
					break;
				case StylePropertyId.Right:
					obj = style.right;
					break;
				case StylePropertyId.TextOverflow:
					obj = style.textOverflow;
					break;
				case StylePropertyId.Top:
					obj = style.top;
					break;
				case StylePropertyId.UnityBackgroundImageTintColor:
					obj = style.unityBackgroundImageTintColor;
					break;
				case StylePropertyId.UnityBackgroundScaleMode:
					obj = style.unityBackgroundScaleMode;
					break;
				case StylePropertyId.UnityOverflowClipBox:
					obj = style.unityOverflowClipBox;
					break;
				case StylePropertyId.UnitySliceBottom:
					obj = style.unitySliceBottom;
					break;
				case StylePropertyId.UnitySliceLeft:
					obj = style.unitySliceLeft;
					break;
				case StylePropertyId.UnitySliceRight:
					obj = style.unitySliceRight;
					break;
				case StylePropertyId.UnitySliceTop:
					obj = style.unitySliceTop;
					break;
				case StylePropertyId.UnityTextOverflowPosition:
					obj = style.unityTextOverflowPosition;
					break;
				case StylePropertyId.Width:
					obj = style.width;
					break;
				default:
					Debug.LogAssertion(string.Format("Cannot get inline style value for property id {0}", id));
					obj = null;
					break;
				}
				break;
			}
			return obj;
		}

		public static void SetInlineStyleValue(IStyle style, StylePropertyId id, object value)
		{
			switch (id)
			{
			case StylePropertyId.Color:
				style.color = (StyleColor)value;
				break;
			case StylePropertyId.FontSize:
				style.fontSize = (StyleLength)value;
				break;
			case StylePropertyId.UnityFont:
				style.unityFont = (StyleFont)value;
				break;
			case StylePropertyId.UnityFontStyleAndWeight:
				style.unityFontStyleAndWeight = (StyleEnum<FontStyle>)value;
				break;
			case StylePropertyId.UnityTextAlign:
				style.unityTextAlign = (StyleEnum<TextAnchor>)value;
				break;
			case StylePropertyId.Visibility:
				style.visibility = (StyleEnum<Visibility>)value;
				break;
			case StylePropertyId.WhiteSpace:
				style.whiteSpace = (StyleEnum<WhiteSpace>)value;
				break;
			default:
				switch (id)
				{
				case StylePropertyId.AlignContent:
					style.alignContent = (StyleEnum<Align>)value;
					break;
				case StylePropertyId.AlignItems:
					style.alignItems = (StyleEnum<Align>)value;
					break;
				case StylePropertyId.AlignSelf:
					style.alignSelf = (StyleEnum<Align>)value;
					break;
				case StylePropertyId.BackgroundColor:
					style.backgroundColor = (StyleColor)value;
					break;
				case StylePropertyId.BackgroundImage:
					style.backgroundImage = (StyleBackground)value;
					break;
				case StylePropertyId.BorderBottomColor:
					style.borderBottomColor = (StyleColor)value;
					break;
				case StylePropertyId.BorderBottomLeftRadius:
					style.borderBottomLeftRadius = (StyleLength)value;
					break;
				case StylePropertyId.BorderBottomRightRadius:
					style.borderBottomRightRadius = (StyleLength)value;
					break;
				case StylePropertyId.BorderBottomWidth:
					style.borderBottomWidth = (StyleFloat)value;
					break;
				case StylePropertyId.BorderLeftColor:
					style.borderLeftColor = (StyleColor)value;
					break;
				case StylePropertyId.BorderLeftWidth:
					style.borderLeftWidth = (StyleFloat)value;
					break;
				case StylePropertyId.BorderRightColor:
					style.borderRightColor = (StyleColor)value;
					break;
				case StylePropertyId.BorderRightWidth:
					style.borderRightWidth = (StyleFloat)value;
					break;
				case StylePropertyId.BorderTopColor:
					style.borderTopColor = (StyleColor)value;
					break;
				case StylePropertyId.BorderTopLeftRadius:
					style.borderTopLeftRadius = (StyleLength)value;
					break;
				case StylePropertyId.BorderTopRightRadius:
					style.borderTopRightRadius = (StyleLength)value;
					break;
				case StylePropertyId.BorderTopWidth:
					style.borderTopWidth = (StyleFloat)value;
					break;
				case StylePropertyId.Bottom:
					style.bottom = (StyleLength)value;
					break;
				case StylePropertyId.Cursor:
					style.cursor = (StyleCursor)value;
					break;
				case StylePropertyId.Display:
					style.display = (StyleEnum<DisplayStyle>)value;
					break;
				case StylePropertyId.FlexBasis:
					style.flexBasis = (StyleLength)value;
					break;
				case StylePropertyId.FlexDirection:
					style.flexDirection = (StyleEnum<FlexDirection>)value;
					break;
				case StylePropertyId.FlexGrow:
					style.flexGrow = (StyleFloat)value;
					break;
				case StylePropertyId.FlexShrink:
					style.flexShrink = (StyleFloat)value;
					break;
				case StylePropertyId.FlexWrap:
					style.flexWrap = (StyleEnum<Wrap>)value;
					break;
				case StylePropertyId.Height:
					style.height = (StyleLength)value;
					break;
				case StylePropertyId.JustifyContent:
					style.justifyContent = (StyleEnum<Justify>)value;
					break;
				case StylePropertyId.Left:
					style.left = (StyleLength)value;
					break;
				case StylePropertyId.MarginBottom:
					style.marginBottom = (StyleLength)value;
					break;
				case StylePropertyId.MarginLeft:
					style.marginLeft = (StyleLength)value;
					break;
				case StylePropertyId.MarginRight:
					style.marginRight = (StyleLength)value;
					break;
				case StylePropertyId.MarginTop:
					style.marginTop = (StyleLength)value;
					break;
				case StylePropertyId.MaxHeight:
					style.maxHeight = (StyleLength)value;
					break;
				case StylePropertyId.MaxWidth:
					style.maxWidth = (StyleLength)value;
					break;
				case StylePropertyId.MinHeight:
					style.minHeight = (StyleLength)value;
					break;
				case StylePropertyId.MinWidth:
					style.minWidth = (StyleLength)value;
					break;
				case StylePropertyId.Opacity:
					style.opacity = (StyleFloat)value;
					break;
				case StylePropertyId.Overflow:
					style.overflow = (StyleEnum<Overflow>)value;
					break;
				case StylePropertyId.PaddingBottom:
					style.paddingBottom = (StyleLength)value;
					break;
				case StylePropertyId.PaddingLeft:
					style.paddingLeft = (StyleLength)value;
					break;
				case StylePropertyId.PaddingRight:
					style.paddingRight = (StyleLength)value;
					break;
				case StylePropertyId.PaddingTop:
					style.paddingTop = (StyleLength)value;
					break;
				case StylePropertyId.Position:
					style.position = (StyleEnum<Position>)value;
					break;
				case StylePropertyId.Right:
					style.right = (StyleLength)value;
					break;
				case StylePropertyId.TextOverflow:
					style.textOverflow = (StyleEnum<TextOverflow>)value;
					break;
				case StylePropertyId.Top:
					style.top = (StyleLength)value;
					break;
				case StylePropertyId.UnityBackgroundImageTintColor:
					style.unityBackgroundImageTintColor = (StyleColor)value;
					break;
				case StylePropertyId.UnityBackgroundScaleMode:
					style.unityBackgroundScaleMode = (StyleEnum<ScaleMode>)value;
					break;
				case StylePropertyId.UnityOverflowClipBox:
					style.unityOverflowClipBox = (StyleEnum<OverflowClipBox>)value;
					break;
				case StylePropertyId.UnitySliceBottom:
					style.unitySliceBottom = (StyleInt)value;
					break;
				case StylePropertyId.UnitySliceLeft:
					style.unitySliceLeft = (StyleInt)value;
					break;
				case StylePropertyId.UnitySliceRight:
					style.unitySliceRight = (StyleInt)value;
					break;
				case StylePropertyId.UnitySliceTop:
					style.unitySliceTop = (StyleInt)value;
					break;
				case StylePropertyId.UnityTextOverflowPosition:
					style.unityTextOverflowPosition = (StyleEnum<TextOverflowPosition>)value;
					break;
				case StylePropertyId.Width:
					style.width = (StyleLength)value;
					break;
				default:
					Debug.LogAssertion(string.Format("Cannot set inline style value for property id {0}", id));
					break;
				}
				break;
			}
		}

		public static Type GetComputedStyleType(StylePropertyId id)
		{
			Type type;
			switch (id)
			{
			case StylePropertyId.Color:
				type = typeof(StyleColor);
				break;
			case StylePropertyId.FontSize:
				type = typeof(StyleLength);
				break;
			case StylePropertyId.UnityFont:
				type = typeof(StyleFont);
				break;
			case StylePropertyId.UnityFontStyleAndWeight:
				type = typeof(StyleEnum<FontStyle>);
				break;
			case StylePropertyId.UnityTextAlign:
				type = typeof(StyleEnum<TextAnchor>);
				break;
			case StylePropertyId.Visibility:
				type = typeof(StyleEnum<Visibility>);
				break;
			case StylePropertyId.WhiteSpace:
				type = typeof(StyleEnum<WhiteSpace>);
				break;
			default:
				switch (id)
				{
				case StylePropertyId.AlignContent:
					type = typeof(StyleEnum<Align>);
					break;
				case StylePropertyId.AlignItems:
					type = typeof(StyleEnum<Align>);
					break;
				case StylePropertyId.AlignSelf:
					type = typeof(StyleEnum<Align>);
					break;
				case StylePropertyId.BackgroundColor:
					type = typeof(StyleColor);
					break;
				case StylePropertyId.BackgroundImage:
					type = typeof(StyleBackground);
					break;
				case StylePropertyId.BorderBottomColor:
					type = typeof(StyleColor);
					break;
				case StylePropertyId.BorderBottomLeftRadius:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.BorderBottomRightRadius:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.BorderBottomWidth:
					type = typeof(StyleFloat);
					break;
				case StylePropertyId.BorderLeftColor:
					type = typeof(StyleColor);
					break;
				case StylePropertyId.BorderLeftWidth:
					type = typeof(StyleFloat);
					break;
				case StylePropertyId.BorderRightColor:
					type = typeof(StyleColor);
					break;
				case StylePropertyId.BorderRightWidth:
					type = typeof(StyleFloat);
					break;
				case StylePropertyId.BorderTopColor:
					type = typeof(StyleColor);
					break;
				case StylePropertyId.BorderTopLeftRadius:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.BorderTopRightRadius:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.BorderTopWidth:
					type = typeof(StyleFloat);
					break;
				case StylePropertyId.Bottom:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.Cursor:
					type = typeof(StyleCursor);
					break;
				case StylePropertyId.Display:
					type = typeof(StyleEnum<DisplayStyle>);
					break;
				case StylePropertyId.FlexBasis:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.FlexDirection:
					type = typeof(StyleEnum<FlexDirection>);
					break;
				case StylePropertyId.FlexGrow:
					type = typeof(StyleFloat);
					break;
				case StylePropertyId.FlexShrink:
					type = typeof(StyleFloat);
					break;
				case StylePropertyId.FlexWrap:
					type = typeof(StyleEnum<Wrap>);
					break;
				case StylePropertyId.Height:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.JustifyContent:
					type = typeof(StyleEnum<Justify>);
					break;
				case StylePropertyId.Left:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MarginBottom:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MarginLeft:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MarginRight:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MarginTop:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MaxHeight:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MaxWidth:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MinHeight:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.MinWidth:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.Opacity:
					type = typeof(StyleFloat);
					break;
				case StylePropertyId.Overflow:
					type = typeof(StyleEnum<Overflow>);
					break;
				case StylePropertyId.PaddingBottom:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.PaddingLeft:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.PaddingRight:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.PaddingTop:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.Position:
					type = typeof(StyleEnum<Position>);
					break;
				case StylePropertyId.Right:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.TextOverflow:
					type = typeof(StyleEnum<TextOverflow>);
					break;
				case StylePropertyId.Top:
					type = typeof(StyleLength);
					break;
				case StylePropertyId.UnityBackgroundImageTintColor:
					type = typeof(StyleColor);
					break;
				case StylePropertyId.UnityBackgroundScaleMode:
					type = typeof(StyleEnum<ScaleMode>);
					break;
				case StylePropertyId.UnityOverflowClipBox:
					type = typeof(StyleEnum<OverflowClipBox>);
					break;
				case StylePropertyId.UnitySliceBottom:
					type = typeof(StyleInt);
					break;
				case StylePropertyId.UnitySliceLeft:
					type = typeof(StyleInt);
					break;
				case StylePropertyId.UnitySliceRight:
					type = typeof(StyleInt);
					break;
				case StylePropertyId.UnitySliceTop:
					type = typeof(StyleInt);
					break;
				case StylePropertyId.UnityTextOverflowPosition:
					type = typeof(StyleEnum<TextOverflowPosition>);
					break;
				case StylePropertyId.Width:
					type = typeof(StyleLength);
					break;
				default:
					Debug.LogAssertion(string.Format("Cannot get computed style type for property id {0}", id));
					type = null;
					break;
				}
				break;
			}
			return type;
		}

		public static string[] GetLonghandPropertyNames(StylePropertyId id)
		{
			string[] array;
			switch (id)
			{
			case StylePropertyId.BorderColor:
				array = new string[] { "border-top-color", "border-right-color", "border-bottom-color", "border-left-color" };
				break;
			case StylePropertyId.BorderRadius:
				array = new string[] { "border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius" };
				break;
			case StylePropertyId.BorderWidth:
				array = new string[] { "border-top-width", "border-right-width", "border-bottom-width", "border-left-width" };
				break;
			case StylePropertyId.Flex:
				array = new string[] { "flex-grow", "flex-shrink", "flex-basis" };
				break;
			case StylePropertyId.Margin:
				array = new string[] { "margin-top", "margin-right", "margin-bottom", "margin-left" };
				break;
			case StylePropertyId.Padding:
				array = new string[] { "padding-top", "padding-right", "padding-bottom", "padding-left" };
				break;
			default:
				Debug.LogAssertion(string.Format("Cannot get longhand property names for property id {0}", id));
				array = null;
				break;
			}
			return array;
		}

		public static bool IsShorthandProperty(StylePropertyId id)
		{
			bool flag;
			switch (id)
			{
			case StylePropertyId.BorderColor:
				flag = true;
				break;
			case StylePropertyId.BorderRadius:
				flag = true;
				break;
			case StylePropertyId.BorderWidth:
				flag = true;
				break;
			case StylePropertyId.Flex:
				flag = true;
				break;
			case StylePropertyId.Margin:
				flag = true;
				break;
			case StylePropertyId.Padding:
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			return flag;
		}

		public static bool IsInheritedProperty(StylePropertyId id)
		{
			bool flag;
			switch (id)
			{
			case StylePropertyId.Color:
				flag = true;
				break;
			case StylePropertyId.FontSize:
				flag = true;
				break;
			case StylePropertyId.UnityFont:
				flag = true;
				break;
			case StylePropertyId.UnityFontStyleAndWeight:
				flag = true;
				break;
			case StylePropertyId.UnityTextAlign:
				flag = true;
				break;
			case StylePropertyId.Visibility:
				flag = true;
				break;
			case StylePropertyId.WhiteSpace:
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			return flag;
		}

		public static StylePropertyId[] GetInheritedProperties()
		{
			return new StylePropertyId[]
			{
				StylePropertyId.Color,
				StylePropertyId.FontSize,
				StylePropertyId.UnityFont,
				StylePropertyId.UnityFontStyleAndWeight,
				StylePropertyId.UnityTextAlign,
				StylePropertyId.Visibility,
				StylePropertyId.WhiteSpace
			};
		}

		internal const int UnitySpecificity = -1;

		internal const int UndefinedSpecificity = 0;

		internal const int InheritedSpecificity = 2147483646;

		internal const int InlineSpecificity = 2147483647;
	}
}
