using System;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	public struct StyleFloat : IStyleValue<float>, IEquatable<StyleFloat>
	{
		public float value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : 0f;
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

		int IStyleValue<float>.specificity
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

		public StyleFloat(float v)
		{
			this = new StyleFloat(v, StyleKeyword.Undefined);
		}

		public StyleFloat(StyleKeyword keyword)
		{
			this = new StyleFloat(0f, keyword);
		}

		internal StyleFloat(float v, StyleKeyword keyword)
		{
			this.m_Specificity = 0;
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		internal bool Apply<U>(U other, StylePropertyApplyMode mode) where U : IStyleValue<float>
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

		bool IStyleValue<float>.Apply<U>(U other, StylePropertyApplyMode mode)
		{
			return this.Apply<U>(other, mode);
		}

		public static bool operator ==(StyleFloat lhs, StyleFloat rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleFloat lhs, StyleFloat rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleFloat(StyleKeyword keyword)
		{
			return new StyleFloat(keyword);
		}

		public static implicit operator StyleFloat(float v)
		{
			return new StyleFloat(v);
		}

		public bool Equals(StyleFloat other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is StyleFloat);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				StyleFloat styleFloat = (StyleFloat)obj;
				flag2 = styleFloat == this;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = 917506989;
			num = num * -1521134295 + this.m_Keyword.GetHashCode();
			num = num * -1521134295 + this.m_Value.GetHashCode();
			return num * -1521134295 + this.m_Specificity.GetHashCode();
		}

		public override string ToString()
		{
			return this.DebugString<float>();
		}

		private StyleKeyword m_Keyword;

		private float m_Value;

		private int m_Specificity;
	}
}
