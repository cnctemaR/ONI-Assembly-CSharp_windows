using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct StyleRotate : IStyleValue<Rotate>, IEquatable<StyleRotate>
	{
		public Rotate value
		{
			get
			{
				StyleKeyword keyword = this.m_Keyword;
				if (!true)
				{
				}
				Rotate rotate;
				switch (keyword)
				{
				case StyleKeyword.Undefined:
					rotate = this.m_Value;
					goto IL_004F;
				case StyleKeyword.Null:
					rotate = Rotate.None();
					goto IL_004F;
				case StyleKeyword.None:
					rotate = Rotate.None();
					goto IL_004F;
				case StyleKeyword.Initial:
					rotate = Rotate.Initial();
					goto IL_004F;
				}
				throw new NotImplementedException();
				IL_004F:
				if (!true)
				{
				}
				return rotate;
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

		public StyleRotate(Rotate v)
		{
			this = new StyleRotate(v, StyleKeyword.Undefined);
		}

		public StyleRotate(StyleKeyword keyword)
		{
			this = new StyleRotate(default(Rotate), keyword);
		}

		public StyleRotate(Quaternion quaternion)
		{
			this = new StyleRotate(quaternion, StyleKeyword.Undefined);
		}

		internal StyleRotate(Rotate v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleRotate lhs, StyleRotate rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleRotate lhs, StyleRotate rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleRotate(StyleKeyword keyword)
		{
			return new StyleRotate(keyword);
		}

		public static implicit operator StyleRotate(Rotate v)
		{
			return new StyleRotate(v);
		}

		public static implicit operator StyleRotate(Quaternion v)
		{
			return new Rotate(v);
		}

		public bool Equals(StyleRotate other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleRotate)
			{
				StyleRotate styleRotate = (StyleRotate)obj;
				flag = this.Equals(styleRotate);
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
			return this.DebugString<Rotate>();
		}

		[SerializeField]
		private Rotate m_Value;

		[SerializeField]
		private StyleKeyword m_Keyword;
	}
}
