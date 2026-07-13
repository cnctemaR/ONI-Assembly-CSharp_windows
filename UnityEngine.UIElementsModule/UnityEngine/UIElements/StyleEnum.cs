using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleEnum<T> : IStyleValue<T>, IEquatable<StyleEnum<T>> where T : struct, IConvertible
	{
		public T value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : default(T);
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

		public StyleEnum(T v)
		{
			this = new StyleEnum<T>(v, StyleKeyword.Undefined);
		}

		public StyleEnum(StyleKeyword keyword)
		{
			this = new StyleEnum<T>(default(T), keyword);
		}

		internal StyleEnum(T v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleEnum<T> lhs, StyleEnum<T> rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && UnsafeUtility.EnumEquals<T>(lhs.m_Value, rhs.m_Value);
		}

		public static bool operator !=(StyleEnum<T> lhs, StyleEnum<T> rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleEnum<T>(StyleKeyword keyword)
		{
			return new StyleEnum<T>(keyword);
		}

		public static implicit operator StyleEnum<T>(T v)
		{
			return new StyleEnum<T>(v);
		}

		public bool Equals(StyleEnum<T> other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleEnum<T>)
			{
				StyleEnum<T> styleEnum = (StyleEnum<T>)obj;
				flag = this.Equals(styleEnum);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (UnsafeUtility.EnumToInt<T>(this.m_Value) * 397) ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<T>();
		}

		internal static bool TryParseString(string str, out StyleEnum<T> styleEnum)
		{
			T t;
			bool flag = Enum.TryParse<T>(str, true, out t);
			bool flag2;
			if (flag)
			{
				styleEnum = new StyleEnum<T>(t);
				flag2 = true;
			}
			else
			{
				styleEnum = default(StyleEnum<T>);
				flag2 = false;
			}
			return flag2;
		}

		[SerializeField]
		private T m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
