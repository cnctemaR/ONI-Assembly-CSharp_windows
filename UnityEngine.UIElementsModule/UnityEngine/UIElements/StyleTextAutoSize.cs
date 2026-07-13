using System;

namespace UnityEngine.UIElements
{
	public struct StyleTextAutoSize : IStyleValue<TextAutoSize>, IEquatable<StyleTextAutoSize>
	{
		public TextAutoSize value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(TextAutoSize);
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

		public StyleTextAutoSize(TextAutoSize v)
		{
			this = new StyleTextAutoSize(v, StyleKeyword.Undefined);
		}

		public StyleTextAutoSize(StyleKeyword keyword)
		{
			this = new StyleTextAutoSize(default(TextAutoSize), keyword);
		}

		internal StyleTextAutoSize(TextAutoSize v, StyleKeyword keyword)
		{
			this.m_Value = v;
			this.m_Keyword = keyword;
		}

		public static bool operator ==(StyleTextAutoSize lhs, StyleTextAutoSize rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value.Equals(rhs.m_Value);
		}

		public static bool operator !=(StyleTextAutoSize lhs, StyleTextAutoSize rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleTextAutoSize(StyleKeyword keyword)
		{
			return new StyleTextAutoSize(keyword);
		}

		public static implicit operator StyleTextAutoSize(TextAutoSize v)
		{
			return new StyleTextAutoSize(v);
		}

		public bool Equals(StyleTextAutoSize other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleTextAutoSize)
			{
				StyleTextAutoSize styleTextAutoSize = (StyleTextAutoSize)obj;
				flag = this.Equals(styleTextAutoSize);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			int num = 917506989;
			num = num * -1521134295 + this.m_Keyword.GetHashCode();
			return num * -1521134295 + this.m_Value.GetHashCode();
		}

		public override string ToString()
		{
			return this.DebugString<TextAutoSize>();
		}

		private StyleKeyword m_Keyword;

		private TextAutoSize m_Value;
	}
}
