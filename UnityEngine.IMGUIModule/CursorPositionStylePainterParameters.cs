using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct CursorPositionStylePainterParameters
	{
		public Rect rect;

		public string text;

		public Font font;

		public int fontSize;

		public FontStyle fontStyle;

		public TextAnchor anchor;

		public float wordWrapWidth;

		public bool richText;

		public int cursorIndex;
	}
}
