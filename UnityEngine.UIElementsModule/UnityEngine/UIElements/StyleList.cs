using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleList<T> : IStyleValue<List<T>>, IEquatable<StyleList<T>>
	{
		public List<T> value
		{
			get
			{
				return (this.m_Keyword == StyleKeyword.Undefined) ? this.m_Value : null;
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

		public StyleList(List<T> v)
		{
			this = new StyleList<T>(v, StyleKeyword.Undefined);
		}

		public StyleList(StyleKeyword keyword)
		{
			this = new StyleList<T>(null, keyword);
		}

		internal StyleList(List<T> v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleList<T> lhs, StyleList<T> rhs)
		{
			bool flag = lhs.m_Keyword != rhs.m_Keyword;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				List<T> value = lhs.m_Value;
				List<T> value2 = rhs.m_Value;
				bool flag3 = value == value2;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = value == null || value2 == null;
					flag2 = !flag4 && value.Count == value2.Count && value.SequenceEqual<T>(value2);
				}
			}
			return flag2;
		}

		public static bool operator !=(StyleList<T> lhs, StyleList<T> rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleList<T>(StyleKeyword keyword)
		{
			return new StyleList<T>(keyword);
		}

		public static implicit operator StyleList<T>(List<T> v)
		{
			return new StyleList<T>(v);
		}

		public bool Equals(StyleList<T> other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleList<T>)
			{
				StyleList<T> styleList = (StyleList<T>)obj;
				flag = this.Equals(styleList);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			int num = 0;
			bool flag = this.m_Value != null && this.m_Value.Count > 0;
			if (flag)
			{
				num = EqualityComparer<T>.Default.GetHashCode(this.m_Value[0]);
				for (int i = 1; i < this.m_Value.Count; i++)
				{
					num = (num * 397) ^ EqualityComparer<T>.Default.GetHashCode(this.m_Value[i]);
				}
			}
			return (num * 397) ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<List<T>>();
		}

		[SerializeField]
		private StyleKeyword m_Keyword;

		[SerializeField]
		private List<T> m_Value;
	}
}
