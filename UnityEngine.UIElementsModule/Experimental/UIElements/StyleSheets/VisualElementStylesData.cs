using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.StyleSheets;
using UnityEngine.Yoga;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal class VisualElementStylesData : ICustomStyle
	{
		public VisualElementStylesData(bool isShared)
		{
			this.isShared = isShared;
		}

		public void Apply(VisualElementStylesData other, StylePropertyApplyMode mode)
		{
			this.m_CustomProperties = other.m_CustomProperties;
			this.width.Apply(other.width, mode);
			this.height.Apply(other.height, mode);
			this.maxWidth.Apply(other.maxWidth, mode);
			this.maxHeight.Apply(other.maxHeight, mode);
			this.minWidth.Apply(other.minWidth, mode);
			this.minHeight.Apply(other.minHeight, mode);
			this.flexBasis.Apply(other.flexBasis, mode);
			this.flexGrow.Apply(other.flexGrow, mode);
			this.flexShrink.Apply(other.flexShrink, mode);
			this.overflow.Apply(other.overflow, mode);
			this.positionLeft.Apply(other.positionLeft, mode);
			this.positionTop.Apply(other.positionTop, mode);
			this.positionRight.Apply(other.positionRight, mode);
			this.positionBottom.Apply(other.positionBottom, mode);
			this.marginLeft.Apply(other.marginLeft, mode);
			this.marginTop.Apply(other.marginTop, mode);
			this.marginRight.Apply(other.marginRight, mode);
			this.marginBottom.Apply(other.marginBottom, mode);
			this.paddingLeft.Apply(other.paddingLeft, mode);
			this.paddingTop.Apply(other.paddingTop, mode);
			this.paddingRight.Apply(other.paddingRight, mode);
			this.paddingBottom.Apply(other.paddingBottom, mode);
			this.positionType.Apply(other.positionType, mode);
			this.alignSelf.Apply(other.alignSelf, mode);
			this.unityTextAlign.Apply(other.unityTextAlign, mode);
			this.fontStyleAndWeight.Apply(other.fontStyleAndWeight, mode);
			this.textClipping.Apply(other.textClipping, mode);
			this.fontSize.Apply(other.fontSize, mode);
			this.font.Apply(other.font, mode);
			this.wordWrap.Apply(other.wordWrap, mode);
			this.color.Apply(other.color, mode);
			this.flexDirection.Apply(other.flexDirection, mode);
			this.backgroundColor.Apply(other.backgroundColor, mode);
			this.borderColor.Apply(other.borderColor, mode);
			this.backgroundImage.Apply(other.backgroundImage, mode);
			this.backgroundScaleMode.Apply(other.backgroundScaleMode, mode);
			this.alignItems.Apply(other.alignItems, mode);
			this.alignContent.Apply(other.alignContent, mode);
			this.justifyContent.Apply(other.justifyContent, mode);
			this.flexWrap.Apply(other.flexWrap, mode);
			this.borderLeftWidth.Apply(other.borderLeftWidth, mode);
			this.borderTopWidth.Apply(other.borderTopWidth, mode);
			this.borderRightWidth.Apply(other.borderRightWidth, mode);
			this.borderBottomWidth.Apply(other.borderBottomWidth, mode);
			this.borderTopLeftRadius.Apply(other.borderTopLeftRadius, mode);
			this.borderTopRightRadius.Apply(other.borderTopRightRadius, mode);
			this.borderBottomRightRadius.Apply(other.borderBottomRightRadius, mode);
			this.borderBottomLeftRadius.Apply(other.borderBottomLeftRadius, mode);
			this.sliceLeft.Apply(other.sliceLeft, mode);
			this.sliceTop.Apply(other.sliceTop, mode);
			this.sliceRight.Apply(other.sliceRight, mode);
			this.sliceBottom.Apply(other.sliceBottom, mode);
			this.opacity.Apply(other.opacity, mode);
			this.cursor.Apply(other.cursor, mode);
			this.visibility.Apply(other.visibility, mode);
		}

		public void WriteToGUIStyle(GUIStyle style)
		{
			style.alignment = (TextAnchor)this.unityTextAlign.GetSpecifiedValueOrDefault((int)style.alignment);
			style.wordWrap = this.wordWrap.GetSpecifiedValueOrDefault(style.wordWrap);
			style.clipping = (TextClipping)this.textClipping.GetSpecifiedValueOrDefault((int)style.clipping);
			if (this.font.value != null)
			{
				style.font = this.font.value;
			}
			style.fontSize = this.fontSize.GetSpecifiedValueOrDefault(style.fontSize);
			style.fontStyle = (FontStyle)this.fontStyleAndWeight.GetSpecifiedValueOrDefault((int)style.fontStyle);
			this.AssignRect(style.margin, ref this.marginLeft, ref this.marginTop, ref this.marginRight, ref this.marginBottom);
			this.AssignRect(style.padding, ref this.paddingLeft, ref this.paddingTop, ref this.paddingRight, ref this.paddingBottom);
			this.AssignRect(style.border, ref this.sliceLeft, ref this.sliceTop, ref this.sliceRight, ref this.sliceBottom);
			this.AssignState(style.normal);
			this.AssignState(style.focused);
			this.AssignState(style.hover);
			this.AssignState(style.active);
			this.AssignState(style.onNormal);
			this.AssignState(style.onFocused);
			this.AssignState(style.onHover);
			this.AssignState(style.onActive);
		}

		private void AssignState(GUIStyleState state)
		{
			state.textColor = this.color.GetSpecifiedValueOrDefault(state.textColor);
			if (this.backgroundImage.value != null)
			{
				state.background = this.backgroundImage.value;
			}
		}

		private void AssignRect(RectOffset rect, ref StyleValue<int> left, ref StyleValue<int> top, ref StyleValue<int> right, ref StyleValue<int> bottom)
		{
			rect.left = left.GetSpecifiedValueOrDefault(rect.left);
			rect.top = top.GetSpecifiedValueOrDefault(rect.top);
			rect.right = right.GetSpecifiedValueOrDefault(rect.right);
			rect.bottom = bottom.GetSpecifiedValueOrDefault(rect.bottom);
		}

		private void AssignRect(RectOffset rect, ref StyleValue<float> left, ref StyleValue<float> top, ref StyleValue<float> right, ref StyleValue<float> bottom)
		{
			rect.left = (int)left.GetSpecifiedValueOrDefault((float)rect.left);
			rect.top = (int)top.GetSpecifiedValueOrDefault((float)rect.top);
			rect.right = (int)right.GetSpecifiedValueOrDefault((float)rect.right);
			rect.bottom = (int)bottom.GetSpecifiedValueOrDefault((float)rect.bottom);
		}

		public void ApplyLayoutValues()
		{
			if (this.yogaNode == null)
			{
				this.yogaNode = new YogaNode(null);
			}
			this.SyncWithLayout(this.yogaNode);
		}

		public StyleValue<float> FlexBasisToFloat()
		{
			StyleValue<float> styleValue;
			if (this.flexBasis.value.isKeyword)
			{
				if (this.flexBasis.value.keyword == StyleValueKeyword.Auto)
				{
					styleValue = new StyleValue<float>(-1f, this.flexBasis.specificity);
				}
				else
				{
					styleValue = new StyleValue<float>(0f, this.flexBasis.specificity);
				}
			}
			else
			{
				styleValue = new StyleValue<float>(this.flexBasis.value.floatValue, this.flexBasis.specificity);
			}
			return styleValue;
		}

		public void SyncWithLayout(YogaNode targetNode)
		{
			targetNode.Flex = float.NaN;
			float specifiedValueOrDefault = this.FlexBasisToFloat().GetSpecifiedValueOrDefault(float.NaN);
			if (specifiedValueOrDefault == -1f)
			{
				targetNode.FlexBasis = YogaValue.Auto();
			}
			else
			{
				targetNode.FlexBasis = specifiedValueOrDefault;
			}
			targetNode.FlexGrow = this.flexGrow.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.FlexShrink = this.flexShrink.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.Left = this.positionLeft.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.Top = this.positionTop.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.Right = this.positionRight.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.Bottom = this.positionBottom.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.MarginLeft = this.marginLeft.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.MarginTop = this.marginTop.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.MarginRight = this.marginRight.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.MarginBottom = this.marginBottom.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.PaddingLeft = this.paddingLeft.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.PaddingTop = this.paddingTop.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.PaddingRight = this.paddingRight.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.PaddingBottom = this.paddingBottom.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.BorderLeftWidth = this.borderLeftWidth.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.BorderTopWidth = this.borderTopWidth.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.BorderRightWidth = this.borderRightWidth.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.BorderBottomWidth = this.borderBottomWidth.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.Width = this.width.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.Height = this.height.GetSpecifiedValueOrDefault(float.NaN);
			PositionType value = (PositionType)this.positionType.value;
			if (value != PositionType.Absolute && value != PositionType.Manual)
			{
				if (value == PositionType.Relative)
				{
					targetNode.PositionType = YogaPositionType.Relative;
				}
			}
			else
			{
				targetNode.PositionType = YogaPositionType.Absolute;
			}
			targetNode.Overflow = (YogaOverflow)this.overflow.value;
			targetNode.AlignSelf = (YogaAlign)this.alignSelf.value;
			targetNode.MaxWidth = this.maxWidth.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.MaxHeight = this.maxHeight.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.MinWidth = this.minWidth.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.MinHeight = this.minHeight.GetSpecifiedValueOrDefault(float.NaN);
			targetNode.FlexDirection = (YogaFlexDirection)this.flexDirection.value;
			targetNode.AlignContent = (YogaAlign)this.alignContent.GetSpecifiedValueOrDefault(1);
			targetNode.AlignItems = (YogaAlign)this.alignItems.GetSpecifiedValueOrDefault(4);
			targetNode.JustifyContent = (YogaJustify)this.justifyContent.value;
			targetNode.Wrap = (YogaWrap)this.flexWrap.value;
		}

		internal void ApplyRule(StyleSheet registry, int specificity, StyleRule rule, StylePropertyID[] propertyIDs)
		{
			int i = 0;
			while (i < rule.properties.Length)
			{
				StyleProperty styleProperty = rule.properties[i];
				StylePropertyID stylePropertyID = propertyIDs[i];
				StyleValueHandle[] values = styleProperty.values;
				switch (stylePropertyID)
				{
				case StylePropertyID.MarginLeft:
					registry.Apply<float>(values, specificity, ref this.marginLeft, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.MarginTop:
					registry.Apply<float>(values, specificity, ref this.marginTop, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.MarginRight:
					registry.Apply<float>(values, specificity, ref this.marginRight, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.MarginBottom:
					registry.Apply<float>(values, specificity, ref this.marginBottom, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.PaddingLeft:
					registry.Apply<float>(values, specificity, ref this.paddingLeft, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.PaddingTop:
					registry.Apply<float>(values, specificity, ref this.paddingTop, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.PaddingRight:
					registry.Apply<float>(values, specificity, ref this.paddingRight, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.PaddingBottom:
					registry.Apply<float>(values, specificity, ref this.paddingBottom, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.Position:
					registry.Apply<int>(values, specificity, ref this.positionType, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Position>));
					break;
				case StylePropertyID.PositionType:
					registry.Apply<int>(values, specificity, ref this.positionType, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<PositionType>));
					break;
				case StylePropertyID.PositionLeft:
					registry.Apply<float>(values, specificity, ref this.positionLeft, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.PositionTop:
					registry.Apply<float>(values, specificity, ref this.positionTop, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.PositionRight:
					registry.Apply<float>(values, specificity, ref this.positionRight, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.PositionBottom:
					registry.Apply<float>(values, specificity, ref this.positionBottom, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.Width:
					registry.Apply<float>(values, specificity, ref this.width, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.Height:
					registry.Apply<float>(values, specificity, ref this.height, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.MinWidth:
					registry.Apply<float>(values, specificity, ref this.minWidth, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.MinHeight:
					registry.Apply<float>(values, specificity, ref this.minHeight, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.MaxWidth:
					registry.Apply<float>(values, specificity, ref this.maxWidth, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.MaxHeight:
					registry.Apply<float>(values, specificity, ref this.maxHeight, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.FlexBasis:
					registry.Apply<FloatOrKeyword>(values, specificity, ref this.flexBasis, new HandlesApplicatorFunction<FloatOrKeyword>(StyleSheetApplicator.ApplyFloatOrKeyword));
					break;
				case StylePropertyID.FlexGrow:
					registry.Apply<float>(values, specificity, ref this.flexGrow, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.FlexShrink:
					registry.Apply<float>(values, specificity, ref this.flexShrink, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderLeftWidth:
					registry.Apply<float>(values, specificity, ref this.borderLeftWidth, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderTopWidth:
					registry.Apply<float>(values, specificity, ref this.borderTopWidth, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderRightWidth:
					registry.Apply<float>(values, specificity, ref this.borderRightWidth, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderBottomWidth:
					registry.Apply<float>(values, specificity, ref this.borderBottomWidth, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderTopLeftRadius:
					registry.Apply<float>(values, specificity, ref this.borderTopLeftRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderTopRightRadius:
					registry.Apply<float>(values, specificity, ref this.borderTopRightRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderBottomRightRadius:
					registry.Apply<float>(values, specificity, ref this.borderBottomRightRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderBottomLeftRadius:
					registry.Apply<float>(values, specificity, ref this.borderBottomLeftRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.FlexDirection:
					registry.Apply<int>(values, specificity, ref this.flexDirection, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<FlexDirection>));
					break;
				case StylePropertyID.FlexWrap:
					registry.Apply<int>(values, specificity, ref this.flexWrap, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Wrap>));
					break;
				case StylePropertyID.JustifyContent:
					registry.Apply<int>(values, specificity, ref this.justifyContent, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Justify>));
					break;
				case StylePropertyID.AlignContent:
					registry.Apply<int>(values, specificity, ref this.alignContent, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Align>));
					break;
				case StylePropertyID.AlignSelf:
					registry.Apply<int>(values, specificity, ref this.alignSelf, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Align>));
					break;
				case StylePropertyID.AlignItems:
					registry.Apply<int>(values, specificity, ref this.alignItems, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Align>));
					break;
				case StylePropertyID.UnityTextAlign:
					registry.Apply<int>(values, specificity, ref this.unityTextAlign, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<TextAnchor>));
					break;
				case StylePropertyID.TextClipping:
					registry.Apply<int>(values, specificity, ref this.textClipping, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<TextClipping>));
					break;
				case StylePropertyID.Font:
					registry.Apply<Font>(values, specificity, ref this.font, new HandlesApplicatorFunction<Font>(StyleSheetApplicator.ApplyFont));
					break;
				case StylePropertyID.FontSize:
					registry.Apply<int>(values, specificity, ref this.fontSize, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyInt));
					break;
				case StylePropertyID.FontStyleAndWeight:
					registry.Apply<int>(values, specificity, ref this.fontStyleAndWeight, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<FontStyle>));
					break;
				case StylePropertyID.BackgroundScaleMode:
					registry.Apply<int>(values, specificity, ref this.backgroundScaleMode, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyInt));
					break;
				case StylePropertyID.Cursor:
					registry.Apply<CursorStyle>(values, specificity, ref this.cursor, new HandlesApplicatorFunction<CursorStyle>(StyleSheetApplicator.ApplyCursor));
					break;
				case StylePropertyID.Visibility:
					registry.Apply<int>(values, specificity, ref this.visibility, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Visibility>));
					break;
				case StylePropertyID.WordWrap:
					registry.Apply<bool>(values, specificity, ref this.wordWrap, new HandlesApplicatorFunction<bool>(StyleSheetApplicator.ApplyBool));
					break;
				case StylePropertyID.BackgroundImage:
					registry.Apply<Texture2D>(values, specificity, ref this.backgroundImage, new HandlesApplicatorFunction<Texture2D>(StyleSheetApplicator.ApplyImage));
					break;
				case StylePropertyID.Color:
					registry.Apply<Color>(values, specificity, ref this.color, new HandlesApplicatorFunction<Color>(StyleSheetApplicator.ApplyColor));
					break;
				case StylePropertyID.BackgroundColor:
					registry.Apply<Color>(values, specificity, ref this.backgroundColor, new HandlesApplicatorFunction<Color>(StyleSheetApplicator.ApplyColor));
					break;
				case StylePropertyID.BorderColor:
					registry.Apply<Color>(values, specificity, ref this.borderColor, new HandlesApplicatorFunction<Color>(StyleSheetApplicator.ApplyColor));
					break;
				case StylePropertyID.Overflow:
					registry.Apply<int>(values, specificity, ref this.overflow, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyEnum<Overflow>));
					break;
				case StylePropertyID.SliceLeft:
					registry.Apply<int>(values, specificity, ref this.sliceLeft, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyInt));
					break;
				case StylePropertyID.SliceTop:
					registry.Apply<int>(values, specificity, ref this.sliceTop, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyInt));
					break;
				case StylePropertyID.SliceRight:
					registry.Apply<int>(values, specificity, ref this.sliceRight, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyInt));
					break;
				case StylePropertyID.SliceBottom:
					registry.Apply<int>(values, specificity, ref this.sliceBottom, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyInt));
					break;
				case StylePropertyID.Opacity:
					registry.Apply<float>(values, specificity, ref this.opacity, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.BorderRadius:
					registry.Apply<float>(values, specificity, ref this.borderTopLeftRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					registry.Apply<float>(values, specificity, ref this.borderTopRightRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					registry.Apply<float>(values, specificity, ref this.borderBottomLeftRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					registry.Apply<float>(values, specificity, ref this.borderBottomRightRadius, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
					break;
				case StylePropertyID.Flex:
					registry.ApplyShorthand(values, specificity, this, new ShorthandApplicatorFunction(StyleSheetApplicator.ApplyFlexShorthand));
					break;
				case StylePropertyID.Margin:
				case StylePropertyID.Padding:
					goto IL_0CF4;
				case StylePropertyID.Custom:
				{
					if (this.m_CustomProperties == null)
					{
						this.m_CustomProperties = new Dictionary<string, CustomProperty>();
					}
					CustomProperty customProperty = default(CustomProperty);
					if (!this.m_CustomProperties.TryGetValue(styleProperty.name, out customProperty) || specificity >= customProperty.specificity)
					{
						customProperty.handles = values;
						customProperty.data = registry;
						customProperty.specificity = specificity;
						this.m_CustomProperties[styleProperty.name] = customProperty;
					}
					break;
				}
				default:
					goto IL_0CF4;
				}
				i++;
				continue;
				IL_0CF4:
				throw new ArgumentException(string.Format("Non exhaustive switch statement (value={0})", stylePropertyID));
			}
		}

		public void ApplyCustomProperty(string propertyName, ref StyleValue<float> target)
		{
			this.ApplyCustomProperty<float>(propertyName, ref target, StyleValueType.Float, new HandlesApplicatorFunction<float>(StyleSheetApplicator.ApplyFloat));
		}

		public void ApplyCustomProperty(string propertyName, ref StyleValue<int> target)
		{
			this.ApplyCustomProperty<int>(propertyName, ref target, StyleValueType.Float, new HandlesApplicatorFunction<int>(StyleSheetApplicator.ApplyInt));
		}

		public void ApplyCustomProperty(string propertyName, ref StyleValue<bool> target)
		{
			this.ApplyCustomProperty<bool>(propertyName, ref target, StyleValueType.Keyword, new HandlesApplicatorFunction<bool>(StyleSheetApplicator.ApplyBool));
		}

		public void ApplyCustomProperty(string propertyName, ref StyleValue<Color> target)
		{
			this.ApplyCustomProperty<Color>(propertyName, ref target, StyleValueType.Color, new HandlesApplicatorFunction<Color>(StyleSheetApplicator.ApplyColor));
		}

		public void ApplyCustomProperty(string propertyName, ref StyleValue<Texture2D> target)
		{
			StyleValue<Texture2D> styleValue = default(StyleValue<Texture2D>);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				customProperty.data.Apply<Texture2D>(customProperty.handles, customProperty.specificity, ref styleValue, new HandlesApplicatorFunction<Texture2D>(StyleSheetApplicator.ApplyImage));
			}
			target.Apply(styleValue, StylePropertyApplyMode.CopyIfNotInline);
		}

		public void ApplyCustomProperty(string propertyName, ref StyleValue<string> target)
		{
			StyleValue<string> styleValue = new StyleValue<string>(string.Empty);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				styleValue.value = customProperty.data.ReadAsString(customProperty.handles[0]);
				styleValue.specificity = customProperty.specificity;
			}
			target.Apply(styleValue, StylePropertyApplyMode.CopyIfNotInline);
		}

		internal void ApplyCustomProperty<T>(string propertyName, ref StyleValue<T> target, StyleValueType valueType, HandlesApplicatorFunction<T> applicatorFunc)
		{
			StyleValue<T> styleValue = default(StyleValue<T>);
			CustomProperty customProperty;
			if (this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProperty))
			{
				StyleValueHandle styleValueHandle = customProperty.handles[0];
				if (styleValueHandle.valueType == valueType)
				{
					customProperty.data.Apply<T>(customProperty.handles, customProperty.specificity, ref styleValue, applicatorFunc);
				}
				else
				{
					Debug.LogWarning(string.Format("Trying to read value as {0} while parsed type is {1}", valueType, styleValueHandle.valueType));
				}
			}
			target.Apply(styleValue, StylePropertyApplyMode.CopyIfNotInline);
		}

		public static VisualElementStylesData none = new VisualElementStylesData(true);

		internal readonly bool isShared;

		internal YogaNode yogaNode;

		internal Dictionary<string, CustomProperty> m_CustomProperties;

		internal StyleValue<float> width;

		internal StyleValue<float> height;

		internal StyleValue<float> maxWidth;

		internal StyleValue<float> maxHeight;

		internal StyleValue<float> minWidth;

		internal StyleValue<float> minHeight;

		internal StyleValue<FloatOrKeyword> flexBasis;

		internal StyleValue<float> flexShrink;

		internal StyleValue<float> flexGrow;

		internal StyleValue<int> overflow;

		internal StyleValue<float> positionLeft;

		internal StyleValue<float> positionTop;

		internal StyleValue<float> positionRight;

		internal StyleValue<float> positionBottom;

		internal StyleValue<float> marginLeft;

		internal StyleValue<float> marginTop;

		internal StyleValue<float> marginRight;

		internal StyleValue<float> marginBottom;

		internal StyleValue<float> paddingLeft;

		internal StyleValue<float> paddingTop;

		internal StyleValue<float> paddingRight;

		internal StyleValue<float> paddingBottom;

		internal StyleValue<int> positionType;

		internal StyleValue<int> alignSelf;

		internal StyleValue<int> unityTextAlign;

		internal StyleValue<int> fontStyleAndWeight;

		internal StyleValue<int> textClipping;

		internal StyleValue<Font> font;

		internal StyleValue<int> fontSize;

		internal StyleValue<bool> wordWrap;

		internal StyleValue<Color> color;

		internal StyleValue<int> flexDirection;

		internal StyleValue<Color> backgroundColor;

		internal StyleValue<Color> borderColor;

		internal StyleValue<Texture2D> backgroundImage;

		internal StyleValue<int> backgroundScaleMode;

		internal StyleValue<int> alignItems;

		internal StyleValue<int> alignContent;

		internal StyleValue<int> justifyContent;

		internal StyleValue<int> flexWrap;

		internal StyleValue<float> borderLeftWidth;

		internal StyleValue<float> borderTopWidth;

		internal StyleValue<float> borderRightWidth;

		internal StyleValue<float> borderBottomWidth;

		internal StyleValue<float> borderTopLeftRadius;

		internal StyleValue<float> borderTopRightRadius;

		internal StyleValue<float> borderBottomRightRadius;

		internal StyleValue<float> borderBottomLeftRadius;

		internal StyleValue<int> sliceLeft;

		internal StyleValue<int> sliceTop;

		internal StyleValue<int> sliceRight;

		internal StyleValue<int> sliceBottom;

		internal StyleValue<float> opacity;

		internal StyleValue<CursorStyle> cursor;

		internal StyleValue<int> visibility;

		internal const Align DefaultAlignContent = Align.FlexStart;

		internal const Align DefaultAlignItems = Align.Stretch;
	}
}
