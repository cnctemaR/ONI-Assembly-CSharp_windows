using System;
using System.Globalization;
using System.Numerics.Hashing;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Numerics
{
	public struct Vector2 : IEquatable<Vector2>, IFormattable
	{
		public static Vector2 Zero
		{
			get
			{
				return default(Vector2);
			}
		}

		public static Vector2 One
		{
			get
			{
				return new Vector2(1f, 1f);
			}
		}

		public static Vector2 UnitX
		{
			get
			{
				return new Vector2(1f, 0f);
			}
		}

		public static Vector2 UnitY
		{
			get
			{
				return new Vector2(0f, 1f);
			}
		}

		public override int GetHashCode()
		{
			return HashHelpers.Combine(this.X.GetHashCode(), this.Y.GetHashCode());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is Vector2 && this.Equals((Vector2)obj);
		}

		public override string ToString()
		{
			return this.ToString("G", CultureInfo.CurrentCulture);
		}

		public string ToString(string format)
		{
			return this.ToString(format, CultureInfo.CurrentCulture);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string numberGroupSeparator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
			stringBuilder.Append('<');
			stringBuilder.Append(this.X.ToString(format, formatProvider));
			stringBuilder.Append(numberGroupSeparator);
			stringBuilder.Append(' ');
			stringBuilder.Append(this.Y.ToString(format, formatProvider));
			stringBuilder.Append('>');
			return stringBuilder.ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Length()
		{
			if (Vector.IsHardwareAccelerated)
			{
				return MathF.Sqrt(Vector2.Dot(this, this));
			}
			return MathF.Sqrt(this.X * this.X + this.Y * this.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float LengthSquared()
		{
			if (Vector.IsHardwareAccelerated)
			{
				return Vector2.Dot(this, this);
			}
			return this.X * this.X + this.Y * this.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector2 value1, Vector2 value2)
		{
			if (Vector.IsHardwareAccelerated)
			{
				Vector2 vector = value1 - value2;
				return MathF.Sqrt(Vector2.Dot(vector, vector));
			}
			float num = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			return MathF.Sqrt(num * num + num2 * num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceSquared(Vector2 value1, Vector2 value2)
		{
			if (Vector.IsHardwareAccelerated)
			{
				Vector2 vector = value1 - value2;
				return Vector2.Dot(vector, vector);
			}
			float num = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			return num * num + num2 * num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Normalize(Vector2 value)
		{
			if (Vector.IsHardwareAccelerated)
			{
				float num = value.Length();
				return value / num;
			}
			float num2 = value.X * value.X + value.Y * value.Y;
			float num3 = 1f / MathF.Sqrt(num2);
			return new Vector2(value.X * num3, value.Y * num3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Reflect(Vector2 vector, Vector2 normal)
		{
			if (Vector.IsHardwareAccelerated)
			{
				float num = Vector2.Dot(vector, normal);
				return vector - 2f * num * normal;
			}
			float num2 = vector.X * normal.X + vector.Y * normal.Y;
			return new Vector2(vector.X - 2f * num2 * normal.X, vector.Y - 2f * num2 * normal.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
		{
			float num = value1.X;
			num = ((num > max.X) ? max.X : num);
			num = ((num < min.X) ? min.X : num);
			float num2 = value1.Y;
			num2 = ((num2 > max.Y) ? max.Y : num2);
			num2 = ((num2 < min.Y) ? min.Y : num2);
			return new Vector2(num, num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
		{
			return new Vector2(value1.X + (value2.X - value1.X) * amount, value1.Y + (value2.Y - value1.Y) * amount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Transform(Vector2 position, Matrix3x2 matrix)
		{
			return new Vector2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M31, position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Transform(Vector2 position, Matrix4x4 matrix)
		{
			return new Vector2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41, position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 TransformNormal(Vector2 normal, Matrix3x2 matrix)
		{
			return new Vector2(normal.X * matrix.M11 + normal.Y * matrix.M21, normal.X * matrix.M12 + normal.Y * matrix.M22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 TransformNormal(Vector2 normal, Matrix4x4 matrix)
		{
			return new Vector2(normal.X * matrix.M11 + normal.Y * matrix.M21, normal.X * matrix.M12 + normal.Y * matrix.M22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Transform(Vector2 value, Quaternion rotation)
		{
			float num = rotation.X + rotation.X;
			float num2 = rotation.Y + rotation.Y;
			float num3 = rotation.Z + rotation.Z;
			float num4 = rotation.W * num3;
			float num5 = rotation.X * num;
			float num6 = rotation.X * num2;
			float num7 = rotation.Y * num2;
			float num8 = rotation.Z * num3;
			return new Vector2(value.X * (1f - num7 - num8) + value.Y * (num6 - num4), value.X * (num6 + num4) + value.Y * (1f - num5 - num8));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Add(Vector2 left, Vector2 right)
		{
			return left + right;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Subtract(Vector2 left, Vector2 right)
		{
			return left - right;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Multiply(Vector2 left, Vector2 right)
		{
			return left * right;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Multiply(Vector2 left, float right)
		{
			return left * right;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Multiply(float left, Vector2 right)
		{
			return left * right;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Divide(Vector2 left, Vector2 right)
		{
			return left / right;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Divide(Vector2 left, float divisor)
		{
			return left / divisor;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Negate(Vector2 value)
		{
			return -value;
		}

		[JitIntrinsic]
		public Vector2(float value)
		{
			this = new Vector2(value, value);
		}

		[JitIntrinsic]
		public Vector2(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(float[] array)
		{
			this.CopyTo(array, 0);
		}

		public void CopyTo(float[] array, int index)
		{
			if (array == null)
			{
				throw new NullReferenceException("The method was called with a null array argument.");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", SR.Format("Index was out of bounds:", index));
			}
			if (array.Length - index < 2)
			{
				throw new ArgumentException(SR.Format("Number of elements in source vector is greater than the destination array", index));
			}
			array[index] = this.X;
			array[index + 1] = this.Y;
		}

		[JitIntrinsic]
		public bool Equals(Vector2 other)
		{
			return this.X == other.X && this.Y == other.Y;
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector2 value1, Vector2 value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y;
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Min(Vector2 value1, Vector2 value2)
		{
			return new Vector2((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y);
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Max(Vector2 value1, Vector2 value2)
		{
			return new Vector2((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y);
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Abs(Vector2 value)
		{
			return new Vector2(MathF.Abs(value.X), MathF.Abs(value.Y));
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 SquareRoot(Vector2 value)
		{
			return new Vector2(MathF.Sqrt(value.X), MathF.Sqrt(value.Y));
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator +(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X + right.X, left.Y + right.Y);
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator -(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X - right.X, left.Y - right.Y);
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X * right.X, left.Y * right.Y);
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(float left, Vector2 right)
		{
			return new Vector2(left, left) * right;
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 left, float right)
		{
			return left * new Vector2(right, right);
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X / right.X, left.Y / right.Y);
		}

		[JitIntrinsic]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(Vector2 value1, float value2)
		{
			float num = 1f / value2;
			return new Vector2(value1.X * num, value1.Y * num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator -(Vector2 value)
		{
			return Vector2.Zero - value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !(left == right);
		}

		public float X;

		public float Y;
	}
}
