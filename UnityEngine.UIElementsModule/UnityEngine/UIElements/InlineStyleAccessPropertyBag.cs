using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	internal class InlineStyleAccessPropertyBag : PropertyBag<InlineStyleAccess>, INamedProperties<InlineStyleAccess>
	{
		public InlineStyleAccessPropertyBag()
		{
			this.m_PropertiesList = new List<IProperty<InlineStyleAccess>>(88);
			this.m_PropertiesHash = new Dictionary<string, IProperty<InlineStyleAccess>>(264);
			this.AddProperty<StyleEnum<Align>, Align>(new InlineStyleAccessPropertyBag.AlignContentProperty());
			this.AddProperty<StyleEnum<Align>, Align>(new InlineStyleAccessPropertyBag.AlignItemsProperty());
			this.AddProperty<StyleEnum<Align>, Align>(new InlineStyleAccessPropertyBag.AlignSelfProperty());
			this.AddProperty<StyleRatio, Ratio>(new InlineStyleAccessPropertyBag.AspectRatioProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.BackgroundColorProperty());
			this.AddProperty<StyleBackground, Background>(new InlineStyleAccessPropertyBag.BackgroundImageProperty());
			this.AddProperty<StyleBackgroundPosition, BackgroundPosition>(new InlineStyleAccessPropertyBag.BackgroundPositionXProperty());
			this.AddProperty<StyleBackgroundPosition, BackgroundPosition>(new InlineStyleAccessPropertyBag.BackgroundPositionYProperty());
			this.AddProperty<StyleBackgroundRepeat, BackgroundRepeat>(new InlineStyleAccessPropertyBag.BackgroundRepeatProperty());
			this.AddProperty<StyleBackgroundSize, BackgroundSize>(new InlineStyleAccessPropertyBag.BackgroundSizeProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.BorderBottomColorProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.BorderBottomLeftRadiusProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.BorderBottomRightRadiusProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.BorderBottomWidthProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.BorderLeftColorProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.BorderLeftWidthProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.BorderRightColorProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.BorderRightWidthProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.BorderTopColorProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.BorderTopLeftRadiusProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.BorderTopRightRadiusProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.BorderTopWidthProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.BottomProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.ColorProperty());
			this.AddProperty<StyleCursor, Cursor>(new InlineStyleAccessPropertyBag.CursorProperty());
			this.AddProperty<StyleEnum<DisplayStyle>, DisplayStyle>(new InlineStyleAccessPropertyBag.DisplayProperty());
			this.AddProperty<StyleList<FilterFunction>, List<FilterFunction>>(new InlineStyleAccessPropertyBag.FilterProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.FlexBasisProperty());
			this.AddProperty<StyleEnum<FlexDirection>, FlexDirection>(new InlineStyleAccessPropertyBag.FlexDirectionProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.FlexGrowProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.FlexShrinkProperty());
			this.AddProperty<StyleEnum<Wrap>, Wrap>(new InlineStyleAccessPropertyBag.FlexWrapProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.FontSizeProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.HeightProperty());
			this.AddProperty<StyleEnum<Justify>, Justify>(new InlineStyleAccessPropertyBag.JustifyContentProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.LeftProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.LetterSpacingProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MarginBottomProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MarginLeftProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MarginRightProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MarginTopProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MaxHeightProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MaxWidthProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MinHeightProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.MinWidthProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.OpacityProperty());
			this.AddProperty<StyleEnum<Overflow>, Overflow>(new InlineStyleAccessPropertyBag.OverflowProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.PaddingBottomProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.PaddingLeftProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.PaddingRightProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.PaddingTopProperty());
			this.AddProperty<StyleEnum<Position>, Position>(new InlineStyleAccessPropertyBag.PositionProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.RightProperty());
			this.AddProperty<StyleRotate, Rotate>(new InlineStyleAccessPropertyBag.RotateProperty());
			this.AddProperty<StyleScale, Scale>(new InlineStyleAccessPropertyBag.ScaleProperty());
			this.AddProperty<StyleEnum<TextOverflow>, TextOverflow>(new InlineStyleAccessPropertyBag.TextOverflowProperty());
			this.AddProperty<StyleTextShadow, TextShadow>(new InlineStyleAccessPropertyBag.TextShadowProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.TopProperty());
			this.AddProperty<StyleTransformOrigin, TransformOrigin>(new InlineStyleAccessPropertyBag.TransformOriginProperty());
			this.AddProperty<StyleList<TimeValue>, List<TimeValue>>(new InlineStyleAccessPropertyBag.TransitionDelayProperty());
			this.AddProperty<StyleList<TimeValue>, List<TimeValue>>(new InlineStyleAccessPropertyBag.TransitionDurationProperty());
			this.AddProperty<StyleList<StylePropertyName>, List<StylePropertyName>>(new InlineStyleAccessPropertyBag.TransitionPropertyProperty());
			this.AddProperty<StyleList<EasingFunction>, List<EasingFunction>>(new InlineStyleAccessPropertyBag.TransitionTimingFunctionProperty());
			this.AddProperty<StyleTranslate, Translate>(new InlineStyleAccessPropertyBag.TranslateProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.UnityBackgroundImageTintColorProperty());
			this.AddProperty<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode>(new InlineStyleAccessPropertyBag.UnityEditorTextRenderingModeProperty());
			this.AddProperty<StyleFont, Font>(new InlineStyleAccessPropertyBag.UnityFontProperty());
			this.AddProperty<StyleFontDefinition, FontDefinition>(new InlineStyleAccessPropertyBag.UnityFontDefinitionProperty());
			this.AddProperty<StyleEnum<FontStyle>, FontStyle>(new InlineStyleAccessPropertyBag.UnityFontStyleAndWeightProperty());
			this.AddProperty<StyleMaterialDefinition, MaterialDefinition>(new InlineStyleAccessPropertyBag.UnityMaterialProperty());
			this.AddProperty<StyleEnum<OverflowClipBox>, OverflowClipBox>(new InlineStyleAccessPropertyBag.UnityOverflowClipBoxProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.UnityParagraphSpacingProperty());
			this.AddProperty<StyleInt, int>(new InlineStyleAccessPropertyBag.UnitySliceBottomProperty());
			this.AddProperty<StyleInt, int>(new InlineStyleAccessPropertyBag.UnitySliceLeftProperty());
			this.AddProperty<StyleInt, int>(new InlineStyleAccessPropertyBag.UnitySliceRightProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.UnitySliceScaleProperty());
			this.AddProperty<StyleInt, int>(new InlineStyleAccessPropertyBag.UnitySliceTopProperty());
			this.AddProperty<StyleEnum<SliceType>, SliceType>(new InlineStyleAccessPropertyBag.UnitySliceTypeProperty());
			this.AddProperty<StyleEnum<TextAnchor>, TextAnchor>(new InlineStyleAccessPropertyBag.UnityTextAlignProperty());
			this.AddProperty<StyleTextAutoSize, TextAutoSize>(new InlineStyleAccessPropertyBag.UnityTextAutoSizeProperty());
			this.AddProperty<StyleEnum<TextGeneratorType>, TextGeneratorType>(new InlineStyleAccessPropertyBag.UnityTextGeneratorProperty());
			this.AddProperty<StyleColor, Color>(new InlineStyleAccessPropertyBag.UnityTextOutlineColorProperty());
			this.AddProperty<StyleFloat, float>(new InlineStyleAccessPropertyBag.UnityTextOutlineWidthProperty());
			this.AddProperty<StyleEnum<TextOverflowPosition>, TextOverflowPosition>(new InlineStyleAccessPropertyBag.UnityTextOverflowPositionProperty());
			this.AddProperty<StyleEnum<Visibility>, Visibility>(new InlineStyleAccessPropertyBag.VisibilityProperty());
			this.AddProperty<StyleEnum<WhiteSpace>, WhiteSpace>(new InlineStyleAccessPropertyBag.WhiteSpaceProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.WidthProperty());
			this.AddProperty<StyleLength, Length>(new InlineStyleAccessPropertyBag.WordSpacingProperty());
		}

		private void AddProperty<TStyleValue, TValue>(InlineStyleAccessPropertyBag.InlineStyleProperty<TStyleValue, TValue> property) where TStyleValue : IStyleValue<TValue>, new()
		{
			this.m_PropertiesList.Add(property);
			this.m_PropertiesHash.Add(property.Name, property);
			bool flag = string.CompareOrdinal(property.Name, property.ussName) != 0;
			if (flag)
			{
				this.m_PropertiesHash.Add(property.ussName, property);
			}
		}

		public override PropertyCollection<InlineStyleAccess> GetProperties()
		{
			return new PropertyCollection<InlineStyleAccess>(this.m_PropertiesList);
		}

		public override PropertyCollection<InlineStyleAccess> GetProperties(ref InlineStyleAccess container)
		{
			return new PropertyCollection<InlineStyleAccess>(this.m_PropertiesList);
		}

		public bool TryGetProperty(ref InlineStyleAccess container, string name, out IProperty<InlineStyleAccess> property)
		{
			return this.m_PropertiesHash.TryGetValue(name, out property);
		}

		private readonly List<IProperty<InlineStyleAccess>> m_PropertiesList;

		private readonly Dictionary<string, IProperty<InlineStyleAccess>> m_PropertiesHash;

		private class AlignContentProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Align>
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
					return false;
				}
			}

			public override StyleEnum<Align> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).alignContent;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Align> value)
			{
				((IStyle)container).alignContent = value;
			}
		}

		private class AlignItemsProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Align>
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
					return false;
				}
			}

			public override StyleEnum<Align> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).alignItems;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Align> value)
			{
				((IStyle)container).alignItems = value;
			}
		}

		private class AlignSelfProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Align>
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
					return false;
				}
			}

			public override StyleEnum<Align> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).alignSelf;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Align> value)
			{
				((IStyle)container).alignSelf = value;
			}
		}

		private class AspectRatioProperty : InlineStyleAccessPropertyBag.InlineStyleRatioProperty
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
					return false;
				}
			}

			public override StyleRatio GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).aspectRatio;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleRatio value)
			{
				((IStyle)container).aspectRatio = value;
			}
		}

		private class BackgroundColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).backgroundColor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).backgroundColor = value;
			}
		}

		private class BackgroundImageProperty : InlineStyleAccessPropertyBag.InlineStyleBackgroundProperty
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
					return false;
				}
			}

			public override StyleBackground GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).backgroundImage;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleBackground value)
			{
				((IStyle)container).backgroundImage = value;
			}
		}

		private class BackgroundPositionXProperty : InlineStyleAccessPropertyBag.InlineStyleBackgroundPositionProperty
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
					return false;
				}
			}

			public override StyleBackgroundPosition GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).backgroundPositionX;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleBackgroundPosition value)
			{
				((IStyle)container).backgroundPositionX = value;
			}
		}

		private class BackgroundPositionYProperty : InlineStyleAccessPropertyBag.InlineStyleBackgroundPositionProperty
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
					return false;
				}
			}

			public override StyleBackgroundPosition GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).backgroundPositionY;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleBackgroundPosition value)
			{
				((IStyle)container).backgroundPositionY = value;
			}
		}

		private class BackgroundRepeatProperty : InlineStyleAccessPropertyBag.InlineStyleBackgroundRepeatProperty
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
					return false;
				}
			}

			public override StyleBackgroundRepeat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).backgroundRepeat;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleBackgroundRepeat value)
			{
				((IStyle)container).backgroundRepeat = value;
			}
		}

		private class BackgroundSizeProperty : InlineStyleAccessPropertyBag.InlineStyleBackgroundSizeProperty
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
					return false;
				}
			}

			public override StyleBackgroundSize GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).backgroundSize;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleBackgroundSize value)
			{
				((IStyle)container).backgroundSize = value;
			}
		}

		private class BorderBottomColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderBottomColor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).borderBottomColor = value;
			}
		}

		private class BorderBottomLeftRadiusProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderBottomLeftRadius;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).borderBottomLeftRadius = value;
			}
		}

		private class BorderBottomRightRadiusProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderBottomRightRadius;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).borderBottomRightRadius = value;
			}
		}

		private class BorderBottomWidthProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderBottomWidth;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).borderBottomWidth = value;
			}
		}

		private class BorderLeftColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderLeftColor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).borderLeftColor = value;
			}
		}

		private class BorderLeftWidthProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderLeftWidth;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).borderLeftWidth = value;
			}
		}

		private class BorderRightColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderRightColor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).borderRightColor = value;
			}
		}

		private class BorderRightWidthProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderRightWidth;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).borderRightWidth = value;
			}
		}

		private class BorderTopColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderTopColor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).borderTopColor = value;
			}
		}

		private class BorderTopLeftRadiusProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderTopLeftRadius;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).borderTopLeftRadius = value;
			}
		}

		private class BorderTopRightRadiusProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderTopRightRadius;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).borderTopRightRadius = value;
			}
		}

		private class BorderTopWidthProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).borderTopWidth;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).borderTopWidth = value;
			}
		}

		private class BottomProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).bottom;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).bottom = value;
			}
		}

		private class ColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).color;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).color = value;
			}
		}

		private class CursorProperty : InlineStyleAccessPropertyBag.InlineStyleCursorProperty
		{
			public override string Name
			{
				get
				{
					return "cursor";
				}
			}

			public override string ussName
			{
				get
				{
					return "cursor";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override StyleCursor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).cursor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleCursor value)
			{
				((IStyle)container).cursor = value;
			}
		}

		private class DisplayProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<DisplayStyle>
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
					return false;
				}
			}

			public override StyleEnum<DisplayStyle> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).display;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<DisplayStyle> value)
			{
				((IStyle)container).display = value;
			}
		}

		private class FilterProperty : InlineStyleAccessPropertyBag.InlineStyleListProperty<FilterFunction>
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
					return false;
				}
			}

			public override StyleList<FilterFunction> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).filter;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleList<FilterFunction> value)
			{
				((IStyle)container).filter = value;
			}
		}

		private class FlexBasisProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).flexBasis;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).flexBasis = value;
			}
		}

		private class FlexDirectionProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<FlexDirection>
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
					return false;
				}
			}

			public override StyleEnum<FlexDirection> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).flexDirection;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<FlexDirection> value)
			{
				((IStyle)container).flexDirection = value;
			}
		}

		private class FlexGrowProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).flexGrow;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).flexGrow = value;
			}
		}

		private class FlexShrinkProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).flexShrink;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).flexShrink = value;
			}
		}

		private class FlexWrapProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Wrap>
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
					return false;
				}
			}

			public override StyleEnum<Wrap> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).flexWrap;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Wrap> value)
			{
				((IStyle)container).flexWrap = value;
			}
		}

		private class FontSizeProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).fontSize;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).fontSize = value;
			}
		}

		private class HeightProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).height;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).height = value;
			}
		}

		private class JustifyContentProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Justify>
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
					return false;
				}
			}

			public override StyleEnum<Justify> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).justifyContent;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Justify> value)
			{
				((IStyle)container).justifyContent = value;
			}
		}

		private class LeftProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).left;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).left = value;
			}
		}

		private class LetterSpacingProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).letterSpacing;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).letterSpacing = value;
			}
		}

		private class MarginBottomProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).marginBottom;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).marginBottom = value;
			}
		}

		private class MarginLeftProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).marginLeft;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).marginLeft = value;
			}
		}

		private class MarginRightProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).marginRight;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).marginRight = value;
			}
		}

		private class MarginTopProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).marginTop;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).marginTop = value;
			}
		}

		private class MaxHeightProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).maxHeight;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).maxHeight = value;
			}
		}

		private class MaxWidthProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).maxWidth;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).maxWidth = value;
			}
		}

		private class MinHeightProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).minHeight;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).minHeight = value;
			}
		}

		private class MinWidthProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).minWidth;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).minWidth = value;
			}
		}

		private class OpacityProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).opacity;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).opacity = value;
			}
		}

		private class OverflowProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Overflow>
		{
			public override string Name
			{
				get
				{
					return "overflow";
				}
			}

			public override string ussName
			{
				get
				{
					return "overflow";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override StyleEnum<Overflow> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).overflow;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Overflow> value)
			{
				((IStyle)container).overflow = value;
			}
		}

		private class PaddingBottomProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).paddingBottom;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).paddingBottom = value;
			}
		}

		private class PaddingLeftProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).paddingLeft;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).paddingLeft = value;
			}
		}

		private class PaddingRightProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).paddingRight;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).paddingRight = value;
			}
		}

		private class PaddingTopProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).paddingTop;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).paddingTop = value;
			}
		}

		private class PositionProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Position>
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
					return false;
				}
			}

			public override StyleEnum<Position> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).position;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Position> value)
			{
				((IStyle)container).position = value;
			}
		}

		private class RightProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).right;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).right = value;
			}
		}

		private class RotateProperty : InlineStyleAccessPropertyBag.InlineStyleRotateProperty
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
					return false;
				}
			}

			public override StyleRotate GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).rotate;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleRotate value)
			{
				((IStyle)container).rotate = value;
			}
		}

		private class ScaleProperty : InlineStyleAccessPropertyBag.InlineStyleScaleProperty
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
					return false;
				}
			}

			public override StyleScale GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).scale;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleScale value)
			{
				((IStyle)container).scale = value;
			}
		}

		private class TextOverflowProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<TextOverflow>
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
					return false;
				}
			}

			public override StyleEnum<TextOverflow> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).textOverflow;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextOverflow> value)
			{
				((IStyle)container).textOverflow = value;
			}
		}

		private class TextShadowProperty : InlineStyleAccessPropertyBag.InlineStyleTextShadowProperty
		{
			public override string Name
			{
				get
				{
					return "textShadow";
				}
			}

			public override string ussName
			{
				get
				{
					return "text-shadow";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override StyleTextShadow GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).textShadow;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleTextShadow value)
			{
				((IStyle)container).textShadow = value;
			}
		}

		private class TopProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).top;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).top = value;
			}
		}

		private class TransformOriginProperty : InlineStyleAccessPropertyBag.InlineStyleTransformOriginProperty
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
					return false;
				}
			}

			public override StyleTransformOrigin GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).transformOrigin;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleTransformOrigin value)
			{
				((IStyle)container).transformOrigin = value;
			}
		}

		private class TransitionDelayProperty : InlineStyleAccessPropertyBag.InlineStyleListProperty<TimeValue>
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
					return false;
				}
			}

			public override StyleList<TimeValue> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).transitionDelay;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleList<TimeValue> value)
			{
				((IStyle)container).transitionDelay = value;
			}
		}

		private class TransitionDurationProperty : InlineStyleAccessPropertyBag.InlineStyleListProperty<TimeValue>
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
					return false;
				}
			}

			public override StyleList<TimeValue> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).transitionDuration;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleList<TimeValue> value)
			{
				((IStyle)container).transitionDuration = value;
			}
		}

		private class TransitionPropertyProperty : InlineStyleAccessPropertyBag.InlineStyleListProperty<StylePropertyName>
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
					return false;
				}
			}

			public override StyleList<StylePropertyName> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).transitionProperty;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleList<StylePropertyName> value)
			{
				((IStyle)container).transitionProperty = value;
			}
		}

		private class TransitionTimingFunctionProperty : InlineStyleAccessPropertyBag.InlineStyleListProperty<EasingFunction>
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
					return false;
				}
			}

			public override StyleList<EasingFunction> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).transitionTimingFunction;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleList<EasingFunction> value)
			{
				((IStyle)container).transitionTimingFunction = value;
			}
		}

		private class TranslateProperty : InlineStyleAccessPropertyBag.InlineStyleTranslateProperty
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
					return false;
				}
			}

			public override StyleTranslate GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).translate;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleTranslate value)
			{
				((IStyle)container).translate = value;
			}
		}

		private class UnityBackgroundImageTintColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityBackgroundImageTintColor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).unityBackgroundImageTintColor = value;
			}
		}

		private class UnityEditorTextRenderingModeProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<EditorTextRenderingMode>
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
					return false;
				}
			}

			public override StyleEnum<EditorTextRenderingMode> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityEditorTextRenderingMode;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<EditorTextRenderingMode> value)
			{
				((IStyle)container).unityEditorTextRenderingMode = value;
			}
		}

		private class UnityFontProperty : InlineStyleAccessPropertyBag.InlineStyleFontProperty
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
					return false;
				}
			}

			public override StyleFont GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityFont;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFont value)
			{
				((IStyle)container).unityFont = value;
			}
		}

		private class UnityFontDefinitionProperty : InlineStyleAccessPropertyBag.InlineStyleFontDefinitionProperty
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
					return false;
				}
			}

			public override StyleFontDefinition GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityFontDefinition;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFontDefinition value)
			{
				((IStyle)container).unityFontDefinition = value;
			}
		}

		private class UnityFontStyleAndWeightProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<FontStyle>
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
					return false;
				}
			}

			public override StyleEnum<FontStyle> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityFontStyleAndWeight;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<FontStyle> value)
			{
				((IStyle)container).unityFontStyleAndWeight = value;
			}
		}

		private class UnityMaterialProperty : InlineStyleAccessPropertyBag.InlineStyleMaterialDefinitionProperty
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
					return false;
				}
			}

			public override StyleMaterialDefinition GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityMaterial;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleMaterialDefinition value)
			{
				((IStyle)container).unityMaterial = value;
			}
		}

		private class UnityOverflowClipBoxProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<OverflowClipBox>
		{
			public override string Name
			{
				get
				{
					return "unityOverflowClipBox";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-overflow-clip-box";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override StyleEnum<OverflowClipBox> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityOverflowClipBox;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<OverflowClipBox> value)
			{
				((IStyle)container).unityOverflowClipBox = value;
			}
		}

		private class UnityParagraphSpacingProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityParagraphSpacing;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).unityParagraphSpacing = value;
			}
		}

		private class UnitySliceBottomProperty : InlineStyleAccessPropertyBag.InlineStyleIntProperty
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
					return false;
				}
			}

			public override StyleInt GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unitySliceBottom;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleInt value)
			{
				((IStyle)container).unitySliceBottom = value;
			}
		}

		private class UnitySliceLeftProperty : InlineStyleAccessPropertyBag.InlineStyleIntProperty
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
					return false;
				}
			}

			public override StyleInt GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unitySliceLeft;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleInt value)
			{
				((IStyle)container).unitySliceLeft = value;
			}
		}

		private class UnitySliceRightProperty : InlineStyleAccessPropertyBag.InlineStyleIntProperty
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
					return false;
				}
			}

			public override StyleInt GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unitySliceRight;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleInt value)
			{
				((IStyle)container).unitySliceRight = value;
			}
		}

		private class UnitySliceScaleProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unitySliceScale;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).unitySliceScale = value;
			}
		}

		private class UnitySliceTopProperty : InlineStyleAccessPropertyBag.InlineStyleIntProperty
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
					return false;
				}
			}

			public override StyleInt GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unitySliceTop;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleInt value)
			{
				((IStyle)container).unitySliceTop = value;
			}
		}

		private class UnitySliceTypeProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<SliceType>
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
					return false;
				}
			}

			public override StyleEnum<SliceType> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unitySliceType;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<SliceType> value)
			{
				((IStyle)container).unitySliceType = value;
			}
		}

		private class UnityTextAlignProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<TextAnchor>
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
					return false;
				}
			}

			public override StyleEnum<TextAnchor> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityTextAlign;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextAnchor> value)
			{
				((IStyle)container).unityTextAlign = value;
			}
		}

		private class UnityTextAutoSizeProperty : InlineStyleAccessPropertyBag.InlineStyleTextAutoSizeProperty
		{
			public override string Name
			{
				get
				{
					return "unityTextAutoSize";
				}
			}

			public override string ussName
			{
				get
				{
					return "-unity-text-auto-size";
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override StyleTextAutoSize GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityTextAutoSize;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleTextAutoSize value)
			{
				((IStyle)container).unityTextAutoSize = value;
			}
		}

		private class UnityTextGeneratorProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<TextGeneratorType>
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
					return false;
				}
			}

			public override StyleEnum<TextGeneratorType> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityTextGenerator;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextGeneratorType> value)
			{
				((IStyle)container).unityTextGenerator = value;
			}
		}

		private class UnityTextOutlineColorProperty : InlineStyleAccessPropertyBag.InlineStyleColorProperty
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
					return false;
				}
			}

			public override StyleColor GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityTextOutlineColor;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleColor value)
			{
				((IStyle)container).unityTextOutlineColor = value;
			}
		}

		private class UnityTextOutlineWidthProperty : InlineStyleAccessPropertyBag.InlineStyleFloatProperty
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
					return false;
				}
			}

			public override StyleFloat GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityTextOutlineWidth;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleFloat value)
			{
				((IStyle)container).unityTextOutlineWidth = value;
			}
		}

		private class UnityTextOverflowPositionProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<TextOverflowPosition>
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
					return false;
				}
			}

			public override StyleEnum<TextOverflowPosition> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).unityTextOverflowPosition;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<TextOverflowPosition> value)
			{
				((IStyle)container).unityTextOverflowPosition = value;
			}
		}

		private class VisibilityProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<Visibility>
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
					return false;
				}
			}

			public override StyleEnum<Visibility> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).visibility;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<Visibility> value)
			{
				((IStyle)container).visibility = value;
			}
		}

		private class WhiteSpaceProperty : InlineStyleAccessPropertyBag.InlineStyleEnumProperty<WhiteSpace>
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
					return false;
				}
			}

			public override StyleEnum<WhiteSpace> GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).whiteSpace;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleEnum<WhiteSpace> value)
			{
				((IStyle)container).whiteSpace = value;
			}
		}

		private class WidthProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).width;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).width = value;
			}
		}

		private class WordSpacingProperty : InlineStyleAccessPropertyBag.InlineStyleLengthProperty
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
					return false;
				}
			}

			public override StyleLength GetValue(ref InlineStyleAccess container)
			{
				return ((IStyle)container).wordSpacing;
			}

			public override void SetValue(ref InlineStyleAccess container, StyleLength value)
			{
				((IStyle)container).wordSpacing = value;
			}
		}

		private abstract class InlineStyleProperty<TStyleValue, TValue> : Property<InlineStyleAccess, TStyleValue> where TStyleValue : IStyleValue<TValue>, new()
		{
			protected InlineStyleProperty()
			{
				ConverterGroups.RegisterGlobal<TStyleValue, TValue>(delegate(ref TStyleValue sv)
				{
					return sv.value;
				});
				ConverterGroups.RegisterGlobal<TValue, TStyleValue>(delegate(ref TValue v)
				{
					TStyleValue tstyleValue = new TStyleValue();
					tstyleValue.value = v;
					return tstyleValue;
				});
				ConverterGroups.RegisterGlobal<TStyleValue, StyleKeyword>(delegate(ref TStyleValue sv)
				{
					return sv.keyword;
				});
				ConverterGroups.RegisterGlobal<StyleKeyword, TStyleValue>(delegate(ref StyleKeyword kw)
				{
					TStyleValue tstyleValue2 = new TStyleValue();
					tstyleValue2.keyword = kw;
					return tstyleValue2;
				});
			}

			public abstract string ussName { get; }
		}

		private abstract class InlineStyleEnumProperty<TValue> : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleEnum<TValue>, TValue> where TValue : struct, IConvertible
		{
		}

		private abstract class InlineStyleColorProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleColor, Color>
		{
			protected InlineStyleColorProperty()
			{
				ConverterGroups.RegisterGlobal<Color32, StyleColor>(delegate(ref Color32 v)
				{
					return new StyleColor(v);
				});
				ConverterGroups.RegisterGlobal<StyleColor, Color32>(delegate(ref StyleColor sv)
				{
					return sv.value;
				});
			}
		}

		private abstract class InlineStyleRatioProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleRatio, Ratio>
		{
			protected InlineStyleRatioProperty()
			{
				ConverterGroups.RegisterGlobal<float, StyleRatio>(delegate(ref float v)
				{
					return new StyleRatio(v);
				});
				ConverterGroups.RegisterGlobal<StyleRatio, float>(delegate(ref StyleRatio sv)
				{
					return sv.value;
				});
			}
		}

		private abstract class InlineStyleBackgroundProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleBackground, Background>
		{
			protected InlineStyleBackgroundProperty()
			{
				ConverterGroups.RegisterGlobal<Texture2D, StyleBackground>(delegate(ref Texture2D v)
				{
					return new StyleBackground(v);
				});
				ConverterGroups.RegisterGlobal<Sprite, StyleBackground>(delegate(ref Sprite v)
				{
					return new StyleBackground(v);
				});
				ConverterGroups.RegisterGlobal<VectorImage, StyleBackground>(delegate(ref VectorImage v)
				{
					return new StyleBackground(v);
				});
				ConverterGroups.RegisterGlobal<RenderTexture, StyleBackground>(delegate(ref RenderTexture v)
				{
					return new StyleBackground(Background.FromRenderTexture(v));
				});
				ConverterGroups.RegisterGlobal<StyleBackground, Texture2D>(delegate(ref StyleBackground sv)
				{
					return sv.value.texture;
				});
				ConverterGroups.RegisterGlobal<StyleBackground, Sprite>(delegate(ref StyleBackground sv)
				{
					return sv.value.sprite;
				});
				ConverterGroups.RegisterGlobal<StyleBackground, RenderTexture>(delegate(ref StyleBackground sv)
				{
					return sv.value.renderTexture;
				});
				ConverterGroups.RegisterGlobal<StyleBackground, VectorImage>(delegate(ref StyleBackground sv)
				{
					return sv.value.vectorImage;
				});
			}
		}

		private abstract class InlineStyleLengthProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleLength, Length>
		{
			protected InlineStyleLengthProperty()
			{
				ConverterGroups.RegisterGlobal<float, StyleLength>(delegate(ref float v)
				{
					return new StyleLength(v);
				});
				ConverterGroups.RegisterGlobal<int, StyleLength>(delegate(ref int v)
				{
					return new StyleLength((float)v);
				});
				ConverterGroups.RegisterGlobal<StyleLength, float>(delegate(ref StyleLength sv)
				{
					return sv.value.value;
				});
				ConverterGroups.RegisterGlobal<StyleLength, int>(delegate(ref StyleLength sv)
				{
					return (int)sv.value.value;
				});
			}
		}

		private abstract class InlineStyleFloatProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleFloat, float>
		{
			protected InlineStyleFloatProperty()
			{
				ConverterGroups.RegisterGlobal<int, StyleFloat>(delegate(ref int v)
				{
					return new StyleFloat((float)v);
				});
				ConverterGroups.RegisterGlobal<StyleFloat, int>(delegate(ref StyleFloat sv)
				{
					return (int)sv.value;
				});
			}
		}

		private abstract class InlineStyleListProperty<T> : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleList<T>, List<T>>
		{
		}

		private abstract class InlineStyleFontProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleFont, Font>
		{
		}

		private abstract class InlineStyleFontDefinitionProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleFontDefinition, FontDefinition>
		{
			protected InlineStyleFontDefinitionProperty()
			{
				ConverterGroups.RegisterGlobal<Font, StyleFontDefinition>(delegate(ref Font v)
				{
					return new StyleFontDefinition(v);
				});
				ConverterGroups.RegisterGlobal<FontAsset, StyleFontDefinition>(delegate(ref FontAsset v)
				{
					return new StyleFontDefinition(v);
				});
				ConverterGroups.RegisterGlobal<StyleFontDefinition, Font>(delegate(ref StyleFontDefinition sv)
				{
					return sv.value.font;
				});
				ConverterGroups.RegisterGlobal<StyleFontDefinition, FontAsset>(delegate(ref StyleFontDefinition sv)
				{
					return sv.value.fontAsset;
				});
			}
		}

		private abstract class InlineStyleIntProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleInt, int>
		{
		}

		private abstract class InlineStyleRotateProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleRotate, Rotate>
		{
		}

		private abstract class InlineStyleScaleProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleScale, Scale>
		{
		}

		private abstract class InlineStyleCursorProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleCursor, Cursor>
		{
		}

		private abstract class InlineStyleTextShadowProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleTextShadow, TextShadow>
		{
		}

		private abstract class InlineStyleTextAutoSizeProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleTextAutoSize, TextAutoSize>
		{
		}

		private abstract class InlineStyleTransformOriginProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleTransformOrigin, TransformOrigin>
		{
		}

		private abstract class InlineStyleTranslateProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleTranslate, Translate>
		{
		}

		private abstract class InlineStyleBackgroundPositionProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleBackgroundPosition, BackgroundPosition>
		{
		}

		private abstract class InlineStyleBackgroundRepeatProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleBackgroundRepeat, BackgroundRepeat>
		{
		}

		private abstract class InlineStyleBackgroundSizeProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleBackgroundSize, BackgroundSize>
		{
		}

		private abstract class InlineStyleMaterialDefinitionProperty : InlineStyleAccessPropertyBag.InlineStyleProperty<StyleMaterialDefinition, MaterialDefinition>
		{
			protected InlineStyleMaterialDefinitionProperty()
			{
				ConverterGroups.RegisterGlobal<MaterialDefinition, StyleMaterialDefinition>(delegate(ref MaterialDefinition v)
				{
					return new StyleMaterialDefinition(v);
				});
				ConverterGroups.RegisterGlobal<StyleMaterialDefinition, MaterialDefinition>(delegate(ref StyleMaterialDefinition sv)
				{
					return sv.value;
				});
			}
		}
	}
}
