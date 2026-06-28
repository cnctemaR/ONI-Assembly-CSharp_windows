using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	public sealed class Matrix : MarshalByRefObject, IDisposable
	{
		internal Matrix(IntPtr ptr)
		{
			this.nativeMatrix = ptr;
		}

		public Matrix()
		{
			Status status = GDIPlus.GdipCreateMatrix(out this.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}

		public Matrix(Rectangle rect, Point[] plgpts)
		{
			if (plgpts == null)
			{
				throw new ArgumentNullException("plgpts");
			}
			if (plgpts.Length != 3)
			{
				throw new ArgumentException("plgpts");
			}
			Status status = GDIPlus.GdipCreateMatrix3I(ref rect, plgpts, out this.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}

		public Matrix(RectangleF rect, PointF[] plgpts)
		{
			if (plgpts == null)
			{
				throw new ArgumentNullException("plgpts");
			}
			if (plgpts.Length != 3)
			{
				throw new ArgumentException("plgpts");
			}
			Status status = GDIPlus.GdipCreateMatrix3(ref rect, plgpts, out this.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}

		public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
		{
			Status status = GDIPlus.GdipCreateMatrix2(m11, m12, m21, m22, dx, dy, out this.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}

		public float[] Elements
		{
			get
			{
				float[] array = new float[6];
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * 6);
				try
				{
					Status status = GDIPlus.GdipGetMatrixElements(this.nativeMatrix, intPtr);
					GDIPlus.CheckStatus(status);
					Marshal.Copy(intPtr, array, 0, 6);
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				return array;
			}
		}

		public bool IsIdentity
		{
			get
			{
				bool flag;
				Status status = GDIPlus.GdipIsMatrixIdentity(this.nativeMatrix, out flag);
				GDIPlus.CheckStatus(status);
				return flag;
			}
		}

		public bool IsInvertible
		{
			get
			{
				bool flag;
				Status status = GDIPlus.GdipIsMatrixInvertible(this.nativeMatrix, out flag);
				GDIPlus.CheckStatus(status);
				return flag;
			}
		}

		public float OffsetX
		{
			get
			{
				return this.Elements[4];
			}
		}

		public float OffsetY
		{
			get
			{
				return this.Elements[5];
			}
		}

		public Matrix Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneMatrix(this.nativeMatrix, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Matrix(intPtr);
		}

		public void Dispose()
		{
			if (this.nativeMatrix != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeleteMatrix(this.nativeMatrix);
				GDIPlus.CheckStatus(status);
				this.nativeMatrix = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		public override bool Equals(object obj)
		{
			Matrix matrix = obj as Matrix;
			if (matrix != null)
			{
				bool flag;
				Status status = GDIPlus.GdipIsMatrixEqual(this.nativeMatrix, matrix.nativeMatrix, out flag);
				GDIPlus.CheckStatus(status);
				return flag;
			}
			return false;
		}

		~Matrix()
		{
			this.Dispose();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Invert()
		{
			Status status = GDIPlus.GdipInvertMatrix(this.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}

		public void Multiply(Matrix matrix)
		{
			this.Multiply(matrix, MatrixOrder.Prepend);
		}

		public void Multiply(Matrix matrix, MatrixOrder order)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			Status status = GDIPlus.GdipMultiplyMatrix(this.nativeMatrix, matrix.nativeMatrix, order);
			GDIPlus.CheckStatus(status);
		}

		public void Reset()
		{
			Status status = GDIPlus.GdipSetMatrixElements(this.nativeMatrix, 1f, 0f, 0f, 1f, 0f, 0f);
			GDIPlus.CheckStatus(status);
		}

		public void Rotate(float angle)
		{
			this.Rotate(angle, MatrixOrder.Prepend);
		}

		public void Rotate(float angle, MatrixOrder order)
		{
			Status status = GDIPlus.GdipRotateMatrix(this.nativeMatrix, angle, order);
			GDIPlus.CheckStatus(status);
		}

		public void RotateAt(float angle, PointF point)
		{
			this.RotateAt(angle, point, MatrixOrder.Prepend);
		}

		public void RotateAt(float angle, PointF point, MatrixOrder order)
		{
			if (order < MatrixOrder.Prepend || order > MatrixOrder.Append)
			{
				throw new ArgumentException("order");
			}
			angle *= 0.017453292f;
			float num = (float)Math.Cos((double)angle);
			float num2 = (float)Math.Sin((double)angle);
			float num3 = -point.X * num + point.Y * num2 + point.X;
			float num4 = -point.X * num2 - point.Y * num + point.Y;
			float[] elements = this.Elements;
			Status status;
			if (order == MatrixOrder.Prepend)
			{
				status = GDIPlus.GdipSetMatrixElements(this.nativeMatrix, num * elements[0] + num2 * elements[2], num * elements[1] + num2 * elements[3], -num2 * elements[0] + num * elements[2], -num2 * elements[1] + num * elements[3], num3 * elements[0] + num4 * elements[2] + elements[4], num3 * elements[1] + num4 * elements[3] + elements[5]);
			}
			else
			{
				status = GDIPlus.GdipSetMatrixElements(this.nativeMatrix, elements[0] * num + elements[1] * -num2, elements[0] * num2 + elements[1] * num, elements[2] * num + elements[3] * -num2, elements[2] * num2 + elements[3] * num, elements[4] * num + elements[5] * -num2 + num3, elements[4] * num2 + elements[5] * num + num4);
			}
			GDIPlus.CheckStatus(status);
		}

		public void Scale(float scaleX, float scaleY)
		{
			this.Scale(scaleX, scaleY, MatrixOrder.Prepend);
		}

		public void Scale(float scaleX, float scaleY, MatrixOrder order)
		{
			Status status = GDIPlus.GdipScaleMatrix(this.nativeMatrix, scaleX, scaleY, order);
			GDIPlus.CheckStatus(status);
		}

		public void Shear(float shearX, float shearY)
		{
			this.Shear(shearX, shearY, MatrixOrder.Prepend);
		}

		public void Shear(float shearX, float shearY, MatrixOrder order)
		{
			Status status = GDIPlus.GdipShearMatrix(this.nativeMatrix, shearX, shearY, order);
			GDIPlus.CheckStatus(status);
		}

		public void TransformPoints(Point[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			Status status = GDIPlus.GdipTransformMatrixPointsI(this.nativeMatrix, pts, pts.Length);
			GDIPlus.CheckStatus(status);
		}

		public void TransformPoints(PointF[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			Status status = GDIPlus.GdipTransformMatrixPoints(this.nativeMatrix, pts, pts.Length);
			GDIPlus.CheckStatus(status);
		}

		public void TransformVectors(Point[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			Status status = GDIPlus.GdipVectorTransformMatrixPointsI(this.nativeMatrix, pts, pts.Length);
			GDIPlus.CheckStatus(status);
		}

		public void TransformVectors(PointF[] pts)
		{
			if (pts == null)
			{
				throw new ArgumentNullException("pts");
			}
			Status status = GDIPlus.GdipVectorTransformMatrixPoints(this.nativeMatrix, pts, pts.Length);
			GDIPlus.CheckStatus(status);
		}

		public void Translate(float offsetX, float offsetY)
		{
			this.Translate(offsetX, offsetY, MatrixOrder.Prepend);
		}

		public void Translate(float offsetX, float offsetY, MatrixOrder order)
		{
			Status status = GDIPlus.GdipTranslateMatrix(this.nativeMatrix, offsetX, offsetY, order);
			GDIPlus.CheckStatus(status);
		}

		public void VectorTransformPoints(Point[] pts)
		{
			this.TransformVectors(pts);
		}

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeMatrix;
			}
			set
			{
				this.nativeMatrix = value;
			}
		}

		internal IntPtr nativeMatrix;
	}
}
