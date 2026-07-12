using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal static class StyleDebug
	{
		public static object GetComputedStyleValue(in ComputedStyle computedStyle, StylePropertyId id)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.color;
				}
				case StylePropertyId.FontSize:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.fontSize;
				}
				case StylePropertyId.LetterSpacing:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.letterSpacing;
				}
				case StylePropertyId.TextShadow:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.textShadow;
				}
				case StylePropertyId.UnityFont:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.unityFont;
				}
				case StylePropertyId.UnityFontDefinition:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.unityFontDefinition;
				}
				case StylePropertyId.UnityFontStyleAndWeight:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.unityFontStyleAndWeight;
				}
				case StylePropertyId.UnityParagraphSpacing:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.unityParagraphSpacing;
				}
				case StylePropertyId.UnityTextAlign:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.unityTextAlign;
				}
				case StylePropertyId.UnityTextOutlineColor:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.unityTextOutlineColor;
				}
				case StylePropertyId.UnityTextOutlineWidth:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.unityTextOutlineWidth;
				}
				case StylePropertyId.Visibility:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.visibility;
				}
				case StylePropertyId.WhiteSpace:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.whiteSpace;
				}
				case StylePropertyId.WordSpacing:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.wordSpacing;
				}
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.alignContent;
					}
					case StylePropertyId.AlignItems:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.alignItems;
					}
					case StylePropertyId.AlignSelf:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.alignSelf;
					}
					case StylePropertyId.BorderBottomWidth:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.borderBottomWidth;
					}
					case StylePropertyId.BorderLeftWidth:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.borderLeftWidth;
					}
					case StylePropertyId.BorderRightWidth:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.borderRightWidth;
					}
					case StylePropertyId.BorderTopWidth:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.borderTopWidth;
					}
					case StylePropertyId.Bottom:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.bottom;
					}
					case StylePropertyId.Display:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.display;
					}
					case StylePropertyId.FlexBasis:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.flexBasis;
					}
					case StylePropertyId.FlexDirection:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.flexDirection;
					}
					case StylePropertyId.FlexGrow:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.flexGrow;
					}
					case StylePropertyId.FlexShrink:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.flexShrink;
					}
					case StylePropertyId.FlexWrap:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.flexWrap;
					}
					case StylePropertyId.Height:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.height;
					}
					case StylePropertyId.JustifyContent:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.justifyContent;
					}
					case StylePropertyId.Left:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.left;
					}
					case StylePropertyId.MarginBottom:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.marginBottom;
					}
					case StylePropertyId.MarginLeft:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.marginLeft;
					}
					case StylePropertyId.MarginRight:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.marginRight;
					}
					case StylePropertyId.MarginTop:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.marginTop;
					}
					case StylePropertyId.MaxHeight:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.maxHeight;
					}
					case StylePropertyId.MaxWidth:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.maxWidth;
					}
					case StylePropertyId.MinHeight:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.minHeight;
					}
					case StylePropertyId.MinWidth:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.minWidth;
					}
					case StylePropertyId.PaddingBottom:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.paddingBottom;
					}
					case StylePropertyId.PaddingLeft:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.paddingLeft;
					}
					case StylePropertyId.PaddingRight:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.paddingRight;
					}
					case StylePropertyId.PaddingTop:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.paddingTop;
					}
					case StylePropertyId.Position:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.position;
					}
					case StylePropertyId.Right:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.right;
					}
					case StylePropertyId.Top:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.top;
					}
					case StylePropertyId.Width:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.width;
					}
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.cursor;
						}
						case StylePropertyId.TextOverflow:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.textOverflow;
						}
						case StylePropertyId.UnityBackgroundImageTintColor:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unityBackgroundImageTintColor;
						}
						case StylePropertyId.UnityOverflowClipBox:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unityOverflowClipBox;
						}
						case StylePropertyId.UnitySliceBottom:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unitySliceBottom;
						}
						case StylePropertyId.UnitySliceLeft:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unitySliceLeft;
						}
						case StylePropertyId.UnitySliceRight:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unitySliceRight;
						}
						case StylePropertyId.UnitySliceScale:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unitySliceScale;
						}
						case StylePropertyId.UnitySliceTop:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unitySliceTop;
						}
						case StylePropertyId.UnityTextOverflowPosition:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.unityTextOverflowPosition;
						}
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.rotate;
				}
				case StylePropertyId.Scale:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.scale;
				}
				case StylePropertyId.TransformOrigin:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.transformOrigin;
				}
				case StylePropertyId.Translate:
				{
					ComputedStyle computedStyle2 = computedStyle;
					return computedStyle2.translate;
				}
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.transitionDelay;
					}
					case StylePropertyId.TransitionDuration:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.transitionDuration;
					}
					case StylePropertyId.TransitionProperty:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.transitionProperty;
					}
					case StylePropertyId.TransitionTimingFunction:
					{
						ComputedStyle computedStyle2 = computedStyle;
						return computedStyle2.transitionTimingFunction;
					}
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.backgroundColor;
						}
						case StylePropertyId.BackgroundImage:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.backgroundImage;
						}
						case StylePropertyId.BackgroundPositionX:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.backgroundPositionX;
						}
						case StylePropertyId.BackgroundPositionY:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.backgroundPositionY;
						}
						case StylePropertyId.BackgroundRepeat:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.backgroundRepeat;
						}
						case StylePropertyId.BackgroundSize:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.backgroundSize;
						}
						case StylePropertyId.BorderBottomColor:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderBottomColor;
						}
						case StylePropertyId.BorderBottomLeftRadius:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderBottomLeftRadius;
						}
						case StylePropertyId.BorderBottomRightRadius:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderBottomRightRadius;
						}
						case StylePropertyId.BorderLeftColor:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderLeftColor;
						}
						case StylePropertyId.BorderRightColor:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderRightColor;
						}
						case StylePropertyId.BorderTopColor:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderTopColor;
						}
						case StylePropertyId.BorderTopLeftRadius:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderTopLeftRadius;
						}
						case StylePropertyId.BorderTopRightRadius:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.borderTopRightRadius;
						}
						case StylePropertyId.Opacity:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.opacity;
						}
						case StylePropertyId.Overflow:
						{
							ComputedStyle computedStyle2 = computedStyle;
							return computedStyle2.overflow;
						}
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot get computed style value for property id {0}", id));
			return null;
		}

		public static Type GetComputedStyleType(StylePropertyId id)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
					return typeof(Color);
				case StylePropertyId.FontSize:
					return typeof(Length);
				case StylePropertyId.LetterSpacing:
					return typeof(Length);
				case StylePropertyId.TextShadow:
					return typeof(TextShadow);
				case StylePropertyId.UnityFont:
					return typeof(Font);
				case StylePropertyId.UnityFontDefinition:
					return typeof(FontDefinition);
				case StylePropertyId.UnityFontStyleAndWeight:
					return typeof(FontStyle);
				case StylePropertyId.UnityParagraphSpacing:
					return typeof(Length);
				case StylePropertyId.UnityTextAlign:
					return typeof(TextAnchor);
				case StylePropertyId.UnityTextOutlineColor:
					return typeof(Color);
				case StylePropertyId.UnityTextOutlineWidth:
					return typeof(float);
				case StylePropertyId.Visibility:
					return typeof(Visibility);
				case StylePropertyId.WhiteSpace:
					return typeof(WhiteSpace);
				case StylePropertyId.WordSpacing:
					return typeof(Length);
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
						return typeof(Align);
					case StylePropertyId.AlignItems:
						return typeof(Align);
					case StylePropertyId.AlignSelf:
						return typeof(Align);
					case StylePropertyId.BorderBottomWidth:
						return typeof(float);
					case StylePropertyId.BorderLeftWidth:
						return typeof(float);
					case StylePropertyId.BorderRightWidth:
						return typeof(float);
					case StylePropertyId.BorderTopWidth:
						return typeof(float);
					case StylePropertyId.Bottom:
						return typeof(Length);
					case StylePropertyId.Display:
						return typeof(DisplayStyle);
					case StylePropertyId.FlexBasis:
						return typeof(Length);
					case StylePropertyId.FlexDirection:
						return typeof(FlexDirection);
					case StylePropertyId.FlexGrow:
						return typeof(float);
					case StylePropertyId.FlexShrink:
						return typeof(float);
					case StylePropertyId.FlexWrap:
						return typeof(Wrap);
					case StylePropertyId.Height:
						return typeof(Length);
					case StylePropertyId.JustifyContent:
						return typeof(Justify);
					case StylePropertyId.Left:
						return typeof(Length);
					case StylePropertyId.MarginBottom:
						return typeof(Length);
					case StylePropertyId.MarginLeft:
						return typeof(Length);
					case StylePropertyId.MarginRight:
						return typeof(Length);
					case StylePropertyId.MarginTop:
						return typeof(Length);
					case StylePropertyId.MaxHeight:
						return typeof(Length);
					case StylePropertyId.MaxWidth:
						return typeof(Length);
					case StylePropertyId.MinHeight:
						return typeof(Length);
					case StylePropertyId.MinWidth:
						return typeof(Length);
					case StylePropertyId.PaddingBottom:
						return typeof(Length);
					case StylePropertyId.PaddingLeft:
						return typeof(Length);
					case StylePropertyId.PaddingRight:
						return typeof(Length);
					case StylePropertyId.PaddingTop:
						return typeof(Length);
					case StylePropertyId.Position:
						return typeof(Position);
					case StylePropertyId.Right:
						return typeof(Length);
					case StylePropertyId.Top:
						return typeof(Length);
					case StylePropertyId.Width:
						return typeof(Length);
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
							return typeof(Cursor);
						case StylePropertyId.TextOverflow:
							return typeof(TextOverflow);
						case StylePropertyId.UnityBackgroundImageTintColor:
							return typeof(Color);
						case StylePropertyId.UnityOverflowClipBox:
							return typeof(OverflowClipBox);
						case StylePropertyId.UnitySliceBottom:
							return typeof(int);
						case StylePropertyId.UnitySliceLeft:
							return typeof(int);
						case StylePropertyId.UnitySliceRight:
							return typeof(int);
						case StylePropertyId.UnitySliceScale:
							return typeof(float);
						case StylePropertyId.UnitySliceTop:
							return typeof(int);
						case StylePropertyId.UnityTextOverflowPosition:
							return typeof(TextOverflowPosition);
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
					return typeof(Rotate);
				case StylePropertyId.Scale:
					return typeof(Scale);
				case StylePropertyId.TransformOrigin:
					return typeof(TransformOrigin);
				case StylePropertyId.Translate:
					return typeof(Translate);
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
						return typeof(List<TimeValue>);
					case StylePropertyId.TransitionDuration:
						return typeof(List<TimeValue>);
					case StylePropertyId.TransitionProperty:
						return typeof(List<StylePropertyName>);
					case StylePropertyId.TransitionTimingFunction:
						return typeof(List<EasingFunction>);
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
							return typeof(Color);
						case StylePropertyId.BackgroundImage:
							return typeof(Background);
						case StylePropertyId.BackgroundPositionX:
							return typeof(BackgroundPosition);
						case StylePropertyId.BackgroundPositionY:
							return typeof(BackgroundPosition);
						case StylePropertyId.BackgroundRepeat:
							return typeof(BackgroundRepeat);
						case StylePropertyId.BackgroundSize:
							return typeof(BackgroundSize);
						case StylePropertyId.BorderBottomColor:
							return typeof(Color);
						case StylePropertyId.BorderBottomLeftRadius:
							return typeof(Length);
						case StylePropertyId.BorderBottomRightRadius:
							return typeof(Length);
						case StylePropertyId.BorderLeftColor:
							return typeof(Color);
						case StylePropertyId.BorderRightColor:
							return typeof(Color);
						case StylePropertyId.BorderTopColor:
							return typeof(Color);
						case StylePropertyId.BorderTopLeftRadius:
							return typeof(Length);
						case StylePropertyId.BorderTopRightRadius:
							return typeof(Length);
						case StylePropertyId.Opacity:
							return typeof(float);
						case StylePropertyId.Overflow:
							return typeof(OverflowInternal);
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot get computed style type for property id {0}", id));
			return null;
		}

		public static Type GetShorthandStyleType(StylePropertyId id)
		{
			switch (id)
			{
			case StylePropertyId.BackgroundPosition:
				return typeof(BackgroundPosition);
			case StylePropertyId.BorderColor:
				return typeof(Color);
			case StylePropertyId.BorderRadius:
				return typeof(Length);
			case StylePropertyId.BorderWidth:
				return typeof(float);
			case StylePropertyId.Margin:
				return typeof(Length);
			case StylePropertyId.Padding:
				return typeof(Length);
			}
			Debug.LogAssertion(string.Format("Cannot get shorthand style type for property id {0}", id));
			return null;
		}

		public static object GetInlineStyleValue(IStyle style, StylePropertyId id)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
					return style.color;
				case StylePropertyId.FontSize:
					return style.fontSize;
				case StylePropertyId.LetterSpacing:
					return style.letterSpacing;
				case StylePropertyId.TextShadow:
					return style.textShadow;
				case StylePropertyId.UnityFont:
					return style.unityFont;
				case StylePropertyId.UnityFontDefinition:
					return style.unityFontDefinition;
				case StylePropertyId.UnityFontStyleAndWeight:
					return style.unityFontStyleAndWeight;
				case StylePropertyId.UnityParagraphSpacing:
					return style.unityParagraphSpacing;
				case StylePropertyId.UnityTextAlign:
					return style.unityTextAlign;
				case StylePropertyId.UnityTextOutlineColor:
					return style.unityTextOutlineColor;
				case StylePropertyId.UnityTextOutlineWidth:
					return style.unityTextOutlineWidth;
				case StylePropertyId.Visibility:
					return style.visibility;
				case StylePropertyId.WhiteSpace:
					return style.whiteSpace;
				case StylePropertyId.WordSpacing:
					return style.wordSpacing;
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
						return style.alignContent;
					case StylePropertyId.AlignItems:
						return style.alignItems;
					case StylePropertyId.AlignSelf:
						return style.alignSelf;
					case StylePropertyId.BorderBottomWidth:
						return style.borderBottomWidth;
					case StylePropertyId.BorderLeftWidth:
						return style.borderLeftWidth;
					case StylePropertyId.BorderRightWidth:
						return style.borderRightWidth;
					case StylePropertyId.BorderTopWidth:
						return style.borderTopWidth;
					case StylePropertyId.Bottom:
						return style.bottom;
					case StylePropertyId.Display:
						return style.display;
					case StylePropertyId.FlexBasis:
						return style.flexBasis;
					case StylePropertyId.FlexDirection:
						return style.flexDirection;
					case StylePropertyId.FlexGrow:
						return style.flexGrow;
					case StylePropertyId.FlexShrink:
						return style.flexShrink;
					case StylePropertyId.FlexWrap:
						return style.flexWrap;
					case StylePropertyId.Height:
						return style.height;
					case StylePropertyId.JustifyContent:
						return style.justifyContent;
					case StylePropertyId.Left:
						return style.left;
					case StylePropertyId.MarginBottom:
						return style.marginBottom;
					case StylePropertyId.MarginLeft:
						return style.marginLeft;
					case StylePropertyId.MarginRight:
						return style.marginRight;
					case StylePropertyId.MarginTop:
						return style.marginTop;
					case StylePropertyId.MaxHeight:
						return style.maxHeight;
					case StylePropertyId.MaxWidth:
						return style.maxWidth;
					case StylePropertyId.MinHeight:
						return style.minHeight;
					case StylePropertyId.MinWidth:
						return style.minWidth;
					case StylePropertyId.PaddingBottom:
						return style.paddingBottom;
					case StylePropertyId.PaddingLeft:
						return style.paddingLeft;
					case StylePropertyId.PaddingRight:
						return style.paddingRight;
					case StylePropertyId.PaddingTop:
						return style.paddingTop;
					case StylePropertyId.Position:
						return style.position;
					case StylePropertyId.Right:
						return style.right;
					case StylePropertyId.Top:
						return style.top;
					case StylePropertyId.Width:
						return style.width;
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
							return style.cursor;
						case StylePropertyId.TextOverflow:
							return style.textOverflow;
						case StylePropertyId.UnityBackgroundImageTintColor:
							return style.unityBackgroundImageTintColor;
						case StylePropertyId.UnityOverflowClipBox:
							return style.unityOverflowClipBox;
						case StylePropertyId.UnitySliceBottom:
							return style.unitySliceBottom;
						case StylePropertyId.UnitySliceLeft:
							return style.unitySliceLeft;
						case StylePropertyId.UnitySliceRight:
							return style.unitySliceRight;
						case StylePropertyId.UnitySliceScale:
							return style.unitySliceScale;
						case StylePropertyId.UnitySliceTop:
							return style.unitySliceTop;
						case StylePropertyId.UnityTextOverflowPosition:
							return style.unityTextOverflowPosition;
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
					return style.rotate;
				case StylePropertyId.Scale:
					return style.scale;
				case StylePropertyId.TransformOrigin:
					return style.transformOrigin;
				case StylePropertyId.Translate:
					return style.translate;
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
						return style.transitionDelay;
					case StylePropertyId.TransitionDuration:
						return style.transitionDuration;
					case StylePropertyId.TransitionProperty:
						return style.transitionProperty;
					case StylePropertyId.TransitionTimingFunction:
						return style.transitionTimingFunction;
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
							return style.backgroundColor;
						case StylePropertyId.BackgroundImage:
							return style.backgroundImage;
						case StylePropertyId.BackgroundPositionX:
							return style.backgroundPositionX;
						case StylePropertyId.BackgroundPositionY:
							return style.backgroundPositionY;
						case StylePropertyId.BackgroundRepeat:
							return style.backgroundRepeat;
						case StylePropertyId.BackgroundSize:
							return style.backgroundSize;
						case StylePropertyId.BorderBottomColor:
							return style.borderBottomColor;
						case StylePropertyId.BorderBottomLeftRadius:
							return style.borderBottomLeftRadius;
						case StylePropertyId.BorderBottomRightRadius:
							return style.borderBottomRightRadius;
						case StylePropertyId.BorderLeftColor:
							return style.borderLeftColor;
						case StylePropertyId.BorderRightColor:
							return style.borderRightColor;
						case StylePropertyId.BorderTopColor:
							return style.borderTopColor;
						case StylePropertyId.BorderTopLeftRadius:
							return style.borderTopLeftRadius;
						case StylePropertyId.BorderTopRightRadius:
							return style.borderTopRightRadius;
						case StylePropertyId.Opacity:
							return style.opacity;
						case StylePropertyId.Overflow:
							return style.overflow;
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot get inline style value for property id {0}", id));
			return null;
		}

		public static void SetInlineStyleValue(IStyle style, StylePropertyId id, object value)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
					style.color = (StyleColor)value;
					return;
				case StylePropertyId.FontSize:
					style.fontSize = (StyleLength)value;
					return;
				case StylePropertyId.LetterSpacing:
					style.letterSpacing = (StyleLength)value;
					return;
				case StylePropertyId.TextShadow:
					style.textShadow = (StyleTextShadow)value;
					return;
				case StylePropertyId.UnityFont:
					style.unityFont = (StyleFont)value;
					return;
				case StylePropertyId.UnityFontDefinition:
					style.unityFontDefinition = (StyleFontDefinition)value;
					return;
				case StylePropertyId.UnityFontStyleAndWeight:
					style.unityFontStyleAndWeight = (StyleEnum<FontStyle>)value;
					return;
				case StylePropertyId.UnityParagraphSpacing:
					style.unityParagraphSpacing = (StyleLength)value;
					return;
				case StylePropertyId.UnityTextAlign:
					style.unityTextAlign = (StyleEnum<TextAnchor>)value;
					return;
				case StylePropertyId.UnityTextOutlineColor:
					style.unityTextOutlineColor = (StyleColor)value;
					return;
				case StylePropertyId.UnityTextOutlineWidth:
					style.unityTextOutlineWidth = (StyleFloat)value;
					return;
				case StylePropertyId.Visibility:
					style.visibility = (StyleEnum<Visibility>)value;
					return;
				case StylePropertyId.WhiteSpace:
					style.whiteSpace = (StyleEnum<WhiteSpace>)value;
					return;
				case StylePropertyId.WordSpacing:
					style.wordSpacing = (StyleLength)value;
					return;
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
						style.alignContent = (StyleEnum<Align>)value;
						return;
					case StylePropertyId.AlignItems:
						style.alignItems = (StyleEnum<Align>)value;
						return;
					case StylePropertyId.AlignSelf:
						style.alignSelf = (StyleEnum<Align>)value;
						return;
					case StylePropertyId.BorderBottomWidth:
						style.borderBottomWidth = (StyleFloat)value;
						return;
					case StylePropertyId.BorderLeftWidth:
						style.borderLeftWidth = (StyleFloat)value;
						return;
					case StylePropertyId.BorderRightWidth:
						style.borderRightWidth = (StyleFloat)value;
						return;
					case StylePropertyId.BorderTopWidth:
						style.borderTopWidth = (StyleFloat)value;
						return;
					case StylePropertyId.Bottom:
						style.bottom = (StyleLength)value;
						return;
					case StylePropertyId.Display:
						style.display = (StyleEnum<DisplayStyle>)value;
						return;
					case StylePropertyId.FlexBasis:
						style.flexBasis = (StyleLength)value;
						return;
					case StylePropertyId.FlexDirection:
						style.flexDirection = (StyleEnum<FlexDirection>)value;
						return;
					case StylePropertyId.FlexGrow:
						style.flexGrow = (StyleFloat)value;
						return;
					case StylePropertyId.FlexShrink:
						style.flexShrink = (StyleFloat)value;
						return;
					case StylePropertyId.FlexWrap:
						style.flexWrap = (StyleEnum<Wrap>)value;
						return;
					case StylePropertyId.Height:
						style.height = (StyleLength)value;
						return;
					case StylePropertyId.JustifyContent:
						style.justifyContent = (StyleEnum<Justify>)value;
						return;
					case StylePropertyId.Left:
						style.left = (StyleLength)value;
						return;
					case StylePropertyId.MarginBottom:
						style.marginBottom = (StyleLength)value;
						return;
					case StylePropertyId.MarginLeft:
						style.marginLeft = (StyleLength)value;
						return;
					case StylePropertyId.MarginRight:
						style.marginRight = (StyleLength)value;
						return;
					case StylePropertyId.MarginTop:
						style.marginTop = (StyleLength)value;
						return;
					case StylePropertyId.MaxHeight:
						style.maxHeight = (StyleLength)value;
						return;
					case StylePropertyId.MaxWidth:
						style.maxWidth = (StyleLength)value;
						return;
					case StylePropertyId.MinHeight:
						style.minHeight = (StyleLength)value;
						return;
					case StylePropertyId.MinWidth:
						style.minWidth = (StyleLength)value;
						return;
					case StylePropertyId.PaddingBottom:
						style.paddingBottom = (StyleLength)value;
						return;
					case StylePropertyId.PaddingLeft:
						style.paddingLeft = (StyleLength)value;
						return;
					case StylePropertyId.PaddingRight:
						style.paddingRight = (StyleLength)value;
						return;
					case StylePropertyId.PaddingTop:
						style.paddingTop = (StyleLength)value;
						return;
					case StylePropertyId.Position:
						style.position = (StyleEnum<Position>)value;
						return;
					case StylePropertyId.Right:
						style.right = (StyleLength)value;
						return;
					case StylePropertyId.Top:
						style.top = (StyleLength)value;
						return;
					case StylePropertyId.Width:
						style.width = (StyleLength)value;
						return;
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
							style.cursor = (StyleCursor)value;
							return;
						case StylePropertyId.TextOverflow:
							style.textOverflow = (StyleEnum<TextOverflow>)value;
							return;
						case StylePropertyId.UnityBackgroundImageTintColor:
							style.unityBackgroundImageTintColor = (StyleColor)value;
							return;
						case StylePropertyId.UnityOverflowClipBox:
							style.unityOverflowClipBox = (StyleEnum<OverflowClipBox>)value;
							return;
						case StylePropertyId.UnitySliceBottom:
							style.unitySliceBottom = (StyleInt)value;
							return;
						case StylePropertyId.UnitySliceLeft:
							style.unitySliceLeft = (StyleInt)value;
							return;
						case StylePropertyId.UnitySliceRight:
							style.unitySliceRight = (StyleInt)value;
							return;
						case StylePropertyId.UnitySliceScale:
							style.unitySliceScale = (StyleFloat)value;
							return;
						case StylePropertyId.UnitySliceTop:
							style.unitySliceTop = (StyleInt)value;
							return;
						case StylePropertyId.UnityTextOverflowPosition:
							style.unityTextOverflowPosition = (StyleEnum<TextOverflowPosition>)value;
							return;
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
					style.rotate = (StyleRotate)value;
					return;
				case StylePropertyId.Scale:
					style.scale = (StyleScale)value;
					return;
				case StylePropertyId.TransformOrigin:
					style.transformOrigin = (StyleTransformOrigin)value;
					return;
				case StylePropertyId.Translate:
					style.translate = (StyleTranslate)value;
					return;
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
						style.transitionDelay = (StyleList<TimeValue>)value;
						return;
					case StylePropertyId.TransitionDuration:
						style.transitionDuration = (StyleList<TimeValue>)value;
						return;
					case StylePropertyId.TransitionProperty:
						style.transitionProperty = (StyleList<StylePropertyName>)value;
						return;
					case StylePropertyId.TransitionTimingFunction:
						style.transitionTimingFunction = (StyleList<EasingFunction>)value;
						return;
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
							style.backgroundColor = (StyleColor)value;
							return;
						case StylePropertyId.BackgroundImage:
							style.backgroundImage = (StyleBackground)value;
							return;
						case StylePropertyId.BackgroundPositionX:
							style.backgroundPositionX = (StyleBackgroundPosition)value;
							return;
						case StylePropertyId.BackgroundPositionY:
							style.backgroundPositionY = (StyleBackgroundPosition)value;
							return;
						case StylePropertyId.BackgroundRepeat:
							style.backgroundRepeat = (StyleBackgroundRepeat)value;
							return;
						case StylePropertyId.BackgroundSize:
							style.backgroundSize = (StyleBackgroundSize)value;
							return;
						case StylePropertyId.BorderBottomColor:
							style.borderBottomColor = (StyleColor)value;
							return;
						case StylePropertyId.BorderBottomLeftRadius:
							style.borderBottomLeftRadius = (StyleLength)value;
							return;
						case StylePropertyId.BorderBottomRightRadius:
							style.borderBottomRightRadius = (StyleLength)value;
							return;
						case StylePropertyId.BorderLeftColor:
							style.borderLeftColor = (StyleColor)value;
							return;
						case StylePropertyId.BorderRightColor:
							style.borderRightColor = (StyleColor)value;
							return;
						case StylePropertyId.BorderTopColor:
							style.borderTopColor = (StyleColor)value;
							return;
						case StylePropertyId.BorderTopLeftRadius:
							style.borderTopLeftRadius = (StyleLength)value;
							return;
						case StylePropertyId.BorderTopRightRadius:
							style.borderTopRightRadius = (StyleLength)value;
							return;
						case StylePropertyId.Opacity:
							style.opacity = (StyleFloat)value;
							return;
						case StylePropertyId.Overflow:
							style.overflow = (StyleEnum<Overflow>)value;
							return;
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot set inline style value for property id {0}", id));
		}

		public static void SetInlineKeyword(IStyle style, StylePropertyId id, StyleKeyword keyword)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
					style.color = keyword;
					return;
				case StylePropertyId.FontSize:
					style.fontSize = keyword;
					return;
				case StylePropertyId.LetterSpacing:
					style.letterSpacing = keyword;
					return;
				case StylePropertyId.TextShadow:
					style.textShadow = keyword;
					return;
				case StylePropertyId.UnityFont:
					style.unityFont = keyword;
					return;
				case StylePropertyId.UnityFontDefinition:
					style.unityFontDefinition = keyword;
					return;
				case StylePropertyId.UnityFontStyleAndWeight:
					style.unityFontStyleAndWeight = keyword;
					return;
				case StylePropertyId.UnityParagraphSpacing:
					style.unityParagraphSpacing = keyword;
					return;
				case StylePropertyId.UnityTextAlign:
					style.unityTextAlign = keyword;
					return;
				case StylePropertyId.UnityTextOutlineColor:
					style.unityTextOutlineColor = keyword;
					return;
				case StylePropertyId.UnityTextOutlineWidth:
					style.unityTextOutlineWidth = keyword;
					return;
				case StylePropertyId.Visibility:
					style.visibility = keyword;
					return;
				case StylePropertyId.WhiteSpace:
					style.whiteSpace = keyword;
					return;
				case StylePropertyId.WordSpacing:
					style.wordSpacing = keyword;
					return;
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
						style.alignContent = keyword;
						return;
					case StylePropertyId.AlignItems:
						style.alignItems = keyword;
						return;
					case StylePropertyId.AlignSelf:
						style.alignSelf = keyword;
						return;
					case StylePropertyId.BorderBottomWidth:
						style.borderBottomWidth = keyword;
						return;
					case StylePropertyId.BorderLeftWidth:
						style.borderLeftWidth = keyword;
						return;
					case StylePropertyId.BorderRightWidth:
						style.borderRightWidth = keyword;
						return;
					case StylePropertyId.BorderTopWidth:
						style.borderTopWidth = keyword;
						return;
					case StylePropertyId.Bottom:
						style.bottom = keyword;
						return;
					case StylePropertyId.Display:
						style.display = keyword;
						return;
					case StylePropertyId.FlexBasis:
						style.flexBasis = keyword;
						return;
					case StylePropertyId.FlexDirection:
						style.flexDirection = keyword;
						return;
					case StylePropertyId.FlexGrow:
						style.flexGrow = keyword;
						return;
					case StylePropertyId.FlexShrink:
						style.flexShrink = keyword;
						return;
					case StylePropertyId.FlexWrap:
						style.flexWrap = keyword;
						return;
					case StylePropertyId.Height:
						style.height = keyword;
						return;
					case StylePropertyId.JustifyContent:
						style.justifyContent = keyword;
						return;
					case StylePropertyId.Left:
						style.left = keyword;
						return;
					case StylePropertyId.MarginBottom:
						style.marginBottom = keyword;
						return;
					case StylePropertyId.MarginLeft:
						style.marginLeft = keyword;
						return;
					case StylePropertyId.MarginRight:
						style.marginRight = keyword;
						return;
					case StylePropertyId.MarginTop:
						style.marginTop = keyword;
						return;
					case StylePropertyId.MaxHeight:
						style.maxHeight = keyword;
						return;
					case StylePropertyId.MaxWidth:
						style.maxWidth = keyword;
						return;
					case StylePropertyId.MinHeight:
						style.minHeight = keyword;
						return;
					case StylePropertyId.MinWidth:
						style.minWidth = keyword;
						return;
					case StylePropertyId.PaddingBottom:
						style.paddingBottom = keyword;
						return;
					case StylePropertyId.PaddingLeft:
						style.paddingLeft = keyword;
						return;
					case StylePropertyId.PaddingRight:
						style.paddingRight = keyword;
						return;
					case StylePropertyId.PaddingTop:
						style.paddingTop = keyword;
						return;
					case StylePropertyId.Position:
						style.position = keyword;
						return;
					case StylePropertyId.Right:
						style.right = keyword;
						return;
					case StylePropertyId.Top:
						style.top = keyword;
						return;
					case StylePropertyId.Width:
						style.width = keyword;
						return;
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
							style.cursor = keyword;
							return;
						case StylePropertyId.TextOverflow:
							style.textOverflow = keyword;
							return;
						case StylePropertyId.UnityBackgroundImageTintColor:
							style.unityBackgroundImageTintColor = keyword;
							return;
						case StylePropertyId.UnityOverflowClipBox:
							style.unityOverflowClipBox = keyword;
							return;
						case StylePropertyId.UnitySliceBottom:
							style.unitySliceBottom = keyword;
							return;
						case StylePropertyId.UnitySliceLeft:
							style.unitySliceLeft = keyword;
							return;
						case StylePropertyId.UnitySliceRight:
							style.unitySliceRight = keyword;
							return;
						case StylePropertyId.UnitySliceScale:
							style.unitySliceScale = keyword;
							return;
						case StylePropertyId.UnitySliceTop:
							style.unitySliceTop = keyword;
							return;
						case StylePropertyId.UnityTextOverflowPosition:
							style.unityTextOverflowPosition = keyword;
							return;
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
					style.rotate = keyword;
					return;
				case StylePropertyId.Scale:
					style.scale = keyword;
					return;
				case StylePropertyId.TransformOrigin:
					style.transformOrigin = keyword;
					return;
				case StylePropertyId.Translate:
					style.translate = keyword;
					return;
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
						style.transitionDelay = keyword;
						return;
					case StylePropertyId.TransitionDuration:
						style.transitionDuration = keyword;
						return;
					case StylePropertyId.TransitionProperty:
						style.transitionProperty = keyword;
						return;
					case StylePropertyId.TransitionTimingFunction:
						style.transitionTimingFunction = keyword;
						return;
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
							style.backgroundColor = keyword;
							return;
						case StylePropertyId.BackgroundImage:
							style.backgroundImage = keyword;
							return;
						case StylePropertyId.BackgroundPositionX:
							style.backgroundPositionX = keyword;
							return;
						case StylePropertyId.BackgroundPositionY:
							style.backgroundPositionY = keyword;
							return;
						case StylePropertyId.BackgroundRepeat:
							style.backgroundRepeat = keyword;
							return;
						case StylePropertyId.BackgroundSize:
							style.backgroundSize = keyword;
							return;
						case StylePropertyId.BorderBottomColor:
							style.borderBottomColor = keyword;
							return;
						case StylePropertyId.BorderBottomLeftRadius:
							style.borderBottomLeftRadius = keyword;
							return;
						case StylePropertyId.BorderBottomRightRadius:
							style.borderBottomRightRadius = keyword;
							return;
						case StylePropertyId.BorderLeftColor:
							style.borderLeftColor = keyword;
							return;
						case StylePropertyId.BorderRightColor:
							style.borderRightColor = keyword;
							return;
						case StylePropertyId.BorderTopColor:
							style.borderTopColor = keyword;
							return;
						case StylePropertyId.BorderTopLeftRadius:
							style.borderTopLeftRadius = keyword;
							return;
						case StylePropertyId.BorderTopRightRadius:
							style.borderTopRightRadius = keyword;
							return;
						case StylePropertyId.Opacity:
							style.opacity = keyword;
							return;
						case StylePropertyId.Overflow:
							style.overflow = keyword;
							return;
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot set inline keyword value for property id {0}", id));
		}

		public static List<StyleKeyword> GetValidKeyword(StylePropertyId id)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
					return new List<StyleKeyword>();
				case StylePropertyId.FontSize:
					return new List<StyleKeyword>();
				case StylePropertyId.LetterSpacing:
					return new List<StyleKeyword>();
				case StylePropertyId.TextShadow:
					return new List<StyleKeyword>();
				case StylePropertyId.UnityFont:
					return new List<StyleKeyword>();
				case StylePropertyId.UnityFontDefinition:
					return new List<StyleKeyword>();
				case StylePropertyId.UnityFontStyleAndWeight:
					return new List<StyleKeyword>();
				case StylePropertyId.UnityParagraphSpacing:
					return new List<StyleKeyword>();
				case StylePropertyId.UnityTextAlign:
					return new List<StyleKeyword>();
				case StylePropertyId.UnityTextOutlineColor:
					return new List<StyleKeyword>();
				case StylePropertyId.UnityTextOutlineWidth:
					return new List<StyleKeyword>();
				case StylePropertyId.Visibility:
					return new List<StyleKeyword>();
				case StylePropertyId.WhiteSpace:
					return new List<StyleKeyword>();
				case StylePropertyId.WordSpacing:
					return new List<StyleKeyword>();
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.AlignItems:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.AlignSelf:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.BorderBottomWidth:
						return new List<StyleKeyword>();
					case StylePropertyId.BorderLeftWidth:
						return new List<StyleKeyword>();
					case StylePropertyId.BorderRightWidth:
						return new List<StyleKeyword>();
					case StylePropertyId.BorderTopWidth:
						return new List<StyleKeyword>();
					case StylePropertyId.Bottom:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.Display:
						return new List<StyleKeyword> { StyleKeyword.None };
					case StylePropertyId.FlexBasis:
						return new List<StyleKeyword>();
					case StylePropertyId.FlexDirection:
						return new List<StyleKeyword>();
					case StylePropertyId.FlexGrow:
						return new List<StyleKeyword>();
					case StylePropertyId.FlexShrink:
						return new List<StyleKeyword>();
					case StylePropertyId.FlexWrap:
						return new List<StyleKeyword>();
					case StylePropertyId.Height:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.JustifyContent:
						return new List<StyleKeyword>();
					case StylePropertyId.Left:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.MarginBottom:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.MarginLeft:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.MarginRight:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.MarginTop:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.MaxHeight:
						return new List<StyleKeyword> { StyleKeyword.None };
					case StylePropertyId.MaxWidth:
						return new List<StyleKeyword> { StyleKeyword.None };
					case StylePropertyId.MinHeight:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.MinWidth:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.PaddingBottom:
						return new List<StyleKeyword>();
					case StylePropertyId.PaddingLeft:
						return new List<StyleKeyword>();
					case StylePropertyId.PaddingRight:
						return new List<StyleKeyword>();
					case StylePropertyId.PaddingTop:
						return new List<StyleKeyword>();
					case StylePropertyId.Position:
						return new List<StyleKeyword>();
					case StylePropertyId.Right:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.Top:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					case StylePropertyId.Width:
						return new List<StyleKeyword> { StyleKeyword.Auto };
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
							return new List<StyleKeyword>();
						case StylePropertyId.TextOverflow:
							return new List<StyleKeyword>();
						case StylePropertyId.UnityBackgroundImageTintColor:
							return new List<StyleKeyword>();
						case StylePropertyId.UnityOverflowClipBox:
							return new List<StyleKeyword>();
						case StylePropertyId.UnitySliceBottom:
							return new List<StyleKeyword>();
						case StylePropertyId.UnitySliceLeft:
							return new List<StyleKeyword>();
						case StylePropertyId.UnitySliceRight:
							return new List<StyleKeyword>();
						case StylePropertyId.UnitySliceScale:
							return new List<StyleKeyword>();
						case StylePropertyId.UnitySliceTop:
							return new List<StyleKeyword>();
						case StylePropertyId.UnityTextOverflowPosition:
							return new List<StyleKeyword>();
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
					return new List<StyleKeyword> { StyleKeyword.None };
				case StylePropertyId.Scale:
					return new List<StyleKeyword> { StyleKeyword.None };
				case StylePropertyId.TransformOrigin:
					return new List<StyleKeyword>();
				case StylePropertyId.Translate:
					return new List<StyleKeyword> { StyleKeyword.None };
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
						return new List<StyleKeyword>();
					case StylePropertyId.TransitionDuration:
						return new List<StyleKeyword>();
					case StylePropertyId.TransitionProperty:
						return new List<StyleKeyword> { StyleKeyword.None };
					case StylePropertyId.TransitionTimingFunction:
						return new List<StyleKeyword>();
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
							return new List<StyleKeyword>();
						case StylePropertyId.BackgroundImage:
							return new List<StyleKeyword> { StyleKeyword.None };
						case StylePropertyId.BackgroundPositionX:
							return new List<StyleKeyword>();
						case StylePropertyId.BackgroundPositionY:
							return new List<StyleKeyword>();
						case StylePropertyId.BackgroundRepeat:
							return new List<StyleKeyword>();
						case StylePropertyId.BackgroundSize:
							return new List<StyleKeyword> { StyleKeyword.Auto };
						case StylePropertyId.BorderBottomColor:
							return new List<StyleKeyword>();
						case StylePropertyId.BorderBottomLeftRadius:
							return new List<StyleKeyword>();
						case StylePropertyId.BorderBottomRightRadius:
							return new List<StyleKeyword>();
						case StylePropertyId.BorderLeftColor:
							return new List<StyleKeyword>();
						case StylePropertyId.BorderRightColor:
							return new List<StyleKeyword>();
						case StylePropertyId.BorderTopColor:
							return new List<StyleKeyword>();
						case StylePropertyId.BorderTopLeftRadius:
							return new List<StyleKeyword>();
						case StylePropertyId.BorderTopRightRadius:
							return new List<StyleKeyword>();
						case StylePropertyId.Opacity:
							return new List<StyleKeyword>();
						case StylePropertyId.Overflow:
							return new List<StyleKeyword>();
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot get valid keyword value for property id {0}", id));
			return null;
		}

		public static object ConvertComputedToInlineStyleValue(StylePropertyId id, object value)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
					return (Color)value;
				case StylePropertyId.FontSize:
					return (Length)value;
				case StylePropertyId.LetterSpacing:
					return (Length)value;
				case StylePropertyId.TextShadow:
					return (TextShadow)value;
				case StylePropertyId.UnityFont:
					return (Font)value;
				case StylePropertyId.UnityFontDefinition:
					return (FontDefinition)value;
				case StylePropertyId.UnityFontStyleAndWeight:
					return (FontStyle)value;
				case StylePropertyId.UnityParagraphSpacing:
					return (Length)value;
				case StylePropertyId.UnityTextAlign:
					return (TextAnchor)value;
				case StylePropertyId.UnityTextOutlineColor:
					return (Color)value;
				case StylePropertyId.UnityTextOutlineWidth:
					return (float)value;
				case StylePropertyId.Visibility:
					return (Visibility)value;
				case StylePropertyId.WhiteSpace:
					return (WhiteSpace)value;
				case StylePropertyId.WordSpacing:
					return (Length)value;
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
						return (Align)value;
					case StylePropertyId.AlignItems:
						return (Align)value;
					case StylePropertyId.AlignSelf:
						return (Align)value;
					case StylePropertyId.BorderBottomWidth:
						return (float)value;
					case StylePropertyId.BorderLeftWidth:
						return (float)value;
					case StylePropertyId.BorderRightWidth:
						return (float)value;
					case StylePropertyId.BorderTopWidth:
						return (float)value;
					case StylePropertyId.Bottom:
						return (Length)value;
					case StylePropertyId.Display:
						return (DisplayStyle)value;
					case StylePropertyId.FlexBasis:
						return (Length)value;
					case StylePropertyId.FlexDirection:
						return (FlexDirection)value;
					case StylePropertyId.FlexGrow:
						return (float)value;
					case StylePropertyId.FlexShrink:
						return (float)value;
					case StylePropertyId.FlexWrap:
						return (Wrap)value;
					case StylePropertyId.Height:
						return (Length)value;
					case StylePropertyId.JustifyContent:
						return (Justify)value;
					case StylePropertyId.Left:
						return (Length)value;
					case StylePropertyId.MarginBottom:
						return (Length)value;
					case StylePropertyId.MarginLeft:
						return (Length)value;
					case StylePropertyId.MarginRight:
						return (Length)value;
					case StylePropertyId.MarginTop:
						return (Length)value;
					case StylePropertyId.MaxHeight:
						return (Length)value;
					case StylePropertyId.MaxWidth:
						return (Length)value;
					case StylePropertyId.MinHeight:
						return (Length)value;
					case StylePropertyId.MinWidth:
						return (Length)value;
					case StylePropertyId.PaddingBottom:
						return (Length)value;
					case StylePropertyId.PaddingLeft:
						return (Length)value;
					case StylePropertyId.PaddingRight:
						return (Length)value;
					case StylePropertyId.PaddingTop:
						return (Length)value;
					case StylePropertyId.Position:
						return (Position)value;
					case StylePropertyId.Right:
						return (Length)value;
					case StylePropertyId.Top:
						return (Length)value;
					case StylePropertyId.Width:
						return (Length)value;
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
							return (Cursor)value;
						case StylePropertyId.TextOverflow:
							return (TextOverflow)value;
						case StylePropertyId.UnityBackgroundImageTintColor:
							return (Color)value;
						case StylePropertyId.UnityOverflowClipBox:
							return (OverflowClipBox)value;
						case StylePropertyId.UnitySliceBottom:
							return (int)value;
						case StylePropertyId.UnitySliceLeft:
							return (int)value;
						case StylePropertyId.UnitySliceRight:
							return (int)value;
						case StylePropertyId.UnitySliceScale:
							return (float)value;
						case StylePropertyId.UnitySliceTop:
							return (int)value;
						case StylePropertyId.UnityTextOverflowPosition:
							return (TextOverflowPosition)value;
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
					return (Rotate)value;
				case StylePropertyId.Scale:
					return (Scale)value;
				case StylePropertyId.TransformOrigin:
					return (TransformOrigin)value;
				case StylePropertyId.Translate:
					return (Translate)value;
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
						return (List<TimeValue>)value;
					case StylePropertyId.TransitionDuration:
						return (List<TimeValue>)value;
					case StylePropertyId.TransitionProperty:
						return (List<StylePropertyName>)value;
					case StylePropertyId.TransitionTimingFunction:
						return (List<EasingFunction>)value;
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
							return (Color)value;
						case StylePropertyId.BackgroundImage:
							return (Background)value;
						case StylePropertyId.BackgroundPositionX:
							return (BackgroundPosition)value;
						case StylePropertyId.BackgroundPositionY:
							return (BackgroundPosition)value;
						case StylePropertyId.BackgroundRepeat:
							return (BackgroundRepeat)value;
						case StylePropertyId.BackgroundSize:
							return (BackgroundSize)value;
						case StylePropertyId.BorderBottomColor:
							return (Color)value;
						case StylePropertyId.BorderBottomLeftRadius:
							return (Length)value;
						case StylePropertyId.BorderBottomRightRadius:
							return (Length)value;
						case StylePropertyId.BorderLeftColor:
							return (Color)value;
						case StylePropertyId.BorderRightColor:
							return (Color)value;
						case StylePropertyId.BorderTopColor:
							return (Color)value;
						case StylePropertyId.BorderTopLeftRadius:
							return (Length)value;
						case StylePropertyId.BorderTopRightRadius:
							return (Length)value;
						case StylePropertyId.Opacity:
							return (float)value;
						case StylePropertyId.Overflow:
							return (Overflow)((OverflowInternal)value);
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot convert computed style value to inline style value for property id {0}", id));
			return null;
		}

		public static Type GetInlineStyleType(StylePropertyId id)
		{
			if (id <= StylePropertyId.UnityTextOverflowPosition)
			{
				switch (id)
				{
				case StylePropertyId.Color:
					return typeof(StyleColor);
				case StylePropertyId.FontSize:
					return typeof(StyleLength);
				case StylePropertyId.LetterSpacing:
					return typeof(StyleLength);
				case StylePropertyId.TextShadow:
					return typeof(StyleTextShadow);
				case StylePropertyId.UnityFont:
					return typeof(StyleFont);
				case StylePropertyId.UnityFontDefinition:
					return typeof(StyleFontDefinition);
				case StylePropertyId.UnityFontStyleAndWeight:
					return typeof(StyleEnum<FontStyle>);
				case StylePropertyId.UnityParagraphSpacing:
					return typeof(StyleLength);
				case StylePropertyId.UnityTextAlign:
					return typeof(StyleEnum<TextAnchor>);
				case StylePropertyId.UnityTextOutlineColor:
					return typeof(StyleColor);
				case StylePropertyId.UnityTextOutlineWidth:
					return typeof(StyleFloat);
				case StylePropertyId.Visibility:
					return typeof(StyleEnum<Visibility>);
				case StylePropertyId.WhiteSpace:
					return typeof(StyleEnum<WhiteSpace>);
				case StylePropertyId.WordSpacing:
					return typeof(StyleLength);
				default:
					switch (id)
					{
					case StylePropertyId.AlignContent:
						return typeof(StyleEnum<Align>);
					case StylePropertyId.AlignItems:
						return typeof(StyleEnum<Align>);
					case StylePropertyId.AlignSelf:
						return typeof(StyleEnum<Align>);
					case StylePropertyId.BorderBottomWidth:
						return typeof(StyleFloat);
					case StylePropertyId.BorderLeftWidth:
						return typeof(StyleFloat);
					case StylePropertyId.BorderRightWidth:
						return typeof(StyleFloat);
					case StylePropertyId.BorderTopWidth:
						return typeof(StyleFloat);
					case StylePropertyId.Bottom:
						return typeof(StyleLength);
					case StylePropertyId.Display:
						return typeof(StyleEnum<DisplayStyle>);
					case StylePropertyId.FlexBasis:
						return typeof(StyleLength);
					case StylePropertyId.FlexDirection:
						return typeof(StyleEnum<FlexDirection>);
					case StylePropertyId.FlexGrow:
						return typeof(StyleFloat);
					case StylePropertyId.FlexShrink:
						return typeof(StyleFloat);
					case StylePropertyId.FlexWrap:
						return typeof(StyleEnum<Wrap>);
					case StylePropertyId.Height:
						return typeof(StyleLength);
					case StylePropertyId.JustifyContent:
						return typeof(StyleEnum<Justify>);
					case StylePropertyId.Left:
						return typeof(StyleLength);
					case StylePropertyId.MarginBottom:
						return typeof(StyleLength);
					case StylePropertyId.MarginLeft:
						return typeof(StyleLength);
					case StylePropertyId.MarginRight:
						return typeof(StyleLength);
					case StylePropertyId.MarginTop:
						return typeof(StyleLength);
					case StylePropertyId.MaxHeight:
						return typeof(StyleLength);
					case StylePropertyId.MaxWidth:
						return typeof(StyleLength);
					case StylePropertyId.MinHeight:
						return typeof(StyleLength);
					case StylePropertyId.MinWidth:
						return typeof(StyleLength);
					case StylePropertyId.PaddingBottom:
						return typeof(StyleLength);
					case StylePropertyId.PaddingLeft:
						return typeof(StyleLength);
					case StylePropertyId.PaddingRight:
						return typeof(StyleLength);
					case StylePropertyId.PaddingTop:
						return typeof(StyleLength);
					case StylePropertyId.Position:
						return typeof(StyleEnum<Position>);
					case StylePropertyId.Right:
						return typeof(StyleLength);
					case StylePropertyId.Top:
						return typeof(StyleLength);
					case StylePropertyId.Width:
						return typeof(StyleLength);
					default:
						switch (id)
						{
						case StylePropertyId.Cursor:
							return typeof(StyleCursor);
						case StylePropertyId.TextOverflow:
							return typeof(StyleEnum<TextOverflow>);
						case StylePropertyId.UnityBackgroundImageTintColor:
							return typeof(StyleColor);
						case StylePropertyId.UnityOverflowClipBox:
							return typeof(StyleEnum<OverflowClipBox>);
						case StylePropertyId.UnitySliceBottom:
							return typeof(StyleInt);
						case StylePropertyId.UnitySliceLeft:
							return typeof(StyleInt);
						case StylePropertyId.UnitySliceRight:
							return typeof(StyleInt);
						case StylePropertyId.UnitySliceScale:
							return typeof(StyleFloat);
						case StylePropertyId.UnitySliceTop:
							return typeof(StyleInt);
						case StylePropertyId.UnityTextOverflowPosition:
							return typeof(StyleEnum<TextOverflowPosition>);
						}
						break;
					}
					break;
				}
			}
			else
			{
				switch (id)
				{
				case StylePropertyId.Rotate:
					return typeof(StyleRotate);
				case StylePropertyId.Scale:
					return typeof(StyleScale);
				case StylePropertyId.TransformOrigin:
					return typeof(StyleTransformOrigin);
				case StylePropertyId.Translate:
					return typeof(StyleTranslate);
				default:
					switch (id)
					{
					case StylePropertyId.TransitionDelay:
						return typeof(StyleList<TimeValue>);
					case StylePropertyId.TransitionDuration:
						return typeof(StyleList<TimeValue>);
					case StylePropertyId.TransitionProperty:
						return typeof(StyleList<StylePropertyName>);
					case StylePropertyId.TransitionTimingFunction:
						return typeof(StyleList<EasingFunction>);
					default:
						switch (id)
						{
						case StylePropertyId.BackgroundColor:
							return typeof(StyleColor);
						case StylePropertyId.BackgroundImage:
							return typeof(StyleBackground);
						case StylePropertyId.BackgroundPositionX:
							return typeof(StyleBackgroundPosition);
						case StylePropertyId.BackgroundPositionY:
							return typeof(StyleBackgroundPosition);
						case StylePropertyId.BackgroundRepeat:
							return typeof(StyleBackgroundRepeat);
						case StylePropertyId.BackgroundSize:
							return typeof(StyleBackgroundSize);
						case StylePropertyId.BorderBottomColor:
							return typeof(StyleColor);
						case StylePropertyId.BorderBottomLeftRadius:
							return typeof(StyleLength);
						case StylePropertyId.BorderBottomRightRadius:
							return typeof(StyleLength);
						case StylePropertyId.BorderLeftColor:
							return typeof(StyleColor);
						case StylePropertyId.BorderRightColor:
							return typeof(StyleColor);
						case StylePropertyId.BorderTopColor:
							return typeof(StyleColor);
						case StylePropertyId.BorderTopLeftRadius:
							return typeof(StyleLength);
						case StylePropertyId.BorderTopRightRadius:
							return typeof(StyleLength);
						case StylePropertyId.Opacity:
							return typeof(StyleFloat);
						case StylePropertyId.Overflow:
							return typeof(StyleEnum<Overflow>);
						}
						break;
					}
					break;
				}
			}
			Debug.LogAssertion(string.Format("Cannot get computed style type for property id {0}", id));
			return null;
		}

		public static string[] GetLonghandPropertyNames(StylePropertyId id)
		{
			string[] array;
			switch (id)
			{
			case StylePropertyId.All:
				array = new string[0];
				break;
			case StylePropertyId.BackgroundPosition:
				array = new string[] { "background-position-x", "background-position-y" };
				break;
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
			case StylePropertyId.Transition:
				array = new string[] { "transition-delay", "transition-duration", "transition-property", "transition-timing-function" };
				break;
			case StylePropertyId.UnityBackgroundScaleMode:
				array = new string[] { "background-position-x", "background-position-y", "background-repeat", "background-size" };
				break;
			case StylePropertyId.UnityTextOutline:
				array = new string[] { "-unity-text-outline-color", "-unity-text-outline-width" };
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
			case StylePropertyId.All:
				flag = true;
				break;
			case StylePropertyId.BackgroundPosition:
				flag = true;
				break;
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
			case StylePropertyId.Transition:
				flag = true;
				break;
			case StylePropertyId.UnityBackgroundScaleMode:
				flag = true;
				break;
			case StylePropertyId.UnityTextOutline:
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
			case StylePropertyId.LetterSpacing:
				flag = true;
				break;
			case StylePropertyId.TextShadow:
				flag = true;
				break;
			case StylePropertyId.UnityFont:
				flag = true;
				break;
			case StylePropertyId.UnityFontDefinition:
				flag = true;
				break;
			case StylePropertyId.UnityFontStyleAndWeight:
				flag = true;
				break;
			case StylePropertyId.UnityParagraphSpacing:
				flag = true;
				break;
			case StylePropertyId.UnityTextAlign:
				flag = true;
				break;
			case StylePropertyId.UnityTextOutlineColor:
				flag = true;
				break;
			case StylePropertyId.UnityTextOutlineWidth:
				flag = true;
				break;
			case StylePropertyId.Visibility:
				flag = true;
				break;
			case StylePropertyId.WhiteSpace:
				flag = true;
				break;
			case StylePropertyId.WordSpacing:
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
				StylePropertyId.LetterSpacing,
				StylePropertyId.TextShadow,
				StylePropertyId.UnityFont,
				StylePropertyId.UnityFontDefinition,
				StylePropertyId.UnityFontStyleAndWeight,
				StylePropertyId.UnityParagraphSpacing,
				StylePropertyId.UnityTextAlign,
				StylePropertyId.UnityTextOutlineColor,
				StylePropertyId.UnityTextOutlineWidth,
				StylePropertyId.Visibility,
				StylePropertyId.WhiteSpace,
				StylePropertyId.WordSpacing
			};
		}

		public static bool IsDiscreteTypeProperty(StylePropertyId id)
		{
			if (id <= StylePropertyId.JustifyContent)
			{
				if (id <= StylePropertyId.Display)
				{
					switch (id)
					{
					case StylePropertyId.UnityFont:
						return true;
					case StylePropertyId.UnityFontDefinition:
						return true;
					case StylePropertyId.UnityFontStyleAndWeight:
						return true;
					case StylePropertyId.UnityParagraphSpacing:
					case StylePropertyId.UnityTextOutlineColor:
					case StylePropertyId.UnityTextOutlineWidth:
						break;
					case StylePropertyId.UnityTextAlign:
						return true;
					case StylePropertyId.Visibility:
						return true;
					case StylePropertyId.WhiteSpace:
						return true;
					default:
						switch (id)
						{
						case StylePropertyId.AlignContent:
							return true;
						case StylePropertyId.AlignItems:
							return true;
						case StylePropertyId.AlignSelf:
							return true;
						default:
							if (id == StylePropertyId.Display)
							{
								return true;
							}
							break;
						}
						break;
					}
				}
				else
				{
					if (id == StylePropertyId.FlexDirection)
					{
						return true;
					}
					if (id == StylePropertyId.FlexWrap)
					{
						return true;
					}
					if (id == StylePropertyId.JustifyContent)
					{
						return true;
					}
				}
			}
			else if (id <= StylePropertyId.UnityOverflowClipBox)
			{
				if (id == StylePropertyId.Position)
				{
					return true;
				}
				if (id == StylePropertyId.TextOverflow)
				{
					return true;
				}
				if (id == StylePropertyId.UnityOverflowClipBox)
				{
					return true;
				}
			}
			else
			{
				if (id == StylePropertyId.UnityTextOverflowPosition)
				{
					return true;
				}
				switch (id)
				{
				case StylePropertyId.BackgroundImage:
					return true;
				case StylePropertyId.BackgroundPositionX:
					return true;
				case StylePropertyId.BackgroundPositionY:
					return true;
				case StylePropertyId.BackgroundRepeat:
					return true;
				default:
					if (id == StylePropertyId.Overflow)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}

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
			bool flag = string.IsNullOrEmpty(name);
			StylePropertyId stylePropertyId;
			if (flag)
			{
				stylePropertyId = StylePropertyId.Unknown;
			}
			else
			{
				StylePropertyId stylePropertyId2;
				bool flag2 = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId2);
				if (flag2)
				{
					stylePropertyId = stylePropertyId2;
				}
				else
				{
					stylePropertyId = StylePropertyId.Unknown;
				}
			}
			return stylePropertyId;
		}

		public static object GetComputedStyleValue(in ComputedStyle computedStyle, string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				StylePropertyId stylePropertyId;
				bool flag2 = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
				if (flag2)
				{
					bool flag3 = stylePropertyId == StylePropertyId.UnityBackgroundScaleMode;
					if (flag3)
					{
						ComputedStyle computedStyle2 = computedStyle;
						BackgroundPosition backgroundPositionX = computedStyle2.backgroundPositionX;
						computedStyle2 = computedStyle;
						BackgroundPosition backgroundPositionY = computedStyle2.backgroundPositionY;
						computedStyle2 = computedStyle;
						BackgroundRepeat backgroundRepeat = computedStyle2.backgroundRepeat;
						computedStyle2 = computedStyle;
						bool flag4;
						obj = BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(backgroundPositionX, backgroundPositionY, backgroundRepeat, computedStyle2.backgroundSize, out flag4);
					}
					else
					{
						obj = StyleDebug.GetComputedStyleValue(in computedStyle, stylePropertyId);
					}
				}
				else
				{
					obj = null;
				}
			}
			return obj;
		}

		public static object GetInlineStyleValue(IStyle style, string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				StylePropertyId stylePropertyId;
				bool flag2 = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
				if (flag2)
				{
					bool flag3 = stylePropertyId == StylePropertyId.UnityBackgroundScaleMode;
					if (flag3)
					{
						obj = style.unityBackgroundScaleMode;
					}
					else
					{
						obj = StyleDebug.GetInlineStyleValue(style, stylePropertyId);
					}
				}
				else
				{
					obj = null;
				}
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

		public static Type GetInlineStyleType(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			Type type;
			if (flag)
			{
				type = null;
			}
			else
			{
				StylePropertyId stylePropertyId;
				bool flag2 = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
				if (flag2)
				{
					bool flag3 = stylePropertyId == StylePropertyId.UnityBackgroundScaleMode;
					if (flag3)
					{
						return typeof(StyleEnum<ScaleMode>);
					}
					bool flag4 = !StyleDebug.IsShorthandProperty(stylePropertyId);
					if (flag4)
					{
						return StyleDebug.GetInlineStyleType(stylePropertyId);
					}
				}
				type = null;
			}
			return type;
		}

		public static Type GetComputedStyleType(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			Type type;
			if (flag)
			{
				type = null;
			}
			else
			{
				StylePropertyId stylePropertyId;
				bool flag2 = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
				if (flag2)
				{
					bool flag3 = stylePropertyId == StylePropertyId.UnityBackgroundScaleMode;
					if (flag3)
					{
						return typeof(ScaleMode);
					}
					bool flag4 = !StyleDebug.IsShorthandProperty(stylePropertyId);
					if (flag4)
					{
						return StyleDebug.GetComputedStyleType(stylePropertyId);
					}
				}
				type = null;
			}
			return type;
		}

		public static void FindSpecifiedStyles(in ComputedStyle computedStyle, IEnumerable<SelectorMatchRecord> matchRecords, Dictionary<StylePropertyId, int> result)
		{
			result.Clear();
			foreach (SelectorMatchRecord selectorMatchRecord in matchRecords)
			{
				int num = selectorMatchRecord.complexSelector.specificity;
				bool isDefaultStyleSheet = selectorMatchRecord.sheet.isDefaultStyleSheet;
				if (isDefaultStyleSheet)
				{
					num = -1;
				}
				StyleProperty[] properties = selectorMatchRecord.complexSelector.rule.properties;
				foreach (StyleProperty styleProperty in properties)
				{
					StylePropertyId stylePropertyId;
					bool flag = StylePropertyUtil.s_NameToId.TryGetValue(styleProperty.name, out stylePropertyId);
					if (flag)
					{
						bool flag2 = StyleDebug.IsShorthandProperty(stylePropertyId);
						if (flag2)
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
				bool flag3 = result.ContainsKey(stylePropertyId2);
				if (!flag3)
				{
					object computedStyleValue = StyleDebug.GetComputedStyleValue(in computedStyle, stylePropertyId2);
					object computedStyleValue2 = StyleDebug.GetComputedStyleValue(InitialStyle.Get(), stylePropertyId2);
					bool flag4 = computedStyleValue != null && !computedStyleValue.Equals(computedStyleValue2);
					if (flag4)
					{
						result[stylePropertyId2] = 2147483646;
					}
				}
			}
		}

		internal const int UnitySpecificity = -1;

		internal const int UndefinedSpecificity = 0;

		internal const int InheritedSpecificity = 2147483646;

		internal const int InlineSpecificity = 2147483647;
	}
}
