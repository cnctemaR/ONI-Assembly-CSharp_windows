using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct RectStylePainterParameters
	{
		public Rect rect;

		public Color color;

		public BorderParameters border;
	}
}
