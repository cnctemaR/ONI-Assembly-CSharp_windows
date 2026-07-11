using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	public struct StyleCursor : IStyleValue<Cursor>, IEquatable<StyleCursor>
	{
		public Cursor value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(Cursor);
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

		int IStyleValue<Cursor>.specificity
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

		public StyleCursor(Cursor v)
		{
			this = new StyleCursor(v, StyleKeyword.Undefined);
		}

		public StyleCursor(StyleKeyword keyword)
		{
			this = new StyleCursor(default(Cursor), keyword);
		}

		internal StyleCursor(Cursor v, StyleKeyword keyword)
		{
			this.m_Specificity = 0;
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		internal bool Apply<U>(U other, StylePropertyApplyMode mode) where U : IStyleValue<Cursor>
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

		bool IStyleValue<Cursor>.Apply<U>(U other, StylePropertyApplyMode mode)
		{
			return this.Apply<U>(other, mode);
		}

		public static bool operator ==(StyleCursor lhs, StyleCursor rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleCursor lhs, StyleCursor rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleCursor(StyleKeyword keyword)
		{
			return new StyleCursor(keyword);
		}

		public static implicit operator StyleCursor(Cursor v)
		{
			return new StyleCursor(v);
		}

		public bool Equals(StyleCursor other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is StyleCursor);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				StyleCursor styleCursor = (StyleCursor)obj;
				flag2 = styleCursor == this;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = 917506989;
			num = num * -1521134295 + this.m_Keyword.GetHashCode();
			num = num * -1521134295 + this.m_Value.GetHashCode();
			return num * -1521134295 + this.m_Specificity.GetHashCode();
		}

		public override string ToString()
		{
			return this.DebugString<Cursor>();
		}

		private StyleKeyword m_Keyword;

		private Cursor m_Value;

		private int m_Specificity;
	}
}
