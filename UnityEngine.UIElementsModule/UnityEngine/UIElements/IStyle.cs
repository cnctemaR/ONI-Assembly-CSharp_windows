using System;

namespace UnityEngine.UIElements
{
	public interface IStyle
	{
		StyleEnum<Align> alignContent { get; set; }

		StyleEnum<Align> alignItems { get; set; }

		StyleEnum<Align> alignSelf { get; set; }

		StyleRatio aspectRatio { get; set; }

		StyleColor backgroundColor { get; set; }

		StyleBackground backgroundImage { get; set; }

		StyleBackgroundPosition backgroundPositionX { get; set; }

		StyleBackgroundPosition backgroundPositionY { get; set; }

		StyleBackgroundRepeat backgroundRepeat { get; set; }

		StyleBackgroundSize backgroundSize { get; set; }

		StyleColor borderBottomColor { get; set; }

		StyleLength borderBottomLeftRadius { get; set; }

		StyleLength borderBottomRightRadius { get; set; }

		StyleFloat borderBottomWidth { get; set; }

		StyleColor borderLeftColor { get; set; }

		StyleFloat borderLeftWidth { get; set; }

		StyleColor borderRightColor { get; set; }

		StyleFloat borderRightWidth { get; set; }

		StyleColor borderTopColor { get; set; }

		StyleLength borderTopLeftRadius { get; set; }

		StyleLength borderTopRightRadius { get; set; }

		StyleFloat borderTopWidth { get; set; }

		StyleLength bottom { get; set; }

		StyleColor color { get; set; }

		StyleCursor cursor { get; set; }

		StyleEnum<DisplayStyle> display { get; set; }

		StyleList<FilterFunction> filter { get; set; }

		StyleLength flexBasis { get; set; }

		StyleEnum<FlexDirection> flexDirection { get; set; }

		StyleFloat flexGrow { get; set; }

		StyleFloat flexShrink { get; set; }

		StyleEnum<Wrap> flexWrap { get; set; }

		StyleLength fontSize { get; set; }

		StyleLength height { get; set; }

		StyleEnum<Justify> justifyContent { get; set; }

		StyleLength left { get; set; }

		StyleLength letterSpacing { get; set; }

		StyleLength marginBottom { get; set; }

		StyleLength marginLeft { get; set; }

		StyleLength marginRight { get; set; }

		StyleLength marginTop { get; set; }

		StyleLength maxHeight { get; set; }

		StyleLength maxWidth { get; set; }

		StyleLength minHeight { get; set; }

		StyleLength minWidth { get; set; }

		StyleFloat opacity { get; set; }

		StyleEnum<Overflow> overflow { get; set; }

		StyleLength paddingBottom { get; set; }

		StyleLength paddingLeft { get; set; }

		StyleLength paddingRight { get; set; }

		StyleLength paddingTop { get; set; }

		StyleEnum<Position> position { get; set; }

		StyleLength right { get; set; }

		StyleRotate rotate { get; set; }

		StyleScale scale { get; set; }

		StyleEnum<TextOverflow> textOverflow { get; set; }

		StyleTextShadow textShadow { get; set; }

		StyleLength top { get; set; }

		StyleTransformOrigin transformOrigin { get; set; }

		StyleList<TimeValue> transitionDelay { get; set; }

		StyleList<TimeValue> transitionDuration { get; set; }

		StyleList<StylePropertyName> transitionProperty { get; set; }

		StyleList<EasingFunction> transitionTimingFunction { get; set; }

		StyleTranslate translate { get; set; }

		StyleColor unityBackgroundImageTintColor { get; set; }

		StyleEnum<EditorTextRenderingMode> unityEditorTextRenderingMode { get; set; }

		StyleFont unityFont { get; set; }

		StyleFontDefinition unityFontDefinition { get; set; }

		StyleEnum<FontStyle> unityFontStyleAndWeight { get; set; }

		StyleMaterialDefinition unityMaterial { get; set; }

		StyleEnum<OverflowClipBox> unityOverflowClipBox { get; set; }

		StyleLength unityParagraphSpacing { get; set; }

		StyleInt unitySliceBottom { get; set; }

		StyleInt unitySliceLeft { get; set; }

		StyleInt unitySliceRight { get; set; }

		StyleFloat unitySliceScale { get; set; }

		StyleInt unitySliceTop { get; set; }

		StyleEnum<SliceType> unitySliceType { get; set; }

		StyleEnum<TextAnchor> unityTextAlign { get; set; }

		StyleTextAutoSize unityTextAutoSize { get; set; }

		StyleEnum<TextGeneratorType> unityTextGenerator { get; set; }

		StyleColor unityTextOutlineColor { get; set; }

		StyleFloat unityTextOutlineWidth { get; set; }

		StyleEnum<TextOverflowPosition> unityTextOverflowPosition { get; set; }

		StyleEnum<Visibility> visibility { get; set; }

		StyleEnum<WhiteSpace> whiteSpace { get; set; }

		StyleLength width { get; set; }

		StyleLength wordSpacing { get; set; }

		[Obsolete("unityBackgroundScaleMode is deprecated. Use background-* properties instead.")]
		StyleEnum<ScaleMode> unityBackgroundScaleMode { get; set; }
	}
}
