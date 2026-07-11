using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.Yoga;

namespace UnityEngine.UIElements
{
	internal class InlineStyleAccess : StyleValueCollection, IStyle
	{
		private VisualElement ve { get; set; }

		public InlineStyleAccess(VisualElement ve)
		{
			this.ve = ve;
			bool isShared = ve.specifiedStyle.isShared;
			if (isShared)
			{
				VisualElementStylesData visualElementStylesData = new VisualElementStylesData(false);
				visualElementStylesData.Apply(ve.m_SharedStyle, StylePropertyApplyMode.Copy);
				ve.m_Style = visualElementStylesData;
			}
		}

		protected override void Finalize()
		{
			try
			{
				StyleValue styleValue = default(StyleValue);
				bool flag = base.TryGetStyleValue(StylePropertyID.BackgroundImage, ref styleValue);
				if (flag)
				{
					bool isAllocated = styleValue.resource.IsAllocated;
					if (isAllocated)
					{
						styleValue.resource.Free();
					}
				}
				bool flag2 = base.TryGetStyleValue(StylePropertyID.Font, ref styleValue);
				if (flag2)
				{
					bool isAllocated2 = styleValue.resource.IsAllocated;
					if (isAllocated2)
					{
						styleValue.resource.Free();
					}
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		StyleLength IStyle.width
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.Width);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.Width, value, this.ve.sharedStyle.width);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Width = this.ve.computedStyle.width.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.height
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.Height);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.Height, value, this.ve.sharedStyle.height);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Height = this.ve.computedStyle.height.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.maxWidth
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MaxWidth);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MaxWidth, value, this.ve.sharedStyle.maxWidth);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MaxWidth = this.ve.computedStyle.maxWidth.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.maxHeight
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MaxHeight);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MaxHeight, value, this.ve.sharedStyle.maxHeight);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MaxHeight = this.ve.computedStyle.maxHeight.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.minWidth
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MinWidth);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MinWidth, value, this.ve.sharedStyle.minWidth);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MinWidth = this.ve.computedStyle.minWidth.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.minHeight
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MinHeight);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MinHeight, value, this.ve.sharedStyle.minHeight);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MinHeight = this.ve.computedStyle.minHeight.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.flexBasis
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.FlexBasis);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.FlexBasis, value, this.ve.sharedStyle.flexBasis);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.FlexBasis = this.ve.computedStyle.flexBasis.ToYogaValue();
				}
			}
		}

		StyleFloat IStyle.flexGrow
		{
			get
			{
				return base.GetStyleFloat(StylePropertyID.FlexGrow);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.FlexGrow, value, this.ve.sharedStyle.flexGrow);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.FlexGrow = this.ve.computedStyle.flexGrow.value;
				}
			}
		}

		StyleFloat IStyle.flexShrink
		{
			get
			{
				return base.GetStyleFloat(StylePropertyID.FlexShrink);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.FlexShrink, value, this.ve.sharedStyle.flexShrink);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.FlexShrink = this.ve.computedStyle.flexShrink.value;
				}
			}
		}

		StyleEnum<Overflow> IStyle.overflow
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.Overflow);
				return new StyleEnum<Overflow>((Overflow)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Overflow>(StylePropertyID.Overflow, value, this.ve.sharedStyle.overflow);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.Overflow);
					this.ve.yogaNode.Overflow = (YogaOverflow)this.ve.computedStyle.overflow.value;
				}
			}
		}

		StyleEnum<OverflowClipBox> IStyle.unityOverflowClipBox
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.OverflowClipBox);
				return new StyleEnum<OverflowClipBox>((OverflowClipBox)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<OverflowClipBox>(StylePropertyID.OverflowClipBox, value, this.ve.sharedStyle.unityOverflowClipBox);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleLength IStyle.left
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PositionLeft);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PositionLeft, value, this.ve.sharedStyle.left);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Left = this.ve.computedStyle.left.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.top
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PositionTop);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PositionTop, value, this.ve.sharedStyle.top);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Top = this.ve.computedStyle.top.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.right
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PositionRight);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PositionRight, value, this.ve.sharedStyle.right);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Right = this.ve.computedStyle.right.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.bottom
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PositionBottom);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PositionBottom, value, this.ve.sharedStyle.bottom);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Bottom = this.ve.computedStyle.bottom.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.marginLeft
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MarginLeft);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MarginLeft, value, this.ve.sharedStyle.marginLeft);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MarginLeft = this.ve.computedStyle.marginLeft.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.marginTop
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MarginTop);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MarginTop, value, this.ve.sharedStyle.marginTop);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MarginTop = this.ve.computedStyle.marginTop.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.marginRight
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MarginRight);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MarginRight, value, this.ve.sharedStyle.marginRight);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MarginRight = this.ve.computedStyle.marginRight.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.marginBottom
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.MarginBottom);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.MarginBottom, value, this.ve.sharedStyle.marginBottom);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.MarginBottom = this.ve.computedStyle.marginBottom.ToYogaValue();
				}
			}
		}

		StyleColor IStyle.borderLeftColor
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.BorderLeftColor);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderLeftColor, value, this.ve.sharedStyle.borderLeftColor);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleColor IStyle.borderTopColor
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.BorderTopColor);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderTopColor, value, this.ve.sharedStyle.borderTopColor);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleColor IStyle.borderRightColor
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.BorderRightColor);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderRightColor, value, this.ve.sharedStyle.borderRightColor);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleColor IStyle.borderBottomColor
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.BorderBottomColor);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderBottomColor, value, this.ve.sharedStyle.borderBottomColor);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleFloat IStyle.borderLeftWidth
		{
			get
			{
				return base.GetStyleFloat(StylePropertyID.BorderLeftWidth);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderLeftWidth, value, this.ve.sharedStyle.borderLeftWidth);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
					this.ve.yogaNode.BorderLeftWidth = this.ve.computedStyle.borderLeftWidth.value;
				}
			}
		}

		StyleFloat IStyle.borderTopWidth
		{
			get
			{
				return base.GetStyleFloat(StylePropertyID.BorderTopWidth);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderTopWidth, value, this.ve.sharedStyle.borderTopWidth);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
					this.ve.yogaNode.BorderTopWidth = this.ve.computedStyle.borderTopWidth.value;
				}
			}
		}

		StyleFloat IStyle.borderRightWidth
		{
			get
			{
				return base.GetStyleFloat(StylePropertyID.BorderRightWidth);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderRightWidth, value, this.ve.sharedStyle.borderRightWidth);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
					this.ve.yogaNode.BorderRightWidth = this.ve.computedStyle.borderRightWidth.value;
				}
			}
		}

		StyleFloat IStyle.borderBottomWidth
		{
			get
			{
				return base.GetStyleFloat(StylePropertyID.BorderBottomWidth);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderBottomWidth, value, this.ve.sharedStyle.borderBottomWidth);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles | VersionChangeType.BorderWidth | VersionChangeType.Repaint);
					this.ve.yogaNode.BorderBottomWidth = this.ve.computedStyle.borderBottomWidth.value;
				}
			}
		}

		StyleLength IStyle.borderTopLeftRadius
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.BorderTopLeftRadius);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderTopLeftRadius, value, this.ve.sharedStyle.borderTopLeftRadius);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
				}
			}
		}

		StyleLength IStyle.borderTopRightRadius
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.BorderTopRightRadius);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderTopRightRadius, value, this.ve.sharedStyle.borderTopRightRadius);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
				}
			}
		}

		StyleLength IStyle.borderBottomRightRadius
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.BorderBottomRightRadius);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderBottomRightRadius, value, this.ve.sharedStyle.borderBottomRightRadius);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
				}
			}
		}

		StyleLength IStyle.borderBottomLeftRadius
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.BorderBottomLeftRadius);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderBottomLeftRadius, value, this.ve.sharedStyle.borderBottomLeftRadius);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.BorderRadius | VersionChangeType.Repaint);
				}
			}
		}

		StyleLength IStyle.paddingLeft
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PaddingLeft);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PaddingLeft, value, this.ve.sharedStyle.paddingLeft);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.PaddingLeft = this.ve.computedStyle.paddingLeft.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.paddingTop
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PaddingTop);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PaddingTop, value, this.ve.sharedStyle.paddingTop);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.PaddingTop = this.ve.computedStyle.paddingTop.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.paddingRight
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PaddingRight);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PaddingRight, value, this.ve.sharedStyle.paddingRight);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.PaddingRight = this.ve.computedStyle.paddingRight.ToYogaValue();
				}
			}
		}

		StyleLength IStyle.paddingBottom
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.PaddingBottom);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.PaddingBottom, value, this.ve.sharedStyle.paddingBottom);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.PaddingBottom = this.ve.computedStyle.paddingBottom.ToYogaValue();
				}
			}
		}

		StyleEnum<Position> IStyle.position
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.Position);
				return new StyleEnum<Position>((Position)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Position>(StylePropertyID.Position, value, this.ve.sharedStyle.position);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.PositionType = (YogaPositionType)this.ve.computedStyle.position.value;
				}
			}
		}

		StyleEnum<Align> IStyle.alignSelf
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.AlignSelf);
				return new StyleEnum<Align>((Align)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Align>(StylePropertyID.AlignSelf, value, this.ve.sharedStyle.alignSelf);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.AlignSelf = (YogaAlign)this.ve.computedStyle.alignSelf.value;
				}
			}
		}

		StyleEnum<TextAnchor> IStyle.unityTextAlign
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.UnityTextAlign);
				return new StyleEnum<TextAnchor>((TextAnchor)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<TextAnchor>(StylePropertyID.UnityTextAlign, value, this.ve.sharedStyle.unityTextAlign);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleEnum<FontStyle> IStyle.unityFontStyleAndWeight
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.FontStyleAndWeight);
				return new StyleEnum<FontStyle>((FontStyle)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<FontStyle>(StylePropertyID.FontStyleAndWeight, value, this.ve.sharedStyle.unityFontStyleAndWeight);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles);
				}
			}
		}

		StyleFont IStyle.unityFont
		{
			get
			{
				return base.GetStyleFont(StylePropertyID.Font);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.Font, value, this.ve.sharedStyle.unityFont);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles);
				}
			}
		}

		StyleLength IStyle.fontSize
		{
			get
			{
				return base.GetStyleLength(StylePropertyID.FontSize);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.FontSize, value, this.ve.sharedStyle.fontSize);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles);
				}
			}
		}

		StyleEnum<WhiteSpace> IStyle.whiteSpace
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.WhiteSpace);
				return new StyleEnum<WhiteSpace>((WhiteSpace)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<WhiteSpace>(StylePropertyID.WhiteSpace, value, this.ve.sharedStyle.whiteSpace);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Styles);
				}
			}
		}

		StyleColor IStyle.color
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.Color);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.Color, value, this.ve.sharedStyle.color);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleEnum<FlexDirection> IStyle.flexDirection
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.FlexDirection);
				return new StyleEnum<FlexDirection>((FlexDirection)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<FlexDirection>(StylePropertyID.FlexDirection, value, this.ve.sharedStyle.flexDirection);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
					this.ve.yogaNode.FlexDirection = (YogaFlexDirection)this.ve.computedStyle.flexDirection.value;
				}
			}
		}

		StyleColor IStyle.backgroundColor
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.BackgroundColor);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BackgroundColor, value, this.ve.sharedStyle.backgroundColor);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleColor IStyle.borderColor
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.BorderLeftColor);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BorderLeftColor, value, this.ve.sharedStyle.borderLeftColor);
				flag |= this.SetStyleValue(StylePropertyID.BorderTopColor, value, this.ve.sharedStyle.borderTopColor);
				flag |= this.SetStyleValue(StylePropertyID.BorderRightColor, value, this.ve.sharedStyle.borderRightColor);
				flag |= this.SetStyleValue(StylePropertyID.BorderBottomColor, value, this.ve.sharedStyle.borderBottomColor);
				bool flag2 = flag;
				if (flag2)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleBackground IStyle.backgroundImage
		{
			get
			{
				return base.GetStyleBackground(StylePropertyID.BackgroundImage);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BackgroundImage, value, this.ve.sharedStyle.backgroundImage);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleEnum<ScaleMode> IStyle.unityBackgroundScaleMode
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.BackgroundScaleMode);
				return new StyleEnum<ScaleMode>((ScaleMode)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<ScaleMode>(StylePropertyID.BackgroundScaleMode, value, this.ve.sharedStyle.unityBackgroundScaleMode);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleColor IStyle.unityBackgroundImageTintColor
		{
			get
			{
				return base.GetStyleColor(StylePropertyID.BackgroundImageTintColor);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.BackgroundImageTintColor, value, this.ve.sharedStyle.unityBackgroundImageTintColor);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleEnum<Align> IStyle.alignItems
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.AlignItems);
				return new StyleEnum<Align>((Align)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Align>(StylePropertyID.AlignItems, value, this.ve.sharedStyle.alignItems);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.AlignItems = (YogaAlign)this.ve.computedStyle.alignItems.value;
				}
			}
		}

		StyleEnum<Align> IStyle.alignContent
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.AlignContent);
				return new StyleEnum<Align>((Align)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Align>(StylePropertyID.AlignContent, value, this.ve.sharedStyle.alignContent);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.AlignContent = (YogaAlign)this.ve.computedStyle.alignContent.value;
				}
			}
		}

		StyleEnum<Justify> IStyle.justifyContent
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.JustifyContent);
				return new StyleEnum<Justify>((Justify)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Justify>(StylePropertyID.JustifyContent, value, this.ve.sharedStyle.justifyContent);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.JustifyContent = (YogaJustify)this.ve.computedStyle.justifyContent.value;
				}
			}
		}

		StyleEnum<Wrap> IStyle.flexWrap
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.FlexWrap);
				return new StyleEnum<Wrap>((Wrap)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Wrap>(StylePropertyID.FlexWrap, value, this.ve.sharedStyle.flexWrap);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Wrap = (YogaWrap)this.ve.computedStyle.flexWrap.value;
				}
			}
		}

		StyleInt IStyle.unitySliceLeft
		{
			get
			{
				return base.GetStyleInt(StylePropertyID.SliceLeft);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.SliceLeft, value, this.ve.sharedStyle.unitySliceLeft);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleInt IStyle.unitySliceTop
		{
			get
			{
				return base.GetStyleInt(StylePropertyID.SliceTop);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.SliceTop, value, this.ve.sharedStyle.unitySliceTop);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleInt IStyle.unitySliceRight
		{
			get
			{
				return base.GetStyleInt(StylePropertyID.SliceRight);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.SliceRight, value, this.ve.sharedStyle.unitySliceRight);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleInt IStyle.unitySliceBottom
		{
			get
			{
				return base.GetStyleInt(StylePropertyID.SliceBottom);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.SliceBottom, value, this.ve.sharedStyle.unitySliceBottom);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleFloat IStyle.opacity
		{
			get
			{
				return base.GetStyleFloat(StylePropertyID.Opacity);
			}
			set
			{
				bool flag = this.SetStyleValue(StylePropertyID.Opacity, value, this.ve.sharedStyle.opacity);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles | VersionChangeType.Opacity);
				}
			}
		}

		StyleEnum<Visibility> IStyle.visibility
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.Visibility);
				return new StyleEnum<Visibility>((Visibility)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<Visibility>(StylePropertyID.Visibility, value, this.ve.sharedStyle.visibility);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.StyleSheet | VersionChangeType.Styles | VersionChangeType.Repaint);
				}
			}
		}

		StyleCursor IStyle.cursor
		{
			get
			{
				StyleCursor styleCursor = default(StyleCursor);
				bool flag = this.TryGetInlineCursor(ref styleCursor);
				StyleCursor styleCursor2;
				if (flag)
				{
					styleCursor2 = styleCursor;
				}
				else
				{
					styleCursor2 = StyleKeyword.Null;
				}
				return styleCursor2;
			}
			set
			{
				bool flag = this.SetInlineCursor(value, this.ve.sharedStyle.cursor);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Styles);
				}
			}
		}

		StyleEnum<DisplayStyle> IStyle.display
		{
			get
			{
				StyleInt styleInt = base.GetStyleInt(StylePropertyID.Display);
				return new StyleEnum<DisplayStyle>((DisplayStyle)styleInt.value, styleInt.keyword);
			}
			set
			{
				bool flag = this.SetStyleValue<DisplayStyle>(StylePropertyID.Display, value, this.ve.sharedStyle.display);
				if (flag)
				{
					this.ve.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Styles);
					this.ve.yogaNode.Display = (YogaDisplay)this.ve.computedStyle.display.value;
				}
			}
		}

		private bool SetStyleValue(StylePropertyID id, StyleLength inlineValue, StyleLength sharedValue)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = base.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				bool flag2 = styleValue.length == inlineValue.value && styleValue.keyword == inlineValue.keyword;
				if (flag2)
				{
					return false;
				}
			}
			styleValue.id = id;
			styleValue.keyword = inlineValue.keyword;
			styleValue.length = inlineValue.value;
			base.SetStyleValue(styleValue);
			int num = int.MaxValue;
			bool flag3 = inlineValue.keyword == StyleKeyword.Null;
			if (flag3)
			{
				num = sharedValue.specificity;
				styleValue.keyword = sharedValue.keyword;
				styleValue.length = sharedValue.value;
			}
			this.ApplyStyleValue(styleValue, num);
			return true;
		}

		private bool SetStyleValue(StylePropertyID id, StyleFloat inlineValue, StyleFloat sharedValue)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = base.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				bool flag2 = styleValue.number == inlineValue.value && styleValue.keyword == inlineValue.keyword;
				if (flag2)
				{
					return false;
				}
			}
			styleValue.id = id;
			styleValue.keyword = inlineValue.keyword;
			styleValue.number = inlineValue.value;
			base.SetStyleValue(styleValue);
			int num = int.MaxValue;
			bool flag3 = inlineValue.keyword == StyleKeyword.Null;
			if (flag3)
			{
				num = sharedValue.specificity;
				styleValue.keyword = sharedValue.keyword;
				styleValue.number = sharedValue.value;
			}
			this.ApplyStyleValue(styleValue, num);
			return true;
		}

		private bool SetStyleValue(StylePropertyID id, StyleInt inlineValue, StyleInt sharedValue)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = base.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				bool flag2 = styleValue.number == (float)inlineValue.value && styleValue.keyword == inlineValue.keyword;
				if (flag2)
				{
					return false;
				}
			}
			styleValue.id = id;
			styleValue.keyword = inlineValue.keyword;
			styleValue.number = (float)inlineValue.value;
			base.SetStyleValue(styleValue);
			int num = int.MaxValue;
			bool flag3 = inlineValue.keyword == StyleKeyword.Null;
			if (flag3)
			{
				num = sharedValue.specificity;
				styleValue.keyword = sharedValue.keyword;
				styleValue.number = (float)sharedValue.value;
			}
			this.ApplyStyleValue(styleValue, num);
			return true;
		}

		private bool SetStyleValue(StylePropertyID id, StyleColor inlineValue, StyleColor sharedValue)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = base.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				bool flag2 = styleValue.color == inlineValue.value && styleValue.keyword == inlineValue.keyword;
				if (flag2)
				{
					return false;
				}
			}
			styleValue.id = id;
			styleValue.keyword = inlineValue.keyword;
			styleValue.color = inlineValue.value;
			base.SetStyleValue(styleValue);
			int num = int.MaxValue;
			bool flag3 = inlineValue.keyword == StyleKeyword.Null;
			if (flag3)
			{
				num = sharedValue.specificity;
				styleValue.keyword = sharedValue.keyword;
				styleValue.color = sharedValue.value;
			}
			this.ApplyStyleValue(styleValue, num);
			return true;
		}

		private bool SetStyleValue<T>(StylePropertyID id, StyleEnum<T> inlineValue, StyleInt sharedValue) where T : struct, IConvertible
		{
			StyleValue styleValue = default(StyleValue);
			int num = UnsafeUtility.EnumToInt<T>(inlineValue.value);
			bool flag = base.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				bool flag2 = styleValue.number == (float)num && styleValue.keyword == inlineValue.keyword;
				if (flag2)
				{
					return false;
				}
			}
			styleValue.id = id;
			styleValue.keyword = inlineValue.keyword;
			styleValue.number = (float)num;
			base.SetStyleValue(styleValue);
			int num2 = int.MaxValue;
			bool flag3 = inlineValue.keyword == StyleKeyword.Null;
			if (flag3)
			{
				num2 = sharedValue.specificity;
				styleValue.keyword = sharedValue.keyword;
				styleValue.number = (float)sharedValue.value;
			}
			this.ApplyStyleValue(styleValue, num2);
			return true;
		}

		private bool SetStyleValue(StylePropertyID id, StyleBackground inlineValue, StyleBackground sharedValue)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = base.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				VectorImage vectorImage = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as VectorImage) : null);
				Texture2D texture2D = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as Texture2D) : null);
				bool flag2 = vectorImage == inlineValue.value.vectorImage && texture2D == inlineValue.value.texture && styleValue.keyword == inlineValue.keyword;
				if (flag2)
				{
					return false;
				}
				bool isAllocated = styleValue.resource.IsAllocated;
				if (isAllocated)
				{
					styleValue.resource.Free();
				}
			}
			styleValue.id = id;
			styleValue.keyword = inlineValue.keyword;
			bool flag3 = inlineValue.value.vectorImage != null;
			if (flag3)
			{
				styleValue.resource = GCHandle.Alloc(inlineValue.value.vectorImage);
			}
			else
			{
				bool flag4 = inlineValue.value.texture != null;
				if (flag4)
				{
					styleValue.resource = GCHandle.Alloc(inlineValue.value.texture);
				}
				else
				{
					styleValue.resource = default(GCHandle);
				}
			}
			base.SetStyleValue(styleValue);
			int num = int.MaxValue;
			bool flag5 = inlineValue.keyword == StyleKeyword.Null;
			if (flag5)
			{
				num = sharedValue.specificity;
				styleValue.keyword = sharedValue.keyword;
				bool flag6 = sharedValue.value.texture != null;
				if (flag6)
				{
					styleValue.resource = GCHandle.Alloc(sharedValue.value.texture);
				}
				else
				{
					bool flag7 = sharedValue.value.vectorImage != null;
					if (flag7)
					{
						styleValue.resource = GCHandle.Alloc(sharedValue.value.vectorImage);
					}
					else
					{
						styleValue.resource = default(GCHandle);
					}
				}
			}
			this.ApplyStyleValue(styleValue, num);
			return true;
		}

		private bool SetStyleValue(StylePropertyID id, StyleFont inlineValue, StyleFont sharedValue)
		{
			StyleValue styleValue = default(StyleValue);
			bool flag = base.TryGetStyleValue(id, ref styleValue);
			if (flag)
			{
				bool isAllocated = styleValue.resource.IsAllocated;
				if (isAllocated)
				{
					Font font = (styleValue.resource.IsAllocated ? (styleValue.resource.Target as Font) : null);
					bool flag2 = font == inlineValue.value && styleValue.keyword == inlineValue.keyword;
					if (flag2)
					{
						return false;
					}
					bool isAllocated2 = styleValue.resource.IsAllocated;
					if (isAllocated2)
					{
						styleValue.resource.Free();
					}
				}
			}
			styleValue.id = id;
			styleValue.keyword = inlineValue.keyword;
			styleValue.resource = ((inlineValue.value != null) ? GCHandle.Alloc(inlineValue.value) : default(GCHandle));
			base.SetStyleValue(styleValue);
			int num = int.MaxValue;
			bool flag3 = inlineValue.keyword == StyleKeyword.Null;
			if (flag3)
			{
				num = sharedValue.specificity;
				styleValue.keyword = sharedValue.keyword;
				styleValue.resource = ((sharedValue.value != null) ? GCHandle.Alloc(sharedValue.value) : default(GCHandle));
			}
			this.ApplyStyleValue(styleValue, num);
			return true;
		}

		private bool SetInlineCursor(StyleCursor inlineValue, StyleCursor sharedValue)
		{
			StyleCursor styleCursor = default(StyleCursor);
			bool flag = this.TryGetInlineCursor(ref styleCursor);
			if (flag)
			{
				bool flag2 = styleCursor.value == inlineValue.value && styleCursor.keyword == inlineValue.keyword;
				if (flag2)
				{
					return false;
				}
			}
			styleCursor.value = inlineValue.value;
			styleCursor.keyword = inlineValue.keyword;
			this.SetInlineCursor(styleCursor);
			int num = int.MaxValue;
			bool flag3 = styleCursor.keyword == StyleKeyword.Null;
			if (flag3)
			{
				num = sharedValue.specificity;
				styleCursor.keyword = sharedValue.keyword;
				styleCursor.value = sharedValue.value;
			}
			this.ve.specifiedStyle.ApplyStyleCursor(styleCursor, num);
			return true;
		}

		private void ApplyStyleValue(StyleValue value, int specificity)
		{
			this.ve.specifiedStyle.ApplyStyleValue(value.id, value, specificity);
		}

		public bool TryGetInlineCursor(ref StyleCursor value)
		{
			bool hasInlineCursor = this.m_HasInlineCursor;
			bool flag;
			if (hasInlineCursor)
			{
				value = this.m_InlineCursor;
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public void SetInlineCursor(StyleCursor value)
		{
			this.m_InlineCursor = value;
			this.m_HasInlineCursor = true;
		}

		private bool m_HasInlineCursor;

		private StyleCursor m_InlineCursor;
	}
}
