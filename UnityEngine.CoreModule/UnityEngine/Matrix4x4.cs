using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Math/MathScripting.h")]
	[Il2CppEagerStaticClassConstruction]
	[NativeClass("Matrix4x4f")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeType(Header = "Runtime/Math/Matrix4x4.h")]
	public struct Matrix4x4 : IEquatable<Matrix4x4>, IFormattable
	{
		[ThreadSafe]
		private readonly Quaternion GetRotation()
		{
			Quaternion quaternion;
			Matrix4x4.GetRotation_Injected(ref this, out quaternion);
			return quaternion;
		}

		[ThreadSafe]
		private readonly Vector3 GetLossyScale()
		{
			Vector3 vector;
			Matrix4x4.GetLossyScale_Injected(ref this, out vector);
			return vector;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private readonly extern bool IsIdentity();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private readonly extern float GetDeterminant();

		[ThreadSafe]
		private readonly FrustumPlanes DecomposeProjection()
		{
			FrustumPlanes frustumPlanes;
			Matrix4x4.DecomposeProjection_Injected(ref this, out frustumPlanes);
			return frustumPlanes;
		}

		public readonly Quaternion rotation
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.GetRotation();
			}
		}

		public readonly Vector3 lossyScale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.GetLossyScale();
			}
		}

		public readonly bool isIdentity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.IsIdentity();
			}
		}

		public readonly float determinant
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.GetDeterminant();
			}
		}

		public readonly FrustumPlanes decomposeProjection
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.DecomposeProjection();
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public readonly extern bool ValidTRS();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Determinant(Matrix4x4 m)
		{
			return m.determinant;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Determinant(in Matrix4x4 m)
		{
			return m.determinant;
		}

		[FreeFunction("MatrixScripting::TRS", IsThreadSafe = true)]
		private static Matrix4x4 Internal_TRS(in Vector3 pos, in Quaternion q, in Vector3 s)
		{
			Matrix4x4 matrix4x;
			Matrix4x4.Internal_TRS_Injected(in pos, in q, in s, out matrix4x);
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
		{
			return Matrix4x4.Internal_TRS(in pos, in q, in s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 TRS(in Vector3 pos, in Quaternion q, in Vector3 s)
		{
			return Matrix4x4.Internal_TRS(in pos, in q, in s);
		}

		[FreeFunction("MatrixScripting::SetTRS", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetTRS(ref Matrix4x4 m, in Vector3 pos, in Quaternion q, in Vector3 s);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetTRS(Vector3 pos, Quaternion q, Vector3 s)
		{
			Matrix4x4.Internal_SetTRS(ref this, in pos, in q, in s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetTRS(in Vector3 pos, in Quaternion q, in Vector3 s)
		{
			Matrix4x4.Internal_SetTRS(ref this, in pos, in q, in s);
		}

		[FreeFunction("MatrixScripting::Inverse3DAffine", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Inverse3DAffine(in Matrix4x4 input, ref Matrix4x4 result);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Inverse3DAffine(Matrix4x4 input, ref Matrix4x4 result)
		{
			return Matrix4x4.Internal_Inverse3DAffine(in input, ref result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Inverse3DAffine(in Matrix4x4 input, ref Matrix4x4 result)
		{
			return Matrix4x4.Internal_Inverse3DAffine(in input, ref result);
		}

		[FreeFunction("MatrixScripting::Inverse", IsThreadSafe = true)]
		private static Matrix4x4 Internal_Inverse(in Matrix4x4 m)
		{
			Matrix4x4 matrix4x;
			Matrix4x4.Internal_Inverse_Injected(in m, out matrix4x);
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Inverse(Matrix4x4 m)
		{
			return Matrix4x4.Internal_Inverse(in m);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Inverse(in Matrix4x4 m)
		{
			return Matrix4x4.Internal_Inverse(in m);
		}

		public readonly Matrix4x4 inverse
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Matrix4x4.Internal_Inverse(in this);
			}
		}

		[FreeFunction("MatrixScripting::Transpose", IsThreadSafe = true)]
		private static Matrix4x4 Internal_Transpose(in Matrix4x4 m)
		{
			Matrix4x4 matrix4x;
			Matrix4x4.Internal_Transpose_Injected(in m, out matrix4x);
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Transpose(Matrix4x4 m)
		{
			return Matrix4x4.Internal_Transpose(in m);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Transpose(in Matrix4x4 m)
		{
			return Matrix4x4.Internal_Transpose(in m);
		}

		public readonly Matrix4x4 transpose
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Matrix4x4.Internal_Transpose(in this);
			}
		}

		[FreeFunction("MatrixScripting::Ortho", IsThreadSafe = true)]
		public static Matrix4x4 Ortho(float left, float right, float bottom, float top, float zNear, float zFar)
		{
			Matrix4x4 matrix4x;
			Matrix4x4.Ortho_Injected(left, right, bottom, top, zNear, zFar, out matrix4x);
			return matrix4x;
		}

		[FreeFunction("MatrixScripting::Perspective", IsThreadSafe = true)]
		public static Matrix4x4 Perspective(float fov, float aspect, float zNear, float zFar)
		{
			Matrix4x4 matrix4x;
			Matrix4x4.Perspective_Injected(fov, aspect, zNear, zFar, out matrix4x);
			return matrix4x;
		}

		[FreeFunction("MatrixScripting::LookAt", IsThreadSafe = true)]
		private static Matrix4x4 Internal_LookAt(in Vector3 from, in Vector3 to, in Vector3 up)
		{
			Matrix4x4 matrix4x;
			Matrix4x4.Internal_LookAt_Injected(in from, in to, in up, out matrix4x);
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 LookAt(Vector3 from, Vector3 to, Vector3 up)
		{
			return Matrix4x4.Internal_LookAt(in from, in to, in up);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 LookAt(in Vector3 from, in Vector3 to, in Vector3 up)
		{
			return Matrix4x4.Internal_LookAt(in from, in to, in up);
		}

		[FreeFunction("MatrixScripting::Frustum", IsThreadSafe = true)]
		public static Matrix4x4 Frustum(float left, float right, float bottom, float top, float zNear, float zFar)
		{
			Matrix4x4 matrix4x;
			Matrix4x4.Frustum_Injected(left, right, bottom, top, zNear, zFar, out matrix4x);
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Frustum(FrustumPlanes fp)
		{
			return Matrix4x4.Frustum(fp.left, fp.right, fp.bottom, fp.top, fp.zNear, fp.zFar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Frustum(in FrustumPlanes fp)
		{
			return Matrix4x4.Frustum(fp.left, fp.right, fp.bottom, fp.top, fp.zNear, fp.zFar);
		}

		[FreeFunction("MatrixScripting::Internal_CompareApproximately", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CompareApproximately(in Matrix4x4 a, in Matrix4x4 b, float threshold);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool CompareApproximately(Matrix4x4 a, Matrix4x4 b, float threshold)
		{
			return Matrix4x4.Internal_CompareApproximately(in a, in b, threshold);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool CompareApproximately(in Matrix4x4 a, in Matrix4x4 b, float threshold)
		{
			return Matrix4x4.Internal_CompareApproximately(in a, in b, threshold);
		}

		public Matrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
		{
			this.m00 = column0.x;
			this.m01 = column1.x;
			this.m02 = column2.x;
			this.m03 = column3.x;
			this.m10 = column0.y;
			this.m11 = column1.y;
			this.m12 = column2.y;
			this.m13 = column3.y;
			this.m20 = column0.z;
			this.m21 = column1.z;
			this.m22 = column2.z;
			this.m23 = column3.z;
			this.m30 = column0.w;
			this.m31 = column1.w;
			this.m32 = column2.w;
			this.m33 = column3.w;
		}

		public Matrix4x4(in Vector4 column0, in Vector4 column1, in Vector4 column2, in Vector4 column3)
		{
			this.m00 = column0.x;
			this.m01 = column1.x;
			this.m02 = column2.x;
			this.m03 = column3.x;
			this.m10 = column0.y;
			this.m11 = column1.y;
			this.m12 = column2.y;
			this.m13 = column3.y;
			this.m20 = column0.z;
			this.m21 = column1.z;
			this.m22 = column2.z;
			this.m23 = column3.z;
			this.m30 = column0.w;
			this.m31 = column1.w;
			this.m32 = column2.w;
			this.m33 = column3.w;
		}

		public float this[int row, int column]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
			{
				return this[row + column * 4];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this[row + column * 4] = value;
			}
		}

		public float this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get
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
					num = this.m20;
					break;
				case 3:
					num = this.m30;
					break;
				case 4:
					num = this.m01;
					break;
				case 5:
					num = this.m11;
					break;
				case 6:
					num = this.m21;
					break;
				case 7:
					num = this.m31;
					break;
				case 8:
					num = this.m02;
					break;
				case 9:
					num = this.m12;
					break;
				case 10:
					num = this.m22;
					break;
				case 11:
					num = this.m32;
					break;
				case 12:
					num = this.m03;
					break;
				case 13:
					num = this.m13;
					break;
				case 14:
					num = this.m23;
					break;
				case 15:
					num = this.m33;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
				return num;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
					this.m20 = value;
					break;
				case 3:
					this.m30 = value;
					break;
				case 4:
					this.m01 = value;
					break;
				case 5:
					this.m11 = value;
					break;
				case 6:
					this.m21 = value;
					break;
				case 7:
					this.m31 = value;
					break;
				case 8:
					this.m02 = value;
					break;
				case 9:
					this.m12 = value;
					break;
				case 10:
					this.m22 = value;
					break;
				case 11:
					this.m32 = value;
					break;
				case 12:
					this.m03 = value;
					break;
				case 13:
					this.m13 = value;
					break;
				case 14:
					this.m23 = value;
					break;
				case 15:
					this.m33 = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return this.GetColumn(0).GetHashCode() ^ (this.GetColumn(1).GetHashCode() << 2) ^ (this.GetColumn(2).GetHashCode() >> 2) ^ (this.GetColumn(3).GetHashCode() >> 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals(object other)
		{
			Matrix4x4 matrix4x;
			bool flag;
			if (other is Matrix4x4)
			{
				matrix4x = (Matrix4x4)other;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(in matrix4x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Matrix4x4 other)
		{
			return this.GetColumn(0).Equals(other.GetColumn(0)) && this.GetColumn(1).Equals(other.GetColumn(1)) && this.GetColumn(2).Equals(other.GetColumn(2)) && this.GetColumn(3).Equals(other.GetColumn(3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Matrix4x4 other)
		{
			return this.GetColumn(0).Equals(other.GetColumn(0)) && this.GetColumn(1).Equals(other.GetColumn(1)) && this.GetColumn(2).Equals(other.GetColumn(2)) && this.GetColumn(3).Equals(other.GetColumn(3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return new Matrix4x4
			{
				m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30,
				m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31,
				m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32,
				m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33,
				m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30,
				m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31,
				m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32,
				m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33,
				m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30,
				m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31,
				m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32,
				m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33,
				m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30,
				m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31,
				m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32,
				m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(Matrix4x4 lhs, Vector4 vector)
		{
			return new Vector4
			{
				x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w,
				y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w,
				z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w,
				w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);
		}

		public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			return !(lhs == rhs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector4 GetColumn(int index)
		{
			Vector4 vector;
			switch (index)
			{
			case 0:
				vector = new Vector4
				{
					x = this.m00,
					y = this.m10,
					z = this.m20,
					w = this.m30
				};
				break;
			case 1:
				vector = new Vector4
				{
					x = this.m01,
					y = this.m11,
					z = this.m21,
					w = this.m31
				};
				break;
			case 2:
				vector = new Vector4
				{
					x = this.m02,
					y = this.m12,
					z = this.m22,
					w = this.m32
				};
				break;
			case 3:
				vector = new Vector4
				{
					x = this.m03,
					y = this.m13,
					z = this.m23,
					w = this.m33
				};
				break;
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector4 GetRow(int index)
		{
			Vector4 vector;
			switch (index)
			{
			case 0:
				vector = new Vector4
				{
					x = this.m00,
					y = this.m01,
					z = this.m02,
					w = this.m03
				};
				break;
			case 1:
				vector = new Vector4
				{
					x = this.m10,
					y = this.m11,
					z = this.m12,
					w = this.m13
				};
				break;
			case 2:
				vector = new Vector4
				{
					x = this.m20,
					y = this.m21,
					z = this.m22,
					w = this.m23
				};
				break;
			case 3:
				vector = new Vector4
				{
					x = this.m30,
					y = this.m31,
					z = this.m32,
					w = this.m33
				};
				break;
			default:
				throw new IndexOutOfRangeException("Invalid row index!");
			}
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 GetPosition()
		{
			return new Vector3
			{
				x = this.m03,
				y = this.m13,
				z = this.m23
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetColumn(int index, Vector4 column)
		{
			this[0, index] = column.x;
			this[1, index] = column.y;
			this[2, index] = column.z;
			this[3, index] = column.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetColumn(int index, in Vector4 column)
		{
			this[0, index] = column.x;
			this[1, index] = column.y;
			this[2, index] = column.z;
			this[3, index] = column.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetRow(int index, Vector4 row)
		{
			this[index, 0] = row.x;
			this[index, 1] = row.y;
			this[index, 2] = row.z;
			this[index, 3] = row.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetRow(int index, in Vector4 row)
		{
			this[index, 0] = row.x;
			this[index, 1] = row.y;
			this[index, 2] = row.z;
			this[index, 3] = row.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 MultiplyPoint(Vector3 point)
		{
			Vector3 vector;
			vector.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
			vector.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
			vector.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
			float num = this.m30 * point.x + this.m31 * point.y + this.m32 * point.z + this.m33;
			num = 1f / num;
			vector.x *= num;
			vector.y *= num;
			vector.z *= num;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 MultiplyPoint(in Vector3 point)
		{
			Vector3 vector;
			vector.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
			vector.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
			vector.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
			float num = this.m30 * point.x + this.m31 * point.y + this.m32 * point.z + this.m33;
			num = 1f / num;
			vector.x *= num;
			vector.y *= num;
			vector.z *= num;
			return vector;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 MultiplyPoint3x4(Vector3 point)
		{
			return new Vector3
			{
				x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03,
				y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13,
				z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 MultiplyPoint3x4(in Vector3 point)
		{
			return new Vector3
			{
				x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03,
				y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13,
				z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 MultiplyVector(Vector3 vector)
		{
			return new Vector3
			{
				x = this.m00 * vector.x + this.m01 * vector.y + this.m02 * vector.z,
				y = this.m10 * vector.x + this.m11 * vector.y + this.m12 * vector.z,
				z = this.m20 * vector.x + this.m21 * vector.y + this.m22 * vector.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector3 MultiplyVector(in Vector3 vector)
		{
			return new Vector3
			{
				x = this.m00 * vector.x + this.m01 * vector.y + this.m02 * vector.z,
				y = this.m10 * vector.x + this.m11 * vector.y + this.m12 * vector.z,
				z = this.m20 * vector.x + this.m21 * vector.y + this.m22 * vector.z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Plane TransformPlane(Plane plane)
		{
			Matrix4x4 inverse = this.inverse;
			Vector3 normal = plane.normal;
			float x = normal.x;
			float y = normal.y;
			float z = normal.z;
			float distance = plane.distance;
			float num = inverse.m00 * x + inverse.m10 * y + inverse.m20 * z + inverse.m30 * distance;
			float num2 = inverse.m01 * x + inverse.m11 * y + inverse.m21 * z + inverse.m31 * distance;
			float num3 = inverse.m02 * x + inverse.m12 * y + inverse.m22 * z + inverse.m32 * distance;
			float num4 = inverse.m03 * x + inverse.m13 * y + inverse.m23 * z + inverse.m33 * distance;
			Vector3 vector = new Vector3
			{
				x = num,
				y = num2,
				z = num3
			};
			return new Plane(in vector, num4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Plane TransformPlane(in Plane plane)
		{
			Matrix4x4 inverse = this.inverse;
			Vector3 normal = plane.normal;
			float x = normal.x;
			float y = normal.y;
			float z = normal.z;
			float distance = plane.distance;
			float num = inverse.m00 * x + inverse.m10 * y + inverse.m20 * z + inverse.m30 * distance;
			float num2 = inverse.m01 * x + inverse.m11 * y + inverse.m21 * z + inverse.m31 * distance;
			float num3 = inverse.m02 * x + inverse.m12 * y + inverse.m22 * z + inverse.m32 * distance;
			float num4 = inverse.m03 * x + inverse.m13 * y + inverse.m23 * z + inverse.m33 * distance;
			Vector3 vector = new Vector3
			{
				x = num,
				y = num2,
				z = num3
			};
			return new Plane(in vector, num4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Scale(Vector3 vector)
		{
			return new Matrix4x4
			{
				m00 = vector.x,
				m01 = 0f,
				m02 = 0f,
				m03 = 0f,
				m10 = 0f,
				m11 = vector.y,
				m12 = 0f,
				m13 = 0f,
				m20 = 0f,
				m21 = 0f,
				m22 = vector.z,
				m23 = 0f,
				m30 = 0f,
				m31 = 0f,
				m32 = 0f,
				m33 = 1f
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Scale(in Vector3 vector)
		{
			return new Matrix4x4
			{
				m00 = vector.x,
				m01 = 0f,
				m02 = 0f,
				m03 = 0f,
				m10 = 0f,
				m11 = vector.y,
				m12 = 0f,
				m13 = 0f,
				m20 = 0f,
				m21 = 0f,
				m22 = vector.z,
				m23 = 0f,
				m30 = 0f,
				m31 = 0f,
				m32 = 0f,
				m33 = 1f
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Translate(Vector3 vector)
		{
			return new Matrix4x4
			{
				m00 = 1f,
				m01 = 0f,
				m02 = 0f,
				m03 = vector.x,
				m10 = 0f,
				m11 = 1f,
				m12 = 0f,
				m13 = vector.y,
				m20 = 0f,
				m21 = 0f,
				m22 = 1f,
				m23 = vector.z,
				m30 = 0f,
				m31 = 0f,
				m32 = 0f,
				m33 = 1f
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Translate(in Vector3 vector)
		{
			return new Matrix4x4
			{
				m00 = 1f,
				m01 = 0f,
				m02 = 0f,
				m03 = vector.x,
				m10 = 0f,
				m11 = 1f,
				m12 = 0f,
				m13 = vector.y,
				m20 = 0f,
				m21 = 0f,
				m22 = 1f,
				m23 = vector.z,
				m30 = 0f,
				m31 = 0f,
				m32 = 0f,
				m33 = 1f
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Rotate(Quaternion q)
		{
			float num = q.x * 2f;
			float num2 = q.y * 2f;
			float num3 = q.z * 2f;
			float num4 = q.x * num;
			float num5 = q.y * num2;
			float num6 = q.z * num3;
			float num7 = q.x * num2;
			float num8 = q.x * num3;
			float num9 = q.y * num3;
			float num10 = q.w * num;
			float num11 = q.w * num2;
			float num12 = q.w * num3;
			Matrix4x4 matrix4x;
			matrix4x.m00 = 1f - (num5 + num6);
			matrix4x.m10 = num7 + num12;
			matrix4x.m20 = num8 - num11;
			matrix4x.m30 = 0f;
			matrix4x.m01 = num7 - num12;
			matrix4x.m11 = 1f - (num4 + num6);
			matrix4x.m21 = num9 + num10;
			matrix4x.m31 = 0f;
			matrix4x.m02 = num8 + num11;
			matrix4x.m12 = num9 - num10;
			matrix4x.m22 = 1f - (num4 + num5);
			matrix4x.m32 = 0f;
			matrix4x.m03 = 0f;
			matrix4x.m13 = 0f;
			matrix4x.m23 = 0f;
			matrix4x.m33 = 1f;
			return matrix4x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Rotate(in Quaternion q)
		{
			float num = q.x * 2f;
			float num2 = q.y * 2f;
			float num3 = q.z * 2f;
			float num4 = q.x * num;
			float num5 = q.y * num2;
			float num6 = q.z * num3;
			float num7 = q.x * num2;
			float num8 = q.x * num3;
			float num9 = q.y * num3;
			float num10 = q.w * num;
			float num11 = q.w * num2;
			float num12 = q.w * num3;
			Matrix4x4 matrix4x;
			matrix4x.m00 = 1f - (num5 + num6);
			matrix4x.m10 = num7 + num12;
			matrix4x.m20 = num8 - num11;
			matrix4x.m30 = 0f;
			matrix4x.m01 = num7 - num12;
			matrix4x.m11 = 1f - (num4 + num6);
			matrix4x.m21 = num9 + num10;
			matrix4x.m31 = 0f;
			matrix4x.m02 = num8 + num11;
			matrix4x.m12 = num9 - num10;
			matrix4x.m22 = 1f - (num4 + num5);
			matrix4x.m32 = 0f;
			matrix4x.m03 = 0f;
			matrix4x.m13 = 0f;
			matrix4x.m23 = 0f;
			matrix4x.m33 = 1f;
			return matrix4x;
		}

		public static Matrix4x4 zero
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Matrix4x4.zeroMatrix;
			}
		}

		public static Matrix4x4 identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Matrix4x4.identityMatrix;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly string ToString()
		{
			return this.ToString(null, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string format)
		{
			return this.ToString(format, null);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = string.IsNullOrEmpty(format);
			if (flag)
			{
				format = "F5";
			}
			bool flag2 = formatProvider == null;
			if (flag2)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return string.Format("{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n", new object[]
			{
				this.m00.ToString(format, formatProvider),
				this.m01.ToString(format, formatProvider),
				this.m02.ToString(format, formatProvider),
				this.m03.ToString(format, formatProvider),
				this.m10.ToString(format, formatProvider),
				this.m11.ToString(format, formatProvider),
				this.m12.ToString(format, formatProvider),
				this.m13.ToString(format, formatProvider),
				this.m20.ToString(format, formatProvider),
				this.m21.ToString(format, formatProvider),
				this.m22.ToString(format, formatProvider),
				this.m23.ToString(format, formatProvider),
				this.m30.ToString(format, formatProvider),
				this.m31.ToString(format, formatProvider),
				this.m32.ToString(format, formatProvider),
				this.m33.ToString(format, formatProvider)
			});
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRotation_Injected(ref Matrix4x4 _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLossyScale_Injected(ref Matrix4x4 _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DecomposeProjection_Injected(ref Matrix4x4 _unity_self, out FrustumPlanes ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_TRS_Injected(in Vector3 pos, in Quaternion q, in Vector3 s, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Inverse_Injected(in Matrix4x4 m, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Transpose_Injected(in Matrix4x4 m, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Ortho_Injected(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Perspective_Injected(float fov, float aspect, float zNear, float zFar, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_LookAt_Injected(in Vector3 from, in Vector3 to, in Vector3 up, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Frustum_Injected(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4x4 ret);

		[NativeName("m_Data[0]")]
		public float m00;

		[NativeName("m_Data[1]")]
		public float m10;

		[NativeName("m_Data[2]")]
		public float m20;

		[NativeName("m_Data[3]")]
		public float m30;

		[NativeName("m_Data[4]")]
		public float m01;

		[NativeName("m_Data[5]")]
		public float m11;

		[NativeName("m_Data[6]")]
		public float m21;

		[NativeName("m_Data[7]")]
		public float m31;

		[NativeName("m_Data[8]")]
		public float m02;

		[NativeName("m_Data[9]")]
		public float m12;

		[NativeName("m_Data[10]")]
		public float m22;

		[NativeName("m_Data[11]")]
		public float m32;

		[NativeName("m_Data[12]")]
		public float m03;

		[NativeName("m_Data[13]")]
		public float m13;

		[NativeName("m_Data[14]")]
		public float m23;

		[NativeName("m_Data[15]")]
		public float m33;

		private static readonly Matrix4x4 zeroMatrix = new Matrix4x4(new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f));

		private static readonly Matrix4x4 identityMatrix = new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f));
	}
}
