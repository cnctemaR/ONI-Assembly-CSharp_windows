using System;

namespace TMPro
{
	public struct TMP_Offset
	{
		public float left
		{
			get
			{
				return this.m_Left;
			}
			set
			{
				this.m_Left = value;
			}
		}

		public float right
		{
			get
			{
				return this.m_Right;
			}
			set
			{
				this.m_Right = value;
			}
		}

		public float top
		{
			get
			{
				return this.m_Top;
			}
			set
			{
				this.m_Top = value;
			}
		}

		public float bottom
		{
			get
			{
				return this.m_Bottom;
			}
			set
			{
				this.m_Bottom = value;
			}
		}

		public float horizontal
		{
			get
			{
				return this.m_Left;
			}
			set
			{
				this.m_Left = value;
				this.m_Right = value;
			}
		}

		public float vertical
		{
			get
			{
				return this.m_Top;
			}
			set
			{
				this.m_Top = value;
				this.m_Bottom = value;
			}
		}

		public static TMP_Offset zero
		{
			get
			{
				return TMP_Offset.k_ZeroOffset;
			}
		}

		public TMP_Offset(float left, float right, float top, float bottom)
		{
			this.m_Left = left;
			this.m_Right = right;
			this.m_Top = top;
			this.m_Bottom = bottom;
		}

		public TMP_Offset(float horizontal, float vertical)
		{
			this.m_Left = horizontal;
			this.m_Right = horizontal;
			this.m_Top = vertical;
			this.m_Bottom = vertical;
		}

		public static bool operator ==(TMP_Offset lhs, TMP_Offset rhs)
		{
			return lhs.m_Left == rhs.m_Left && lhs.m_Right == rhs.m_Right && lhs.m_Top == rhs.m_Top && lhs.m_Bottom == rhs.m_Bottom;
		}

		public static bool operator !=(TMP_Offset lhs, TMP_Offset rhs)
		{
			return !(lhs == rhs);
		}

		public static TMP_Offset operator *(TMP_Offset a, float b)
		{
			return new TMP_Offset(a.m_Left * b, a.m_Right * b, a.m_Top * b, a.m_Bottom * b);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(TMP_Offset other)
		{
			return base.Equals(other);
		}

		private float m_Left;

		private float m_Right;

		private float m_Top;

		private float m_Bottom;

		private static readonly TMP_Offset k_ZeroOffset = new TMP_Offset(0f, 0f, 0f, 0f);
	}
}
