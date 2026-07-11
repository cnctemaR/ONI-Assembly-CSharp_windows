using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal class InheritedStylesData : IEquatable<InheritedStylesData>
	{
		public InheritedStylesData()
		{
			this.color = StyleSheetCache.GetInitialValue(StylePropertyID.Color).color;
		}

		public InheritedStylesData(InheritedStylesData other)
		{
			this.CopyFrom(other);
		}

		public void CopyFrom(InheritedStylesData other)
		{
			bool flag = other != null;
			if (flag)
			{
				this.color = other.color;
				this.font = other.font;
				this.fontSize = other.fontSize;
				this.visibility = other.visibility;
				this.whiteSpace = other.whiteSpace;
				this.unityFontStyle = other.unityFontStyle;
				this.unityTextAlign = other.unityTextAlign;
			}
		}

		public bool Equals(InheritedStylesData other)
		{
			bool flag = other == null;
			return !flag && (this.color == other.color && this.font == other.font && this.fontSize == other.fontSize && this.unityFontStyle == other.unityFontStyle && this.unityTextAlign == other.unityTextAlign && this.visibility == other.visibility) && this.whiteSpace == other.whiteSpace;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is InheritedStylesData);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				InheritedStylesData inheritedStylesData = (InheritedStylesData)obj;
				flag2 = this.Equals(inheritedStylesData);
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = -2037960190;
			num = num * -1521134295 + this.color.GetHashCode();
			num = num * -1521134295 + this.font.GetHashCode();
			num = num * -1521134295 + this.fontSize.GetHashCode();
			num = num * -1521134295 + this.unityFontStyle.GetHashCode();
			num = num * -1521134295 + this.unityTextAlign.GetHashCode();
			num = num * -1521134295 + this.visibility.GetHashCode();
			return num * -1521134295 + this.whiteSpace.GetHashCode();
		}

		public static readonly InheritedStylesData none = new InheritedStylesData();

		public StyleColor color;

		public StyleFont font;

		public StyleLength fontSize;

		public StyleInt unityFontStyle;

		public StyleInt unityTextAlign;

		public StyleInt visibility;

		public StyleInt whiteSpace;
	}
}
