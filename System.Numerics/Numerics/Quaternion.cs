using System;
using System.Globalization;

namespace System.Numerics
{
	public struct Quaternion : IEquatable<Quaternion>
	{
		public static Quaternion Identity
		{
			get
			{
				return new Quaternion(0f, 0f, 0f, 1f);
			}
		}

		public bool IsIdentity
		{
			get
			{
				return this.X == 0f && this.Y == 0f && this.Z == 0f && this.W == 1f;
			}
		}

		public Quaternion(float x, float y, float z, float w)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		public Quaternion(Vector3 vectorPart, float scalarPart)
		{
			this.X = vectorPart.X;
			this.Y = vectorPart.Y;
			this.Z = vectorPart.Z;
			this.W = scalarPart;
		}

		public float Length()
		{
			return MathF.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
		}

		public float LengthSquared()
		{
			return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
		}

		public static Quaternion Normalize(Quaternion value)
		{
			float num = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
			float num2 = 1f / MathF.Sqrt(num);
			Quaternion quaternion;
			quaternion.X = value.X * num2;
			quaternion.Y = value.Y * num2;
			quaternion.Z = value.Z * num2;
			quaternion.W = value.W * num2;
			return quaternion;
		}

		public static Quaternion Conjugate(Quaternion value)
		{
			Quaternion quaternion;
			quaternion.X = -value.X;
			quaternion.Y = -value.Y;
			quaternion.Z = -value.Z;
			quaternion.W = value.W;
			return quaternion;
		}

		public static Quaternion Inverse(Quaternion value)
		{
			float num = value.X * value.X + value.Y * value.Y + value.Z * value.Z + value.W * value.W;
			float num2 = 1f / num;
			Quaternion quaternion;
			quaternion.X = -value.X * num2;
			quaternion.Y = -value.Y * num2;
			quaternion.Z = -value.Z * num2;
			quaternion.W = value.W * num2;
			return quaternion;
		}

		public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
		{
			float num = angle * 0.5f;
			float num2 = MathF.Sin(num);
			float num3 = MathF.Cos(num);
			Quaternion quaternion;
			quaternion.X = axis.X * num2;
			quaternion.Y = axis.Y * num2;
			quaternion.Z = axis.Z * num2;
			quaternion.W = num3;
			return quaternion;
		}

		public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			float num = roll * 0.5f;
			float num2 = MathF.Sin(num);
			float num3 = MathF.Cos(num);
			float num4 = pitch * 0.5f;
			float num5 = MathF.Sin(num4);
			float num6 = MathF.Cos(num4);
			float num7 = yaw * 0.5f;
			float num8 = MathF.Sin(num7);
			float num9 = MathF.Cos(num7);
			Quaternion quaternion;
			quaternion.X = num9 * num5 * num3 + num8 * num6 * num2;
			quaternion.Y = num8 * num6 * num3 - num9 * num5 * num2;
			quaternion.Z = num9 * num6 * num2 - num8 * num5 * num3;
			quaternion.W = num9 * num6 * num3 + num8 * num5 * num2;
			return quaternion;
		}

		public static Quaternion CreateFromRotationMatrix(Matrix4x4 matrix)
		{
			float num = matrix.M11 + matrix.M22 + matrix.M33;
			Quaternion quaternion = default(Quaternion);
			if (num > 0f)
			{
				float num2 = MathF.Sqrt(num + 1f);
				quaternion.W = num2 * 0.5f;
				num2 = 0.5f / num2;
				quaternion.X = (matrix.M23 - matrix.M32) * num2;
				quaternion.Y = (matrix.M31 - matrix.M13) * num2;
				quaternion.Z = (matrix.M12 - matrix.M21) * num2;
			}
			else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
			{
				float num3 = MathF.Sqrt(1f + matrix.M11 - matrix.M22 - matrix.M33);
				float num4 = 0.5f / num3;
				quaternion.X = 0.5f * num3;
				quaternion.Y = (matrix.M12 + matrix.M21) * num4;
				quaternion.Z = (matrix.M13 + matrix.M31) * num4;
				quaternion.W = (matrix.M23 - matrix.M32) * num4;
			}
			else if (matrix.M22 > matrix.M33)
			{
				float num5 = MathF.Sqrt(1f + matrix.M22 - matrix.M11 - matrix.M33);
				float num6 = 0.5f / num5;
				quaternion.X = (matrix.M21 + matrix.M12) * num6;
				quaternion.Y = 0.5f * num5;
				quaternion.Z = (matrix.M32 + matrix.M23) * num6;
				quaternion.W = (matrix.M31 - matrix.M13) * num6;
			}
			else
			{
				float num7 = MathF.Sqrt(1f + matrix.M33 - matrix.M11 - matrix.M22);
				float num8 = 0.5f / num7;
				quaternion.X = (matrix.M31 + matrix.M13) * num8;
				quaternion.Y = (matrix.M32 + matrix.M23) * num8;
				quaternion.Z = 0.5f * num7;
				quaternion.W = (matrix.M12 - matrix.M21) * num8;
			}
			return quaternion;
		}

		public static float Dot(Quaternion quaternion1, Quaternion quaternion2)
		{
			return quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
		}

		public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
		{
			float num = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
			bool flag = false;
			if (num < 0f)
			{
				flag = true;
				num = -num;
			}
			float num2;
			float num3;
			if (num > 0.999999f)
			{
				num2 = 1f - amount;
				num3 = (flag ? (-amount) : amount);
			}
			else
			{
				float num4 = MathF.Acos(num);
				float num5 = 1f / MathF.Sin(num4);
				num2 = MathF.Sin((1f - amount) * num4) * num5;
				num3 = (flag ? (-MathF.Sin(amount * num4) * num5) : (MathF.Sin(amount * num4) * num5));
			}
			Quaternion quaternion3;
			quaternion3.X = num2 * quaternion1.X + num3 * quaternion2.X;
			quaternion3.Y = num2 * quaternion1.Y + num3 * quaternion2.Y;
			quaternion3.Z = num2 * quaternion1.Z + num3 * quaternion2.Z;
			quaternion3.W = num2 * quaternion1.W + num3 * quaternion2.W;
			return quaternion3;
		}

		public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
		{
			float num = 1f - amount;
			Quaternion quaternion3 = default(Quaternion);
			if (quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W >= 0f)
			{
				quaternion3.X = num * quaternion1.X + amount * quaternion2.X;
				quaternion3.Y = num * quaternion1.Y + amount * quaternion2.Y;
				quaternion3.Z = num * quaternion1.Z + amount * quaternion2.Z;
				quaternion3.W = num * quaternion1.W + amount * quaternion2.W;
			}
			else
			{
				quaternion3.X = num * quaternion1.X - amount * quaternion2.X;
				quaternion3.Y = num * quaternion1.Y - amount * quaternion2.Y;
				quaternion3.Z = num * quaternion1.Z - amount * quaternion2.Z;
				quaternion3.W = num * quaternion1.W - amount * quaternion2.W;
			}
			float num2 = quaternion3.X * quaternion3.X + quaternion3.Y * quaternion3.Y + quaternion3.Z * quaternion3.Z + quaternion3.W * quaternion3.W;
			float num3 = 1f / MathF.Sqrt(num2);
			quaternion3.X *= num3;
			quaternion3.Y *= num3;
			quaternion3.Z *= num3;
			quaternion3.W *= num3;
			return quaternion3;
		}

		public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
		{
			float x = value2.X;
			float y = value2.Y;
			float z = value2.Z;
			float w = value2.W;
			float x2 = value1.X;
			float y2 = value1.Y;
			float z2 = value1.Z;
			float w2 = value1.W;
			float num = y * z2 - z * y2;
			float num2 = z * x2 - x * z2;
			float num3 = x * y2 - y * x2;
			float num4 = x * x2 + y * y2 + z * z2;
			Quaternion quaternion;
			quaternion.X = x * w2 + x2 * w + num;
			quaternion.Y = y * w2 + y2 * w + num2;
			quaternion.Z = z * w2 + z2 * w + num3;
			quaternion.W = w * w2 - num4;
			return quaternion;
		}

		public static Quaternion Negate(Quaternion value)
		{
			Quaternion quaternion;
			quaternion.X = -value.X;
			quaternion.Y = -value.Y;
			quaternion.Z = -value.Z;
			quaternion.W = -value.W;
			return quaternion;
		}

		public static Quaternion Add(Quaternion value1, Quaternion value2)
		{
			Quaternion quaternion;
			quaternion.X = value1.X + value2.X;
			quaternion.Y = value1.Y + value2.Y;
			quaternion.Z = value1.Z + value2.Z;
			quaternion.W = value1.W + value2.W;
			return quaternion;
		}

		public static Quaternion Subtract(Quaternion value1, Quaternion value2)
		{
			Quaternion quaternion;
			quaternion.X = value1.X - value2.X;
			quaternion.Y = value1.Y - value2.Y;
			quaternion.Z = value1.Z - value2.Z;
			quaternion.W = value1.W - value2.W;
			return quaternion;
		}

		public static Quaternion Multiply(Quaternion value1, Quaternion value2)
		{
			float x = value1.X;
			float y = value1.Y;
			float z = value1.Z;
			float w = value1.W;
			float x2 = value2.X;
			float y2 = value2.Y;
			float z2 = value2.Z;
			float w2 = value2.W;
			float num = y * z2 - z * y2;
			float num2 = z * x2 - x * z2;
			float num3 = x * y2 - y * x2;
			float num4 = x * x2 + y * y2 + z * z2;
			Quaternion quaternion;
			quaternion.X = x * w2 + x2 * w + num;
			quaternion.Y = y * w2 + y2 * w + num2;
			quaternion.Z = z * w2 + z2 * w + num3;
			quaternion.W = w * w2 - num4;
			return quaternion;
		}

		public static Quaternion Multiply(Quaternion value1, float value2)
		{
			Quaternion quaternion;
			quaternion.X = value1.X * value2;
			quaternion.Y = value1.Y * value2;
			quaternion.Z = value1.Z * value2;
			quaternion.W = value1.W * value2;
			return quaternion;
		}

		public static Quaternion Divide(Quaternion value1, Quaternion value2)
		{
			float x = value1.X;
			float y = value1.Y;
			float z = value1.Z;
			float w = value1.W;
			float num = value2.X * value2.X + value2.Y * value2.Y + value2.Z * value2.Z + value2.W * value2.W;
			float num2 = 1f / num;
			float num3 = -value2.X * num2;
			float num4 = -value2.Y * num2;
			float num5 = -value2.Z * num2;
			float num6 = value2.W * num2;
			float num7 = y * num5 - z * num4;
			float num8 = z * num3 - x * num5;
			float num9 = x * num4 - y * num3;
			float num10 = x * num3 + y * num4 + z * num5;
			Quaternion quaternion;
			quaternion.X = x * num6 + num3 * w + num7;
			quaternion.Y = y * num6 + num4 * w + num8;
			quaternion.Z = z * num6 + num5 * w + num9;
			quaternion.W = w * num6 - num10;
			return quaternion;
		}

		public static Quaternion operator -(Quaternion value)
		{
			Quaternion quaternion;
			quaternion.X = -value.X;
			quaternion.Y = -value.Y;
			quaternion.Z = -value.Z;
			quaternion.W = -value.W;
			return quaternion;
		}

		public static Quaternion operator +(Quaternion value1, Quaternion value2)
		{
			Quaternion quaternion;
			quaternion.X = value1.X + value2.X;
			quaternion.Y = value1.Y + value2.Y;
			quaternion.Z = value1.Z + value2.Z;
			quaternion.W = value1.W + value2.W;
			return quaternion;
		}

		public static Quaternion operator -(Quaternion value1, Quaternion value2)
		{
			Quaternion quaternion;
			quaternion.X = value1.X - value2.X;
			quaternion.Y = value1.Y - value2.Y;
			quaternion.Z = value1.Z - value2.Z;
			quaternion.W = value1.W - value2.W;
			return quaternion;
		}

		public static Quaternion operator *(Quaternion value1, Quaternion value2)
		{
			float x = value1.X;
			float y = value1.Y;
			float z = value1.Z;
			float w = value1.W;
			float x2 = value2.X;
			float y2 = value2.Y;
			float z2 = value2.Z;
			float w2 = value2.W;
			float num = y * z2 - z * y2;
			float num2 = z * x2 - x * z2;
			float num3 = x * y2 - y * x2;
			float num4 = x * x2 + y * y2 + z * z2;
			Quaternion quaternion;
			quaternion.X = x * w2 + x2 * w + num;
			quaternion.Y = y * w2 + y2 * w + num2;
			quaternion.Z = z * w2 + z2 * w + num3;
			quaternion.W = w * w2 - num4;
			return quaternion;
		}

		public static Quaternion operator *(Quaternion value1, float value2)
		{
			Quaternion quaternion;
			quaternion.X = value1.X * value2;
			quaternion.Y = value1.Y * value2;
			quaternion.Z = value1.Z * value2;
			quaternion.W = value1.W * value2;
			return quaternion;
		}

		public static Quaternion operator /(Quaternion value1, Quaternion value2)
		{
			float x = value1.X;
			float y = value1.Y;
			float z = value1.Z;
			float w = value1.W;
			float num = value2.X * value2.X + value2.Y * value2.Y + value2.Z * value2.Z + value2.W * value2.W;
			float num2 = 1f / num;
			float num3 = -value2.X * num2;
			float num4 = -value2.Y * num2;
			float num5 = -value2.Z * num2;
			float num6 = value2.W * num2;
			float num7 = y * num5 - z * num4;
			float num8 = z * num3 - x * num5;
			float num9 = x * num4 - y * num3;
			float num10 = x * num3 + y * num4 + z * num5;
			Quaternion quaternion;
			quaternion.X = x * num6 + num3 * w + num7;
			quaternion.Y = y * num6 + num4 * w + num8;
			quaternion.Z = z * num6 + num5 * w + num9;
			quaternion.W = w * num6 - num10;
			return quaternion;
		}

		public static bool operator ==(Quaternion value1, Quaternion value2)
		{
			return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z && value1.W == value2.W;
		}

		public static bool operator !=(Quaternion value1, Quaternion value2)
		{
			return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z || value1.W != value2.W;
		}

		public bool Equals(Quaternion other)
		{
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
		}

		public override bool Equals(object obj)
		{
			return obj is Quaternion && this.Equals((Quaternion)obj);
		}

		public override string ToString()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			return string.Format(currentCulture, "{{X:{0} Y:{1} Z:{2} W:{3}}}", new object[]
			{
				this.X.ToString(currentCulture),
				this.Y.ToString(currentCulture),
				this.Z.ToString(currentCulture),
				this.W.ToString(currentCulture)
			});
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode() + this.W.GetHashCode();
		}

		public float X;

		public float Y;

		public float Z;

		public float W;
	}
}
