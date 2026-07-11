using System;
using System.Globalization;

namespace System.Numerics
{
	public struct Matrix4x4 : IEquatable<Matrix4x4>
	{
		public static Matrix4x4 Identity
		{
			get
			{
				return Matrix4x4._identity;
			}
		}

		public bool IsIdentity
		{
			get
			{
				return this.M11 == 1f && this.M22 == 1f && this.M33 == 1f && this.M44 == 1f && this.M12 == 0f && this.M13 == 0f && this.M14 == 0f && this.M21 == 0f && this.M23 == 0f && this.M24 == 0f && this.M31 == 0f && this.M32 == 0f && this.M34 == 0f && this.M41 == 0f && this.M42 == 0f && this.M43 == 0f;
			}
		}

		public Vector3 Translation
		{
			get
			{
				return new Vector3(this.M41, this.M42, this.M43);
			}
			set
			{
				this.M41 = value.X;
				this.M42 = value.Y;
				this.M43 = value.Z;
			}
		}

		public Matrix4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
		{
			this.M11 = m11;
			this.M12 = m12;
			this.M13 = m13;
			this.M14 = m14;
			this.M21 = m21;
			this.M22 = m22;
			this.M23 = m23;
			this.M24 = m24;
			this.M31 = m31;
			this.M32 = m32;
			this.M33 = m33;
			this.M34 = m34;
			this.M41 = m41;
			this.M42 = m42;
			this.M43 = m43;
			this.M44 = m44;
		}

		public Matrix4x4(Matrix3x2 value)
		{
			this.M11 = value.M11;
			this.M12 = value.M12;
			this.M13 = 0f;
			this.M14 = 0f;
			this.M21 = value.M21;
			this.M22 = value.M22;
			this.M23 = 0f;
			this.M24 = 0f;
			this.M31 = 0f;
			this.M32 = 0f;
			this.M33 = 1f;
			this.M34 = 0f;
			this.M41 = value.M31;
			this.M42 = value.M32;
			this.M43 = 0f;
			this.M44 = 1f;
		}

		public static Matrix4x4 CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
		{
			Vector3 vector = new Vector3(objectPosition.X - cameraPosition.X, objectPosition.Y - cameraPosition.Y, objectPosition.Z - cameraPosition.Z);
			float num = vector.LengthSquared();
			if (num < 0.0001f)
			{
				vector = -cameraForwardVector;
			}
			else
			{
				vector = Vector3.Multiply(vector, 1f / MathF.Sqrt(num));
			}
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Matrix4x4 matrix4x;
			matrix4x.M11 = vector2.X;
			matrix4x.M12 = vector2.Y;
			matrix4x.M13 = vector2.Z;
			matrix4x.M14 = 0f;
			matrix4x.M21 = vector3.X;
			matrix4x.M22 = vector3.Y;
			matrix4x.M23 = vector3.Z;
			matrix4x.M24 = 0f;
			matrix4x.M31 = vector.X;
			matrix4x.M32 = vector.Y;
			matrix4x.M33 = vector.Z;
			matrix4x.M34 = 0f;
			matrix4x.M41 = objectPosition.X;
			matrix4x.M42 = objectPosition.Y;
			matrix4x.M43 = objectPosition.Z;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateConstrainedBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 rotateAxis, Vector3 cameraForwardVector, Vector3 objectForwardVector)
		{
			Vector3 vector = new Vector3(objectPosition.X - cameraPosition.X, objectPosition.Y - cameraPosition.Y, objectPosition.Z - cameraPosition.Z);
			float num = vector.LengthSquared();
			if (num < 0.0001f)
			{
				vector = -cameraForwardVector;
			}
			else
			{
				vector = Vector3.Multiply(vector, 1f / MathF.Sqrt(num));
			}
			Vector3 vector2;
			Vector3 vector3;
			if (MathF.Abs(Vector3.Dot(rotateAxis, vector)) > 0.99825466f)
			{
				vector2 = objectForwardVector;
				if (MathF.Abs(Vector3.Dot(rotateAxis, vector2)) > 0.99825466f)
				{
					vector2 = ((MathF.Abs(rotateAxis.Z) > 0.99825466f) ? new Vector3(1f, 0f, 0f) : new Vector3(0f, 0f, -1f));
				}
				vector3 = Vector3.Normalize(Vector3.Cross(rotateAxis, vector2));
				vector2 = Vector3.Normalize(Vector3.Cross(vector3, rotateAxis));
			}
			else
			{
				vector3 = Vector3.Normalize(Vector3.Cross(rotateAxis, vector));
				vector2 = Vector3.Normalize(Vector3.Cross(vector3, rotateAxis));
			}
			Matrix4x4 matrix4x;
			matrix4x.M11 = vector3.X;
			matrix4x.M12 = vector3.Y;
			matrix4x.M13 = vector3.Z;
			matrix4x.M14 = 0f;
			matrix4x.M21 = rotateAxis.X;
			matrix4x.M22 = rotateAxis.Y;
			matrix4x.M23 = rotateAxis.Z;
			matrix4x.M24 = 0f;
			matrix4x.M31 = vector2.X;
			matrix4x.M32 = vector2.Y;
			matrix4x.M33 = vector2.Z;
			matrix4x.M34 = 0f;
			matrix4x.M41 = objectPosition.X;
			matrix4x.M42 = objectPosition.Y;
			matrix4x.M43 = objectPosition.Z;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateTranslation(Vector3 position)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = 1f;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = 1f;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = 1f;
			matrix4x.M34 = 0f;
			matrix4x.M41 = position.X;
			matrix4x.M42 = position.Y;
			matrix4x.M43 = position.Z;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateTranslation(float xPosition, float yPosition, float zPosition)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = 1f;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = 1f;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = 1f;
			matrix4x.M34 = 0f;
			matrix4x.M41 = xPosition;
			matrix4x.M42 = yPosition;
			matrix4x.M43 = zPosition;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateScale(float xScale, float yScale, float zScale)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = xScale;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = yScale;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = zScale;
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateScale(float xScale, float yScale, float zScale, Vector3 centerPoint)
		{
			float num = centerPoint.X * (1f - xScale);
			float num2 = centerPoint.Y * (1f - yScale);
			float num3 = centerPoint.Z * (1f - zScale);
			Matrix4x4 matrix4x;
			matrix4x.M11 = xScale;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = yScale;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = zScale;
			matrix4x.M34 = 0f;
			matrix4x.M41 = num;
			matrix4x.M42 = num2;
			matrix4x.M43 = num3;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateScale(Vector3 scales)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = scales.X;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = scales.Y;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = scales.Z;
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateScale(Vector3 scales, Vector3 centerPoint)
		{
			float num = centerPoint.X * (1f - scales.X);
			float num2 = centerPoint.Y * (1f - scales.Y);
			float num3 = centerPoint.Z * (1f - scales.Z);
			Matrix4x4 matrix4x;
			matrix4x.M11 = scales.X;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = scales.Y;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = scales.Z;
			matrix4x.M34 = 0f;
			matrix4x.M41 = num;
			matrix4x.M42 = num2;
			matrix4x.M43 = num3;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateScale(float scale)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = scale;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = scale;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = scale;
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateScale(float scale, Vector3 centerPoint)
		{
			float num = centerPoint.X * (1f - scale);
			float num2 = centerPoint.Y * (1f - scale);
			float num3 = centerPoint.Z * (1f - scale);
			Matrix4x4 matrix4x;
			matrix4x.M11 = scale;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = scale;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = scale;
			matrix4x.M34 = 0f;
			matrix4x.M41 = num;
			matrix4x.M42 = num2;
			matrix4x.M43 = num3;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateRotationX(float radians)
		{
			float num = MathF.Cos(radians);
			float num2 = MathF.Sin(radians);
			Matrix4x4 matrix4x;
			matrix4x.M11 = 1f;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = num;
			matrix4x.M23 = num2;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = -num2;
			matrix4x.M33 = num;
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateRotationX(float radians, Vector3 centerPoint)
		{
			float num = MathF.Cos(radians);
			float num2 = MathF.Sin(radians);
			float num3 = centerPoint.Y * (1f - num) + centerPoint.Z * num2;
			float num4 = centerPoint.Z * (1f - num) - centerPoint.Y * num2;
			Matrix4x4 matrix4x;
			matrix4x.M11 = 1f;
			matrix4x.M12 = 0f;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = num;
			matrix4x.M23 = num2;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = -num2;
			matrix4x.M33 = num;
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = num3;
			matrix4x.M43 = num4;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateRotationY(float radians)
		{
			float num = MathF.Cos(radians);
			float num2 = MathF.Sin(radians);
			Matrix4x4 matrix4x;
			matrix4x.M11 = num;
			matrix4x.M12 = 0f;
			matrix4x.M13 = -num2;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = 1f;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = num2;
			matrix4x.M32 = 0f;
			matrix4x.M33 = num;
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateRotationY(float radians, Vector3 centerPoint)
		{
			float num = MathF.Cos(radians);
			float num2 = MathF.Sin(radians);
			float num3 = centerPoint.X * (1f - num) - centerPoint.Z * num2;
			float num4 = centerPoint.Z * (1f - num) + centerPoint.X * num2;
			Matrix4x4 matrix4x;
			matrix4x.M11 = num;
			matrix4x.M12 = 0f;
			matrix4x.M13 = -num2;
			matrix4x.M14 = 0f;
			matrix4x.M21 = 0f;
			matrix4x.M22 = 1f;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = num2;
			matrix4x.M32 = 0f;
			matrix4x.M33 = num;
			matrix4x.M34 = 0f;
			matrix4x.M41 = num3;
			matrix4x.M42 = 0f;
			matrix4x.M43 = num4;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateRotationZ(float radians)
		{
			float num = MathF.Cos(radians);
			float num2 = MathF.Sin(radians);
			Matrix4x4 matrix4x;
			matrix4x.M11 = num;
			matrix4x.M12 = num2;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = -num2;
			matrix4x.M22 = num;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = 1f;
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateRotationZ(float radians, Vector3 centerPoint)
		{
			float num = MathF.Cos(radians);
			float num2 = MathF.Sin(radians);
			float num3 = centerPoint.X * (1f - num) + centerPoint.Y * num2;
			float num4 = centerPoint.Y * (1f - num) - centerPoint.X * num2;
			Matrix4x4 matrix4x;
			matrix4x.M11 = num;
			matrix4x.M12 = num2;
			matrix4x.M13 = 0f;
			matrix4x.M14 = 0f;
			matrix4x.M21 = -num2;
			matrix4x.M22 = num;
			matrix4x.M23 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M31 = 0f;
			matrix4x.M32 = 0f;
			matrix4x.M33 = 1f;
			matrix4x.M34 = 0f;
			matrix4x.M41 = num3;
			matrix4x.M42 = num4;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateFromAxisAngle(Vector3 axis, float angle)
		{
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;
			float num = MathF.Sin(angle);
			float num2 = MathF.Cos(angle);
			float num3 = x * x;
			float num4 = y * y;
			float num5 = z * z;
			float num6 = x * y;
			float num7 = x * z;
			float num8 = y * z;
			Matrix4x4 matrix4x;
			matrix4x.M11 = num3 + num2 * (1f - num3);
			matrix4x.M12 = num6 - num2 * num6 + num * z;
			matrix4x.M13 = num7 - num2 * num7 - num * y;
			matrix4x.M14 = 0f;
			matrix4x.M21 = num6 - num2 * num6 - num * z;
			matrix4x.M22 = num4 + num2 * (1f - num4);
			matrix4x.M23 = num8 - num2 * num8 + num * x;
			matrix4x.M24 = 0f;
			matrix4x.M31 = num7 - num2 * num7 + num * y;
			matrix4x.M32 = num8 - num2 * num8 - num * x;
			matrix4x.M33 = num5 + num2 * (1f - num5);
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			if (fieldOfView <= 0f || fieldOfView >= 3.1415927f)
			{
				throw new ArgumentOutOfRangeException("fieldOfView");
			}
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("nearPlaneDistance");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("farPlaneDistance");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentOutOfRangeException("nearPlaneDistance");
			}
			float num = 1f / MathF.Tan(fieldOfView * 0.5f);
			float num2 = num / aspectRatio;
			Matrix4x4 matrix4x;
			matrix4x.M11 = num2;
			matrix4x.M12 = (matrix4x.M13 = (matrix4x.M14 = 0f));
			matrix4x.M22 = num;
			matrix4x.M21 = (matrix4x.M23 = (matrix4x.M24 = 0f));
			matrix4x.M31 = (matrix4x.M32 = 0f);
			float num3 = (float.IsPositiveInfinity(farPlaneDistance) ? (-1f) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance)));
			matrix4x.M33 = num3;
			matrix4x.M34 = -1f;
			matrix4x.M41 = (matrix4x.M42 = (matrix4x.M44 = 0f));
			matrix4x.M43 = nearPlaneDistance * num3;
			return matrix4x;
		}

		public static Matrix4x4 CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("nearPlaneDistance");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("farPlaneDistance");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentOutOfRangeException("nearPlaneDistance");
			}
			Matrix4x4 matrix4x;
			matrix4x.M11 = 2f * nearPlaneDistance / width;
			matrix4x.M12 = (matrix4x.M13 = (matrix4x.M14 = 0f));
			matrix4x.M22 = 2f * nearPlaneDistance / height;
			matrix4x.M21 = (matrix4x.M23 = (matrix4x.M24 = 0f));
			float num = (float.IsPositiveInfinity(farPlaneDistance) ? (-1f) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance)));
			matrix4x.M33 = num;
			matrix4x.M31 = (matrix4x.M32 = 0f);
			matrix4x.M34 = -1f;
			matrix4x.M41 = (matrix4x.M42 = (matrix4x.M44 = 0f));
			matrix4x.M43 = nearPlaneDistance * num;
			return matrix4x;
		}

		public static Matrix4x4 CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
		{
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("nearPlaneDistance");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentOutOfRangeException("farPlaneDistance");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentOutOfRangeException("nearPlaneDistance");
			}
			Matrix4x4 matrix4x;
			matrix4x.M11 = 2f * nearPlaneDistance / (right - left);
			matrix4x.M12 = (matrix4x.M13 = (matrix4x.M14 = 0f));
			matrix4x.M22 = 2f * nearPlaneDistance / (top - bottom);
			matrix4x.M21 = (matrix4x.M23 = (matrix4x.M24 = 0f));
			matrix4x.M31 = (left + right) / (right - left);
			matrix4x.M32 = (top + bottom) / (top - bottom);
			float num = (float.IsPositiveInfinity(farPlaneDistance) ? (-1f) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance)));
			matrix4x.M33 = num;
			matrix4x.M34 = -1f;
			matrix4x.M43 = nearPlaneDistance * num;
			matrix4x.M41 = (matrix4x.M42 = (matrix4x.M44 = 0f));
			return matrix4x;
		}

		public static Matrix4x4 CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = 2f / width;
			matrix4x.M12 = (matrix4x.M13 = (matrix4x.M14 = 0f));
			matrix4x.M22 = 2f / height;
			matrix4x.M21 = (matrix4x.M23 = (matrix4x.M24 = 0f));
			matrix4x.M33 = 1f / (zNearPlane - zFarPlane);
			matrix4x.M31 = (matrix4x.M32 = (matrix4x.M34 = 0f));
			matrix4x.M41 = (matrix4x.M42 = 0f);
			matrix4x.M43 = zNearPlane / (zNearPlane - zFarPlane);
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = 2f / (right - left);
			matrix4x.M12 = (matrix4x.M13 = (matrix4x.M14 = 0f));
			matrix4x.M22 = 2f / (top - bottom);
			matrix4x.M21 = (matrix4x.M23 = (matrix4x.M24 = 0f));
			matrix4x.M33 = 1f / (zNearPlane - zFarPlane);
			matrix4x.M31 = (matrix4x.M32 = (matrix4x.M34 = 0f));
			matrix4x.M41 = (left + right) / (left - right);
			matrix4x.M42 = (top + bottom) / (bottom - top);
			matrix4x.M43 = zNearPlane / (zNearPlane - zFarPlane);
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
		{
			Vector3 vector = Vector3.Normalize(cameraPosition - cameraTarget);
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Matrix4x4 matrix4x;
			matrix4x.M11 = vector2.X;
			matrix4x.M12 = vector3.X;
			matrix4x.M13 = vector.X;
			matrix4x.M14 = 0f;
			matrix4x.M21 = vector2.Y;
			matrix4x.M22 = vector3.Y;
			matrix4x.M23 = vector.Y;
			matrix4x.M24 = 0f;
			matrix4x.M31 = vector2.Z;
			matrix4x.M32 = vector3.Z;
			matrix4x.M33 = vector.Z;
			matrix4x.M34 = 0f;
			matrix4x.M41 = -Vector3.Dot(vector2, cameraPosition);
			matrix4x.M42 = -Vector3.Dot(vector3, cameraPosition);
			matrix4x.M43 = -Vector3.Dot(vector, cameraPosition);
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
		{
			Vector3 vector = Vector3.Normalize(-forward);
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Matrix4x4 matrix4x;
			matrix4x.M11 = vector2.X;
			matrix4x.M12 = vector2.Y;
			matrix4x.M13 = vector2.Z;
			matrix4x.M14 = 0f;
			matrix4x.M21 = vector3.X;
			matrix4x.M22 = vector3.Y;
			matrix4x.M23 = vector3.Z;
			matrix4x.M24 = 0f;
			matrix4x.M31 = vector.X;
			matrix4x.M32 = vector.Y;
			matrix4x.M33 = vector.Z;
			matrix4x.M34 = 0f;
			matrix4x.M41 = position.X;
			matrix4x.M42 = position.Y;
			matrix4x.M43 = position.Z;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateFromQuaternion(Quaternion quaternion)
		{
			float num = quaternion.X * quaternion.X;
			float num2 = quaternion.Y * quaternion.Y;
			float num3 = quaternion.Z * quaternion.Z;
			float num4 = quaternion.X * quaternion.Y;
			float num5 = quaternion.Z * quaternion.W;
			float num6 = quaternion.Z * quaternion.X;
			float num7 = quaternion.Y * quaternion.W;
			float num8 = quaternion.Y * quaternion.Z;
			float num9 = quaternion.X * quaternion.W;
			Matrix4x4 matrix4x;
			matrix4x.M11 = 1f - 2f * (num2 + num3);
			matrix4x.M12 = 2f * (num4 + num5);
			matrix4x.M13 = 2f * (num6 - num7);
			matrix4x.M14 = 0f;
			matrix4x.M21 = 2f * (num4 - num5);
			matrix4x.M22 = 1f - 2f * (num3 + num);
			matrix4x.M23 = 2f * (num8 + num9);
			matrix4x.M24 = 0f;
			matrix4x.M31 = 2f * (num6 + num7);
			matrix4x.M32 = 2f * (num8 - num9);
			matrix4x.M33 = 1f - 2f * (num2 + num);
			matrix4x.M34 = 0f;
			matrix4x.M41 = 0f;
			matrix4x.M42 = 0f;
			matrix4x.M43 = 0f;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			return Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
		}

		public static Matrix4x4 CreateShadow(Vector3 lightDirection, Plane plane)
		{
			Plane plane2 = Plane.Normalize(plane);
			float num = plane2.Normal.X * lightDirection.X + plane2.Normal.Y * lightDirection.Y + plane2.Normal.Z * lightDirection.Z;
			float num2 = -plane2.Normal.X;
			float num3 = -plane2.Normal.Y;
			float num4 = -plane2.Normal.Z;
			float num5 = -plane2.D;
			Matrix4x4 matrix4x;
			matrix4x.M11 = num2 * lightDirection.X + num;
			matrix4x.M21 = num3 * lightDirection.X;
			matrix4x.M31 = num4 * lightDirection.X;
			matrix4x.M41 = num5 * lightDirection.X;
			matrix4x.M12 = num2 * lightDirection.Y;
			matrix4x.M22 = num3 * lightDirection.Y + num;
			matrix4x.M32 = num4 * lightDirection.Y;
			matrix4x.M42 = num5 * lightDirection.Y;
			matrix4x.M13 = num2 * lightDirection.Z;
			matrix4x.M23 = num3 * lightDirection.Z;
			matrix4x.M33 = num4 * lightDirection.Z + num;
			matrix4x.M43 = num5 * lightDirection.Z;
			matrix4x.M14 = 0f;
			matrix4x.M24 = 0f;
			matrix4x.M34 = 0f;
			matrix4x.M44 = num;
			return matrix4x;
		}

		public static Matrix4x4 CreateReflection(Plane value)
		{
			value = Plane.Normalize(value);
			float x = value.Normal.X;
			float y = value.Normal.Y;
			float z = value.Normal.Z;
			float num = -2f * x;
			float num2 = -2f * y;
			float num3 = -2f * z;
			Matrix4x4 matrix4x;
			matrix4x.M11 = num * x + 1f;
			matrix4x.M12 = num2 * x;
			matrix4x.M13 = num3 * x;
			matrix4x.M14 = 0f;
			matrix4x.M21 = num * y;
			matrix4x.M22 = num2 * y + 1f;
			matrix4x.M23 = num3 * y;
			matrix4x.M24 = 0f;
			matrix4x.M31 = num * z;
			matrix4x.M32 = num2 * z;
			matrix4x.M33 = num3 * z + 1f;
			matrix4x.M34 = 0f;
			matrix4x.M41 = num * value.D;
			matrix4x.M42 = num2 * value.D;
			matrix4x.M43 = num3 * value.D;
			matrix4x.M44 = 1f;
			return matrix4x;
		}

		public float GetDeterminant()
		{
			float m = this.M11;
			float m2 = this.M12;
			float m3 = this.M13;
			float m4 = this.M14;
			float m5 = this.M21;
			float m6 = this.M22;
			float m7 = this.M23;
			float m8 = this.M24;
			float m9 = this.M31;
			float m10 = this.M32;
			float m11 = this.M33;
			float m12 = this.M34;
			float m13 = this.M41;
			float m14 = this.M42;
			float m15 = this.M43;
			float m16 = this.M44;
			float num = m11 * m16 - m12 * m15;
			float num2 = m10 * m16 - m12 * m14;
			float num3 = m10 * m15 - m11 * m14;
			float num4 = m9 * m16 - m12 * m13;
			float num5 = m9 * m15 - m11 * m13;
			float num6 = m9 * m14 - m10 * m13;
			return m * (m6 * num - m7 * num2 + m8 * num3) - m2 * (m5 * num - m7 * num4 + m8 * num5) + m3 * (m5 * num2 - m6 * num4 + m8 * num6) - m4 * (m5 * num3 - m6 * num5 + m7 * num6);
		}

		public static bool Invert(Matrix4x4 matrix, out Matrix4x4 result)
		{
			float m = matrix.M11;
			float m2 = matrix.M12;
			float m3 = matrix.M13;
			float m4 = matrix.M14;
			float m5 = matrix.M21;
			float m6 = matrix.M22;
			float m7 = matrix.M23;
			float m8 = matrix.M24;
			float m9 = matrix.M31;
			float m10 = matrix.M32;
			float m11 = matrix.M33;
			float m12 = matrix.M34;
			float m13 = matrix.M41;
			float m14 = matrix.M42;
			float m15 = matrix.M43;
			float m16 = matrix.M44;
			float num = m11 * m16 - m12 * m15;
			float num2 = m10 * m16 - m12 * m14;
			float num3 = m10 * m15 - m11 * m14;
			float num4 = m9 * m16 - m12 * m13;
			float num5 = m9 * m15 - m11 * m13;
			float num6 = m9 * m14 - m10 * m13;
			float num7 = m6 * num - m7 * num2 + m8 * num3;
			float num8 = -(m5 * num - m7 * num4 + m8 * num5);
			float num9 = m5 * num2 - m6 * num4 + m8 * num6;
			float num10 = -(m5 * num3 - m6 * num5 + m7 * num6);
			float num11 = m * num7 + m2 * num8 + m3 * num9 + m4 * num10;
			if (MathF.Abs(num11) < 1E-45f)
			{
				result = new Matrix4x4(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);
				return false;
			}
			float num12 = 1f / num11;
			result.M11 = num7 * num12;
			result.M21 = num8 * num12;
			result.M31 = num9 * num12;
			result.M41 = num10 * num12;
			result.M12 = -(m2 * num - m3 * num2 + m4 * num3) * num12;
			result.M22 = (m * num - m3 * num4 + m4 * num5) * num12;
			result.M32 = -(m * num2 - m2 * num4 + m4 * num6) * num12;
			result.M42 = (m * num3 - m2 * num5 + m3 * num6) * num12;
			float num13 = m7 * m16 - m8 * m15;
			float num14 = m6 * m16 - m8 * m14;
			float num15 = m6 * m15 - m7 * m14;
			float num16 = m5 * m16 - m8 * m13;
			float num17 = m5 * m15 - m7 * m13;
			float num18 = m5 * m14 - m6 * m13;
			result.M13 = (m2 * num13 - m3 * num14 + m4 * num15) * num12;
			result.M23 = -(m * num13 - m3 * num16 + m4 * num17) * num12;
			result.M33 = (m * num14 - m2 * num16 + m4 * num18) * num12;
			result.M43 = -(m * num15 - m2 * num17 + m3 * num18) * num12;
			float num19 = m7 * m12 - m8 * m11;
			float num20 = m6 * m12 - m8 * m10;
			float num21 = m6 * m11 - m7 * m10;
			float num22 = m5 * m12 - m8 * m9;
			float num23 = m5 * m11 - m7 * m9;
			float num24 = m5 * m10 - m6 * m9;
			result.M14 = -(m2 * num19 - m3 * num20 + m4 * num21) * num12;
			result.M24 = (m * num19 - m3 * num22 + m4 * num23) * num12;
			result.M34 = -(m * num20 - m2 * num22 + m4 * num24) * num12;
			result.M44 = (m * num21 - m2 * num23 + m3 * num24) * num12;
			return true;
		}

		public unsafe static bool Decompose(Matrix4x4 matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
		{
			bool flag = true;
			fixed (Vector3* ptr = &scale)
			{
				float* ptr2 = (float*)ptr;
				Matrix4x4.VectorBasis vectorBasis;
				Vector3** ptr3 = (Vector3**)(&vectorBasis);
				Matrix4x4 identity = Matrix4x4.Identity;
				Matrix4x4.CanonicalBasis canonicalBasis = default(Matrix4x4.CanonicalBasis);
				Vector3* ptr4 = &canonicalBasis.Row0;
				canonicalBasis.Row0 = new Vector3(1f, 0f, 0f);
				canonicalBasis.Row1 = new Vector3(0f, 1f, 0f);
				canonicalBasis.Row2 = new Vector3(0f, 0f, 1f);
				translation = new Vector3(matrix.M41, matrix.M42, matrix.M43);
				*(IntPtr*)ptr3 = &identity.M11;
				*(IntPtr*)(ptr3 + sizeof(Vector3*) / sizeof(Vector3*)) = &identity.M21;
				*(IntPtr*)(ptr3 + (IntPtr)2 * (IntPtr)sizeof(Vector3*) / (IntPtr)sizeof(Vector3*)) = &identity.M31;
				*(*(IntPtr*)ptr3) = new Vector3(matrix.M11, matrix.M12, matrix.M13);
				*(*(IntPtr*)(ptr3 + sizeof(Vector3*) / sizeof(Vector3*))) = new Vector3(matrix.M21, matrix.M22, matrix.M23);
				*(*(IntPtr*)(ptr3 + (IntPtr)2 * (IntPtr)sizeof(Vector3*) / (IntPtr)sizeof(Vector3*))) = new Vector3(matrix.M31, matrix.M32, matrix.M33);
				scale.X = ((IntPtr*)ptr3)->Length();
				scale.Y = ((IntPtr*)(ptr3 + sizeof(Vector3*) / sizeof(Vector3*)))->Length();
				scale.Z = ((IntPtr*)(ptr3 + (IntPtr)2 * (IntPtr)sizeof(Vector3*) / (IntPtr)sizeof(Vector3*)))->Length();
				float num = *ptr2;
				float num2 = ptr2[1];
				float num3 = ptr2[2];
				uint num4;
				uint num5;
				uint num6;
				if (num < num2)
				{
					if (num2 < num3)
					{
						num4 = 2U;
						num5 = 1U;
						num6 = 0U;
					}
					else
					{
						num4 = 1U;
						if (num < num3)
						{
							num5 = 2U;
							num6 = 0U;
						}
						else
						{
							num5 = 0U;
							num6 = 2U;
						}
					}
				}
				else if (num < num3)
				{
					num4 = 2U;
					num5 = 0U;
					num6 = 1U;
				}
				else
				{
					num4 = 0U;
					if (num2 < num3)
					{
						num5 = 2U;
						num6 = 1U;
					}
					else
					{
						num5 = 1U;
						num6 = 2U;
					}
				}
				if (ptr2[(ulong)num4 * 4UL / 4UL] < 0.0001f)
				{
					*(*(IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))) = ptr4[(ulong)num4 * (ulong)((long)sizeof(Vector3)) / (ulong)sizeof(Vector3)];
				}
				*(*(IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))) = Vector3.Normalize(*(*(IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))));
				if (ptr2[(ulong)num5 * 4UL / 4UL] < 0.0001f)
				{
					float num7 = MathF.Abs(((IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*)))->X);
					float num8 = MathF.Abs(((IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*)))->Y);
					float num9 = MathF.Abs(((IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*)))->Z);
					uint num10;
					if (num7 < num8)
					{
						if (num8 < num9)
						{
							num10 = 0U;
						}
						else if (num7 < num9)
						{
							num10 = 0U;
						}
						else
						{
							num10 = 2U;
						}
					}
					else if (num7 < num9)
					{
						num10 = 1U;
					}
					else if (num8 < num9)
					{
						num10 = 1U;
					}
					else
					{
						num10 = 2U;
					}
					*(*(IntPtr*)(ptr3 + (ulong)num5 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))) = Vector3.Cross(*(*(IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))), ptr4[(ulong)num10 * (ulong)((long)sizeof(Vector3)) / (ulong)sizeof(Vector3)]);
				}
				*(*(IntPtr*)(ptr3 + (ulong)num5 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))) = Vector3.Normalize(*(*(IntPtr*)(ptr3 + (ulong)num5 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))));
				if (ptr2[(ulong)num6 * 4UL / 4UL] < 0.0001f)
				{
					*(*(IntPtr*)(ptr3 + (ulong)num6 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))) = Vector3.Cross(*(*(IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))), *(*(IntPtr*)(ptr3 + (ulong)num5 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))));
				}
				*(*(IntPtr*)(ptr3 + (ulong)num6 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))) = Vector3.Normalize(*(*(IntPtr*)(ptr3 + (ulong)num6 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))));
				float num11 = identity.GetDeterminant();
				if (num11 < 0f)
				{
					ptr2[(ulong)num4 * 4UL / 4UL] = -ptr2[(ulong)num4 * 4UL / 4UL];
					*(*(IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))) = -(*(*(IntPtr*)(ptr3 + (ulong)num4 * (ulong)((long)sizeof(Vector3*)) / (ulong)sizeof(Vector3*))));
					num11 = -num11;
				}
				num11 -= 1f;
				num11 *= num11;
				if (0.0001f < num11)
				{
					rotation = Quaternion.Identity;
					flag = false;
				}
				else
				{
					rotation = Quaternion.CreateFromRotationMatrix(identity);
				}
			}
			return flag;
		}

		public static Matrix4x4 Transform(Matrix4x4 value, Quaternion rotation)
		{
			float num = rotation.X + rotation.X;
			float num2 = rotation.Y + rotation.Y;
			float num3 = rotation.Z + rotation.Z;
			float num4 = rotation.W * num;
			float num5 = rotation.W * num2;
			float num6 = rotation.W * num3;
			float num7 = rotation.X * num;
			float num8 = rotation.X * num2;
			float num9 = rotation.X * num3;
			float num10 = rotation.Y * num2;
			float num11 = rotation.Y * num3;
			float num12 = rotation.Z * num3;
			float num13 = 1f - num10 - num12;
			float num14 = num8 - num6;
			float num15 = num9 + num5;
			float num16 = num8 + num6;
			float num17 = 1f - num7 - num12;
			float num18 = num11 - num4;
			float num19 = num9 - num5;
			float num20 = num11 + num4;
			float num21 = 1f - num7 - num10;
			Matrix4x4 matrix4x;
			matrix4x.M11 = value.M11 * num13 + value.M12 * num14 + value.M13 * num15;
			matrix4x.M12 = value.M11 * num16 + value.M12 * num17 + value.M13 * num18;
			matrix4x.M13 = value.M11 * num19 + value.M12 * num20 + value.M13 * num21;
			matrix4x.M14 = value.M14;
			matrix4x.M21 = value.M21 * num13 + value.M22 * num14 + value.M23 * num15;
			matrix4x.M22 = value.M21 * num16 + value.M22 * num17 + value.M23 * num18;
			matrix4x.M23 = value.M21 * num19 + value.M22 * num20 + value.M23 * num21;
			matrix4x.M24 = value.M24;
			matrix4x.M31 = value.M31 * num13 + value.M32 * num14 + value.M33 * num15;
			matrix4x.M32 = value.M31 * num16 + value.M32 * num17 + value.M33 * num18;
			matrix4x.M33 = value.M31 * num19 + value.M32 * num20 + value.M33 * num21;
			matrix4x.M34 = value.M34;
			matrix4x.M41 = value.M41 * num13 + value.M42 * num14 + value.M43 * num15;
			matrix4x.M42 = value.M41 * num16 + value.M42 * num17 + value.M43 * num18;
			matrix4x.M43 = value.M41 * num19 + value.M42 * num20 + value.M43 * num21;
			matrix4x.M44 = value.M44;
			return matrix4x;
		}

		public static Matrix4x4 Transpose(Matrix4x4 matrix)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = matrix.M11;
			matrix4x.M12 = matrix.M21;
			matrix4x.M13 = matrix.M31;
			matrix4x.M14 = matrix.M41;
			matrix4x.M21 = matrix.M12;
			matrix4x.M22 = matrix.M22;
			matrix4x.M23 = matrix.M32;
			matrix4x.M24 = matrix.M42;
			matrix4x.M31 = matrix.M13;
			matrix4x.M32 = matrix.M23;
			matrix4x.M33 = matrix.M33;
			matrix4x.M34 = matrix.M43;
			matrix4x.M41 = matrix.M14;
			matrix4x.M42 = matrix.M24;
			matrix4x.M43 = matrix.M34;
			matrix4x.M44 = matrix.M44;
			return matrix4x;
		}

		public static Matrix4x4 Lerp(Matrix4x4 matrix1, Matrix4x4 matrix2, float amount)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;
			matrix4x.M12 = matrix1.M12 + (matrix2.M12 - matrix1.M12) * amount;
			matrix4x.M13 = matrix1.M13 + (matrix2.M13 - matrix1.M13) * amount;
			matrix4x.M14 = matrix1.M14 + (matrix2.M14 - matrix1.M14) * amount;
			matrix4x.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;
			matrix4x.M22 = matrix1.M22 + (matrix2.M22 - matrix1.M22) * amount;
			matrix4x.M23 = matrix1.M23 + (matrix2.M23 - matrix1.M23) * amount;
			matrix4x.M24 = matrix1.M24 + (matrix2.M24 - matrix1.M24) * amount;
			matrix4x.M31 = matrix1.M31 + (matrix2.M31 - matrix1.M31) * amount;
			matrix4x.M32 = matrix1.M32 + (matrix2.M32 - matrix1.M32) * amount;
			matrix4x.M33 = matrix1.M33 + (matrix2.M33 - matrix1.M33) * amount;
			matrix4x.M34 = matrix1.M34 + (matrix2.M34 - matrix1.M34) * amount;
			matrix4x.M41 = matrix1.M41 + (matrix2.M41 - matrix1.M41) * amount;
			matrix4x.M42 = matrix1.M42 + (matrix2.M42 - matrix1.M42) * amount;
			matrix4x.M43 = matrix1.M43 + (matrix2.M43 - matrix1.M43) * amount;
			matrix4x.M44 = matrix1.M44 + (matrix2.M44 - matrix1.M44) * amount;
			return matrix4x;
		}

		public static Matrix4x4 Negate(Matrix4x4 value)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = -value.M11;
			matrix4x.M12 = -value.M12;
			matrix4x.M13 = -value.M13;
			matrix4x.M14 = -value.M14;
			matrix4x.M21 = -value.M21;
			matrix4x.M22 = -value.M22;
			matrix4x.M23 = -value.M23;
			matrix4x.M24 = -value.M24;
			matrix4x.M31 = -value.M31;
			matrix4x.M32 = -value.M32;
			matrix4x.M33 = -value.M33;
			matrix4x.M34 = -value.M34;
			matrix4x.M41 = -value.M41;
			matrix4x.M42 = -value.M42;
			matrix4x.M43 = -value.M43;
			matrix4x.M44 = -value.M44;
			return matrix4x;
		}

		public static Matrix4x4 Add(Matrix4x4 value1, Matrix4x4 value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 + value2.M11;
			matrix4x.M12 = value1.M12 + value2.M12;
			matrix4x.M13 = value1.M13 + value2.M13;
			matrix4x.M14 = value1.M14 + value2.M14;
			matrix4x.M21 = value1.M21 + value2.M21;
			matrix4x.M22 = value1.M22 + value2.M22;
			matrix4x.M23 = value1.M23 + value2.M23;
			matrix4x.M24 = value1.M24 + value2.M24;
			matrix4x.M31 = value1.M31 + value2.M31;
			matrix4x.M32 = value1.M32 + value2.M32;
			matrix4x.M33 = value1.M33 + value2.M33;
			matrix4x.M34 = value1.M34 + value2.M34;
			matrix4x.M41 = value1.M41 + value2.M41;
			matrix4x.M42 = value1.M42 + value2.M42;
			matrix4x.M43 = value1.M43 + value2.M43;
			matrix4x.M44 = value1.M44 + value2.M44;
			return matrix4x;
		}

		public static Matrix4x4 Subtract(Matrix4x4 value1, Matrix4x4 value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 - value2.M11;
			matrix4x.M12 = value1.M12 - value2.M12;
			matrix4x.M13 = value1.M13 - value2.M13;
			matrix4x.M14 = value1.M14 - value2.M14;
			matrix4x.M21 = value1.M21 - value2.M21;
			matrix4x.M22 = value1.M22 - value2.M22;
			matrix4x.M23 = value1.M23 - value2.M23;
			matrix4x.M24 = value1.M24 - value2.M24;
			matrix4x.M31 = value1.M31 - value2.M31;
			matrix4x.M32 = value1.M32 - value2.M32;
			matrix4x.M33 = value1.M33 - value2.M33;
			matrix4x.M34 = value1.M34 - value2.M34;
			matrix4x.M41 = value1.M41 - value2.M41;
			matrix4x.M42 = value1.M42 - value2.M42;
			matrix4x.M43 = value1.M43 - value2.M43;
			matrix4x.M44 = value1.M44 - value2.M44;
			return matrix4x;
		}

		public static Matrix4x4 Multiply(Matrix4x4 value1, Matrix4x4 value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41;
			matrix4x.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42;
			matrix4x.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43;
			matrix4x.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44;
			matrix4x.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41;
			matrix4x.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42;
			matrix4x.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43;
			matrix4x.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44;
			matrix4x.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41;
			matrix4x.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42;
			matrix4x.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43;
			matrix4x.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44;
			matrix4x.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41;
			matrix4x.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42;
			matrix4x.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43;
			matrix4x.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44;
			return matrix4x;
		}

		public static Matrix4x4 Multiply(Matrix4x4 value1, float value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 * value2;
			matrix4x.M12 = value1.M12 * value2;
			matrix4x.M13 = value1.M13 * value2;
			matrix4x.M14 = value1.M14 * value2;
			matrix4x.M21 = value1.M21 * value2;
			matrix4x.M22 = value1.M22 * value2;
			matrix4x.M23 = value1.M23 * value2;
			matrix4x.M24 = value1.M24 * value2;
			matrix4x.M31 = value1.M31 * value2;
			matrix4x.M32 = value1.M32 * value2;
			matrix4x.M33 = value1.M33 * value2;
			matrix4x.M34 = value1.M34 * value2;
			matrix4x.M41 = value1.M41 * value2;
			matrix4x.M42 = value1.M42 * value2;
			matrix4x.M43 = value1.M43 * value2;
			matrix4x.M44 = value1.M44 * value2;
			return matrix4x;
		}

		public static Matrix4x4 operator -(Matrix4x4 value)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = -value.M11;
			matrix4x.M12 = -value.M12;
			matrix4x.M13 = -value.M13;
			matrix4x.M14 = -value.M14;
			matrix4x.M21 = -value.M21;
			matrix4x.M22 = -value.M22;
			matrix4x.M23 = -value.M23;
			matrix4x.M24 = -value.M24;
			matrix4x.M31 = -value.M31;
			matrix4x.M32 = -value.M32;
			matrix4x.M33 = -value.M33;
			matrix4x.M34 = -value.M34;
			matrix4x.M41 = -value.M41;
			matrix4x.M42 = -value.M42;
			matrix4x.M43 = -value.M43;
			matrix4x.M44 = -value.M44;
			return matrix4x;
		}

		public static Matrix4x4 operator +(Matrix4x4 value1, Matrix4x4 value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 + value2.M11;
			matrix4x.M12 = value1.M12 + value2.M12;
			matrix4x.M13 = value1.M13 + value2.M13;
			matrix4x.M14 = value1.M14 + value2.M14;
			matrix4x.M21 = value1.M21 + value2.M21;
			matrix4x.M22 = value1.M22 + value2.M22;
			matrix4x.M23 = value1.M23 + value2.M23;
			matrix4x.M24 = value1.M24 + value2.M24;
			matrix4x.M31 = value1.M31 + value2.M31;
			matrix4x.M32 = value1.M32 + value2.M32;
			matrix4x.M33 = value1.M33 + value2.M33;
			matrix4x.M34 = value1.M34 + value2.M34;
			matrix4x.M41 = value1.M41 + value2.M41;
			matrix4x.M42 = value1.M42 + value2.M42;
			matrix4x.M43 = value1.M43 + value2.M43;
			matrix4x.M44 = value1.M44 + value2.M44;
			return matrix4x;
		}

		public static Matrix4x4 operator -(Matrix4x4 value1, Matrix4x4 value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 - value2.M11;
			matrix4x.M12 = value1.M12 - value2.M12;
			matrix4x.M13 = value1.M13 - value2.M13;
			matrix4x.M14 = value1.M14 - value2.M14;
			matrix4x.M21 = value1.M21 - value2.M21;
			matrix4x.M22 = value1.M22 - value2.M22;
			matrix4x.M23 = value1.M23 - value2.M23;
			matrix4x.M24 = value1.M24 - value2.M24;
			matrix4x.M31 = value1.M31 - value2.M31;
			matrix4x.M32 = value1.M32 - value2.M32;
			matrix4x.M33 = value1.M33 - value2.M33;
			matrix4x.M34 = value1.M34 - value2.M34;
			matrix4x.M41 = value1.M41 - value2.M41;
			matrix4x.M42 = value1.M42 - value2.M42;
			matrix4x.M43 = value1.M43 - value2.M43;
			matrix4x.M44 = value1.M44 - value2.M44;
			return matrix4x;
		}

		public static Matrix4x4 operator *(Matrix4x4 value1, Matrix4x4 value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41;
			matrix4x.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42;
			matrix4x.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43;
			matrix4x.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44;
			matrix4x.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41;
			matrix4x.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42;
			matrix4x.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43;
			matrix4x.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44;
			matrix4x.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41;
			matrix4x.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42;
			matrix4x.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43;
			matrix4x.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44;
			matrix4x.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41;
			matrix4x.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42;
			matrix4x.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43;
			matrix4x.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44;
			return matrix4x;
		}

		public static Matrix4x4 operator *(Matrix4x4 value1, float value2)
		{
			Matrix4x4 matrix4x;
			matrix4x.M11 = value1.M11 * value2;
			matrix4x.M12 = value1.M12 * value2;
			matrix4x.M13 = value1.M13 * value2;
			matrix4x.M14 = value1.M14 * value2;
			matrix4x.M21 = value1.M21 * value2;
			matrix4x.M22 = value1.M22 * value2;
			matrix4x.M23 = value1.M23 * value2;
			matrix4x.M24 = value1.M24 * value2;
			matrix4x.M31 = value1.M31 * value2;
			matrix4x.M32 = value1.M32 * value2;
			matrix4x.M33 = value1.M33 * value2;
			matrix4x.M34 = value1.M34 * value2;
			matrix4x.M41 = value1.M41 * value2;
			matrix4x.M42 = value1.M42 * value2;
			matrix4x.M43 = value1.M43 * value2;
			matrix4x.M44 = value1.M44 * value2;
			return matrix4x;
		}

		public static bool operator ==(Matrix4x4 value1, Matrix4x4 value2)
		{
			return value1.M11 == value2.M11 && value1.M22 == value2.M22 && value1.M33 == value2.M33 && value1.M44 == value2.M44 && value1.M12 == value2.M12 && value1.M13 == value2.M13 && value1.M14 == value2.M14 && value1.M21 == value2.M21 && value1.M23 == value2.M23 && value1.M24 == value2.M24 && value1.M31 == value2.M31 && value1.M32 == value2.M32 && value1.M34 == value2.M34 && value1.M41 == value2.M41 && value1.M42 == value2.M42 && value1.M43 == value2.M43;
		}

		public static bool operator !=(Matrix4x4 value1, Matrix4x4 value2)
		{
			return value1.M11 != value2.M11 || value1.M12 != value2.M12 || value1.M13 != value2.M13 || value1.M14 != value2.M14 || value1.M21 != value2.M21 || value1.M22 != value2.M22 || value1.M23 != value2.M23 || value1.M24 != value2.M24 || value1.M31 != value2.M31 || value1.M32 != value2.M32 || value1.M33 != value2.M33 || value1.M34 != value2.M34 || value1.M41 != value2.M41 || value1.M42 != value2.M42 || value1.M43 != value2.M43 || value1.M44 != value2.M44;
		}

		public bool Equals(Matrix4x4 other)
		{
			return this.M11 == other.M11 && this.M22 == other.M22 && this.M33 == other.M33 && this.M44 == other.M44 && this.M12 == other.M12 && this.M13 == other.M13 && this.M14 == other.M14 && this.M21 == other.M21 && this.M23 == other.M23 && this.M24 == other.M24 && this.M31 == other.M31 && this.M32 == other.M32 && this.M34 == other.M34 && this.M41 == other.M41 && this.M42 == other.M42 && this.M43 == other.M43;
		}

		public override bool Equals(object obj)
		{
			return obj is Matrix4x4 && this.Equals((Matrix4x4)obj);
		}

		public override string ToString()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			return string.Format(currentCulture, "{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} }}", new object[]
			{
				this.M11.ToString(currentCulture),
				this.M12.ToString(currentCulture),
				this.M13.ToString(currentCulture),
				this.M14.ToString(currentCulture),
				this.M21.ToString(currentCulture),
				this.M22.ToString(currentCulture),
				this.M23.ToString(currentCulture),
				this.M24.ToString(currentCulture),
				this.M31.ToString(currentCulture),
				this.M32.ToString(currentCulture),
				this.M33.ToString(currentCulture),
				this.M34.ToString(currentCulture),
				this.M41.ToString(currentCulture),
				this.M42.ToString(currentCulture),
				this.M43.ToString(currentCulture),
				this.M44.ToString(currentCulture)
			});
		}

		public override int GetHashCode()
		{
			return this.M11.GetHashCode() + this.M12.GetHashCode() + this.M13.GetHashCode() + this.M14.GetHashCode() + this.M21.GetHashCode() + this.M22.GetHashCode() + this.M23.GetHashCode() + this.M24.GetHashCode() + this.M31.GetHashCode() + this.M32.GetHashCode() + this.M33.GetHashCode() + this.M34.GetHashCode() + this.M41.GetHashCode() + this.M42.GetHashCode() + this.M43.GetHashCode() + this.M44.GetHashCode();
		}

		public float M11;

		public float M12;

		public float M13;

		public float M14;

		public float M21;

		public float M22;

		public float M23;

		public float M24;

		public float M31;

		public float M32;

		public float M33;

		public float M34;

		public float M41;

		public float M42;

		public float M43;

		public float M44;

		private static readonly Matrix4x4 _identity = new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

		private struct CanonicalBasis
		{
			public Vector3 Row0;

			public Vector3 Row1;

			public Vector3 Row2;
		}

		private struct VectorBasis
		{
			public unsafe Vector3* Element0;

			public unsafe Vector3* Element1;

			public unsafe Vector3* Element2;
		}
	}
}
