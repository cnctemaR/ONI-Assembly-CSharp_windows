using System;

namespace UnityEngine.TextCore.Text
{
	internal class TextGenerationSettings : IEquatable<TextGenerationSettings>
	{
		public bool Equals(TextGenerationSettings other)
		{
			bool flag = other == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this == other;
				flag2 = flag3 || (this.text == other.text && this.screenRect.Equals(other.screenRect) && this.margins.Equals(other.margins) && this.scale.Equals(other.scale) && object.Equals(this.fontAsset, other.fontAsset) && object.Equals(this.material, other.material) && object.Equals(this.spriteAsset, other.spriteAsset) && object.Equals(this.styleSheet, other.styleSheet) && this.fontStyle == other.fontStyle && object.Equals(this.textSettings, other.textSettings) && this.textAlignment == other.textAlignment && this.overflowMode == other.overflowMode && this.wordWrap == other.wordWrap && this.wordWrappingRatio.Equals(other.wordWrappingRatio) && this.color.Equals(other.color) && object.Equals(this.fontColorGradient, other.fontColorGradient) && object.Equals(this.fontColorGradientPreset, other.fontColorGradientPreset) && this.tintSprites == other.tintSprites && this.overrideRichTextColors == other.overrideRichTextColors && this.shouldConvertToLinearSpace == other.shouldConvertToLinearSpace && this.fontSize.Equals(other.fontSize) && this.autoSize == other.autoSize && this.fontSizeMin.Equals(other.fontSizeMin) && this.fontSizeMax.Equals(other.fontSizeMax) && this.enableKerning == other.enableKerning && this.richText == other.richText && this.isRightToLeft == other.isRightToLeft && this.extraPadding == other.extraPadding && this.parseControlCharacters == other.parseControlCharacters && this.isOrthographic == other.isOrthographic && this.tagNoParsing == other.tagNoParsing && this.characterSpacing.Equals(other.characterSpacing) && this.wordSpacing.Equals(other.wordSpacing) && this.lineSpacing.Equals(other.lineSpacing) && this.paragraphSpacing.Equals(other.paragraphSpacing) && this.lineSpacingMax.Equals(other.lineSpacingMax) && this.textWrappingMode == other.textWrappingMode && this.maxVisibleCharacters == other.maxVisibleCharacters && this.maxVisibleWords == other.maxVisibleWords && this.maxVisibleLines == other.maxVisibleLines && this.firstVisibleCharacter == other.firstVisibleCharacter && this.useMaxVisibleDescender == other.useMaxVisibleDescender && this.fontWeight == other.fontWeight && this.pageToDisplay == other.pageToDisplay && this.horizontalMapping == other.horizontalMapping && this.verticalMapping == other.verticalMapping && this.uvLineOffset.Equals(other.uvLineOffset) && this.geometrySortingOrder == other.geometrySortingOrder && this.inverseYAxis == other.inverseYAxis && this.charWidthMaxAdj.Equals(other.charWidthMaxAdj) && this.inputSource == other.inputSource);
			}
			return flag2;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this == obj;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = obj.GetType() != base.GetType();
					flag2 = !flag4 && this.Equals((TextGenerationSettings)obj);
				}
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			HashCode hashCode = default(HashCode);
			hashCode.Add<string>(this.text);
			hashCode.Add<Rect>(this.screenRect);
			hashCode.Add<Vector4>(this.margins);
			hashCode.Add<float>(this.scale);
			hashCode.Add<FontAsset>(this.fontAsset);
			hashCode.Add<Material>(this.material);
			hashCode.Add<SpriteAsset>(this.spriteAsset);
			hashCode.Add<TextStyleSheet>(this.styleSheet);
			hashCode.Add<int>((int)this.fontStyle);
			hashCode.Add<TextSettings>(this.textSettings);
			hashCode.Add<int>((int)this.textAlignment);
			hashCode.Add<int>((int)this.overflowMode);
			hashCode.Add<bool>(this.wordWrap);
			hashCode.Add<float>(this.wordWrappingRatio);
			hashCode.Add<Color>(this.color);
			hashCode.Add<TextColorGradient>(this.fontColorGradient);
			hashCode.Add<TextColorGradient>(this.fontColorGradientPreset);
			hashCode.Add<bool>(this.tintSprites);
			hashCode.Add<bool>(this.overrideRichTextColors);
			hashCode.Add<bool>(this.shouldConvertToLinearSpace);
			hashCode.Add<float>(this.fontSize);
			hashCode.Add<bool>(this.autoSize);
			hashCode.Add<float>(this.fontSizeMin);
			hashCode.Add<float>(this.fontSizeMax);
			hashCode.Add<bool>(this.enableKerning);
			hashCode.Add<bool>(this.richText);
			hashCode.Add<bool>(this.isRightToLeft);
			hashCode.Add<float>(this.extraPadding);
			hashCode.Add<bool>(this.parseControlCharacters);
			hashCode.Add<bool>(this.isOrthographic);
			hashCode.Add<bool>(this.tagNoParsing);
			hashCode.Add<float>(this.characterSpacing);
			hashCode.Add<float>(this.wordSpacing);
			hashCode.Add<float>(this.lineSpacing);
			hashCode.Add<float>(this.paragraphSpacing);
			hashCode.Add<float>(this.lineSpacingMax);
			hashCode.Add<int>((int)this.textWrappingMode);
			hashCode.Add<int>(this.maxVisibleCharacters);
			hashCode.Add<int>(this.maxVisibleWords);
			hashCode.Add<int>(this.maxVisibleLines);
			hashCode.Add<int>(this.firstVisibleCharacter);
			hashCode.Add<bool>(this.useMaxVisibleDescender);
			hashCode.Add<int>((int)this.fontWeight);
			hashCode.Add<int>(this.pageToDisplay);
			hashCode.Add<int>((int)this.horizontalMapping);
			hashCode.Add<int>((int)this.verticalMapping);
			hashCode.Add<float>(this.uvLineOffset);
			hashCode.Add<int>((int)this.geometrySortingOrder);
			hashCode.Add<bool>(this.inverseYAxis);
			hashCode.Add<float>(this.charWidthMaxAdj);
			hashCode.Add<int>((int)this.inputSource);
			return hashCode.ToHashCode();
		}

		public static bool operator ==(TextGenerationSettings left, TextGenerationSettings right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(TextGenerationSettings left, TextGenerationSettings right)
		{
			return !object.Equals(left, right);
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}\n {2}: {3}\n {4}: {5}\n {6}: {7}\n {8}: {9}\n {10}: {11}\n {12}: {13}\n {14}: {15}\n {16}: {17}\n {18}: {19}\n {20}: {21}\n {22}: {23}\n {24}: {25}\n {26}: {27}\n {28}: {29}\n {30}: {31}\n {32}: {33}\n {34}: {35}\n {36}: {37}\n {38}: {39}\n {40}: {41}\n {42}: {43}\n {44}: {45}\n {46}: {47}\n {48}: {49}\n {50}: {51}\n {52}: {53}\n {54}: {55}\n {56}: {57}\n {58}: {59}\n {60}: {61}\n {62}: {63}\n {64}: {65}\n {66}: {67}\n {68}: {69}\n {70}: {71}\n {72}: {73}\n {74}: {75}\n {76}: {77}\n {78}: {79}\n {80}: {81}\n {82}: {83}\n {84}: {85}\n {86}: {87}\n {88}: {89}\n {90}: {91}\n {92}: {93}\n {94}: {95}\n {96}: {97}\n {98}: {99}\n {100}: {101}", new object[]
			{
				"text", this.text, "screenRect", this.screenRect, "margins", this.margins, "scale", this.scale, "fontAsset", this.fontAsset,
				"material", this.material, "spriteAsset", this.spriteAsset, "styleSheet", this.styleSheet, "fontStyle", this.fontStyle, "textSettings", this.textSettings,
				"textAlignment", this.textAlignment, "overflowMode", this.overflowMode, "wordWrap", this.wordWrap, "wordWrappingRatio", this.wordWrappingRatio, "color", this.color,
				"fontColorGradient", this.fontColorGradient, "fontColorGradientPreset", this.fontColorGradientPreset, "tintSprites", this.tintSprites, "overrideRichTextColors", this.overrideRichTextColors, "shouldConvertToLinearSpace", this.shouldConvertToLinearSpace,
				"fontSize", this.fontSize, "autoSize", this.autoSize, "fontSizeMin", this.fontSizeMin, "fontSizeMax", this.fontSizeMax, "enableKerning", this.enableKerning,
				"richText", this.richText, "isRightToLeft", this.isRightToLeft, "extraPadding", this.extraPadding, "parseControlCharacters", this.parseControlCharacters, "isOrthographic", this.isOrthographic,
				"tagNoParsing", this.tagNoParsing, "characterSpacing", this.characterSpacing, "wordSpacing", this.wordSpacing, "lineSpacing", this.lineSpacing, "paragraphSpacing", this.paragraphSpacing,
				"lineSpacingMax", this.lineSpacingMax, "textWrappingMode", this.textWrappingMode, "maxVisibleCharacters", this.maxVisibleCharacters, "maxVisibleWords", this.maxVisibleWords, "maxVisibleLines", this.maxVisibleLines,
				"firstVisibleCharacter", this.firstVisibleCharacter, "useMaxVisibleDescender", this.useMaxVisibleDescender, "fontWeight", this.fontWeight, "pageToDisplay", this.pageToDisplay, "horizontalMapping", this.horizontalMapping,
				"verticalMapping", this.verticalMapping, "uvLineOffset", this.uvLineOffset, "geometrySortingOrder", this.geometrySortingOrder, "inverseYAxis", this.inverseYAxis, "charWidthMaxAdj", this.charWidthMaxAdj,
				"inputSource", this.inputSource
			});
		}

		public string text;

		public Rect screenRect;

		public Vector4 margins;

		public float scale = 1f;

		public FontAsset fontAsset;

		public Material material;

		public SpriteAsset spriteAsset;

		public TextStyleSheet styleSheet;

		public FontStyles fontStyle = FontStyles.Normal;

		public TextSettings textSettings;

		public TextAlignment textAlignment = TextAlignment.TopLeft;

		public TextOverflowMode overflowMode = TextOverflowMode.Overflow;

		public bool wordWrap = false;

		public float wordWrappingRatio;

		public Color color = Color.white;

		public TextColorGradient fontColorGradient;

		public TextColorGradient fontColorGradientPreset;

		public bool tintSprites;

		public bool overrideRichTextColors;

		public bool shouldConvertToLinearSpace = true;

		public float fontSize = 18f;

		public bool autoSize;

		public float fontSizeMin;

		public float fontSizeMax;

		public bool enableKerning = true;

		public bool richText;

		public bool isRightToLeft;

		public float extraPadding = 6f;

		public bool parseControlCharacters = true;

		public bool isOrthographic = true;

		public bool tagNoParsing = false;

		public float characterSpacing;

		public float wordSpacing;

		public float lineSpacing;

		public float paragraphSpacing;

		public float lineSpacingMax;

		public TextWrappingMode textWrappingMode = TextWrappingMode.Normal;

		public int maxVisibleCharacters = 99999;

		public int maxVisibleWords = 99999;

		public int maxVisibleLines = 99999;

		public int firstVisibleCharacter = 0;

		public bool useMaxVisibleDescender;

		public TextFontWeight fontWeight = TextFontWeight.Regular;

		public int pageToDisplay = 1;

		public TextureMapping horizontalMapping = TextureMapping.Character;

		public TextureMapping verticalMapping = TextureMapping.Character;

		public float uvLineOffset;

		public VertexSortingOrder geometrySortingOrder = VertexSortingOrder.Normal;

		public bool inverseYAxis;

		public float charWidthMaxAdj;

		internal TextInputSource inputSource = TextInputSource.TextString;
	}
}
