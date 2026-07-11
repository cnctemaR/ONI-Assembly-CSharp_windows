using System;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	public interface IStyle
	{
		/// <summary>
		///   <para>Fixed width of an element for the layout.</para>
		/// </summary>
		StyleValue<float> width { get; set; }

		/// <summary>
		///   <para>Fixed height of an element for the layout.</para>
		/// </summary>
		StyleValue<float> height { get; set; }

		/// <summary>
		///   <para>Maximum width for an element, when it is flexible or measures its own size.</para>
		/// </summary>
		StyleValue<float> maxWidth { get; set; }

		/// <summary>
		///   <para>Maximum height for an element, when it is flexible or measures its own size.</para>
		/// </summary>
		StyleValue<float> maxHeight { get; set; }

		/// <summary>
		///   <para>Minimum height for an element, when it is flexible or measures its own size.</para>
		/// </summary>
		StyleValue<float> minWidth { get; set; }

		/// <summary>
		///   <para>Minimum height for an element, when it is flexible or measures its own size.</para>
		/// </summary>
		StyleValue<float> minHeight { get; set; }

		/// <summary>
		///   <para>Ration of this element in its parent during the layout phase.</para>
		/// </summary>
		StyleValue<float> flex { get; set; }

		/// <summary>
		///   <para>Initial main size of a flex item, on the main flex axis. The final layout mught be smaller or larger, according to the flex shrinking and growing determined by the flex property.</para>
		/// </summary>
		StyleValue<float> flexBasis { get; set; }

		/// <summary>
		///   <para>Specifies how much the item will grow relative to the rest of the flexible items inside the same container.</para>
		/// </summary>
		StyleValue<float> flexGrow { get; set; }

		/// <summary>
		///   <para>Specifies how the item will shrink relative to the rest of the flexible items inside the same container.</para>
		/// </summary>
		StyleValue<float> flexShrink { get; set; }

		/// <summary>
		///   <para>Direction of the main axis to layout children in a container.</para>
		/// </summary>
		StyleValue<FlexDirection> flexDirection { get; set; }

		/// <summary>
		///   <para>Placement of children over multiple lines if not enough space is available in this container.</para>
		/// </summary>
		StyleValue<Wrap> flexWrap { get; set; }

		StyleValue<Overflow> overflow { get; set; }

		/// <summary>
		///   <para>Left distance from the element's box during layout.</para>
		/// </summary>
		StyleValue<float> positionLeft { get; set; }

		/// <summary>
		///   <para>Top distance from the element's box during layout.</para>
		/// </summary>
		StyleValue<float> positionTop { get; set; }

		/// <summary>
		///   <para>Right distance from the element's box during layout.</para>
		/// </summary>
		StyleValue<float> positionRight { get; set; }

		/// <summary>
		///   <para>Bottom distance from the element's box during layout.</para>
		/// </summary>
		StyleValue<float> positionBottom { get; set; }

		/// <summary>
		///   <para>Space reserved for the left edge of the margin during the layout phase.</para>
		/// </summary>
		StyleValue<float> marginLeft { get; set; }

		/// <summary>
		///   <para>Space reserved for the top edge of the margin during the layout phase.</para>
		/// </summary>
		StyleValue<float> marginTop { get; set; }

		/// <summary>
		///   <para>Space reserved for the right edge of the margin during the layout phase.</para>
		/// </summary>
		StyleValue<float> marginRight { get; set; }

		/// <summary>
		///   <para>Space reserved for the bottom edge of the margin during the layout phase.</para>
		/// </summary>
		StyleValue<float> marginBottom { get; set; }

		/// <summary>
		///   <para>Space reserved for the left edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderLeft { get; set; }

		/// <summary>
		///   <para>Space reserved for the top edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderTop { get; set; }

		/// <summary>
		///   <para>Space reserved for the right edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderRight { get; set; }

		/// <summary>
		///   <para>Space reserved for the bottom edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderBottom { get; set; }

		/// <summary>
		///   <para>Space reserved for the left edge of the padding during the layout phase.</para>
		/// </summary>
		StyleValue<float> paddingLeft { get; set; }

		/// <summary>
		///   <para>Space reserved for the top edge of the padding during the layout phase.</para>
		/// </summary>
		StyleValue<float> paddingTop { get; set; }

		/// <summary>
		///   <para>Space reserved for the right edge of the padding during the layout phase.</para>
		/// </summary>
		StyleValue<float> paddingRight { get; set; }

		/// <summary>
		///   <para>Space reserved for the bottom edge of the padding during the layout phase.</para>
		/// </summary>
		StyleValue<float> paddingBottom { get; set; }

		/// <summary>
		///   <para>Element's positioning in its parent container.</para>
		/// </summary>
		StyleValue<PositionType> positionType { get; set; }

		/// <summary>
		///   <para>Similar to align-items, but only for this specific element.</para>
		/// </summary>
		StyleValue<Align> alignSelf { get; set; }

		/// <summary>
		///   <para>Text alignment in the element's box.</para>
		/// </summary>
		StyleValue<TextAnchor> textAlignment { get; set; }

		/// <summary>
		///   <para>Font style to draw the element's text.</para>
		/// </summary>
		StyleValue<FontStyle> fontStyle { get; set; }

		/// <summary>
		///   <para>Clipping if the text does not fit in the element's box.</para>
		/// </summary>
		StyleValue<TextClipping> textClipping { get; set; }

		/// <summary>
		///   <para>Font to draw the element's text.</para>
		/// </summary>
		StyleValue<Font> font { get; set; }

		/// <summary>
		///   <para>Font size to draw the element's text.</para>
		/// </summary>
		StyleValue<int> fontSize { get; set; }

		/// <summary>
		///   <para>Word wrapping over multiple lines if not enough space is available to draw the text of an element.</para>
		/// </summary>
		StyleValue<bool> wordWrap { get; set; }

		/// <summary>
		///   <para>Color to use when drawing the text of an element.</para>
		/// </summary>
		StyleValue<Color> textColor { get; set; }

		/// <summary>
		///   <para>Background color to paint in the element's box.</para>
		/// </summary>
		StyleValue<Color> backgroundColor { get; set; }

		/// <summary>
		///   <para>Color of the border to paint inside the element's box.</para>
		/// </summary>
		StyleValue<Color> borderColor { get; set; }

		/// <summary>
		///   <para>Background image to paint in the element's box.</para>
		/// </summary>
		StyleValue<Texture2D> backgroundImage { get; set; }

		/// <summary>
		///   <para>Background image scaling in the element's box.</para>
		/// </summary>
		StyleValue<ScaleMode> backgroundSize { get; set; }

		/// <summary>
		///   <para>Alignment of children on the cross axis of this container.</para>
		/// </summary>
		StyleValue<Align> alignItems { get; set; }

		/// <summary>
		///   <para>Alignment of the whole area of children on the cross axis if they span over multiple lines in this container.</para>
		/// </summary>
		StyleValue<Align> alignContent { get; set; }

		/// <summary>
		///   <para>Justification of children on the main axis of this container.</para>
		/// </summary>
		StyleValue<Justify> justifyContent { get; set; }

		/// <summary>
		///   <para>Space reserved for the left edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderLeftWidth { get; set; }

		/// <summary>
		///   <para>Space reserved for the top edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderTopWidth { get; set; }

		/// <summary>
		///   <para>Space reserved for the right edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderRightWidth { get; set; }

		/// <summary>
		///   <para>Space reserved for the bottom edge of the border during the layout phase.</para>
		/// </summary>
		StyleValue<float> borderBottomWidth { get; set; }

		/// <summary>
		///   <para>This is the radius of every corner when a rounded rectangle is drawn in the element's box.</para>
		/// </summary>
		StyleValue<float> borderRadius { get; set; }

		/// <summary>
		///   <para>This is the radius of the top-left corner when a rounded rectangle is drawn in the element's box.</para>
		/// </summary>
		StyleValue<float> borderTopLeftRadius { get; set; }

		/// <summary>
		///   <para>This is the radius of the top-right corner when a rounded rectangle is drawn in the element's box.</para>
		/// </summary>
		StyleValue<float> borderTopRightRadius { get; set; }

		/// <summary>
		///   <para>This is the radius of the bottom-right corner when a rounded rectangle is drawn in the element's box.</para>
		/// </summary>
		StyleValue<float> borderBottomRightRadius { get; set; }

		/// <summary>
		///   <para>This is the radius of the bottom-left corner when a rounded rectangle is drawn in the element's box.</para>
		/// </summary>
		StyleValue<float> borderBottomLeftRadius { get; set; }

		/// <summary>
		///   <para>Size of the 9-slice's left edge when painting an element's background image.</para>
		/// </summary>
		StyleValue<int> sliceLeft { get; set; }

		/// <summary>
		///   <para>Size of the 9-slice's top edge when painting an element's background image.</para>
		/// </summary>
		StyleValue<int> sliceTop { get; set; }

		/// <summary>
		///   <para>Size of the 9-slice's right edge when painting an element's background image.</para>
		/// </summary>
		StyleValue<int> sliceRight { get; set; }

		/// <summary>
		///   <para>Size of the 9-slice's bottom edge when painting an element's background image.</para>
		/// </summary>
		StyleValue<int> sliceBottom { get; set; }

		StyleValue<float> opacity { get; set; }

		/// <summary>
		///   <para>Mouse cursor to display when the mouse pointer is over an element.</para>
		/// </summary>
		StyleValue<CursorStyle> cursor { get; set; }

		/// <summary>
		///   <para>Specifies whether or not an element is visible.</para>
		/// </summary>
		StyleValue<Visibility> visibility { get; set; }
	}
}
