using System;

namespace UnityEngine.UIElements
{
	internal struct InheritedData : IStyleDataGroup<InheritedData>, IEquatable<InheritedData>
	{
		public InheritedData Copy()
		{
			return this;
		}

		public void CopyFrom(ref InheritedData other)
		{
			this = other;
		}

		public static bool operator ==(InheritedData lhs, InheritedData rhs)
		{
			return lhs.color == rhs.color && lhs.fontSize == rhs.fontSize && lhs.letterSpacing == rhs.letterSpacing && lhs.textShadow == rhs.textShadow && lhs.unityEditorTextRenderingMode == rhs.unityEditorTextRenderingMode && lhs.unityFont == rhs.unityFont && lhs.unityFontDefinition == rhs.unityFontDefinition && lhs.unityFontStyleAndWeight == rhs.unityFontStyleAndWeight && lhs.unityMaterial == rhs.unityMaterial && lhs.unityParagraphSpacing == rhs.unityParagraphSpacing && lhs.unityTextAlign == rhs.unityTextAlign && lhs.unityTextAutoSize == rhs.unityTextAutoSize && lhs.unityTextGenerator == rhs.unityTextGenerator && lhs.unityTextOutlineColor == rhs.unityTextOutlineColor && lhs.unityTextOutlineWidth == rhs.unityTextOutlineWidth && lhs.visibility == rhs.visibility && lhs.whiteSpace == rhs.whiteSpace && lhs.wordSpacing == rhs.wordSpacing;
		}

		public static bool operator !=(InheritedData lhs, InheritedData rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(InheritedData other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is InheritedData && this.Equals((InheritedData)obj);
		}

		public override int GetHashCode()
		{
			int num = this.color.GetHashCode();
			num = (num * 397) ^ this.fontSize.GetHashCode();
			num = (num * 397) ^ this.letterSpacing.GetHashCode();
			num = (num * 397) ^ this.textShadow.GetHashCode();
			num = (num * 397) ^ (int)this.unityEditorTextRenderingMode;
			num = (num * 397) ^ ((this.unityFont == null) ? 0 : this.unityFont.GetHashCode());
			num = (num * 397) ^ this.unityFontDefinition.GetHashCode();
			num = (num * 397) ^ (int)this.unityFontStyleAndWeight;
			num = (num * 397) ^ ((this.unityMaterial == null) ? 0 : this.unityMaterial.GetHashCode());
			num = (num * 397) ^ this.unityParagraphSpacing.GetHashCode();
			num = (num * 397) ^ (int)this.unityTextAlign;
			num = (num * 397) ^ this.unityTextAutoSize.GetHashCode();
			num = (num * 397) ^ (int)this.unityTextGenerator;
			num = (num * 397) ^ this.unityTextOutlineColor.GetHashCode();
			num = (num * 397) ^ this.unityTextOutlineWidth.GetHashCode();
			num = (num * 397) ^ (int)this.visibility;
			num = (num * 397) ^ (int)this.whiteSpace;
			return (num * 397) ^ this.wordSpacing.GetHashCode();
		}

		public Color color;

		public Length fontSize;

		public Length letterSpacing;

		public TextShadow textShadow;

		public EditorTextRenderingMode unityEditorTextRenderingMode;

		public Font unityFont;

		public FontDefinition unityFontDefinition;

		public FontStyle unityFontStyleAndWeight;

		public MaterialDefinition unityMaterial;

		public Length unityParagraphSpacing;

		public TextAnchor unityTextAlign;

		public TextAutoSize unityTextAutoSize;

		public TextGeneratorType unityTextGenerator;

		public Color unityTextOutlineColor;

		public float unityTextOutlineWidth;

		public Visibility visibility;

		public WhiteSpace whiteSpace;

		public Length wordSpacing;
	}
}
