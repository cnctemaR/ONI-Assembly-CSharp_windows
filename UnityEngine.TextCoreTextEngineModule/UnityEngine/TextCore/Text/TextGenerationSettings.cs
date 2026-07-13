using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal class TextGenerationSettings : IEquatable<TextGenerationSettings>
	{
		public RenderedText renderedText
		{
			get
			{
				return this.m_RenderedText;
			}
			set
			{
				this.m_RenderedText = value;
				this.m_CachedRenderedText = null;
			}
		}

		public string text
		{
			get
			{
				string text;
				if ((text = this.m_CachedRenderedText) == null)
				{
					text = (this.m_CachedRenderedText = this.renderedText.CreateString());
				}
				return text;
			}
			set
			{
				this.renderedText = new RenderedText(value);
			}
		}

		public TextGenerationSettings()
		{
		}

		internal TextGenerationSettings(TextGenerationSettings tgs)
		{
			this.m_RenderedText = tgs.m_RenderedText;
			this.m_CachedRenderedText = tgs.m_CachedRenderedText;
			this.screenRect = tgs.screenRect;
			this.fontAsset = tgs.fontAsset;
			this.fontStyle = tgs.fontStyle;
			this.textSettings = tgs.textSettings;
			this.textAlignment = tgs.textAlignment;
			this.overflowMode = tgs.overflowMode;
			this.shouldConvertToLinearSpace = tgs.shouldConvertToLinearSpace;
			this.fontSize = tgs.fontSize;
			this.emojiFallbackSupport = tgs.emojiFallbackSupport;
			this.richText = tgs.richText;
			this.isRightToLeft = tgs.isRightToLeft;
			this.extraPadding = tgs.extraPadding;
			this.parseControlCharacters = tgs.parseControlCharacters;
			this.isPlaceholder = tgs.isPlaceholder;
			this.characterSpacing = tgs.characterSpacing;
			this.wordSpacing = tgs.wordSpacing;
			this.paragraphSpacing = tgs.paragraphSpacing;
			this.textWrappingMode = tgs.textWrappingMode;
			this.fontWeight = tgs.fontWeight;
			this.isIMGUI = tgs.isIMGUI;
		}

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
				flag2 = flag3 || (this.m_RenderedText.Equals(other.m_RenderedText) && this.screenRect.Equals(other.screenRect) && object.Equals(this.fontAsset, other.fontAsset) && this.fontStyle == other.fontStyle && object.Equals(this.textSettings, other.textSettings) && this.textAlignment == other.textAlignment && this.overflowMode == other.overflowMode && this.color.Equals(other.color) && this.fontSize.Equals(other.fontSize) && this.shouldConvertToLinearSpace == other.shouldConvertToLinearSpace && this.emojiFallbackSupport == other.emojiFallbackSupport && this.richText == other.richText && this.isRightToLeft == other.isRightToLeft && this.extraPadding == other.extraPadding && this.parseControlCharacters == other.parseControlCharacters && this.isPlaceholder == other.isPlaceholder && this.characterSpacing.Equals(other.characterSpacing) && this.wordSpacing.Equals(other.wordSpacing) && this.paragraphSpacing.Equals(other.paragraphSpacing) && this.textWrappingMode == other.textWrappingMode && this.fontWeight == other.fontWeight && this.isIMGUI == other.isIMGUI);
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
			hashCode.Add<RenderedText>(this.m_RenderedText);
			hashCode.Add<Rect>(this.screenRect);
			hashCode.Add<FontAsset>(this.fontAsset);
			hashCode.Add<int>((int)this.fontStyle);
			hashCode.Add<TextSettings>(this.textSettings);
			hashCode.Add<int>((int)this.textAlignment);
			hashCode.Add<int>((int)this.overflowMode);
			hashCode.Add<Color>(this.color);
			hashCode.Add<bool>(this.shouldConvertToLinearSpace);
			hashCode.Add<int>(this.fontSize);
			hashCode.Add<bool>(this.emojiFallbackSupport);
			hashCode.Add<bool>(this.richText);
			hashCode.Add<bool>(this.isRightToLeft);
			hashCode.Add<float>(this.extraPadding);
			hashCode.Add<bool>(this.parseControlCharacters);
			hashCode.Add<bool>(this.isPlaceholder);
			hashCode.Add<float>(this.characterSpacing);
			hashCode.Add<float>(this.wordSpacing);
			hashCode.Add<float>(this.paragraphSpacing);
			hashCode.Add<int>((int)this.textWrappingMode);
			hashCode.Add<int>((int)this.fontWeight);
			hashCode.Add<bool>(this.isIMGUI);
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
			return string.Concat(new string[]
			{
				string.Format("{0}: {1}\n {2}: {3}\n {4}: {5}\n ", new object[] { "text", this.text, "screenRect", this.screenRect, "fontAsset", this.fontAsset }),
				string.Format("{0}: {1}\n {2}: {3}\n {4}: {5}\n {6}: {7}\n {8}: {9}\n ", new object[] { "fontStyle", this.fontStyle, "textSettings", this.textSettings, "textAlignment", this.textAlignment, "overflowMode", this.overflowMode, "textWrappingMode", this.textWrappingMode }),
				string.Format("{0}: {1}\n {2}: {3}\n {4}: {5}\n {6}: {7}\n {8}: {9}\n ", new object[] { "color", this.color, "fontSize", this.fontSize, "richText", this.richText, "isRightToLeft", this.isRightToLeft, "extraPadding", this.extraPadding }),
				string.Format("{0}: {1}\n {2}: {3}\n {4}: {5}\n {6}: {7}\n ", new object[] { "parseControlCharacters", this.parseControlCharacters, "characterSpacing", this.characterSpacing, "wordSpacing", this.wordSpacing, "paragraphSpacing", this.paragraphSpacing }),
				string.Format("{0}: {1}\n {2}: {3}\n {4}: {5}\n {6}: {7}", new object[] { "textWrappingMode", this.textWrappingMode, "fontWeight", this.fontWeight, "shouldConvertToLinearSpace", this.shouldConvertToLinearSpace, "isPlaceholder", this.isPlaceholder })
			});
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal static Func<bool> IsEditorTextRenderingModeBitmap;

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal static Func<bool> IsEditorTextRenderingModeRaster;

		private RenderedText m_RenderedText;

		private string m_CachedRenderedText;

		public Rect screenRect;

		public FontAsset fontAsset;

		public FontStyles fontStyle = FontStyles.Normal;

		public TextSettings textSettings;

		public TextAlignment textAlignment = TextAlignment.TopLeft;

		public TextOverflowMode overflowMode = TextOverflowMode.Overflow;

		public const float wordWrappingRatio = 0.4f;

		public Color color = Color.white;

		public bool shouldConvertToLinearSpace = true;

		public int fontSize = 18;

		public const bool autoSize = false;

		public const float fontSizeMin = 0f;

		public const float fontSizeMax = 0f;

		internal static readonly List<OTL_FeatureTag> fontFeatures = new List<OTL_FeatureTag> { OTL_FeatureTag.kern };

		public bool emojiFallbackSupport = true;

		public bool richText;

		public bool isRightToLeft;

		public float extraPadding = 6f;

		public bool parseControlCharacters = true;

		public bool isPlaceholder = false;

		public const bool tagNoParsing = false;

		public float characterSpacing;

		public float wordSpacing;

		public const float lineSpacing = 0f;

		public float paragraphSpacing;

		public const float lineSpacingMax = 0f;

		public TextWrappingMode textWrappingMode = TextWrappingMode.Normal;

		public const int maxVisibleCharacters = 99999;

		public const int maxVisibleWords = 99999;

		public const int maxVisibleLines = 99999;

		public const int firstVisibleCharacter = 0;

		public const bool useMaxVisibleDescender = false;

		public TextFontWeight fontWeight = TextFontWeight.Regular;

		public bool isIMGUI;

		public const float charWidthMaxAdj = 0f;

		public float pixelsPerPoint = 1f;
	}
}
