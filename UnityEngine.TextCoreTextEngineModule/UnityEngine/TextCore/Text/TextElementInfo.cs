using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal struct TextElementInfo
	{
		public override string ToString()
		{
			return string.Format("{0}: {1}\n{2}: {3}\n{4}: {5}\n{6}: {7}\n{8}: {9}\n{10}: {11}\n{12}: {13}\n{14}: {15}\n{16}: {17}\n{18}: {19}\n{20}: {21}\n{22}: {23}\n{24}: {25}\n{26}: {27}\n{28}: {29}\n{30}: {31}\n{32}: {33}\n{34}: {35}\n{36}: {37}\n{38}: {39}\n{40}: {41}\n{42}: {43}\n{44}: {45}\n{46}: {47}\n{48}: {49}\n{50}: {51}\n{52}: {53}\n{54}: {55}\n{56}: {57}\n{58}: {59}\n{60}: {61}\n{62}: {63}\n{64}: {65}\n{66}: {67}\n{68}: {69}\n{70}: {71}\n{72}: {73}\n{74}: {75}\n{76}: {77}", new object[]
			{
				"character", this.character, "index", this.index, "elementType", this.elementType, "stringLength", this.stringLength, "textElement", this.textElement,
				"alternativeGlyph", this.alternativeGlyph, "fontAsset", this.fontAsset, "spriteAsset", this.spriteAsset, "material", this.material, "materialReferenceIndex", this.materialReferenceIndex,
				"isUsingAlternateTypeface", this.isUsingAlternateTypeface, "pointSize", this.pointSize, "lineNumber", this.lineNumber, "vertexIndex", this.vertexIndex, "vertexTopLeft", this.vertexTopLeft,
				"vertexBottomLeft", this.vertexBottomLeft, "vertexTopRight", this.vertexTopRight, "vertexBottomRight", this.vertexBottomRight, "topLeft", this.topLeft, "bottomLeft", this.bottomLeft,
				"topRight", this.topRight, "bottomRight", this.bottomRight, "origin", this.origin, "ascender", this.ascender, "baseLine", this.baseLine,
				"descender", this.descender, "adjustedAscender", this.adjustedAscender, "adjustedDescender", this.adjustedDescender, "adjustedHorizontalAdvance", this.adjustedHorizontalAdvance, "xAdvance", this.xAdvance,
				"aspectRatio", this.aspectRatio, "scale", this.scale, "color", this.color, "underlineColor", this.underlineColor, "strikethroughColor", this.strikethroughColor,
				"highlightColor", this.highlightColor, "highlightState", this.highlightState, "style", this.style, "isVisible", this.isVisible
			});
		}

		internal string ToStringTest()
		{
			return string.Concat(new string[]
			{
				"topLeft.x: ",
				this.topLeft.x.ToString("F4"),
				"\n topLeft.y: ",
				this.topLeft.y.ToString("F4"),
				"\n topRight.x: ",
				this.topRight.x.ToString("F4"),
				"\n topRight.y: ",
				this.topRight.y.ToString("F4"),
				"\n  bottomLeft.x: ",
				this.bottomLeft.x.ToString("F4"),
				"\n bottomLeft.y: ",
				this.bottomLeft.y.ToString("F4"),
				"\n  bottomRight.x: ",
				this.bottomRight.x.ToString("F4"),
				"\n bottomRight.y: ",
				this.bottomRight.y.ToString("F4"),
				"\norigin: ",
				this.origin.ToString("F4"),
				"\nxAdvance: ",
				this.xAdvance.ToString("F4"),
				"\n"
			});
		}

		public uint character;

		public int index;

		public TextElementType elementType;

		public int stringLength;

		public TextElement textElement;

		public Glyph alternativeGlyph;

		public FontAsset fontAsset;

		public SpriteAsset spriteAsset;

		public Material material;

		public int materialReferenceIndex;

		public bool isUsingAlternateTypeface;

		public float pointSize;

		public int lineNumber;

		public int vertexIndex;

		public TextVertex vertexTopLeft;

		public TextVertex vertexBottomLeft;

		public TextVertex vertexTopRight;

		public TextVertex vertexBottomRight;

		public Vector3 topLeft;

		public Vector3 bottomLeft;

		public Vector3 topRight;

		public Vector3 bottomRight;

		public float origin;

		public float ascender;

		public float baseLine;

		public float descender;

		internal float adjustedAscender;

		internal float adjustedDescender;

		internal float adjustedHorizontalAdvance;

		public float xAdvance;

		public float aspectRatio;

		public float scale;

		public Color32 color;

		public Color32 underlineColor;

		public int underlineVertexIndex;

		public Color32 strikethroughColor;

		public int strikethroughVertexIndex;

		public Color32 highlightColor;

		public HighlightState highlightState;

		public FontStyles style;

		public bool isVisible;
	}
}
