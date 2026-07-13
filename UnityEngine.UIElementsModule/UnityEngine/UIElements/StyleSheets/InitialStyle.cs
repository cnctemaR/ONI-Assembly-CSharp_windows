using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class InitialStyle
	{
		public static ref ComputedStyle Get()
		{
			return ref InitialStyle.s_InitialStyle;
		}

		public static ComputedStyle Acquire()
		{
			return InitialStyle.s_InitialStyle.Acquire();
		}

		static InitialStyle()
		{
			InitialStyle.s_InitialStyle.layoutData.Write().alignContent = Align.FlexStart;
			InitialStyle.s_InitialStyle.layoutData.Write().alignItems = Align.Stretch;
			InitialStyle.s_InitialStyle.layoutData.Write().alignSelf = Align.Auto;
			InitialStyle.s_InitialStyle.layoutData.Write().aspectRatio = StyleKeyword.Auto.ToStyleRatio();
			InitialStyle.s_InitialStyle.visualData.Write().backgroundColor = Color.clear;
			InitialStyle.s_InitialStyle.visualData.Write().backgroundImage = default(Background);
			InitialStyle.s_InitialStyle.visualData.Write().backgroundPositionX = BackgroundPosition.Initial();
			InitialStyle.s_InitialStyle.visualData.Write().backgroundPositionY = BackgroundPosition.Initial();
			InitialStyle.s_InitialStyle.visualData.Write().backgroundRepeat = BackgroundRepeat.Initial();
			InitialStyle.s_InitialStyle.visualData.Write().backgroundSize = BackgroundSize.Initial();
			InitialStyle.s_InitialStyle.visualData.Write().borderBottomColor = Color.clear;
			InitialStyle.s_InitialStyle.visualData.Write().borderBottomLeftRadius = 0f;
			InitialStyle.s_InitialStyle.visualData.Write().borderBottomRightRadius = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().borderBottomWidth = 0f;
			InitialStyle.s_InitialStyle.visualData.Write().borderLeftColor = Color.clear;
			InitialStyle.s_InitialStyle.layoutData.Write().borderLeftWidth = 0f;
			InitialStyle.s_InitialStyle.visualData.Write().borderRightColor = Color.clear;
			InitialStyle.s_InitialStyle.layoutData.Write().borderRightWidth = 0f;
			InitialStyle.s_InitialStyle.visualData.Write().borderTopColor = Color.clear;
			InitialStyle.s_InitialStyle.visualData.Write().borderTopLeftRadius = 0f;
			InitialStyle.s_InitialStyle.visualData.Write().borderTopRightRadius = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().borderTopWidth = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().bottom = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.inheritedData.Write().color = Color.black;
			InitialStyle.s_InitialStyle.rareData.Write().cursor = default(Cursor);
			InitialStyle.s_InitialStyle.layoutData.Write().display = DisplayStyle.Flex;
			InitialStyle.s_InitialStyle.visualData.Write().filter = new List<FilterFunction>();
			InitialStyle.s_InitialStyle.layoutData.Write().flexBasis = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.layoutData.Write().flexDirection = FlexDirection.Column;
			InitialStyle.s_InitialStyle.layoutData.Write().flexGrow = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().flexShrink = 1f;
			InitialStyle.s_InitialStyle.layoutData.Write().flexWrap = Wrap.NoWrap;
			InitialStyle.s_InitialStyle.inheritedData.Write().fontSize = 14f;
			InitialStyle.s_InitialStyle.layoutData.Write().height = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.layoutData.Write().justifyContent = Justify.FlexStart;
			InitialStyle.s_InitialStyle.layoutData.Write().left = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.inheritedData.Write().letterSpacing = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().marginBottom = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().marginLeft = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().marginRight = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().marginTop = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().maxHeight = StyleKeyword.None.ToLength();
			InitialStyle.s_InitialStyle.layoutData.Write().maxWidth = StyleKeyword.None.ToLength();
			InitialStyle.s_InitialStyle.layoutData.Write().minHeight = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.layoutData.Write().minWidth = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.visualData.Write().opacity = 1f;
			InitialStyle.s_InitialStyle.visualData.Write().overflow = OverflowInternal.Visible;
			InitialStyle.s_InitialStyle.layoutData.Write().paddingBottom = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().paddingLeft = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().paddingRight = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().paddingTop = 0f;
			InitialStyle.s_InitialStyle.layoutData.Write().position = Position.Relative;
			InitialStyle.s_InitialStyle.layoutData.Write().right = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.transformData.Write().rotate = StyleKeyword.None.ToRotate();
			InitialStyle.s_InitialStyle.transformData.Write().scale = StyleKeyword.None.ToScale();
			InitialStyle.s_InitialStyle.rareData.Write().textOverflow = TextOverflow.Clip;
			InitialStyle.s_InitialStyle.inheritedData.Write().textShadow = default(TextShadow);
			InitialStyle.s_InitialStyle.layoutData.Write().top = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.transformData.Write().transformOrigin = TransformOrigin.Initial();
			InitialStyle.s_InitialStyle.transitionData.Write().transitionDelay = new List<TimeValue> { 0f };
			InitialStyle.s_InitialStyle.transitionData.Write().transitionDuration = new List<TimeValue> { 0f };
			InitialStyle.s_InitialStyle.transitionData.Write().transitionProperty = new List<StylePropertyName> { "all" };
			InitialStyle.s_InitialStyle.transitionData.Write().transitionTimingFunction = new List<EasingFunction> { EasingMode.Ease };
			InitialStyle.s_InitialStyle.transformData.Write().translate = StyleKeyword.None.ToTranslate();
			InitialStyle.s_InitialStyle.rareData.Write().unityBackgroundImageTintColor = Color.white;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityEditorTextRenderingMode = EditorTextRenderingMode.SDF;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityFont = null;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityFontDefinition = default(FontDefinition);
			InitialStyle.s_InitialStyle.inheritedData.Write().unityFontStyleAndWeight = FontStyle.Normal;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityMaterial = default(MaterialDefinition);
			InitialStyle.s_InitialStyle.rareData.Write().unityOverflowClipBox = OverflowClipBox.PaddingBox;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityParagraphSpacing = 0f;
			InitialStyle.s_InitialStyle.rareData.Write().unitySliceBottom = 0;
			InitialStyle.s_InitialStyle.rareData.Write().unitySliceLeft = 0;
			InitialStyle.s_InitialStyle.rareData.Write().unitySliceRight = 0;
			InitialStyle.s_InitialStyle.rareData.Write().unitySliceScale = 1f;
			InitialStyle.s_InitialStyle.rareData.Write().unitySliceTop = 0;
			InitialStyle.s_InitialStyle.rareData.Write().unitySliceType = SliceType.Sliced;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityTextAlign = TextAnchor.UpperLeft;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityTextAutoSize = StyleKeyword.None.ToTextAutoSize();
			InitialStyle.s_InitialStyle.inheritedData.Write().unityTextGenerator = TextGeneratorType.Standard;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityTextOutlineColor = Color.clear;
			InitialStyle.s_InitialStyle.inheritedData.Write().unityTextOutlineWidth = 0f;
			InitialStyle.s_InitialStyle.rareData.Write().unityTextOverflowPosition = TextOverflowPosition.End;
			InitialStyle.s_InitialStyle.inheritedData.Write().visibility = Visibility.Visible;
			InitialStyle.s_InitialStyle.inheritedData.Write().whiteSpace = WhiteSpace.Normal;
			InitialStyle.s_InitialStyle.layoutData.Write().width = StyleKeyword.Auto.ToLength();
			InitialStyle.s_InitialStyle.inheritedData.Write().wordSpacing = 0f;
		}

		public static Align alignContent
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().alignContent;
			}
		}

		public static Align alignItems
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().alignItems;
			}
		}

		public static Align alignSelf
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().alignSelf;
			}
		}

		public static Ratio aspectRatio
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().aspectRatio;
			}
		}

		public static Color backgroundColor
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().backgroundColor;
			}
		}

		public static Background backgroundImage
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().backgroundImage;
			}
		}

		public static BackgroundPosition backgroundPositionX
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().backgroundPositionX;
			}
		}

		public static BackgroundPosition backgroundPositionY
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().backgroundPositionY;
			}
		}

		public static BackgroundRepeat backgroundRepeat
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().backgroundRepeat;
			}
		}

		public static BackgroundSize backgroundSize
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().backgroundSize;
			}
		}

		public static Color borderBottomColor
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderBottomColor;
			}
		}

		public static Length borderBottomLeftRadius
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderBottomLeftRadius;
			}
		}

		public static Length borderBottomRightRadius
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderBottomRightRadius;
			}
		}

		public static float borderBottomWidth
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().borderBottomWidth;
			}
		}

		public static Color borderLeftColor
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderLeftColor;
			}
		}

		public static float borderLeftWidth
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().borderLeftWidth;
			}
		}

		public static Color borderRightColor
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderRightColor;
			}
		}

		public static float borderRightWidth
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().borderRightWidth;
			}
		}

		public static Color borderTopColor
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderTopColor;
			}
		}

		public static Length borderTopLeftRadius
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderTopLeftRadius;
			}
		}

		public static Length borderTopRightRadius
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().borderTopRightRadius;
			}
		}

		public static float borderTopWidth
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().borderTopWidth;
			}
		}

		public static Length bottom
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().bottom;
			}
		}

		public static Color color
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().color;
			}
		}

		public static Cursor cursor
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().cursor;
			}
		}

		public static DisplayStyle display
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().display;
			}
		}

		public static List<FilterFunction> filter
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().filter;
			}
		}

		public static Length flexBasis
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().flexBasis;
			}
		}

		public static FlexDirection flexDirection
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().flexDirection;
			}
		}

		public static float flexGrow
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().flexGrow;
			}
		}

		public static float flexShrink
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().flexShrink;
			}
		}

		public static Wrap flexWrap
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().flexWrap;
			}
		}

		public static Length fontSize
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().fontSize;
			}
		}

		public static Length height
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().height;
			}
		}

		public static Justify justifyContent
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().justifyContent;
			}
		}

		public static Length left
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().left;
			}
		}

		public static Length letterSpacing
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().letterSpacing;
			}
		}

		public static Length marginBottom
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().marginBottom;
			}
		}

		public static Length marginLeft
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().marginLeft;
			}
		}

		public static Length marginRight
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().marginRight;
			}
		}

		public static Length marginTop
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().marginTop;
			}
		}

		public static Length maxHeight
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().maxHeight;
			}
		}

		public static Length maxWidth
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().maxWidth;
			}
		}

		public static Length minHeight
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().minHeight;
			}
		}

		public static Length minWidth
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().minWidth;
			}
		}

		public static float opacity
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().opacity;
			}
		}

		public static OverflowInternal overflow
		{
			get
			{
				return InitialStyle.s_InitialStyle.visualData.Read().overflow;
			}
		}

		public static Length paddingBottom
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().paddingBottom;
			}
		}

		public static Length paddingLeft
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().paddingLeft;
			}
		}

		public static Length paddingRight
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().paddingRight;
			}
		}

		public static Length paddingTop
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().paddingTop;
			}
		}

		public static Position position
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().position;
			}
		}

		public static Length right
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().right;
			}
		}

		public static Rotate rotate
		{
			get
			{
				return InitialStyle.s_InitialStyle.transformData.Read().rotate;
			}
		}

		public static Scale scale
		{
			get
			{
				return InitialStyle.s_InitialStyle.transformData.Read().scale;
			}
		}

		public static TextOverflow textOverflow
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().textOverflow;
			}
		}

		public static TextShadow textShadow
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().textShadow;
			}
		}

		public static Length top
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().top;
			}
		}

		public static TransformOrigin transformOrigin
		{
			get
			{
				return InitialStyle.s_InitialStyle.transformData.Read().transformOrigin;
			}
		}

		public static List<TimeValue> transitionDelay
		{
			get
			{
				return InitialStyle.s_InitialStyle.transitionData.Read().transitionDelay;
			}
		}

		public static List<TimeValue> transitionDuration
		{
			get
			{
				return InitialStyle.s_InitialStyle.transitionData.Read().transitionDuration;
			}
		}

		public static List<StylePropertyName> transitionProperty
		{
			get
			{
				return InitialStyle.s_InitialStyle.transitionData.Read().transitionProperty;
			}
		}

		public static List<EasingFunction> transitionTimingFunction
		{
			get
			{
				return InitialStyle.s_InitialStyle.transitionData.Read().transitionTimingFunction;
			}
		}

		public static Translate translate
		{
			get
			{
				return InitialStyle.s_InitialStyle.transformData.Read().translate;
			}
		}

		public static Color unityBackgroundImageTintColor
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unityBackgroundImageTintColor;
			}
		}

		public static EditorTextRenderingMode unityEditorTextRenderingMode
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityEditorTextRenderingMode;
			}
		}

		public static Font unityFont
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityFont;
			}
		}

		public static FontDefinition unityFontDefinition
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityFontDefinition;
			}
		}

		public static FontStyle unityFontStyleAndWeight
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityFontStyleAndWeight;
			}
		}

		public static MaterialDefinition unityMaterial
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityMaterial;
			}
		}

		public static OverflowClipBox unityOverflowClipBox
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unityOverflowClipBox;
			}
		}

		public static Length unityParagraphSpacing
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityParagraphSpacing;
			}
		}

		public static int unitySliceBottom
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unitySliceBottom;
			}
		}

		public static int unitySliceLeft
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unitySliceLeft;
			}
		}

		public static int unitySliceRight
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unitySliceRight;
			}
		}

		public static float unitySliceScale
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unitySliceScale;
			}
		}

		public static int unitySliceTop
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unitySliceTop;
			}
		}

		public static SliceType unitySliceType
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unitySliceType;
			}
		}

		public static TextAnchor unityTextAlign
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityTextAlign;
			}
		}

		public static TextAutoSize unityTextAutoSize
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityTextAutoSize;
			}
		}

		public static TextGeneratorType unityTextGenerator
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityTextGenerator;
			}
		}

		public static Color unityTextOutlineColor
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityTextOutlineColor;
			}
		}

		public static float unityTextOutlineWidth
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().unityTextOutlineWidth;
			}
		}

		public static TextOverflowPosition unityTextOverflowPosition
		{
			get
			{
				return InitialStyle.s_InitialStyle.rareData.Read().unityTextOverflowPosition;
			}
		}

		public static Visibility visibility
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().visibility;
			}
		}

		public static WhiteSpace whiteSpace
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().whiteSpace;
			}
		}

		public static Length width
		{
			get
			{
				return InitialStyle.s_InitialStyle.layoutData.Read().width;
			}
		}

		public static Length wordSpacing
		{
			get
			{
				return InitialStyle.s_InitialStyle.inheritedData.Read().wordSpacing;
			}
		}

		private static ComputedStyle s_InitialStyle = ComputedStyle.CreateInitial();
	}
}
