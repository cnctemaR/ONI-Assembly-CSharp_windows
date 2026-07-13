using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleBackgroundSize : IStyleValue<BackgroundSize>, IEquatable<StyleBackgroundSize>
	{
		public BackgroundSize value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(BackgroundSize);
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

		public StyleBackgroundSize(BackgroundSize v)
		{
			this = new StyleBackgroundSize(v, StyleKeyword.Undefined);
		}

		public StyleBackgroundSize(StyleKeyword keyword)
		{
			this = new StyleBackgroundSize(default(BackgroundSize), keyword);
		}

		internal StyleBackgroundSize(BackgroundSize v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleBackgroundSize lhs, StyleBackgroundSize rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleBackgroundSize lhs, StyleBackgroundSize rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleBackgroundSize(StyleKeyword keyword)
		{
			return new StyleBackgroundSize(keyword);
		}

		public static implicit operator StyleBackgroundSize(BackgroundSize v)
		{
			return new StyleBackgroundSize(v);
		}

		public bool Equals(StyleBackgroundSize other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleBackgroundSize)
			{
				StyleBackgroundSize styleBackgroundSize = (StyleBackgroundSize)obj;
				flag = this.Equals(styleBackgroundSize);
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
			return this.DebugString<BackgroundSize>();
		}

		[SerializeField]
		private BackgroundSize m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
