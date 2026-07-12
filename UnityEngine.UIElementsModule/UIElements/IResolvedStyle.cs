using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public interface IResolvedStyle
	{
		Align alignContent { get; }

		Align alignItems { get; }

		Align alignSelf { get; }

		Color backgroundColor { get; }

		Background backgroundImage { get; }

		BackgroundPosition backgroundPositionX { get; }

		BackgroundPosition backgroundPositionY { get; }

		BackgroundRepeat backgroundRepeat { get; }

		BackgroundSize backgroundSize { get; }

		Color borderBottomColor { get; }

		float borderBottomLeftRadius { get; }

		float borderBottomRightRadius { get; }

		float borderBottomWidth { get; }

		Color borderLeftColor { get; }

		float borderLeftWidth { get; }

		Color borderRightColor { get; }

		float borderRightWidth { get; }

		Color borderTopColor { get; }

		float borderTopLeftRadius { get; }

		float borderTopRightRadius { get; }

		float borderTopWidth { get; }

		float bottom { get; }

		Color color { get; }

		DisplayStyle display { get; }

		StyleFloat flexBasis { get; }

		FlexDirection flexDirection { get; }

		float flexGrow { get; }

		float flexShrink { get; }

		Wrap flexWrap { get; }

		float fontSize { get; }

		float height { get; }

		Justify justifyContent { get; }

		float left { get; }

		float letterSpacing { get; }

		float marginBottom { get; }

		float marginLeft { get; }

		float marginRight { get; }

		float marginTop { get; }

		StyleFloat maxHeight { get; }

		StyleFloat maxWidth { get; }

		StyleFloat minHeight { get; }

		StyleFloat minWidth { get; }

		float opacity { get; }

		float paddingBottom { get; }

		float paddingLeft { get; }

		float paddingRight { get; }

		float paddingTop { get; }

		Position position { get; }

		float right { get; }

		Rotate rotate { get; }

		Scale scale { get; }

		TextOverflow textOverflow { get; }

		float top { get; }

		Vector3 transformOrigin { get; }

		IEnumerable<TimeValue> transitionDelay { get; }

		IEnumerable<TimeValue> transitionDuration { get; }

		IEnumerable<StylePropertyName> transitionProperty { get; }

		IEnumerable<EasingFunction> transitionTimingFunction { get; }

		Vector3 translate { get; }

		Color unityBackgroundImageTintColor { get; }

		Font unityFont { get; }

		FontDefinition unityFontDefinition { get; }

		FontStyle unityFontStyleAndWeight { get; }

		float unityParagraphSpacing { get; }

		int unitySliceBottom { get; }

		int unitySliceLeft { get; }

		int unitySliceRight { get; }

		float unitySliceScale { get; }

		int unitySliceTop { get; }

		TextAnchor unityTextAlign { get; }

		Color unityTextOutlineColor { get; }

		float unityTextOutlineWidth { get; }

		TextOverflowPosition unityTextOverflowPosition { get; }

		Visibility visibility { get; }

		WhiteSpace whiteSpace { get; }

		float width { get; }

		float wordSpacing { get; }

		[Obsolete("unityBackgroundScaleMode is deprecated. Use background-* properties instead.")]
		StyleEnum<ScaleMode> unityBackgroundScaleMode { get; }
	}
}
