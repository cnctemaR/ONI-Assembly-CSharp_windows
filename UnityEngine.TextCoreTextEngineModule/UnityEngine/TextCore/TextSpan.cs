using System;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct TextSpan
	{
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				string.Format("{0}: {1}\n", "color", this.color),
				string.Format("{0}: {1}\n", "fontStyle", this.fontStyle),
				string.Format("{0}: {1}\n", "fontWeight", this.fontWeight),
				string.Format("{0}: {1}\n", "linkID", this.linkID),
				string.Format("{0}: {1}\n", "fontSize", this.fontSize),
				string.Format("{0}: {1}", "fontAsset", this.fontAsset),
				string.Format("{0}: {1}\n", "startIndex", this.startIndex),
				string.Format("{0}: {1}", "length", this.length)
			});
		}

		public int startIndex;

		public int length;

		public IntPtr fontAsset;

		public int fontSize;

		public Color32 color;

		public FontStyles fontStyle;

		public TextFontWeight fontWeight;

		public int mspace;

		public RichTextTagParser.TagUnitType mspaceUnitType;

		public int cspace;

		public RichTextTagParser.TagUnitType cspaceUnitType;

		public int linkID;

		public HorizontalAlignment alignment;

		public Color32 highlightColor;

		public Vector4 highlightPadding;

		public GlyphMetrics spriteMetrics;

		public int spriteID;

		public bool spriteTint;

		public int spriteScale;

		public Color32 spriteColor;

		public int margin;

		public MarginDirection marginDirection;

		public RichTextTagParser.TagUnitType marginUnitType;
	}
}
