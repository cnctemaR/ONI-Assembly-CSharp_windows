using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	public struct Matrix2D : IEquatable<Matrix2D>
	{
		public Matrix2D(Vector2 column0, Vector2 column1, Vector2 column2)
		{
			this.m00 = column0.x;
			this.m01 = column1.x;
			this.m02 = column2.x;
			this.m10 = column0.y;
			this.m11 = column1.y;
			this.m12 = column2.y;
		}

		public float this[int row, int column]
		{
			get
			{
				return this[row + column * 2];
			}
			set
			{
				this[row + column * 2] = value;
			}
		}

		public float this[int index]
		{
			get
			{
				float num;
				switch (index)
				{
				case 0:
					num = this.m00;
					break;
				case 1:
					num = this.m10;
					break;
				case 2:
					num = this.m01;
					break;
				case 3:
					num = this.m11;
					break;
				case 4:
					num = this.m02;
					break;
				case 5:
					num = this.m12;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
				return num;
			}
			set
			{
				switch (index)
				{
				case 0:
					this.m00 = value;
					break;
				case 1:
					this.m10 = value;
					break;
				case 2:
					this.m01 = value;
					break;
				case 3:
					this.m11 = value;
					break;
				case 4:
					this.m02 = value;
					break;
				case 5:
					this.m12 = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}

		public override int GetHashCode()
		{
			return this.GetColumn(0).GetHashCode() ^ (this.GetColumn(1).GetHashCode() << 2) ^ (this.GetColumn(2).GetHashCode() >> 2);
		}

		public override bool Equals(object other)
		{
			bool flag = !(other is Matrix2D);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Matrix2D matrix2D = (Matrix2D)other;
				flag2 = this.GetColumn(0).Equals(matrix2D.GetColumn(0)) && this.GetColumn(1).Equals(matrix2D.GetColumn(1)) && this.GetColumn(2).Equals(matrix2D.GetColumn(2));
			}
			return flag2;
		}

		public static Matrix2D operator *(Matrix2D lhs, Matrix2D rhs)
		{
			Matrix2D matrix2D;
			matrix2D.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10;
			matrix2D.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11;
			matrix2D.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02;
			matrix2D.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10;
			matrix2D.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11;
			matrix2D.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12;
			return matrix2D;
		}

		public static Vector2 operator *(Matrix2D lhs, Vector2 vector)
		{
			Vector2 vector2;
			vector2.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02;
			vector2.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12;
			return vector2;
		}

		public static bool operator ==(Matrix2D lhs, Matrix2D rhs)
		{
			return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2);
		}

		public static bool operator !=(Matrix2D lhs, Matrix2D rhs)
		{
			return !(lhs == rhs);
		}

		public Vector2 GetColumn(int index)
		{
			Vector2 vector;
			switch (index)
			{
			case 0:
				vector = new Vector2(this.m00, this.m10);
				break;
			case 1:
				vector = new Vector2(this.m01, this.m11);
				break;
			case 2:
				vector = new Vector2(this.m02, this.m12);
				break;
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
			return vector;
		}

		public Vector3 GetRow(int index)
		{
			Vector3 vector;
			if (index != 0)
			{
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid row index!");
				}
				vector = new Vector3(this.m10, this.m11, this.m12);
			}
			else
			{
				vector = new Vector3(this.m00, this.m01, this.m02);
			}
			return vector;
		}

		public void SetColumn(int index, Vector2 column)
		{
			this[0, index] = column.x;
			this[1, index] = column.y;
		}

		public void SetRow(int index, Vector3 row)
		{
			this[index, 0] = row.x;
			this[index, 1] = row.y;
			this[index, 2] = row.z;
		}

		public Vector2 MultiplyPoint(Vector2 point)
		{
			Vector2 vector;
			vector.x = this.m00 * point.x + this.m01 * point.y + this.m02;
			vector.y = this.m10 * point.x + this.m11 * point.y + this.m12;
			return vector;
		}

		public Vector2 MultiplyVector(Vector2 vector)
		{
			Vector2 vector2;
			vector2.x = this.m00 * vector.x + this.m01 * vector.y;
			vector2.y = this.m10 * vector.x + this.m11 * vector.y;
			return vector2;
		}

		public Matrix2D Inverse()
		{
			Matrix2D matrix2D = default(Matrix2D);
			float num = this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
			bool flag = Mathf.Approximately(0f, num);
			Matrix2D matrix2D2;
			if (flag)
			{
				matrix2D2 = Matrix2D.zero;
			}
			else
			{
				float num2 = 1f / num;
				matrix2D[0, 0] = this[1, 1] * num2;
				matrix2D[0, 1] = -this[0, 1] * num2;
				matrix2D[1, 0] = -this[1, 0] * num2;
				matrix2D[1, 1] = this[0, 0] * num2;
				matrix2D[0, 2] = -(this[0, 2] * matrix2D[0, 0] + this[1, 2] * matrix2D[0, 1]);
				matrix2D[1, 2] = -(this[0, 2] * matrix2D[1, 0] + this[1, 2] * matrix2D[1, 1]);
				matrix2D2 = matrix2D;
			}
			return matrix2D2;
		}

		public static Matrix2D Scale(Vector2 vector)
		{
			Matrix2D matrix2D;
			matrix2D.m00 = vector.x;
			matrix2D.m01 = 0f;
			matrix2D.m02 = 0f;
			matrix2D.m10 = 0f;
			matrix2D.m11 = vector.y;
			matrix2D.m12 = 0f;
			return matrix2D;
		}

		public static Matrix2D Translate(Vector2 vector)
		{
			Matrix2D matrix2D;
			matrix2D.m00 = 1f;
			matrix2D.m01 = 0f;
			matrix2D.m02 = vector.x;
			matrix2D.m10 = 0f;
			matrix2D.m11 = 1f;
			matrix2D.m12 = vector.y;
			return matrix2D;
		}

		public static Matrix2D RotateRH(float angleRadians)
		{
			return Matrix2D.RotateLH(-angleRadians);
		}

		public static Matrix2D RotateLH(float angleRadians)
		{
			float num = Mathf.Sin(angleRadians);
			float num2 = Mathf.Cos(angleRadians);
			Matrix2D matrix2D;
			matrix2D.m00 = num2;
			matrix2D.m10 = -num;
			matrix2D.m01 = num;
			matrix2D.m11 = num2;
			matrix2D.m02 = 0f;
			matrix2D.m12 = 0f;
			return matrix2D;
		}

		public static Matrix2D SkewX(float angleRadians)
		{
			Matrix2D matrix2D;
			matrix2D.m00 = 1f;
			matrix2D.m01 = Mathf.Tan(angleRadians);
			matrix2D.m02 = 0f;
			matrix2D.m10 = 0f;
			matrix2D.m11 = 1f;
			matrix2D.m12 = 0f;
			return matrix2D;
		}

		public static Matrix2D SkewY(float angleRadians)
		{
			Matrix2D matrix2D;
			matrix2D.m00 = 1f;
			matrix2D.m01 = 0f;
			matrix2D.m02 = 0f;
			matrix2D.m10 = Mathf.Tan(angleRadians);
			matrix2D.m11 = 1f;
			matrix2D.m12 = 0f;
			return matrix2D;
		}

		public static Matrix2D zero
		{
			get
			{
				return Matrix2D.zeroMatrix;
			}
		}

		public static Matrix2D identity
		{
			get
			{
				return Matrix2D.identityMatrix;
			}
		}

		internal Matrix4x4 ToMatrix4x4()
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m00 = this.m00;
			identity.m01 = this.m01;
			identity.m03 = this.m02;
			identity.m10 = this.m10;
			identity.m11 = this.m11;
			identity.m13 = this.m12;
			return identity;
		}

		public override string ToString()
		{
			return string.Format("{0:F5}\t{1:F5}\t{2:F5}\n{3:F5}\t{4:F5}\t{5:F5}\n", new object[] { this.m00, this.m01, this.m02, this.m10, this.m11, this.m12 });
		}

		public string ToString(string format)
		{
			return string.Format("{0}\t{1}\t{2}\n{3}\t{4}\t{5}\n", new object[]
			{
				this.m00.ToString(format),
				this.m01.ToString(format),
				this.m02.ToString(format),
				this.m10.ToString(format),
				this.m11.ToString(format),
				this.m12.ToString(format)
			});
		}

		public bool Equals(Matrix2D other)
		{
			return this == other;
		}

		public float m00;

		public float m10;

		public float m01;

		public float m11;

		public float m02;

		public float m12;

		private static readonly Matrix2D zeroMatrix = new Matrix2D(new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));

		private static readonly Matrix2D identityMatrix = new Matrix2D(new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f));
	}
}
