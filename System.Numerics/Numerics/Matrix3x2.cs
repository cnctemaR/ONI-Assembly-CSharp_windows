using System;
using System.Globalization;

namespace System.Numerics
{
	public struct Matrix3x2 : IEquatable<Matrix3x2>
	{
		public static Matrix3x2 Identity
		{
			get
			{
				return Matrix3x2._identity;
			}
		}

		public bool IsIdentity
		{
			get
			{
				return this.M11 == 1f && this.M22 == 1f && this.M12 == 0f && this.M21 == 0f && this.M31 == 0f && this.M32 == 0f;
			}
		}

		public Vector2 Translation
		{
			get
			{
				return new Vector2(this.M31, this.M32);
			}
			set
			{
				this.M31 = value.X;
				this.M32 = value.Y;
			}
		}

		public Matrix3x2(float m11, float m12, float m21, float m22, float m31, float m32)
		{
			this.M11 = m11;
			this.M12 = m12;
			this.M21 = m21;
			this.M22 = m22;
			this.M31 = m31;
			this.M32 = m32;
		}

		public static Matrix3x2 CreateTranslation(Vector2 position)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = 1f;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = 1f;
			matrix3x.M31 = position.X;
			matrix3x.M32 = position.Y;
			return matrix3x;
		}

		public static Matrix3x2 CreateTranslation(float xPosition, float yPosition)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = 1f;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = 1f;
			matrix3x.M31 = xPosition;
			matrix3x.M32 = yPosition;
			return matrix3x;
		}

		public static Matrix3x2 CreateScale(float xScale, float yScale)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = xScale;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = yScale;
			matrix3x.M31 = 0f;
			matrix3x.M32 = 0f;
			return matrix3x;
		}

		public static Matrix3x2 CreateScale(float xScale, float yScale, Vector2 centerPoint)
		{
			float num = centerPoint.X * (1f - xScale);
			float num2 = centerPoint.Y * (1f - yScale);
			Matrix3x2 matrix3x;
			matrix3x.M11 = xScale;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = yScale;
			matrix3x.M31 = num;
			matrix3x.M32 = num2;
			return matrix3x;
		}

		public static Matrix3x2 CreateScale(Vector2 scales)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = scales.X;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = scales.Y;
			matrix3x.M31 = 0f;
			matrix3x.M32 = 0f;
			return matrix3x;
		}

		public static Matrix3x2 CreateScale(Vector2 scales, Vector2 centerPoint)
		{
			float num = centerPoint.X * (1f - scales.X);
			float num2 = centerPoint.Y * (1f - scales.Y);
			Matrix3x2 matrix3x;
			matrix3x.M11 = scales.X;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = scales.Y;
			matrix3x.M31 = num;
			matrix3x.M32 = num2;
			return matrix3x;
		}

		public static Matrix3x2 CreateScale(float scale)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = scale;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = scale;
			matrix3x.M31 = 0f;
			matrix3x.M32 = 0f;
			return matrix3x;
		}

		public static Matrix3x2 CreateScale(float scale, Vector2 centerPoint)
		{
			float num = centerPoint.X * (1f - scale);
			float num2 = centerPoint.Y * (1f - scale);
			Matrix3x2 matrix3x;
			matrix3x.M11 = scale;
			matrix3x.M12 = 0f;
			matrix3x.M21 = 0f;
			matrix3x.M22 = scale;
			matrix3x.M31 = num;
			matrix3x.M32 = num2;
			return matrix3x;
		}

		public static Matrix3x2 CreateSkew(float radiansX, float radiansY)
		{
			float num = MathF.Tan(radiansX);
			float num2 = MathF.Tan(radiansY);
			Matrix3x2 matrix3x;
			matrix3x.M11 = 1f;
			matrix3x.M12 = num2;
			matrix3x.M21 = num;
			matrix3x.M22 = 1f;
			matrix3x.M31 = 0f;
			matrix3x.M32 = 0f;
			return matrix3x;
		}

		public static Matrix3x2 CreateSkew(float radiansX, float radiansY, Vector2 centerPoint)
		{
			float num = MathF.Tan(radiansX);
			float num2 = MathF.Tan(radiansY);
			float num3 = -centerPoint.Y * num;
			float num4 = -centerPoint.X * num2;
			Matrix3x2 matrix3x;
			matrix3x.M11 = 1f;
			matrix3x.M12 = num2;
			matrix3x.M21 = num;
			matrix3x.M22 = 1f;
			matrix3x.M31 = num3;
			matrix3x.M32 = num4;
			return matrix3x;
		}

		public static Matrix3x2 CreateRotation(float radians)
		{
			radians = MathF.IEEERemainder(radians, 6.2831855f);
			float num;
			float num2;
			if (radians > -1.7453294E-05f && radians < 1.7453294E-05f)
			{
				num = 1f;
				num2 = 0f;
			}
			else if (radians > 1.570779f && radians < 1.5708138f)
			{
				num = 0f;
				num2 = 1f;
			}
			else if (radians < -3.1415753f || radians > 3.1415753f)
			{
				num = -1f;
				num2 = 0f;
			}
			else if (radians > -1.5708138f && radians < -1.570779f)
			{
				num = 0f;
				num2 = -1f;
			}
			else
			{
				num = MathF.Cos(radians);
				num2 = MathF.Sin(radians);
			}
			Matrix3x2 matrix3x;
			matrix3x.M11 = num;
			matrix3x.M12 = num2;
			matrix3x.M21 = -num2;
			matrix3x.M22 = num;
			matrix3x.M31 = 0f;
			matrix3x.M32 = 0f;
			return matrix3x;
		}

		public static Matrix3x2 CreateRotation(float radians, Vector2 centerPoint)
		{
			radians = MathF.IEEERemainder(radians, 6.2831855f);
			float num;
			float num2;
			if (radians > -1.7453294E-05f && radians < 1.7453294E-05f)
			{
				num = 1f;
				num2 = 0f;
			}
			else if (radians > 1.570779f && radians < 1.5708138f)
			{
				num = 0f;
				num2 = 1f;
			}
			else if (radians < -3.1415753f || radians > 3.1415753f)
			{
				num = -1f;
				num2 = 0f;
			}
			else if (radians > -1.5708138f && radians < -1.570779f)
			{
				num = 0f;
				num2 = -1f;
			}
			else
			{
				num = MathF.Cos(radians);
				num2 = MathF.Sin(radians);
			}
			float num3 = centerPoint.X * (1f - num) + centerPoint.Y * num2;
			float num4 = centerPoint.Y * (1f - num) - centerPoint.X * num2;
			Matrix3x2 matrix3x;
			matrix3x.M11 = num;
			matrix3x.M12 = num2;
			matrix3x.M21 = -num2;
			matrix3x.M22 = num;
			matrix3x.M31 = num3;
			matrix3x.M32 = num4;
			return matrix3x;
		}

		public float GetDeterminant()
		{
			return this.M11 * this.M22 - this.M21 * this.M12;
		}

		public static bool Invert(Matrix3x2 matrix, out Matrix3x2 result)
		{
			float num = matrix.M11 * matrix.M22 - matrix.M21 * matrix.M12;
			if (MathF.Abs(num) < 1E-45f)
			{
				result = new Matrix3x2(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
				return false;
			}
			float num2 = 1f / num;
			result.M11 = matrix.M22 * num2;
			result.M12 = -matrix.M12 * num2;
			result.M21 = -matrix.M21 * num2;
			result.M22 = matrix.M11 * num2;
			result.M31 = (matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22) * num2;
			result.M32 = (matrix.M31 * matrix.M12 - matrix.M11 * matrix.M32) * num2;
			return true;
		}

		public static Matrix3x2 Lerp(Matrix3x2 matrix1, Matrix3x2 matrix2, float amount)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;
			matrix3x.M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount;
			matrix3x.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;
			matrix3x.M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount;
			matrix3x.M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount;
			matrix3x.M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount;
			return matrix3x;
		}

		public static Matrix3x2 Negate(Matrix3x2 value)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = -value.M11;
			matrix3x.M12 = -value.M12;
			matrix3x.M21 = -value.M21;
			matrix3x.M22 = -value.M22;
			matrix3x.M31 = -value.M31;
			matrix3x.M32 = -value.M32;
			return matrix3x;
		}

		public static Matrix3x2 Add(Matrix3x2 value1, Matrix3x2 value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 + value2.M11;
			matrix3x.M12 = value1.M12 + value2.M12;
			matrix3x.M21 = value1.M21 + value2.M21;
			matrix3x.M22 = value1.M22 + value2.M22;
			matrix3x.M31 = value1.M31 + value2.M31;
			matrix3x.M32 = value1.M32 + value2.M32;
			return matrix3x;
		}

		public static Matrix3x2 Subtract(Matrix3x2 value1, Matrix3x2 value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 - value2.M11;
			matrix3x.M12 = value1.M12 - value2.M12;
			matrix3x.M21 = value1.M21 - value2.M21;
			matrix3x.M22 = value1.M22 - value2.M22;
			matrix3x.M31 = value1.M31 - value2.M31;
			matrix3x.M32 = value1.M32 - value2.M32;
			return matrix3x;
		}

		public static Matrix3x2 Multiply(Matrix3x2 value1, Matrix3x2 value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21;
			matrix3x.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22;
			matrix3x.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21;
			matrix3x.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22;
			matrix3x.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value2.M31;
			matrix3x.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value2.M32;
			return matrix3x;
		}

		public static Matrix3x2 Multiply(Matrix3x2 value1, float value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 * value2;
			matrix3x.M12 = value1.M12 * value2;
			matrix3x.M21 = value1.M21 * value2;
			matrix3x.M22 = value1.M22 * value2;
			matrix3x.M31 = value1.M31 * value2;
			matrix3x.M32 = value1.M32 * value2;
			return matrix3x;
		}

		public static Matrix3x2 operator -(Matrix3x2 value)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = -value.M11;
			matrix3x.M12 = -value.M12;
			matrix3x.M21 = -value.M21;
			matrix3x.M22 = -value.M22;
			matrix3x.M31 = -value.M31;
			matrix3x.M32 = -value.M32;
			return matrix3x;
		}

		public static Matrix3x2 operator +(Matrix3x2 value1, Matrix3x2 value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 + value2.M11;
			matrix3x.M12 = value1.M12 + value2.M12;
			matrix3x.M21 = value1.M21 + value2.M21;
			matrix3x.M22 = value1.M22 + value2.M22;
			matrix3x.M31 = value1.M31 + value2.M31;
			matrix3x.M32 = value1.M32 + value2.M32;
			return matrix3x;
		}

		public static Matrix3x2 operator -(Matrix3x2 value1, Matrix3x2 value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 - value2.M11;
			matrix3x.M12 = value1.M12 - value2.M12;
			matrix3x.M21 = value1.M21 - value2.M21;
			matrix3x.M22 = value1.M22 - value2.M22;
			matrix3x.M31 = value1.M31 - value2.M31;
			matrix3x.M32 = value1.M32 - value2.M32;
			return matrix3x;
		}

		public static Matrix3x2 operator *(Matrix3x2 value1, Matrix3x2 value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21;
			matrix3x.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22;
			matrix3x.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21;
			matrix3x.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22;
			matrix3x.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value2.M31;
			matrix3x.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value2.M32;
			return matrix3x;
		}

		public static Matrix3x2 operator *(Matrix3x2 value1, float value2)
		{
			Matrix3x2 matrix3x;
			matrix3x.M11 = value1.M11 * value2;
			matrix3x.M12 = value1.M12 * value2;
			matrix3x.M21 = value1.M21 * value2;
			matrix3x.M22 = value1.M22 * value2;
			matrix3x.M31 = value1.M31 * value2;
			matrix3x.M32 = value1.M32 * value2;
			return matrix3x;
		}

		public static bool operator ==(Matrix3x2 value1, Matrix3x2 value2)
		{
			return value1.M11 == value2.M11 && value1.M22 == value2.M22 && value1.M12 == value2.M12 && value1.M21 == value2.M21 && value1.M31 == value2.M31 && value1.M32 == value2.M32;
		}

		public static bool operator !=(Matrix3x2 value1, Matrix3x2 value2)
		{
			return value1.M11 != value2.M11 || value1.M12 != value2.M12 || value1.M21 != value2.M21 || value1.M22 != value2.M22 || value1.M31 != value2.M31 || value1.M32 != value2.M32;
		}

		public bool Equals(Matrix3x2 other)
		{
			return this.M11 == other.M11 && this.M22 == other.M22 && this.M12 == other.M12 && this.M21 == other.M21 && this.M31 == other.M31 && this.M32 == other.M32;
		}

		public override bool Equals(object obj)
		{
			return obj is Matrix3x2 && this.Equals((Matrix3x2)obj);
		}

		public override string ToString()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			return string.Format(currentCulture, "{{ {{M11:{0} M12:{1}}} {{M21:{2} M22:{3}}} {{M31:{4} M32:{5}}} }}", new object[]
			{
				this.M11.ToString(currentCulture),
				this.M12.ToString(currentCulture),
				this.M21.ToString(currentCulture),
				this.M22.ToString(currentCulture),
				this.M31.ToString(currentCulture),
				this.M32.ToString(currentCulture)
			});
		}

		public override int GetHashCode()
		{
			return this.M11.GetHashCode() + this.M12.GetHashCode() + this.M21.GetHashCode() + this.M22.GetHashCode() + this.M31.GetHashCode() + this.M32.GetHashCode();
		}

		public float M11;

		public float M12;

		public float M21;

		public float M22;

		public float M31;

		public float M32;

		private static readonly Matrix3x2 _identity = new Matrix3x2(1f, 0f, 0f, 1f, 0f, 0f);
	}
}
