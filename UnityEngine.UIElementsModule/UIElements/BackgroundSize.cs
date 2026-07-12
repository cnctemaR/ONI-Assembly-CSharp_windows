using System;

namespace UnityEngine.UIElements
{
	public struct BackgroundSize : IEquatable<BackgroundSize>
	{
		public BackgroundSizeType sizeType
		{
			get
			{
				return this.m_SizeType;
			}
			set
			{
				this.m_SizeType = value;
				this.m_X = new Length(0f);
				this.m_Y = new Length(0f);
			}
		}

		public Length x
		{
			get
			{
				return this.m_X;
			}
			set
			{
				this.m_X = value;
				this.m_SizeType = BackgroundSizeType.Length;
			}
		}

		public Length y
		{
			get
			{
				return this.m_Y;
			}
			set
			{
				this.m_Y = value;
				this.m_SizeType = BackgroundSizeType.Length;
			}
		}

		public BackgroundSize(Length sizeX, Length sizeY)
		{
			this.m_SizeType = BackgroundSizeType.Length;
			this.m_X = sizeX;
			this.m_Y = sizeY;
		}

		public BackgroundSize(BackgroundSizeType sizeType)
		{
			this.m_SizeType = sizeType;
			this.m_X = new Length(0f);
			this.m_Y = new Length(0f);
		}

		internal static BackgroundSize Initial()
		{
			return BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.StretchToFill);
		}

		public override bool Equals(object obj)
		{
			return obj is BackgroundSize && this.Equals((BackgroundSize)obj);
		}

		public bool Equals(BackgroundSize other)
		{
			return other.x == this.x && other.y == this.y && other.sizeType == this.sizeType;
		}

		public override int GetHashCode()
		{
			int num = 1500536833;
			num = num * -1521134295 + this.m_SizeType.GetHashCode();
			num = num * -1521134295 + this.m_X.GetHashCode();
			return num * -1521134295 + this.m_Y.GetHashCode();
		}

		public static bool operator ==(BackgroundSize style1, BackgroundSize style2)
		{
			return style1.Equals(style2);
		}

		public static bool operator !=(BackgroundSize style1, BackgroundSize style2)
		{
			return !(style1 == style2);
		}

		public override string ToString()
		{
			return string.Format("(sizeType:{0} x:{1}, y:{2})", this.sizeType, this.x, this.y);
		}

		private BackgroundSizeType m_SizeType;

		private Length m_X;

		private Length m_Y;
	}
}
