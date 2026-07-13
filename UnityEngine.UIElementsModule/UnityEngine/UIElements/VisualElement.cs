using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Profiling;
using Unity.Properties;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements.Layout;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	public class VisualElement : Focusable, IResolvedStyle, IStylePropertyAnimations, ITransform, ITransitionAnimations, IExperimentalFeatures, IVisualElementScheduler
	{
		Align IResolvedStyle.alignContent
		{
			get
			{
				return this.resolvedStyle.alignContent;
			}
		}

		Align IResolvedStyle.alignItems
		{
			get
			{
				return this.resolvedStyle.alignItems;
			}
		}

		Align IResolvedStyle.alignSelf
		{
			get
			{
				return this.resolvedStyle.alignSelf;
			}
		}

		Ratio IResolvedStyle.aspectRatio
		{
			get
			{
				return this.resolvedStyle.aspectRatio;
			}
		}

		Color IResolvedStyle.backgroundColor
		{
			get
			{
				return this.resolvedStyle.backgroundColor;
			}
		}

		Background IResolvedStyle.backgroundImage
		{
			get
			{
				return this.resolvedStyle.backgroundImage;
			}
		}

		BackgroundPosition IResolvedStyle.backgroundPositionX
		{
			get
			{
				return this.resolvedStyle.backgroundPositionX;
			}
		}

		BackgroundPosition IResolvedStyle.backgroundPositionY
		{
			get
			{
				return this.resolvedStyle.backgroundPositionY;
			}
		}

		BackgroundRepeat IResolvedStyle.backgroundRepeat
		{
			get
			{
				return this.resolvedStyle.backgroundRepeat;
			}
		}

		BackgroundSize IResolvedStyle.backgroundSize
		{
			get
			{
				return this.resolvedStyle.backgroundSize;
			}
		}

		Color IResolvedStyle.borderBottomColor
		{
			get
			{
				return this.resolvedStyle.borderBottomColor;
			}
		}

		float IResolvedStyle.borderBottomLeftRadius
		{
			get
			{
				return this.resolvedStyle.borderBottomLeftRadius;
			}
		}

		float IResolvedStyle.borderBottomRightRadius
		{
			get
			{
				return this.resolvedStyle.borderBottomRightRadius;
			}
		}

		float IResolvedStyle.borderBottomWidth
		{
			get
			{
				return this.resolvedStyle.borderBottomWidth;
			}
		}

		Color IResolvedStyle.borderLeftColor
		{
			get
			{
				return this.resolvedStyle.borderLeftColor;
			}
		}

		float IResolvedStyle.borderLeftWidth
		{
			get
			{
				return this.resolvedStyle.borderLeftWidth;
			}
		}

		Color IResolvedStyle.borderRightColor
		{
			get
			{
				return this.resolvedStyle.borderRightColor;
			}
		}

		float IResolvedStyle.borderRightWidth
		{
			get
			{
				return this.resolvedStyle.borderRightWidth;
			}
		}

		Color IResolvedStyle.borderTopColor
		{
			get
			{
				return this.resolvedStyle.borderTopColor;
			}
		}

		float IResolvedStyle.borderTopLeftRadius
		{
			get
			{
				return this.resolvedStyle.borderTopLeftRadius;
			}
		}

		float IResolvedStyle.borderTopRightRadius
		{
			get
			{
				return this.resolvedStyle.borderTopRightRadius;
			}
		}

		float IResolvedStyle.borderTopWidth
		{
			get
			{
				return this.resolvedStyle.borderTopWidth;
			}
		}

		float IResolvedStyle.bottom
		{
			get
			{
				return this.resolvedStyle.bottom;
			}
		}

		Color IResolvedStyle.color
		{
			get
			{
				return this.resolvedStyle.color;
			}
		}

		DisplayStyle IResolvedStyle.display
		{
			get
			{
				return this.resolvedStyle.display;
			}
		}

		IEnumerable<FilterFunction> IResolvedStyle.filter
		{
			get
			{
				return this.resolvedStyle.filter;
			}
		}

		StyleFloat IResolvedStyle.flexBasis
		{
			get
			{
				return this.resolvedStyle.flexBasis;
			}
		}

		FlexDirection IResolvedStyle.flexDirection
		{
			get
			{
				return this.resolvedStyle.flexDirection;
			}
		}

		float IResolvedStyle.flexGrow
		{
			get
			{
				return this.resolvedStyle.flexGrow;
			}
		}

		float IResolvedStyle.flexShrink
		{
			get
			{
				return this.resolvedStyle.flexShrink;
			}
		}

		Wrap IResolvedStyle.flexWrap
		{
			get
			{
				return this.resolvedStyle.flexWrap;
			}
		}

		float IResolvedStyle.fontSize
		{
			get
			{
				return this.resolvedStyle.fontSize;
			}
		}

		float IResolvedStyle.height
		{
			get
			{
				return this.resolvedStyle.height;
			}
		}

		Justify IResolvedStyle.justifyContent
		{
			get
			{
				return this.resolvedStyle.justifyContent;
			}
		}

		float IResolvedStyle.left
		{
			get
			{
				return this.resolvedStyle.left;
			}
		}

		float IResolvedStyle.letterSpacing
		{
			get
			{
				return this.resolvedStyle.letterSpacing;
			}
		}

		float IResolvedStyle.marginBottom
		{
			get
			{
				return this.resolvedStyle.marginBottom;
			}
		}

		float IResolvedStyle.marginLeft
		{
			get
			{
				return this.resolvedStyle.marginLeft;
			}
		}

		float IResolvedStyle.marginRight
		{
			get
			{
				return this.resolvedStyle.marginRight;
			}
		}

		float IResolvedStyle.marginTop
		{
			get
			{
				return this.resolvedStyle.marginTop;
			}
		}

		StyleFloat IResolvedStyle.maxHeight
		{
			get
			{
				return this.resolvedStyle.maxHeight;
			}
		}

		StyleFloat IResolvedStyle.maxWidth
		{
			get
			{
				return this.resolvedStyle.maxWidth;
			}
		}

		StyleFloat IResolvedStyle.minHeight
		{
			get
			{
				return this.resolvedStyle.minHeight;
			}
		}

		StyleFloat IResolvedStyle.minWidth
		{
			get
			{
				return this.resolvedStyle.minWidth;
			}
		}

		float IResolvedStyle.opacity
		{
			get
			{
				return this.resolvedStyle.opacity;
			}
		}

		float IResolvedStyle.paddingBottom
		{
			get
			{
				return this.resolvedStyle.paddingBottom;
			}
		}

		float IResolvedStyle.paddingLeft
		{
			get
			{
				return this.resolvedStyle.paddingLeft;
			}
		}

		float IResolvedStyle.paddingRight
		{
			get
			{
				return this.resolvedStyle.paddingRight;
			}
		}

		float IResolvedStyle.paddingTop
		{
			get
			{
				return this.resolvedStyle.paddingTop;
			}
		}

		Position IResolvedStyle.position
		{
			get
			{
				return this.resolvedStyle.position;
			}
		}

		float IResolvedStyle.right
		{
			get
			{
				return this.resolvedStyle.right;
			}
		}

		Rotate IResolvedStyle.rotate
		{
			get
			{
				return this.resolvedStyle.rotate;
			}
		}

		Scale IResolvedStyle.scale
		{
			get
			{
				return this.resolvedStyle.scale;
			}
		}

		TextOverflow IResolvedStyle.textOverflow
		{
			get
			{
				return this.resolvedStyle.textOverflow;
			}
		}

		float IResolvedStyle.top
		{
			get
			{
				return this.resolvedStyle.top;
			}
		}

		Vector3 IResolvedStyle.transformOrigin
		{
			get
			{
				return this.resolvedStyle.transformOrigin;
			}
		}

		IEnumerable<TimeValue> IResolvedStyle.transitionDelay
		{
			get
			{
				return this.resolvedStyle.transitionDelay;
			}
		}

		IEnumerable<TimeValue> IResolvedStyle.transitionDuration
		{
			get
			{
				return this.resolvedStyle.transitionDuration;
			}
		}

		IEnumerable<StylePropertyName> IResolvedStyle.transitionProperty
		{
			get
			{
				return this.resolvedStyle.transitionProperty;
			}
		}

		IEnumerable<EasingFunction> IResolvedStyle.transitionTimingFunction
		{
			get
			{
				return this.resolvedStyle.transitionTimingFunction;
			}
		}

		Vector3 IResolvedStyle.translate
		{
			get
			{
				return this.resolvedStyle.translate;
			}
		}

		Color IResolvedStyle.unityBackgroundImageTintColor
		{
			get
			{
				return this.resolvedStyle.unityBackgroundImageTintColor;
			}
		}

		EditorTextRenderingMode IResolvedStyle.unityEditorTextRenderingMode
		{
			get
			{
				return this.resolvedStyle.unityEditorTextRenderingMode;
			}
		}

		Font IResolvedStyle.unityFont
		{
			get
			{
				return this.resolvedStyle.unityFont;
			}
		}

		FontDefinition IResolvedStyle.unityFontDefinition
		{
			get
			{
				return this.resolvedStyle.unityFontDefinition;
			}
		}

		FontStyle IResolvedStyle.unityFontStyleAndWeight
		{
			get
			{
				return this.resolvedStyle.unityFontStyleAndWeight;
			}
		}

		MaterialDefinition IResolvedStyle.unityMaterial
		{
			get
			{
				return this.resolvedStyle.unityMaterial;
			}
		}

		float IResolvedStyle.unityParagraphSpacing
		{
			get
			{
				return this.resolvedStyle.unityParagraphSpacing;
			}
		}

		int IResolvedStyle.unitySliceBottom
		{
			get
			{
				return this.resolvedStyle.unitySliceBottom;
			}
		}

		int IResolvedStyle.unitySliceLeft
		{
			get
			{
				return this.resolvedStyle.unitySliceLeft;
			}
		}

		int IResolvedStyle.unitySliceRight
		{
			get
			{
				return this.resolvedStyle.unitySliceRight;
			}
		}

		float IResolvedStyle.unitySliceScale
		{
			get
			{
				return this.resolvedStyle.unitySliceScale;
			}
		}

		int IResolvedStyle.unitySliceTop
		{
			get
			{
				return this.resolvedStyle.unitySliceTop;
			}
		}

		SliceType IResolvedStyle.unitySliceType
		{
			get
			{
				return this.resolvedStyle.unitySliceType;
			}
		}

		TextAnchor IResolvedStyle.unityTextAlign
		{
			get
			{
				return this.resolvedStyle.unityTextAlign;
			}
		}

		TextGeneratorType IResolvedStyle.unityTextGenerator
		{
			get
			{
				return this.resolvedStyle.unityTextGenerator;
			}
		}

		Color IResolvedStyle.unityTextOutlineColor
		{
			get
			{
				return this.resolvedStyle.unityTextOutlineColor;
			}
		}

		float IResolvedStyle.unityTextOutlineWidth
		{
			get
			{
				return this.resolvedStyle.unityTextOutlineWidth;
			}
		}

		TextOverflowPosition IResolvedStyle.unityTextOverflowPosition
		{
			get
			{
				return this.resolvedStyle.unityTextOverflowPosition;
			}
		}

		Visibility IResolvedStyle.visibility
		{
			get
			{
				return this.resolvedStyle.visibility;
			}
		}

		WhiteSpace IResolvedStyle.whiteSpace
		{
			get
			{
				return this.resolvedStyle.whiteSpace;
			}
		}

		float IResolvedStyle.width
		{
			get
			{
				return this.resolvedStyle.width;
			}
		}

		float IResolvedStyle.wordSpacing
		{
			get
			{
				return this.resolvedStyle.wordSpacing;
			}
		}

		internal bool hasRunningAnimations
		{
			get
			{
				return this.styleAnimation.runningAnimationCount > 0;
			}
		}

		internal bool hasCompletedAnimations
		{
			get
			{
				return this.styleAnimation.completedAnimationCount > 0;
			}
		}

		int IStylePropertyAnimations.runningAnimationCount { get; set; }

		int IStylePropertyAnimations.completedAnimationCount { get; set; }

		private IStylePropertyAnimationSystem GetStylePropertyAnimationSystem()
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			return (elementPanel != null) ? elementPanel.styleAnimationSystem : null;
		}

		internal IStylePropertyAnimations styleAnimation
		{
			get
			{
				return this;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool HasRunningAnimation(StylePropertyId id)
		{
			IStylePropertyAnimationSystem stylePropertyAnimationSystem = this.GetStylePropertyAnimationSystem();
			return stylePropertyAnimationSystem != null && stylePropertyAnimationSystem.HasRunningAnimation(this, id);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void CancelAnimation(StylePropertyId id)
		{
			IStylePropertyAnimationSystem stylePropertyAnimationSystem = this.GetStylePropertyAnimationSystem();
			if (stylePropertyAnimationSystem != null)
			{
				stylePropertyAnimationSystem.CancelAnimation(this, id);
			}
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, float from, float to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, int from, int to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Length from, Length to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Color from, Color to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.StartEnum(StylePropertyId id, int from, int to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransitionEnum(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Background from, Background to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, FontDefinition from, FontDefinition to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Font from, Font to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, TextShadow from, TextShadow to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Scale from, Scale to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Translate from, Translate to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Rotate from, Rotate to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, Ratio from, Ratio to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, TransformOrigin from, TransformOrigin to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, BackgroundPosition from, BackgroundPosition to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, BackgroundRepeat from, BackgroundRepeat to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, BackgroundSize from, BackgroundSize to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, List<FilterFunction> from, List<FilterFunction> to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		bool IStylePropertyAnimations.Start(StylePropertyId id, MaterialDefinition from, MaterialDefinition to, int durationMs, int delayMs, Func<float, float> easingCurve)
		{
			return this.GetStylePropertyAnimationSystem().StartTransition(this, id, from, to, durationMs, delayMs, easingCurve);
		}

		void IStylePropertyAnimations.CancelAnimation(StylePropertyId id)
		{
			IStylePropertyAnimationSystem stylePropertyAnimationSystem = this.GetStylePropertyAnimationSystem();
			if (stylePropertyAnimationSystem != null)
			{
				stylePropertyAnimationSystem.CancelAnimation(this, id);
			}
		}

		void IStylePropertyAnimations.CancelAllAnimations()
		{
			bool flag = this.hasRunningAnimations || this.hasCompletedAnimations;
			if (flag)
			{
				IStylePropertyAnimationSystem stylePropertyAnimationSystem = this.GetStylePropertyAnimationSystem();
				if (stylePropertyAnimationSystem != null)
				{
					stylePropertyAnimationSystem.CancelAllAnimations(this);
				}
			}
		}

		bool IStylePropertyAnimations.HasRunningAnimation(StylePropertyId id)
		{
			return this.hasRunningAnimations && this.GetStylePropertyAnimationSystem().HasRunningAnimation(this, id);
		}

		void IStylePropertyAnimations.UpdateAnimation(StylePropertyId id)
		{
			this.GetStylePropertyAnimationSystem().UpdateAnimation(this, id);
		}

		void IStylePropertyAnimations.GetAllAnimations(List<StylePropertyId> outPropertyIds)
		{
			bool flag = this.hasRunningAnimations || this.hasCompletedAnimations;
			if (flag)
			{
				this.GetStylePropertyAnimationSystem().GetAllAnimations(this, outPropertyIds);
			}
		}

		internal bool TryConvertLengthUnits(StylePropertyId id, ref Length from, ref Length to, int subPropertyIndex = 0)
		{
			bool flag = from.IsAuto() || from.IsNone() || to.IsAuto() || to.IsNone();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = float.IsNaN(from.value) || float.IsNaN(to.value);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = from.unit == to.unit;
					if (flag4)
					{
						flag2 = true;
					}
					else
					{
						bool flag5 = to.unit == LengthUnit.Pixel;
						if (flag5)
						{
							bool flag6 = Mathf.Approximately(from.value, 0f);
							if (flag6)
							{
								from = new Length(0f, LengthUnit.Pixel);
								return true;
							}
							float? parentSizeForLengthConversion = this.GetParentSizeForLengthConversion(id, subPropertyIndex);
							bool flag7 = parentSizeForLengthConversion == null || parentSizeForLengthConversion.Value < 0f;
							if (flag7)
							{
								return false;
							}
							from = new Length(from.value * parentSizeForLengthConversion.Value / 100f, LengthUnit.Pixel);
						}
						else
						{
							Assert.AreEqual<LengthUnit>(LengthUnit.Percent, to.unit);
							float? parentSizeForLengthConversion2 = this.GetParentSizeForLengthConversion(id, subPropertyIndex);
							bool flag8 = parentSizeForLengthConversion2 == null || parentSizeForLengthConversion2.Value <= 0f;
							if (flag8)
							{
								return false;
							}
							from = new Length(from.value * 100f / parentSizeForLengthConversion2.Value, LengthUnit.Percent);
						}
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		internal bool TryConvertTransformOriginUnits(ref TransformOrigin from, ref TransformOrigin to)
		{
			Length x = from.x;
			Length y = from.y;
			Length x2 = to.x;
			Length y2 = to.y;
			bool flag = !this.TryConvertLengthUnits(StylePropertyId.TransformOrigin, ref x, ref x2, 0);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = !this.TryConvertLengthUnits(StylePropertyId.TransformOrigin, ref y, ref y2, 1);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					from.x = x;
					from.y = y;
					flag2 = true;
				}
			}
			return flag2;
		}

		internal bool TryConvertTranslateUnits(ref Translate from, ref Translate to)
		{
			Length x = from.x;
			Length y = from.y;
			Length x2 = to.x;
			Length y2 = to.y;
			bool flag = !this.TryConvertLengthUnits(StylePropertyId.Translate, ref x, ref x2, 0);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = !this.TryConvertLengthUnits(StylePropertyId.Translate, ref y, ref y2, 1);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					from.x = x;
					from.y = y;
					flag2 = true;
				}
			}
			return flag2;
		}

		internal bool TryConvertBackgroundPositionUnits(ref BackgroundPosition from, ref BackgroundPosition to)
		{
			Length offset = from.offset;
			Length offset2 = to.offset;
			bool flag = !this.TryConvertLengthUnits(StylePropertyId.BackgroundPosition, ref offset, ref offset2, 0);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				from.offset = offset;
				flag2 = true;
			}
			return flag2;
		}

		internal bool TryConvertBackgroundSizeUnits(ref BackgroundSize from, ref BackgroundSize to)
		{
			Length x = from.x;
			Length y = from.y;
			Length x2 = to.x;
			Length y2 = to.y;
			bool flag = !this.TryConvertLengthUnits(StylePropertyId.BackgroundSize, ref x, ref x2, 0);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = !this.TryConvertLengthUnits(StylePropertyId.BackgroundSize, ref y, ref y2, 1);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					from.x = x;
					from.y = y;
					flag2 = true;
				}
			}
			return flag2;
		}

		private float? GetParentSizeForLengthConversion(StylePropertyId id, int subPropertyIndex = 0)
		{
			if (id <= StylePropertyId.WordSpacing)
			{
				if (id - StylePropertyId.FontSize <= 1 || id == StylePropertyId.UnityParagraphSpacing || id == StylePropertyId.WordSpacing)
				{
					return null;
				}
			}
			else if (id <= StylePropertyId.Translate)
			{
				switch (id)
				{
				case StylePropertyId.Bottom:
				case StylePropertyId.Height:
				case StylePropertyId.MaxHeight:
				case StylePropertyId.MinHeight:
				case StylePropertyId.Top:
				{
					VisualElement parent = this.hierarchy.parent;
					return (parent != null) ? new float?(parent.resolvedStyle.height) : null;
				}
				case StylePropertyId.Display:
				case StylePropertyId.FlexDirection:
				case StylePropertyId.FlexGrow:
				case StylePropertyId.FlexShrink:
				case StylePropertyId.FlexWrap:
				case StylePropertyId.JustifyContent:
				case StylePropertyId.Position:
					break;
				case StylePropertyId.FlexBasis:
				{
					bool flag = this.hierarchy.parent == null;
					if (flag)
					{
						return null;
					}
					FlexDirection flexDirection = this.hierarchy.parent.resolvedStyle.flexDirection;
					FlexDirection flexDirection2 = flexDirection;
					if (flexDirection2 > FlexDirection.ColumnReverse)
					{
						return new float?(this.hierarchy.parent.resolvedStyle.width);
					}
					return new float?(this.hierarchy.parent.resolvedStyle.height);
				}
				case StylePropertyId.Left:
				case StylePropertyId.MarginBottom:
				case StylePropertyId.MarginLeft:
				case StylePropertyId.MarginRight:
				case StylePropertyId.MarginTop:
				case StylePropertyId.MaxWidth:
				case StylePropertyId.MinWidth:
				case StylePropertyId.PaddingBottom:
				case StylePropertyId.PaddingLeft:
				case StylePropertyId.PaddingRight:
				case StylePropertyId.PaddingTop:
				case StylePropertyId.Right:
				case StylePropertyId.Width:
				{
					VisualElement parent2 = this.hierarchy.parent;
					return (parent2 != null) ? new float?(parent2.resolvedStyle.width) : null;
				}
				default:
					if (id - StylePropertyId.TransformOrigin <= 1)
					{
						return new float?((subPropertyIndex == 0) ? this.resolvedStyle.width : this.resolvedStyle.height);
					}
					break;
				}
			}
			else if (id - StylePropertyId.BorderBottomLeftRadius <= 1 || id - StylePropertyId.BorderTopLeftRadius <= 1)
			{
				return new float?(this.resolvedStyle.width);
			}
			return null;
		}

		internal bool isCompositeRoot
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.CompositeRoot) == VisualElementFlags.CompositeRoot;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.CompositeRoot) : (this.m_Flags & ~VisualElementFlags.CompositeRoot));
			}
		}

		internal bool areAncestorsAndSelfDisplayed
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.HierarchyDisplayed) == VisualElementFlags.HierarchyDisplayed;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.HierarchyDisplayed) : (this.m_Flags & ~VisualElementFlags.HierarchyDisplayed));
				bool flag = this.renderData == null;
				if (!flag)
				{
					bool flag2 = value && (this.renderData.pendingRepaint || this.renderData.pendingHierarchicalRepaint);
					if (flag2)
					{
						this.IncrementVersion(VersionChangeType.Repaint);
					}
				}
			}
		}

		internal bool hasOneOrMorePointerCaptures
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.PointerCapture) == VisualElementFlags.PointerCapture;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.PointerCapture) : (this.m_Flags & ~VisualElementFlags.PointerCapture));
			}
		}

		internal VisualElementFlags flags
		{
			get
			{
				RenderData renderData = this.renderData;
				bool flag;
				if (renderData == null || (renderData.flags & RenderDataFlags.IsClippingRectDirty) != RenderDataFlags.IsClippingRectDirty)
				{
					RenderData renderData2 = this.nestedRenderData;
					flag = renderData2 != null && (renderData2.flags & RenderDataFlags.IsClippingRectDirty) == RenderDataFlags.IsClippingRectDirty;
				}
				else
				{
					flag = true;
				}
				bool flag2 = flag;
				VisualElementFlags visualElementFlags;
				if (flag2)
				{
					visualElementFlags = this.m_Flags | VisualElementFlags.WorldClipDirty;
				}
				else
				{
					visualElementFlags = this.m_Flags;
				}
				return visualElementFlags;
			}
			set
			{
				this.m_Flags = value;
				bool flag = (this.m_Flags & VisualElementFlags.WorldClipDirty) == VisualElementFlags.WorldClipDirty;
				if (flag)
				{
					bool flag2 = this.renderData != null;
					if (flag2)
					{
						this.renderData.flags |= RenderDataFlags.IsClippingRectDirty;
						this.m_Flags &= ~VisualElementFlags.WorldClipDirty;
					}
					bool flag3 = this.nestedRenderData != null;
					if (flag3)
					{
						this.nestedRenderData.flags |= RenderDataFlags.IsClippingRectDirty;
						Debug.Assert(this.renderData != null, "renderData should not be null when nestedRenderData is not null");
					}
				}
			}
		}

		[CreateProperty]
		public string viewDataKey
		{
			get
			{
				return this.m_ViewDataKey;
			}
			set
			{
				bool flag = this.m_ViewDataKey != value;
				if (flag)
				{
					this.m_ViewDataKey = value;
					bool flag2 = !string.IsNullOrEmpty(value);
					if (flag2)
					{
						this.IncrementVersion(VersionChangeType.ViewData);
					}
					base.NotifyPropertyChanged(in VisualElement.viewDataKeyProperty);
				}
			}
		}

		internal bool enableViewDataPersistence
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.EnableViewDataPersistence) == VisualElementFlags.EnableViewDataPersistence;
			}
			private set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.EnableViewDataPersistence) : (this.m_Flags & ~VisualElementFlags.EnableViewDataPersistence));
			}
		}

		[CreateProperty]
		public object userData
		{
			get
			{
				bool flag = this.m_PropertyBag != null;
				object obj2;
				if (flag)
				{
					object obj;
					this.m_PropertyBag.TryGetValue(VisualElement.userDataPropertyKey, out obj);
					obj2 = obj;
				}
				else
				{
					obj2 = null;
				}
				return obj2;
			}
			set
			{
				object userData = this.userData;
				this.SetPropertyInternal(VisualElement.userDataPropertyKey, value);
				bool flag = userData != this.userData;
				if (flag)
				{
					base.NotifyPropertyChanged(in VisualElement.userDataProperty);
				}
			}
		}

		public override bool canGrabFocus
		{
			get
			{
				bool flag = false;
				for (VisualElement visualElement = this.hierarchy.parent; visualElement != null; visualElement = visualElement.parent)
				{
					bool isCompositeRoot = visualElement.isCompositeRoot;
					if (isCompositeRoot)
					{
						flag |= !visualElement.canGrabFocus;
						break;
					}
				}
				return !flag && this.visible && this.resolvedStyle.display != DisplayStyle.None && this.enabledInHierarchy && base.canGrabFocus;
			}
		}

		public override FocusController focusController
		{
			get
			{
				IPanel panel = this.panel;
				return (panel != null) ? panel.focusController : null;
			}
		}

		[CreateProperty]
		public bool disablePlayModeTint
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		internal Color playModeTintColor
		{
			get
			{
				return this.disablePlayModeTint ? Color.white : UIElementsUtility.editorPlayModeTintColor;
			}
		}

		[CreateProperty]
		public UsageHints usageHints
		{
			get
			{
				return (((this.renderHints & RenderHints.GroupTransform) != RenderHints.None) ? UsageHints.GroupTransform : UsageHints.None) | (((this.renderHints & RenderHints.BoneTransform) != RenderHints.None) ? UsageHints.DynamicTransform : UsageHints.None) | (((this.renderHints & RenderHints.MaskContainer) != RenderHints.None) ? UsageHints.MaskContainer : UsageHints.None) | (((this.renderHints & RenderHints.DynamicColor) != RenderHints.None) ? UsageHints.DynamicColor : UsageHints.None) | (((this.renderHints & RenderHints.DynamicPostProcessing) != RenderHints.None) ? UsageHints.DynamicPostProcessing : UsageHints.None) | (((this.renderHints & RenderHints.LargePixelCoverage) != RenderHints.None) ? UsageHints.LargePixelCoverage : UsageHints.None);
			}
			set
			{
				bool flag = (value & UsageHints.GroupTransform) > UsageHints.None;
				if (flag)
				{
					this.renderHints |= RenderHints.GroupTransform;
				}
				else
				{
					this.renderHints &= ~RenderHints.GroupTransform;
				}
				bool flag2 = (value & UsageHints.DynamicTransform) > UsageHints.None;
				if (flag2)
				{
					this.renderHints |= RenderHints.BoneTransform;
				}
				else
				{
					this.renderHints &= ~RenderHints.BoneTransform;
				}
				bool flag3 = (value & UsageHints.MaskContainer) > UsageHints.None;
				if (flag3)
				{
					this.renderHints |= RenderHints.MaskContainer;
				}
				else
				{
					this.renderHints &= ~RenderHints.MaskContainer;
				}
				bool flag4 = (value & UsageHints.DynamicColor) > UsageHints.None;
				if (flag4)
				{
					this.renderHints |= RenderHints.DynamicColor;
				}
				else
				{
					this.renderHints &= ~RenderHints.DynamicColor;
				}
				bool flag5 = (value & UsageHints.DynamicPostProcessing) > UsageHints.None;
				if (flag5)
				{
					this.renderHints |= RenderHints.DynamicPostProcessing;
				}
				else
				{
					this.renderHints &= ~RenderHints.DynamicPostProcessing;
				}
				bool flag6 = (value & UsageHints.LargePixelCoverage) > UsageHints.None;
				if (flag6)
				{
					this.renderHints |= RenderHints.LargePixelCoverage;
				}
				else
				{
					this.renderHints &= ~RenderHints.LargePixelCoverage;
				}
				base.NotifyPropertyChanged(in VisualElement.usageHintsProperty);
			}
		}

		internal RenderHints renderHints
		{
			get
			{
				return this.m_RenderHints;
			}
			set
			{
				RenderHints renderHints = this.m_RenderHints & ~(RenderHints.DirtyGroupTransform | RenderHints.DirtyBoneTransform | RenderHints.DirtyClipWithScissors | RenderHints.DirtyMaskContainer | RenderHints.DirtyDynamicColor | RenderHints.DirtyDynamicPostProcessing | RenderHints.DirtyLargePixelCoverage);
				RenderHints renderHints2 = value & ~(RenderHints.DirtyGroupTransform | RenderHints.DirtyBoneTransform | RenderHints.DirtyClipWithScissors | RenderHints.DirtyMaskContainer | RenderHints.DirtyDynamicColor | RenderHints.DirtyDynamicPostProcessing | RenderHints.DirtyLargePixelCoverage);
				RenderHints renderHints3 = renderHints ^ renderHints2;
				bool flag = renderHints3 > RenderHints.None;
				if (flag)
				{
					RenderHints renderHints4 = this.m_RenderHints & RenderHints.DirtyAll;
					RenderHints renderHints5 = renderHints3 << 7;
					this.m_RenderHints = renderHints2 | renderHints4 | renderHints5;
					this.IncrementVersion(VersionChangeType.RenderHints);
				}
			}
		}

		internal void MarkRenderHintsClean()
		{
			this.m_RenderHints &= ~(RenderHints.DirtyGroupTransform | RenderHints.DirtyBoneTransform | RenderHints.DirtyClipWithScissors | RenderHints.DirtyMaskContainer | RenderHints.DirtyDynamicColor | RenderHints.DirtyDynamicPostProcessing | RenderHints.DirtyLargePixelCoverage);
		}

		internal bool useRenderTexture
		{
			get
			{
				Rect layout = this.layout;
				bool flag = layout.width <= 0f || layout.height <= 0f || float.IsNaN(layout.width) || float.IsNaN(layout.height);
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					List<FilterFunction> list = this.resolvedStyle.filter as List<FilterFunction>;
					bool flag3 = list == null;
					if (flag3)
					{
						throw new ArgumentException("resolvedStyle.filter is not a List<FilterFunction>");
					}
					bool flag4 = false;
					foreach (FilterFunction filterFunction in list)
					{
						FilterFunctionDefinition definition = filterFunction.GetDefinition();
						bool flag5 = definition == null;
						if (!flag5)
						{
							foreach (PostProcessingPass postProcessingPass in definition.passes)
							{
								bool flag6 = postProcessingPass.material != null;
								if (flag6)
								{
									flag4 = true;
									break;
								}
							}
						}
					}
					flag2 = flag4 || (this.renderHints & RenderHints.DynamicPostProcessing) > RenderHints.None;
				}
				return flag2;
			}
		}

		[Obsolete("When writing the value, use VisualElement.style.translate, VisualElement.style.rotate or VisualElement.style.scale instead. When reading the value, use VisualElement.resolvedStyle.translate, scale and rotate")]
		public ITransform transform
		{
			get
			{
				return this;
			}
		}

		Vector3 ITransform.position
		{
			get
			{
				return this.resolvedStyle.translate;
			}
			set
			{
				this.style.translate = new Translate(value.x, value.y, value.z);
			}
		}

		Quaternion ITransform.rotation
		{
			get
			{
				return this.resolvedStyle.rotate.ToQuaternion();
			}
			set
			{
				this.style.rotate = new Rotate(value);
			}
		}

		Vector3 ITransform.scale
		{
			get
			{
				Vector3 value = this.resolvedStyle.scale.value;
				BaseVisualElementPanel elementPanel = this.elementPanel;
				bool flag = elementPanel != null && elementPanel.isFlat;
				if (flag)
				{
					value.z = 1f;
				}
				return value;
			}
			set
			{
				this.style.scale = new Scale(value);
			}
		}

		Matrix4x4 ITransform.matrix
		{
			get
			{
				Vector3 value = this.resolvedStyle.scale.value;
				BaseVisualElementPanel elementPanel = this.elementPanel;
				bool flag = elementPanel != null && elementPanel.isFlat;
				if (flag)
				{
					value.z = 1f;
				}
				return Matrix4x4.TRS(this.resolvedStyle.translate, this.resolvedStyle.rotate.ToQuaternion(), value);
			}
		}

		internal bool isLayoutManual
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.LayoutManual) == VisualElementFlags.LayoutManual;
			}
			private set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.LayoutManual) : (this.m_Flags & ~VisualElementFlags.LayoutManual));
			}
		}

		public float scaledPixelsPerPoint
		{
			get
			{
				bool flag = this.elementPanel == null;
				float num;
				if (flag)
				{
					Debug.LogWarning("Trying to access the DPI setting of a visual element that is not on a panel.");
					num = GUIUtility.pixelsPerPoint;
				}
				else
				{
					num = this.elementPanel.scaledPixelsPerPoint;
				}
				return num;
			}
		}

		[Obsolete("scaledPixelsPerPoint_noChecks is deprecated. Use scaledPixelsPerPoint instead.")]
		internal float scaledPixelsPerPoint_noChecks
		{
			get
			{
				BaseVisualElementPanel elementPanel = this.elementPanel;
				return (elementPanel != null) ? elementPanel.scaledPixelsPerPoint : GUIUtility.pixelsPerPoint;
			}
		}

		[Obsolete("unityBackgroundScaleMode is deprecated. Use background-* properties instead.")]
		StyleEnum<ScaleMode> IResolvedStyle.unityBackgroundScaleMode
		{
			get
			{
				return this.resolvedStyle.unityBackgroundScaleMode;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public Rect layout
		{
			get
			{
				Rect layout = this.m_Layout;
				bool flag = !this.layoutNode.IsUndefined && !this.isLayoutManual;
				if (flag)
				{
					layout.x = this.layoutNode.LayoutX;
					layout.y = this.layoutNode.LayoutY;
					layout.width = this.layoutNode.LayoutWidth;
					layout.height = this.layoutNode.LayoutHeight;
				}
				return layout;
			}
			internal set
			{
				bool flag = this.isLayoutManual && this.m_Layout == value;
				if (!flag)
				{
					Rect layout = this.layout;
					VersionChangeType versionChangeType = (VersionChangeType)0;
					bool flag2 = !Mathf.Approximately(layout.x, value.x) || !Mathf.Approximately(layout.y, value.y);
					if (flag2)
					{
						versionChangeType |= VersionChangeType.Transform;
					}
					bool flag3 = !Mathf.Approximately(layout.width, value.width) || !Mathf.Approximately(layout.height, value.height);
					if (flag3)
					{
						versionChangeType |= VersionChangeType.Size;
					}
					this.m_Layout = value;
					this.isLayoutManual = true;
					IStyle style = this.style;
					style.position = Position.Absolute;
					style.marginLeft = 0f;
					style.marginRight = 0f;
					style.marginBottom = 0f;
					style.marginTop = 0f;
					style.left = value.x;
					style.top = value.y;
					style.right = float.NaN;
					style.bottom = float.NaN;
					style.width = value.width;
					style.height = value.height;
					bool flag4 = versionChangeType > (VersionChangeType)0;
					if (flag4)
					{
						this.IncrementVersion(versionChangeType);
					}
				}
			}
		}

		internal void ClearManualLayout()
		{
			this.isLayoutManual = false;
			IStyle style = this.style;
			style.position = StyleKeyword.Null;
			style.marginLeft = StyleKeyword.Null;
			style.marginRight = StyleKeyword.Null;
			style.marginBottom = StyleKeyword.Null;
			style.marginTop = StyleKeyword.Null;
			style.left = StyleKeyword.Null;
			style.top = StyleKeyword.Null;
			style.right = StyleKeyword.Null;
			style.bottom = StyleKeyword.Null;
			style.width = StyleKeyword.Null;
			style.height = StyleKeyword.Null;
		}

		[CreateProperty(ReadOnly = true)]
		public Rect contentRect
		{
			get
			{
				Spacing spacing = new Spacing(this.resolvedStyle.paddingLeft, this.resolvedStyle.paddingTop, this.resolvedStyle.paddingRight, this.resolvedStyle.paddingBottom);
				return this.paddingRect - spacing;
			}
		}

		protected Rect paddingRect
		{
			get
			{
				Spacing spacing = new Spacing(this.resolvedStyle.borderLeftWidth, this.resolvedStyle.borderTopWidth, this.resolvedStyle.borderRightWidth, this.resolvedStyle.borderBottomWidth);
				return this.rect - spacing;
			}
		}

		internal bool needs3DBounds
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.Needs3DBounds) > (VisualElementFlags)0;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.Needs3DBounds) : (this.m_Flags & ~VisualElementFlags.Needs3DBounds));
			}
		}

		internal bool isLocalBounds3DDirty
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.LocalBounds3DDirty) > (VisualElementFlags)0;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.LocalBounds3DDirty) : (this.m_Flags & ~VisualElementFlags.LocalBounds3DDirty));
			}
		}

		internal bool isLocalBoundsWithoutNested3DDirty
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.LocalBoundsWithoutNested3DDirty) > (VisualElementFlags)0;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.LocalBoundsWithoutNested3DDirty) : (this.m_Flags & ~VisualElementFlags.LocalBoundsWithoutNested3DDirty));
			}
		}

		internal bool isBoundingBoxDirty
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.BoundingBoxDirty) == VisualElementFlags.BoundingBoxDirty;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.BoundingBoxDirty) : (this.m_Flags & ~VisualElementFlags.BoundingBoxDirty));
			}
		}

		internal bool isWorldBoundingBoxDirty
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.WorldBoundingBoxDirty) == VisualElementFlags.WorldBoundingBoxDirty;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.WorldBoundingBoxDirty) : (this.m_Flags & ~VisualElementFlags.WorldBoundingBoxDirty));
			}
		}

		internal bool isWorldBoundingBoxOrDependenciesDirty
		{
			get
			{
				return (this.m_Flags & (VisualElementFlags.WorldTransformDirty | VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty)) > (VisualElementFlags)0;
			}
		}

		internal Rect boundingBox
		{
			get
			{
				bool isBoundingBoxDirty = this.isBoundingBoxDirty;
				if (isBoundingBoxDirty)
				{
					this.UpdateBoundingBox();
					this.isBoundingBoxDirty = false;
				}
				return this.m_BoundingBox;
			}
		}

		internal Rect boundingBoxWithoutNested
		{
			get
			{
				bool isBoundingBoxDirty = this.isBoundingBoxDirty;
				if (isBoundingBoxDirty)
				{
					this.UpdateBoundingBox();
					this.isBoundingBoxDirty = false;
				}
				return WorldSpaceDataStore.GetWorldSpaceData(this).boundingBoxWithoutNested;
			}
		}

		internal Rect worldBoundingBox
		{
			get
			{
				bool isWorldBoundingBoxOrDependenciesDirty = this.isWorldBoundingBoxOrDependenciesDirty;
				if (isWorldBoundingBoxOrDependenciesDirty)
				{
					this.UpdateWorldBoundingBox();
					this.isWorldBoundingBoxDirty = false;
				}
				return this.m_WorldBoundingBox;
			}
		}

		private Rect boundingBoxInParentSpace
		{
			get
			{
				Rect boundingBox = this.boundingBox;
				this.TransformAlignedRectToParentSpace(ref boundingBox);
				return boundingBox;
			}
		}

		internal void UpdateBoundingBox()
		{
			bool flag = this.elementPanel != null && !this.elementPanel.isFlat;
			Rect rect = this.rect;
			bool flag2 = float.IsNaN(rect.x) || float.IsNaN(rect.y) || float.IsNaN(rect.width) || float.IsNaN(rect.height);
			Rect rect2;
			if (flag2)
			{
				this.m_BoundingBox = Rect.zero;
				rect2 = Rect.zero;
			}
			else
			{
				this.m_BoundingBox = rect;
				rect2 = rect;
				bool flag3 = !this.ShouldClip() && this.resolvedStyle.display == DisplayStyle.Flex;
				if (flag3)
				{
					int count = this.m_Children.Count;
					for (int i = 0; i < count; i++)
					{
						VisualElement visualElement = this.m_Children[i];
						bool flag4 = !visualElement.areAncestorsAndSelfDisplayed;
						if (!flag4)
						{
							Rect boundingBoxInParentSpace = visualElement.boundingBoxInParentSpace;
							this.m_BoundingBox.xMin = Math.Min(this.m_BoundingBox.xMin, boundingBoxInParentSpace.xMin);
							this.m_BoundingBox.xMax = Math.Max(this.m_BoundingBox.xMax, boundingBoxInParentSpace.xMax);
							this.m_BoundingBox.yMin = Math.Min(this.m_BoundingBox.yMin, boundingBoxInParentSpace.yMin);
							this.m_BoundingBox.yMax = Math.Max(this.m_BoundingBox.yMax, boundingBoxInParentSpace.yMax);
							bool flag5 = flag && !(visualElement is UIDocumentRootElement);
							if (flag5)
							{
								rect2.xMin = Math.Min(rect2.xMin, boundingBoxInParentSpace.xMin);
								rect2.xMax = Math.Max(rect2.xMax, boundingBoxInParentSpace.xMax);
								rect2.yMin = Math.Min(rect2.yMin, boundingBoxInParentSpace.yMin);
								rect2.yMax = Math.Max(rect2.yMax, boundingBoxInParentSpace.yMax);
							}
						}
					}
				}
			}
			bool flag6 = flag;
			if (flag6)
			{
				WorldSpaceData worldSpaceData = WorldSpaceDataStore.GetWorldSpaceData(this);
				worldSpaceData.boundingBoxWithoutNested = rect2;
				WorldSpaceDataStore.SetWorldSpaceData(this, worldSpaceData);
			}
			this.isWorldBoundingBoxDirty = true;
		}

		internal void UpdateWorldBoundingBox()
		{
			this.m_WorldBoundingBox = this.boundingBox;
			VisualElement.TransformAlignedRect(this.worldTransformRef, ref this.m_WorldBoundingBox);
		}

		internal Bounds localBounds3D
		{
			get
			{
				bool isLocalBounds3DDirty = this.isLocalBounds3DDirty;
				if (isLocalBounds3DDirty)
				{
					this.UpdateBounds3D();
					this.isLocalBounds3DDirty = false;
				}
				return WorldSpaceDataStore.GetWorldSpaceData(this).localBounds3D;
			}
		}

		internal Bounds localBoundsPicking3D
		{
			get
			{
				bool isLocalBounds3DDirty = this.isLocalBounds3DDirty;
				if (isLocalBounds3DDirty)
				{
					this.UpdateBounds3D();
					this.isLocalBounds3DDirty = false;
				}
				return WorldSpaceDataStore.GetWorldSpaceData(this).localBounds3D;
			}
		}

		internal Bounds localBounds3DWithoutNested3D
		{
			get
			{
				bool isLocalBoundsWithoutNested3DDirty = this.isLocalBoundsWithoutNested3DDirty;
				if (isLocalBoundsWithoutNested3DDirty)
				{
					this.UpdateBounds3D();
					this.isLocalBoundsWithoutNested3DDirty = false;
				}
				return WorldSpaceDataStore.GetWorldSpaceData(this).localBoundsWithoutNested3D;
			}
		}

		private void UpdateBounds3D()
		{
			bool flag = !this.areAncestorsAndSelfDisplayed;
			if (flag)
			{
				WorldSpaceDataStore.ClearLocalBounds3DData(this);
			}
			else
			{
				bool flag2 = !this.needs3DBounds;
				if (flag2)
				{
					Rect boundingBox = this.boundingBox;
					Bounds bounds = new Bounds(boundingBox.center, boundingBox.size);
					Rect boundingBoxWithoutNested = this.boundingBoxWithoutNested;
					Bounds bounds2 = new Bounds(boundingBoxWithoutNested.center, boundingBoxWithoutNested.size);
					WorldSpaceDataStore.SetWorldSpaceData(this, new WorldSpaceData
					{
						localBounds3D = bounds,
						localBoundsPicking3D = bounds,
						localBoundsWithoutNested3D = bounds2,
						boundingBoxWithoutNested = boundingBoxWithoutNested
					});
				}
				else
				{
					Bounds bounds3 = new Bounds(this.rect.center, this.rect.size);
					Bounds bounds4 = bounds3;
					Bounds bounds5 = ((this.pickingMode == PickingMode.Position) ? bounds4 : WorldSpaceData.k_Empty3DBounds);
					bool flag3 = !this.ShouldClip();
					if (flag3)
					{
						int childCount = this.hierarchy.childCount;
						for (int i = 0; i < childCount; i++)
						{
							VisualElement visualElement = this.hierarchy[i];
							bool flag4 = visualElement is UIDocumentRootElement;
							bool flag5 = !flag4;
							if (flag5)
							{
								Bounds localBounds3DWithoutNested3D = visualElement.localBounds3DWithoutNested3D;
								bool flag6 = localBounds3DWithoutNested3D.extents.x >= 0f;
								if (flag6)
								{
									visualElement.TransformAlignedBoundsToParentSpace(ref localBounds3DWithoutNested3D);
									bounds3.Encapsulate(localBounds3DWithoutNested3D);
								}
							}
							Bounds localBounds3D = visualElement.localBounds3D;
							bool flag7 = localBounds3D.extents.x >= 0f;
							if (flag7)
							{
								visualElement.TransformAlignedBoundsToParentSpace(ref localBounds3D);
								bounds4.Encapsulate(localBounds3D);
							}
							Bounds localBoundsPicking3D = visualElement.localBoundsPicking3D;
							bool flag8 = localBoundsPicking3D.extents.x >= 0f;
							if (flag8)
							{
								visualElement.TransformAlignedBoundsToParentSpace(ref localBoundsPicking3D);
								bounds5.Encapsulate(localBoundsPicking3D);
							}
						}
					}
					WorldSpaceData worldSpaceData = WorldSpaceDataStore.GetWorldSpaceData(this);
					worldSpaceData.localBounds3D = bounds4;
					worldSpaceData.localBoundsPicking3D = bounds5;
					worldSpaceData.localBoundsWithoutNested3D = bounds3;
					WorldSpaceDataStore.SetWorldSpaceData(this, worldSpaceData);
				}
			}
		}

		[CreateProperty(ReadOnly = true)]
		public Rect worldBound
		{
			get
			{
				Rect rect = this.rect;
				VisualElement.TransformAlignedRect(this.worldTransformRef, ref rect);
				return rect;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public Rect localBound
		{
			get
			{
				Rect rect = this.rect;
				this.TransformAlignedRectToParentSpace(ref rect);
				return rect;
			}
		}

		internal Rect rect
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				Rect layout = this.layout;
				return new Rect(0f, 0f, layout.width, layout.height);
			}
		}

		internal bool isWorldSpaceRootUIDocument
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.m_Flags & VisualElementFlags.IsWorldSpaceRootUIDocument) == VisualElementFlags.IsWorldSpaceRootUIDocument;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.IsWorldSpaceRootUIDocument) : (this.m_Flags & ~VisualElementFlags.IsWorldSpaceRootUIDocument));
			}
		}

		internal bool isWorldTransformDirty
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.WorldTransformDirty) == VisualElementFlags.WorldTransformDirty;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.WorldTransformDirty) : (this.m_Flags & ~VisualElementFlags.WorldTransformDirty));
			}
		}

		internal bool isWorldTransformInverseDirty
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.WorldTransformInverseDirty) == VisualElementFlags.WorldTransformInverseDirty;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.WorldTransformInverseDirty) : (this.m_Flags & ~VisualElementFlags.WorldTransformInverseDirty));
			}
		}

		internal bool isWorldTransformInverseOrDependenciesDirty
		{
			get
			{
				return (this.m_Flags & (VisualElementFlags.WorldTransformDirty | VisualElementFlags.WorldTransformInverseDirty)) > (VisualElementFlags)0;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public Matrix4x4 worldTransform
		{
			get
			{
				bool isWorldTransformDirty = this.isWorldTransformDirty;
				if (isWorldTransformDirty)
				{
					this.UpdateWorldTransform();
				}
				return this.m_WorldTransformCache;
			}
		}

		internal ref Matrix4x4 worldTransformRef
		{
			get
			{
				bool isWorldTransformDirty = this.isWorldTransformDirty;
				if (isWorldTransformDirty)
				{
					this.UpdateWorldTransform();
				}
				return ref this.m_WorldTransformCache;
			}
		}

		internal ref Matrix4x4 worldTransformInverse
		{
			get
			{
				bool isWorldTransformInverseOrDependenciesDirty = this.isWorldTransformInverseOrDependenciesDirty;
				if (isWorldTransformInverseOrDependenciesDirty)
				{
					this.UpdateWorldTransformInverse();
				}
				return ref this.m_WorldTransformInverseCache;
			}
		}

		internal void UpdateWorldTransform()
		{
			bool flag = this.elementPanel != null && !this.elementPanel.duringLayoutPhase;
			if (flag)
			{
				this.isWorldTransformDirty = false;
			}
			bool flag2 = this.hierarchy.parent != null;
			if (flag2)
			{
				bool hasDefaultRotationAndScale = this.hasDefaultRotationAndScale;
				if (hasDefaultRotationAndScale)
				{
					VisualElement.TranslateMatrix34(this.hierarchy.parent.worldTransformRef, this.positionWithLayout, out this.m_WorldTransformCache);
				}
				else
				{
					Matrix4x4 matrix4x;
					this.GetPivotedMatrixWithLayout(out matrix4x);
					VisualElement.MultiplyMatrix34(this.hierarchy.parent.worldTransformRef, ref matrix4x, out this.m_WorldTransformCache);
				}
			}
			else
			{
				this.GetPivotedMatrixWithLayout(out this.m_WorldTransformCache);
			}
			this.isWorldTransformInverseDirty = true;
			this.isWorldBoundingBoxDirty = true;
		}

		internal void UpdateWorldTransformInverse()
		{
			Matrix4x4.Inverse3DAffine(this.worldTransform, ref this.m_WorldTransformInverseCache);
			this.isWorldTransformInverseDirty = false;
		}

		internal bool isWorldClipDirty
		{
			get
			{
				return (this.flags & VisualElementFlags.WorldClipDirty) == VisualElementFlags.WorldClipDirty;
			}
		}

		internal Rect worldClip
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				RenderData renderData = this.renderData;
				return (renderData != null) ? renderData.clippingRect : Rect.zero;
			}
		}

		internal Rect nestedTreeWorldClip
		{
			get
			{
				RenderData renderData = this.nestedRenderData;
				return (renderData != null) ? renderData.clippingRect : Rect.zero;
			}
		}

		internal void EnsureWorldTransformAndClipUpToDate()
		{
			bool flag = this.renderData == null;
			if (!flag)
			{
				bool isWorldTransformDirty = this.isWorldTransformDirty;
				if (isWorldTransformDirty)
				{
					this.UpdateWorldTransform();
				}
				this.renderData.UpdateClippingRect();
				this.renderData.flags &= ~RenderDataFlags.IsClippingRectDirty;
			}
		}

		internal static Rect ComputeAAAlignedBound(Rect position, Matrix4x4 mat)
		{
			Rect rect = position;
			Vector3 vector = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y, 0f));
			Vector3 vector2 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y, 0f));
			Vector3 vector3 = mat.MultiplyPoint3x4(new Vector3(rect.x, rect.y + rect.height, 0f));
			Vector3 vector4 = mat.MultiplyPoint3x4(new Vector3(rect.x + rect.width, rect.y + rect.height, 0f));
			return Rect.MinMaxRect(Mathf.Min(vector.x, Mathf.Min(vector2.x, Mathf.Min(vector3.x, vector4.x))), Mathf.Min(vector.y, Mathf.Min(vector2.y, Mathf.Min(vector3.y, vector4.y))), Mathf.Max(vector.x, Mathf.Max(vector2.x, Mathf.Max(vector3.x, vector4.x))), Mathf.Max(vector.y, Mathf.Max(vector2.y, Mathf.Max(vector3.y, vector4.y))));
		}

		internal bool receivesHierarchyGeometryChangedEvents
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.ReceivesHierarchyGeometryChangedEvents) == VisualElementFlags.ReceivesHierarchyGeometryChangedEvents;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.ReceivesHierarchyGeometryChangedEvents) : (this.m_Flags & ~VisualElementFlags.ReceivesHierarchyGeometryChangedEvents));
			}
		}

		internal bool boundingBoxDirtiedSinceLastLayoutPass
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass) == VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass) : (this.m_Flags & ~VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass));
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal PseudoStates pseudoStates
		{
			get
			{
				return this.m_PseudoStates;
			}
			set
			{
				PseudoStates pseudoStates = this.m_PseudoStates ^ value;
				this.m_PseudoStates = value;
				bool flag = pseudoStates > PseudoStates.None;
				if (flag)
				{
					bool flag2 = pseudoStates != PseudoStates.Root;
					if (flag2)
					{
						PseudoStates pseudoStates2 = pseudoStates & value;
						PseudoStates pseudoStates3 = pseudoStates ^ pseudoStates2;
						bool flag3 = (this.triggerPseudoMask & pseudoStates2) != PseudoStates.None || (this.dependencyPseudoMask & pseudoStates3) > PseudoStates.None;
						if (flag3)
						{
							this.IncrementVersion(VersionChangeType.StyleSheet);
						}
					}
				}
			}
		}

		public bool hasActivePseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Active) > PseudoStates.None;
			}
		}

		public bool hasInactivePseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Active) == PseudoStates.None;
			}
		}

		public bool hasHoverPseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Hover) > PseudoStates.None;
			}
		}

		public bool hasCheckedPseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Checked) > PseudoStates.None;
			}
		}

		public bool hasEnabledPseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Disabled) == PseudoStates.None;
			}
		}

		public bool hasDisabledPseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Disabled) > PseudoStates.None;
			}
		}

		public bool hasFocusPseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Focus) > PseudoStates.None;
			}
		}

		public bool hasRootPseudoState
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Root) > PseudoStates.None;
			}
		}

		public void SetActivePseudoState(bool value)
		{
			this.pseudoStates = (value ? (this.pseudoStates | PseudoStates.Active) : (this.pseudoStates & ~PseudoStates.Active));
		}

		public void SetCheckedPseudoState(bool value)
		{
			this.pseudoStates = (value ? (this.pseudoStates | PseudoStates.Checked) : (this.pseudoStates & ~PseudoStates.Checked));
		}

		internal int containedPointerIds { get; set; }

		internal void UpdateHoverPseudoState()
		{
			bool flag = this.containedPointerIds == 0 || this.panel == null;
			if (flag)
			{
				this.pseudoStates &= ~PseudoStates.Hover;
			}
			else
			{
				bool flag2 = false;
				for (int i = 0; i < PointerId.maxPointers; i++)
				{
					bool flag3 = (this.containedPointerIds & (1 << i)) != 0;
					if (flag3)
					{
						IEventHandler capturingElement = this.panel.GetCapturingElement(i);
						bool flag4 = VisualElement.IsPartOfCapturedChain(this, in capturingElement);
						if (flag4)
						{
							flag2 = true;
							break;
						}
					}
				}
				bool flag5 = flag2;
				if (flag5)
				{
					this.pseudoStates |= PseudoStates.Hover;
				}
				else
				{
					this.pseudoStates &= ~PseudoStates.Hover;
				}
			}
		}

		private static bool IsPartOfCapturedChain(VisualElement self, in IEventHandler capturingElement)
		{
			bool flag = self == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = capturingElement == null;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = capturingElement == self;
					flag2 = flag4 || self.Contains(capturingElement as VisualElement);
				}
			}
			return flag2;
		}

		internal void UpdateHoverPseudoStateAfterCaptureChange(int pointerId)
		{
			for (VisualElement visualElement = this; visualElement != null; visualElement = visualElement.parent)
			{
				visualElement.UpdateHoverPseudoState();
			}
			BaseVisualElementPanel elementPanel = this.elementPanel;
			VisualElement visualElement2 = ((elementPanel != null) ? elementPanel.GetTopElementUnderPointer(pointerId) : null);
			VisualElement visualElement3 = visualElement2;
			while (visualElement3 != null && visualElement3 != this)
			{
				visualElement3.UpdateHoverPseudoState();
				visualElement3 = visualElement3.parent;
			}
		}

		internal void UpdatePointerCaptureFlag()
		{
			bool flag = false;
			for (int i = 0; i < PointerId.maxPointers; i++)
			{
				bool flag2 = this.HasPointerCapture(i);
				if (flag2)
				{
					flag = true;
					break;
				}
			}
			this.hasOneOrMorePointerCaptures = flag;
		}

		[CreateProperty]
		public PickingMode pickingMode
		{
			get
			{
				return this.m_PickingMode;
			}
			set
			{
				bool flag = this.m_PickingMode == value;
				if (!flag)
				{
					this.m_PickingMode = value;
					this.IncrementVersion(VersionChangeType.Picking);
					base.NotifyPropertyChanged(in VisualElement.pickingModeProperty);
				}
			}
		}

		[CreateProperty]
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				bool flag = this.m_Name == value;
				if (!flag)
				{
					this.m_Name = value;
					this.IncrementVersion(VersionChangeType.StyleSheet);
					base.NotifyPropertyChanged(in VisualElement.nameProperty);
				}
			}
		}

		internal List<string> classList
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				bool flag = this.m_ClassList == VisualElement.s_EmptyClassList;
				if (flag)
				{
					this.m_ClassList = ObjectListPool<string>.Get();
				}
				return this.m_ClassList;
			}
		}

		internal string fullTypeName
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.typeData.fullTypeName;
			}
		}

		internal string typeName
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
			get
			{
				return this.typeData.typeName;
			}
		}

		internal ref LayoutNode layoutNode
		{
			get
			{
				return ref this.m_LayoutNode;
			}
		}

		internal ref ComputedStyle computedStyle
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return ref this.m_Style;
			}
		}

		internal bool hasInlineStyle
		{
			get
			{
				return this.inlineStyleAccess != null;
			}
		}

		internal bool styleInitialized
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.StyleInitialized) == VisualElementFlags.StyleInitialized;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.StyleInitialized) : (this.m_Flags & ~VisualElementFlags.StyleInitialized));
			}
		}

		internal float opacity
		{
			get
			{
				return this.resolvedStyle.opacity;
			}
			set
			{
				this.style.opacity = value;
			}
		}

		private void ChangeIMGUIContainerCount(int delta)
		{
			for (VisualElement visualElement = this; visualElement != null; visualElement = visualElement.hierarchy.parent)
			{
				visualElement.imguiContainerDescendantCount += delta;
			}
		}

		[DynamicDependency("InitializeUIElementsManaged", typeof(UIElementsInitialization))]
		public unsafe VisualElement()
		{
			this.m_Children = VisualElement.s_EmptyList;
			this.controlid = (VisualElement.s_NextId += 1U);
			this.hierarchy = new VisualElement.Hierarchy(this);
			this.m_ClassList = VisualElement.s_EmptyClassList;
			this.flags = VisualElementFlags.Init;
			this.enabledSelf = true;
			this.focusable = false;
			this.name = string.Empty;
			*this.layoutNode = LayoutManager.SharedManager.CreateNode();
			this.renderHints = RenderHints.None;
			int num;
			int num2;
			int num3;
			int num4;
			EventInterestReflectionUtils.GetDefaultEventInterests(base.GetType(), out num, out num2, out num3, out num4);
			this.m_TrickleDownHandleEventCategories = num3;
			this.m_BubbleUpHandleEventCategories = num4 | num2 | num;
			this.UpdateEventInterestSelfCategories();
		}

		protected override void Finalize()
		{
			try
			{
				LayoutManager.SharedManager.EnqueueNodeForRecycling(ref this.m_LayoutNode);
				VisualElement.s_FinalizerCount++;
			}
			catch (Exception ex)
			{
				Debug.LogError("An exception occured in a VisualElement finalizer, please report a bug.");
				Debug.LogException(ex);
			}
			finally
			{
				base.Finalize();
			}
		}

		internal void SetTooltip(TooltipEvent e)
		{
			VisualElement visualElement = e.currentTarget as VisualElement;
			bool flag = visualElement != null && !string.IsNullOrEmpty(visualElement.tooltip);
			if (flag)
			{
				bool flag2 = e.rect != Rect.zero;
				if (flag2)
				{
					e.rect = e.rect;
				}
				else
				{
					Rect worldBound = visualElement.worldBound;
					Rect worldClip = visualElement.worldClip;
					e.rect = new Rect(worldBound.x, worldBound.y, Math.Clamp(worldBound.width, 0f, worldClip.width), Mathf.Clamp(worldBound.height, 0f, worldClip.height));
				}
				e.tooltip = visualElement.tooltip;
				e.StopImmediatePropagation();
			}
		}

		public sealed override void Focus()
		{
			bool flag = !this.canGrabFocus && this.hierarchy.parent != null;
			if (flag)
			{
				this.hierarchy.parent.Focus();
			}
			else
			{
				base.Focus();
			}
		}

		internal long TimeSinceStartupMs()
		{
			bool flag = this.elementPanel != null;
			long num;
			if (flag)
			{
				num = this.elementPanel.TimeSinceStartupMs();
			}
			else
			{
				num = (long)(BaseVisualElementPanel.DefaultTimeSinceStartup() * 1000.0);
			}
			return num;
		}

		internal void SetPanel(BaseVisualElementPanel p)
		{
			bool flag = this.panel == p;
			if (!flag)
			{
				List<VisualElement> list = VisualElementListPool.Get(0);
				try
				{
					list.Add(this);
					this.GatherAllChildren(list);
					EventDispatcherGate? eventDispatcherGate = null;
					bool flag2 = ((p != null) ? p.dispatcher : null) != null;
					if (flag2)
					{
						eventDispatcherGate = new EventDispatcherGate?(new EventDispatcherGate(p.dispatcher));
					}
					EventDispatcherGate? eventDispatcherGate2 = null;
					IPanel panel = this.panel;
					bool flag3 = ((panel != null) ? panel.dispatcher : null) != null && this.panel.dispatcher != ((p != null) ? p.dispatcher : null);
					if (flag3)
					{
						eventDispatcherGate2 = new EventDispatcherGate?(new EventDispatcherGate(this.panel.dispatcher));
					}
					BaseVisualElementPanel elementPanel = this.elementPanel;
					uint num = ((elementPanel != null) ? elementPanel.hierarchyVersion : 0U);
					EventDispatcherGate? eventDispatcherGate3 = eventDispatcherGate;
					try
					{
						EventDispatcherGate? eventDispatcherGate4 = eventDispatcherGate2;
						try
						{
							IPanel panel2 = this.panel;
							if (panel2 != null)
							{
								EventDispatcher dispatcher = panel2.dispatcher;
								if (dispatcher != null)
								{
									dispatcher.m_ClickDetector.Cleanup(list);
								}
							}
							foreach (VisualElement visualElement in list)
							{
								visualElement.WillChangePanel(p);
							}
							uint num2 = ((elementPanel != null) ? elementPanel.hierarchyVersion : 0U);
							bool flag4 = num != num2;
							if (flag4)
							{
								list.Clear();
								list.Add(this);
								this.GatherAllChildren(list);
							}
							VisualElementFlags visualElementFlags = ((p != null) ? VisualElementFlags.NeedsAttachToPanelEvent : ((VisualElementFlags)0));
							this.InvokeHierarchyChanged(HierarchyChangeType.DetachedFromPanel, list);
							foreach (VisualElement visualElement2 in list)
							{
								visualElement2.elementPanel = p;
								visualElement2.flags |= visualElementFlags;
								visualElement2.m_CachedNextParentWithEventInterests = null;
							}
							this.InvokeHierarchyChanged(HierarchyChangeType.AttachedToPanel, list);
							foreach (VisualElement visualElement3 in list)
							{
								visualElement3.HasChangedPanel(elementPanel);
							}
						}
						finally
						{
							if (eventDispatcherGate4 != null)
							{
								((IDisposable)eventDispatcherGate4.GetValueOrDefault()).Dispose();
							}
						}
					}
					finally
					{
						if (eventDispatcherGate3 != null)
						{
							((IDisposable)eventDispatcherGate3.GetValueOrDefault()).Dispose();
						}
					}
				}
				finally
				{
					VisualElementListPool.Release(list);
				}
			}
		}

		private void WillChangePanel(BaseVisualElementPanel destinationPanel)
		{
			bool flag = this.elementPanel != null;
			if (flag)
			{
				this.UnregisterRunningAnimations();
				this.CreateBindingRequests();
				this.DetachDataSource();
				bool flag2 = this.containedPointerIds != 0;
				if (flag2)
				{
					this.elementPanel.RemoveElementFromPointerCache(this);
					this.elementPanel.CommitElementUnderPointers();
				}
				bool hasOneOrMorePointerCaptures = this.hasOneOrMorePointerCaptures;
				if (hasOneOrMorePointerCaptures)
				{
					for (int i = 0; i < PointerId.maxPointers; i++)
					{
						bool flag3 = this.HasPointerCapture(i);
						if (flag3)
						{
							this.ReleasePointer(i);
							this.elementPanel.ProcessPointerCapture(i);
						}
					}
				}
				bool flag4 = (this.m_Flags & VisualElementFlags.NeedsAttachToPanelEvent) == (VisualElementFlags)0;
				if (flag4)
				{
					bool flag5 = this.HasSelfEventInterests(EventBase<DetachFromPanelEvent>.EventCategory);
					if (flag5)
					{
						using (DetachFromPanelEvent pooled = PanelChangedEventBase<DetachFromPanelEvent>.GetPooled(this.elementPanel, destinationPanel))
						{
							EventDispatchUtilities.SendEventDirectlyToTarget(pooled, this.elementPanel, this);
						}
					}
				}
				this.UnregisterRunningAnimations();
			}
		}

		private void HasChangedPanel(BaseVisualElementPanel prevPanel)
		{
			bool flag = this.elementPanel != null;
			if (flag)
			{
				this.layoutNode.Config = this.elementPanel.layoutConfig;
				this.layoutNode.SoftReset();
				this.RegisterRunningAnimations();
				this.ProcessBindingRequests();
				this.AttachDataSource();
				this.pseudoStates &= ~(PseudoStates.Active | PseudoStates.Hover);
				bool flag2 = (this.pseudoStates & PseudoStates.Focus) > PseudoStates.None;
				if (flag2)
				{
					bool flag3 = !this.focusController.IsFocused(this);
					if (flag3)
					{
						this.pseudoStates &= ~PseudoStates.Focus;
					}
				}
				this.m_Flags &= ~VisualElementFlags.HierarchyDisplayed;
				bool flag4 = (this.m_Flags & VisualElementFlags.NeedsAttachToPanelEvent) == VisualElementFlags.NeedsAttachToPanelEvent;
				if (flag4)
				{
					bool flag5 = this.HasSelfEventInterests(EventBase<AttachToPanelEvent>.EventCategory);
					if (flag5)
					{
						using (AttachToPanelEvent pooled = PanelChangedEventBase<AttachToPanelEvent>.GetPooled(prevPanel, this.elementPanel))
						{
							EventDispatchUtilities.SendEventDirectlyToTarget(pooled, this.elementPanel, this);
						}
					}
					this.m_Flags &= ~VisualElementFlags.NeedsAttachToPanelEvent;
				}
			}
			else
			{
				this.layoutNode.Config = LayoutManager.SharedManager.GetDefaultConfig();
				this.layoutNode.Cache.ClearCachedMeasurements();
			}
			this.styleInitialized = false;
			this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.StyleSheet | VersionChangeType.Transform);
			bool flag6 = !string.IsNullOrEmpty(this.viewDataKey);
			if (flag6)
			{
				this.IncrementVersion(VersionChangeType.ViewData);
			}
		}

		public sealed override void SendEvent(EventBase e)
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			if (elementPanel != null)
			{
				elementPanel.SendEvent(e, DispatchMode.Default);
			}
		}

		internal sealed override void SendEvent(EventBase e, DispatchMode dispatchMode)
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			if (elementPanel != null)
			{
				elementPanel.SendEvent(e, dispatchMode);
			}
		}

		internal sealed override void HandleEvent(EventBase e)
		{
			EventDispatchUtilities.HandleEvent(e, this);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void IncrementVersion(VersionChangeType changeType)
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			if (elementPanel != null)
			{
				elementPanel.OnVersionChanged(this, changeType);
			}
		}

		internal void InvokeHierarchyChanged(HierarchyChangeType changeType, IReadOnlyList<VisualElement> additionalContext = null)
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			if (elementPanel != null)
			{
				elementPanel.InvokeHierarchyChanged(this, changeType, additionalContext);
			}
		}

		[Obsolete("SetEnabledFromHierarchy is deprecated and will be removed in a future release. Please use SetEnabled instead.")]
		protected internal bool SetEnabledFromHierarchy(bool state)
		{
			return this.SetEnabledFromHierarchyPrivate(state);
		}

		private bool SetEnabledFromHierarchyPrivate(bool state)
		{
			bool enabledInHierarchy = this.enabledInHierarchy;
			bool flag = false;
			if (state)
			{
				bool isParentEnabledInHierarchy = this.isParentEnabledInHierarchy;
				if (isParentEnabledInHierarchy)
				{
					bool enabledSelf = this.enabledSelf;
					if (enabledSelf)
					{
						this.RemoveFromClassList(VisualElement.disabledUssClassName);
					}
					else
					{
						flag = true;
						this.AddToClassList(VisualElement.disabledUssClassName);
					}
				}
				else
				{
					flag = true;
					this.RemoveFromClassList(VisualElement.disabledUssClassName);
				}
			}
			else
			{
				flag = true;
				this.EnableInClassList(VisualElement.disabledUssClassName, this.isParentEnabledInHierarchy);
			}
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = this.focusController != null && this.focusController.IsFocused(this);
				if (flag3)
				{
					EventDispatcherGate? eventDispatcherGate = null;
					IPanel panel = this.panel;
					bool flag4 = ((panel != null) ? panel.dispatcher : null) != null;
					if (flag4)
					{
						eventDispatcherGate = new EventDispatcherGate?(new EventDispatcherGate(this.panel.dispatcher));
					}
					EventDispatcherGate? eventDispatcherGate2 = eventDispatcherGate;
					try
					{
						base.BlurImmediately();
					}
					finally
					{
						if (eventDispatcherGate2 != null)
						{
							((IDisposable)eventDispatcherGate2.GetValueOrDefault()).Dispose();
						}
					}
				}
				this.pseudoStates |= PseudoStates.Disabled;
			}
			else
			{
				this.pseudoStates &= ~PseudoStates.Disabled;
			}
			return enabledInHierarchy != this.enabledInHierarchy;
		}

		private bool isParentEnabledInHierarchy
		{
			get
			{
				return this.hierarchy.parent == null || this.hierarchy.parent.enabledInHierarchy;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public bool enabledInHierarchy
		{
			get
			{
				return (this.pseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;
			}
		}

		[CreateProperty]
		public bool enabledSelf
		{
			get
			{
				return this.m_EnabledSelf;
			}
			set
			{
				bool flag = this.m_EnabledSelf == value;
				if (!flag)
				{
					this.m_EnabledSelf = value;
					base.NotifyPropertyChanged(in VisualElement.enabledSelfProperty);
					this.PropagateEnabledToChildren(value);
				}
			}
		}

		public void SetEnabled(bool value)
		{
			this.enabledSelf = value;
		}

		private void PropagateEnabledToChildren(bool value)
		{
			bool flag = this.SetEnabledFromHierarchyPrivate(value);
			if (flag)
			{
				int count = this.m_Children.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_Children[i].PropagateEnabledToChildren(value);
				}
			}
		}

		[CreateProperty]
		public LanguageDirection languageDirection
		{
			get
			{
				return this.m_LanguageDirection;
			}
			set
			{
				bool flag = this.m_LanguageDirection == value;
				if (!flag)
				{
					this.m_LanguageDirection = value;
					this.localLanguageDirection = this.m_LanguageDirection;
					base.NotifyPropertyChanged(in VisualElement.languageDirectionProperty);
				}
			}
		}

		internal LanguageDirection localLanguageDirection
		{
			get
			{
				return this.m_LocalLanguageDirection;
			}
			set
			{
				bool flag = this.m_LocalLanguageDirection == value;
				if (!flag)
				{
					this.m_LocalLanguageDirection = value;
					this.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
					int count = this.m_Children.Count;
					for (int i = 0; i < count; i++)
					{
						bool flag2 = this.m_Children[i].languageDirection == LanguageDirection.Inherit;
						if (flag2)
						{
							this.m_Children[i].localLanguageDirection = this.m_LocalLanguageDirection;
						}
					}
				}
			}
		}

		[CreateProperty]
		public bool visible
		{
			get
			{
				return this.resolvedStyle.visibility == Visibility.Visible;
			}
			set
			{
				bool visible = this.visible;
				this.style.visibility = (value ? Visibility.Visible : Visibility.Hidden);
				bool flag = visible != this.visible;
				if (flag)
				{
					base.NotifyPropertyChanged(in VisualElement.visibleProperty);
				}
			}
		}

		public void MarkDirtyRepaint()
		{
			this.IncrementVersion(VersionChangeType.Repaint);
		}

		public bool IsMarkedForRepaint()
		{
			bool flag = this.renderData == null;
			return flag || (this.renderData.dirtiedValues & RenderDataDirtyTypes.Visuals) == RenderDataDirtyTypes.Visuals;
		}

		public Action<MeshGenerationContext> generateVisualContent { get; set; }

		internal void InvokeGenerateVisualContent(MeshGenerationContext mgc)
		{
			bool flag = this.generateVisualContent != null;
			if (flag)
			{
				try
				{
					using (VisualElement.k_GenerateVisualContentMarker.Auto())
					{
						this.generateVisualContent(mgc);
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		internal void GetFullHierarchicalViewDataKey(StringBuilder key)
		{
			bool flag = this.parent != null;
			if (flag)
			{
				this.parent.GetFullHierarchicalViewDataKey(key);
			}
			bool flag2 = !string.IsNullOrEmpty(this.viewDataKey);
			if (flag2)
			{
				key.Append("__");
				key.Append(this.viewDataKey);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal string GetFullHierarchicalViewDataKey()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.GetFullHierarchicalViewDataKey(stringBuilder);
			return stringBuilder.ToString();
		}

		internal T GetOrCreateViewData<T>(object existing, string key) where T : class, new()
		{
			Debug.Assert(this.elementPanel != null, "VisualElement.elementPanel is null! Cannot load persistent data.");
			ISerializableJsonDictionary serializableJsonDictionary = ((this.elementPanel == null || this.elementPanel.getViewDataDictionary == null) ? null : this.elementPanel.getViewDataDictionary());
			bool flag = serializableJsonDictionary == null || string.IsNullOrEmpty(this.viewDataKey) || !this.enableViewDataPersistence;
			T t;
			if (flag)
			{
				bool flag2 = existing != null;
				if (flag2)
				{
					t = existing as T;
				}
				else
				{
					t = new T();
				}
			}
			else
			{
				string text = "__";
				Type typeFromHandle = typeof(T);
				string text2 = key + text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
				bool flag3 = !serializableJsonDictionary.ContainsKey(text2);
				if (flag3)
				{
					serializableJsonDictionary.Set<T>(text2, new T());
				}
				t = serializableJsonDictionary.Get<T>(text2);
			}
			return t;
		}

		internal T GetOrCreateViewData<T>(ScriptableObject existing, string key) where T : ScriptableObject
		{
			Debug.Assert(this.elementPanel != null, "VisualElement.elementPanel is null! Cannot load view data.");
			ISerializableJsonDictionary serializableJsonDictionary = ((this.elementPanel == null || this.elementPanel.getViewDataDictionary == null) ? null : this.elementPanel.getViewDataDictionary());
			bool flag = serializableJsonDictionary == null || string.IsNullOrEmpty(this.viewDataKey) || !this.enableViewDataPersistence;
			T t;
			if (flag)
			{
				bool flag2 = existing != null;
				if (flag2)
				{
					t = existing as T;
				}
				else
				{
					t = ScriptableObject.CreateInstance<T>();
				}
			}
			else
			{
				string text = "__";
				Type typeFromHandle = typeof(T);
				string text2 = key + text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null);
				bool flag3 = !serializableJsonDictionary.ContainsKey(text2);
				if (flag3)
				{
					serializableJsonDictionary.Set<T>(text2, ScriptableObject.CreateInstance<T>());
				}
				t = serializableJsonDictionary.GetScriptable<T>(text2);
			}
			return t;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void OverwriteFromViewData(object obj, string key)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			Debug.Assert(this.elementPanel != null, "VisualElement.elementPanel is null! Cannot load view data.");
			ISerializableJsonDictionary serializableJsonDictionary = ((this.elementPanel == null || this.elementPanel.getViewDataDictionary == null) ? null : this.elementPanel.getViewDataDictionary());
			bool flag2 = serializableJsonDictionary == null || string.IsNullOrEmpty(this.viewDataKey) || !this.enableViewDataPersistence;
			if (!flag2)
			{
				string text = "__";
				Type type = obj.GetType();
				string text2 = key + text + ((type != null) ? type.ToString() : null);
				bool flag3 = !serializableJsonDictionary.ContainsKey(text2);
				if (flag3)
				{
					serializableJsonDictionary.Set<object>(text2, obj);
				}
				else
				{
					serializableJsonDictionary.Overwrite(obj, text2);
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void SaveViewData()
		{
			bool flag = this.elementPanel != null && this.elementPanel.saveViewData != null && !string.IsNullOrEmpty(this.viewDataKey) && this.enableViewDataPersistence;
			if (flag)
			{
				this.elementPanel.saveViewData();
			}
		}

		internal bool IsViewDataPersitenceSupportedOnChildren(bool existingState)
		{
			bool flag = existingState;
			bool flag2 = string.IsNullOrEmpty(this.viewDataKey) && this != this.contentContainer;
			if (flag2)
			{
				flag = false;
			}
			bool flag3 = this.parent != null && this == this.parent.contentContainer;
			if (flag3)
			{
				flag = true;
			}
			return flag;
		}

		internal void OnViewDataReady(bool enablePersistence)
		{
			this.enableViewDataPersistence = enablePersistence;
			this.OnViewDataReady();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal virtual void OnViewDataReady()
		{
		}

		public virtual bool ContainsPoint(Vector2 localPoint)
		{
			return this.rect.Contains(localPoint);
		}

		public virtual bool Overlaps(Rect rectangle)
		{
			return this.rect.Overlaps(rectangle, true);
		}

		internal bool requireMeasureFunction
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.RequireMeasureFunction) == VisualElementFlags.RequireMeasureFunction;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.RequireMeasureFunction) : (this.m_Flags & ~VisualElementFlags.RequireMeasureFunction));
				bool flag = value && !this.layoutNode.UsesMeasure;
				if (flag)
				{
					this.AssignMeasureFunction();
				}
				else
				{
					bool flag2 = !value && this.layoutNode.UsesMeasure;
					if (flag2)
					{
						this.RemoveMeasureFunction();
					}
				}
			}
		}

		private void AssignMeasureFunction()
		{
			this.layoutNode.SetOwner(this);
			this.layoutNode.UsesMeasure = true;
		}

		private void RemoveMeasureFunction()
		{
			this.layoutNode.UsesMeasure = false;
			this.layoutNode.SetOwner(null);
		}

		protected internal virtual Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			return new Vector2(float.NaN, float.NaN);
		}

		internal unsafe static void Measure(VisualElement ve, ref LayoutNode node, float width, LayoutMeasureMode widthMode, float height, LayoutMeasureMode heightMode, out LayoutSize result)
		{
			result = default(LayoutSize);
			Debug.Assert(node.Equals(*ve.layoutNode), "LayoutNode instance mismatch");
			Vector2 vector = ve.DoMeasure(width, (VisualElement.MeasureMode)widthMode, height, (VisualElement.MeasureMode)heightMode);
			float scaledPixelsPerPoint = ve.scaledPixelsPerPoint;
			result = new LayoutSize(AlignmentUtils.RoundToPixelGrid(vector.x, scaledPixelsPerPoint, 0.02f), AlignmentUtils.RoundToPixelGrid(vector.y, scaledPixelsPerPoint, 0.02f));
		}

		internal void SetSize(Vector2 size)
		{
			Rect layout = this.layout;
			layout.width = size.x;
			layout.height = size.y;
			this.layout = layout;
		}

		private unsafe void FinalizeLayout()
		{
			this.layoutNode.CopyFromComputedStyle(*this.computedStyle);
		}

		internal void SetInlineRule(StyleSheet sheet, StyleRule rule)
		{
			bool flag = this.inlineStyleAccess == null;
			if (flag)
			{
				this.inlineStyleAccess = new InlineStyleAccess(this);
			}
			this.inlineStyleAccess.SetInlineRule(sheet, rule);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal unsafe void UpdateInlineRule(StyleSheet sheet, StyleRule rule)
		{
			ComputedStyle computedStyle = this.computedStyle.Acquire();
			long matchingRulesHash = this.computedStyle.matchingRulesHash;
			ComputedStyle computedStyle2;
			bool flag = !StyleCache.TryGetValue(matchingRulesHash, out computedStyle2);
			if (flag)
			{
				computedStyle2 = *InitialStyle.Get();
			}
			this.m_Style.CopyFrom(ref computedStyle2);
			this.SetInlineRule(sheet, rule);
			this.FinalizeLayout();
			VersionChangeType versionChangeType = ComputedStyle.CompareChanges(ref computedStyle, this.computedStyle);
			computedStyle.Release();
			this.IncrementVersion(versionChangeType);
		}

		internal void SetComputedStyle(ref ComputedStyle newStyle)
		{
			bool flag = this.m_Style.matchingRulesHash == newStyle.matchingRulesHash;
			if (!flag)
			{
				VersionChangeType versionChangeType = ComputedStyle.CompareChanges(ref this.m_Style, ref newStyle);
				this.m_Style.CopyFrom(ref newStyle);
				this.FinalizeLayout();
				BaseVisualElementPanel elementPanel = this.elementPanel;
				bool flag2 = ((elementPanel != null) ? elementPanel.GetTopElementUnderPointer(PointerId.mousePointerId) : null) == this;
				if (flag2)
				{
					this.elementPanel.cursorManager.SetCursor(this.m_Style.cursor);
				}
				this.IncrementVersion(versionChangeType);
			}
		}

		internal void ResetPositionProperties()
		{
			bool flag = !this.hasInlineStyle;
			if (!flag)
			{
				this.style.position = StyleKeyword.Null;
				this.style.marginLeft = StyleKeyword.Null;
				this.style.marginRight = StyleKeyword.Null;
				this.style.marginBottom = StyleKeyword.Null;
				this.style.marginTop = StyleKeyword.Null;
				this.style.left = StyleKeyword.Null;
				this.style.top = StyleKeyword.Null;
				this.style.right = StyleKeyword.Null;
				this.style.bottom = StyleKeyword.Null;
				this.style.width = StyleKeyword.Null;
				this.style.height = StyleKeyword.Null;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.GetType().Name,
				" ",
				this.name,
				" ",
				this.layout.ToString(),
				" world rect: ",
				this.worldBound.ToString()
			});
		}

		public IEnumerable<string> GetClasses()
		{
			return this.m_ClassList;
		}

		internal List<string> GetClassesForIteration()
		{
			return this.m_ClassList;
		}

		public void ClearClassList()
		{
			bool flag = this.m_ClassList.Count > 0;
			if (flag)
			{
				ObjectListPool<string>.Release(this.m_ClassList);
				this.m_ClassList = VisualElement.s_EmptyClassList;
				this.IncrementVersion(VersionChangeType.StyleSheet);
			}
		}

		public void AddToClassList(string className)
		{
			bool flag = string.IsNullOrEmpty(className);
			if (!flag)
			{
				bool flag2 = this.m_ClassList == VisualElement.s_EmptyClassList;
				if (flag2)
				{
					this.m_ClassList = ObjectListPool<string>.Get();
				}
				else
				{
					bool flag3 = this.m_ClassList.Contains(className);
					if (flag3)
					{
						return;
					}
					bool flag4 = this.m_ClassList.Capacity == this.m_ClassList.Count;
					if (flag4)
					{
						this.m_ClassList.Capacity++;
					}
				}
				this.m_ClassList.Add(className);
				this.IncrementVersion(VersionChangeType.StyleSheet);
			}
		}

		public void RemoveFromClassList(string className)
		{
			bool flag = this.m_ClassList.Remove(className);
			if (flag)
			{
				bool flag2 = this.m_ClassList.Count == 0;
				if (flag2)
				{
					ObjectListPool<string>.Release(this.m_ClassList);
					this.m_ClassList = VisualElement.s_EmptyClassList;
				}
				this.IncrementVersion(VersionChangeType.StyleSheet);
			}
		}

		public void ToggleInClassList(string className)
		{
			bool flag = this.ClassListContains(className);
			if (flag)
			{
				this.RemoveFromClassList(className);
			}
			else
			{
				this.AddToClassList(className);
			}
		}

		public void EnableInClassList(string className, bool enable)
		{
			if (enable)
			{
				this.AddToClassList(className);
			}
			else
			{
				this.RemoveFromClassList(className);
			}
		}

		public bool ClassListContains(string cls)
		{
			for (int i = 0; i < this.m_ClassList.Count; i++)
			{
				bool flag = this.m_ClassList[i].Equals(cls, StringComparison.Ordinal);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public object FindAncestorUserData()
		{
			for (VisualElement visualElement = this.parent; visualElement != null; visualElement = visualElement.parent)
			{
				bool flag = visualElement.userData != null;
				if (flag)
				{
					return visualElement.userData;
				}
			}
			return null;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
		internal object GetProperty(PropertyName key)
		{
			VisualElement.CheckUserKeyArgument(key);
			bool flag = this.m_PropertyBag != null;
			object obj2;
			if (flag)
			{
				object obj;
				this.m_PropertyBag.TryGetValue(key, out obj);
				obj2 = obj;
			}
			else
			{
				obj2 = null;
			}
			return obj2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
		internal void SetProperty(PropertyName key, object value)
		{
			VisualElement.CheckUserKeyArgument(key);
			this.SetPropertyInternal(key, value);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
		internal bool HasProperty(PropertyName key)
		{
			VisualElement.CheckUserKeyArgument(key);
			Dictionary<PropertyName, object> propertyBag = this.m_PropertyBag;
			return propertyBag != null && propertyBag.ContainsKey(key);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal bool ClearProperty(PropertyName key)
		{
			VisualElement.CheckUserKeyArgument(key);
			Dictionary<PropertyName, object> propertyBag = this.m_PropertyBag;
			return propertyBag != null && propertyBag.Remove(key);
		}

		private static void CheckUserKeyArgument(PropertyName key)
		{
			bool flag = PropertyName.IsNullOrEmpty(key);
			if (flag)
			{
				throw new ArgumentNullException("key");
			}
			bool flag2 = key == VisualElement.userDataPropertyKey;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("The {0} key is reserved by the system", VisualElement.userDataPropertyKey));
			}
		}

		private void SetPropertyInternal(PropertyName key, object value)
		{
			if (this.m_PropertyBag == null)
			{
				this.m_PropertyBag = new Dictionary<PropertyName, object>();
			}
			this.m_PropertyBag[key] = value;
		}

		internal void UpdateCursorStyle(long eventType)
		{
			bool flag = this.elementPanel == null;
			if (!flag)
			{
				bool flag2 = eventType == EventBase<MouseCaptureOutEvent>.TypeId();
				if (flag2)
				{
					VisualElement topElementUnderPointer = this.elementPanel.GetTopElementUnderPointer(PointerId.mousePointerId);
					bool flag3 = topElementUnderPointer != null;
					if (flag3)
					{
						this.elementPanel.cursorManager.SetCursor(topElementUnderPointer.computedStyle.cursor);
					}
					else
					{
						this.elementPanel.cursorManager.ResetCursor();
					}
				}
				else
				{
					IEventHandler capturingElement = this.elementPanel.GetCapturingElement(PointerId.mousePointerId);
					bool flag4 = capturingElement != null && capturingElement != this;
					if (!flag4)
					{
						bool flag5 = eventType == EventBase<MouseOverEvent>.TypeId() && this.elementPanel.GetTopElementUnderPointer(PointerId.mousePointerId) == this;
						if (flag5)
						{
							this.elementPanel.cursorManager.SetCursor(this.computedStyle.cursor);
						}
						else
						{
							bool flag6 = eventType == EventBase<MouseOutEvent>.TypeId() && capturingElement == null;
							if (flag6)
							{
								this.elementPanel.cursorManager.ResetCursor();
							}
						}
					}
				}
			}
		}

		private VisualElementAnimationSystem GetAnimationSystem()
		{
			bool flag = this.elementPanel != null;
			VisualElementAnimationSystem visualElementAnimationSystem;
			if (flag)
			{
				visualElementAnimationSystem = this.elementPanel.GetUpdater(VisualTreeUpdatePhase.Animation) as VisualElementAnimationSystem;
			}
			else
			{
				visualElementAnimationSystem = null;
			}
			return visualElementAnimationSystem;
		}

		internal void RegisterAnimation(IValueAnimationUpdate anim)
		{
			bool flag = this.m_RunningAnimations == null;
			if (flag)
			{
				this.m_RunningAnimations = new List<IValueAnimationUpdate>();
			}
			this.m_RunningAnimations.Add(anim);
			VisualElementAnimationSystem animationSystem = this.GetAnimationSystem();
			bool flag2 = animationSystem != null;
			if (flag2)
			{
				animationSystem.RegisterAnimation(anim);
			}
		}

		internal void UnregisterAnimation(IValueAnimationUpdate anim)
		{
			bool flag = this.m_RunningAnimations != null;
			if (flag)
			{
				this.m_RunningAnimations.Remove(anim);
			}
			VisualElementAnimationSystem animationSystem = this.GetAnimationSystem();
			bool flag2 = animationSystem != null;
			if (flag2)
			{
				animationSystem.UnregisterAnimation(anim);
			}
		}

		private void UnregisterRunningAnimations()
		{
			bool flag = this.m_RunningAnimations != null && this.m_RunningAnimations.Count > 0;
			if (flag)
			{
				VisualElementAnimationSystem animationSystem = this.GetAnimationSystem();
				bool flag2 = animationSystem != null;
				if (flag2)
				{
					animationSystem.UnregisterAnimations(this.m_RunningAnimations);
				}
			}
			this.styleAnimation.CancelAllAnimations();
		}

		private void RegisterRunningAnimations()
		{
			bool flag = this.m_RunningAnimations != null && this.m_RunningAnimations.Count > 0;
			if (flag)
			{
				VisualElementAnimationSystem animationSystem = this.GetAnimationSystem();
				bool flag2 = animationSystem != null;
				if (flag2)
				{
					animationSystem.RegisterAnimations(this.m_RunningAnimations);
				}
			}
		}

		ValueAnimation<float> ITransitionAnimations.Start(float from, float to, int durationMs, Action<VisualElement, float> onValueChanged)
		{
			return this.experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
		}

		ValueAnimation<Rect> ITransitionAnimations.Start(Rect from, Rect to, int durationMs, Action<VisualElement, Rect> onValueChanged)
		{
			return this.experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
		}

		ValueAnimation<Color> ITransitionAnimations.Start(Color from, Color to, int durationMs, Action<VisualElement, Color> onValueChanged)
		{
			return this.experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
		}

		ValueAnimation<Vector3> ITransitionAnimations.Start(Vector3 from, Vector3 to, int durationMs, Action<VisualElement, Vector3> onValueChanged)
		{
			return this.experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
		}

		ValueAnimation<Vector2> ITransitionAnimations.Start(Vector2 from, Vector2 to, int durationMs, Action<VisualElement, Vector2> onValueChanged)
		{
			return this.experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
		}

		ValueAnimation<Quaternion> ITransitionAnimations.Start(Quaternion from, Quaternion to, int durationMs, Action<VisualElement, Quaternion> onValueChanged)
		{
			return this.experimental.animation.Start((VisualElement e) => from, to, durationMs, onValueChanged);
		}

		ValueAnimation<StyleValues> ITransitionAnimations.Start(StyleValues from, StyleValues to, int durationMs)
		{
			bool flag = from.m_StyleValues == null;
			if (flag)
			{
				from.Values();
			}
			bool flag2 = to.m_StyleValues == null;
			if (flag2)
			{
				to.Values();
			}
			return this.Start((VisualElement e) => from, to, durationMs);
		}

		ValueAnimation<float> ITransitionAnimations.Start(Func<VisualElement, float> fromValueGetter, float to, int durationMs, Action<VisualElement, float> onValueChanged)
		{
			return VisualElement.StartAnimation<float>(ValueAnimation<float>.Create(this, new Func<float, float, float, float>(Lerp.Interpolate)), fromValueGetter, to, durationMs, onValueChanged);
		}

		ValueAnimation<Rect> ITransitionAnimations.Start(Func<VisualElement, Rect> fromValueGetter, Rect to, int durationMs, Action<VisualElement, Rect> onValueChanged)
		{
			return VisualElement.StartAnimation<Rect>(ValueAnimation<Rect>.Create(this, new Func<Rect, Rect, float, Rect>(Lerp.Interpolate)), fromValueGetter, to, durationMs, onValueChanged);
		}

		ValueAnimation<Color> ITransitionAnimations.Start(Func<VisualElement, Color> fromValueGetter, Color to, int durationMs, Action<VisualElement, Color> onValueChanged)
		{
			return VisualElement.StartAnimation<Color>(ValueAnimation<Color>.Create(this, new Func<Color, Color, float, Color>(Lerp.Interpolate)), fromValueGetter, to, durationMs, onValueChanged);
		}

		ValueAnimation<Vector3> ITransitionAnimations.Start(Func<VisualElement, Vector3> fromValueGetter, Vector3 to, int durationMs, Action<VisualElement, Vector3> onValueChanged)
		{
			return VisualElement.StartAnimation<Vector3>(ValueAnimation<Vector3>.Create(this, new Func<Vector3, Vector3, float, Vector3>(Lerp.Interpolate)), fromValueGetter, to, durationMs, onValueChanged);
		}

		ValueAnimation<Vector2> ITransitionAnimations.Start(Func<VisualElement, Vector2> fromValueGetter, Vector2 to, int durationMs, Action<VisualElement, Vector2> onValueChanged)
		{
			return VisualElement.StartAnimation<Vector2>(ValueAnimation<Vector2>.Create(this, new Func<Vector2, Vector2, float, Vector2>(Lerp.Interpolate)), fromValueGetter, to, durationMs, onValueChanged);
		}

		ValueAnimation<Quaternion> ITransitionAnimations.Start(Func<VisualElement, Quaternion> fromValueGetter, Quaternion to, int durationMs, Action<VisualElement, Quaternion> onValueChanged)
		{
			return VisualElement.StartAnimation<Quaternion>(ValueAnimation<Quaternion>.Create(this, new Func<Quaternion, Quaternion, float, Quaternion>(Lerp.Interpolate)), fromValueGetter, to, durationMs, onValueChanged);
		}

		private static ValueAnimation<T> StartAnimation<T>(ValueAnimation<T> anim, Func<VisualElement, T> fromValueGetter, T to, int durationMs, Action<VisualElement, T> onValueChanged)
		{
			anim.initialValue = fromValueGetter;
			anim.to = to;
			anim.durationMs = durationMs;
			anim.valueUpdated = onValueChanged;
			anim.Start();
			return anim;
		}

		private static void AssignStyleValues(VisualElement ve, StyleValues src)
		{
			IStyle style = ve.style;
			bool flag = src.m_StyleValues != null;
			if (flag)
			{
				foreach (StyleValue styleValue in src.m_StyleValues.m_Values)
				{
					StylePropertyId id = styleValue.id;
					StylePropertyId stylePropertyId = id;
					if (stylePropertyId <= StylePropertyId.Width)
					{
						if (stylePropertyId <= StylePropertyId.Color)
						{
							if (stylePropertyId != StylePropertyId.Unknown)
							{
								if (stylePropertyId == StylePropertyId.Color)
								{
									style.color = styleValue.color;
								}
							}
						}
						else if (stylePropertyId != StylePropertyId.FontSize)
						{
							switch (stylePropertyId)
							{
							case StylePropertyId.BorderBottomWidth:
								style.borderBottomWidth = styleValue.number;
								break;
							case StylePropertyId.BorderLeftWidth:
								style.borderLeftWidth = styleValue.number;
								break;
							case StylePropertyId.BorderRightWidth:
								style.borderRightWidth = styleValue.number;
								break;
							case StylePropertyId.BorderTopWidth:
								style.borderTopWidth = styleValue.number;
								break;
							case StylePropertyId.Bottom:
								style.bottom = styleValue.number;
								break;
							case StylePropertyId.FlexGrow:
								style.flexGrow = styleValue.number;
								break;
							case StylePropertyId.FlexShrink:
								style.flexShrink = styleValue.number;
								break;
							case StylePropertyId.Height:
								style.height = styleValue.number;
								break;
							case StylePropertyId.Left:
								style.left = styleValue.number;
								break;
							case StylePropertyId.MarginBottom:
								style.marginBottom = styleValue.number;
								break;
							case StylePropertyId.MarginLeft:
								style.marginLeft = styleValue.number;
								break;
							case StylePropertyId.MarginRight:
								style.marginRight = styleValue.number;
								break;
							case StylePropertyId.MarginTop:
								style.marginTop = styleValue.number;
								break;
							case StylePropertyId.PaddingBottom:
								style.paddingBottom = styleValue.number;
								break;
							case StylePropertyId.PaddingLeft:
								style.paddingLeft = styleValue.number;
								break;
							case StylePropertyId.PaddingRight:
								style.paddingRight = styleValue.number;
								break;
							case StylePropertyId.PaddingTop:
								style.paddingTop = styleValue.number;
								break;
							case StylePropertyId.Right:
								style.right = styleValue.number;
								break;
							case StylePropertyId.Top:
								style.top = styleValue.number;
								break;
							case StylePropertyId.Width:
								style.width = styleValue.number;
								break;
							}
						}
						else
						{
							style.fontSize = styleValue.number;
						}
					}
					else if (stylePropertyId <= StylePropertyId.BorderColor)
					{
						if (stylePropertyId != StylePropertyId.UnityBackgroundImageTintColor)
						{
							if (stylePropertyId == StylePropertyId.BorderColor)
							{
								style.borderLeftColor = styleValue.color;
								style.borderTopColor = styleValue.color;
								style.borderRightColor = styleValue.color;
								style.borderBottomColor = styleValue.color;
							}
						}
						else
						{
							style.unityBackgroundImageTintColor = styleValue.color;
						}
					}
					else if (stylePropertyId != StylePropertyId.BackgroundColor)
					{
						switch (stylePropertyId)
						{
						case StylePropertyId.BorderBottomLeftRadius:
							style.borderBottomLeftRadius = styleValue.number;
							break;
						case StylePropertyId.BorderBottomRightRadius:
							style.borderBottomRightRadius = styleValue.number;
							break;
						case StylePropertyId.BorderTopLeftRadius:
							style.borderTopLeftRadius = styleValue.number;
							break;
						case StylePropertyId.BorderTopRightRadius:
							style.borderTopRightRadius = styleValue.number;
							break;
						case StylePropertyId.Opacity:
							style.opacity = styleValue.number;
							break;
						}
					}
					else
					{
						style.backgroundColor = styleValue.color;
					}
				}
			}
		}

		private StyleValues ReadCurrentValues(VisualElement ve, StyleValues targetValuesToRead)
		{
			StyleValues styleValues = default(StyleValues);
			IResolvedStyle resolvedStyle = ve.resolvedStyle;
			bool flag = targetValuesToRead.m_StyleValues != null;
			if (flag)
			{
				foreach (StyleValue styleValue in targetValuesToRead.m_StyleValues.m_Values)
				{
					StylePropertyId id = styleValue.id;
					StylePropertyId stylePropertyId = id;
					if (stylePropertyId <= StylePropertyId.Width)
					{
						if (stylePropertyId != StylePropertyId.Unknown)
						{
							if (stylePropertyId != StylePropertyId.Color)
							{
								switch (stylePropertyId)
								{
								case StylePropertyId.BorderBottomWidth:
									styleValues.borderBottomWidth = resolvedStyle.borderBottomWidth;
									break;
								case StylePropertyId.BorderLeftWidth:
									styleValues.borderLeftWidth = resolvedStyle.borderLeftWidth;
									break;
								case StylePropertyId.BorderRightWidth:
									styleValues.borderRightWidth = resolvedStyle.borderRightWidth;
									break;
								case StylePropertyId.BorderTopWidth:
									styleValues.borderTopWidth = resolvedStyle.borderTopWidth;
									break;
								case StylePropertyId.Bottom:
									styleValues.bottom = resolvedStyle.bottom;
									break;
								case StylePropertyId.FlexGrow:
									styleValues.flexGrow = resolvedStyle.flexGrow;
									break;
								case StylePropertyId.FlexShrink:
									styleValues.flexShrink = resolvedStyle.flexShrink;
									break;
								case StylePropertyId.Height:
									styleValues.height = resolvedStyle.height;
									break;
								case StylePropertyId.Left:
									styleValues.left = resolvedStyle.left;
									break;
								case StylePropertyId.MarginBottom:
									styleValues.marginBottom = resolvedStyle.marginBottom;
									break;
								case StylePropertyId.MarginLeft:
									styleValues.marginLeft = resolvedStyle.marginLeft;
									break;
								case StylePropertyId.MarginRight:
									styleValues.marginRight = resolvedStyle.marginRight;
									break;
								case StylePropertyId.MarginTop:
									styleValues.marginTop = resolvedStyle.marginTop;
									break;
								case StylePropertyId.PaddingBottom:
									styleValues.paddingBottom = resolvedStyle.paddingBottom;
									break;
								case StylePropertyId.PaddingLeft:
									styleValues.paddingLeft = resolvedStyle.paddingLeft;
									break;
								case StylePropertyId.PaddingRight:
									styleValues.paddingRight = resolvedStyle.paddingRight;
									break;
								case StylePropertyId.PaddingTop:
									styleValues.paddingTop = resolvedStyle.paddingTop;
									break;
								case StylePropertyId.Right:
									styleValues.right = resolvedStyle.right;
									break;
								case StylePropertyId.Top:
									styleValues.top = resolvedStyle.top;
									break;
								case StylePropertyId.Width:
									styleValues.width = resolvedStyle.width;
									break;
								}
							}
							else
							{
								styleValues.color = resolvedStyle.color;
							}
						}
					}
					else if (stylePropertyId <= StylePropertyId.BorderColor)
					{
						if (stylePropertyId != StylePropertyId.UnityBackgroundImageTintColor)
						{
							if (stylePropertyId == StylePropertyId.BorderColor)
							{
								styleValues.borderColor = resolvedStyle.borderLeftColor;
							}
						}
						else
						{
							styleValues.unityBackgroundImageTintColor = resolvedStyle.unityBackgroundImageTintColor;
						}
					}
					else if (stylePropertyId != StylePropertyId.BackgroundColor)
					{
						switch (stylePropertyId)
						{
						case StylePropertyId.BorderBottomLeftRadius:
							styleValues.borderBottomLeftRadius = resolvedStyle.borderBottomLeftRadius;
							break;
						case StylePropertyId.BorderBottomRightRadius:
							styleValues.borderBottomRightRadius = resolvedStyle.borderBottomRightRadius;
							break;
						case StylePropertyId.BorderTopLeftRadius:
							styleValues.borderTopLeftRadius = resolvedStyle.borderTopLeftRadius;
							break;
						case StylePropertyId.BorderTopRightRadius:
							styleValues.borderTopRightRadius = resolvedStyle.borderTopRightRadius;
							break;
						case StylePropertyId.Opacity:
							styleValues.opacity = resolvedStyle.opacity;
							break;
						}
					}
					else
					{
						styleValues.backgroundColor = resolvedStyle.backgroundColor;
					}
				}
			}
			return styleValues;
		}

		ValueAnimation<StyleValues> ITransitionAnimations.Start(StyleValues to, int durationMs)
		{
			bool flag = to.m_StyleValues == null;
			if (flag)
			{
				to.Values();
			}
			return this.Start((VisualElement e) => this.ReadCurrentValues(e, to), to, durationMs);
		}

		private ValueAnimation<StyleValues> Start(Func<VisualElement, StyleValues> fromValueGetter, StyleValues to, int durationMs)
		{
			return VisualElement.StartAnimation<StyleValues>(ValueAnimation<StyleValues>.Create(this, new Func<StyleValues, StyleValues, float, StyleValues>(Lerp.Interpolate)), fromValueGetter, to, durationMs, new Action<VisualElement, StyleValues>(VisualElement.AssignStyleValues));
		}

		ValueAnimation<Rect> ITransitionAnimations.Layout(Rect to, int durationMs)
		{
			return this.experimental.animation.Start((VisualElement e) => new Rect(e.resolvedStyle.left, e.resolvedStyle.top, e.resolvedStyle.width, e.resolvedStyle.height), to, durationMs, delegate(VisualElement e, Rect c)
			{
				e.style.left = c.x;
				e.style.top = c.y;
				e.style.width = c.width;
				e.style.height = c.height;
			});
		}

		ValueAnimation<Vector2> ITransitionAnimations.TopLeft(Vector2 to, int durationMs)
		{
			return this.experimental.animation.Start((VisualElement e) => new Vector2(e.resolvedStyle.left, e.resolvedStyle.top), to, durationMs, delegate(VisualElement e, Vector2 c)
			{
				e.style.left = c.x;
				e.style.top = c.y;
			});
		}

		ValueAnimation<Vector2> ITransitionAnimations.Size(Vector2 to, int durationMs)
		{
			return this.experimental.animation.Start((VisualElement e) => e.layout.size, to, durationMs, delegate(VisualElement e, Vector2 c)
			{
				e.style.width = c.x;
				e.style.height = c.y;
			});
		}

		ValueAnimation<float> ITransitionAnimations.Scale(float to, int durationMs)
		{
			return this.experimental.animation.Start((VisualElement e) => e.transform.scale.x, to, durationMs, delegate(VisualElement e, float c)
			{
				e.transform.scale = new Vector3(c, c, c);
			});
		}

		ValueAnimation<Vector3> ITransitionAnimations.Position(Vector3 to, int durationMs)
		{
			return this.experimental.animation.Start((VisualElement e) => e.transform.position, to, durationMs, delegate(VisualElement e, Vector3 c)
			{
				e.transform.position = c;
			});
		}

		ValueAnimation<Quaternion> ITransitionAnimations.Rotation(Quaternion to, int durationMs)
		{
			return this.experimental.animation.Start((VisualElement e) => e.transform.rotation, to, durationMs, delegate(VisualElement e, Quaternion c)
			{
				e.transform.rotation = c;
			});
		}

		[CreateProperty]
		public object dataSource
		{
			get
			{
				return this.m_DataSource;
			}
			set
			{
				bool flag = this.m_DataSource == value;
				if (!flag)
				{
					object dataSource = this.m_DataSource;
					this.m_DataSource = value;
					this.TrackSource(dataSource, this.m_DataSource);
					this.IncrementVersion(VersionChangeType.DataSource);
					base.NotifyPropertyChanged(in VisualElement.dataSourceProperty);
				}
			}
		}

		internal Object dataSourceUnityObject
		{
			get
			{
				return this.dataSource as Object;
			}
			set
			{
				this.dataSource = value;
			}
		}

		[CreateProperty]
		public unsafe PropertyPath dataSourcePath
		{
			get
			{
				PathRef dataSourcePath = this.m_DataSourcePath;
				return (dataSourcePath != null) ? (*dataSourcePath.path) : default(PropertyPath);
			}
			set
			{
				bool flag = this.m_DataSourcePath == null && value.IsEmpty;
				if (!flag)
				{
					PathRef pathRef;
					if ((pathRef = this.m_DataSourcePath) == null)
					{
						pathRef = (this.m_DataSourcePath = new PathRef());
					}
					ref PropertyPath path = ref pathRef.path;
					bool flag2 = path == value;
					if (!flag2)
					{
						path = value;
						this.IncrementVersion(VersionChangeType.DataSource);
						base.NotifyPropertyChanged(in VisualElement.dataSourcePathProperty);
					}
				}
			}
		}

		internal bool isDataSourcePathEmpty
		{
			get
			{
				return this.m_DataSourcePath == null || this.m_DataSourcePath.IsEmpty;
			}
		}

		internal string dataSourcePathString
		{
			get
			{
				return this.dataSourcePath.ToString();
			}
			set
			{
				this.dataSourcePath = new PropertyPath(value);
			}
		}

		private List<Binding> bindings
		{
			get
			{
				List<Binding> list;
				if ((list = this.m_Bindings) == null)
				{
					list = (this.m_Bindings = new List<Binding>());
				}
				return list;
			}
			set
			{
				this.m_Bindings = value;
			}
		}

		public Type dataSourceType { get; set; }

		internal string dataSourceTypeString
		{
			get
			{
				return UxmlUtility.TypeToString(this.dataSourceType);
			}
			set
			{
				this.dataSourceType = UxmlUtility.ParseType(value, null);
			}
		}

		public void SetBinding(BindingId bindingId, Binding binding)
		{
			this.RegisterBinding(bindingId, binding);
		}

		public Binding GetBinding(BindingId bindingId)
		{
			Binding binding;
			return this.TryGetBinding(bindingId, out binding) ? binding : null;
		}

		public bool TryGetBinding(BindingId bindingId, out Binding binding)
		{
			BindingInfo bindingInfo;
			bool flag = DataBindingUtility.TryGetBinding(this, in bindingId, out bindingInfo);
			bool flag2;
			if (flag)
			{
				binding = bindingInfo.binding;
				flag2 = true;
			}
			else
			{
				binding = null;
				flag2 = false;
			}
			return flag2;
		}

		public IEnumerable<BindingInfo> GetBindingInfos()
		{
			VisualElement.<GetBindingInfos>d__616 <GetBindingInfos>d__ = new VisualElement.<GetBindingInfos>d__616(-2);
			<GetBindingInfos>d__.<>4__this = this;
			return <GetBindingInfos>d__;
		}

		public void GetBindingInfos(List<BindingInfo> bindingInfos)
		{
			DataBindingUtility.GetBindingsForElement(this, bindingInfos);
		}

		public bool HasBinding(BindingId bindingId)
		{
			Binding binding;
			return this.TryGetBinding(bindingId, out binding);
		}

		public void ClearBinding(BindingId bindingId)
		{
			this.SetBinding(bindingId, null);
			List<Binding> bindings = this.bindings;
			if (bindings != null)
			{
				bindings.RemoveAll(delegate(Binding b)
				{
					BindingId bindingId2 = b.property;
					return (in bindingId2) == (in bindingId);
				});
			}
		}

		public void ClearBindings()
		{
			DataBindingManager.CreateClearAllBindingsRequest(this);
			List<Binding> bindings = this.bindings;
			if (bindings != null)
			{
				bindings.Clear();
			}
			bool flag = this.panel != null;
			if (flag)
			{
				this.ProcessBindingRequests();
			}
		}

		public DataSourceContext GetHierarchicalDataSourceContext()
		{
			VisualElement visualElement = this;
			PropertyPath propertyPath = default(PropertyPath);
			while (visualElement != null)
			{
				bool flag = !visualElement.isDataSourcePathEmpty;
				if (flag)
				{
					PropertyPath dataSourcePath = visualElement.dataSourcePath;
					propertyPath = PropertyPath.Combine(in dataSourcePath, in propertyPath);
				}
				bool flag2 = visualElement.dataSource != null;
				if (flag2)
				{
					object dataSource = visualElement.dataSource;
					return new DataSourceContext(dataSource, in propertyPath);
				}
				visualElement = visualElement.hierarchy.parent;
			}
			return new DataSourceContext(null, in propertyPath);
		}

		public DataSourceContext GetDataSourceContext(BindingId bindingId)
		{
			DataSourceContext dataSourceContext;
			bool flag = this.TryGetDataSourceContext(bindingId, out dataSourceContext);
			if (flag)
			{
				return dataSourceContext;
			}
			throw new ArgumentOutOfRangeException("bindingId", string.Format("[UI Toolkit] could not get binding with id '{0}' on the element.", bindingId));
		}

		public bool TryGetDataSourceContext(BindingId bindingId, out DataSourceContext context)
		{
			Binding binding = this.GetBinding(bindingId);
			Binding binding2 = binding;
			Binding binding3 = binding2;
			bool flag;
			if (binding3 != null)
			{
				IDataSourceProvider dataSourceProvider = binding3 as IDataSourceProvider;
				if (dataSourceProvider != null)
				{
					if (dataSourceProvider.dataSource != null)
					{
						object dataSource = dataSourceProvider.dataSource;
						PropertyPath propertyPath = dataSourceProvider.dataSourcePath;
						context = new DataSourceContext(dataSource, in propertyPath);
						goto IL_00BD;
					}
					bool isEmpty = dataSourceProvider.dataSourcePath.IsEmpty;
					if (!isEmpty)
					{
						IDataSourceProvider dataSourceProvider2 = dataSourceProvider;
						DataSourceContext hierarchicalDataSourceContext = this.GetHierarchicalDataSourceContext();
						object dataSource2 = hierarchicalDataSourceContext.dataSource;
						PropertyPath propertyPath = hierarchicalDataSourceContext.dataSourcePath;
						PropertyPath dataSourcePath = dataSourceProvider2.dataSourcePath;
						PropertyPath propertyPath2 = PropertyPath.Combine(in propertyPath, in dataSourcePath);
						context = new DataSourceContext(dataSource2, in propertyPath2);
						goto IL_00BD;
					}
				}
				context = this.GetHierarchicalDataSourceContext();
				IL_00BD:
				flag = true;
			}
			else
			{
				context = default(DataSourceContext);
				flag = false;
			}
			return flag;
		}

		public bool TryGetLastBindingToUIResult(in BindingId bindingId, out BindingResult result)
		{
			bool flag = this.elementPanel == null;
			bool flag2;
			if (flag)
			{
				result = default(BindingResult);
				flag2 = false;
			}
			else
			{
				DataBindingManager dataBindingManager = this.elementPanel.dataBindingManager;
				DataBindingManager.BindingData bindingData;
				bool flag3 = dataBindingManager.TryGetBindingData(this, in bindingId, out bindingData) && dataBindingManager.TryGetLastUIBindingResult(bindingData, out result);
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					result = default(BindingResult);
					flag2 = false;
				}
			}
			return flag2;
		}

		public bool TryGetLastBindingToSourceResult(in BindingId bindingId, out BindingResult result)
		{
			bool flag = this.elementPanel == null;
			bool flag2;
			if (flag)
			{
				result = default(BindingResult);
				flag2 = false;
			}
			else
			{
				DataBindingManager dataBindingManager = this.elementPanel.dataBindingManager;
				DataBindingManager.BindingData bindingData;
				bool flag3 = dataBindingManager.TryGetBindingData(this, in bindingId, out bindingData) && dataBindingManager.TryGetLastSourceBindingResult(bindingData, out result);
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					result = default(BindingResult);
					flag2 = false;
				}
			}
			return flag2;
		}

		private void RegisterBinding(BindingId bindingId, Binding binding)
		{
			this.AddBindingRequest(bindingId, binding);
			bool flag = this.panel != null;
			if (flag)
			{
				this.ProcessBindingRequests();
			}
		}

		internal void AddBindingRequest(BindingId bindingId, Binding binding)
		{
			DataBindingManager.CreateBindingRequest(this, in bindingId, binding);
		}

		private void ProcessBindingRequests()
		{
			Assert.IsFalse(this.elementPanel == null, null);
			bool flag = DataBindingManager.AnyPendingBindingRequests(this);
			if (flag)
			{
				this.IncrementVersion(VersionChangeType.BindingRegistration);
			}
		}

		private void CreateBindingRequests()
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			Assert.IsFalse(elementPanel == null, null);
			elementPanel.dataBindingManager.TransferBindingRequests(this);
		}

		private void TrackSource(object previous, object current)
		{
			BaseVisualElementPanel elementPanel = this.elementPanel;
			DataBindingManager dataBindingManager = ((elementPanel != null) ? elementPanel.dataBindingManager : null);
			bool flag = dataBindingManager == null;
			if (!flag)
			{
				bool flag2 = (this.m_Flags & VisualElementFlags.DetachedDataSource) == VisualElementFlags.DetachedDataSource;
				if (!flag2)
				{
					BaseVisualElementPanel elementPanel2 = this.elementPanel;
					if (elementPanel2 != null)
					{
						elementPanel2.dataBindingManager.TrackDataSource(previous, current);
					}
				}
			}
		}

		private void DetachDataSource()
		{
			this.TrackSource(this.dataSource, null);
			this.m_Flags |= VisualElementFlags.DetachedDataSource;
		}

		private void AttachDataSource()
		{
			this.m_Flags &= ~VisualElementFlags.DetachedDataSource;
			this.TrackSource(null, this.dataSource);
		}

		private void DirtyNextParentWithEventInterests()
		{
			bool flag = this.m_CachedNextParentWithEventInterests != null && this.m_NextParentCachedVersion == this.m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion;
			if (flag)
			{
				this.m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion = (VisualElement.s_NextParentVersion += 1U);
			}
		}

		internal void SetAsNextParentWithEventInterests()
		{
			bool flag = this.m_NextParentRequiredVersion > 0U;
			if (!flag)
			{
				this.m_NextParentRequiredVersion = (VisualElement.s_NextParentVersion += 1U);
				bool flag2 = this.m_CachedNextParentWithEventInterests != null && this.m_NextParentCachedVersion == this.m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion;
				if (flag2)
				{
					this.m_CachedNextParentWithEventInterests.m_NextParentRequiredVersion = (VisualElement.s_NextParentVersion += 1U);
				}
			}
		}

		internal bool GetCachedNextParentWithEventInterests(out VisualElement nextParent)
		{
			nextParent = this.m_CachedNextParentWithEventInterests;
			return nextParent != null && nextParent.m_NextParentRequiredVersion == this.m_NextParentCachedVersion;
		}

		internal VisualElement nextParentWithEventInterests
		{
			get
			{
				VisualElement visualElement;
				bool cachedNextParentWithEventInterests = this.GetCachedNextParentWithEventInterests(out visualElement);
				VisualElement visualElement2;
				if (cachedNextParentWithEventInterests)
				{
					visualElement2 = visualElement;
				}
				else
				{
					for (VisualElement visualElement3 = this.hierarchy.parent; visualElement3 != null; visualElement3 = visualElement3.hierarchy.parent)
					{
						bool flag = visualElement3.m_NextParentRequiredVersion > 0U;
						if (flag)
						{
							this.PropagateCachedNextParentWithEventInterests(visualElement3, visualElement3);
							return visualElement3;
						}
						VisualElement visualElement4;
						bool cachedNextParentWithEventInterests2 = visualElement3.GetCachedNextParentWithEventInterests(out visualElement4);
						if (cachedNextParentWithEventInterests2)
						{
							this.PropagateCachedNextParentWithEventInterests(visualElement4, visualElement3);
							return visualElement4;
						}
					}
					this.m_CachedNextParentWithEventInterests = null;
					visualElement2 = null;
				}
				return visualElement2;
			}
		}

		private void PropagateCachedNextParentWithEventInterests(VisualElement nextParent, VisualElement stopParent)
		{
			for (VisualElement visualElement = this; visualElement != stopParent; visualElement = visualElement.hierarchy.parent)
			{
				visualElement.m_CachedNextParentWithEventInterests = nextParent;
				visualElement.m_NextParentCachedVersion = nextParent.m_NextParentRequiredVersion;
			}
		}

		internal void AddEventCallbackCategories(int eventCategories, TrickleDown trickleDown)
		{
			bool flag = trickleDown == TrickleDown.TrickleDown;
			if (flag)
			{
				this.m_TrickleDownEventCallbackCategories |= eventCategories;
			}
			else
			{
				this.m_BubbleUpEventCallbackCategories |= eventCategories;
			}
			this.UpdateEventInterestSelfCategories();
		}

		internal void RemoveEventCallbackCategories(int eventCategories, TrickleDown trickleDown)
		{
			bool flag = trickleDown == TrickleDown.TrickleDown;
			if (flag)
			{
				this.m_TrickleDownEventCallbackCategories &= ~eventCategories;
			}
			else
			{
				this.m_BubbleUpEventCallbackCategories &= ~eventCategories;
			}
			this.UpdateEventInterestSelfCategories();
		}

		internal int eventInterestSelfCategories
		{
			get
			{
				return this.m_EventInterestSelfCategories;
			}
		}

		internal int eventInterestParentCategories
		{
			get
			{
				bool flag = this.elementPanel == null;
				int num;
				if (flag)
				{
					num = -1;
				}
				else
				{
					bool isEventInterestParentCategoriesDirty = this.isEventInterestParentCategoriesDirty;
					if (isEventInterestParentCategoriesDirty)
					{
						this.UpdateEventInterestParentCategories();
						this.isEventInterestParentCategoriesDirty = false;
					}
					num = this.m_CachedEventInterestParentCategories;
				}
				return num;
			}
		}

		internal bool isEventInterestParentCategoriesDirty
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.EventInterestParentCategoriesDirty) == VisualElementFlags.EventInterestParentCategoriesDirty;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.EventInterestParentCategoriesDirty) : (this.m_Flags & ~VisualElementFlags.EventInterestParentCategoriesDirty));
			}
		}

		private void UpdateEventInterestSelfCategories()
		{
			int num = this.m_TrickleDownHandleEventCategories | this.m_BubbleUpHandleEventCategories | this.m_TrickleDownEventCallbackCategories | this.m_BubbleUpEventCallbackCategories;
			bool flag = this.m_EventInterestSelfCategories != num;
			if (flag)
			{
				int num2 = this.m_EventInterestSelfCategories ^ num;
				bool flag2 = (num2 & -5537) != 0;
				if (flag2)
				{
					this.SetAsNextParentWithEventInterests();
					this.IncrementVersion(VersionChangeType.EventCallbackCategories);
				}
				else
				{
					this.m_CachedEventInterestParentCategories |= num;
				}
				this.m_EventInterestSelfCategories = num;
			}
		}

		private void UpdateEventInterestParentCategories()
		{
			this.m_CachedEventInterestParentCategories = this.m_EventInterestSelfCategories;
			VisualElement nextParentWithEventInterests = this.nextParentWithEventInterests;
			bool flag = nextParentWithEventInterests == null;
			if (!flag)
			{
				this.m_CachedEventInterestParentCategories |= nextParentWithEventInterests.eventInterestParentCategories;
				bool flag2 = this.hierarchy.parent != null;
				if (flag2)
				{
					for (VisualElement visualElement = this.hierarchy.parent; visualElement != nextParentWithEventInterests; visualElement = visualElement.hierarchy.parent)
					{
						visualElement.m_CachedEventInterestParentCategories = this.m_CachedEventInterestParentCategories;
						visualElement.isEventInterestParentCategoriesDirty = false;
					}
				}
			}
		}

		internal bool HasParentEventInterests(EventCategory eventCategory)
		{
			return (this.eventInterestParentCategories & (1 << (int)eventCategory)) != 0;
		}

		internal bool HasParentEventInterests(int eventCategories)
		{
			return (this.eventInterestParentCategories & eventCategories) != 0;
		}

		internal bool HasSelfEventInterests(EventCategory eventCategory)
		{
			return (this.m_EventInterestSelfCategories & (1 << (int)eventCategory)) != 0;
		}

		internal bool HasSelfEventInterests(int eventCategories)
		{
			return (this.m_EventInterestSelfCategories & eventCategories) != 0;
		}

		internal bool HasTrickleDownEventInterests(int eventCategories)
		{
			return ((this.m_TrickleDownHandleEventCategories | this.m_TrickleDownEventCallbackCategories) & eventCategories) != 0;
		}

		internal bool HasBubbleUpEventInterests(int eventCategories)
		{
			return ((this.m_BubbleUpHandleEventCategories | this.m_BubbleUpEventCallbackCategories) & eventCategories) != 0;
		}

		internal bool HasTrickleDownEventCallbacks(int eventCategories)
		{
			return (this.m_TrickleDownEventCallbackCategories & eventCategories) != 0;
		}

		internal bool HasBubbleUpEventCallbacks(int eventCategories)
		{
			return (this.m_BubbleUpEventCallbackCategories & eventCategories) != 0;
		}

		internal bool HasTrickleDownHandleEvent(EventCategory eventCategory)
		{
			return (this.m_TrickleDownHandleEventCategories & (1 << (int)eventCategory)) != 0;
		}

		internal bool HasTrickleDownHandleEvent(int eventCategories)
		{
			return (this.m_TrickleDownHandleEventCategories & eventCategories) != 0;
		}

		internal bool HasBubbleUpHandleEvent(EventCategory eventCategory)
		{
			return (this.m_BubbleUpHandleEventCategories & (1 << (int)eventCategory)) != 0;
		}

		internal bool HasBubbleUpHandleEvent(int eventCategories)
		{
			return (this.m_BubbleUpHandleEventCategories & eventCategories) != 0;
		}

		public IExperimentalFeatures experimental
		{
			get
			{
				return this;
			}
		}

		ITransitionAnimations IExperimentalFeatures.animation
		{
			get
			{
				return this;
			}
		}

		public VisualElement.Hierarchy hierarchy { get; }

		internal bool isRootVisualContainer
		{
			get
			{
				return this.styleSheets.count > 0;
			}
		}

		[Obsolete("VisualElement.cacheAsBitmap is deprecated and has no effect")]
		public bool cacheAsBitmap { get; set; }

		internal bool disableClipping
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.DisableClipping) == VisualElementFlags.DisableClipping;
			}
			set
			{
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.DisableClipping) : (this.m_Flags & ~VisualElementFlags.DisableClipping));
			}
		}

		internal bool ShouldClip()
		{
			return this.computedStyle.overflow != OverflowInternal.Visible && !this.disableClipping;
		}

		internal bool disableRendering
		{
			get
			{
				return (this.m_Flags & VisualElementFlags.DisableRendering) == VisualElementFlags.DisableRendering;
			}
			set
			{
				VisualElementFlags flags = this.m_Flags;
				this.m_Flags = (value ? (this.m_Flags | VisualElementFlags.DisableRendering) : (this.m_Flags & ~VisualElementFlags.DisableRendering));
				bool flag = flags != this.m_Flags;
				if (flag)
				{
					this.IncrementVersion(VersionChangeType.DisableRendering);
				}
			}
		}

		public VisualElement parent
		{
			get
			{
				return this.m_LogicalParent;
			}
		}

		internal BaseVisualElementPanel elementPanel
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get;
			private set; }

		[CreateProperty(ReadOnly = true)]
		public IPanel panel
		{
			get
			{
				return this.elementPanel;
			}
		}

		public virtual VisualElement contentContainer
		{
			get
			{
				return this;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public VisualTreeAsset visualTreeAssetSource
		{
			get
			{
				return this.m_VisualTreeAssetSource;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.m_VisualTreeAssetSource = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
		internal VisualElementAsset visualElementAsset
		{
			get
			{
				return (VisualElementAsset)this.GetProperty("--unity-visual-element-asset-property");
			}
			set
			{
				this.SetProperty("--unity-visual-element-asset-property", value);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal TemplateAsset templateAsset
		{
			get
			{
				return (TemplateAsset)this.GetProperty("--unity-linked-template-asset-owner");
			}
			set
			{
				this.SetProperty("--unity-linked-template-asset-owner", value);
			}
		}

		public void Add(VisualElement child)
		{
			bool flag = child == null;
			if (!flag)
			{
				VisualElement contentContainer = this.contentContainer;
				bool flag2 = contentContainer == null;
				if (flag2)
				{
					throw new InvalidOperationException("You can't add directly to this VisualElement. Use hierarchy.Add() if you know what you're doing.");
				}
				bool flag3 = contentContainer == this;
				if (flag3)
				{
					this.hierarchy.Add(child);
				}
				else if (contentContainer != null)
				{
					contentContainer.Add(child);
				}
				child.m_LogicalParent = this;
			}
		}

		internal void Add(VisualElement child, bool ignoreContentContainer)
		{
			if (ignoreContentContainer)
			{
				this.hierarchy.Add(child);
			}
			else
			{
				this.Add(child);
			}
		}

		public void Insert(int index, VisualElement element)
		{
			bool flag = element == null;
			if (!flag)
			{
				bool flag2 = this.contentContainer == this;
				if (flag2)
				{
					this.hierarchy.Insert(index, element);
				}
				else
				{
					VisualElement contentContainer = this.contentContainer;
					if (contentContainer != null)
					{
						contentContainer.Insert(index, element);
					}
				}
				element.m_LogicalParent = this;
			}
		}

		internal void Insert(int index, VisualElement element, bool ignoreContentContainer)
		{
			if (ignoreContentContainer)
			{
				this.hierarchy.Insert(index, element);
			}
			else
			{
				this.Insert(index, element);
			}
		}

		public void Remove(VisualElement element)
		{
			bool flag = this.contentContainer == this;
			if (flag)
			{
				this.hierarchy.Remove(element);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Remove(element);
				}
			}
		}

		public void RemoveAt(int index)
		{
			bool flag = this.contentContainer == this;
			if (flag)
			{
				this.hierarchy.RemoveAt(index);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.RemoveAt(index);
				}
			}
		}

		public void Clear()
		{
			bool flag = this.contentContainer == this;
			if (flag)
			{
				this.hierarchy.Clear();
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Clear();
				}
			}
		}

		public VisualElement ElementAt(int index)
		{
			return this[index];
		}

		public VisualElement this[int key]
		{
			get
			{
				bool flag = this.contentContainer == this;
				VisualElement visualElement;
				if (flag)
				{
					visualElement = this.hierarchy[key];
				}
				else
				{
					VisualElement contentContainer = this.contentContainer;
					visualElement = ((contentContainer != null) ? contentContainer[key] : null);
				}
				return visualElement;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public int childCount
		{
			get
			{
				bool flag = this.contentContainer == this;
				int num;
				if (flag)
				{
					num = this.hierarchy.childCount;
				}
				else
				{
					VisualElement contentContainer = this.contentContainer;
					num = ((contentContainer != null) ? contentContainer.childCount : 0);
				}
				return num;
			}
		}

		internal int ChildCount(bool ignoreContentContainer)
		{
			int num;
			if (ignoreContentContainer)
			{
				num = this.hierarchy.childCount;
			}
			else
			{
				num = this.childCount;
			}
			return num;
		}

		public int IndexOf(VisualElement element)
		{
			bool flag = this.contentContainer == this;
			int num;
			if (flag)
			{
				num = this.hierarchy.IndexOf(element);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				num = ((contentContainer != null) ? contentContainer.IndexOf(element) : (-1));
			}
			return num;
		}

		internal int IndexOf(VisualElement element, bool ignoreContentContainer)
		{
			int num;
			if (ignoreContentContainer)
			{
				num = this.hierarchy.IndexOf(element);
			}
			else
			{
				num = this.IndexOf(element);
			}
			return num;
		}

		internal VisualElement ElementAtTreePath(List<int> childIndexes)
		{
			VisualElement visualElement = this;
			foreach (int num in childIndexes)
			{
				bool flag = num >= 0 && num < visualElement.hierarchy.childCount;
				if (!flag)
				{
					return null;
				}
				visualElement = visualElement.hierarchy[num];
			}
			return visualElement;
		}

		internal bool FindElementInTree(VisualElement element, List<int> outChildIndexes)
		{
			VisualElement visualElement = element;
			for (VisualElement visualElement2 = visualElement.hierarchy.parent; visualElement2 != null; visualElement2 = visualElement2.hierarchy.parent)
			{
				outChildIndexes.Insert(0, visualElement2.hierarchy.IndexOf(visualElement));
				bool flag = visualElement2 == this;
				if (flag)
				{
					return true;
				}
				visualElement = visualElement2;
			}
			outChildIndexes.Clear();
			return false;
		}

		public IEnumerable<VisualElement> Children()
		{
			bool flag = this.contentContainer == this;
			IEnumerable<VisualElement> enumerable;
			if (flag)
			{
				enumerable = this.hierarchy.Children();
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				enumerable = ((contentContainer != null) ? contentContainer.Children() : null) ?? VisualElement.s_EmptyList;
			}
			return enumerable;
		}

		public void Sort(Comparison<VisualElement> comp)
		{
			bool flag = this.contentContainer == this;
			if (flag)
			{
				this.hierarchy.Sort(comp);
			}
			else
			{
				VisualElement contentContainer = this.contentContainer;
				if (contentContainer != null)
				{
					contentContainer.Sort(comp);
				}
			}
		}

		public void BringToFront()
		{
			bool flag = this.hierarchy.parent == null;
			if (!flag)
			{
				this.hierarchy.parent.hierarchy.BringToFront(this);
			}
		}

		public void SendToBack()
		{
			bool flag = this.hierarchy.parent == null;
			if (!flag)
			{
				this.hierarchy.parent.hierarchy.SendToBack(this);
			}
		}

		public void PlaceBehind(VisualElement sibling)
		{
			bool flag = sibling == null;
			if (flag)
			{
				throw new ArgumentNullException("sibling");
			}
			bool flag2 = this.hierarchy.parent == null || sibling.hierarchy.parent != this.hierarchy.parent;
			if (flag2)
			{
				throw new ArgumentException("VisualElements are not siblings");
			}
			this.hierarchy.parent.hierarchy.PlaceBehind(this, sibling);
		}

		public void PlaceInFront(VisualElement sibling)
		{
			bool flag = sibling == null;
			if (flag)
			{
				throw new ArgumentNullException("sibling");
			}
			bool flag2 = this.hierarchy.parent == null || sibling.hierarchy.parent != this.hierarchy.parent;
			if (flag2)
			{
				throw new ArgumentException("VisualElements are not siblings");
			}
			this.hierarchy.parent.hierarchy.PlaceInFront(this, sibling);
		}

		internal virtual void OnChildAdded(VisualElement child)
		{
		}

		internal virtual void OnChildRemoved(VisualElement child)
		{
		}

		public void RemoveFromHierarchy()
		{
			bool flag = this.hierarchy.parent != null;
			if (flag)
			{
				this.hierarchy.parent.hierarchy.Remove(this);
			}
		}

		public T GetFirstOfType<T>() where T : class
		{
			T t = this as T;
			bool flag = t != null;
			T t2;
			if (flag)
			{
				t2 = t;
			}
			else
			{
				t2 = this.GetFirstAncestorOfType<T>();
			}
			return t2;
		}

		public T GetFirstAncestorOfType<T>() where T : class
		{
			for (VisualElement visualElement = this.hierarchy.parent; visualElement != null; visualElement = visualElement.hierarchy.parent)
			{
				T t = visualElement as T;
				bool flag = t != null;
				if (flag)
				{
					return t;
				}
			}
			return default(T);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
		internal VisualElement GetFirstAncestorWhere(Predicate<VisualElement> predicate)
		{
			for (VisualElement visualElement = this.hierarchy.parent; visualElement != null; visualElement = visualElement.hierarchy.parent)
			{
				bool flag = predicate(visualElement);
				if (flag)
				{
					return visualElement;
				}
			}
			return null;
		}

		public bool Contains(VisualElement child)
		{
			while (child != null)
			{
				bool flag = child.hierarchy.parent == this;
				if (flag)
				{
					return true;
				}
				child = child.hierarchy.parent;
			}
			return false;
		}

		private void GatherAllChildren(List<VisualElement> elements)
		{
			bool flag = this.m_Children.Count > 0;
			if (flag)
			{
				int i = elements.Count;
				elements.AddRange(this.m_Children);
				while (i < elements.Count)
				{
					VisualElement visualElement = elements[i];
					elements.AddRange(visualElement.m_Children);
					i++;
				}
			}
		}

		public VisualElement FindCommonAncestor(VisualElement other)
		{
			bool flag = other == null;
			if (flag)
			{
				throw new ArgumentNullException("other");
			}
			bool flag2 = this.panel != other.panel;
			VisualElement visualElement;
			if (flag2)
			{
				visualElement = null;
			}
			else
			{
				VisualElement visualElement2 = this;
				int i = 0;
				while (visualElement2 != null)
				{
					i++;
					visualElement2 = visualElement2.hierarchy.parent;
				}
				VisualElement visualElement3 = other;
				int j = 0;
				while (visualElement3 != null)
				{
					j++;
					visualElement3 = visualElement3.hierarchy.parent;
				}
				visualElement2 = this;
				visualElement3 = other;
				while (i > j)
				{
					i--;
					visualElement2 = visualElement2.hierarchy.parent;
				}
				while (j > i)
				{
					j--;
					visualElement3 = visualElement3.hierarchy.parent;
				}
				while (visualElement2 != visualElement3)
				{
					visualElement2 = visualElement2.hierarchy.parent;
					visualElement3 = visualElement3.hierarchy.parent;
				}
				visualElement = visualElement2;
			}
			return visualElement;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal VisualElement GetRoot()
		{
			bool flag = this.panel != null;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = this.panel.visualTree;
			}
			else
			{
				VisualElement visualElement2 = this;
				while (visualElement2.m_PhysicalParent != null)
				{
					visualElement2 = visualElement2.m_PhysicalParent;
				}
				visualElement = visualElement2;
			}
			return visualElement;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal VisualElement GetRootVisualContainer(bool stopAtNearestRoot = false)
		{
			VisualElement visualElement = null;
			for (VisualElement visualElement2 = this; visualElement2 != null; visualElement2 = visualElement2.hierarchy.parent)
			{
				bool isRootVisualContainer = visualElement2.isRootVisualContainer;
				if (isRootVisualContainer)
				{
					visualElement = visualElement2;
					if (stopAtNearestRoot)
					{
						return visualElement;
					}
				}
			}
			return visualElement;
		}

		internal VisualElement GetNextElementDepthFirst()
		{
			bool flag = this.m_Children.Count > 0;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = this.m_Children[0];
			}
			else
			{
				VisualElement visualElement2 = this.m_PhysicalParent;
				VisualElement visualElement3 = this;
				while (visualElement2 != null)
				{
					int i;
					for (i = 0; i < visualElement2.m_Children.Count; i++)
					{
						bool flag2 = visualElement2.m_Children[i] == visualElement3;
						if (flag2)
						{
							break;
						}
					}
					bool flag3 = i < visualElement2.m_Children.Count - 1;
					if (flag3)
					{
						return visualElement2.m_Children[i + 1];
					}
					visualElement3 = visualElement2;
					visualElement2 = visualElement2.m_PhysicalParent;
				}
				visualElement = null;
			}
			return visualElement;
		}

		internal VisualElement GetPreviousElementDepthFirst()
		{
			bool flag = this.m_PhysicalParent != null;
			VisualElement visualElement2;
			if (flag)
			{
				int i;
				for (i = 0; i < this.m_PhysicalParent.m_Children.Count; i++)
				{
					bool flag2 = this.m_PhysicalParent.m_Children[i] == this;
					if (flag2)
					{
						break;
					}
				}
				bool flag3 = i > 0;
				if (flag3)
				{
					VisualElement visualElement = this.m_PhysicalParent.m_Children[i - 1];
					while (visualElement.m_Children.Count > 0)
					{
						visualElement = visualElement.m_Children[visualElement.m_Children.Count - 1];
					}
					visualElement2 = visualElement;
				}
				else
				{
					visualElement2 = this.m_PhysicalParent;
				}
			}
			else
			{
				visualElement2 = null;
			}
			return visualElement2;
		}

		internal VisualElement RetargetElement(VisualElement retargetAgainst)
		{
			bool flag = retargetAgainst == null;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = this;
			}
			else
			{
				VisualElement visualElement2 = retargetAgainst.m_PhysicalParent ?? retargetAgainst;
				while (visualElement2.m_PhysicalParent != null && !visualElement2.isCompositeRoot)
				{
					visualElement2 = visualElement2.m_PhysicalParent;
				}
				VisualElement visualElement3 = this;
				VisualElement visualElement4 = this.m_PhysicalParent;
				while (visualElement4 != null)
				{
					visualElement4 = visualElement4.m_PhysicalParent;
					bool flag2 = visualElement4 == visualElement2;
					if (flag2)
					{
						return visualElement3;
					}
					bool flag3 = visualElement4 != null && visualElement4.isCompositeRoot;
					if (flag3)
					{
						visualElement3 = visualElement4;
					}
				}
				visualElement = this;
			}
			return visualElement;
		}

		private Vector3 positionWithLayout
		{
			get
			{
				return this.ResolveTranslate() + this.layout.min;
			}
		}

		internal void GetPivotedMatrixWithLayout(out Matrix4x4 result)
		{
			Vector3 vector = this.ResolveTransformOrigin();
			result = Matrix4x4.TRS(this.positionWithLayout + vector, this.ResolveRotation(), this.ResolveScale());
			VisualElement.TranslateMatrix34InPlace(ref result, -vector);
		}

		internal bool hasDefaultRotationAndScale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.computedStyle.rotate.angle.value == 0f && this.computedStyle.scale.value == Vector3.one;
			}
		}

		internal bool has3DTransform
		{
			get
			{
				return this.has3DTranslation || this.has3DRotation;
			}
		}

		private bool has3DTranslation
		{
			get
			{
				return this.computedStyle.translate.z != 0f;
			}
		}

		private bool has3DRotation
		{
			get
			{
				Rotate rotate = this.computedStyle.rotate;
				return rotate.angle != 0f && rotate.axis != Vector3.forward;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float Min(float a, float b, float c, float d)
		{
			return Mathf.Min(Mathf.Min(a, b), Mathf.Min(c, d));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float Max(float a, float b, float c, float d)
		{
			return Mathf.Max(Mathf.Max(a, b), Mathf.Max(c, d));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void TransformAlignedBoundsToParentSpace(ref Bounds bounds)
		{
			bool hasDefaultRotationAndScale = this.hasDefaultRotationAndScale;
			if (hasDefaultRotationAndScale)
			{
				bounds.center += this.positionWithLayout;
			}
			else
			{
				Matrix4x4 matrix4x;
				this.GetPivotedMatrixWithLayout(out matrix4x);
				bounds = VisualElement.CalculateConservativeBounds(ref matrix4x, bounds);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void TransformAlignedRectToParentSpace(ref Rect rect)
		{
			bool hasDefaultRotationAndScale = this.hasDefaultRotationAndScale;
			if (hasDefaultRotationAndScale)
			{
				rect.position += this.positionWithLayout;
			}
			else
			{
				Matrix4x4 matrix4x;
				this.GetPivotedMatrixWithLayout(out matrix4x);
				rect = VisualElement.CalculateConservativeRect(ref matrix4x, rect);
			}
		}

		internal static Rect CalculateConservativeRect(ref Matrix4x4 matrix, Rect rect)
		{
			bool flag = float.IsNaN(rect.height) | float.IsNaN(rect.width) | float.IsNaN(rect.x) | float.IsNaN(rect.y);
			Rect rect2;
			if (flag)
			{
				rect = new Rect(VisualElement.MultiplyMatrix44Point2(ref matrix, rect.position), VisualElement.MultiplyVector2(ref matrix, rect.size));
				VisualElement.OrderMinMaxRect(ref rect);
				rect2 = rect;
			}
			else
			{
				Vector2 vector = new Vector2(rect.xMin, rect.yMin);
				Vector2 vector2 = new Vector2(rect.xMax, rect.yMax);
				Vector2 vector3 = new Vector2(rect.xMax, rect.yMin);
				Vector2 vector4 = new Vector2(rect.xMin, rect.yMax);
				Vector3 vector5 = matrix.MultiplyPoint3x4(vector);
				Vector3 vector6 = matrix.MultiplyPoint3x4(vector2);
				Vector3 vector7 = matrix.MultiplyPoint3x4(vector3);
				Vector3 vector8 = matrix.MultiplyPoint3x4(vector4);
				Vector2 vector9 = new Vector2(VisualElement.Min(vector5.x, vector6.x, vector7.x, vector8.x), VisualElement.Min(vector5.y, vector6.y, vector7.y, vector8.y));
				Vector2 vector10 = new Vector2(VisualElement.Max(vector5.x, vector6.x, vector7.x, vector8.x), VisualElement.Max(vector5.y, vector6.y, vector7.y, vector8.y));
				rect2 = new Rect(vector9.x, vector9.y, vector10.x - vector9.x, vector10.y - vector9.y);
			}
			return rect2;
		}

		internal static Bounds CalculateConservativeBounds(ref Matrix4x4 matrix, Bounds bounds)
		{
			bool flag = VisualElement.<CalculateConservativeBounds>g__IsNaN|773_0(bounds.center) | VisualElement.<CalculateConservativeBounds>g__IsNaN|773_0(bounds.extents);
			Bounds bounds2;
			if (flag)
			{
				bounds = new Bounds(matrix.MultiplyPoint3x4(bounds.center), matrix.MultiplyVector(bounds.size));
				VisualElement.OrderMinMaxBounds(ref bounds);
				bounds2 = bounds;
			}
			else
			{
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				for (int i = 0; i < 8; i++)
				{
					Vector3 vector3 = new Vector3(((i & 1) != 0) ? max.x : min.x, ((i & 2) != 0) ? max.y : min.y, ((i & 4) != 0) ? max.z : min.z);
					vector3 = matrix.MultiplyPoint3x4(vector3);
					vector = ((i == 0) ? vector3 : Vector3.Min(vector, vector3));
					vector2 = ((i == 0) ? vector3 : Vector3.Max(vector2, vector3));
				}
				bounds.SetMinMax(vector, vector2);
				bounds2 = bounds;
			}
			return bounds2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void TransformAlignedRect(ref Matrix4x4 matrix, ref Rect rect)
		{
			rect = VisualElement.CalculateConservativeRect(ref matrix, rect);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void TransformAlignedBounds(ref Matrix4x4 matrix, ref Bounds bounds)
		{
			bounds = VisualElement.CalculateConservativeBounds(ref matrix, bounds);
		}

		internal static void OrderMinMaxRect(ref Rect rect)
		{
			bool flag = rect.width < 0f;
			if (flag)
			{
				rect.x += rect.width;
				rect.width = -rect.width;
			}
			bool flag2 = rect.height < 0f;
			if (flag2)
			{
				rect.y += rect.height;
				rect.height = -rect.height;
			}
		}

		internal static void OrderMinMaxBounds(ref Bounds bounds)
		{
			Vector3 extents = bounds.extents;
			bounds.extents = new Vector3(Mathf.Abs(extents.x), Mathf.Abs(extents.y), Mathf.Abs(extents.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2 MultiplyMatrix44Point2(ref Matrix4x4 lhs, Vector2 point)
		{
			Vector2 vector;
			vector.x = lhs.m00 * point.x + lhs.m01 * point.y + lhs.m03;
			vector.y = lhs.m10 * point.x + lhs.m11 * point.y + lhs.m13;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector3 MultiplyMatrix44Point2ToPoint3(ref Matrix4x4 lhs, Vector2 point)
		{
			Vector3 vector;
			vector.x = lhs.m00 * point.x + lhs.m01 * point.y + lhs.m03;
			vector.y = lhs.m10 * point.x + lhs.m11 * point.y + lhs.m13;
			vector.z = lhs.m20 * point.x + lhs.m21 * point.y + lhs.m23;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2 MultiplyMatrix44Point3ToPoint2(ref Matrix4x4 lhs, Vector3 point)
		{
			Vector2 vector;
			vector.x = lhs.m00 * point.x + lhs.m01 * point.y + lhs.m02 * point.z + lhs.m03;
			vector.y = lhs.m10 * point.x + lhs.m11 * point.y + lhs.m12 * point.z + lhs.m13;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2 MultiplyVector2(ref Matrix4x4 lhs, Vector2 vector)
		{
			Vector2 vector2;
			vector2.x = lhs.m00 * vector.x + lhs.m01 * vector.y;
			vector2.y = lhs.m10 * vector.x + lhs.m11 * vector.y;
			return vector2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Rect MultiplyMatrix44Rect2(ref Matrix4x4 lhs, Rect r)
		{
			r.position = VisualElement.MultiplyMatrix44Point2(ref lhs, r.position);
			r.size = VisualElement.MultiplyVector2(ref lhs, r.size);
			return r;
		}

		internal static void MultiplyMatrix34(ref Matrix4x4 lhs, ref Matrix4x4 rhs, out Matrix4x4 res)
		{
			res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20;
			res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21;
			res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22;
			res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03;
			res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20;
			res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21;
			res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22;
			res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13;
			res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20;
			res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21;
			res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22;
			res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23;
			res.m30 = 0f;
			res.m31 = 0f;
			res.m32 = 0f;
			res.m33 = 1f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void TranslateMatrix34(ref Matrix4x4 lhs, Vector3 rhs, out Matrix4x4 res)
		{
			res = lhs;
			VisualElement.TranslateMatrix34InPlace(ref res, rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void TranslateMatrix34InPlace(ref Matrix4x4 lhs, Vector3 rhs)
		{
			lhs.m03 += lhs.m00 * rhs.x + lhs.m01 * rhs.y + lhs.m02 * rhs.z;
			lhs.m13 += lhs.m10 * rhs.x + lhs.m11 * rhs.y + lhs.m12 * rhs.z;
			lhs.m23 += lhs.m20 * rhs.x + lhs.m21 * rhs.y + lhs.m22 * rhs.z;
		}

		public IVisualElementScheduler schedule
		{
			get
			{
				return this;
			}
		}

		IVisualElementScheduledItem IVisualElementScheduler.Execute(Action<TimerState> timerUpdateEvent)
		{
			VisualElement.TimerStateScheduledItem timerStateScheduledItem = new VisualElement.TimerStateScheduledItem(this, timerUpdateEvent)
			{
				timerUpdateStopCondition = ScheduledItem.OnceCondition
			};
			timerStateScheduledItem.Resume();
			return timerStateScheduledItem;
		}

		IVisualElementScheduledItem IVisualElementScheduler.Execute(Action updateEvent)
		{
			VisualElement.SimpleScheduledItem simpleScheduledItem = new VisualElement.SimpleScheduledItem(this, updateEvent)
			{
				timerUpdateStopCondition = ScheduledItem.OnceCondition
			};
			simpleScheduledItem.Resume();
			return simpleScheduledItem;
		}

		[CreateProperty]
		public IStyle style
		{
			get
			{
				bool flag = this.inlineStyleAccess == null;
				if (flag)
				{
					this.inlineStyleAccess = new InlineStyleAccess(this);
				}
				return this.inlineStyleAccess;
			}
		}

		[CreateProperty]
		public IResolvedStyle resolvedStyle
		{
			get
			{
				bool flag = this.resolvedStyleAccess == null;
				if (flag)
				{
					this.resolvedStyleAccess = new ResolvedStyleAccess(this);
				}
				return this.resolvedStyleAccess;
			}
		}

		public ICustomStyle customStyle
		{
			get
			{
				VisualElement.s_CustomStyleAccess.SetContext(this.computedStyle.customProperties, this.computedStyle.dpiScaling);
				return VisualElement.s_CustomStyleAccess;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public VisualElementStyleSheetSet styleSheets
		{
			get
			{
				return new VisualElementStyleSheetSet(this);
			}
		}

		internal void AddStyleSheetPath(string sheetPath)
		{
			StyleSheet styleSheet = Panel.LoadResource(sheetPath, typeof(StyleSheet), this.scaledPixelsPerPoint_noChecks) as StyleSheet;
			bool flag = styleSheet == null;
			if (flag)
			{
				bool flag2 = !VisualElement.s_InternalStyleSheetPath.IsMatch(sheetPath);
				if (flag2)
				{
					Debug.LogWarning(string.Format("Style sheet not found for path \"{0}\"", sheetPath));
				}
			}
			else
			{
				this.styleSheets.Add(styleSheet);
			}
		}

		internal bool HasStyleSheetPath(string sheetPath)
		{
			StyleSheet styleSheet = Panel.LoadResource(sheetPath, typeof(StyleSheet), this.scaledPixelsPerPoint_noChecks) as StyleSheet;
			bool flag = styleSheet == null;
			bool flag2;
			if (flag)
			{
				Debug.LogWarning(string.Format("Style sheet not found for path \"{0}\"", sheetPath));
				flag2 = false;
			}
			else
			{
				flag2 = this.styleSheets.Contains(styleSheet);
			}
			return flag2;
		}

		internal void RemoveStyleSheetPath(string sheetPath)
		{
			StyleSheet styleSheet = Panel.LoadResource(sheetPath, typeof(StyleSheet), this.scaledPixelsPerPoint_noChecks) as StyleSheet;
			bool flag = styleSheet == null;
			if (flag)
			{
				Debug.LogWarning(string.Format("Style sheet not found for path \"{0}\"", sheetPath));
			}
			else
			{
				this.styleSheets.Remove(styleSheet);
			}
		}

		internal StyleFloat ResolveLengthValue(Length length, bool isRow)
		{
			bool flag = length.IsAuto();
			StyleFloat styleFloat;
			if (flag)
			{
				styleFloat = new StyleFloat(StyleKeyword.Auto);
			}
			else
			{
				bool flag2 = length.IsNone();
				if (flag2)
				{
					styleFloat = new StyleFloat(StyleKeyword.None);
				}
				else
				{
					bool flag3 = length.unit != LengthUnit.Percent;
					if (flag3)
					{
						styleFloat = new StyleFloat(length.value);
					}
					else
					{
						VisualElement parent = this.hierarchy.parent;
						bool flag4 = parent == null;
						if (flag4)
						{
							styleFloat = 0f;
						}
						else
						{
							float num = (isRow ? parent.resolvedStyle.width : parent.resolvedStyle.height);
							styleFloat = length.value * num / 100f;
						}
					}
				}
			}
			return styleFloat;
		}

		internal Vector3 ResolveTranslate()
		{
			Translate translate = this.computedStyle.translate;
			Length x = translate.x;
			bool flag = x.unit == LengthUnit.Percent;
			float num;
			if (flag)
			{
				float width = this.resolvedStyle.width;
				num = (float.IsNaN(width) ? 0f : (width * x.value / 100f));
			}
			else
			{
				num = x.value;
				num = (float.IsNaN(num) ? 0f : num);
			}
			Length y = translate.y;
			bool flag2 = y.unit == LengthUnit.Percent;
			float num2;
			if (flag2)
			{
				float height = this.resolvedStyle.height;
				num2 = (float.IsNaN(height) ? 0f : (height * y.value / 100f));
			}
			else
			{
				num2 = y.value;
				num2 = (float.IsNaN(num2) ? 0f : num2);
			}
			float num3 = translate.z;
			num3 = (float.IsNaN(num3) ? 0f : num3);
			return new Vector3(num, num2, num3);
		}

		internal Vector3 ResolveTransformOrigin()
		{
			TransformOrigin transformOrigin = this.computedStyle.transformOrigin;
			Length x = transformOrigin.x;
			bool flag = x.IsNone();
			float num;
			if (flag)
			{
				float width = this.resolvedStyle.width;
				num = (float.IsNaN(width) ? 0f : (width / 2f));
			}
			else
			{
				bool flag2 = x.unit == LengthUnit.Percent;
				if (flag2)
				{
					float width2 = this.resolvedStyle.width;
					num = (float.IsNaN(width2) ? 0f : (width2 * x.value / 100f));
				}
				else
				{
					num = x.value;
				}
			}
			Length y = transformOrigin.y;
			bool flag3 = y.IsNone();
			float num2;
			if (flag3)
			{
				float height = this.resolvedStyle.height;
				num2 = (float.IsNaN(height) ? 0f : (height / 2f));
			}
			else
			{
				bool flag4 = y.unit == LengthUnit.Percent;
				if (flag4)
				{
					float height2 = this.resolvedStyle.height;
					num2 = (float.IsNaN(height2) ? 0f : (height2 * y.value / 100f));
				}
				else
				{
					num2 = y.value;
				}
			}
			float z = transformOrigin.z;
			return new Vector3(num, num2, z);
		}

		private Quaternion ResolveRotation()
		{
			Rotate rotate = this.computedStyle.rotate;
			Vector3 axis = rotate.axis;
			bool flag = float.IsNaN(rotate.angle.value) || float.IsNaN(axis.x) || float.IsNaN(axis.y) || float.IsNaN(axis.z);
			if (flag)
			{
				rotate = Rotate.Initial();
			}
			return rotate.ToQuaternion();
		}

		private Vector3 ResolveScale()
		{
			Vector3 value = this.computedStyle.scale.value;
			BaseVisualElementPanel elementPanel = this.elementPanel;
			bool flag = elementPanel != null && elementPanel.isFlat;
			if (flag)
			{
				value.z = 1f;
			}
			return (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z)) ? Vector3.one : value;
		}

		[CreateProperty]
		public string tooltip
		{
			get
			{
				string text = this.GetProperty(VisualElement.tooltipPropertyKey) as string;
				return text ?? string.Empty;
			}
			set
			{
				bool flag = !this.HasProperty(VisualElement.tooltipPropertyKey);
				if (flag)
				{
					bool flag2 = string.IsNullOrEmpty(value);
					if (flag2)
					{
						return;
					}
					base.RegisterCallback<TooltipEvent>(new EventCallback<TooltipEvent>(this.SetTooltip), TrickleDown.NoTrickleDown);
				}
				string text = this.GetProperty(VisualElement.tooltipPropertyKey) as string;
				bool flag3 = string.CompareOrdinal(text, value) == 0;
				if (!flag3)
				{
					this.SetProperty(VisualElement.tooltipPropertyKey, value);
					base.NotifyPropertyChanged(in VisualElement.tooltipProperty);
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal static VisualElement.TypeData GetOrCreateTypeData(Type t)
		{
			VisualElement.TypeData typeData;
			bool flag = !VisualElement.s_TypeData.TryGetValue(t, out typeData);
			if (flag)
			{
				typeData = new VisualElement.TypeData(t);
				VisualElement.s_TypeData.Add(t, typeData);
			}
			return typeData;
		}

		private VisualElement.TypeData typeData
		{
			get
			{
				bool flag = this.m_TypeData == null;
				if (flag)
				{
					Type type = base.GetType();
					bool flag2 = !VisualElement.s_TypeData.TryGetValue(type, out this.m_TypeData);
					if (flag2)
					{
						this.m_TypeData = new VisualElement.TypeData(type);
						VisualElement.s_TypeData.Add(type, this.m_TypeData);
					}
				}
				return this.m_TypeData;
			}
		}

		[CompilerGenerated]
		internal static bool <CalculateConservativeBounds>g__IsNaN|773_0(Vector3 v)
		{
			return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
		}

		internal static uint s_NextId;

		private static List<string> s_EmptyClassList = new List<string>(0);

		internal static readonly PropertyName userDataPropertyKey = new PropertyName("--unity-user-data");

		public static readonly string disabledUssClassName = "unity-disabled";

		private string m_Name;

		private List<string> m_ClassList;

		private Dictionary<PropertyName, object> m_PropertyBag;

		private VisualElementFlags m_Flags;

		private string m_ViewDataKey;

		private RenderHints m_RenderHints;

		internal Rect lastLayout;

		internal Rect lastPseudoPadding;

		internal RenderData renderData;

		internal RenderData nestedRenderData;

		internal int hierarchyDepth;

		internal int insertionIndex = -1;

		private Rect m_Layout;

		private Rect m_BoundingBox;

		private const VisualElementFlags worldBoundingBoxDirtyDependencies = VisualElementFlags.WorldTransformDirty | VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty;

		private Rect m_WorldBoundingBox;

		private const VisualElementFlags worldTransformInverseDirtyDependencies = VisualElementFlags.WorldTransformDirty | VisualElementFlags.WorldTransformInverseDirty;

		private Matrix4x4 m_WorldTransformCache = Matrix4x4.identity;

		private Matrix4x4 m_WorldTransformInverseCache = Matrix4x4.identity;

		internal PseudoStates triggerPseudoMask;

		internal PseudoStates dependencyPseudoMask;

		private PseudoStates m_PseudoStates;

		private PickingMode m_PickingMode;

		private LayoutNode m_LayoutNode;

		internal ComputedStyle m_Style = InitialStyle.Acquire();

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleVariableContext variableContext = StyleVariableContext.none;

		internal int inheritedStylesHash = 0;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal readonly uint controlid;

		internal int imguiContainerDescendantCount = 0;

		internal static int s_FinalizerCount = 0;

		private bool m_EnabledSelf;

		private LanguageDirection m_LanguageDirection;

		private LanguageDirection m_LocalLanguageDirection;

		private static readonly ProfilerMarker k_GenerateVisualContentMarker = new ProfilerMarker("GenerateVisualContent");

		private List<IValueAnimationUpdate> m_RunningAnimations;

		internal static readonly BindingId childCountProperty = "childCount";

		internal static readonly BindingId contentRectProperty = "contentRect";

		internal static readonly BindingId dataSourcePathProperty = "dataSourcePath";

		internal static readonly BindingId dataSourceProperty = "dataSource";

		internal static readonly BindingId disablePlayModeTintProperty = "disablePlayModeTint";

		internal static readonly BindingId enabledInHierarchyProperty = "enabledInHierarchy";

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal static readonly BindingId enabledSelfProperty = "enabledSelf";

		internal static readonly BindingId layoutProperty = "layout";

		internal static readonly BindingId languageDirectionProperty = "languageDirection";

		internal static readonly BindingId localBoundProperty = "localBound";

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal static readonly BindingId nameProperty = "name";

		internal static readonly BindingId panelProperty = "panel";

		internal static readonly BindingId pickingModeProperty = "pickingMode";

		internal static readonly BindingId styleSheetsProperty = "styleSheets";

		internal static readonly BindingId tooltipProperty = "tooltip";

		internal static readonly BindingId usageHintsProperty = "usageHints";

		internal static readonly BindingId userDataProperty = "userData";

		internal static readonly BindingId viewDataKeyProperty = "viewDataKey";

		internal static readonly BindingId visibleProperty = "visible";

		internal static readonly BindingId visualTreeAssetSourceProperty = "visualTreeAssetSource";

		internal static readonly BindingId worldBoundProperty = "worldBound";

		internal static readonly BindingId worldTransformProperty = "worldTransform";

		private object m_DataSource;

		private PathRef m_DataSourcePath;

		private List<Binding> m_Bindings;

		private readonly int m_TrickleDownHandleEventCategories;

		private readonly int m_BubbleUpHandleEventCategories;

		private int m_BubbleUpEventCallbackCategories = 0;

		private int m_TrickleDownEventCallbackCategories = 0;

		private int m_EventInterestSelfCategories = 0;

		private int m_CachedEventInterestParentCategories = 0;

		private static uint s_NextParentVersion;

		private uint m_NextParentCachedVersion;

		private uint m_NextParentRequiredVersion;

		private VisualElement m_CachedNextParentWithEventInterests;

		private const string k_VisualElementAssetPropertyName = "--unity-visual-element-asset-property";

		private const string k_LinkedTemplateAssetOwnerPropertyName = "--unity-linked-template-asset-owner";

		internal const string k_RootVisualContainerName = "rootVisualContainer";

		private VisualElement m_PhysicalParent;

		private VisualElement m_LogicalParent;

		private static readonly List<VisualElement> s_EmptyList = new List<VisualElement>();

		private List<VisualElement> m_Children;

		private VisualTreeAsset m_VisualTreeAssetSource = null;

		internal static VisualElement.CustomStyleAccess s_CustomStyleAccess = new VisualElement.CustomStyleAccess();

		internal InlineStyleAccess inlineStyleAccess;

		internal ResolvedStyleAccess resolvedStyleAccess;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal List<StyleSheet> styleSheetList;

		private static readonly Regex s_InternalStyleSheetPath = new Regex("^instanceId:[-0-9]+$", RegexOptions.Compiled);

		internal static readonly PropertyName tooltipPropertyKey = new PropertyName("--unity-tooltip");

		private static readonly Dictionary<Type, VisualElement.TypeData> s_TypeData = new Dictionary<Type, VisualElement.TypeData>();

		private VisualElement.TypeData m_TypeData;

		internal static class ResolvedStyleProperties
		{
			internal static readonly BindingId alignContentProperty = "resolvedStyle.alignContent";

			internal static readonly BindingId alignItemsProperty = "resolvedStyle.alignItems";

			internal static readonly BindingId alignSelfProperty = "resolvedStyle.alignSelf";

			internal static readonly BindingId aspectRatioProperty = "resolvedStyle.aspectRatio";

			internal static readonly BindingId backgroundColorProperty = "resolvedStyle.backgroundColor";

			internal static readonly BindingId backgroundImageProperty = "resolvedStyle.backgroundImage";

			internal static readonly BindingId backgroundPositionXProperty = "resolvedStyle.backgroundPositionX";

			internal static readonly BindingId backgroundPositionYProperty = "resolvedStyle.backgroundPositionY";

			internal static readonly BindingId backgroundRepeatProperty = "resolvedStyle.backgroundRepeat";

			internal static readonly BindingId backgroundSizeProperty = "resolvedStyle.backgroundSize";

			internal static readonly BindingId borderBottomColorProperty = "resolvedStyle.borderBottomColor";

			internal static readonly BindingId borderBottomLeftRadiusProperty = "resolvedStyle.borderBottomLeftRadius";

			internal static readonly BindingId borderBottomRightRadiusProperty = "resolvedStyle.borderBottomRightRadius";

			internal static readonly BindingId borderBottomWidthProperty = "resolvedStyle.borderBottomWidth";

			internal static readonly BindingId borderLeftColorProperty = "resolvedStyle.borderLeftColor";

			internal static readonly BindingId borderLeftWidthProperty = "resolvedStyle.borderLeftWidth";

			internal static readonly BindingId borderRightColorProperty = "resolvedStyle.borderRightColor";

			internal static readonly BindingId borderRightWidthProperty = "resolvedStyle.borderRightWidth";

			internal static readonly BindingId borderTopColorProperty = "resolvedStyle.borderTopColor";

			internal static readonly BindingId borderTopLeftRadiusProperty = "resolvedStyle.borderTopLeftRadius";

			internal static readonly BindingId borderTopRightRadiusProperty = "resolvedStyle.borderTopRightRadius";

			internal static readonly BindingId borderTopWidthProperty = "resolvedStyle.borderTopWidth";

			internal static readonly BindingId bottomProperty = "resolvedStyle.bottom";

			internal static readonly BindingId colorProperty = "resolvedStyle.color";

			internal static readonly BindingId displayProperty = "resolvedStyle.display";

			internal static readonly BindingId filterProperty = "resolvedStyle.filter";

			internal static readonly BindingId flexBasisProperty = "resolvedStyle.flexBasis";

			internal static readonly BindingId flexDirectionProperty = "resolvedStyle.flexDirection";

			internal static readonly BindingId flexGrowProperty = "resolvedStyle.flexGrow";

			internal static readonly BindingId flexShrinkProperty = "resolvedStyle.flexShrink";

			internal static readonly BindingId flexWrapProperty = "resolvedStyle.flexWrap";

			internal static readonly BindingId fontSizeProperty = "resolvedStyle.fontSize";

			internal static readonly BindingId heightProperty = "resolvedStyle.height";

			internal static readonly BindingId justifyContentProperty = "resolvedStyle.justifyContent";

			internal static readonly BindingId leftProperty = "resolvedStyle.left";

			internal static readonly BindingId letterSpacingProperty = "resolvedStyle.letterSpacing";

			internal static readonly BindingId marginBottomProperty = "resolvedStyle.marginBottom";

			internal static readonly BindingId marginLeftProperty = "resolvedStyle.marginLeft";

			internal static readonly BindingId marginRightProperty = "resolvedStyle.marginRight";

			internal static readonly BindingId marginTopProperty = "resolvedStyle.marginTop";

			internal static readonly BindingId maxHeightProperty = "resolvedStyle.maxHeight";

			internal static readonly BindingId maxWidthProperty = "resolvedStyle.maxWidth";

			internal static readonly BindingId minHeightProperty = "resolvedStyle.minHeight";

			internal static readonly BindingId minWidthProperty = "resolvedStyle.minWidth";

			internal static readonly BindingId opacityProperty = "resolvedStyle.opacity";

			internal static readonly BindingId paddingBottomProperty = "resolvedStyle.paddingBottom";

			internal static readonly BindingId paddingLeftProperty = "resolvedStyle.paddingLeft";

			internal static readonly BindingId paddingRightProperty = "resolvedStyle.paddingRight";

			internal static readonly BindingId paddingTopProperty = "resolvedStyle.paddingTop";

			internal static readonly BindingId positionProperty = "resolvedStyle.position";

			internal static readonly BindingId rightProperty = "resolvedStyle.right";

			internal static readonly BindingId rotateProperty = "resolvedStyle.rotate";

			internal static readonly BindingId scaleProperty = "resolvedStyle.scale";

			internal static readonly BindingId textOverflowProperty = "resolvedStyle.textOverflow";

			internal static readonly BindingId topProperty = "resolvedStyle.top";

			internal static readonly BindingId transformOriginProperty = "resolvedStyle.transformOrigin";

			internal static readonly BindingId transitionDelayProperty = "resolvedStyle.transitionDelay";

			internal static readonly BindingId transitionDurationProperty = "resolvedStyle.transitionDuration";

			internal static readonly BindingId transitionPropertyProperty = "resolvedStyle.transitionProperty";

			internal static readonly BindingId transitionTimingFunctionProperty = "resolvedStyle.transitionTimingFunction";

			internal static readonly BindingId translateProperty = "resolvedStyle.translate";

			internal static readonly BindingId unityBackgroundImageTintColorProperty = "resolvedStyle.unityBackgroundImageTintColor";

			internal static readonly BindingId unityEditorTextRenderingModeProperty = "resolvedStyle.unityEditorTextRenderingMode";

			internal static readonly BindingId unityFontProperty = "resolvedStyle.unityFont";

			internal static readonly BindingId unityFontDefinitionProperty = "resolvedStyle.unityFontDefinition";

			internal static readonly BindingId unityFontStyleAndWeightProperty = "resolvedStyle.unityFontStyleAndWeight";

			internal static readonly BindingId unityMaterialProperty = "resolvedStyle.unityMaterial";

			internal static readonly BindingId unityParagraphSpacingProperty = "resolvedStyle.unityParagraphSpacing";

			internal static readonly BindingId unitySliceBottomProperty = "resolvedStyle.unitySliceBottom";

			internal static readonly BindingId unitySliceLeftProperty = "resolvedStyle.unitySliceLeft";

			internal static readonly BindingId unitySliceRightProperty = "resolvedStyle.unitySliceRight";

			internal static readonly BindingId unitySliceScaleProperty = "resolvedStyle.unitySliceScale";

			internal static readonly BindingId unitySliceTopProperty = "resolvedStyle.unitySliceTop";

			internal static readonly BindingId unitySliceTypeProperty = "resolvedStyle.unitySliceType";

			internal static readonly BindingId unityTextAlignProperty = "resolvedStyle.unityTextAlign";

			internal static readonly BindingId unityTextGeneratorProperty = "resolvedStyle.unityTextGenerator";

			internal static readonly BindingId unityTextOutlineColorProperty = "resolvedStyle.unityTextOutlineColor";

			internal static readonly BindingId unityTextOutlineWidthProperty = "resolvedStyle.unityTextOutlineWidth";

			internal static readonly BindingId unityTextOverflowPositionProperty = "resolvedStyle.unityTextOverflowPosition";

			internal static readonly BindingId visibilityProperty = "resolvedStyle.visibility";

			internal static readonly BindingId whiteSpaceProperty = "resolvedStyle.whiteSpace";

			internal static readonly BindingId widthProperty = "resolvedStyle.width";

			internal static readonly BindingId wordSpacingProperty = "resolvedStyle.wordSpacing";
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static class StyleProperties
		{
			internal static readonly BindingId alignContentProperty = "style.alignContent";

			internal static readonly BindingId alignItemsProperty = "style.alignItems";

			internal static readonly BindingId alignSelfProperty = "style.alignSelf";

			internal static readonly BindingId aspectRatioProperty = "style.aspectRatio";

			internal static readonly BindingId backgroundColorProperty = "style.backgroundColor";

			internal static readonly BindingId backgroundImageProperty = "style.backgroundImage";

			internal static readonly BindingId backgroundPositionXProperty = "style.backgroundPositionX";

			internal static readonly BindingId backgroundPositionYProperty = "style.backgroundPositionY";

			internal static readonly BindingId backgroundRepeatProperty = "style.backgroundRepeat";

			internal static readonly BindingId backgroundSizeProperty = "style.backgroundSize";

			internal static readonly BindingId borderBottomColorProperty = "style.borderBottomColor";

			internal static readonly BindingId borderBottomLeftRadiusProperty = "style.borderBottomLeftRadius";

			internal static readonly BindingId borderBottomRightRadiusProperty = "style.borderBottomRightRadius";

			internal static readonly BindingId borderBottomWidthProperty = "style.borderBottomWidth";

			internal static readonly BindingId borderLeftColorProperty = "style.borderLeftColor";

			internal static readonly BindingId borderLeftWidthProperty = "style.borderLeftWidth";

			internal static readonly BindingId borderRightColorProperty = "style.borderRightColor";

			internal static readonly BindingId borderRightWidthProperty = "style.borderRightWidth";

			internal static readonly BindingId borderTopColorProperty = "style.borderTopColor";

			internal static readonly BindingId borderTopLeftRadiusProperty = "style.borderTopLeftRadius";

			internal static readonly BindingId borderTopRightRadiusProperty = "style.borderTopRightRadius";

			internal static readonly BindingId borderTopWidthProperty = "style.borderTopWidth";

			internal static readonly BindingId bottomProperty = "style.bottom";

			internal static readonly BindingId colorProperty = "style.color";

			internal static readonly BindingId cursorProperty = "style.cursor";

			internal static readonly BindingId displayProperty = "style.display";

			internal static readonly BindingId filterProperty = "style.filter";

			internal static readonly BindingId flexBasisProperty = "style.flexBasis";

			internal static readonly BindingId flexDirectionProperty = "style.flexDirection";

			internal static readonly BindingId flexGrowProperty = "style.flexGrow";

			internal static readonly BindingId flexShrinkProperty = "style.flexShrink";

			internal static readonly BindingId flexWrapProperty = "style.flexWrap";

			internal static readonly BindingId fontSizeProperty = "style.fontSize";

			internal static readonly BindingId heightProperty = "style.height";

			internal static readonly BindingId justifyContentProperty = "style.justifyContent";

			internal static readonly BindingId leftProperty = "style.left";

			internal static readonly BindingId letterSpacingProperty = "style.letterSpacing";

			internal static readonly BindingId marginBottomProperty = "style.marginBottom";

			internal static readonly BindingId marginLeftProperty = "style.marginLeft";

			internal static readonly BindingId marginRightProperty = "style.marginRight";

			internal static readonly BindingId marginTopProperty = "style.marginTop";

			internal static readonly BindingId maxHeightProperty = "style.maxHeight";

			internal static readonly BindingId maxWidthProperty = "style.maxWidth";

			internal static readonly BindingId minHeightProperty = "style.minHeight";

			internal static readonly BindingId minWidthProperty = "style.minWidth";

			internal static readonly BindingId opacityProperty = "style.opacity";

			internal static readonly BindingId overflowProperty = "style.overflow";

			internal static readonly BindingId paddingBottomProperty = "style.paddingBottom";

			internal static readonly BindingId paddingLeftProperty = "style.paddingLeft";

			internal static readonly BindingId paddingRightProperty = "style.paddingRight";

			internal static readonly BindingId paddingTopProperty = "style.paddingTop";

			internal static readonly BindingId positionProperty = "style.position";

			internal static readonly BindingId rightProperty = "style.right";

			internal static readonly BindingId rotateProperty = "style.rotate";

			internal static readonly BindingId scaleProperty = "style.scale";

			internal static readonly BindingId textOverflowProperty = "style.textOverflow";

			internal static readonly BindingId textShadowProperty = "style.textShadow";

			internal static readonly BindingId topProperty = "style.top";

			internal static readonly BindingId transformOriginProperty = "style.transformOrigin";

			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal static readonly BindingId transitionDelayProperty = "style.transitionDelay";

			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal static readonly BindingId transitionDurationProperty = "style.transitionDuration";

			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal static readonly BindingId transitionPropertyProperty = "style.transitionProperty";

			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal static readonly BindingId transitionTimingFunctionProperty = "style.transitionTimingFunction";

			internal static readonly BindingId translateProperty = "style.translate";

			internal static readonly BindingId unityBackgroundImageTintColorProperty = "style.unityBackgroundImageTintColor";

			internal static readonly BindingId unityEditorTextRenderingModeProperty = "style.unityEditorTextRenderingMode";

			internal static readonly BindingId unityFontProperty = "style.unityFont";

			internal static readonly BindingId unityFontDefinitionProperty = "style.unityFontDefinition";

			internal static readonly BindingId unityFontStyleAndWeightProperty = "style.unityFontStyleAndWeight";

			internal static readonly BindingId unityMaterialProperty = "style.unityMaterial";

			internal static readonly BindingId unityOverflowClipBoxProperty = "style.unityOverflowClipBox";

			internal static readonly BindingId unityParagraphSpacingProperty = "style.unityParagraphSpacing";

			internal static readonly BindingId unitySliceBottomProperty = "style.unitySliceBottom";

			internal static readonly BindingId unitySliceLeftProperty = "style.unitySliceLeft";

			internal static readonly BindingId unitySliceRightProperty = "style.unitySliceRight";

			internal static readonly BindingId unitySliceScaleProperty = "style.unitySliceScale";

			internal static readonly BindingId unitySliceTopProperty = "style.unitySliceTop";

			internal static readonly BindingId unitySliceTypeProperty = "style.unitySliceType";

			internal static readonly BindingId unityTextAlignProperty = "style.unityTextAlign";

			internal static readonly BindingId unityTextAutoSizeProperty = "style.unityTextAutoSize";

			internal static readonly BindingId unityTextGeneratorProperty = "style.unityTextGenerator";

			internal static readonly BindingId unityTextOutlineColorProperty = "style.unityTextOutlineColor";

			internal static readonly BindingId unityTextOutlineWidthProperty = "style.unityTextOutlineWidth";

			internal static readonly BindingId unityTextOverflowPositionProperty = "style.unityTextOverflowPosition";

			internal static readonly BindingId visibilityProperty = "style.visibility";

			internal static readonly BindingId whiteSpaceProperty = "style.whiteSpace";

			internal static readonly BindingId widthProperty = "style.width";

			internal static readonly BindingId wordSpacingProperty = "style.wordSpacing";
		}

		[Serializable]
		public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(VisualElement.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("name", "name", null, Array.Empty<string>()),
					new UxmlAttributeNames("enabledSelf", "enabled", null, Array.Empty<string>()),
					new UxmlAttributeNames("viewDataKey", "view-data-key", null, Array.Empty<string>()),
					new UxmlAttributeNames("pickingMode", "picking-mode", null, new string[] { "pickingMode" }),
					new UxmlAttributeNames("tooltip", "tooltip", null, Array.Empty<string>()),
					new UxmlAttributeNames("usageHints", "usage-hints", null, Array.Empty<string>()),
					new UxmlAttributeNames("tabIndex", "tabindex", null, Array.Empty<string>()),
					new UxmlAttributeNames("focusable", "focusable", null, Array.Empty<string>()),
					new UxmlAttributeNames("languageDirection", "language-direction", null, Array.Empty<string>()),
					new UxmlAttributeNames("dataSourceUnityObject", "data-source", null, Array.Empty<string>()),
					new UxmlAttributeNames("dataSourcePathString", "data-source-path", null, Array.Empty<string>()),
					new UxmlAttributeNames("dataSourceTypeString", "data-source-type", typeof(object), Array.Empty<string>()),
					new UxmlAttributeNames("bindings", "Bindings", null, Array.Empty<string>())
				}, false);
			}

			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal bool HasBindingInternal(string property)
			{
				bool flag = this.bindings == null;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					foreach (Binding.UxmlSerializedData uxmlSerializedData in this.bindings)
					{
						bool flag3 = uxmlSerializedData.property == property;
						if (flag3)
						{
							return true;
						}
					}
					flag2 = false;
				}
				return flag2;
			}

			[ExcludeFromDocs]
			public override object CreateInstance()
			{
				return new VisualElement();
			}

			[ExcludeFromDocs]
			public override void Deserialize(object obj)
			{
				VisualElement visualElement = (VisualElement)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.name_UxmlAttributeFlags);
				if (flag)
				{
					visualElement.name = this.name;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.enabledSelf_UxmlAttributeFlags);
				if (flag2)
				{
					visualElement.enabledSelf = this.enabledSelf;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.viewDataKey_UxmlAttributeFlags);
				if (flag3)
				{
					visualElement.viewDataKey = this.viewDataKey;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.pickingMode_UxmlAttributeFlags);
				if (flag4)
				{
					visualElement.pickingMode = this.pickingMode;
				}
				bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.tooltip_UxmlAttributeFlags);
				if (flag5)
				{
					visualElement.tooltip = this.tooltip;
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.usageHints_UxmlAttributeFlags);
				if (flag6)
				{
					visualElement.usageHints = this.usageHints;
				}
				bool flag7 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.tabIndex_UxmlAttributeFlags);
				if (flag7)
				{
					visualElement.tabIndex = this.tabIndex;
				}
				bool flag8 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.focusable_UxmlAttributeFlags);
				if (flag8)
				{
					visualElement.focusable = this.focusable;
				}
				bool flag9 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.dataSourceUnityObject_UxmlAttributeFlags);
				if (flag9)
				{
					visualElement.dataSourceUnityObject = (this.dataSourceUnityObject ? this.dataSourceUnityObject : null);
				}
				bool flag10 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.dataSourcePathString_UxmlAttributeFlags);
				if (flag10)
				{
					visualElement.dataSourcePathString = this.dataSourcePathString;
				}
				bool flag11 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.dataSourceTypeString_UxmlAttributeFlags);
				if (flag11)
				{
					visualElement.dataSourceTypeString = this.dataSourceTypeString;
				}
				bool flag12 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.languageDirection_UxmlAttributeFlags);
				if (flag12)
				{
					visualElement.languageDirection = this.languageDirection;
				}
				bool flag13 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.bindings_UxmlAttributeFlags);
				if (flag13)
				{
					visualElement.bindings.Clear();
					bool flag14 = this.bindings != null;
					if (flag14)
					{
						foreach (Binding.UxmlSerializedData uxmlSerializedData in this.bindings)
						{
							Binding binding = (Binding)uxmlSerializedData.CreateInstance();
							uxmlSerializedData.Deserialize(binding);
							visualElement.SetBinding(binding.property, binding);
							visualElement.bindings.Add(binding);
						}
					}
				}
			}

			[HideInInspector]
			[SerializeField]
			private string name;

			[SerializeReference]
			[HideInInspector]
			[UxmlObjectReference("Bindings")]
			private List<Binding.UxmlSerializedData> bindings;

			[SerializeField]
			private string tooltip;

			[HideInInspector]
			[SerializeField]
			[UxmlAttribute("data-source-path")]
			[Tooltip("The path to the value in the data source used by this binding. To see resolved bindings in the UI Builder, define a path that is compatible with the target source property.")]
			private string dataSourcePathString;

			[SerializeField]
			[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
			[UxmlAttribute("data-source-type")]
			[UxmlTypeReference(typeof(object))]
			[HideInInspector]
			private string dataSourceTypeString;

			[SerializeField]
			[UxmlAttributeBindingPath("dataSource")]
			[Tooltip("A data source is a collection of information. By default, a binding will inherit the existing data source from the hierarchy. You can instead define another object here as the data source, or define the type of property it may be if the source is not yet available.")]
			[HideInInspector]
			[DataSourceDrawer]
			[UxmlAttribute("data-source")]
			private Object dataSourceUnityObject;

			[SerializeField]
			private string viewDataKey;

			[SerializeField]
			[UxmlAttribute(obsoleteNames = new string[] { "pickingMode" })]
			private PickingMode pickingMode;

			[SerializeField]
			private UsageHints usageHints;

			[SerializeField]
			private LanguageDirection languageDirection;

			[UxmlAttribute("tabindex")]
			[SerializeField]
			private int tabIndex;

			[SerializeField]
			private bool focusable;

			[SerializeField]
			[UxmlAttribute("enabled")]
			[Tooltip("Sets the element to disabled which will not accept input. Utilizes the :disabled pseudo state.")]
			private bool enabledSelf;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags name_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags enabledSelf_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags viewDataKey_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags pickingMode_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags tooltip_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags usageHints_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags tabIndex_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags focusable_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags languageDirection_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags dataSourceUnityObject_UxmlAttributeFlags;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags dataSourcePathString_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags dataSourceTypeString_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags bindings_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public class UxmlFactory : UxmlFactory<VisualElement, VisualElement.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public class UxmlTraits : UnityEngine.UIElements.UxmlTraits
		{
			protected UxmlIntAttributeDescription focusIndex { get; set; } = new UxmlIntAttributeDescription
			{
				name = null,
				obsoleteNames = new string[] { "focus-index", "focusIndex" },
				defaultValue = -1
			};

			protected UxmlBoolAttributeDescription focusable { get; set; } = new UxmlBoolAttributeDescription
			{
				name = "focusable",
				defaultValue = false
			};

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield return new UxmlChildElementDescription(typeof(VisualElement));
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				bool flag = ve == null;
				if (flag)
				{
					throw new ArgumentNullException("ve");
				}
				ve.name = this.m_Name.GetValueFromBag(bag, cc);
				ve.enabledSelf = this.m_EnabledSelf.GetValueFromBag(bag, cc);
				ve.viewDataKey = this.m_ViewDataKey.GetValueFromBag(bag, cc);
				ve.pickingMode = this.m_PickingMode.GetValueFromBag(bag, cc);
				ve.usageHints = this.m_UsageHints.GetValueFromBag(bag, cc);
				ve.tooltip = this.m_Tooltip.GetValueFromBag(bag, cc);
				int num = 0;
				bool flag2 = this.focusIndex.TryGetValueFromBag(bag, cc, ref num);
				if (flag2)
				{
					ve.tabIndex = ((num >= 0) ? num : 0);
					ve.focusable = num >= 0;
				}
				ve.tabIndex = this.m_TabIndex.GetValueFromBag(bag, cc);
				ve.focusable = this.focusable.GetValueFromBag(bag, cc);
				ve.dataSource = this.m_DataSource.GetValueFromBag(bag, cc);
				ve.dataSourcePath = new PropertyPath(this.m_DataSourcePath.GetValueFromBag(bag, cc));
			}

			protected UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
			{
				name = "name"
			};

			private UxmlBoolAttributeDescription m_EnabledSelf = new UxmlBoolAttributeDescription
			{
				name = "enabled",
				defaultValue = true
			};

			private UxmlStringAttributeDescription m_ViewDataKey = new UxmlStringAttributeDescription
			{
				name = "view-data-key"
			};

			protected UxmlEnumAttributeDescription<PickingMode> m_PickingMode = new UxmlEnumAttributeDescription<PickingMode>
			{
				name = "picking-mode",
				obsoleteNames = new string[] { "pickingMode" }
			};

			private UxmlStringAttributeDescription m_Tooltip = new UxmlStringAttributeDescription
			{
				name = "tooltip"
			};

			private UxmlEnumAttributeDescription<UsageHints> m_UsageHints = new UxmlEnumAttributeDescription<UsageHints>
			{
				name = "usage-hints"
			};

			private UxmlIntAttributeDescription m_TabIndex = new UxmlIntAttributeDescription
			{
				name = "tabindex",
				defaultValue = 0
			};

			private UxmlStringAttributeDescription m_Class = new UxmlStringAttributeDescription
			{
				name = "class"
			};

			private UxmlStringAttributeDescription m_ContentContainer = new UxmlStringAttributeDescription
			{
				name = "content-container",
				obsoleteNames = new string[] { "contentContainer" }
			};

			private UxmlStringAttributeDescription m_Style = new UxmlStringAttributeDescription
			{
				name = "style"
			};

			private UxmlAssetAttributeDescription<Object> m_DataSource = new UxmlAssetAttributeDescription<Object>
			{
				name = "data-source"
			};

			private UxmlStringAttributeDescription m_DataSourcePath = new UxmlStringAttributeDescription
			{
				name = "data-source-path"
			};
		}

		public enum MeasureMode
		{
			Undefined,
			Exactly,
			AtMost
		}

		public struct Hierarchy
		{
			public VisualElement parent
			{
				get
				{
					return this.m_Owner.m_PhysicalParent;
				}
			}

			internal List<VisualElement> children
			{
				get
				{
					return this.m_Owner.m_Children;
				}
			}

			internal Hierarchy(VisualElement element)
			{
				this.m_Owner = element;
			}

			public void Add(VisualElement child)
			{
				bool flag = child == null;
				if (flag)
				{
					throw new ArgumentException("Cannot add null child");
				}
				this.Insert(this.childCount, child);
			}

			public void Insert(int index, VisualElement child)
			{
				bool flag = child == null;
				if (flag)
				{
					throw new ArgumentException("Cannot insert null child");
				}
				bool flag2 = index > this.childCount;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("Index out of range: " + index.ToString());
				}
				bool flag3 = child == this.m_Owner;
				if (flag3)
				{
					throw new ArgumentException("Cannot insert element as its own child");
				}
				bool flag4 = this.m_Owner.elementPanel != null && this.m_Owner.elementPanel.duringLayoutPhase;
				if (flag4)
				{
					throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
				}
				child.RemoveFromHierarchy();
				bool flag5 = this.m_Owner.m_Children == VisualElement.s_EmptyList;
				if (flag5)
				{
					this.m_Owner.m_Children = VisualElementListPool.Get(0);
				}
				bool usesMeasure = this.m_Owner.layoutNode.UsesMeasure;
				if (usesMeasure)
				{
					this.m_Owner.RemoveMeasureFunction();
				}
				this.PutChildAtIndex(child, index);
				int num = child.imguiContainerDescendantCount + (child.isIMGUIContainer ? 1 : 0);
				bool flag6 = num > 0;
				if (flag6)
				{
					this.m_Owner.ChangeIMGUIContainerCount(num);
				}
				child.hierarchy.SetParent(this.m_Owner);
				child.PropagateEnabledToChildren(this.m_Owner.enabledInHierarchy);
				bool flag7 = child.languageDirection == LanguageDirection.Inherit;
				if (flag7)
				{
					child.localLanguageDirection = this.m_Owner.localLanguageDirection;
				}
				child.InvokeHierarchyChanged(HierarchyChangeType.AddedToParent, null);
				child.IncrementVersion(VersionChangeType.Hierarchy);
				this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
				this.m_Owner.OnChildAdded(child);
			}

			public void Remove(VisualElement child)
			{
				bool flag = child == null;
				if (flag)
				{
					throw new ArgumentException("Cannot remove null child");
				}
				bool flag2 = child.hierarchy.parent != this.m_Owner;
				if (flag2)
				{
					throw new ArgumentException("This VisualElement is not my child");
				}
				int num = this.m_Owner.m_Children.IndexOf(child);
				this.RemoveAt(num);
			}

			public void RemoveAt(int index)
			{
				bool flag = this.m_Owner.elementPanel != null && this.m_Owner.elementPanel.duringLayoutPhase;
				if (flag)
				{
					throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
				}
				bool flag2 = index < 0 || index >= this.childCount;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("Index out of range: " + index.ToString());
				}
				VisualElement visualElement = this.m_Owner.m_Children[index];
				BaseVisualElementPanel elementPanel = visualElement.elementPanel;
				RuntimePanel runtimePanel = elementPanel as RuntimePanel;
				bool flag3 = runtimePanel != null && !elementPanel.isFlat;
				if (flag3)
				{
					WorldSpaceDataStore.ClearWorldSpaceData(visualElement);
				}
				visualElement.InvokeHierarchyChanged(HierarchyChangeType.RemovedFromParent, null);
				this.RemoveChildAtIndex(index);
				int num = visualElement.imguiContainerDescendantCount + (visualElement.isIMGUIContainer ? 1 : 0);
				bool flag4 = num > 0;
				if (flag4)
				{
					this.m_Owner.ChangeIMGUIContainerCount(-num);
				}
				visualElement.hierarchy.SetParent(null);
				bool flag5 = this.childCount == 0;
				if (flag5)
				{
					this.ReleaseChildList();
					bool requireMeasureFunction = this.m_Owner.requireMeasureFunction;
					if (requireMeasureFunction)
					{
						this.m_Owner.AssignMeasureFunction();
					}
				}
				BaseVisualElementPanel elementPanel2 = this.m_Owner.elementPanel;
				if (elementPanel2 != null)
				{
					elementPanel2.OnVersionChanged(visualElement, VersionChangeType.Hierarchy);
				}
				this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
				this.m_Owner.OnChildRemoved(visualElement);
			}

			public void Clear()
			{
				bool flag = this.m_Owner.elementPanel != null && this.m_Owner.elementPanel.duringLayoutPhase;
				if (flag)
				{
					throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
				}
				bool flag2 = this.childCount > 0;
				if (flag2)
				{
					List<VisualElement> list = VisualElementListPool.Copy(this.m_Owner.m_Children);
					BaseVisualElementPanel elementPanel = this.m_Owner.elementPanel;
					RuntimePanel runtimePanel = elementPanel as RuntimePanel;
					bool flag3 = runtimePanel != null && !elementPanel.isFlat;
					if (flag3)
					{
						foreach (VisualElement visualElement in this.m_Owner.m_Children)
						{
							WorldSpaceDataStore.ClearWorldSpaceData(visualElement);
						}
					}
					this.ReleaseChildList();
					this.m_Owner.layoutNode.Clear();
					bool requireMeasureFunction = this.m_Owner.requireMeasureFunction;
					if (requireMeasureFunction)
					{
						this.m_Owner.AssignMeasureFunction();
					}
					foreach (VisualElement visualElement2 in list)
					{
						visualElement2.InvokeHierarchyChanged(HierarchyChangeType.RemovedFromParent, null);
						visualElement2.hierarchy.SetParent(null);
						visualElement2.m_LogicalParent = null;
						BaseVisualElementPanel elementPanel2 = this.m_Owner.elementPanel;
						if (elementPanel2 != null)
						{
							elementPanel2.OnVersionChanged(visualElement2, VersionChangeType.Hierarchy);
						}
						this.m_Owner.OnChildRemoved(visualElement2);
					}
					bool flag4 = this.m_Owner.imguiContainerDescendantCount > 0;
					if (flag4)
					{
						int num = this.m_Owner.imguiContainerDescendantCount;
						bool isIMGUIContainer = this.m_Owner.isIMGUIContainer;
						if (isIMGUIContainer)
						{
							num--;
						}
						this.m_Owner.ChangeIMGUIContainerCount(-num);
					}
					VisualElementListPool.Release(list);
					this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
				}
			}

			internal void BringToFront(VisualElement child)
			{
				bool flag = this.childCount > 1;
				if (flag)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					bool flag2 = num >= 0 && num < this.childCount - 1;
					if (flag2)
					{
						this.MoveChildElement(child, num, this.childCount);
					}
				}
			}

			internal void SendToBack(VisualElement child)
			{
				bool flag = this.childCount > 1;
				if (flag)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					bool flag2 = num > 0;
					if (flag2)
					{
						this.MoveChildElement(child, num, 0);
					}
				}
			}

			internal void PlaceBehind(VisualElement child, VisualElement over)
			{
				bool flag = this.childCount > 0;
				if (flag)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					bool flag2 = num < 0;
					if (!flag2)
					{
						int num2 = this.m_Owner.m_Children.IndexOf(over);
						bool flag3 = num2 > 0 && num < num2;
						if (flag3)
						{
							num2--;
						}
						this.MoveChildElement(child, num, num2);
					}
				}
			}

			internal void PlaceInFront(VisualElement child, VisualElement under)
			{
				bool flag = this.childCount > 0;
				if (flag)
				{
					int num = this.m_Owner.m_Children.IndexOf(child);
					bool flag2 = num < 0;
					if (!flag2)
					{
						int num2 = this.m_Owner.m_Children.IndexOf(under);
						bool flag3 = num > num2;
						if (flag3)
						{
							num2++;
						}
						this.MoveChildElement(child, num, num2);
					}
				}
			}

			private void MoveChildElement(VisualElement child, int currentIndex, int nextIndex)
			{
				bool flag = this.m_Owner.elementPanel != null && this.m_Owner.elementPanel.duringLayoutPhase;
				if (flag)
				{
					throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
				}
				child.InvokeHierarchyChanged(HierarchyChangeType.RemovedFromParent, null);
				this.RemoveChildAtIndex(currentIndex);
				this.PutChildAtIndex(child, nextIndex);
				child.InvokeHierarchyChanged(HierarchyChangeType.AddedToParent, null);
				this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
			}

			public int childCount
			{
				get
				{
					return this.m_Owner.m_Children.Count;
				}
			}

			public VisualElement this[int key]
			{
				get
				{
					return this.m_Owner.m_Children[key];
				}
			}

			public int IndexOf(VisualElement element)
			{
				return this.m_Owner.m_Children.IndexOf(element);
			}

			public VisualElement ElementAt(int index)
			{
				return this[index];
			}

			public IEnumerable<VisualElement> Children()
			{
				return this.m_Owner.m_Children;
			}

			private void SetParent(VisualElement value)
			{
				this.m_Owner.m_PhysicalParent = value;
				this.m_Owner.m_LogicalParent = value;
				this.m_Owner.DirtyNextParentWithEventInterests();
				this.m_Owner.SetPanel((value != null) ? value.elementPanel : null);
				bool flag = this.m_Owner.m_PhysicalParent != value;
				if (flag)
				{
					Debug.LogError("Modifying the parent of a VisualElement while it’s already being modified is not allowed and can cause undefined behavior. Did you change the hierarchy during an AttachToPanelEvent or DetachFromPanelEvent?");
				}
			}

			public unsafe void Sort(Comparison<VisualElement> comp)
			{
				bool flag = this.m_Owner.elementPanel != null && this.m_Owner.elementPanel.duringLayoutPhase;
				if (flag)
				{
					throw new InvalidOperationException("Cannot modify VisualElement hierarchy during layout calculation");
				}
				bool flag2 = this.childCount > 1;
				if (flag2)
				{
					this.m_Owner.m_Children.Sort(comp);
					this.m_Owner.layoutNode.Clear();
					for (int i = 0; i < this.m_Owner.m_Children.Count; i++)
					{
						this.m_Owner.layoutNode.Insert(i, *this.m_Owner.m_Children[i].layoutNode);
					}
					this.m_Owner.InvokeHierarchyChanged(HierarchyChangeType.ChildrenReordered, null);
					this.m_Owner.IncrementVersion(VersionChangeType.Hierarchy);
				}
			}

			private unsafe void PutChildAtIndex(VisualElement child, int index)
			{
				bool flag = index >= this.childCount;
				if (flag)
				{
					this.m_Owner.m_Children.Add(child);
					this.m_Owner.layoutNode.Insert(this.m_Owner.layoutNode.Count, *child.layoutNode);
				}
				else
				{
					this.m_Owner.m_Children.Insert(index, child);
					this.m_Owner.layoutNode.Insert(index, *child.layoutNode);
				}
			}

			private void RemoveChildAtIndex(int index)
			{
				this.m_Owner.m_Children.RemoveAt(index);
				this.m_Owner.layoutNode.RemoveAt(index);
			}

			private void ReleaseChildList()
			{
				bool flag = this.m_Owner.m_Children != VisualElement.s_EmptyList;
				if (flag)
				{
					List<VisualElement> children = this.m_Owner.m_Children;
					this.m_Owner.m_Children = VisualElement.s_EmptyList;
					VisualElementListPool.Release(children);
				}
			}

			public bool Equals(VisualElement.Hierarchy other)
			{
				return other == this;
			}

			public override bool Equals(object obj)
			{
				bool flag = obj == null;
				return !flag && obj is VisualElement.Hierarchy && this.Equals((VisualElement.Hierarchy)obj);
			}

			public override int GetHashCode()
			{
				return (this.m_Owner != null) ? this.m_Owner.GetHashCode() : 0;
			}

			public static bool operator ==(VisualElement.Hierarchy x, VisualElement.Hierarchy y)
			{
				return x.m_Owner == y.m_Owner;
			}

			public static bool operator !=(VisualElement.Hierarchy x, VisualElement.Hierarchy y)
			{
				return !(x == y);
			}

			private const string k_InvalidHierarchyChangeMsg = "Cannot modify VisualElement hierarchy during layout calculation";

			private readonly VisualElement m_Owner;
		}

		private abstract class BaseVisualElementScheduledItem : ScheduledItem, IVisualElementScheduledItem
		{
			public VisualElement element { get; private set; }

			public bool isScheduled
			{
				get
				{
					return this.scheduler != null;
				}
			}

			public bool isActive { get; private set; }

			public bool isDetaching { get; private set; }

			protected BaseVisualElementScheduledItem(VisualElement handler)
				: base(handler.TimeSinceStartupMs())
			{
				this.element = handler;
				this.m_OnAttachToPanelCallback = new EventCallback<AttachToPanelEvent>(this.OnElementAttachToPanelCallback);
				this.m_OnDetachFromPanelCallback = new EventCallback<DetachFromPanelEvent>(this.OnElementDetachFromPanelCallback);
			}

			private void SetActive(bool action)
			{
				bool flag = this.isActive != action;
				if (flag)
				{
					this.isActive = action;
					bool isActive = this.isActive;
					if (isActive)
					{
						this.element.RegisterCallback<AttachToPanelEvent>(this.m_OnAttachToPanelCallback, TrickleDown.NoTrickleDown);
						this.element.RegisterCallback<DetachFromPanelEvent>(this.m_OnDetachFromPanelCallback, TrickleDown.NoTrickleDown);
						this.SendActivation();
					}
					else
					{
						this.element.UnregisterCallback<AttachToPanelEvent>(this.m_OnAttachToPanelCallback, TrickleDown.NoTrickleDown);
						this.element.UnregisterCallback<DetachFromPanelEvent>(this.m_OnDetachFromPanelCallback, TrickleDown.NoTrickleDown);
						this.SendDeactivation();
					}
				}
			}

			private void SendActivation()
			{
				bool flag = this.CanBeActivated();
				if (flag)
				{
					this.OnPanelActivate();
				}
			}

			private void SendDeactivation()
			{
				bool flag = this.CanBeActivated();
				if (flag)
				{
					this.OnPanelDeactivate();
				}
			}

			private void OnElementAttachToPanelCallback(AttachToPanelEvent evt)
			{
				bool isActive = this.isActive;
				if (isActive)
				{
					bool flag = this.isScheduled && this.scheduler != this.element.elementPanel.scheduler;
					if (flag)
					{
						this.OnPanelDeactivate();
					}
					this.SendActivation();
				}
			}

			private void OnElementDetachFromPanelCallback(DetachFromPanelEvent evt)
			{
				bool flag = !this.isActive;
				if (!flag)
				{
					this.isDetaching = true;
					try
					{
						this.SendDeactivation();
					}
					finally
					{
						this.isDetaching = false;
					}
				}
			}

			public IVisualElementScheduledItem StartingIn(long delayMs)
			{
				base.delayMs = delayMs;
				return this;
			}

			public IVisualElementScheduledItem Until(Func<bool> stopCondition)
			{
				bool flag = stopCondition == null;
				if (flag)
				{
					stopCondition = ScheduledItem.ForeverCondition;
				}
				this.timerUpdateStopCondition = stopCondition;
				return this;
			}

			public IVisualElementScheduledItem ForDuration(long durationMs)
			{
				base.SetDuration(durationMs);
				return this;
			}

			public IVisualElementScheduledItem Every(long intervalMs)
			{
				base.intervalMs = intervalMs;
				bool flag = this.timerUpdateStopCondition == ScheduledItem.OnceCondition;
				if (flag)
				{
					this.timerUpdateStopCondition = ScheduledItem.ForeverCondition;
				}
				return this;
			}

			internal override void OnItemUnscheduled()
			{
				base.OnItemUnscheduled();
				this.scheduler = null;
				bool flag = !this.isDetaching;
				if (flag)
				{
					this.SetActive(false);
				}
			}

			public void Resume()
			{
				this.SetActive(true);
			}

			public void Pause()
			{
				this.SetActive(false);
			}

			public void ExecuteLater(long delayMs)
			{
				bool flag = !this.isScheduled;
				if (flag)
				{
					this.Resume();
				}
				base.ResetStartTime(this.element.TimeSinceStartupMs());
				this.StartingIn(delayMs);
			}

			public void OnPanelActivate()
			{
				bool flag = !this.isScheduled;
				if (flag)
				{
					base.ResetStartTime(this.element.TimeSinceStartupMs());
					this.scheduler = this.element.elementPanel.scheduler;
					this.scheduler.Schedule(this);
				}
			}

			public void OnPanelDeactivate()
			{
				bool isScheduled = this.isScheduled;
				if (isScheduled)
				{
					TimerEventScheduler timerEventScheduler = this.scheduler;
					this.scheduler = null;
					timerEventScheduler.Unschedule(this);
				}
			}

			public bool CanBeActivated()
			{
				return this.element != null && this.element.elementPanel != null && this.element.elementPanel.scheduler != null;
			}

			public TimerEventScheduler scheduler = null;

			private readonly EventCallback<AttachToPanelEvent> m_OnAttachToPanelCallback;

			private readonly EventCallback<DetachFromPanelEvent> m_OnDetachFromPanelCallback;
		}

		private abstract class VisualElementScheduledItem<ActionType> : VisualElement.BaseVisualElementScheduledItem
		{
			public VisualElementScheduledItem(VisualElement handler, ActionType upEvent)
				: base(handler)
			{
				this.updateEvent = upEvent;
			}

			public static bool Matches(ScheduledItem item, ActionType updateEvent)
			{
				VisualElement.VisualElementScheduledItem<ActionType> visualElementScheduledItem = item as VisualElement.VisualElementScheduledItem<ActionType>;
				bool flag = visualElementScheduledItem != null;
				return flag && EqualityComparer<ActionType>.Default.Equals(visualElementScheduledItem.updateEvent, updateEvent);
			}

			public ActionType updateEvent;
		}

		private class TimerStateScheduledItem : VisualElement.VisualElementScheduledItem<Action<TimerState>>
		{
			public TimerStateScheduledItem(VisualElement handler, Action<TimerState> updateEvent)
				: base(handler, updateEvent)
			{
			}

			public override void PerformTimerUpdate(TimerState state)
			{
				bool isScheduled = base.isScheduled;
				if (isScheduled)
				{
					this.updateEvent(state);
				}
			}
		}

		private class SimpleScheduledItem : VisualElement.VisualElementScheduledItem<Action>
		{
			public SimpleScheduledItem(VisualElement handler, Action updateEvent)
				: base(handler, updateEvent)
			{
			}

			public override void PerformTimerUpdate(TimerState state)
			{
				bool isScheduled = base.isScheduled;
				if (isScheduled)
				{
					this.updateEvent();
				}
			}
		}

		internal class CustomStyleAccess : ICustomStyle
		{
			public void SetContext(Dictionary<string, StylePropertyValue> customProperties, float dpiScaling)
			{
				this.m_CustomProperties = customProperties;
				this.m_DpiScaling = dpiScaling;
			}

			public bool TryGetValue(CustomStyleProperty<float> property, out float value)
			{
				StylePropertyValue stylePropertyValue;
				bool flag = this.TryGetValue(property.name, StyleValueType.Float, out stylePropertyValue);
				if (flag)
				{
					bool flag2 = stylePropertyValue.sheet.TryReadFloat(stylePropertyValue.handle, out value);
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
				StylePropertyValue stylePropertyValue;
				bool flag = this.TryGetValue(property.name, StyleValueType.Float, out stylePropertyValue);
				if (flag)
				{
					float num;
					bool flag2 = stylePropertyValue.sheet.TryReadFloat(stylePropertyValue.handle, out num);
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
				StylePropertyValue stylePropertyValue;
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out stylePropertyValue);
				bool flag2;
				if (flag)
				{
					value = stylePropertyValue.sheet.ReadKeyword(stylePropertyValue.handle) == StyleValueKeyword.True;
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
				StylePropertyValue stylePropertyValue;
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out stylePropertyValue);
				if (flag)
				{
					StyleValueHandle handle = stylePropertyValue.handle;
					StyleValueType valueType = handle.valueType;
					StyleValueType styleValueType = valueType;
					if (styleValueType == StyleValueType.Color || styleValueType == StyleValueType.Enum)
					{
						return stylePropertyValue.sheet.TryReadColor(stylePropertyValue.handle, out value);
					}
					VisualElement.CustomStyleAccess.LogCustomPropertyWarning(property.name, StyleValueType.Color, stylePropertyValue);
				}
				value = Color.clear;
				return false;
			}

			public bool TryGetValue(CustomStyleProperty<Texture2D> property, out Texture2D value)
			{
				StylePropertyValue stylePropertyValue;
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out stylePropertyValue);
				if (flag)
				{
					ImageSource imageSource = default(ImageSource);
					bool flag2 = StylePropertyReader.TryGetImageSourceFromValue(stylePropertyValue, this.m_DpiScaling, out imageSource) && imageSource.texture != null;
					if (flag2)
					{
						value = imageSource.texture;
						return true;
					}
				}
				value = null;
				return false;
			}

			public bool TryGetValue(CustomStyleProperty<Sprite> property, out Sprite value)
			{
				StylePropertyValue stylePropertyValue;
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out stylePropertyValue);
				if (flag)
				{
					ImageSource imageSource = default(ImageSource);
					bool flag2 = StylePropertyReader.TryGetImageSourceFromValue(stylePropertyValue, this.m_DpiScaling, out imageSource) && imageSource.sprite != null;
					if (flag2)
					{
						value = imageSource.sprite;
						return true;
					}
				}
				value = null;
				return false;
			}

			public bool TryGetValue(CustomStyleProperty<VectorImage> property, out VectorImage value)
			{
				StylePropertyValue stylePropertyValue;
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out stylePropertyValue);
				if (flag)
				{
					ImageSource imageSource = default(ImageSource);
					bool flag2 = StylePropertyReader.TryGetImageSourceFromValue(stylePropertyValue, this.m_DpiScaling, out imageSource) && imageSource.vectorImage != null;
					if (flag2)
					{
						value = imageSource.vectorImage;
						return true;
					}
				}
				value = null;
				return false;
			}

			public bool TryGetValue<T>(CustomStyleProperty<T> property, out T value) where T : Object
			{
				StylePropertyValue stylePropertyValue;
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out stylePropertyValue);
				if (flag)
				{
					Object @object;
					bool flag2 = stylePropertyValue.sheet.TryReadAssetReference(stylePropertyValue.handle, out @object);
					if (flag2)
					{
						value = @object as T;
						return value != null;
					}
				}
				value = default(T);
				return false;
			}

			public bool TryGetValue(CustomStyleProperty<string> property, out string value)
			{
				StylePropertyValue stylePropertyValue;
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(property.name, out stylePropertyValue);
				bool flag2;
				if (flag)
				{
					value = stylePropertyValue.sheet.ReadAsString(stylePropertyValue.handle);
					flag2 = true;
				}
				else
				{
					value = string.Empty;
					flag2 = false;
				}
				return flag2;
			}

			private bool TryGetValue(string propertyName, StyleValueType valueType, out StylePropertyValue customProp)
			{
				customProp = default(StylePropertyValue);
				bool flag = this.m_CustomProperties != null && this.m_CustomProperties.TryGetValue(propertyName, out customProp);
				bool flag3;
				if (flag)
				{
					StyleValueHandle handle = customProp.handle;
					bool flag2 = handle.valueType != valueType;
					if (flag2)
					{
						VisualElement.CustomStyleAccess.LogCustomPropertyWarning(propertyName, valueType, customProp);
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

			private static void LogCustomPropertyWarning(string propertyName, StyleValueType valueType, StylePropertyValue customProp)
			{
				Debug.LogWarning(string.Format("Trying to read custom property {0} value as {1} while parsed type is {2}", propertyName, valueType, customProp.handle.valueType));
			}

			private Dictionary<string, StylePropertyValue> m_CustomProperties;

			private float m_DpiScaling;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal class TypeData
		{
			public Type type { get; }

			public TypeData(Type type)
			{
				this.type = type;
			}

			public string fullTypeName
			{
				get
				{
					bool flag = string.IsNullOrEmpty(this.m_FullTypeName);
					if (flag)
					{
						this.m_FullTypeName = this.type.FullName;
					}
					return this.m_FullTypeName;
				}
			}

			public string typeName
			{
				get
				{
					bool flag = string.IsNullOrEmpty(this.m_TypeName);
					if (flag)
					{
						bool isGenericType = this.type.IsGenericType;
						this.m_TypeName = this.type.Name;
						bool flag2 = isGenericType;
						if (flag2)
						{
							int num = this.m_TypeName.IndexOf('`');
							bool flag3 = num >= 0;
							if (flag3)
							{
								this.m_TypeName = this.m_TypeName.Remove(num);
							}
						}
					}
					return this.m_TypeName;
				}
			}

			private string m_FullTypeName = string.Empty;

			private string m_TypeName = string.Empty;
		}
	}
}
