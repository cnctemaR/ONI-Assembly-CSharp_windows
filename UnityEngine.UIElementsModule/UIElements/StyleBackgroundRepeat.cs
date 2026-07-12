using System;

namespace UnityEngine.UIElements
{
	public struct StyleBackgroundRepeat : IStyleValue<BackgroundRepeat>, IEquatable<StyleBackgroundRepeat>
	{
		public BackgroundRepeat value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(BackgroundRepeat);
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

		public StyleBackgroundRepeat(BackgroundRepeat v)
		{
			this = new StyleBackgroundRepeat(v, StyleKeyword.Undefined);
		}

		public StyleBackgroundRepeat(StyleKeyword keyword)
		{
			this = new StyleBackgroundRepeat(default(BackgroundRepeat), keyword);
		}

		internal StyleBackgroundRepeat(BackgroundRepeat v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleBackgroundRepeat lhs, StyleBackgroundRepeat rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleBackgroundRepeat lhs, StyleBackgroundRepeat rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleBackgroundRepeat(StyleKeyword keyword)
		{
			return new StyleBackgroundRepeat(keyword);
		}

		public static implicit operator StyleBackgroundRepeat(BackgroundRepeat v)
		{
			return new StyleBackgroundRepeat(v);
		}

		public bool Equals(StyleBackgroundRepeat other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleBackgroundRepeat)
			{
				StyleBackgroundRepeat styleBackgroundRepeat = (StyleBackgroundRepeat)obj;
				flag = this.Equals(styleBackgroundRepeat);
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
			return this.DebugString<BackgroundRepeat>();
		}

		private BackgroundRepeat m_Value;

		private StyleKeyword m_Keyword;
	}
}
