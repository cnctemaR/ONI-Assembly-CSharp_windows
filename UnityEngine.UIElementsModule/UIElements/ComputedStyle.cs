using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal struct ComputedStyle
	{
		private VisualElementStylesData stylesData
		{
			get
			{
				return this.m_Element.specifiedStyle;
			}
		}

		private InheritedStylesData inheritedStylesData
		{
			get
			{
				return this.m_Element.inheritedStyle;
			}
		}

		public ComputedStyle(VisualElement element)
		{
			this.m_Element = element;
		}

		public StyleLength width
		{
			get
			{
				return this.stylesData.width;
			}
		}

		public StyleLength height
		{
			get
			{
				return this.stylesData.height;
			}
		}

		public StyleLength maxWidth
		{
			get
			{
				return this.stylesData.maxWidth;
			}
		}

		public StyleLength maxHeight
		{
			get
			{
				return this.stylesData.maxHeight;
			}
		}

		public StyleLength minWidth
		{
			get
			{
				return this.stylesData.minWidth;
			}
		}

		public StyleLength minHeight
		{
			get
			{
				return this.stylesData.minHeight;
			}
		}

		public StyleLength flexBasis
		{
			get
			{
				return this.stylesData.flexBasis;
			}
		}

		public StyleFloat flexGrow
		{
			get
			{
				return this.stylesData.flexGrow;
			}
		}

		public StyleFloat flexShrink
		{
			get
			{
				return this.stylesData.flexShrink;
			}
		}

		public StyleEnum<FlexDirection> flexDirection
		{
			get
			{
				return this.stylesData.flexDirection.ToStyleEnum((FlexDirection)this.stylesData.flexDirection.value);
			}
		}

		public StyleEnum<Wrap> flexWrap
		{
			get
			{
				return this.stylesData.flexWrap.ToStyleEnum((Wrap)this.stylesData.flexWrap.value);
			}
		}

		public StyleEnum<Overflow> overflow
		{
			get
			{
				return this.stylesData.overflow.ToStyleEnum((Overflow)this.stylesData.overflow.value);
			}
		}

		public StyleEnum<OverflowClipBox> unityOverflowClipBox
		{
			get
			{
				return this.stylesData.unityOverflowClipBox.ToStyleEnum((OverflowClipBox)this.stylesData.unityOverflowClipBox.value);
			}
		}

		public StyleLength left
		{
			get
			{
				return this.stylesData.left;
			}
		}

		public StyleLength top
		{
			get
			{
				return this.stylesData.top;
			}
		}

		public StyleLength right
		{
			get
			{
				return this.stylesData.right;
			}
		}

		public StyleLength bottom
		{
			get
			{
				return this.stylesData.bottom;
			}
		}

		public StyleLength marginLeft
		{
			get
			{
				return this.stylesData.marginLeft;
			}
		}

		public StyleLength marginTop
		{
			get
			{
				return this.stylesData.marginTop;
			}
		}

		public StyleLength marginRight
		{
			get
			{
				return this.stylesData.marginRight;
			}
		}

		public StyleLength marginBottom
		{
			get
			{
				return this.stylesData.marginBottom;
			}
		}

		public StyleLength paddingLeft
		{
			get
			{
				return this.stylesData.paddingLeft;
			}
		}

		public StyleLength paddingTop
		{
			get
			{
				return this.stylesData.paddingTop;
			}
		}

		public StyleLength paddingRight
		{
			get
			{
				return this.stylesData.paddingRight;
			}
		}

		public StyleLength paddingBottom
		{
			get
			{
				return this.stylesData.paddingBottom;
			}
		}

		public StyleEnum<Position> position
		{
			get
			{
				return this.stylesData.position.ToStyleEnum((Position)this.stylesData.position.value);
			}
		}

		public StyleEnum<Align> alignSelf
		{
			get
			{
				return this.stylesData.alignSelf.ToStyleEnum((Align)this.stylesData.alignSelf.value);
			}
		}

		public StyleColor backgroundColor
		{
			get
			{
				return this.stylesData.backgroundColor;
			}
		}

		public StyleBackground backgroundImage
		{
			get
			{
				return this.stylesData.backgroundImage;
			}
		}

		public StyleEnum<ScaleMode> unityBackgroundScaleMode
		{
			get
			{
				return this.stylesData.unityBackgroundScaleMode.ToStyleEnum((ScaleMode)this.stylesData.unityBackgroundScaleMode.value);
			}
		}

		public StyleColor unityBackgroundImageTintColor
		{
			get
			{
				return this.stylesData.unityBackgroundImageTintColor;
			}
		}

		public StyleEnum<Align> alignItems
		{
			get
			{
				return this.stylesData.alignItems.ToStyleEnum((Align)this.stylesData.alignItems.value);
			}
		}

		public StyleEnum<Align> alignContent
		{
			get
			{
				return this.stylesData.alignContent.ToStyleEnum((Align)this.stylesData.alignContent.value);
			}
		}

		public StyleEnum<Justify> justifyContent
		{
			get
			{
				return this.stylesData.justifyContent.ToStyleEnum((Justify)this.stylesData.justifyContent.value);
			}
		}

		public StyleColor borderLeftColor
		{
			get
			{
				return this.stylesData.borderLeftColor;
			}
		}

		public StyleColor borderTopColor
		{
			get
			{
				return this.stylesData.borderTopColor;
			}
		}

		public StyleColor borderRightColor
		{
			get
			{
				return this.stylesData.borderRightColor;
			}
		}

		public StyleColor borderBottomColor
		{
			get
			{
				return this.stylesData.borderBottomColor;
			}
		}

		public StyleFloat borderLeftWidth
		{
			get
			{
				return this.stylesData.borderLeftWidth;
			}
		}

		public StyleFloat borderTopWidth
		{
			get
			{
				return this.stylesData.borderTopWidth;
			}
		}

		public StyleFloat borderRightWidth
		{
			get
			{
				return this.stylesData.borderRightWidth;
			}
		}

		public StyleFloat borderBottomWidth
		{
			get
			{
				return this.stylesData.borderBottomWidth;
			}
		}

		public StyleLength borderTopLeftRadius
		{
			get
			{
				return this.stylesData.borderTopLeftRadius;
			}
		}

		public StyleLength borderTopRightRadius
		{
			get
			{
				return this.stylesData.borderTopRightRadius;
			}
		}

		public StyleLength borderBottomRightRadius
		{
			get
			{
				return this.stylesData.borderBottomRightRadius;
			}
		}

		public StyleLength borderBottomLeftRadius
		{
			get
			{
				return this.stylesData.borderBottomLeftRadius;
			}
		}

		public StyleInt unitySliceLeft
		{
			get
			{
				return this.stylesData.unitySliceLeft;
			}
		}

		public StyleInt unitySliceTop
		{
			get
			{
				return this.stylesData.unitySliceTop;
			}
		}

		public StyleInt unitySliceRight
		{
			get
			{
				return this.stylesData.unitySliceRight;
			}
		}

		public StyleInt unitySliceBottom
		{
			get
			{
				return this.stylesData.unitySliceBottom;
			}
		}

		public StyleFloat opacity
		{
			get
			{
				return this.stylesData.opacity;
			}
		}

		public StyleEnum<DisplayStyle> display
		{
			get
			{
				return this.stylesData.display.ToStyleEnum((DisplayStyle)this.stylesData.display.value);
			}
		}

		public StyleCursor cursor
		{
			get
			{
				return this.stylesData.cursor;
			}
		}

		public StyleColor color
		{
			get
			{
				return (this.stylesData.color.specificity != 0) ? this.stylesData.color : this.inheritedStylesData.color;
			}
		}

		public StyleFont unityFont
		{
			get
			{
				return (this.stylesData.unityFont.specificity != 0) ? this.stylesData.unityFont : this.inheritedStylesData.font;
			}
		}

		public StyleLength fontSize
		{
			get
			{
				int specificity = this.stylesData.fontSize.specificity;
				bool flag = specificity != 0;
				StyleLength styleLength;
				if (flag)
				{
					float num = ComputedStyle.CalculatePixelFontSize(this.m_Element);
					styleLength = new StyleLength(num)
					{
						specificity = specificity
					};
				}
				else
				{
					styleLength = this.inheritedStylesData.fontSize;
				}
				return styleLength;
			}
		}

		public StyleEnum<FontStyle> unityFontStyleAndWeight
		{
			get
			{
				StyleInt styleInt = ((this.stylesData.unityFontStyleAndWeight.specificity != 0) ? this.stylesData.unityFontStyleAndWeight : this.inheritedStylesData.unityFontStyle);
				return styleInt.ToStyleEnum((FontStyle)styleInt.value);
			}
		}

		public StyleEnum<TextAnchor> unityTextAlign
		{
			get
			{
				StyleInt styleInt = ((this.stylesData.unityTextAlign.specificity != 0) ? this.stylesData.unityTextAlign : this.inheritedStylesData.unityTextAlign);
				return styleInt.ToStyleEnum((TextAnchor)styleInt.value);
			}
		}

		public StyleEnum<Visibility> visibility
		{
			get
			{
				StyleInt styleInt = ((this.stylesData.visibility.specificity != 0) ? this.stylesData.visibility : this.inheritedStylesData.visibility);
				return styleInt.ToStyleEnum((Visibility)styleInt.value);
			}
		}

		public StyleEnum<WhiteSpace> whiteSpace
		{
			get
			{
				StyleInt styleInt = ((this.stylesData.whiteSpace.specificity != 0) ? this.stylesData.whiteSpace : this.inheritedStylesData.whiteSpace);
				return styleInt.ToStyleEnum((WhiteSpace)styleInt.value);
			}
		}

		public static float CalculatePixelFontSize(VisualElement ve)
		{
			Length value = ve.specifiedStyle.fontSize.value;
			bool flag = value.unit == LengthUnit.Percent;
			if (flag)
			{
				VisualElement parent = ve.hierarchy.parent;
				float num = ((parent != null) ? parent.resolvedStyle.fontSize : 0f);
				float num2 = num * value.value / 100f;
				value = new Length(num2);
			}
			return value.value;
		}

		private VisualElement m_Element;
	}
}
