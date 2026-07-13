using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleTranslate : IStyleValue<Translate>, IEquatable<StyleTranslate>
	{
		public Translate value
		{
			get
			{
				StyleKeyword keyword = this.m_Keyword;
				if (!true)
				{
				}
				Translate translate;
				switch (keyword)
				{
				case StyleKeyword.Undefined:
					translate = this.m_Value;
					goto IL_004F;
				case StyleKeyword.Null:
					translate = Translate.None();
					goto IL_004F;
				case StyleKeyword.None:
					translate = Translate.None();
					goto IL_004F;
				case StyleKeyword.Initial:
					translate = Translate.None();
					goto IL_004F;
				}
				throw new NotImplementedException();
				IL_004F:
				if (!true)
				{
				}
				return translate;
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

		public StyleTranslate(Translate v)
		{
			this = new StyleTranslate(v, StyleKeyword.Undefined);
		}

		public StyleTranslate(StyleKeyword keyword)
		{
			this = new StyleTranslate(default(Translate), keyword);
		}

		internal StyleTranslate(Translate v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleTranslate lhs, StyleTranslate rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleTranslate lhs, StyleTranslate rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleTranslate(StyleKeyword keyword)
		{
			return new StyleTranslate(keyword);
		}

		public static implicit operator StyleTranslate(Translate v)
		{
			return new StyleTranslate(v);
		}

		public static implicit operator StyleTranslate(Vector3 v)
		{
			return new StyleTranslate(v);
		}

		public static implicit operator StyleTranslate(Vector2 v)
		{
			return new StyleTranslate(v);
		}

		public bool Equals(StyleTranslate other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleTranslate)
			{
				StyleTranslate styleTranslate = (StyleTranslate)obj;
				flag = this.Equals(styleTranslate);
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
			return this.DebugString<Translate>();
		}

		[SerializeField]
		private Translate m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
