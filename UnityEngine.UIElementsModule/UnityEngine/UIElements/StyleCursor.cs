using System;

namespace UnityEngine.UIElements
{
	[Serializable]
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
			this.m_Keyword = keyword;
			this.m_Value = v;
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
			bool flag;
			if (obj is StyleCursor)
			{
				StyleCursor styleCursor = (StyleCursor)obj;
				flag = this.Equals(styleCursor);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.m_Value.GetHashCode() * 397) ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<Cursor>();
		}

		[SerializeField]
		private Cursor m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
