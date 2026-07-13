using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements
{
	internal static class UIElementsInitialization
	{
		[RequiredMember]
		[RequiredByNativeCode(true)]
		public static void InitializeUIElementsManaged()
		{
			UIElementsInitialization.RegisterBuiltInPropertyBags();
		}

		internal static void RegisterBuiltInPropertyBags()
		{
			PropertyBag.Register<InlineStyleAccess>(new InlineStyleAccessPropertyBag());
			PropertyBag.Register<ResolvedStyleAccess>(new ResolvedStyleAccessPropertyBag());
			PropertyBag.Register<StyleEnum<Align>>(new StyleValuePropertyBag<StyleEnum<Align>, Align>());
			PropertyBag.Register<StyleEnum<DisplayStyle>>(new StyleValuePropertyBag<StyleEnum<DisplayStyle>, DisplayStyle>());
			PropertyBag.Register<StyleEnum<EasingMode>>(new StyleValuePropertyBag<StyleEnum<EasingMode>, EasingMode>());
			PropertyBag.Register<StyleEnum<FlexDirection>>(new StyleValuePropertyBag<StyleEnum<FlexDirection>, FlexDirection>());
			PropertyBag.Register<StyleEnum<FontStyle>>(new StyleValuePropertyBag<StyleEnum<FontStyle>, FontStyle>());
			PropertyBag.Register<StyleEnum<Justify>>(new StyleValuePropertyBag<StyleEnum<Justify>, Justify>());
			PropertyBag.Register<StyleEnum<Overflow>>(new StyleValuePropertyBag<StyleEnum<Overflow>, Overflow>());
			PropertyBag.Register<StyleEnum<OverflowClipBox>>(new StyleValuePropertyBag<StyleEnum<OverflowClipBox>, OverflowClipBox>());
			PropertyBag.Register<StyleEnum<OverflowInternal>>(new StyleValuePropertyBag<StyleEnum<OverflowInternal>, OverflowInternal>());
			PropertyBag.Register<StyleEnum<Position>>(new StyleValuePropertyBag<StyleEnum<Position>, Position>());
			PropertyBag.Register<StyleEnum<ScaleMode>>(new StyleValuePropertyBag<StyleEnum<ScaleMode>, ScaleMode>());
			PropertyBag.Register<StyleEnum<SliceType>>(new StyleValuePropertyBag<StyleEnum<SliceType>, SliceType>());
			PropertyBag.Register<StyleEnum<TextAnchor>>(new StyleValuePropertyBag<StyleEnum<TextAnchor>, TextAnchor>());
			PropertyBag.Register<StyleEnum<TextGeneratorType>>(new StyleValuePropertyBag<StyleEnum<TextGeneratorType>, TextGeneratorType>());
			PropertyBag.Register<StyleEnum<TextOverflow>>(new StyleValuePropertyBag<StyleEnum<TextOverflow>, TextOverflow>());
			PropertyBag.Register<StyleEnum<EditorTextRenderingMode>>(new StyleValuePropertyBag<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode>());
			PropertyBag.Register<StyleEnum<TextOverflowPosition>>(new StyleValuePropertyBag<StyleEnum<TextOverflowPosition>, TextOverflowPosition>());
			PropertyBag.Register<StyleEnum<TransformOriginOffset>>(new StyleValuePropertyBag<StyleEnum<TransformOriginOffset>, TransformOriginOffset>());
			PropertyBag.Register<StyleEnum<Visibility>>(new StyleValuePropertyBag<StyleEnum<Visibility>, Visibility>());
			PropertyBag.Register<StyleEnum<WhiteSpace>>(new StyleValuePropertyBag<StyleEnum<WhiteSpace>, WhiteSpace>());
			PropertyBag.Register<StyleEnum<Wrap>>(new StyleValuePropertyBag<StyleEnum<Wrap>, Wrap>());
			PropertyBag.Register<StyleBackground>(new StyleValuePropertyBag<StyleBackground, Background>());
			PropertyBag.Register<Length>(new Length.PropertyBag());
			PropertyBag.Register<StyleBackgroundPosition>(new StyleValuePropertyBag<StyleBackgroundPosition, BackgroundPosition>());
			PropertyBag.Register<BackgroundPosition>(new BackgroundPosition.PropertyBag());
			PropertyBag.Register<StyleBackgroundRepeat>(new StyleValuePropertyBag<StyleBackgroundRepeat, BackgroundRepeat>());
			PropertyBag.Register<BackgroundRepeat>(new BackgroundRepeat.PropertyBag());
			PropertyBag.Register<StyleBackgroundSize>(new StyleValuePropertyBag<StyleBackgroundSize, BackgroundSize>());
			PropertyBag.Register<BackgroundSize>(new BackgroundSize.PropertyBag());
			PropertyBag.Register<StyleColor>(new StyleValuePropertyBag<StyleColor, Color>());
			PropertyBag.Register<StyleCursor>(new StyleValuePropertyBag<StyleCursor, Cursor>());
			PropertyBag.Register<Cursor>(new Cursor.PropertyBag());
			PropertyBag.Register<StyleFloat>(new StyleValuePropertyBag<StyleFloat, float>());
			PropertyBag.Register<StyleFont>(new StyleValuePropertyBag<StyleFont, Font>());
			PropertyBag.Register<StyleFontDefinition>(new StyleValuePropertyBag<StyleFontDefinition, FontDefinition>());
			PropertyBag.Register<FontDefinition>(new FontDefinition.PropertyBag());
			PropertyBag.Register<StyleInt>(new StyleValuePropertyBag<StyleInt, int>());
			PropertyBag.Register<StyleLength>(new StyleValuePropertyBag<StyleLength, Length>());
			PropertyBag.Register<Background>(new Background.PropertyBag());
			PropertyBag.Register<StyleList<EasingFunction>>(new StyleValuePropertyBag<StyleList<EasingFunction>, List<EasingFunction>>());
			PropertyBag.Register<EasingFunction>(new EasingFunction.PropertyBag());
			PropertyBag.RegisterList<StyleList<EasingFunction>, EasingFunction>();
			PropertyBag.Register<StyleList<StylePropertyName>>(new StyleValuePropertyBag<StyleList<StylePropertyName>, List<StylePropertyName>>());
			PropertyBag.Register<StylePropertyName>(new StylePropertyName.PropertyBag());
			PropertyBag.RegisterList<StyleList<StylePropertyName>, StylePropertyName>();
			PropertyBag.Register<StyleList<TimeValue>>(new StyleValuePropertyBag<StyleList<TimeValue>, List<TimeValue>>());
			PropertyBag.Register<TimeValue>(new TimeValue.PropertyBag());
			PropertyBag.RegisterList<StyleList<TimeValue>, TimeValue>();
			PropertyBag.Register<StyleRotate>(new StyleValuePropertyBag<StyleRotate, Rotate>());
			PropertyBag.Register<Rotate>(new Rotate.PropertyBag());
			PropertyBag.Register<StyleRatio>(new StyleValuePropertyBag<StyleRatio, Ratio>());
			PropertyBag.Register<Ratio>(new Ratio.PropertyBag());
			PropertyBag.Register<Angle>(new Angle.PropertyBag());
			PropertyBag.Register<StyleScale>(new StyleValuePropertyBag<StyleScale, Scale>());
			PropertyBag.Register<Scale>(new Scale.PropertyBag());
			PropertyBag.Register<StyleTransformOrigin>(new StyleValuePropertyBag<StyleTransformOrigin, TransformOrigin>());
			PropertyBag.Register<TransformOrigin>(new TransformOrigin.PropertyBag());
			PropertyBag.Register<StyleTranslate>(new StyleValuePropertyBag<StyleTranslate, Translate>());
			PropertyBag.Register<Translate>(new Translate.PropertyBag());
			PropertyBag.Register<StyleTextShadow>(new StyleValuePropertyBag<StyleTextShadow, TextShadow>());
			PropertyBag.Register<TextShadow>(new TextShadow.PropertyBag());
			PropertyBag.Register<StyleTextAutoSize>(new StyleValuePropertyBag<StyleTextAutoSize, TextAutoSize>());
			PropertyBag.Register<TextAutoSize>(new TextAutoSize.PropertyBag());
			PropertyBag.Register<StyleList<FilterFunction>>(new StyleValuePropertyBag<StyleList<FilterFunction>, List<FilterFunction>>());
			PropertyBag.Register<MaterialDefinition>(new MaterialDefinition.PropertyBag());
			PropertyBag.Register<StyleMaterialDefinition>(new StyleValuePropertyBag<StyleMaterialDefinition, MaterialDefinition>());
		}
	}
}
