using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class ResolvedStyleAccess : IResolvedStyle
	{
		public Align alignContent
		{
			get
			{
				return this.ve.computedStyle.alignContent;
			}
		}

		public Align alignItems
		{
			get
			{
				return this.ve.computedStyle.alignItems;
			}
		}

		public Align alignSelf
		{
			get
			{
				return this.ve.computedStyle.alignSelf;
			}
		}

		public Ratio aspectRatio
		{
			get
			{
				return this.ve.computedStyle.aspectRatio;
			}
		}

		public Color backgroundColor
		{
			get
			{
				return this.ve.computedStyle.backgroundColor;
			}
		}

		public Background backgroundImage
		{
			get
			{
				return this.ve.computedStyle.backgroundImage;
			}
		}

		public BackgroundPosition backgroundPositionX
		{
			get
			{
				return this.ve.computedStyle.backgroundPositionX;
			}
		}

		public BackgroundPosition backgroundPositionY
		{
			get
			{
				return this.ve.computedStyle.backgroundPositionY;
			}
		}

		public BackgroundRepeat backgroundRepeat
		{
			get
			{
				return this.ve.computedStyle.backgroundRepeat;
			}
		}

		public BackgroundSize backgroundSize
		{
			get
			{
				return this.ve.computedStyle.backgroundSize;
			}
		}

		public Color borderBottomColor
		{
			get
			{
				return this.ve.computedStyle.borderBottomColor;
			}
		}

		public float borderBottomLeftRadius
		{
			get
			{
				return this.ve.computedStyle.borderBottomLeftRadius.value;
			}
		}

		public float borderBottomRightRadius
		{
			get
			{
				return this.ve.computedStyle.borderBottomRightRadius.value;
			}
		}

		public float borderBottomWidth
		{
			get
			{
				return this.ve.layoutNode.LayoutBorderBottom;
			}
		}

		public Color borderLeftColor
		{
			get
			{
				return this.ve.computedStyle.borderLeftColor;
			}
		}

		public float borderLeftWidth
		{
			get
			{
				return this.ve.layoutNode.LayoutBorderLeft;
			}
		}

		public Color borderRightColor
		{
			get
			{
				return this.ve.computedStyle.borderRightColor;
			}
		}

		public float borderRightWidth
		{
			get
			{
				return this.ve.layoutNode.LayoutBorderRight;
			}
		}

		public Color borderTopColor
		{
			get
			{
				return this.ve.computedStyle.borderTopColor;
			}
		}

		public float borderTopLeftRadius
		{
			get
			{
				return this.ve.computedStyle.borderTopLeftRadius.value;
			}
		}

		public float borderTopRightRadius
		{
			get
			{
				return this.ve.computedStyle.borderTopRightRadius.value;
			}
		}

		public float borderTopWidth
		{
			get
			{
				return this.ve.layoutNode.LayoutBorderTop;
			}
		}

		public float bottom
		{
			get
			{
				return this.ve.layoutNode.LayoutBottom;
			}
		}

		public Color color
		{
			get
			{
				return this.ve.computedStyle.color;
			}
		}

		public DisplayStyle display
		{
			get
			{
				return this.ve.computedStyle.display;
			}
		}

		public IEnumerable<FilterFunction> filter
		{
			get
			{
				return this.ve.computedStyle.filter;
			}
		}

		public StyleFloat flexBasis
		{
			get
			{
				return new StyleFloat(this.ve.layoutNode.ComputedFlexBasis);
			}
		}

		public FlexDirection flexDirection
		{
			get
			{
				return this.ve.computedStyle.flexDirection;
			}
		}

		public float flexGrow
		{
			get
			{
				return this.ve.computedStyle.flexGrow;
			}
		}

		public float flexShrink
		{
			get
			{
				return this.ve.computedStyle.flexShrink;
			}
		}

		public Wrap flexWrap
		{
			get
			{
				return this.ve.computedStyle.flexWrap;
			}
		}

		public float fontSize
		{
			get
			{
				return this.ve.computedStyle.fontSize.value;
			}
		}

		public float height
		{
			get
			{
				return this.ve.layoutNode.LayoutHeight;
			}
		}

		public Justify justifyContent
		{
			get
			{
				return this.ve.computedStyle.justifyContent;
			}
		}

		public float left
		{
			get
			{
				return this.ve.layoutNode.LayoutX;
			}
		}

		public float letterSpacing
		{
			get
			{
				return this.ve.computedStyle.letterSpacing.value;
			}
		}

		public float marginBottom
		{
			get
			{
				return this.ve.layoutNode.LayoutMarginBottom;
			}
		}

		public float marginLeft
		{
			get
			{
				return this.ve.layoutNode.LayoutMarginLeft;
			}
		}

		public float marginRight
		{
			get
			{
				return this.ve.layoutNode.LayoutMarginRight;
			}
		}

		public float marginTop
		{
			get
			{
				return this.ve.layoutNode.LayoutMarginTop;
			}
		}

		public StyleFloat maxHeight
		{
			get
			{
				return this.ve.ResolveLengthValue(this.ve.computedStyle.maxHeight, false);
			}
		}

		public StyleFloat maxWidth
		{
			get
			{
				return this.ve.ResolveLengthValue(this.ve.computedStyle.maxWidth, true);
			}
		}

		public StyleFloat minHeight
		{
			get
			{
				return this.ve.ResolveLengthValue(this.ve.computedStyle.minHeight, false);
			}
		}

		public StyleFloat minWidth
		{
			get
			{
				return this.ve.ResolveLengthValue(this.ve.computedStyle.minWidth, true);
			}
		}

		public float opacity
		{
			get
			{
				return this.ve.computedStyle.opacity;
			}
		}

		public float paddingBottom
		{
			get
			{
				return this.ve.layoutNode.LayoutPaddingBottom;
			}
		}

		public float paddingLeft
		{
			get
			{
				return this.ve.layoutNode.LayoutPaddingLeft;
			}
		}

		public float paddingRight
		{
			get
			{
				return this.ve.layoutNode.LayoutPaddingRight;
			}
		}

		public float paddingTop
		{
			get
			{
				return this.ve.layoutNode.LayoutPaddingTop;
			}
		}

		public Position position
		{
			get
			{
				return this.ve.computedStyle.position;
			}
		}

		public float right
		{
			get
			{
				return this.ve.layoutNode.LayoutRight;
			}
		}

		public Rotate rotate
		{
			get
			{
				return this.ve.computedStyle.rotate;
			}
		}

		public Scale scale
		{
			get
			{
				return this.ve.computedStyle.scale;
			}
		}

		public TextOverflow textOverflow
		{
			get
			{
				return this.ve.computedStyle.textOverflow;
			}
		}

		public float top
		{
			get
			{
				return this.ve.layoutNode.LayoutY;
			}
		}

		public Vector3 transformOrigin
		{
			get
			{
				return this.ve.ResolveTransformOrigin();
			}
		}

		public IEnumerable<TimeValue> transitionDelay
		{
			get
			{
				return this.ve.computedStyle.transitionDelay;
			}
		}

		public IEnumerable<TimeValue> transitionDuration
		{
			get
			{
				return this.ve.computedStyle.transitionDuration;
			}
		}

		public IEnumerable<StylePropertyName> transitionProperty
		{
			get
			{
				return this.ve.computedStyle.transitionProperty;
			}
		}

		public IEnumerable<EasingFunction> transitionTimingFunction
		{
			get
			{
				return this.ve.computedStyle.transitionTimingFunction;
			}
		}

		public Vector3 translate
		{
			get
			{
				return this.ve.ResolveTranslate();
			}
		}

		public Color unityBackgroundImageTintColor
		{
			get
			{
				return this.ve.computedStyle.unityBackgroundImageTintColor;
			}
		}

		public EditorTextRenderingMode unityEditorTextRenderingMode
		{
			get
			{
				return this.ve.computedStyle.unityEditorTextRenderingMode;
			}
		}

		public Font unityFont
		{
			get
			{
				return this.ve.computedStyle.unityFont;
			}
		}

		public FontDefinition unityFontDefinition
		{
			get
			{
				return this.ve.computedStyle.unityFontDefinition;
			}
		}

		public FontStyle unityFontStyleAndWeight
		{
			get
			{
				return this.ve.computedStyle.unityFontStyleAndWeight;
			}
		}

		public MaterialDefinition unityMaterial
		{
			get
			{
				return this.ve.computedStyle.unityMaterial;
			}
		}

		public float unityParagraphSpacing
		{
			get
			{
				return this.ve.computedStyle.unityParagraphSpacing.value;
			}
		}

		public int unitySliceBottom
		{
			get
			{
				return this.ve.computedStyle.unitySliceBottom;
			}
		}

		public int unitySliceLeft
		{
			get
			{
				return this.ve.computedStyle.unitySliceLeft;
			}
		}

		public int unitySliceRight
		{
			get
			{
				return this.ve.computedStyle.unitySliceRight;
			}
		}

		public float unitySliceScale
		{
			get
			{
				return this.ve.computedStyle.unitySliceScale;
			}
		}

		public int unitySliceTop
		{
			get
			{
				return this.ve.computedStyle.unitySliceTop;
			}
		}

		public SliceType unitySliceType
		{
			get
			{
				return this.ve.computedStyle.unitySliceType;
			}
		}

		public TextAnchor unityTextAlign
		{
			get
			{
				return this.ve.computedStyle.unityTextAlign;
			}
		}

		public TextGeneratorType unityTextGenerator
		{
			get
			{
				return this.ve.computedStyle.unityTextGenerator;
			}
		}

		public Color unityTextOutlineColor
		{
			get
			{
				return this.ve.computedStyle.unityTextOutlineColor;
			}
		}

		public float unityTextOutlineWidth
		{
			get
			{
				return this.ve.computedStyle.unityTextOutlineWidth;
			}
		}

		public TextOverflowPosition unityTextOverflowPosition
		{
			get
			{
				return this.ve.computedStyle.unityTextOverflowPosition;
			}
		}

		public Visibility visibility
		{
			get
			{
				return this.ve.computedStyle.visibility;
			}
		}

		public WhiteSpace whiteSpace
		{
			get
			{
				return this.ve.computedStyle.whiteSpace;
			}
		}

		public float width
		{
			get
			{
				return this.ve.layoutNode.LayoutWidth;
			}
		}

		public float wordSpacing
		{
			get
			{
				return this.ve.computedStyle.wordSpacing.value;
			}
		}

		private VisualElement ve { get; }

		public ResolvedStyleAccess(VisualElement ve)
		{
			this.ve = ve;
		}

		[Obsolete("unityBackgroundScaleMode is deprecated. Use background-* properties instead.")]
		public StyleEnum<ScaleMode> unityBackgroundScaleMode
		{
			get
			{
				bool flag;
				return BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(this.ve.computedStyle.backgroundPositionX, this.ve.computedStyle.backgroundPositionY, this.ve.computedStyle.backgroundRepeat, this.ve.computedStyle.backgroundSize, out flag);
			}
		}
	}
}
