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
			return lhs.color == rhs.color && lhs.fontSize == rhs.fontSize && lhs.letterSpacing == rhs.letterSpacing && lhs.textShadow == rhs.textShadow && lhs.unityFont == rhs.unityFont && lhs.unityFontDefinition == rhs.unityFontDefinition && lhs.unityFontStyleAndWeight == rhs.unityFontStyleAndWeight && lhs.unityParagraphSpacing == rhs.unityParagraphSpacing && lhs.unityTextAlign == rhs.unityTextAlign && lhs.unityTextOutlineColor == rhs.unityTextOutlineColor && lhs.unityTextOutlineWidth == rhs.unityTextOutlineWidth && lhs.visibility == rhs.visibility && lhs.whiteSpace == rhs.whiteSpace && lhs.wordSpacing == rhs.wordSpacing;
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
			num = (num * 397) ^ ((this.unityFont == null) ? 0 : this.unityFont.GetHashCode());
			num = (num * 397) ^ this.unityFontDefinition.GetHashCode();
			num = (num * 397) ^ (int)this.unityFontStyleAndWeight;
			num = (num * 397) ^ this.unityParagraphSpacing.GetHashCode();
			num = (num * 397) ^ (int)this.unityTextAlign;
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

		public Font unityFont;

		public FontDefinition unityFontDefinition;

		public FontStyle unityFontStyleAndWeight;

		public Length unityParagraphSpacing;

		public TextAnchor unityTextAlign;

		public Color unityTextOutlineColor;

		public float unityTextOutlineWidth;

		public Visibility visibility;

		public WhiteSpace whiteSpace;

		public Length wordSpacing;
	}
}
