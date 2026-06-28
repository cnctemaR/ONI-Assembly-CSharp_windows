using System;
using System.ComponentModel;

namespace System.Drawing.Drawing2D
{
	[MonoTODO("libgdiplus/cairo doesn't support path gradients - unless it can be mapped to a radial gradient")]
	public sealed class PathGradientBrush : Brush
	{
		internal PathGradientBrush(IntPtr native)
			: base(native)
		{
		}

		public PathGradientBrush(GraphicsPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			Status status = GDIPlus.GdipCreatePathGradientFromPath(path.NativeObject, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public PathGradientBrush(Point[] points)
			: this(points, WrapMode.Clamp)
		{
		}

		public PathGradientBrush(PointF[] points)
			: this(points, WrapMode.Clamp)
		{
		}

		public PathGradientBrush(Point[] points, WrapMode wrapMode)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("WrapMode");
			}
			Status status = GDIPlus.GdipCreatePathGradientI(points, points.Length, wrapMode, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public PathGradientBrush(PointF[] points, WrapMode wrapMode)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("WrapMode");
			}
			Status status = GDIPlus.GdipCreatePathGradient(points, points.Length, wrapMode, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public Blend Blend
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPathGradientBlendCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				float[] array = new float[num];
				float[] array2 = new float[num];
				status = GDIPlus.GdipGetPathGradientBlend(this.nativeObject, array, array2, num);
				GDIPlus.CheckStatus(status);
				return new Blend
				{
					Factors = array,
					Positions = array2
				};
			}
			set
			{
				float[] factors = value.Factors;
				float[] positions = value.Positions;
				int num = factors.Length;
				if (num == 0 || positions.Length == 0)
				{
					throw new ArgumentException("Invalid Blend object. It should have at least 2 elements in each of the factors and positions arrays.");
				}
				if (num != positions.Length)
				{
					throw new ArgumentException("Invalid Blend object. It should contain the same number of factors and positions values.");
				}
				if (positions[0] != 0f)
				{
					throw new ArgumentException("Invalid Blend object. The positions array must have 0.0 as its first element.");
				}
				if (positions[num - 1] != 1f)
				{
					throw new ArgumentException("Invalid Blend object. The positions array must have 1.0 as its last element.");
				}
				Status status = GDIPlus.GdipSetPathGradientBlend(this.nativeObject, factors, positions, num);
				GDIPlus.CheckStatus(status);
			}
		}

		public Color CenterColor
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPathGradientCenterColor(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return Color.FromArgb(num);
			}
			set
			{
				Status status = GDIPlus.GdipSetPathGradientCenterColor(this.nativeObject, value.ToArgb());
				GDIPlus.CheckStatus(status);
			}
		}

		public PointF CenterPoint
		{
			get
			{
				PointF pointF;
				Status status = GDIPlus.GdipGetPathGradientCenterPoint(this.nativeObject, out pointF);
				GDIPlus.CheckStatus(status);
				return pointF;
			}
			set
			{
				PointF pointF = value;
				Status status = GDIPlus.GdipSetPathGradientCenterPoint(this.nativeObject, ref pointF);
				GDIPlus.CheckStatus(status);
			}
		}

		public PointF FocusScales
		{
			get
			{
				float num;
				float num2;
				Status status = GDIPlus.GdipGetPathGradientFocusScales(this.nativeObject, out num, out num2);
				GDIPlus.CheckStatus(status);
				return new PointF(num, num2);
			}
			set
			{
				Status status = GDIPlus.GdipSetPathGradientFocusScales(this.nativeObject, value.X, value.Y);
				GDIPlus.CheckStatus(status);
			}
		}

		public ColorBlend InterpolationColors
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPathGradientPresetBlendCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				if (num < 1)
				{
					num = 1;
				}
				int[] array = new int[num];
				float[] array2 = new float[num];
				if (num > 1)
				{
					status = GDIPlus.GdipGetPathGradientPresetBlend(this.nativeObject, array, array2, num);
					GDIPlus.CheckStatus(status);
				}
				ColorBlend colorBlend = new ColorBlend();
				Color[] array3 = new Color[num];
				for (int i = 0; i < num; i++)
				{
					array3[i] = Color.FromArgb(array[i]);
				}
				colorBlend.Colors = array3;
				colorBlend.Positions = array2;
				return colorBlend;
			}
			set
			{
				Color[] colors = value.Colors;
				float[] positions = value.Positions;
				int num = colors.Length;
				if (num == 0 || positions.Length == 0)
				{
					throw new ArgumentException("Invalid ColorBlend object. It should have at least 2 elements in each of the colors and positions arrays.");
				}
				if (num != positions.Length)
				{
					throw new ArgumentException("Invalid ColorBlend object. It should contain the same number of positions and color values.");
				}
				if (positions[0] != 0f)
				{
					throw new ArgumentException("Invalid ColorBlend object. The positions array must have 0.0 as its first element.");
				}
				if (positions[num - 1] != 1f)
				{
					throw new ArgumentException("Invalid ColorBlend object. The positions array must have 1.0 as its last element.");
				}
				int[] array = new int[colors.Length];
				for (int i = 0; i < colors.Length; i++)
				{
					array[i] = colors[i].ToArgb();
				}
				Status status = GDIPlus.GdipSetPathGradientPresetBlend(this.nativeObject, array, positions, num);
				GDIPlus.CheckStatus(status);
			}
		}

		public RectangleF Rectangle
		{
			get
			{
				RectangleF rectangleF;
				Status status = GDIPlus.GdipGetPathGradientRect(this.nativeObject, out rectangleF);
				GDIPlus.CheckStatus(status);
				return rectangleF;
			}
		}

		public Color[] SurroundColors
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPathGradientSurroundColorCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				int[] array = new int[num];
				status = GDIPlus.GdipGetPathGradientSurroundColorsWithCount(this.nativeObject, array, ref num);
				GDIPlus.CheckStatus(status);
				Color[] array2 = new Color[num];
				for (int i = 0; i < num; i++)
				{
					array2[i] = Color.FromArgb(array[i]);
				}
				return array2;
			}
			set
			{
				int num = value.Length;
				int[] array = new int[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = value[i].ToArgb();
				}
				Status status = GDIPlus.GdipSetPathGradientSurroundColorsWithCount(this.nativeObject, array, ref num);
				GDIPlus.CheckStatus(status);
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				Status status = GDIPlus.GdipGetPathGradientTransform(this.nativeObject, matrix.nativeMatrix);
				GDIPlus.CheckStatus(status);
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Transform");
				}
				Status status = GDIPlus.GdipSetPathGradientTransform(this.nativeObject, value.nativeMatrix);
				GDIPlus.CheckStatus(status);
			}
		}

		public WrapMode WrapMode
		{
			get
			{
				WrapMode wrapMode;
				Status status = GDIPlus.GdipGetPathGradientWrapMode(this.nativeObject, out wrapMode);
				GDIPlus.CheckStatus(status);
				return wrapMode;
			}
			set
			{
				if (value < WrapMode.Tile || value > WrapMode.Clamp)
				{
					throw new InvalidEnumArgumentException("WrapMode");
				}
				Status status = GDIPlus.GdipSetPathGradientWrapMode(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public void MultiplyTransform(Matrix matrix)
		{
			this.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}

		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			Status status = GDIPlus.GdipMultiplyPathGradientTransform(this.nativeObject, matrix.nativeMatrix, order);
			GDIPlus.CheckStatus(status);
		}

		public void ResetTransform()
		{
			Status status = GDIPlus.GdipResetPathGradientTransform(this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			Status status = GDIPlus.GdipRotatePathGradientTransform(this.nativeObject, angle, order);
			GDIPlus.CheckStatus(status);
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipScalePathGradientTransform(this.nativeObject, sx, sy, order);
			GDIPlus.CheckStatus(status);
		}

		public void SetBlendTriangularShape(float focus)
		{
			this.SetBlendTriangularShape(focus, 1f);
		}

		public void SetBlendTriangularShape(float focus, float scale)
		{
			if (focus < 0f || focus > 1f || scale < 0f || scale > 1f)
			{
				throw new ArgumentException("Invalid parameter passed.");
			}
			Status status = GDIPlus.GdipSetPathGradientLinearBlend(this.nativeObject, focus, scale);
			GDIPlus.CheckStatus(status);
		}

		public void SetSigmaBellShape(float focus)
		{
			this.SetSigmaBellShape(focus, 1f);
		}

		public void SetSigmaBellShape(float focus, float scale)
		{
			if (focus < 0f || focus > 1f || scale < 0f || scale > 1f)
			{
				throw new ArgumentException("Invalid parameter passed.");
			}
			Status status = GDIPlus.GdipSetPathGradientSigmaBlend(this.nativeObject, focus, scale);
			GDIPlus.CheckStatus(status);
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipTranslatePathGradientTransform(this.nativeObject, dx, dy, order);
			GDIPlus.CheckStatus(status);
		}

		public override object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneBrush(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new PathGradientBrush(intPtr);
		}
	}
}
