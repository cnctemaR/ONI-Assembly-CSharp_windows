using System;
using System.Collections.Generic;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	internal class ResolvedStyleAccessPropertyBag : PropertyBag<ResolvedStyleAccess>, INamedProperties<ResolvedStyleAccess>
	{
		public ResolvedStyleAccessPropertyBag()
		{
			this.m_PropertiesList = new List<IProperty<ResolvedStyleAccess>>(83);
			this.m_PropertiesHash = new Dictionary<string, IProperty<ResolvedStyleAccess>>(249);
			this.AddProperty<Align>(new ResolvedStyleAccessPropertyBag.AlignContentProperty());
			this.AddProperty<Align>(new ResolvedStyleAccessPropertyBag.AlignItemsProperty());
			this.AddProperty<Align>(new ResolvedStyleAccessPropertyBag.AlignSelfProperty());
			this.AddProperty<Ratio>(new ResolvedStyleAccessPropertyBag.AspectRatioProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.BackgroundColorProperty());
			this.AddProperty<Background>(new ResolvedStyleAccessPropertyBag.BackgroundImageProperty());
			this.AddProperty<BackgroundPosition>(new ResolvedStyleAccessPropertyBag.BackgroundPositionXProperty());
			this.AddProperty<BackgroundPosition>(new ResolvedStyleAccessPropertyBag.BackgroundPositionYProperty());
			this.AddProperty<BackgroundRepeat>(new ResolvedStyleAccessPropertyBag.BackgroundRepeatProperty());
			this.AddProperty<BackgroundSize>(new ResolvedStyleAccessPropertyBag.BackgroundSizeProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.BorderBottomColorProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderBottomLeftRadiusProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderBottomRightRadiusProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderBottomWidthProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.BorderLeftColorProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderLeftWidthProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.BorderRightColorProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderRightWidthProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.BorderTopColorProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderTopLeftRadiusProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderTopRightRadiusProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BorderTopWidthProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.BottomProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.ColorProperty());
			this.AddProperty<DisplayStyle>(new ResolvedStyleAccessPropertyBag.DisplayProperty());
			this.AddProperty<IEnumerable<FilterFunction>>(new ResolvedStyleAccessPropertyBag.FilterProperty());
			this.AddProperty<StyleFloat>(new ResolvedStyleAccessPropertyBag.FlexBasisProperty());
			this.AddProperty<FlexDirection>(new ResolvedStyleAccessPropertyBag.FlexDirectionProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.FlexGrowProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.FlexShrinkProperty());
			this.AddProperty<Wrap>(new ResolvedStyleAccessPropertyBag.FlexWrapProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.FontSizeProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.HeightProperty());
			this.AddProperty<Justify>(new ResolvedStyleAccessPropertyBag.JustifyContentProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.LeftProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.LetterSpacingProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.MarginBottomProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.MarginLeftProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.MarginRightProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.MarginTopProperty());
			this.AddProperty<StyleFloat>(new ResolvedStyleAccessPropertyBag.MaxHeightProperty());
			this.AddProperty<StyleFloat>(new ResolvedStyleAccessPropertyBag.MaxWidthProperty());
			this.AddProperty<StyleFloat>(new ResolvedStyleAccessPropertyBag.MinHeightProperty());
			this.AddProperty<StyleFloat>(new ResolvedStyleAccessPropertyBag.MinWidthProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.OpacityProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.PaddingBottomProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.PaddingLeftProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.PaddingRightProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.PaddingTopProperty());
			this.AddProperty<Position>(new ResolvedStyleAccessPropertyBag.PositionProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.RightProperty());
			this.AddProperty<Rotate>(new ResolvedStyleAccessPropertyBag.RotateProperty());
			this.AddProperty<Scale>(new ResolvedStyleAccessPropertyBag.ScaleProperty());
			this.AddProperty<TextOverflow>(new ResolvedStyleAccessPropertyBag.TextOverflowProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.TopProperty());
			this.AddProperty<Vector3>(new ResolvedStyleAccessPropertyBag.TransformOriginProperty());
			this.AddProperty<IEnumerable<TimeValue>>(new ResolvedStyleAccessPropertyBag.TransitionDelayProperty());
			this.AddProperty<IEnumerable<TimeValue>>(new ResolvedStyleAccessPropertyBag.TransitionDurationProperty());
			this.AddProperty<IEnumerable<StylePropertyName>>(new ResolvedStyleAccessPropertyBag.TransitionPropertyProperty());
			this.AddProperty<IEnumerable<EasingFunction>>(new ResolvedStyleAccessPropertyBag.TransitionTimingFunctionProperty());
			this.AddProperty<Vector3>(new ResolvedStyleAccessPropertyBag.TranslateProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.UnityBackgroundImageTintColorProperty());
			this.AddProperty<EditorTextRenderingMode>(new ResolvedStyleAccessPropertyBag.UnityEditorTextRenderingModeProperty());
			this.AddProperty<Font>(new ResolvedStyleAccessPropertyBag.UnityFontProperty());
			this.AddProperty<FontDefinition>(new ResolvedStyleAccessPropertyBag.UnityFontDefinitionProperty());
			this.AddProperty<FontStyle>(new ResolvedStyleAccessPropertyBag.UnityFontStyleAndWeightProperty());
			this.AddProperty<MaterialDefinition>(new ResolvedStyleAccessPropertyBag.UnityMaterialProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.UnityParagraphSpacingProperty());
			this.AddProperty<int>(new ResolvedStyleAccessPropertyBag.UnitySliceBottomProperty());
			this.AddProperty<int>(new ResolvedStyleAccessPropertyBag.UnitySliceLeftProperty());
			this.AddProperty<int>(new ResolvedStyleAccessPropertyBag.UnitySliceRightProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.UnitySliceScaleProperty());
			this.AddProperty<int>(new ResolvedStyleAccessPropertyBag.UnitySliceTopProperty());
			this.AddProperty<SliceType>(new ResolvedStyleAccessPropertyBag.UnitySliceTypeProperty());
			this.AddProperty<TextAnchor>(new ResolvedStyleAccessPropertyBag.UnityTextAlignProperty());
			this.AddProperty<TextGeneratorType>(new ResolvedStyleAccessPropertyBag.UnityTextGeneratorProperty());
			this.AddProperty<Color>(new ResolvedStyleAccessPropertyBag.UnityTextOutlineColorProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.UnityTextOutlineWidthProperty());
			this.AddProperty<TextOverflowPosition>(new ResolvedStyleAccessPropertyBag.UnityTextOverflowPositionProperty());
			this.AddProperty<Visibility>(new ResolvedStyleAccessPropertyBag.VisibilityProperty());
			this.AddProperty<WhiteSpace>(new ResolvedStyleAccessPropertyBag.WhiteSpaceProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.WidthProperty());
			this.AddProperty<float>(new ResolvedStyleAccessPropertyBag.WordSpacingProperty());
		}

		private void AddProperty<TValue>(ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<TValue> property)
		{
			this.m_PropertiesList.Add(property);
			this.m_PropertiesHash.Add(property.Name, property);
			bool flag = string.CompareOrdinal(property.Name, property.ussName) != 0;
			if (flag)
			{
				this.m_PropertiesHash.Add(property.ussName, property);
			}
		}

		public override PropertyCollection<ResolvedStyleAccess> GetProperties()
		{
			return new PropertyCollection<ResolvedStyleAccess>(this.m_PropertiesList);
		}

		public override PropertyCollection<ResolvedStyleAccess> GetProperties(ref ResolvedStyleAccess container)
		{
			return new PropertyCollection<ResolvedStyleAccess>(this.m_PropertiesList);
		}

		public bool TryGetProperty(ref ResolvedStyleAccess container, string name, out IProperty<ResolvedStyleAccess> property)
		{
			return this.m_PropertiesHash.TryGetValue(name, out property);
		}

		private readonly List<IProperty<ResolvedStyleAccess>> m_PropertiesList;

		private readonly Dictionary<string, IProperty<ResolvedStyleAccess>> m_PropertiesHash;

		private class AlignContentProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<Align>
		{
			public override string Name
			{
				get
				{
					return "alignContent";
				}
			}

			public override string ussName
			{
				get
				{
					return "align-content";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Align GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).alignContent;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Align value)
			{
				throw new InvalidOperationException();
			}
		}

		private class AlignItemsProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<Align>
		{
			public override string Name
			{
				get
				{
					return "alignItems";
				}
			}

			public override string ussName
			{
				get
				{
					return "align-items";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Align GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).alignItems;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Align value)
			{
				throw new InvalidOperationException();
			}
		}

		private class AlignSelfProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<Align>
		{
			public override string Name
			{
				get
				{
					return "alignSelf";
				}
			}

			public override string ussName
			{
				get
				{
					return "align-self";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Align GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).alignSelf;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Align value)
			{
				throw new InvalidOperationException();
			}
		}

		private class AspectRatioProperty : ResolvedStyleAccessPropertyBag.ResolvedRatioProperty
		{
			public override string Name
			{
				get
				{
					return "aspectRatio";
				}
			}

			public override string ussName
			{
				get
				{
					return "aspect-ratio";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Ratio GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).aspectRatio;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Ratio value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BackgroundColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "backgroundColor";
				}
			}

			public override string ussName
			{
				get
				{
					return "background-color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).backgroundColor;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BackgroundImageProperty : ResolvedStyleAccessPropertyBag.ResolvedBackgroundProperty
		{
			public override string Name
			{
				get
				{
					return "backgroundImage";
				}
			}

			public override string ussName
			{
				get
				{
					return "background-image";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Background GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).backgroundImage;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Background value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BackgroundPositionXProperty : ResolvedStyleAccessPropertyBag.ResolvedBackgroundPositionProperty
		{
			public override string Name
			{
				get
				{
					return "backgroundPositionX";
				}
			}

			public override string ussName
			{
				get
				{
					return "background-position-x";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override BackgroundPosition GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).backgroundPositionX;
			}

			public override void SetValue(ref ResolvedStyleAccess container, BackgroundPosition value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BackgroundPositionYProperty : ResolvedStyleAccessPropertyBag.ResolvedBackgroundPositionProperty
		{
			public override string Name
			{
				get
				{
					return "backgroundPositionY";
				}
			}

			public override string ussName
			{
				get
				{
					return "background-position-y";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override BackgroundPosition GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).backgroundPositionY;
			}

			public override void SetValue(ref ResolvedStyleAccess container, BackgroundPosition value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BackgroundRepeatProperty : ResolvedStyleAccessPropertyBag.ResolvedBackgroundRepeatProperty
		{
			public override string Name
			{
				get
				{
					return "backgroundRepeat";
				}
			}

			public override string ussName
			{
				get
				{
					return "background-repeat";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override BackgroundRepeat GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).backgroundRepeat;
			}

			public override void SetValue(ref ResolvedStyleAccess container, BackgroundRepeat value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BackgroundSizeProperty : ResolvedStyleAccessPropertyBag.ResolvedBackgroundSizeProperty
		{
			public override string Name
			{
				get
				{
					return "backgroundSize";
				}
			}

			public override string ussName
			{
				get
				{
					return "background-size";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override BackgroundSize GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).backgroundSize;
			}

			public override void SetValue(ref ResolvedStyleAccess container, BackgroundSize value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderBottomColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "borderBottomColor";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-bottom-color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderBottomColor;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderBottomLeftRadiusProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderBottomLeftRadius";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-bottom-left-radius";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderBottomLeftRadius;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderBottomRightRadiusProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderBottomRightRadius";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-bottom-right-radius";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderBottomRightRadius;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderBottomWidthProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderBottomWidth";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-bottom-width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderBottomWidth;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderLeftColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "borderLeftColor";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-left-color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderLeftColor;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderLeftWidthProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderLeftWidth";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-left-width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderLeftWidth;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderRightColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "borderRightColor";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-right-color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderRightColor;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderRightWidthProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderRightWidth";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-right-width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderRightWidth;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderTopColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "borderTopColor";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-top-color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderTopColor;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderTopLeftRadiusProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderTopLeftRadius";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-top-left-radius";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderTopLeftRadius;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderTopRightRadiusProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderTopRightRadius";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-top-right-radius";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderTopRightRadius;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BorderTopWidthProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "borderTopWidth";
				}
			}

			public override string ussName
			{
				get
				{
					return "border-top-width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).borderTopWidth;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class BottomProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "bottom";
				}
			}

			public override string ussName
			{
				get
				{
					return "bottom";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).bottom;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class ColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "color";
				}
			}

			public override string ussName
			{
				get
				{
					return "color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).color;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class DisplayProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<DisplayStyle>
		{
			public override string Name
			{
				get
				{
					return "display";
				}
			}

			public override string ussName
			{
				get
				{
					return "display";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override DisplayStyle GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).display;
			}

			public override void SetValue(ref ResolvedStyleAccess container, DisplayStyle value)
			{
				throw new InvalidOperationException();
			}
		}

		private class FilterProperty : ResolvedStyleAccessPropertyBag.ResolvedListProperty<FilterFunction>
		{
			public override string Name
			{
				get
				{
					return "filter";
				}
			}

			public override string ussName
			{
				get
				{
					return "filter";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override IEnumerable<FilterFunction> GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).filter;
			}

			public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<FilterFunction> value)
			{
				throw new InvalidOperationException();
			}
		}

		private class FlexBasisProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleFloatProperty
		{
			public override string Name
			{
				get
				{
					return "flexBasis";
				}
			}

			public override string ussName
			{
				get
				{
					return "flex-basis";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override StyleFloat GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).flexBasis;
			}

			public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
			{
				throw new InvalidOperationException();
			}
		}

		private class FlexDirectionProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<FlexDirection>
		{
			public override string Name
			{
				get
				{
					return "flexDirection";
				}
			}

			public override string ussName
			{
				get
				{
					return "flex-direction";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override FlexDirection GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).flexDirection;
			}

			public override void SetValue(ref ResolvedStyleAccess container, FlexDirection value)
			{
				throw new InvalidOperationException();
			}
		}

		private class FlexGrowProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "flexGrow";
				}
			}

			public override string ussName
			{
				get
				{
					return "flex-grow";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).flexGrow;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class FlexShrinkProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "flexShrink";
				}
			}

			public override string ussName
			{
				get
				{
					return "flex-shrink";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).flexShrink;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class FlexWrapProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<Wrap>
		{
			public override string Name
			{
				get
				{
					return "flexWrap";
				}
			}

			public override string ussName
			{
				get
				{
					return "flex-wrap";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Wrap GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).flexWrap;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Wrap value)
			{
				throw new InvalidOperationException();
			}
		}

		private class FontSizeProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "fontSize";
				}
			}

			public override string ussName
			{
				get
				{
					return "font-size";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).fontSize;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class HeightProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "height";
				}
			}

			public override string ussName
			{
				get
				{
					return "height";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).height;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class JustifyContentProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<Justify>
		{
			public override string Name
			{
				get
				{
					return "justifyContent";
				}
			}

			public override string ussName
			{
				get
				{
					return "justify-content";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Justify GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).justifyContent;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Justify value)
			{
				throw new InvalidOperationException();
			}
		}

		private class LeftProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "left";
				}
			}

			public override string ussName
			{
				get
				{
					return "left";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).left;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class LetterSpacingProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "letterSpacing";
				}
			}

			public override string ussName
			{
				get
				{
					return "letter-spacing";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).letterSpacing;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MarginBottomProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "marginBottom";
				}
			}

			public override string ussName
			{
				get
				{
					return "margin-bottom";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).marginBottom;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MarginLeftProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "marginLeft";
				}
			}

			public override string ussName
			{
				get
				{
					return "margin-left";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).marginLeft;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MarginRightProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "marginRight";
				}
			}

			public override string ussName
			{
				get
				{
					return "margin-right";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).marginRight;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MarginTopProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "marginTop";
				}
			}

			public override string ussName
			{
				get
				{
					return "margin-top";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).marginTop;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MaxHeightProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleFloatProperty
		{
			public override string Name
			{
				get
				{
					return "maxHeight";
				}
			}

			public override string ussName
			{
				get
				{
					return "max-height";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override StyleFloat GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).maxHeight;
			}

			public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MaxWidthProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleFloatProperty
		{
			public override string Name
			{
				get
				{
					return "maxWidth";
				}
			}

			public override string ussName
			{
				get
				{
					return "max-width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override StyleFloat GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).maxWidth;
			}

			public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MinHeightProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleFloatProperty
		{
			public override string Name
			{
				get
				{
					return "minHeight";
				}
			}

			public override string ussName
			{
				get
				{
					return "min-height";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override StyleFloat GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).minHeight;
			}

			public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
			{
				throw new InvalidOperationException();
			}
		}

		private class MinWidthProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleFloatProperty
		{
			public override string Name
			{
				get
				{
					return "minWidth";
				}
			}

			public override string ussName
			{
				get
				{
					return "min-width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override StyleFloat GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).minWidth;
			}

			public override void SetValue(ref ResolvedStyleAccess container, StyleFloat value)
			{
				throw new InvalidOperationException();
			}
		}

		private class OpacityProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "opacity";
				}
			}

			public override string ussName
			{
				get
				{
					return "opacity";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).opacity;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class PaddingBottomProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "paddingBottom";
				}
			}

			public override string ussName
			{
				get
				{
					return "padding-bottom";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).paddingBottom;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class PaddingLeftProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "paddingLeft";
				}
			}

			public override string ussName
			{
				get
				{
					return "padding-left";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).paddingLeft;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class PaddingRightProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "paddingRight";
				}
			}

			public override string ussName
			{
				get
				{
					return "padding-right";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).paddingRight;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class PaddingTopProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "paddingTop";
				}
			}

			public override string ussName
			{
				get
				{
					return "padding-top";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).paddingTop;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class PositionProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<Position>
		{
			public override string Name
			{
				get
				{
					return "position";
				}
			}

			public override string ussName
			{
				get
				{
					return "position";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Position GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).position;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Position value)
			{
				throw new InvalidOperationException();
			}
		}

		private class RightProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "right";
				}
			}

			public override string ussName
			{
				get
				{
					return "right";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).right;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class RotateProperty : ResolvedStyleAccessPropertyBag.ResolvedRotateProperty
		{
			public override string Name
			{
				get
				{
					return "rotate";
				}
			}

			public override string ussName
			{
				get
				{
					return "rotate";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Rotate GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).rotate;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Rotate value)
			{
				throw new InvalidOperationException();
			}
		}

		private class ScaleProperty : ResolvedStyleAccessPropertyBag.ResolvedScaleProperty
		{
			public override string Name
			{
				get
				{
					return "scale";
				}
			}

			public override string ussName
			{
				get
				{
					return "scale";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Scale GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).scale;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Scale value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TextOverflowProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<TextOverflow>
		{
			public override string Name
			{
				get
				{
					return "textOverflow";
				}
			}

			public override string ussName
			{
				get
				{
					return "text-overflow";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override TextOverflow GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).textOverflow;
			}

			public override void SetValue(ref ResolvedStyleAccess container, TextOverflow value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TopProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "top";
				}
			}

			public override string ussName
			{
				get
				{
					return "top";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).top;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TransformOriginProperty : ResolvedStyleAccessPropertyBag.ResolvedVector3Property
		{
			public override string Name
			{
				get
				{
					return "transformOrigin";
				}
			}

			public override string ussName
			{
				get
				{
					return "transform-origin";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Vector3 GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).transformOrigin;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Vector3 value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TransitionDelayProperty : ResolvedStyleAccessPropertyBag.ResolvedListProperty<TimeValue>
		{
			public override string Name
			{
				get
				{
					return "transitionDelay";
				}
			}

			public override string ussName
			{
				get
				{
					return "transition-delay";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override IEnumerable<TimeValue> GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).transitionDelay;
			}

			public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<TimeValue> value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TransitionDurationProperty : ResolvedStyleAccessPropertyBag.ResolvedListProperty<TimeValue>
		{
			public override string Name
			{
				get
				{
					return "transitionDuration";
				}
			}

			public override string ussName
			{
				get
				{
					return "transition-duration";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override IEnumerable<TimeValue> GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).transitionDuration;
			}

			public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<TimeValue> value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TransitionPropertyProperty : ResolvedStyleAccessPropertyBag.ResolvedListProperty<StylePropertyName>
		{
			public override string Name
			{
				get
				{
					return "transitionProperty";
				}
			}

			public override string ussName
			{
				get
				{
					return "transition-property";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override IEnumerable<StylePropertyName> GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).transitionProperty;
			}

			public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<StylePropertyName> value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TransitionTimingFunctionProperty : ResolvedStyleAccessPropertyBag.ResolvedListProperty<EasingFunction>
		{
			public override string Name
			{
				get
				{
					return "transitionTimingFunction";
				}
			}

			public override string ussName
			{
				get
				{
					return "transition-timing-function";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override IEnumerable<EasingFunction> GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).transitionTimingFunction;
			}

			public override void SetValue(ref ResolvedStyleAccess container, IEnumerable<EasingFunction> value)
			{
				throw new InvalidOperationException();
			}
		}

		private class TranslateProperty : ResolvedStyleAccessPropertyBag.ResolvedVector3Property
		{
			public override string Name
			{
				get
				{
					return "translate";
				}
			}

			public override string ussName
			{
				get
				{
					return "translate";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Vector3 GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).translate;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Vector3 value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityBackgroundImageTintColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "unityBackgroundImageTintColor";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-background-image-tint-color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityBackgroundImageTintColor;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityEditorTextRenderingModeProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<EditorTextRenderingMode>
		{
			public override string Name
			{
				get
				{
					return "unityEditorTextRenderingMode";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-editor-text-rendering-mode";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override EditorTextRenderingMode GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityEditorTextRenderingMode;
			}

			public override void SetValue(ref ResolvedStyleAccess container, EditorTextRenderingMode value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityFontProperty : ResolvedStyleAccessPropertyBag.ResolvedFontProperty
		{
			public override string Name
			{
				get
				{
					return "unityFont";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-font";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Font GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityFont;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Font value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityFontDefinitionProperty : ResolvedStyleAccessPropertyBag.ResolvedFontDefinitionProperty
		{
			public override string Name
			{
				get
				{
					return "unityFontDefinition";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-font-definition";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override FontDefinition GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityFontDefinition;
			}

			public override void SetValue(ref ResolvedStyleAccess container, FontDefinition value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityFontStyleAndWeightProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<FontStyle>
		{
			public override string Name
			{
				get
				{
					return "unityFontStyleAndWeight";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-font-style";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override FontStyle GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityFontStyleAndWeight;
			}

			public override void SetValue(ref ResolvedStyleAccess container, FontStyle value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityMaterialProperty : ResolvedStyleAccessPropertyBag.ResolvedMaterialDefinitionProperty
		{
			public override string Name
			{
				get
				{
					return "unityMaterial";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-material";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override MaterialDefinition GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityMaterial;
			}

			public override void SetValue(ref ResolvedStyleAccess container, MaterialDefinition value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityParagraphSpacingProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "unityParagraphSpacing";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-paragraph-spacing";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityParagraphSpacing;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnitySliceBottomProperty : ResolvedStyleAccessPropertyBag.ResolvedIntProperty
		{
			public override string Name
			{
				get
				{
					return "unitySliceBottom";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-slice-bottom";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unitySliceBottom;
			}

			public override void SetValue(ref ResolvedStyleAccess container, int value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnitySliceLeftProperty : ResolvedStyleAccessPropertyBag.ResolvedIntProperty
		{
			public override string Name
			{
				get
				{
					return "unitySliceLeft";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-slice-left";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unitySliceLeft;
			}

			public override void SetValue(ref ResolvedStyleAccess container, int value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnitySliceRightProperty : ResolvedStyleAccessPropertyBag.ResolvedIntProperty
		{
			public override string Name
			{
				get
				{
					return "unitySliceRight";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-slice-right";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unitySliceRight;
			}

			public override void SetValue(ref ResolvedStyleAccess container, int value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnitySliceScaleProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "unitySliceScale";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-slice-scale";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unitySliceScale;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnitySliceTopProperty : ResolvedStyleAccessPropertyBag.ResolvedIntProperty
		{
			public override string Name
			{
				get
				{
					return "unitySliceTop";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-slice-top";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override int GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unitySliceTop;
			}

			public override void SetValue(ref ResolvedStyleAccess container, int value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnitySliceTypeProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<SliceType>
		{
			public override string Name
			{
				get
				{
					return "unitySliceType";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-slice-type";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override SliceType GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unitySliceType;
			}

			public override void SetValue(ref ResolvedStyleAccess container, SliceType value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityTextAlignProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<TextAnchor>
		{
			public override string Name
			{
				get
				{
					return "unityTextAlign";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-text-align";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override TextAnchor GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityTextAlign;
			}

			public override void SetValue(ref ResolvedStyleAccess container, TextAnchor value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityTextGeneratorProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<TextGeneratorType>
		{
			public override string Name
			{
				get
				{
					return "unityTextGenerator";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-text-generator";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override TextGeneratorType GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityTextGenerator;
			}

			public override void SetValue(ref ResolvedStyleAccess container, TextGeneratorType value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityTextOutlineColorProperty : ResolvedStyleAccessPropertyBag.ResolvedColorProperty
		{
			public override string Name
			{
				get
				{
					return "unityTextOutlineColor";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-text-outline-color";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Color GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityTextOutlineColor;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Color value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityTextOutlineWidthProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "unityTextOutlineWidth";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-text-outline-width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityTextOutlineWidth;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class UnityTextOverflowPositionProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<TextOverflowPosition>
		{
			public override string Name
			{
				get
				{
					return "unityTextOverflowPosition";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-text-overflow-position";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override TextOverflowPosition GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).unityTextOverflowPosition;
			}

			public override void SetValue(ref ResolvedStyleAccess container, TextOverflowPosition value)
			{
				throw new InvalidOperationException();
			}
		}

		private class VisibilityProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<Visibility>
		{
			public override string Name
			{
				get
				{
					return "visibility";
				}
			}

			public override string ussName
			{
				get
				{
					return "visibility";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Visibility GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).visibility;
			}

			public override void SetValue(ref ResolvedStyleAccess container, Visibility value)
			{
				throw new InvalidOperationException();
			}
		}

		private class WhiteSpaceProperty : ResolvedStyleAccessPropertyBag.ResolvedEnumProperty<WhiteSpace>
		{
			public override string Name
			{
				get
				{
					return "whiteSpace";
				}
			}

			public override string ussName
			{
				get
				{
					return "white-space";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override WhiteSpace GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).whiteSpace;
			}

			public override void SetValue(ref ResolvedStyleAccess container, WhiteSpace value)
			{
				throw new InvalidOperationException();
			}
		}

		private class WidthProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "width";
				}
			}

			public override string ussName
			{
				get
				{
					return "width";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).width;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private class WordSpacingProperty : ResolvedStyleAccessPropertyBag.ResolvedFloatProperty
		{
			public override string Name
			{
				get
				{
					return "wordSpacing";
				}
			}

			public override string ussName
			{
				get
				{
					return "word-spacing";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override float GetValue(ref ResolvedStyleAccess container)
			{
				return ((IResolvedStyle)container).wordSpacing;
			}

			public override void SetValue(ref ResolvedStyleAccess container, float value)
			{
				throw new InvalidOperationException();
			}
		}

		private abstract class ResolvedStyleProperty<TValue> : Property<ResolvedStyleAccess, TValue>
		{
			public abstract string ussName { get; }
		}

		private abstract class ResolvedEnumProperty<TValue> : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<TValue> where TValue : struct, IConvertible
		{
		}

		private abstract class ResolvedColorProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<Color>
		{
		}

		private abstract class ResolvedBackgroundProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<Background>
		{
		}

		private abstract class ResolvedFloatProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<float>
		{
		}

		private abstract class ResolvedStyleFloatProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<StyleFloat>
		{
		}

		private abstract class ResolvedListProperty<T> : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<IEnumerable<T>>
		{
		}

		private abstract class ResolvedFixedList4Property<T> : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<IEnumerable<T>>
		{
		}

		private abstract class ResolvedFontProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<Font>
		{
		}

		private abstract class ResolvedFontDefinitionProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<FontDefinition>
		{
		}

		private abstract class ResolvedIntProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<int>
		{
		}

		private abstract class ResolvedRotateProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<Rotate>
		{
		}

		private abstract class ResolvedScaleProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<Scale>
		{
		}

		private abstract class ResolvedVector3Property : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<Vector3>
		{
		}

		private abstract class ResolvedBackgroundPositionProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<BackgroundPosition>
		{
		}

		private abstract class ResolvedBackgroundRepeatProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<BackgroundRepeat>
		{
		}

		private abstract class ResolvedBackgroundSizeProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<BackgroundSize>
		{
		}

		private abstract class ResolvedMaterialDefinitionProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<MaterialDefinition>
		{
		}

		private abstract class ResolvedRatioProperty : ResolvedStyleAccessPropertyBag.ResolvedStyleProperty<Ratio>
		{
		}
	}
}
