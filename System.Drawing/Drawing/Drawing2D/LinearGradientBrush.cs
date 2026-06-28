using System;
using System.ComponentModel;

namespace System.Drawing.Drawing2D
{
	public sealed class LinearGradientBrush : Brush
	{
		internal LinearGradientBrush(IntPtr native)
			: base(native)
		{
			Status status = GDIPlus.GdipGetLineRect(native, out this.rectangle);
			GDIPlus.CheckStatus(status);
		}

		public LinearGradientBrush(Point point1, Point point2, Color color1, Color color2)
		{
			Status status = GDIPlus.GdipCreateLineBrushI(ref point1, ref point2, color1.ToArgb(), color2.ToArgb(), WrapMode.Tile, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			status = GDIPlus.GdipGetLineRect(this.nativeObject, out this.rectangle);
			GDIPlus.CheckStatus(status);
		}

		public LinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
		{
			Status status = GDIPlus.GdipCreateLineBrush(ref point1, ref point2, color1.ToArgb(), color2.ToArgb(), WrapMode.Tile, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			status = GDIPlus.GdipGetLineRect(this.nativeObject, out this.rectangle);
			GDIPlus.CheckStatus(status);
		}

		public LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
		{
			Status status = GDIPlus.GdipCreateLineBrushFromRectI(ref rect, color1.ToArgb(), color2.ToArgb(), linearGradientMode, WrapMode.Tile, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			this.rectangle = rect;
		}

		public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
			: this(rect, color1, color2, angle, false)
		{
		}

		public LinearGradientBrush(RectangleF rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
		{
			Status status = GDIPlus.GdipCreateLineBrushFromRect(ref rect, color1.ToArgb(), color2.ToArgb(), linearGradientMode, WrapMode.Tile, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			this.rectangle = rect;
		}

		public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle)
			: this(rect, color1, color2, angle, false)
		{
		}

		public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle, bool isAngleScaleable)
		{
			Status status = GDIPlus.GdipCreateLineBrushFromRectWithAngleI(ref rect, color1.ToArgb(), color2.ToArgb(), angle, isAngleScaleable, WrapMode.Tile, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			this.rectangle = rect;
		}

		public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle, bool isAngleScaleable)
		{
			Status status = GDIPlus.GdipCreateLineBrushFromRectWithAngle(ref rect, color1.ToArgb(), color2.ToArgb(), angle, isAngleScaleable, WrapMode.Tile, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			this.rectangle = rect;
		}

		public Blend Blend
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetLineBlendCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				float[] array = new float[num];
				float[] array2 = new float[num];
				status = GDIPlus.GdipGetLineBlend(this.nativeObject, array, array2, num);
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
				Status status = GDIPlus.GdipSetLineBlend(this.nativeObject, factors, positions, num);
				GDIPlus.CheckStatus(status);
			}
		}

		[MonoTODO("The GammaCorrection value is ignored when using libgdiplus.")]
		public bool GammaCorrection
		{
			get
			{
				bool flag;
				Status status = GDIPlus.GdipGetLineGammaCorrection(this.nativeObject, out flag);
				GDIPlus.CheckStatus(status);
				return flag;
			}
			set
			{
				Status status = GDIPlus.GdipSetLineGammaCorrection(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public ColorBlend InterpolationColors
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetLinePresetBlendCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				int[] array = new int[num];
				float[] array2 = new float[num];
				status = GDIPlus.GdipGetLinePresetBlend(this.nativeObject, array, array2, num);
				GDIPlus.CheckStatus(status);
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
				if (value == null)
				{
					throw new ArgumentException("InterpolationColors is null");
				}
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
				Status status = GDIPlus.GdipSetLinePresetBlend(this.nativeObject, array, positions, num);
				GDIPlus.CheckStatus(status);
			}
		}

		public Color[] LinearColors
		{
			get
			{
				int[] array = new int[2];
				Status status = GDIPlus.GdipGetLineColors(this.nativeObject, array);
				GDIPlus.CheckStatus(status);
				return new Color[]
				{
					Color.FromArgb(array[0]),
					Color.FromArgb(array[1])
				};
			}
			set
			{
				Status status = GDIPlus.GdipSetLineColors(this.nativeObject, value[0].ToArgb(), value[1].ToArgb());
				GDIPlus.CheckStatus(status);
			}
		}

		public RectangleF Rectangle
		{
			get
			{
				return this.rectangle;
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				Status status = GDIPlus.GdipGetLineTransform(this.nativeObject, matrix.nativeMatrix);
				GDIPlus.CheckStatus(status);
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Transform");
				}
				Status status = GDIPlus.GdipSetLineTransform(this.nativeObject, value.nativeMatrix);
				GDIPlus.CheckStatus(status);
			}
		}

		public WrapMode WrapMode
		{
			get
			{
				WrapMode wrapMode;
				Status status = GDIPlus.GdipGetLineWrapMode(this.nativeObject, out wrapMode);
				GDIPlus.CheckStatus(status);
				return wrapMode;
			}
			set
			{
				if (value < WrapMode.Tile || value > WrapMode.Clamp)
				{
					throw new InvalidEnumArgumentException("WrapMode");
				}
				Status status = GDIPlus.GdipSetLineWrapMode(this.nativeObject, value);
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
			Status status = GDIPlus.GdipMultiplyLineTransform(this.nativeObject, matrix.nativeMatrix, order);
			GDIPlus.CheckStatus(status);
		}

		public void ResetTransform()
		{
			Status status = GDIPlus.GdipResetLineTransform(this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			Status status = GDIPlus.GdipRotateLineTransform(this.nativeObject, angle, order);
			GDIPlus.CheckStatus(status);
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipScaleLineTransform(this.nativeObject, sx, sy, order);
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
			Status status = GDIPlus.GdipSetLineLinearBlend(this.nativeObject, focus, scale);
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
			Status status = GDIPlus.GdipSetLineSigmaBlend(this.nativeObject, focus, scale);
			GDIPlus.CheckStatus(status);
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipTranslateLineTransform(this.nativeObject, dx, dy, order);
			GDIPlus.CheckStatus(status);
		}

		public override object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneBrush(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new LinearGradientBrush(intPtr);
		}

		private RectangleF rectangle;
	}
}
