using System;

namespace UnityEngine.TextCore.Text
{
	internal struct Offset
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

		public static Offset zero
		{
			get
			{
				return Offset.k_ZeroOffset;
			}
		}

		public Offset(float left, float right, float top, float bottom)
		{
			this.m_Left = left;
			this.m_Right = right;
			this.m_Top = top;
			this.m_Bottom = bottom;
		}

		public static bool operator ==(Offset lhs, Offset rhs)
		{
			return lhs.m_Left == rhs.m_Left && lhs.m_Right == rhs.m_Right && lhs.m_Top == rhs.m_Top && lhs.m_Bottom == rhs.m_Bottom;
		}

		public static bool operator !=(Offset lhs, Offset rhs)
		{
			return !(lhs == rhs);
		}

		public static Offset operator *(Offset a, float b)
		{
			return new Offset(a.m_Left * b, a.m_Right * b, a.m_Top * b, a.m_Bottom * b);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public bool Equals(Offset other)
		{
			return base.Equals(other);
		}

		private float m_Left;

		private float m_Right;

		private float m_Top;

		private float m_Bottom;

		private static readonly Offset k_ZeroOffset = new Offset(0f, 0f, 0f, 0f);
	}
}
