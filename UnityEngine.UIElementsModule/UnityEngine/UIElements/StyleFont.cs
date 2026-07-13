using System;

namespace UnityEngine.UIElements
{
	[Serializable]
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
			this.m_Keyword = keyword;
			this.m_Value = v;
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
			bool flag;
			if (obj is StyleFont)
			{
				StyleFont styleFont = (StyleFont)obj;
				flag = this.Equals(styleFont);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (((this.m_Value != null) ? this.m_Value.GetHashCode() : 0) * 397) ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<Font>();
		}

		[SerializeField]
		private Font m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
