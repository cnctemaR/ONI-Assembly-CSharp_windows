using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleColor : IStyleValue<Color>, IEquatable<StyleColor>
	{
		public Color value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : Color.clear;
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

		public StyleColor(Color v)
		{
			this = new StyleColor(v, StyleKeyword.Undefined);
		}

		public StyleColor(StyleKeyword keyword)
		{
			this = new StyleColor(Color.clear, keyword);
		}

		internal StyleColor(Color v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleColor lhs, StyleColor rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleColor lhs, StyleColor rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(StyleColor lhs, Color rhs)
		{
			StyleColor styleColor = new StyleColor(rhs);
			return lhs == styleColor;
		}

		public static bool operator !=(StyleColor lhs, Color rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleColor(StyleKeyword keyword)
		{
			return new StyleColor(keyword);
		}

		public static implicit operator StyleColor(Color v)
		{
			return new StyleColor(v);
		}

		public bool Equals(StyleColor other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleColor)
			{
				StyleColor styleColor = (StyleColor)obj;
				flag = this.Equals(styleColor);
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
			return this.DebugString<Color>();
		}

		[SerializeField]
		private Color m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
