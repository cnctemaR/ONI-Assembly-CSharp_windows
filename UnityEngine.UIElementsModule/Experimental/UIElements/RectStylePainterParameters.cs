using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct RectStylePainterParameters
	{
		public static RectStylePainterParameters GetDefault(VisualElement ve)
		{
			IStyle style = ve.style;
			RectStylePainterParameters rectStylePainterParameters = new RectStylePainterParameters
			{
				rect = GUIUtility.AlignRectToDevice(ve.rect),
				color = style.backgroundColor
			};
			BorderParameters.SetFromStyle(ref rectStylePainterParameters.border, style);
			return rectStylePainterParameters;
		}

		public Rect rect;

		public Color color;

		public BorderParameters border;
	}
}
