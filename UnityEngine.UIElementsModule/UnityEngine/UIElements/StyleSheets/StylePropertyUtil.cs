using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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
			case StyleEnumType.Axis:
			{
				bool flag6 = string.Equals(value, "x", StringComparison.OrdinalIgnoreCase);
				if (flag6)
				{
					intValue = 0;
					return true;
				}
				bool flag7 = string.Equals(value, "y", StringComparison.OrdinalIgnoreCase);
				if (flag7)
				{
					intValue = 1;
					return true;
				}
				bool flag8 = string.Equals(value, "z", StringComparison.OrdinalIgnoreCase);
				if (flag8)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.BackgroundPositionKeyword:
			{
				bool flag9 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
				if (flag9)
				{
					intValue = 0;
					return true;
				}
				bool flag10 = string.Equals(value, "top", StringComparison.OrdinalIgnoreCase);
				if (flag10)
				{
					intValue = 1;
					return true;
				}
				bool flag11 = string.Equals(value, "bottom", StringComparison.OrdinalIgnoreCase);
				if (flag11)
				{
					intValue = 2;
					return true;
				}
				bool flag12 = string.Equals(value, "left", StringComparison.OrdinalIgnoreCase);
				if (flag12)
				{
					intValue = 3;
					return true;
				}
				bool flag13 = string.Equals(value, "right", StringComparison.OrdinalIgnoreCase);
				if (flag13)
				{
					intValue = 4;
					return true;
				}
				break;
			}
			case StyleEnumType.BackgroundSizeType:
			{
				bool flag14 = string.Equals(value, "cover", StringComparison.OrdinalIgnoreCase);
				if (flag14)
				{
					intValue = 1;
					return true;
				}
				bool flag15 = string.Equals(value, "contain", StringComparison.OrdinalIgnoreCase);
				if (flag15)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.DisplayStyle:
			{
				bool flag16 = string.Equals(value, "flex", StringComparison.OrdinalIgnoreCase);
				if (flag16)
				{
					intValue = 0;
					return true;
				}
				bool flag17 = string.Equals(value, "none", StringComparison.OrdinalIgnoreCase);
				if (flag17)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.EasingMode:
			{
				bool flag18 = string.Equals(value, "ease", StringComparison.OrdinalIgnoreCase);
				if (flag18)
				{
					intValue = 0;
					return true;
				}
				bool flag19 = string.Equals(value, "ease-in", StringComparison.OrdinalIgnoreCase);
				if (flag19)
				{
					intValue = 1;
					return true;
				}
				bool flag20 = string.Equals(value, "ease-out", StringComparison.OrdinalIgnoreCase);
				if (flag20)
				{
					intValue = 2;
					return true;
				}
				bool flag21 = string.Equals(value, "ease-in-out", StringComparison.OrdinalIgnoreCase);
				if (flag21)
				{
					intValue = 3;
					return true;
				}
				bool flag22 = string.Equals(value, "linear", StringComparison.OrdinalIgnoreCase);
				if (flag22)
				{
					intValue = 4;
					return true;
				}
				bool flag23 = string.Equals(value, "ease-in-sine", StringComparison.OrdinalIgnoreCase);
				if (flag23)
				{
					intValue = 5;
					return true;
				}
				bool flag24 = string.Equals(value, "ease-out-sine", StringComparison.OrdinalIgnoreCase);
				if (flag24)
				{
					intValue = 6;
					return true;
				}
				bool flag25 = string.Equals(value, "ease-in-out-sine", StringComparison.OrdinalIgnoreCase);
				if (flag25)
				{
					intValue = 7;
					return true;
				}
				bool flag26 = string.Equals(value, "ease-in-cubic", StringComparison.OrdinalIgnoreCase);
				if (flag26)
				{
					intValue = 8;
					return true;
				}
				bool flag27 = string.Equals(value, "ease-out-cubic", StringComparison.OrdinalIgnoreCase);
				if (flag27)
				{
					intValue = 9;
					return true;
				}
				bool flag28 = string.Equals(value, "ease-in-out-cubic", StringComparison.OrdinalIgnoreCase);
				if (flag28)
				{
					intValue = 10;
					return true;
				}
				bool flag29 = string.Equals(value, "ease-in-circ", StringComparison.OrdinalIgnoreCase);
				if (flag29)
				{
					intValue = 11;
					return true;
				}
				bool flag30 = string.Equals(value, "ease-out-circ", StringComparison.OrdinalIgnoreCase);
				if (flag30)
				{
					intValue = 12;
					return true;
				}
				bool flag31 = string.Equals(value, "ease-in-out-circ", StringComparison.OrdinalIgnoreCase);
				if (flag31)
				{
					intValue = 13;
					return true;
				}
				bool flag32 = string.Equals(value, "ease-in-elastic", StringComparison.OrdinalIgnoreCase);
				if (flag32)
				{
					intValue = 14;
					return true;
				}
				bool flag33 = string.Equals(value, "ease-out-elastic", StringComparison.OrdinalIgnoreCase);
				if (flag33)
				{
					intValue = 15;
					return true;
				}
				bool flag34 = string.Equals(value, "ease-in-out-elastic", StringComparison.OrdinalIgnoreCase);
				if (flag34)
				{
					intValue = 16;
					return true;
				}
				bool flag35 = string.Equals(value, "ease-in-back", StringComparison.OrdinalIgnoreCase);
				if (flag35)
				{
					intValue = 17;
					return true;
				}
				bool flag36 = string.Equals(value, "ease-out-back", StringComparison.OrdinalIgnoreCase);
				if (flag36)
				{
					intValue = 18;
					return true;
				}
				bool flag37 = string.Equals(value, "ease-in-out-back", StringComparison.OrdinalIgnoreCase);
				if (flag37)
				{
					intValue = 19;
					return true;
				}
				bool flag38 = string.Equals(value, "ease-in-bounce", StringComparison.OrdinalIgnoreCase);
				if (flag38)
				{
					intValue = 20;
					return true;
				}
				bool flag39 = string.Equals(value, "ease-out-bounce", StringComparison.OrdinalIgnoreCase);
				if (flag39)
				{
					intValue = 21;
					return true;
				}
				bool flag40 = string.Equals(value, "ease-in-out-bounce", StringComparison.OrdinalIgnoreCase);
				if (flag40)
				{
					intValue = 22;
					return true;
				}
				break;
			}
			case StyleEnumType.EditorTextRenderingMode:
			{
				bool flag41 = string.Equals(value, "sdf", StringComparison.OrdinalIgnoreCase);
				if (flag41)
				{
					intValue = 0;
					return true;
				}
				bool flag42 = string.Equals(value, "bitmap", StringComparison.OrdinalIgnoreCase);
				if (flag42)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.FlexDirection:
			{
				bool flag43 = string.Equals(value, "column", StringComparison.OrdinalIgnoreCase);
				if (flag43)
				{
					intValue = 0;
					return true;
				}
				bool flag44 = string.Equals(value, "column-reverse", StringComparison.OrdinalIgnoreCase);
				if (flag44)
				{
					intValue = 1;
					return true;
				}
				bool flag45 = string.Equals(value, "row", StringComparison.OrdinalIgnoreCase);
				if (flag45)
				{
					intValue = 2;
					return true;
				}
				bool flag46 = string.Equals(value, "row-reverse", StringComparison.OrdinalIgnoreCase);
				if (flag46)
				{
					intValue = 3;
					return true;
				}
				break;
			}
			case StyleEnumType.FontStyle:
			{
				bool flag47 = string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase);
				if (flag47)
				{
					intValue = 0;
					return true;
				}
				bool flag48 = string.Equals(value, "bold", StringComparison.OrdinalIgnoreCase);
				if (flag48)
				{
					intValue = 1;
					return true;
				}
				bool flag49 = string.Equals(value, "italic", StringComparison.OrdinalIgnoreCase);
				if (flag49)
				{
					intValue = 2;
					return true;
				}
				bool flag50 = string.Equals(value, "bold-and-italic", StringComparison.OrdinalIgnoreCase);
				if (flag50)
				{
					intValue = 3;
					return true;
				}
				break;
			}
			case StyleEnumType.Justify:
			{
				bool flag51 = string.Equals(value, "flex-start", StringComparison.OrdinalIgnoreCase);
				if (flag51)
				{
					intValue = 0;
					return true;
				}
				bool flag52 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
				if (flag52)
				{
					intValue = 1;
					return true;
				}
				bool flag53 = string.Equals(value, "flex-end", StringComparison.OrdinalIgnoreCase);
				if (flag53)
				{
					intValue = 2;
					return true;
				}
				bool flag54 = string.Equals(value, "space-between", StringComparison.OrdinalIgnoreCase);
				if (flag54)
				{
					intValue = 3;
					return true;
				}
				bool flag55 = string.Equals(value, "space-around", StringComparison.OrdinalIgnoreCase);
				if (flag55)
				{
					intValue = 4;
					return true;
				}
				bool flag56 = string.Equals(value, "space-evenly", StringComparison.OrdinalIgnoreCase);
				if (flag56)
				{
					intValue = 5;
					return true;
				}
				break;
			}
			case StyleEnumType.Overflow:
			{
				bool flag57 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag57)
				{
					intValue = 0;
					return true;
				}
				bool flag58 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
				if (flag58)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.OverflowClipBox:
			{
				bool flag59 = string.Equals(value, "padding-box", StringComparison.OrdinalIgnoreCase);
				if (flag59)
				{
					intValue = 0;
					return true;
				}
				bool flag60 = string.Equals(value, "content-box", StringComparison.OrdinalIgnoreCase);
				if (flag60)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.OverflowInternal:
			{
				bool flag61 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag61)
				{
					intValue = 0;
					return true;
				}
				bool flag62 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
				if (flag62)
				{
					intValue = 1;
					return true;
				}
				bool flag63 = string.Equals(value, "scroll", StringComparison.OrdinalIgnoreCase);
				if (flag63)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.Position:
			{
				bool flag64 = string.Equals(value, "relative", StringComparison.OrdinalIgnoreCase);
				if (flag64)
				{
					intValue = 0;
					return true;
				}
				bool flag65 = string.Equals(value, "absolute", StringComparison.OrdinalIgnoreCase);
				if (flag65)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.Repeat:
			{
				bool flag66 = string.Equals(value, "no-repeat", StringComparison.OrdinalIgnoreCase);
				if (flag66)
				{
					intValue = 0;
					return true;
				}
				bool flag67 = string.Equals(value, "repeat", StringComparison.OrdinalIgnoreCase);
				if (flag67)
				{
					intValue = 3;
					return true;
				}
				bool flag68 = string.Equals(value, "space", StringComparison.OrdinalIgnoreCase);
				if (flag68)
				{
					intValue = 1;
					return true;
				}
				bool flag69 = string.Equals(value, "round", StringComparison.OrdinalIgnoreCase);
				if (flag69)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.RepeatXY:
			{
				bool flag70 = string.Equals(value, "repeat-x", StringComparison.OrdinalIgnoreCase);
				if (flag70)
				{
					intValue = 0;
					return true;
				}
				bool flag71 = string.Equals(value, "repeat-y", StringComparison.OrdinalIgnoreCase);
				if (flag71)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.ScaleMode:
			{
				bool flag72 = string.Equals(value, "stretch-to-fill", StringComparison.OrdinalIgnoreCase);
				if (flag72)
				{
					intValue = 0;
					return true;
				}
				bool flag73 = string.Equals(value, "scale-and-crop", StringComparison.OrdinalIgnoreCase);
				if (flag73)
				{
					intValue = 1;
					return true;
				}
				bool flag74 = string.Equals(value, "scale-to-fit", StringComparison.OrdinalIgnoreCase);
				if (flag74)
				{
					intValue = 2;
					return true;
				}
				break;
			}
			case StyleEnumType.SliceType:
			{
				bool flag75 = string.Equals(value, "sliced", StringComparison.OrdinalIgnoreCase);
				if (flag75)
				{
					intValue = 0;
					return true;
				}
				bool flag76 = string.Equals(value, "tiled", StringComparison.OrdinalIgnoreCase);
				if (flag76)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.TextAnchor:
			{
				bool flag77 = string.Equals(value, "upper-left", StringComparison.OrdinalIgnoreCase);
				if (flag77)
				{
					intValue = 0;
					return true;
				}
				bool flag78 = string.Equals(value, "upper-center", StringComparison.OrdinalIgnoreCase);
				if (flag78)
				{
					intValue = 1;
					return true;
				}
				bool flag79 = string.Equals(value, "upper-right", StringComparison.OrdinalIgnoreCase);
				if (flag79)
				{
					intValue = 2;
					return true;
				}
				bool flag80 = string.Equals(value, "middle-left", StringComparison.OrdinalIgnoreCase);
				if (flag80)
				{
					intValue = 3;
					return true;
				}
				bool flag81 = string.Equals(value, "middle-center", StringComparison.OrdinalIgnoreCase);
				if (flag81)
				{
					intValue = 4;
					return true;
				}
				bool flag82 = string.Equals(value, "middle-right", StringComparison.OrdinalIgnoreCase);
				if (flag82)
				{
					intValue = 5;
					return true;
				}
				bool flag83 = string.Equals(value, "lower-left", StringComparison.OrdinalIgnoreCase);
				if (flag83)
				{
					intValue = 6;
					return true;
				}
				bool flag84 = string.Equals(value, "lower-center", StringComparison.OrdinalIgnoreCase);
				if (flag84)
				{
					intValue = 7;
					return true;
				}
				bool flag85 = string.Equals(value, "lower-right", StringComparison.OrdinalIgnoreCase);
				if (flag85)
				{
					intValue = 8;
					return true;
				}
				break;
			}
			case StyleEnumType.TextAutoSizeMode:
			{
				bool flag86 = string.Equals(value, "none", StringComparison.OrdinalIgnoreCase);
				if (flag86)
				{
					intValue = 0;
					return true;
				}
				bool flag87 = string.Equals(value, "best-fit", StringComparison.OrdinalIgnoreCase);
				if (flag87)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.TextGeneratorType:
			{
				bool flag88 = string.Equals(value, "advanced", StringComparison.OrdinalIgnoreCase);
				if (flag88)
				{
					intValue = 1;
					return true;
				}
				bool flag89 = string.Equals(value, "standard", StringComparison.OrdinalIgnoreCase);
				if (flag89)
				{
					intValue = 0;
					return true;
				}
				break;
			}
			case StyleEnumType.TextOverflow:
			{
				bool flag90 = string.Equals(value, "clip", StringComparison.OrdinalIgnoreCase);
				if (flag90)
				{
					intValue = 0;
					return true;
				}
				bool flag91 = string.Equals(value, "ellipsis", StringComparison.OrdinalIgnoreCase);
				if (flag91)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.TextOverflowPosition:
			{
				bool flag92 = string.Equals(value, "start", StringComparison.OrdinalIgnoreCase);
				if (flag92)
				{
					intValue = 1;
					return true;
				}
				bool flag93 = string.Equals(value, "middle", StringComparison.OrdinalIgnoreCase);
				if (flag93)
				{
					intValue = 2;
					return true;
				}
				bool flag94 = string.Equals(value, "end", StringComparison.OrdinalIgnoreCase);
				if (flag94)
				{
					intValue = 0;
					return true;
				}
				break;
			}
			case StyleEnumType.TransformOriginOffset:
			{
				bool flag95 = string.Equals(value, "left", StringComparison.OrdinalIgnoreCase);
				if (flag95)
				{
					intValue = 1;
					return true;
				}
				bool flag96 = string.Equals(value, "right", StringComparison.OrdinalIgnoreCase);
				if (flag96)
				{
					intValue = 2;
					return true;
				}
				bool flag97 = string.Equals(value, "top", StringComparison.OrdinalIgnoreCase);
				if (flag97)
				{
					intValue = 3;
					return true;
				}
				bool flag98 = string.Equals(value, "bottom", StringComparison.OrdinalIgnoreCase);
				if (flag98)
				{
					intValue = 4;
					return true;
				}
				bool flag99 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
				if (flag99)
				{
					intValue = 5;
					return true;
				}
				break;
			}
			case StyleEnumType.Visibility:
			{
				bool flag100 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag100)
				{
					intValue = 0;
					return true;
				}
				bool flag101 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
				if (flag101)
				{
					intValue = 1;
					return true;
				}
				break;
			}
			case StyleEnumType.WhiteSpace:
			{
				bool flag102 = string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase);
				if (flag102)
				{
					intValue = 0;
					return true;
				}
				bool flag103 = string.Equals(value, "nowrap", StringComparison.OrdinalIgnoreCase);
				if (flag103)
				{
					intValue = 1;
					return true;
				}
				bool flag104 = string.Equals(value, "pre", StringComparison.OrdinalIgnoreCase);
				if (flag104)
				{
					intValue = 2;
					return true;
				}
				bool flag105 = string.Equals(value, "pre-wrap", StringComparison.OrdinalIgnoreCase);
				if (flag105)
				{
					intValue = 3;
					return true;
				}
				break;
			}
			case StyleEnumType.Wrap:
			{
				bool flag106 = string.Equals(value, "nowrap", StringComparison.OrdinalIgnoreCase);
				if (flag106)
				{
					intValue = 0;
					return true;
				}
				bool flag107 = string.Equals(value, "wrap", StringComparison.OrdinalIgnoreCase);
				if (flag107)
				{
					intValue = 1;
					return true;
				}
				bool flag108 = string.Equals(value, "wrap-reverse", StringComparison.OrdinalIgnoreCase);
				if (flag108)
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
			if (id <= StylePropertyId.UnityMaterial)
			{
				if (id == StylePropertyId.Custom)
				{
					return new Type[] { typeof(Object) };
				}
				switch (id)
				{
				case StylePropertyId.UnityFont:
					return new Type[] { typeof(Font) };
				case StylePropertyId.UnityFontDefinition:
					return FontDefinition.allowedAssetTypes;
				case StylePropertyId.UnityMaterial:
					return MaterialDefinition.allowedAssetTypes;
				}
			}
			else
			{
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

		public static bool StyleValueCanHoldResource(StylePropertyId id)
		{
			if (id <= StylePropertyId.UnityMaterial)
			{
				if (id == StylePropertyId.Custom)
				{
					return true;
				}
				switch (id)
				{
				case StylePropertyId.UnityFont:
					return true;
				case StylePropertyId.UnityFontDefinition:
					return true;
				case StylePropertyId.UnityMaterial:
					return true;
				}
			}
			else
			{
				if (id == StylePropertyId.Cursor)
				{
					return true;
				}
				if (id == StylePropertyId.BackgroundImage)
				{
					return true;
				}
			}
			return false;
		}

		internal static Dictionary<string, StylePropertyId> propertyNameToStylePropertyId
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return StylePropertyUtil.s_NameToId;
			}
		}

		internal static Dictionary<StylePropertyId, string> stylePropertyIdToPropertyName
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return StylePropertyUtil.s_IdToName;
			}
		}

		internal static Dictionary<string, string> ussNameToCSharpName
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return StylePropertyUtil.s_UssNameToCSharpName;
			}
		}

		internal static Dictionary<string, string> cSharpNameToUssName
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return StylePropertyUtil.s_CSharpNameToUssName;
			}
		}

		public static bool IsAnimatable(StylePropertyId id)
		{
			return StylePropertyUtil.s_AnimatableProperties.Contains(id);
		}

		public static IEnumerable<StylePropertyId> AllPropertyIds()
		{
			return StylePropertyUtil.s_IdToName.Keys;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		public static string ToDisplayString(this LengthUnit unit)
		{
			if (!true)
			{
			}
			string text;
			if (unit != LengthUnit.Pixel)
			{
				if (unit != LengthUnit.Percent)
				{
					text = "-";
				}
				else
				{
					text = "%";
				}
			}
			else
			{
				text = "px";
			}
			if (!true)
			{
			}
			return text;
		}

		public static string ToDisplayString(this TimeUnit unit)
		{
			if (!true)
			{
			}
			string text;
			if (unit != TimeUnit.Second)
			{
				if (unit != TimeUnit.Millisecond)
				{
					text = "-";
				}
				else
				{
					text = "ms";
				}
			}
			else
			{
				text = "s";
			}
			if (!true)
			{
			}
			return text;
		}

		public static string ToDisplayString(this AngleUnit unit)
		{
			if (!true)
			{
			}
			string text;
			switch (unit)
			{
			case AngleUnit.Degree:
				text = "deg";
				break;
			case AngleUnit.Gradian:
				text = "grad";
				break;
			case AngleUnit.Radian:
				text = "rad";
				break;
			case AngleUnit.Turn:
				text = "turn";
				break;
			default:
				text = "-";
				break;
			}
			if (!true)
			{
			}
			return text;
		}

		public static bool TryParseFloat(ReadOnlySpan<char> floatStr, out float value)
		{
			return float.TryParse(floatStr, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture.NumberFormat, out value);
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
				"aspect-ratio",
				StylePropertyId.AspectRatio
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
				"filter",
				StylePropertyId.Filter
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
				"-unity-editor-text-rendering-mode",
				StylePropertyId.UnityEditorTextRenderingMode
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
				"-unity-material",
				StylePropertyId.UnityMaterial
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
				"-unity-slice-type",
				StylePropertyId.UnitySliceType
			},
			{
				"-unity-text-align",
				StylePropertyId.UnityTextAlign
			},
			{
				"-unity-text-auto-size",
				StylePropertyId.UnityTextAutoSize
			},
			{
				"-unity-text-generator",
				StylePropertyId.UnityTextGenerator
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
				StylePropertyId.AspectRatio,
				"aspect-ratio"
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
				StylePropertyId.Filter,
				"filter"
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
				StylePropertyId.UnityEditorTextRenderingMode,
				"-unity-editor-text-rendering-mode"
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
				StylePropertyId.UnityMaterial,
				"-unity-material"
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
				StylePropertyId.UnitySliceType,
				"-unity-slice-type"
			},
			{
				StylePropertyId.UnityTextAlign,
				"-unity-text-align"
			},
			{
				StylePropertyId.UnityTextAutoSize,
				"-unity-text-auto-size"
			},
			{
				StylePropertyId.UnityTextGenerator,
				"-unity-text-generator"
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

		internal static readonly Dictionary<string, string> s_UssNameToCSharpName = new Dictionary<string, string>
		{
			{ "align-content", "alignContent" },
			{ "align-items", "alignItems" },
			{ "align-self", "alignSelf" },
			{ "all", "all" },
			{ "aspect-ratio", "aspectRatio" },
			{ "background-color", "backgroundColor" },
			{ "background-image", "backgroundImage" },
			{ "background-position", "backgroundPosition" },
			{ "background-position-x", "backgroundPositionX" },
			{ "background-position-y", "backgroundPositionY" },
			{ "background-repeat", "backgroundRepeat" },
			{ "background-size", "backgroundSize" },
			{ "border-bottom-color", "borderBottomColor" },
			{ "border-bottom-left-radius", "borderBottomLeftRadius" },
			{ "border-bottom-right-radius", "borderBottomRightRadius" },
			{ "border-bottom-width", "borderBottomWidth" },
			{ "border-color", "borderColor" },
			{ "border-left-color", "borderLeftColor" },
			{ "border-left-width", "borderLeftWidth" },
			{ "border-radius", "borderRadius" },
			{ "border-right-color", "borderRightColor" },
			{ "border-right-width", "borderRightWidth" },
			{ "border-top-color", "borderTopColor" },
			{ "border-top-left-radius", "borderTopLeftRadius" },
			{ "border-top-right-radius", "borderTopRightRadius" },
			{ "border-top-width", "borderTopWidth" },
			{ "border-width", "borderWidth" },
			{ "bottom", "bottom" },
			{ "color", "color" },
			{ "cursor", "cursor" },
			{ "display", "display" },
			{ "filter", "filter" },
			{ "flex", "flex" },
			{ "flex-basis", "flexBasis" },
			{ "flex-direction", "flexDirection" },
			{ "flex-grow", "flexGrow" },
			{ "flex-shrink", "flexShrink" },
			{ "flex-wrap", "flexWrap" },
			{ "font-size", "fontSize" },
			{ "height", "height" },
			{ "justify-content", "justifyContent" },
			{ "left", "left" },
			{ "letter-spacing", "letterSpacing" },
			{ "margin", "margin" },
			{ "margin-bottom", "marginBottom" },
			{ "margin-left", "marginLeft" },
			{ "margin-right", "marginRight" },
			{ "margin-top", "marginTop" },
			{ "max-height", "maxHeight" },
			{ "max-width", "maxWidth" },
			{ "min-height", "minHeight" },
			{ "min-width", "minWidth" },
			{ "opacity", "opacity" },
			{ "overflow", "overflow" },
			{ "padding", "padding" },
			{ "padding-bottom", "paddingBottom" },
			{ "padding-left", "paddingLeft" },
			{ "padding-right", "paddingRight" },
			{ "padding-top", "paddingTop" },
			{ "position", "position" },
			{ "right", "right" },
			{ "rotate", "rotate" },
			{ "scale", "scale" },
			{ "text-overflow", "textOverflow" },
			{ "text-shadow", "textShadow" },
			{ "top", "top" },
			{ "transform-origin", "transformOrigin" },
			{ "transition", "transition" },
			{ "transition-delay", "transitionDelay" },
			{ "transition-duration", "transitionDuration" },
			{ "transition-property", "transitionProperty" },
			{ "transition-timing-function", "transitionTimingFunction" },
			{ "translate", "translate" },
			{ "-unity-background-image-tint-color", "unityBackgroundImageTintColor" },
			{ "-unity-background-scale-mode", "unityBackgroundScaleMode" },
			{ "-unity-editor-text-rendering-mode", "unityEditorTextRenderingMode" },
			{ "-unity-font", "unityFont" },
			{ "-unity-font-definition", "unityFontDefinition" },
			{ "-unity-font-style", "unityFontStyleAndWeight" },
			{ "-unity-material", "unityMaterial" },
			{ "-unity-overflow-clip-box", "unityOverflowClipBox" },
			{ "-unity-paragraph-spacing", "unityParagraphSpacing" },
			{ "-unity-slice-bottom", "unitySliceBottom" },
			{ "-unity-slice-left", "unitySliceLeft" },
			{ "-unity-slice-right", "unitySliceRight" },
			{ "-unity-slice-scale", "unitySliceScale" },
			{ "-unity-slice-top", "unitySliceTop" },
			{ "-unity-slice-type", "unitySliceType" },
			{ "-unity-text-align", "unityTextAlign" },
			{ "-unity-text-auto-size", "unityTextAutoSize" },
			{ "-unity-text-generator", "unityTextGenerator" },
			{ "-unity-text-outline", "unityTextOutline" },
			{ "-unity-text-outline-color", "unityTextOutlineColor" },
			{ "-unity-text-outline-width", "unityTextOutlineWidth" },
			{ "-unity-text-overflow-position", "unityTextOverflowPosition" },
			{ "visibility", "visibility" },
			{ "white-space", "whiteSpace" },
			{ "width", "width" },
			{ "word-spacing", "wordSpacing" }
		};

		internal static readonly Dictionary<string, string> s_CSharpNameToUssName = new Dictionary<string, string>
		{
			{ "alignContent", "align-content" },
			{ "alignItems", "align-items" },
			{ "alignSelf", "align-self" },
			{ "all", "all" },
			{ "aspectRatio", "aspect-ratio" },
			{ "backgroundColor", "background-color" },
			{ "backgroundImage", "background-image" },
			{ "backgroundPosition", "background-position" },
			{ "backgroundPositionX", "background-position-x" },
			{ "backgroundPositionY", "background-position-y" },
			{ "backgroundRepeat", "background-repeat" },
			{ "backgroundSize", "background-size" },
			{ "borderBottomColor", "border-bottom-color" },
			{ "borderBottomLeftRadius", "border-bottom-left-radius" },
			{ "borderBottomRightRadius", "border-bottom-right-radius" },
			{ "borderBottomWidth", "border-bottom-width" },
			{ "borderColor", "border-color" },
			{ "borderLeftColor", "border-left-color" },
			{ "borderLeftWidth", "border-left-width" },
			{ "borderRadius", "border-radius" },
			{ "borderRightColor", "border-right-color" },
			{ "borderRightWidth", "border-right-width" },
			{ "borderTopColor", "border-top-color" },
			{ "borderTopLeftRadius", "border-top-left-radius" },
			{ "borderTopRightRadius", "border-top-right-radius" },
			{ "borderTopWidth", "border-top-width" },
			{ "borderWidth", "border-width" },
			{ "bottom", "bottom" },
			{ "color", "color" },
			{ "cursor", "cursor" },
			{ "display", "display" },
			{ "filter", "filter" },
			{ "flex", "flex" },
			{ "flexBasis", "flex-basis" },
			{ "flexDirection", "flex-direction" },
			{ "flexGrow", "flex-grow" },
			{ "flexShrink", "flex-shrink" },
			{ "flexWrap", "flex-wrap" },
			{ "fontSize", "font-size" },
			{ "height", "height" },
			{ "justifyContent", "justify-content" },
			{ "left", "left" },
			{ "letterSpacing", "letter-spacing" },
			{ "margin", "margin" },
			{ "marginBottom", "margin-bottom" },
			{ "marginLeft", "margin-left" },
			{ "marginRight", "margin-right" },
			{ "marginTop", "margin-top" },
			{ "maxHeight", "max-height" },
			{ "maxWidth", "max-width" },
			{ "minHeight", "min-height" },
			{ "minWidth", "min-width" },
			{ "opacity", "opacity" },
			{ "overflow", "overflow" },
			{ "padding", "padding" },
			{ "paddingBottom", "padding-bottom" },
			{ "paddingLeft", "padding-left" },
			{ "paddingRight", "padding-right" },
			{ "paddingTop", "padding-top" },
			{ "position", "position" },
			{ "right", "right" },
			{ "rotate", "rotate" },
			{ "scale", "scale" },
			{ "textOverflow", "text-overflow" },
			{ "textShadow", "text-shadow" },
			{ "top", "top" },
			{ "transformOrigin", "transform-origin" },
			{ "transition", "transition" },
			{ "transitionDelay", "transition-delay" },
			{ "transitionDuration", "transition-duration" },
			{ "transitionProperty", "transition-property" },
			{ "transitionTimingFunction", "transition-timing-function" },
			{ "translate", "translate" },
			{ "unityBackgroundImageTintColor", "-unity-background-image-tint-color" },
			{ "unityBackgroundScaleMode", "-unity-background-scale-mode" },
			{ "unityEditorTextRenderingMode", "-unity-editor-text-rendering-mode" },
			{ "unityFont", "-unity-font" },
			{ "unityFontDefinition", "-unity-font-definition" },
			{ "unityFontStyleAndWeight", "-unity-font-style" },
			{ "unityMaterial", "-unity-material" },
			{ "unityOverflowClipBox", "-unity-overflow-clip-box" },
			{ "unityParagraphSpacing", "-unity-paragraph-spacing" },
			{ "unitySliceBottom", "-unity-slice-bottom" },
			{ "unitySliceLeft", "-unity-slice-left" },
			{ "unitySliceRight", "-unity-slice-right" },
			{ "unitySliceScale", "-unity-slice-scale" },
			{ "unitySliceTop", "-unity-slice-top" },
			{ "unitySliceType", "-unity-slice-type" },
			{ "unityTextAlign", "-unity-text-align" },
			{ "unityTextAutoSize", "-unity-text-auto-size" },
			{ "unityTextGenerator", "-unity-text-generator" },
			{ "unityTextOutline", "-unity-text-outline" },
			{ "unityTextOutlineColor", "-unity-text-outline-color" },
			{ "unityTextOutlineWidth", "-unity-text-outline-width" },
			{ "unityTextOverflowPosition", "-unity-text-overflow-position" },
			{ "visibility", "visibility" },
			{ "whiteSpace", "white-space" },
			{ "width", "width" },
			{ "wordSpacing", "word-spacing" }
		};

		internal static readonly HashSet<StylePropertyId> s_AnimatableProperties = new HashSet<StylePropertyId>
		{
			StylePropertyId.AlignContent,
			StylePropertyId.AlignItems,
			StylePropertyId.AlignSelf,
			StylePropertyId.All,
			StylePropertyId.AspectRatio,
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
			StylePropertyId.Filter,
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
			StylePropertyId.UnityMaterial,
			StylePropertyId.UnityOverflowClipBox,
			StylePropertyId.UnityParagraphSpacing,
			StylePropertyId.UnitySliceBottom,
			StylePropertyId.UnitySliceLeft,
			StylePropertyId.UnitySliceRight,
			StylePropertyId.UnitySliceScale,
			StylePropertyId.UnitySliceTop,
			StylePropertyId.UnitySliceType,
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
