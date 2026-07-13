using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public struct StyleRatio : IStyleValue<Ratio>, IEquatable<StyleRatio>
	{
		public Ratio value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : float.NaN;
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
				this.m_Value = float.NaN;
			}
		}

		public StyleRatio(Ratio value)
		{
			this = new StyleRatio(value, StyleKeyword.Undefined);
		}

		public StyleRatio(StyleKeyword keyword)
		{
			this = new StyleRatio(float.NaN, keyword);
		}

		internal StyleRatio(Ratio value, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = value;
		}

		public static StyleRatio Auto()
		{
			return new StyleRatio(float.NaN, StyleKeyword.Auto);
		}

		internal bool IsAuto()
		{
			return this.m_Keyword == StyleKeyword.Auto;
		}

		public static implicit operator StyleRatio(float value)
		{
			return new StyleRatio(value);
		}

		public static implicit operator float(StyleRatio value)
		{
			return value.value;
		}

		public static implicit operator StyleRatio(Ratio value)
		{
			return new StyleRatio(value);
		}

		public static implicit operator Ratio(StyleRatio value)
		{
			return value.value;
		}

		public static implicit operator StyleKeyword(StyleRatio value)
		{
			return value.keyword;
		}

		public static implicit operator StyleRatio(StyleKeyword value)
		{
			return new StyleRatio(value);
		}

		public static bool operator ==(StyleRatio lhs, StyleRatio rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleRatio lhs, StyleRatio rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(StyleRatio other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleRatio)
			{
				StyleRatio styleRatio = (StyleRatio)obj;
				flag = this.Equals(styleRatio);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.m_Value.GetHashCode() * 793;
		}

		public override string ToString()
		{
			return this.IsAuto() ? StyleValueKeyword.Auto.ToUssString() : this.m_Value.value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		private Ratio m_Value;

		private StyleKeyword m_Keyword;
	}
}
