using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	public struct StyleFont : IStyleValue<Font>, IEquatable<StyleFont>
	{
		public Font value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : null;
			}
			set
			{
				this.m_Value = value;
				this.m_Keyword = StyleKeyword.Undefined;
			}
		}

		internal int specificity
		{
			get
			{
				return this.m_Specificity;
			}
			set
			{
				this.m_Specificity = value;
			}
		}

		int IStyleValue<Font>.specificity
		{
			get
			{
				return this.specificity;
			}
			set
			{
				this.specificity = value;
			}
		}

		public StyleKeyword keyword
		{
			get
			{
				return this.m_Keyword;
			}
			set
			{
				this.m_Keyword = value;
			}
		}

		public StyleFont(Font v)
		{
			this = new StyleFont(v, StyleKeyword.Undefined);
		}

		public StyleFont(StyleKeyword keyword)
		{
			this = new StyleFont(null, keyword);
		}

		internal StyleFont(Font v, StyleKeyword keyword)
		{
			this.m_Specificity = 0;
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		internal bool Apply<U>(U other, StylePropertyApplyMode mode) where U : IStyleValue<Font>
		{
			bool flag = StyleValueExtensions.CanApply(this.specificity, other.specificity, mode);
			bool flag2;
			if (flag)
			{
				this.value = other.value;
				this.keyword = other.keyword;
				this.specificity = other.specificity;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		bool IStyleValue<Font>.Apply<U>(U other, StylePropertyApplyMode mode)
		{
			return this.Apply<U>(other, mode);
		}

		public static bool operator ==(StyleFont lhs, StyleFont rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleFont lhs, StyleFont rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleFont(StyleKeyword keyword)
		{
			return new StyleFont(keyword);
		}

		public static implicit operator StyleFont(Font v)
		{
			return new StyleFont(v);
		}

		public bool Equals(StyleFont other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is StyleFont);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				StyleFont styleFont = (StyleFont)obj;
				flag2 = styleFont == this;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = 917506989;
			num = num * -1521134295 + this.m_Keyword.GetHashCode();
			num = num * -1521134295 + EqualityComparer<Font>.Default.GetHashCode(this.m_Value);
			return num * -1521134295 + this.m_Specificity.GetHashCode();
		}

		public override string ToString()
		{
			return this.DebugString<Font>();
		}

		private StyleKeyword m_Keyword;

		private Font m_Value;

		private int m_Specificity;
	}
}
