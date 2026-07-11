using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	public struct StyleLength : IStyleValue<Length>, IEquatable<StyleLength>
	{
		public Length value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(Length);
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

		int IStyleValue<Length>.specificity
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

		public StyleLength(float v)
		{
			this = new StyleLength(new Length(v, LengthUnit.Pixel), StyleKeyword.Undefined);
		}

		public StyleLength(Length v)
		{
			this = new StyleLength(v, StyleKeyword.Undefined);
		}

		public StyleLength(StyleKeyword keyword)
		{
			this = new StyleLength(default(Length), keyword);
		}

		internal StyleLength(Length v, StyleKeyword keyword)
		{
			this.m_Specificity = 0;
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		internal bool Apply<U>(U other, StylePropertyApplyMode mode) where U : IStyleValue<Length>
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

		bool IStyleValue<Length>.Apply<U>(U other, StylePropertyApplyMode mode)
		{
			return this.Apply<U>(other, mode);
		}

		public static bool operator ==(StyleLength lhs, StyleLength rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleLength lhs, StyleLength rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleLength(StyleKeyword keyword)
		{
			return new StyleLength(keyword);
		}

		public static implicit operator StyleLength(float v)
		{
			return new StyleLength(v);
		}

		public static implicit operator StyleLength(Length v)
		{
			return new StyleLength(v);
		}

		public bool Equals(StyleLength other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is StyleLength);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				StyleLength styleLength = (StyleLength)obj;
				flag2 = styleLength == this;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = -1977396678;
			num = num * -1521134295 + this.m_Keyword.GetHashCode();
			num = num * -1521134295 + this.m_Value.GetHashCode();
			return num * -1521134295 + this.m_Specificity.GetHashCode();
		}

		public override string ToString()
		{
			return this.DebugString<Length>();
		}

		private StyleKeyword m_Keyword;

		private Length m_Value;

		private int m_Specificity;
	}
}
