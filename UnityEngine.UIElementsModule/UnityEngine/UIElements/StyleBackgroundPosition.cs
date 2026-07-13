using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleBackgroundPosition : IStyleValue<BackgroundPosition>, IEquatable<StyleBackgroundPosition>
	{
		public BackgroundPosition value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(BackgroundPosition);
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

		public StyleBackgroundPosition(BackgroundPosition v)
		{
			this = new StyleBackgroundPosition(v, StyleKeyword.Undefined);
		}

		public StyleBackgroundPosition(StyleKeyword keyword)
		{
			this = new StyleBackgroundPosition(default(BackgroundPosition), keyword);
		}

		internal StyleBackgroundPosition(BackgroundPosition v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleBackgroundPosition lhs, StyleBackgroundPosition rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleBackgroundPosition lhs, StyleBackgroundPosition rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleBackgroundPosition(StyleKeyword keyword)
		{
			return new StyleBackgroundPosition(keyword);
		}

		public static implicit operator StyleBackgroundPosition(BackgroundPosition v)
		{
			return new StyleBackgroundPosition(v);
		}

		public bool Equals(StyleBackgroundPosition other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleBackgroundPosition)
			{
				StyleBackgroundPosition styleBackgroundPosition = (StyleBackgroundPosition)obj;
				flag = this.Equals(styleBackgroundPosition);
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
			return this.DebugString<BackgroundPosition>();
		}

		[SerializeField]
		private BackgroundPosition m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
