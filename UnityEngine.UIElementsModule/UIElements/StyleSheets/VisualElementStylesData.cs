using System;
using System.Collections.Generic;
using UnityEngine.Yoga;

namespace UnityEngine.UIElements.StyleSheets
{
	internal class VisualElementStylesData : ICustomStyle
	{
		public int customPropertiesCount
		{
			get
			{
				return (this.m_CustomProperties != null) ? this.m_CustomProperties.Count : 0;
			}
		}

		public VisualElementStylesData(bool isShared)
		{
			this.isShared = isShared;
			this.dpiScaling = GUIUtility.pixelsPerPoint;
			this.left = StyleSheetCache.GetInitialValue(StylePropertyID.PositionLeft).ToStyleLength();
			this.top = StyleSheetCache.GetInitialValue(StylePropertyID.PositionTop).ToStyleLength();
			this.right = StyleSheetCache.GetInitialValue(StylePropertyID.PositionRight).ToStyleLength();
			this.bottom = StyleSheetCache.GetInitialValue(StylePropertyID.PositionBottom).ToStyleLength();
			this.width = StyleSheetCache.GetInitialValue(StylePropertyID.Width).ToStyleLength();
			this.height = StyleSheetCache.GetInitialValue(StylePropertyID.Height).ToStyleLength();
			this.minWidth = StyleSheetCache.GetInitialValue(StylePropertyID.MinWidth).ToStyleLength();
			this.minHeight = StyleSheetCache.GetInitialValue(StylePropertyID.MinHeight).ToStyleLength();
			this.maxWidth = StyleSheetCache.GetInitialValue(StylePropertyID.MaxWidth).ToStyleLength();
			this.maxHeight = StyleSheetCache.GetInitialValue(StylePropertyID.MaxHeight).ToStyleLength();
			this.alignSelf = (int)StyleSheetCache.GetInitialValue(StylePropertyID.AlignSelf).number;
			this.alignItems = (int)StyleSheetCache.GetInitialValue(StylePropertyID.AlignItems).number;
			this.alignContent = (int)StyleSheetCache.GetInitialValue(StylePropertyID.AlignContent).number;
			this.flexGrow = StyleSheetCache.GetInitialValue(StylePropertyID.FlexGrow).ToStyleFloat();
			this.flexShrink = StyleSheetCache.GetInitialValue(StylePropertyID.FlexShrink).ToStyleFloat();
			this.flexBasis = StyleSheetCache.GetInitialValue(StylePropertyID.FlexBasis).ToStyleLength();
			this.color = StyleSheetCache.GetInitialValue(StylePropertyID.Color).color;
			this.borderLeftColor = StyleSheetCache.GetInitialValue(StylePropertyID.BorderLeftColor).color;
			this.borderTopColor = StyleSheetCache.GetInitialValue(StylePropertyID.BorderTopColor).color;
			this.borderRightColor = StyleSheetCache.GetInitialValue(StylePropertyID.BorderRightColor).color;
			this.borderBottomColor = StyleSheetCache.GetInitialValue(StylePropertyID.BorderBottomColor).color;
			this.opacity = StyleSheetCache.GetInitialValue(StylePropertyID.Opacity).number;
			this.unityBackgroundImageTintColor = StyleSheetCache.GetInitialValue(StylePropertyID.BackgroundImageTintColor).color;
		}

		public void Apply(VisualElementStylesData other, StylePropertyApplyMode mode)
		{
			this.m_CustomProperties = other.m_CustomProperties;
			this.width.Apply<StyleLength>(other.width, mode);
			this.height.Apply<StyleLength>(other.height, mode);
			this.maxWidth.Apply<StyleLength>(other.maxWidth, mode);
			this.maxHeight.Apply<StyleLength>(other.maxHeight, mode);
			this.minWidth.Apply<StyleLength>(other.minWidth, mode);
			this.minHeight.Apply<StyleLength>(other.minHeight, mode);
			this.flexBasis.Apply<StyleLength>(other.flexBasis, mode);
			this.flexGrow.Apply<StyleFloat>(other.flexGrow, mode);
			this.flexShrink.Apply<StyleFloat>(other.flexShrink, mode);
			this.overflow.Apply<StyleInt>(other.overflow, mode);
			this.unityOverflowClipBox.Apply<StyleInt>(other.unityOverflowClipBox, mode);
			this.left.Apply<StyleLength>(other.left, mode);
			this.top.Apply<StyleLength>(other.top, mode);
			this.right.Apply<StyleLength>(other.right, mode);
			this.bottom.Apply<StyleLength>(other.bottom, mode);
			this.marginLeft.Apply<StyleLength>(other.marginLeft, mode);
			this.marginTop.Apply<StyleLength>(other.marginTop, mode);
			this.marginRight.Apply<StyleLength>(other.marginRight, mode);
			this.marginBottom.Apply<StyleLength>(other.marginBottom, mode);
			this.paddingLeft.Apply<StyleLength>(other.paddingLeft, mode);
			this.paddingTop.Apply<StyleLength>(other.paddingTop, mode);
			this.paddingRight.Apply<StyleLength>(other.paddingRight, mode);
			this.paddingBottom.Apply<StyleLength>(other.paddingBottom, mode);
			this.position.Apply<StyleInt>(other.position, mode);
			this.alignSelf.Apply<StyleInt>(other.alignSelf, mode);
			this.unityTextAlign.Apply<StyleInt>(other.unityTextAlign, mode);
			this.unityFontStyleAndWeight.Apply<StyleInt>(other.unityFontStyleAndWeight, mode);
			this.fontSize.Apply<StyleLength>(other.fontSize, mode);
			this.unityFont.Apply<StyleFont>(other.unityFont, mode);
			this.whiteSpace.Apply<StyleInt>(other.whiteSpace, mode);
			this.color.Apply<StyleColor>(other.color, mode);
			this.flexDirection.Apply<StyleInt>(other.flexDirection, mode);
			this.backgroundColor.Apply<StyleColor>(other.backgroundColor, mode);
			this.backgroundImage.Apply<StyleBackground>(other.backgroundImage, mode);
			this.unityBackgroundScaleMode.Apply<StyleInt>(other.unityBackgroundScaleMode, mode);
			this.unityBackgroundImageTintColor.Apply<StyleColor>(other.unityBackgroundImageTintColor, mode);
			this.alignItems.Apply<StyleInt>(other.alignItems, mode);
			this.alignContent.Apply<StyleInt>(other.alignContent, mode);
			this.justifyContent.Apply<StyleInt>(other.justifyContent, mode);
			this.flexWrap.Apply<StyleInt>(other.flexWrap, mode);
			this.borderLeftColor.Apply<StyleColor>(other.borderLeftColor, mode);
			this.borderTopColor.Apply<StyleColor>(other.borderTopColor, mode);
			this.borderRightColor.Apply<StyleColor>(other.borderRightColor, mode);
			this.borderBottomColor.Apply<StyleColor>(other.borderBottomColor, mode);
			this.borderLeftWidth.Apply<StyleFloat>(other.borderLeftWidth, mode);
			this.borderTopWidth.Apply<StyleFloat>(other.borderTopWidth, mode);
			this.borderRightWidth.Apply<StyleFloat>(other.borderRightWidth, mode);
			this.borderBottomWidth.Apply<StyleFloat>(other.borderBottomWidth, mode);
			this.borderTopLeftRadius.Apply<StyleLength>(other.borderTopLeftRadius, mode);
			this.borderTopRightRadius.Apply<StyleLength>(other.borderTopRightRadius, mode);
			this.borderBottomRightRadius.Apply<StyleLength>(other.borderBottomRightRadius, mode);
			this.borderBottomLeftRadius.Apply<StyleLength>(other.borderBottomLeftRadius, mode);
			this.unitySliceLeft.Apply<StyleInt>(other.unitySliceLeft, mode);
			this.unitySliceTop.Apply<StyleInt>(other.unitySliceTop, mode);
			this.unitySliceRight.Apply<StyleInt>(other.unitySliceRight, mode);
			this.unitySliceBottom.Apply<StyleInt>(other.unitySliceBottom, mode);
			this.opacity.Apply<StyleFloat>(other.opacity, mode);
			this.cursor.Apply<StyleCursor>(other.cursor, mode);
			this.visibility.Apply<StyleInt>(other.visibility, mode);
			this.display.Apply<StyleInt>(other.display, mode);
			this.dpiScaling = other.dpiScaling;
		}

		public void ApplyLayoutValues()
		{
			bool flag = this.yogaNode == null;
			if (flag)
			{
				this.yogaNode = new YogaNode(null);
			}
			this.SyncWithLayout(this.yogaNode);
		}

		public void SyncWithLayout(YogaNode targetNode)
		{
			targetNode.Flex = float.NaN;
			targetNode.FlexGrow = this.flexGrow.value;
			targetNode.FlexShrink = this.flexShrink.value;
			targetNode.FlexBasis = this.flexBasis.ToYogaValue();
			targetNode.Left = this.left.ToYogaValue();
			targetNode.Top = this.top.ToYogaValue();
			targetNode.Right = this.right.ToYogaValue();
			targetNode.Bottom = this.bottom.ToYogaValue();
			targetNode.MarginLeft = this.marginLeft.ToYogaValue();
			targetNode.MarginTop = this.marginTop.ToYogaValue();
			targetNode.MarginRight = this.marginRight.ToYogaValue();
			targetNode.MarginBottom = this.marginBottom.ToYogaValue();
			targetNode.PaddingLeft = this.paddingLeft.ToYogaValue();
			targetNode.PaddingTop = this.paddingTop.ToYogaValue();
			targetNode.PaddingRight = this.paddingRight.ToYogaValue();
			targetNode.PaddingBottom = this.paddingBottom.ToYogaValue();
			targetNode.BorderLeftWidth = this.borderLeftWidth.value;
			targetNode.BorderTopWidth = this.borderTopWidth.value;
			targetNode.BorderRightWidth = this.borderRightWidth.value;
			targetNode.BorderBottomWidth = this.borderBottomWidth.value;
			targetNode.Width = this.width.ToYogaValue();
			targetNode.Height = this.height.ToYogaValue();
			targetNode.PositionType = (YogaPositionType)this.position.value;
			targetNode.Overflow = (YogaOverflow)this.overflow.value;
			targetNode.AlignSelf = (YogaAlign)this.alignSelf.value;
			targetNode.MaxWidth = this.maxWidth.ToYogaValue();
			targetNode.MaxHeight = this.maxHeight.ToYogaValue();
			targetNode.MinWidth = this.minWidth.ToYogaValue();
			targetNode.MinHeight = this.minHeight.ToYogaValue();
			targetNode.FlexDirection = (YogaFlexDirection)this.flexDirection.value;
			targetNode.AlignContent = (YogaAlign)this.alignContent.value;
			targetNode.AlignItems = (YogaAlign)this.alignItems.value;
			targetNode.JustifyContent = (YogaJustify)this.justifyContent.value;
			targetNode.Wrap = (YogaWrap)this.flexWrap.value;
			targetNode.Display = (YogaDisplay)this.display.value;
		}

		internal void ApplyProperties(StylePropertyReader reader, InheritedStylesData inheritedStylesData)
		{
			this.dpiScaling = reader.dpiScaling;
			StylePropertyID stylePropertyID = reader.propertyID;
			while (stylePropertyID != StylePropertyID.Unknown)
			{
				StyleValueHandle handle = reader.GetValue(0).handle;
				bool flag = handle.valueType == StyleValueType.Keyword;
				if (!flag)
				{
					goto IL_006A;
				}
				bool flag2 = handle.valueIndex == 1;
				if (flag2)
				{
					this.ApplyInitialStyleValue(reader);
				}
				else
				{
					bool flag3 = handle.valueIndex == 3;
					if (!flag3)
					{
						goto IL_006A;
					}
					this.ApplyUnsetStyleValue(reader, inheritedStylesData);
				}
				IL_00A7:
				stylePropertyID = reader.MoveNextProperty();
				continue;
				IL_006A:
				StylePropertyID stylePropertyID2 = stylePropertyID;
				if (stylePropertyID2 != StylePropertyID.Unknown)
				{
					if (stylePropertyID2 - StylePropertyID.BorderColor > 5)
					{
						if (stylePropertyID2 != StylePropertyID.Custom)
						{
							this.ApplyStyleProperty(reader);
						}
						else
						{
							this.ApplyCustomStyleProperty(reader);
						}
					}
					else
					{
						this.ApplyShorthandProperty(reader);
					}
				}
				goto IL_00A7;
			}
		}

		internal void ApplyStyleCursor(StyleCursor styleCursor, int specificity)
		{
			VisualElementStylesData.s_StyleValuePropertyReader.Set(styleCursor, specificity);
			this.cursor = VisualElementStylesData.s_StyleValuePropertyReader.ReadStyleCursor(0);
		}

		internal void ApplyStyleValue(StylePropertyID propertyID, StyleValue value, int specificity)
		{
			bool flag = value.keyword == StyleKeyword.Initial;
			if (flag)
			{
				this.ApplyInitialStyleValue(propertyID, specificity);
			}
			else
			{
				VisualElementStylesData.s_StyleValuePropertyReader.Set(propertyID, value, specificity);
				this.ApplyStyleProperty(VisualElementStylesData.s_StyleValuePropertyReader);
			}
		}

		private void ApplyInitialStyleValue(StylePropertyReader reader)
		{
			bool flag = reader.propertyID == StylePropertyID.Custom;
			if (flag)
			{
				this.RemoveCustomStyleProperty(reader.property.name);
			}
			else
			{
				this.ApplyInitialStyleValue(reader.propertyID, reader.specificity);
			}
		}

		private void ApplyInitialStyleValue(StylePropertyID propertyID, int specificity)
		{
			StylePropertyID stylePropertyID = propertyID;
			if (stylePropertyID != StylePropertyID.Unknown)
			{
				switch (stylePropertyID)
				{
				case StylePropertyID.BorderColor:
				{
					StyleValue styleValue = StyleSheetCache.GetInitialValue(StylePropertyID.BorderLeftColor);
					this.ApplyStyleValue(styleValue.id, styleValue, specificity);
					styleValue = StyleSheetCache.GetInitialValue(StylePropertyID.BorderTopColor);
					this.ApplyStyleValue(styleValue.id, styleValue, specificity);
					styleValue = StyleSheetCache.GetInitialValue(StylePropertyID.BorderRightColor);
					this.ApplyStyleValue(styleValue.id, styleValue, specificity);
					styleValue = StyleSheetCache.GetInitialValue(StylePropertyID.BorderBottomColor);
					this.ApplyStyleValue(styleValue.id, styleValue, specificity);
					return;
				}
				case StylePropertyID.BorderRadius:
				{
					StyleValue styleValue2 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderTopLeftRadius);
					this.ApplyStyleValue(styleValue2.id, styleValue2, specificity);
					styleValue2 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderTopRightRadius);
					this.ApplyStyleValue(styleValue2.id, styleValue2, specificity);
					styleValue2 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderBottomLeftRadius);
					this.ApplyStyleValue(styleValue2.id, styleValue2, specificity);
					styleValue2 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderBottomRightRadius);
					this.ApplyStyleValue(styleValue2.id, styleValue2, specificity);
					return;
				}
				case StylePropertyID.BorderWidth:
				{
					StyleValue styleValue3 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderLeftWidth);
					this.ApplyStyleValue(styleValue3.id, styleValue3, specificity);
					styleValue3 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderTopWidth);
					this.ApplyStyleValue(styleValue3.id, styleValue3, specificity);
					styleValue3 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderRightWidth);
					this.ApplyStyleValue(styleValue3.id, styleValue3, specificity);
					styleValue3 = StyleSheetCache.GetInitialValue(StylePropertyID.BorderBottomWidth);
					this.ApplyStyleValue(styleValue3.id, styleValue3, specificity);
					return;
				}
				case StylePropertyID.Flex:
				{
					StyleValue styleValue4 = StyleSheetCache.GetInitialValue(StylePropertyID.FlexGrow);
					this.ApplyStyleValue(styleValue4.id, styleValue4, specificity);
					styleValue4 = StyleSheetCache.GetInitialValue(StylePropertyID.FlexShrink);
					this.ApplyStyleValue(styleValue4.id, styleValue4, specificity);
					styleValue4 = StyleSheetCache.GetInitialValue(StylePropertyID.FlexBasis);
					this.ApplyStyleValue(styleValue4.id, styleValue4, specificity);
					return;
				}
				case StylePropertyID.Margin:
				{
					StyleValue styleValue5 = StyleSheetCache.GetInitialValue(StylePropertyID.MarginLeft);
					this.ApplyStyleValue(styleValue5.id, styleValue5, specificity);
					styleValue5 = StyleSheetCache.GetInitialValue(StylePropertyID.MarginTop);
					this.ApplyStyleValue(styleValue5.id, styleValue5, specificity);
					styleValue5 = StyleSheetCache.GetInitialValue(StylePropertyID.MarginRight);
					this.ApplyStyleValue(styleValue5.id, styleValue5, specificity);
					styleValue5 = StyleSheetCache.GetInitialValue(StylePropertyID.MarginBottom);
					this.ApplyStyleValue(styleValue5.id, styleValue5, specificity);
					return;
				}
				case StylePropertyID.Padding:
				{
					StyleValue styleValue6 = StyleSheetCache.GetInitialValue(StylePropertyID.PaddingLeft);
					this.ApplyStyleValue(styleValue6.id, styleValue6, specificity);
					styleValue6 = StyleSheetCache.GetInitialValue(StylePropertyID.PaddingTop);
					this.ApplyStyleValue(styleValue6.id, styleValue6, specificity);
					styleValue6 = StyleSheetCache.GetInitialValue(StylePropertyID.PaddingRight);
					this.ApplyStyleValue(styleValue6.id, styleValue6, specificity);
					styleValue6 = StyleSheetCache.GetInitialValue(StylePropertyID.PaddingBottom);
					this.ApplyStyleValue(styleValue6.id, styleValue6, specificity);
					return;
				}
				case StylePropertyID.Cursor:
					this.ApplyStyleCursor(default(StyleCursor), specificity);
					return;
				case StylePropertyID.Custom:
					break;
				default:
				{
					StyleValue initialValue = StyleSheetCache.GetInitialValue(propertyID);
					Debug.Assert(initialValue.keyword != StyleKeyword.Initial, "Recursive apply initial value");
					this.ApplyStyleValue(initialValue.id, initialValue, specificity);
					return;
				}
				}
			}
			Debug.LogAssertion("Unexpected style property ID " + propertyID.ToString() + ".");
		}

		private void ApplyUnsetStyleValue(StylePropertyReader reader, InheritedStylesData inheritedStylesData)
		{
			bool flag = inheritedStylesData == null;
			if (flag)
			{
				this.ApplyInitialStyleValue(reader);
			}
			int specificity = reader.specificity;
			StylePropertyID propertyID = reader.propertyID;
			switch (propertyID)
			{
			case StylePropertyID.UnityTextAlign:
				this.unityTextAlign = inheritedStylesData.unityTextAlign;
				this.unityTextAlign.specificity = specificity;
				return;
			case StylePropertyID.WhiteSpace:
				this.whiteSpace = inheritedStylesData.whiteSpace;
				this.whiteSpace.specificity = specificity;
				return;
			case StylePropertyID.Font:
				this.unityFont = inheritedStylesData.font;
				this.unityFont.specificity = specificity;
				return;
			case StylePropertyID.FontSize:
				this.fontSize = inheritedStylesData.fontSize;
				this.fontSize.specificity = specificity;
				return;
			case StylePropertyID.FontStyleAndWeight:
				this.unityFontStyleAndWeight = inheritedStylesData.unityFontStyle;
				this.unityFontStyleAndWeight.specificity = specificity;
				return;
			case StylePropertyID.BackgroundScaleMode:
			case StylePropertyID.Overflow:
			case StylePropertyID.OverflowClipBox:
			case StylePropertyID.Display:
			case StylePropertyID.BackgroundImage:
				break;
			case StylePropertyID.Visibility:
				this.visibility = inheritedStylesData.visibility;
				this.visibility.specificity = specificity;
				return;
			case StylePropertyID.Color:
				this.color = inheritedStylesData.color;
				this.color.specificity = specificity;
				return;
			default:
				if (propertyID == StylePropertyID.Custom)
				{
					this.RemoveCustomStyleProperty(reader.property.name);
					return;
				}
				break;
			}
			this.ApplyInitialStyleValue(reader.propertyID, specificity);
		}

		internal void ApplyStyleProperty(IStylePropertyReader reader)
		{
			switch (reader.propertyID)
			{
			case StylePropertyID.MarginLeft:
				this.marginLeft = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.MarginTop:
				this.marginTop = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.MarginRight:
				this.marginRight = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.MarginBottom:
				this.marginBottom = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.PaddingLeft:
				this.paddingLeft = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.PaddingTop:
				this.paddingTop = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.PaddingRight:
				this.paddingRight = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.PaddingBottom:
				this.paddingBottom = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.Position:
				this.position = reader.ReadStyleEnum<Position>(0);
				return;
			case StylePropertyID.PositionLeft:
				this.left = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.PositionTop:
				this.top = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.PositionRight:
				this.right = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.PositionBottom:
				this.bottom = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.Width:
				this.width = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.Height:
				this.height = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.MinWidth:
				this.minWidth = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.MinHeight:
				this.minHeight = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.MaxWidth:
				this.maxWidth = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.MaxHeight:
				this.maxHeight = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.FlexBasis:
				this.flexBasis = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.FlexGrow:
				this.flexGrow = reader.ReadStyleFloat(0);
				return;
			case StylePropertyID.FlexShrink:
				this.flexShrink = reader.ReadStyleFloat(0);
				return;
			case StylePropertyID.BorderLeftColor:
				this.borderLeftColor = reader.ReadStyleColor(0);
				return;
			case StylePropertyID.BorderTopColor:
				this.borderTopColor = reader.ReadStyleColor(0);
				return;
			case StylePropertyID.BorderRightColor:
				this.borderRightColor = reader.ReadStyleColor(0);
				return;
			case StylePropertyID.BorderBottomColor:
				this.borderBottomColor = reader.ReadStyleColor(0);
				return;
			case StylePropertyID.BorderLeftWidth:
				this.borderLeftWidth = reader.ReadStyleFloat(0);
				return;
			case StylePropertyID.BorderTopWidth:
				this.borderTopWidth = reader.ReadStyleFloat(0);
				return;
			case StylePropertyID.BorderRightWidth:
				this.borderRightWidth = reader.ReadStyleFloat(0);
				return;
			case StylePropertyID.BorderBottomWidth:
				this.borderBottomWidth = reader.ReadStyleFloat(0);
				return;
			case StylePropertyID.BorderTopLeftRadius:
				this.borderTopLeftRadius = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.BorderTopRightRadius:
				this.borderTopRightRadius = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.BorderBottomRightRadius:
				this.borderBottomRightRadius = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.BorderBottomLeftRadius:
				this.borderBottomLeftRadius = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.FlexDirection:
				this.flexDirection = reader.ReadStyleEnum<FlexDirection>(0);
				return;
			case StylePropertyID.FlexWrap:
				this.flexWrap = reader.ReadStyleEnum<Wrap>(0);
				return;
			case StylePropertyID.JustifyContent:
				this.justifyContent = reader.ReadStyleEnum<Justify>(0);
				return;
			case StylePropertyID.AlignContent:
				StyleSheetApplicator.ApplyAlign(reader, ref this.alignContent);
				return;
			case StylePropertyID.AlignSelf:
				StyleSheetApplicator.ApplyAlign(reader, ref this.alignSelf);
				return;
			case StylePropertyID.AlignItems:
				StyleSheetApplicator.ApplyAlign(reader, ref this.alignItems);
				return;
			case StylePropertyID.UnityTextAlign:
				this.unityTextAlign = reader.ReadStyleEnum<TextAnchor>(0);
				return;
			case StylePropertyID.WhiteSpace:
				this.whiteSpace = reader.ReadStyleEnum<WhiteSpace>(0);
				return;
			case StylePropertyID.Font:
				this.unityFont = reader.ReadStyleFont(0);
				return;
			case StylePropertyID.FontSize:
				this.fontSize = reader.ReadStyleLength(0);
				return;
			case StylePropertyID.FontStyleAndWeight:
				this.unityFontStyleAndWeight = reader.ReadStyleEnum<FontStyle>(0);
				return;
			case StylePropertyID.BackgroundScaleMode:
				this.unityBackgroundScaleMode = reader.ReadStyleEnum<ScaleMode>(0);
				return;
			case StylePropertyID.Visibility:
				this.visibility = reader.ReadStyleEnum<Visibility>(0);
				return;
			case StylePropertyID.Overflow:
				this.overflow = reader.ReadStyleEnum<OverflowInternal>(0);
				return;
			case StylePropertyID.OverflowClipBox:
				this.unityOverflowClipBox = reader.ReadStyleEnum<OverflowClipBox>(0);
				return;
			case StylePropertyID.Display:
				StyleSheetApplicator.ApplyDisplay(reader, ref this.display);
				return;
			case StylePropertyID.BackgroundImage:
				this.backgroundImage = reader.ReadStyleBackground(0);
				return;
			case StylePropertyID.Color:
				this.color = reader.ReadStyleColor(0);
				return;
			case StylePropertyID.BackgroundColor:
				this.backgroundColor = reader.ReadStyleColor(0);
				return;
			case StylePropertyID.BackgroundImageTintColor:
				this.unityBackgroundImageTintColor = reader.ReadStyleColor(0);
				return;
			case StylePropertyID.SliceLeft:
				this.unitySliceLeft = reader.ReadStyleInt(0);
				return;
			case StylePropertyID.SliceTop:
				this.unitySliceTop = reader.ReadStyleInt(0);
				return;
			case StylePropertyID.SliceRight:
				this.unitySliceRight = reader.ReadStyleInt(0);
				return;
			case StylePropertyID.SliceBottom:
				this.unitySliceBottom = reader.ReadStyleInt(0);
				return;
			case StylePropertyID.Opacity:
				this.opacity = reader.ReadStyleFloat(0);
				return;
			case StylePropertyID.Cursor:
				this.cursor = reader.ReadStyleCursor(0);
				return;
			}
			throw new ArgumentException(string.Format("Non exhaustive switch statement (value={0})", reader.propertyID));
		}

		internal void ApplyShorthandProperty(StylePropertyReader reader)
		{
			switch (reader.propertyID)
			{
			case StylePropertyID.BorderColor:
				ShorthandApplicator.ApplyBorderColor(reader, this);
				break;
			case StylePropertyID.BorderRadius:
				ShorthandApplicator.ApplyBorderRadius(reader, this);
				break;
			case StylePropertyID.BorderWidth:
				ShorthandApplicator.ApplyBorderWidth(reader, this);
				break;
			case StylePropertyID.Flex:
				ShorthandApplicator.ApplyFlex(reader, this);
				break;
			case StylePropertyID.Margin:
				ShorthandApplicator.ApplyMargin(reader, this);
				break;
			case StylePropertyID.Padding:
				ShorthandApplicator.ApplyPadding(reader, this);
				break;
			default:
				throw new ArgumentException(string.Format("Non exhaustive switch statement (value={0})", reader.propertyID));
			}
		}

		private void RemoveCustomStyleProperty(string name)
		{
			bool flag = this.m_CustomProperties == null || !this.m_CustomProperties.ContainsKey(name);
			if (!flag)
			{
				this.m_CustomProperties.Remove(name);
			}
		}

		private void ApplyCustomStyleProperty(StylePropertyReader reader)
		{
			bool flag = this.m_CustomProperties == null;
			if (flag)
			{
				this.m_CustomProperties = new Dictionary<string, CustomPropertyHandle>();
			}
			StyleProperty property = reader.property;
			int specificity = reader.specificity;
			CustomPropertyHandle customPropertyHandle = default(CustomPropertyHandle);
			bool flag2 = !this.m_CustomProperties.TryGetValue(property.name, out customPropertyHandle) || specificity >= customPropertyHandle.specificity;
			if (flag2)
			{
				customPropertyHandle.value = reader.GetValue(0);
				customPropertyHandle.specificity = specificity;
				this.m_CustomProperties[property.name] = customPropertyHandle;
			}
		}

		public bool TryGetValue(CustomStyleProperty<float> property, out float value)
		{
			CustomPropertyHandle customPropertyHandle;
			bool flag = this.TryGetValue(property.name, StyleValueType.Float, out customPropertyHandle);
			if (flag)
			{
				StylePropertyValue value2 = customPropertyHandle.value;
				bool flag2 = value2.sheet.TryReadFloat(value2.handle, out value);
				if (flag2)
				{
					return true;
				}
			}
			value = 0f;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<int> property, out int value)
		{
			CustomPropertyHandle customPropertyHandle;
			bool flag = this.TryGetValue(property.name, StyleValueType.Float, out customPropertyHandle);
			if (flag)
			{
				float num = 0f;
				StylePropertyValue value2 = customPropertyHandle.value;
				bool flag2 = value2.sheet.TryReadFloat(value2.handle, out num);
				if (flag2)
				{
					value = (int)num;
					return true;
				}
			}
			value = 0;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<bool> property, out bool value)
		{
			CustomPropertyHandle customPropertyHandle;
			bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out customPropertyHandle);
			bool flag2;
			if (flag)
			{
				StylePropertyValue value2 = customPropertyHandle.value;
				value = value2.sheet.ReadKeyword(value2.handle) == StyleValueKeyword.True;
				flag2 = true;
			}
			else
			{
				value = false;
				flag2 = false;
			}
			return flag2;
		}

		public bool TryGetValue(CustomStyleProperty<Color> property, out Color value)
		{
			CustomPropertyHandle customPropertyHandle;
			bool flag = this.TryGetValue(property.name, StyleValueType.Color, out customPropertyHandle);
			if (flag)
			{
				StylePropertyValue value2 = customPropertyHandle.value;
				bool flag2 = value2.sheet.TryReadColor(value2.handle, out value);
				if (flag2)
				{
					return true;
				}
			}
			value = Color.clear;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<Texture2D> property, out Texture2D value)
		{
			CustomPropertyHandle customPropertyHandle;
			bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out customPropertyHandle);
			if (flag)
			{
				ImageSource imageSource = default(ImageSource);
				StylePropertyValue value2 = customPropertyHandle.value;
				bool flag2 = StylePropertyReader.TryGetImageSourceFromValue(value2, this.dpiScaling, out imageSource) && imageSource.texture != null;
				if (flag2)
				{
					value = imageSource.texture;
					return true;
				}
			}
			value = null;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<VectorImage> property, out VectorImage value)
		{
			CustomPropertyHandle customPropertyHandle;
			bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out customPropertyHandle);
			if (flag)
			{
				ImageSource imageSource = default(ImageSource);
				StylePropertyValue value2 = customPropertyHandle.value;
				bool flag2 = StylePropertyReader.TryGetImageSourceFromValue(value2, this.dpiScaling, out imageSource) && imageSource.vectorImage != null;
				if (flag2)
				{
					value = imageSource.vectorImage;
					return true;
				}
			}
			value = null;
			return false;
		}

		public bool TryGetValue(CustomStyleProperty<string> property, out string value)
		{
			CustomPropertyHandle customPropertyHandle;
			bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out customPropertyHandle);
			bool flag2;
			if (flag)
			{
				StylePropertyValue value2 = customPropertyHandle.value;
				value = value2.sheet.ReadAsString(value2.handle);
				flag2 = true;
			}
			else
			{
				value = string.Empty;
				flag2 = false;
			}
			return flag2;
		}

		private bool TryGetValue(string propertyName, StyleValueType valueType, out CustomPropertyHandle customPropertyHandle)
		{
			customPropertyHandle = default(CustomPropertyHandle);
			bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customPropertyHandle);
			bool flag3;
			if (flag)
			{
				StyleValueHandle handle = customPropertyHandle.value.handle;
				bool flag2 = handle.valueType != valueType;
				if (flag2)
				{
					Debug.LogWarning(string.Format("Trying to read value as {0} while parsed type is {1}", valueType, handle.valueType));
					flag3 = false;
				}
				else
				{
					flag3 = true;
				}
			}
			else
			{
				flag3 = false;
			}
			return flag3;
		}

		private static StyleValuePropertyReader s_StyleValuePropertyReader = new StyleValuePropertyReader();

		public static readonly VisualElementStylesData none = new VisualElementStylesData(true);

		internal readonly bool isShared;

		internal YogaNode yogaNode;

		internal Dictionary<string, CustomPropertyHandle> m_CustomProperties;

		internal StyleLength width;

		internal StyleLength height;

		internal StyleLength maxWidth;

		internal StyleLength maxHeight;

		internal StyleLength minWidth;

		internal StyleLength minHeight;

		internal StyleLength flexBasis;

		internal StyleFloat flexShrink;

		internal StyleFloat flexGrow;

		internal StyleInt overflow;

		internal StyleInt unityOverflowClipBox;

		internal StyleLength left;

		internal StyleLength top;

		internal StyleLength right;

		internal StyleLength bottom;

		internal StyleLength marginLeft;

		internal StyleLength marginTop;

		internal StyleLength marginRight;

		internal StyleLength marginBottom;

		internal StyleLength paddingLeft;

		internal StyleLength paddingTop;

		internal StyleLength paddingRight;

		internal StyleLength paddingBottom;

		internal StyleInt position;

		internal StyleInt alignSelf;

		internal StyleInt unityTextAlign;

		internal StyleInt unityFontStyleAndWeight;

		internal StyleFont unityFont;

		internal StyleLength fontSize;

		internal StyleInt whiteSpace;

		internal StyleColor color;

		internal StyleInt flexDirection;

		internal StyleColor backgroundColor;

		internal StyleBackground backgroundImage;

		internal StyleInt unityBackgroundScaleMode;

		internal StyleColor unityBackgroundImageTintColor;

		internal StyleInt alignItems;

		internal StyleInt alignContent;

		internal StyleInt justifyContent;

		internal StyleInt flexWrap;

		internal StyleColor borderLeftColor;

		internal StyleColor borderTopColor;

		internal StyleColor borderRightColor;

		internal StyleColor borderBottomColor;

		internal StyleFloat borderLeftWidth;

		internal StyleFloat borderTopWidth;

		internal StyleFloat borderRightWidth;

		internal StyleFloat borderBottomWidth;

		internal StyleLength borderTopLeftRadius;

		internal StyleLength borderTopRightRadius;

		internal StyleLength borderBottomRightRadius;

		internal StyleLength borderBottomLeftRadius;

		internal StyleInt unitySliceLeft;

		internal StyleInt unitySliceTop;

		internal StyleInt unitySliceRight;

		internal StyleInt unitySliceBottom;

		internal StyleFloat opacity;

		internal StyleCursor cursor;

		internal StyleInt visibility;

		internal StyleInt display;

		internal float dpiScaling;
	}
}
