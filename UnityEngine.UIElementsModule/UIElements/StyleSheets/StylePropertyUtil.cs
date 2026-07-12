using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class StylePropertyUtil
	{
		public static bool TryGetEnumIntValue(StyleEnumType enumType, string value, out int intValue)
		{
			intValue = 0;
			switch (enumType)
			{
			case StyleEnumType.Align:
			{
				bool flag = string.Equals(value, "auto", StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					intValue = 0;
					return true;
				}
				bool flag2 = string.Equals(value, "flex-start", StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					intValue = 1;
					return true;
				}
				bool flag3 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
				if (flag3)
				{
					intValue = 2;
					return true;
				}
				bool flag4 = string.Equals(value, "flex-end", StringComparison.OrdinalIgnoreCase);
				if (flag4)
				{
					intValue = 3;
					return true;
				}
				bool flag5 = string.Equals(value, "stretch", StringComparison.OrdinalIgnoreCase);
				if (flag5)
				{
					intValue = 4;
					return true;
				}
				break;
			}
			case StyleEnumType.BackgroundPositionKeyword:
			{
				bool flag6 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
				if (flag6)
				{
					intValue = 0;
					return true;
				}
				bool flag7 = string.Equals(value, "top", StringComparison.OrdinalIgnoreCase);
				if (flag7)
				{
					intValue = 1;
					return true;
				}
				bool flag8 = string.Equals(value, "bottom", StringComparison.OrdinalIgnoreCase);
				if (flag8)
				{
					intValue = 2;
					return true;
				}
				bool flag9 = string.Equals(value, "left", StringComparison.OrdinalIgnoreCase);
				if (flag9)
				{
					intValue = 3;
					return true;
				}
				bool flag10 = string.Equals(value, "right", StringComparison.OrdinalIgnoreCase);
				if (flag10)
				{
					intValue = 4;
					return true;
				}
				break;
			}
			case StyleEnumType.BackgroundSizeType:
			{
				bool flag11 = string.Equals(value, "cover", StringComparison.OrdinalIgnoreCase);
				if (flag11)
				{
					intValue = 1;
					return true;
				}
				bool flag12 = string.Equals(value, "contain", StringComparison.OrdinalIgnoreCase);
				if (flag12)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.DisplayStyle:
			{
				bool flag13 = string.Equals(value, "flex", StringComparison.OrdinalIgnoreCase);
				if (flag13)
				{
					intValue = 0;
					return true;
				}
				bool flag14 = string.Equals(value, "none", StringComparison.OrdinalIgnoreCase);
				if (flag14)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.EasingMode:
			{
				bool flag15 = string.Equals(value, "ease", StringComparison.OrdinalIgnoreCase);
				if (flag15)
				{
					intValue = 0;
					return true;
				}
				bool flag16 = string.Equals(value, "ease-in", StringComparison.OrdinalIgnoreCase);
				if (flag16)
				{
					intValue = 1;
					return true;
				}
				bool flag17 = string.Equals(value, "ease-out", StringComparison.OrdinalIgnoreCase);
				if (flag17)
				{
					intValue = 2;
					return true;
				}
				bool flag18 = string.Equals(value, "ease-in-out", StringComparison.OrdinalIgnoreCase);
				if (flag18)
				{
					intValue = 3;
					return true;
				}
				bool flag19 = string.Equals(value, "linear", StringComparison.OrdinalIgnoreCase);
				if (flag19)
				{
					intValue = 4;
					return true;
				}
				bool flag20 = string.Equals(value, "ease-in-sine", StringComparison.OrdinalIgnoreCase);
				if (flag20)
				{
					intValue = 5;
					return true;
				}
				bool flag21 = string.Equals(value, "ease-out-sine", StringComparison.OrdinalIgnoreCase);
				if (flag21)
				{
					intValue = 6;
					return true;
				}
				bool flag22 = string.Equals(value, "ease-in-out-sine", StringComparison.OrdinalIgnoreCase);
				if (flag22)
				{
					intValue = 7;
					return true;
				}
				bool flag23 = string.Equals(value, "ease-in-cubic", StringComparison.OrdinalIgnoreCase);
				if (flag23)
				{
					intValue = 8;
					return true;
				}
				bool flag24 = string.Equals(value, "ease-out-cubic", StringComparison.OrdinalIgnoreCase);
				if (flag24)
				{
					intValue = 9;
					return true;
				}
				bool flag25 = string.Equals(value, "ease-in-out-cubic", StringComparison.OrdinalIgnoreCase);
				if (flag25)
				{
					intValue = 10;
					return true;
				}
				bool flag26 = string.Equals(value, "ease-in-circ", StringComparison.OrdinalIgnoreCase);
				if (flag26)
				{
					intValue = 11;
					return true;
				}
				bool flag27 = string.Equals(value, "ease-out-circ", StringComparison.OrdinalIgnoreCase);
				if (flag27)
				{
					intValue = 12;
					return true;
				}
				bool flag28 = string.Equals(value, "ease-in-out-circ", StringComparison.OrdinalIgnoreCase);
				if (flag28)
				{
					intValue = 13;
					return true;
				}
				bool flag29 = string.Equals(value, "ease-in-elastic", StringComparison.OrdinalIgnoreCase);
				if (flag29)
				{
					intValue = 14;
					return true;
				}
				bool flag30 = string.Equals(value, "ease-out-elastic", StringComparison.OrdinalIgnoreCase);
				if (flag30)
				{
					intValue = 15;
					return true;
				}
				bool flag31 = string.Equals(value, "ease-in-out-elastic", StringComparison.OrdinalIgnoreCase);
				if (flag31)
				{
					intValue = 16;
					return true;
				}
				bool flag32 = string.Equals(value, "ease-in-back", StringComparison.OrdinalIgnoreCase);
				if (flag32)
				{
					intValue = 17;
					return true;
				}
				bool flag33 = string.Equals(value, "ease-out-back", StringComparison.OrdinalIgnoreCase);
				if (flag33)
				{
					intValue = 18;
					return true;
				}
				bool flag34 = string.Equals(value, "ease-in-out-back", StringComparison.OrdinalIgnoreCase);
				if (flag34)
				{
					intValue = 19;
					return true;
				}
				bool flag35 = string.Equals(value, "ease-in-bounce", StringComparison.OrdinalIgnoreCase);
				if (flag35)
				{
					intValue = 20;
					return true;
				}
				bool flag36 = string.Equals(value, "ease-out-bounce", StringComparison.OrdinalIgnoreCase);
				if (flag36)
				{
					intValue = 21;
					return true;
				}
				bool flag37 = string.Equals(value, "ease-in-out-bounce", StringComparison.OrdinalIgnoreCase);
				if (flag37)
				{
					intValue = 22;
					return true;
				}
				break;
			}
			case StyleEnumType.FlexDirection:
			{
				bool flag38 = string.Equals(value, "column", StringComparison.OrdinalIgnoreCase);
				if (flag38)
				{
					intValue = 0;
					return true;
				}
				bool flag39 = string.Equals(value, "column-reverse", StringComparison.OrdinalIgnoreCase);
				if (flag39)
				{
					intValue = 1;
					return true;
				}
				bool flag40 = string.Equals(value, "row", StringComparison.OrdinalIgnoreCase);
				if (flag40)
				{
					intValue = 2;
					return true;
				}
				bool flag41 = string.Equals(value, "row-reverse", StringComparison.OrdinalIgnoreCase);
				if (flag41)
				{
					intValue = 3;
					return true;
				}
				break;
			}
			case StyleEnumType.FontStyle:
			{
				bool flag42 = string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase);
				if (flag42)
				{
					intValue = 0;
					return true;
				}
				bool flag43 = string.Equals(value, "bold", StringComparison.OrdinalIgnoreCase);
				if (flag43)
				{
					intValue = 1;
					return true;
				}
				bool flag44 = string.Equals(value, "italic", StringComparison.OrdinalIgnoreCase);
				if (flag44)
				{
					intValue = 2;
					return true;
				}
				bool flag45 = string.Equals(value, "bold-and-italic", StringComparison.OrdinalIgnoreCase);
				if (flag45)
				{
					intValue = 3;
					return true;
				}
				break;
			}
			case StyleEnumType.Justify:
			{
				bool flag46 = string.Equals(value, "flex-start", StringComparison.OrdinalIgnoreCase);
				if (flag46)
				{
					intValue = 0;
					return true;
				}
				bool flag47 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
				if (flag47)
				{
					intValue = 1;
					return true;
				}
				bool flag48 = string.Equals(value, "flex-end", StringComparison.OrdinalIgnoreCase);
				if (flag48)
				{
					intValue = 2;
					return true;
				}
				bool flag49 = string.Equals(value, "space-between", StringComparison.OrdinalIgnoreCase);
				if (flag49)
				{
					intValue = 3;
					return true;
				}
				bool flag50 = string.Equals(value, "space-around", StringComparison.OrdinalIgnoreCase);
				if (flag50)
				{
					intValue = 4;
					return true;
				}
				break;
			}
			case StyleEnumType.Overflow:
			{
				bool flag51 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag51)
				{
					intValue = 0;
					return true;
				}
				bool flag52 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
				if (flag52)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.OverflowClipBox:
			{
				bool flag53 = string.Equals(value, "padding-box", StringComparison.OrdinalIgnoreCase);
				if (flag53)
				{
					intValue = 0;
					return true;
				}
				bool flag54 = string.Equals(value, "content-box", StringComparison.OrdinalIgnoreCase);
				if (flag54)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.OverflowInternal:
			{
				bool flag55 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag55)
				{
					intValue = 0;
					return true;
				}
				bool flag56 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
				if (flag56)
				{
					intValue = 1;
					return true;
				}
				bool flag57 = string.Equals(value, "scroll", StringComparison.OrdinalIgnoreCase);
				if (flag57)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.Position:
			{
				bool flag58 = string.Equals(value, "relative", StringComparison.OrdinalIgnoreCase);
				if (flag58)
				{
					intValue = 0;
					return true;
				}
				bool flag59 = string.Equals(value, "absolute", StringComparison.OrdinalIgnoreCase);
				if (flag59)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.Repeat:
			{
				bool flag60 = string.Equals(value, "no-repeat", StringComparison.OrdinalIgnoreCase);
				if (flag60)
				{
					intValue = 0;
					return true;
				}
				bool flag61 = string.Equals(value, "repeat", StringComparison.OrdinalIgnoreCase);
				if (flag61)
				{
					intValue = 3;
					return true;
				}
				bool flag62 = string.Equals(value, "space", StringComparison.OrdinalIgnoreCase);
				if (flag62)
				{
					intValue = 1;
					return true;
				}
				bool flag63 = string.Equals(value, "round", StringComparison.OrdinalIgnoreCase);
				if (flag63)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.RepeatXY:
			{
				bool flag64 = string.Equals(value, "repeat-x", StringComparison.OrdinalIgnoreCase);
				if (flag64)
				{
					intValue = 0;
					return true;
				}
				bool flag65 = string.Equals(value, "repeat-y", StringComparison.OrdinalIgnoreCase);
				if (flag65)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.ScaleMode:
			{
				bool flag66 = string.Equals(value, "stretch-to-fill", StringComparison.OrdinalIgnoreCase);
				if (flag66)
				{
					intValue = 0;
					return true;
				}
				bool flag67 = string.Equals(value, "scale-and-crop", StringComparison.OrdinalIgnoreCase);
				if (flag67)
				{
					intValue = 1;
					return true;
				}
				bool flag68 = string.Equals(value, "scale-to-fit", StringComparison.OrdinalIgnoreCase);
				if (flag68)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.TextAnchor:
			{
				bool flag69 = string.Equals(value, "upper-left", StringComparison.OrdinalIgnoreCase);
				if (flag69)
				{
					intValue = 0;
					return true;
				}
				bool flag70 = string.Equals(value, "upper-center", StringComparison.OrdinalIgnoreCase);
				if (flag70)
				{
					intValue = 1;
					return true;
				}
				bool flag71 = string.Equals(value, "upper-right", StringComparison.OrdinalIgnoreCase);
				if (flag71)
				{
					intValue = 2;
					return true;
				}
				bool flag72 = string.Equals(value, "middle-left", StringComparison.OrdinalIgnoreCase);
				if (flag72)
				{
					intValue = 3;
					return true;
				}
				bool flag73 = string.Equals(value, "middle-center", StringComparison.OrdinalIgnoreCase);
				if (flag73)
				{
					intValue = 4;
					return true;
				}
				bool flag74 = string.Equals(value, "middle-right", StringComparison.OrdinalIgnoreCase);
				if (flag74)
				{
					intValue = 5;
					return true;
				}
				bool flag75 = string.Equals(value, "lower-left", StringComparison.OrdinalIgnoreCase);
				if (flag75)
				{
					intValue = 6;
					return true;
				}
				bool flag76 = string.Equals(value, "lower-center", StringComparison.OrdinalIgnoreCase);
				if (flag76)
				{
					intValue = 7;
					return true;
				}
				bool flag77 = string.Equals(value, "lower-right", StringComparison.OrdinalIgnoreCase);
				if (flag77)
				{
					intValue = 8;
					return true;
				}
				break;
			}
			case StyleEnumType.TextOverflow:
			{
				bool flag78 = string.Equals(value, "clip", StringComparison.OrdinalIgnoreCase);
				if (flag78)
				{
					intValue = 0;
					return true;
				}
				bool flag79 = string.Equals(value, "ellipsis", StringComparison.OrdinalIgnoreCase);
				if (flag79)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.TextOverflowPosition:
			{
				bool flag80 = string.Equals(value, "start", StringComparison.OrdinalIgnoreCase);
				if (flag80)
				{
					intValue = 1;
					return true;
				}
				bool flag81 = string.Equals(value, "middle", StringComparison.OrdinalIgnoreCase);
				if (flag81)
				{
					intValue = 2;
					return true;
				}
				bool flag82 = string.Equals(value, "end", StringComparison.OrdinalIgnoreCase);
				if (flag82)
				{
					intValue = 0;
					return true;
				}
				break;
			}
			case StyleEnumType.TransformOriginOffset:
			{
				bool flag83 = string.Equals(value, "left", StringComparison.OrdinalIgnoreCase);
				if (flag83)
				{
					intValue = 1;
					return true;
				}
				bool flag84 = string.Equals(value, "right", StringComparison.OrdinalIgnoreCase);
				if (flag84)
				{
					intValue = 2;
					return true;
				}
				bool flag85 = string.Equals(value, "top", StringComparison.OrdinalIgnoreCase);
				if (flag85)
				{
					intValue = 3;
					return true;
				}
				bool flag86 = string.Equals(value, "bottom", StringComparison.OrdinalIgnoreCase);
				if (flag86)
				{
					intValue = 4;
					return true;
				}
				bool flag87 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
				if (flag87)
				{
					intValue = 5;
					return true;
				}
				break;
			}
			case StyleEnumType.Visibility:
			{
				bool flag88 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag88)
				{
					intValue = 0;
					return true;
				}
				bool flag89 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
				if (flag89)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.WhiteSpace:
			{
				bool flag90 = string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase);
				if (flag90)
				{
					intValue = 0;
					return true;
				}
				bool flag91 = string.Equals(value, "nowrap", StringComparison.OrdinalIgnoreCase);
				if (flag91)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.Wrap:
			{
				bool flag92 = string.Equals(value, "nowrap", StringComparison.OrdinalIgnoreCase);
				if (flag92)
				{
					intValue = 0;
					return true;
				}
				bool flag93 = string.Equals(value, "wrap", StringComparison.OrdinalIgnoreCase);
				if (flag93)
				{
					intValue = 1;
					return true;
				}
				bool flag94 = string.Equals(value, "wrap-reverse", StringComparison.OrdinalIgnoreCase);
				if (flag94)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			}
			return false;
		}

		public static bool IsMatchingShorthand(StylePropertyId shorthand, StylePropertyId id)
		{
			switch (shorthand)
			{
			case StylePropertyId.All:
				return true;
			case StylePropertyId.BackgroundPosition:
				return id == StylePropertyId.BackgroundPositionX || id == StylePropertyId.BackgroundPositionY;
			case StylePropertyId.BorderColor:
				return id == StylePropertyId.BorderTopColor || id == StylePropertyId.BorderRightColor || id == StylePropertyId.BorderBottomColor || id == StylePropertyId.BorderLeftColor;
			case StylePropertyId.BorderRadius:
				return id == StylePropertyId.BorderTopLeftRadius || id == StylePropertyId.BorderTopRightRadius || id == StylePropertyId.BorderBottomRightRadius || id == StylePropertyId.BorderBottomLeftRadius;
			case StylePropertyId.BorderWidth:
				return id == StylePropertyId.BorderTopWidth || id == StylePropertyId.BorderRightWidth || id == StylePropertyId.BorderBottomWidth || id == StylePropertyId.BorderLeftWidth;
			case StylePropertyId.Flex:
				return id == StylePropertyId.FlexGrow || id == StylePropertyId.FlexShrink || id == StylePropertyId.FlexBasis;
			case StylePropertyId.Margin:
				return id == StylePropertyId.MarginTop || id == StylePropertyId.MarginRight || id == StylePropertyId.MarginBottom || id == StylePropertyId.MarginLeft;
			case StylePropertyId.Padding:
				return id == StylePropertyId.PaddingTop || id == StylePropertyId.PaddingRight || id == StylePropertyId.PaddingBottom || id == StylePropertyId.PaddingLeft;
			case StylePropertyId.UnityBackgroundScaleMode:
				return id == StylePropertyId.BackgroundPositionX || id == StylePropertyId.BackgroundPositionY || id == StylePropertyId.BackgroundRepeat || id == StylePropertyId.BackgroundSize;
			case StylePropertyId.UnityTextOutline:
				return id == StylePropertyId.UnityTextOutlineColor || id == StylePropertyId.UnityTextOutlineWidth;
			}
			return false;
		}

		public static IEnumerable<Type> GetAllowedAssetTypesForProperty(StylePropertyId id)
		{
			if (id <= StylePropertyId.UnityFont)
			{
				if (id == StylePropertyId.Custom)
				{
					return new Type[] { typeof(Object) };
				}
				if (id == StylePropertyId.UnityFont)
				{
					return new Type[] { typeof(Font) };
				}
			}
			else
			{
				if (id == StylePropertyId.UnityFontDefinition)
				{
					return FontDefinition.allowedAssetTypes;
				}
				if (id == StylePropertyId.Cursor)
				{
					return Cursor.allowedAssetTypes;
				}
				if (id == StylePropertyId.BackgroundImage)
				{
					return Background.allowedAssetTypes;
				}
			}
			return Enumerable.Empty<Type>();
		}

		public static bool IsAnimatable(StylePropertyId id)
		{
			return StylePropertyUtil.s_AnimatableProperties.Contains(id);
		}

		public static IEnumerable<StylePropertyId> AllPropertyIds()
		{
			return StylePropertyUtil.s_IdToName.Keys;
		}

		public const int k_GroupOffset = 16;

		internal static readonly Dictionary<string, StylePropertyId> s_NameToId = new Dictionary<string, StylePropertyId>
		{
			{
				"align-content",
				StylePropertyId.AlignContent
			},
			{
				"align-items",
				StylePropertyId.AlignItems
			},
			{
				"align-self",
				StylePropertyId.AlignSelf
			},
			{
				"all",
				StylePropertyId.All
			},
			{
				"background-color",
				StylePropertyId.BackgroundColor
			},
			{
				"background-image",
				StylePropertyId.BackgroundImage
			},
			{
				"background-position",
				StylePropertyId.BackgroundPosition
			},
			{
				"background-position-x",
				StylePropertyId.BackgroundPositionX
			},
			{
				"background-position-y",
				StylePropertyId.BackgroundPositionY
			},
			{
				"background-repeat",
				StylePropertyId.BackgroundRepeat
			},
			{
				"background-size",
				StylePropertyId.BackgroundSize
			},
			{
				"border-bottom-color",
				StylePropertyId.BorderBottomColor
			},
			{
				"border-bottom-left-radius",
				StylePropertyId.BorderBottomLeftRadius
			},
			{
				"border-bottom-right-radius",
				StylePropertyId.BorderBottomRightRadius
			},
			{
				"border-bottom-width",
				StylePropertyId.BorderBottomWidth
			},
			{
				"border-color",
				StylePropertyId.BorderColor
			},
			{
				"border-left-color",
				StylePropertyId.BorderLeftColor
			},
			{
				"border-left-width",
				StylePropertyId.BorderLeftWidth
			},
			{
				"border-radius",
				StylePropertyId.BorderRadius
			},
			{
				"border-right-color",
				StylePropertyId.BorderRightColor
			},
			{
				"border-right-width",
				StylePropertyId.BorderRightWidth
			},
			{
				"border-top-color",
				StylePropertyId.BorderTopColor
			},
			{
				"border-top-left-radius",
				StylePropertyId.BorderTopLeftRadius
			},
			{
				"border-top-right-radius",
				StylePropertyId.BorderTopRightRadius
			},
			{
				"border-top-width",
				StylePropertyId.BorderTopWidth
			},
			{
				"border-width",
				StylePropertyId.BorderWidth
			},
			{
				"bottom",
				StylePropertyId.Bottom
			},
			{
				"color",
				StylePropertyId.Color
			},
			{
				"cursor",
				StylePropertyId.Cursor
			},
			{
				"display",
				StylePropertyId.Display
			},
			{
				"flex",
				StylePropertyId.Flex
			},
			{
				"flex-basis",
				StylePropertyId.FlexBasis
			},
			{
				"flex-direction",
				StylePropertyId.FlexDirection
			},
			{
				"flex-grow",
				StylePropertyId.FlexGrow
			},
			{
				"flex-shrink",
				StylePropertyId.FlexShrink
			},
			{
				"flex-wrap",
				StylePropertyId.FlexWrap
			},
			{
				"font-size",
				StylePropertyId.FontSize
			},
			{
				"height",
				StylePropertyId.Height
			},
			{
				"justify-content",
				StylePropertyId.JustifyContent
			},
			{
				"left",
				StylePropertyId.Left
			},
			{
				"letter-spacing",
				StylePropertyId.LetterSpacing
			},
			{
				"margin",
				StylePropertyId.Margin
			},
			{
				"margin-bottom",
				StylePropertyId.MarginBottom
			},
			{
				"margin-left",
				StylePropertyId.MarginLeft
			},
			{
				"margin-right",
				StylePropertyId.MarginRight
			},
			{
				"margin-top",
				StylePropertyId.MarginTop
			},
			{
				"max-height",
				StylePropertyId.MaxHeight
			},
			{
				"max-width",
				StylePropertyId.MaxWidth
			},
			{
				"min-height",
				StylePropertyId.MinHeight
			},
			{
				"min-width",
				StylePropertyId.MinWidth
			},
			{
				"opacity",
				StylePropertyId.Opacity
			},
			{
				"overflow",
				StylePropertyId.Overflow
			},
			{
				"padding",
				StylePropertyId.Padding
			},
			{
				"padding-bottom",
				StylePropertyId.PaddingBottom
			},
			{
				"padding-left",
				StylePropertyId.PaddingLeft
			},
			{
				"padding-right",
				StylePropertyId.PaddingRight
			},
			{
				"padding-top",
				StylePropertyId.PaddingTop
			},
			{
				"position",
				StylePropertyId.Position
			},
			{
				"right",
				StylePropertyId.Right
			},
			{
				"rotate",
				StylePropertyId.Rotate
			},
			{
				"scale",
				StylePropertyId.Scale
			},
			{
				"text-overflow",
				StylePropertyId.TextOverflow
			},
			{
				"text-shadow",
				StylePropertyId.TextShadow
			},
			{
				"top",
				StylePropertyId.Top
			},
			{
				"transform-origin",
				StylePropertyId.TransformOrigin
			},
			{
				"transition",
				StylePropertyId.Transition
			},
			{
				"transition-delay",
				StylePropertyId.TransitionDelay
			},
			{
				"transition-duration",
				StylePropertyId.TransitionDuration
			},
			{
				"transition-property",
				StylePropertyId.TransitionProperty
			},
			{
				"transition-timing-function",
				StylePropertyId.TransitionTimingFunction
			},
			{
				"translate",
				StylePropertyId.Translate
			},
			{
				"-unity-background-image-tint-color",
				StylePropertyId.UnityBackgroundImageTintColor
			},
			{
				"-unity-background-scale-mode",
				StylePropertyId.UnityBackgroundScaleMode
			},
			{
				"-unity-font",
				StylePropertyId.UnityFont
			},
			{
				"-unity-font-definition",
				StylePropertyId.UnityFontDefinition
			},
			{
				"-unity-font-style",
				StylePropertyId.UnityFontStyleAndWeight
			},
			{
				"-unity-overflow-clip-box",
				StylePropertyId.UnityOverflowClipBox
			},
			{
				"-unity-paragraph-spacing",
				StylePropertyId.UnityParagraphSpacing
			},
			{
				"-unity-slice-bottom",
				StylePropertyId.UnitySliceBottom
			},
			{
				"-unity-slice-left",
				StylePropertyId.UnitySliceLeft
			},
			{
				"-unity-slice-right",
				StylePropertyId.UnitySliceRight
			},
			{
				"-unity-slice-scale",
				StylePropertyId.UnitySliceScale
			},
			{
				"-unity-slice-top",
				StylePropertyId.UnitySliceTop
			},
			{
				"-unity-text-align",
				StylePropertyId.UnityTextAlign
			},
			{
				"-unity-text-outline",
				StylePropertyId.UnityTextOutline
			},
			{
				"-unity-text-outline-color",
				StylePropertyId.UnityTextOutlineColor
			},
			{
				"-unity-text-outline-width",
				StylePropertyId.UnityTextOutlineWidth
			},
			{
				"-unity-text-overflow-position",
				StylePropertyId.UnityTextOverflowPosition
			},
			{
				"visibility",
				StylePropertyId.Visibility
			},
			{
				"white-space",
				StylePropertyId.WhiteSpace
			},
			{
				"width",
				StylePropertyId.Width
			},
			{
				"word-spacing",
				StylePropertyId.WordSpacing
			}
		};

		internal static readonly Dictionary<StylePropertyId, string> s_IdToName = new Dictionary<StylePropertyId, string>
		{
			{
				StylePropertyId.AlignContent,
				"align-content"
			},
			{
				StylePropertyId.AlignItems,
				"align-items"
			},
			{
				StylePropertyId.AlignSelf,
				"align-self"
			},
			{
				StylePropertyId.All,
				"all"
			},
			{
				StylePropertyId.BackgroundColor,
				"background-color"
			},
			{
				StylePropertyId.BackgroundImage,
				"background-image"
			},
			{
				StylePropertyId.BackgroundPosition,
				"background-position"
			},
			{
				StylePropertyId.BackgroundPositionX,
				"background-position-x"
			},
			{
				StylePropertyId.BackgroundPositionY,
				"background-position-y"
			},
			{
				StylePropertyId.BackgroundRepeat,
				"background-repeat"
			},
			{
				StylePropertyId.BackgroundSize,
				"background-size"
			},
			{
				StylePropertyId.BorderBottomColor,
				"border-bottom-color"
			},
			{
				StylePropertyId.BorderBottomLeftRadius,
				"border-bottom-left-radius"
			},
			{
				StylePropertyId.BorderBottomRightRadius,
				"border-bottom-right-radius"
			},
			{
				StylePropertyId.BorderBottomWidth,
				"border-bottom-width"
			},
			{
				StylePropertyId.BorderColor,
				"border-color"
			},
			{
				StylePropertyId.BorderLeftColor,
				"border-left-color"
			},
			{
				StylePropertyId.BorderLeftWidth,
				"border-left-width"
			},
			{
				StylePropertyId.BorderRadius,
				"border-radius"
			},
			{
				StylePropertyId.BorderRightColor,
				"border-right-color"
			},
			{
				StylePropertyId.BorderRightWidth,
				"border-right-width"
			},
			{
				StylePropertyId.BorderTopColor,
				"border-top-color"
			},
			{
				StylePropertyId.BorderTopLeftRadius,
				"border-top-left-radius"
			},
			{
				StylePropertyId.BorderTopRightRadius,
				"border-top-right-radius"
			},
			{
				StylePropertyId.BorderTopWidth,
				"border-top-width"
			},
			{
				StylePropertyId.BorderWidth,
				"border-width"
			},
			{
				StylePropertyId.Bottom,
				"bottom"
			},
			{
				StylePropertyId.Color,
				"color"
			},
			{
				StylePropertyId.Cursor,
				"cursor"
			},
			{
				StylePropertyId.Display,
				"display"
			},
			{
				StylePropertyId.Flex,
				"flex"
			},
			{
				StylePropertyId.FlexBasis,
				"flex-basis"
			},
			{
				StylePropertyId.FlexDirection,
				"flex-direction"
			},
			{
				StylePropertyId.FlexGrow,
				"flex-grow"
			},
			{
				StylePropertyId.FlexShrink,
				"flex-shrink"
			},
			{
				StylePropertyId.FlexWrap,
				"flex-wrap"
			},
			{
				StylePropertyId.FontSize,
				"font-size"
			},
			{
				StylePropertyId.Height,
				"height"
			},
			{
				StylePropertyId.JustifyContent,
				"justify-content"
			},
			{
				StylePropertyId.Left,
				"left"
			},
			{
				StylePropertyId.LetterSpacing,
				"letter-spacing"
			},
			{
				StylePropertyId.Margin,
				"margin"
			},
			{
				StylePropertyId.MarginBottom,
				"margin-bottom"
			},
			{
				StylePropertyId.MarginLeft,
				"margin-left"
			},
			{
				StylePropertyId.MarginRight,
				"margin-right"
			},
			{
				StylePropertyId.MarginTop,
				"margin-top"
			},
			{
				StylePropertyId.MaxHeight,
				"max-height"
			},
			{
				StylePropertyId.MaxWidth,
				"max-width"
			},
			{
				StylePropertyId.MinHeight,
				"min-height"
			},
			{
				StylePropertyId.MinWidth,
				"min-width"
			},
			{
				StylePropertyId.Opacity,
				"opacity"
			},
			{
				StylePropertyId.Overflow,
				"overflow"
			},
			{
				StylePropertyId.Padding,
				"padding"
			},
			{
				StylePropertyId.PaddingBottom,
				"padding-bottom"
			},
			{
				StylePropertyId.PaddingLeft,
				"padding-left"
			},
			{
				StylePropertyId.PaddingRight,
				"padding-right"
			},
			{
				StylePropertyId.PaddingTop,
				"padding-top"
			},
			{
				StylePropertyId.Position,
				"position"
			},
			{
				StylePropertyId.Right,
				"right"
			},
			{
				StylePropertyId.Rotate,
				"rotate"
			},
			{
				StylePropertyId.Scale,
				"scale"
			},
			{
				StylePropertyId.TextOverflow,
				"text-overflow"
			},
			{
				StylePropertyId.TextShadow,
				"text-shadow"
			},
			{
				StylePropertyId.Top,
				"top"
			},
			{
				StylePropertyId.TransformOrigin,
				"transform-origin"
			},
			{
				StylePropertyId.Transition,
				"transition"
			},
			{
				StylePropertyId.TransitionDelay,
				"transition-delay"
			},
			{
				StylePropertyId.TransitionDuration,
				"transition-duration"
			},
			{
				StylePropertyId.TransitionProperty,
				"transition-property"
			},
			{
				StylePropertyId.TransitionTimingFunction,
				"transition-timing-function"
			},
			{
				StylePropertyId.Translate,
				"translate"
			},
			{
				StylePropertyId.UnityBackgroundImageTintColor,
				"-unity-background-image-tint-color"
			},
			{
				StylePropertyId.UnityBackgroundScaleMode,
				"-unity-background-scale-mode"
			},
			{
				StylePropertyId.UnityFont,
				"-unity-font"
			},
			{
				StylePropertyId.UnityFontDefinition,
				"-unity-font-definition"
			},
			{
				StylePropertyId.UnityFontStyleAndWeight,
				"-unity-font-style"
			},
			{
				StylePropertyId.UnityOverflowClipBox,
				"-unity-overflow-clip-box"
			},
			{
				StylePropertyId.UnityParagraphSpacing,
				"-unity-paragraph-spacing"
			},
			{
				StylePropertyId.UnitySliceBottom,
				"-unity-slice-bottom"
			},
			{
				StylePropertyId.UnitySliceLeft,
				"-unity-slice-left"
			},
			{
				StylePropertyId.UnitySliceRight,
				"-unity-slice-right"
			},
			{
				StylePropertyId.UnitySliceScale,
				"-unity-slice-scale"
			},
			{
				StylePropertyId.UnitySliceTop,
				"-unity-slice-top"
			},
			{
				StylePropertyId.UnityTextAlign,
				"-unity-text-align"
			},
			{
				StylePropertyId.UnityTextOutline,
				"-unity-text-outline"
			},
			{
				StylePropertyId.UnityTextOutlineColor,
				"-unity-text-outline-color"
			},
			{
				StylePropertyId.UnityTextOutlineWidth,
				"-unity-text-outline-width"
			},
			{
				StylePropertyId.UnityTextOverflowPosition,
				"-unity-text-overflow-position"
			},
			{
				StylePropertyId.Visibility,
				"visibility"
			},
			{
				StylePropertyId.WhiteSpace,
				"white-space"
			},
			{
				StylePropertyId.Width,
				"width"
			},
			{
				StylePropertyId.WordSpacing,
				"word-spacing"
			}
		};

		internal static readonly HashSet<StylePropertyId> s_AnimatableProperties = new HashSet<StylePropertyId>
		{
			StylePropertyId.AlignContent,
			StylePropertyId.AlignItems,
			StylePropertyId.AlignSelf,
			StylePropertyId.All,
			StylePropertyId.BackgroundColor,
			StylePropertyId.BackgroundImage,
			StylePropertyId.BackgroundPosition,
			StylePropertyId.BackgroundPositionX,
			StylePropertyId.BackgroundPositionY,
			StylePropertyId.BackgroundRepeat,
			StylePropertyId.BackgroundSize,
			StylePropertyId.BorderBottomColor,
			StylePropertyId.BorderBottomLeftRadius,
			StylePropertyId.BorderBottomRightRadius,
			StylePropertyId.BorderBottomWidth,
			StylePropertyId.BorderColor,
			StylePropertyId.BorderLeftColor,
			StylePropertyId.BorderLeftWidth,
			StylePropertyId.BorderRadius,
			StylePropertyId.BorderRightColor,
			StylePropertyId.BorderRightWidth,
			StylePropertyId.BorderTopColor,
			StylePropertyId.BorderTopLeftRadius,
			StylePropertyId.BorderTopRightRadius,
			StylePropertyId.BorderTopWidth,
			StylePropertyId.BorderWidth,
			StylePropertyId.Bottom,
			StylePropertyId.Color,
			StylePropertyId.Display,
			StylePropertyId.Flex,
			StylePropertyId.FlexBasis,
			StylePropertyId.FlexDirection,
			StylePropertyId.FlexGrow,
			StylePropertyId.FlexShrink,
			StylePropertyId.FlexWrap,
			StylePropertyId.FontSize,
			StylePropertyId.Height,
			StylePropertyId.JustifyContent,
			StylePropertyId.Left,
			StylePropertyId.LetterSpacing,
			StylePropertyId.Margin,
			StylePropertyId.MarginBottom,
			StylePropertyId.MarginLeft,
			StylePropertyId.MarginRight,
			StylePropertyId.MarginTop,
			StylePropertyId.MaxHeight,
			StylePropertyId.MaxWidth,
			StylePropertyId.MinHeight,
			StylePropertyId.MinWidth,
			StylePropertyId.Opacity,
			StylePropertyId.Overflow,
			StylePropertyId.Padding,
			StylePropertyId.PaddingBottom,
			StylePropertyId.PaddingLeft,
			StylePropertyId.PaddingRight,
			StylePropertyId.PaddingTop,
			StylePropertyId.Position,
			StylePropertyId.Right,
			StylePropertyId.Rotate,
			StylePropertyId.Scale,
			StylePropertyId.TextOverflow,
			StylePropertyId.TextShadow,
			StylePropertyId.Top,
			StylePropertyId.TransformOrigin,
			StylePropertyId.Translate,
			StylePropertyId.UnityBackgroundImageTintColor,
			StylePropertyId.UnityBackgroundScaleMode,
			StylePropertyId.UnityFont,
			StylePropertyId.UnityFontDefinition,
			StylePropertyId.UnityFontStyleAndWeight,
			StylePropertyId.UnityOverflowClipBox,
			StylePropertyId.UnityParagraphSpacing,
			StylePropertyId.UnitySliceBottom,
			StylePropertyId.UnitySliceLeft,
			StylePropertyId.UnitySliceRight,
			StylePropertyId.UnitySliceScale,
			StylePropertyId.UnitySliceTop,
			StylePropertyId.UnityTextAlign,
			StylePropertyId.UnityTextOutline,
			StylePropertyId.UnityTextOutlineColor,
			StylePropertyId.UnityTextOutlineWidth,
			StylePropertyId.UnityTextOverflowPosition,
			StylePropertyId.Visibility,
			StylePropertyId.WhiteSpace,
			StylePropertyId.Width,
			StylePropertyId.WordSpacing
		};

		internal static readonly Dictionary<StylePropertyId, UsageHints> s_AnimatableWithUsageHintProperties = new Dictionary<StylePropertyId, UsageHints>
		{
			{
				StylePropertyId.BackgroundColor,
				UsageHints.DynamicColor
			},
			{
				StylePropertyId.BorderBottomColor,
				UsageHints.DynamicColor
			},
			{
				StylePropertyId.BorderColor,
				UsageHints.DynamicColor
			},
			{
				StylePropertyId.BorderLeftColor,
				UsageHints.DynamicColor
			},
			{
				StylePropertyId.BorderRightColor,
				UsageHints.DynamicColor
			},
			{
				StylePropertyId.BorderTopColor,
				UsageHints.DynamicColor
			},
			{
				StylePropertyId.Color,
				UsageHints.DynamicColor
			},
			{
				StylePropertyId.Rotate,
				UsageHints.DynamicTransform
			},
			{
				StylePropertyId.Scale,
				UsageHints.DynamicTransform
			},
			{
				StylePropertyId.TransformOrigin,
				UsageHints.DynamicTransform
			},
			{
				StylePropertyId.Translate,
				UsageHints.DynamicTransform
			},
			{
				StylePropertyId.UnityBackgroundImageTintColor,
				UsageHints.DynamicColor
			}
		};
	}
}
