using System;

namespace LibTessDotNet
{
	internal struct Vec3
	{
		public float this[int index]
		{
			get
			{
				bool flag = index == 0;
				float num;
				if (flag)
				{
					num = this.X;
				}
				else
				{
					bool flag2 = index == 1;
					if (flag2)
					{
						num = this.Y;
					}
					else
					{
						bool flag3 = index == 2;
						if (!flag3)
						{
							throw new IndexOutOfRangeException();
						}
						num = this.Z;
					}
				}
				return num;
			}
			set
			{
				bool flag = index == 0;
				if (flag)
				{
					this.X = value;
				}
				else
				{
					bool flag2 = index == 1;
					if (flag2)
					{
						this.Y = value;
					}
					else
					{
						bool flag3 = index == 2;
						if (!flag3)
						{
							throw new IndexOutOfRangeException();
						}
						this.Z = value;
					}
				}
			}
		}

		public static void Sub(ref Vec3 lhs, ref Vec3 rhs, out Vec3 result)
		{
			result.X = lhs.X - rhs.X;
			result.Y = lhs.Y - rhs.Y;
			result.Z = lhs.Z - rhs.Z;
		}

		public static void Neg(ref Vec3 v)
		{
			v.X = -v.X;
			v.Y = -v.Y;
			v.Z = -v.Z;
		}

		public static void Dot(ref Vec3 u, ref Vec3 v, out float dot)
		{
			dot = u.X * v.X + u.Y * v.Y + u.Z * v.Z;
		}

		public static void Normalize(ref Vec3 v)
		{
			float num = v.X * v.X + v.Y * v.Y + v.Z * v.Z;
			num = 1f / (float)Math.Sqrt((double)num);
			v.X *= num;
			v.Y *= num;
			v.Z *= num;
		}

		public static int LongAxis(ref Vec3 v)
		{
			int num = 0;
			bool flag = Math.Abs(v.Y) > Math.Abs(v.X);
			if (flag)
			{
				num = 1;
			}
			bool flag2 = Math.Abs(v.Z) > Math.Abs((num == 0) ? v.X : v.Y);
			if (flag2)
			{
				num = 2;
			}
			return num;
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", this.X, this.Y, this.Z);
		}

		public static readonly Vec3 Zero = default(Vec3);

		public float X;

		public float Y;

		public float Z;
	}
}
