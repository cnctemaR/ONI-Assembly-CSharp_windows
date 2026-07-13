using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleInt : IStyleValue<int>, IEquatable<StyleInt>
	{
		public int value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : 0;
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

		public StyleInt(int v)
		{
			this = new StyleInt(v, StyleKeyword.Undefined);
		}

		public StyleInt(StyleKeyword keyword)
		{
			this = new StyleInt(0, keyword);
		}

		internal StyleInt(int v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleInt lhs, StyleInt rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleInt lhs, StyleInt rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleInt(StyleKeyword keyword)
		{
			return new StyleInt(keyword);
		}

		public static implicit operator StyleInt(int v)
		{
			return new StyleInt(v);
		}

		public bool Equals(StyleInt other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleInt)
			{
				StyleInt styleInt = (StyleInt)obj;
				flag = this.Equals(styleInt);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.m_Value * 397) ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<int>();
		}

		[SerializeField]
		private int m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
