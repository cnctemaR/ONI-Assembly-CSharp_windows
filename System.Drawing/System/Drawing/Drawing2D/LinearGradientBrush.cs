using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	public sealed class LinearGradientBrush : Brush
	{
		internal LinearGradientBrush(IntPtr native)
		{
			Status status = GDIPlus.GdipGetLineRect(native, out this.rectangle);
			base.SetNativeBrush(native);
			GDIPlus.CheckStatus(status);
		}

		public LinearGradientBrush(Point point1, Point point2, Color color1, Color color2)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateLineBrushI(ref point1, ref point2, color1.ToArgb(), color2.ToArgb(), WrapMode.Tile, out intPtr));
			base.SetNativeBrush(intPtr);
			GDIPlus.CheckStatus(GDIPlus.GdipGetLineRect(intPtr, out this.rectangle));
		}

		public LinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateLineBrush(ref point1, ref point2, color1.ToArgb(), color2.ToArgb(), WrapMode.Tile, out intPtr));
			base.SetNativeBrush(intPtr);
			GDIPlus.CheckStatus(GDIPlus.GdipGetLineRect(intPtr, out this.rectangle));
		}

		public LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
		{
			if (linearGradientMode < LinearGradientMode.Horizontal || linearGradientMode > LinearGradientMode.BackwardDiagonal)
			{
				throw new InvalidEnumArgumentException("linearGradientMode", (int)linearGradientMode, typeof(LinearGradientMode));
			}
			if (rect.Width == 0 || rect.Height == 0)
			{
				throw new ArgumentException(string.Format("Rectangle '{0}' cannot have a width or height equal to 0.", rect.ToString()));
			}
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateLineBrushFromRectI(ref rect, color1.ToArgb(), color2.ToArgb(), linearGradientMode, WrapMode.Tile, out intPtr));
			base.SetNativeBrush(intPtr);
			this.rectangle = rect;
		}

		public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
			: this(rect, color1, color2, angle, false)
		{
		}

		public LinearGradientBrush(RectangleF rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
		{
			if (linearGradientMode < LinearGradientMode.Horizontal || linearGradientMode > LinearGradientMode.BackwardDiagonal)
			{
				throw new InvalidEnumArgumentException("linearGradientMode", (int)linearGradientMode, typeof(LinearGradientMode));
			}
			if ((double)rect.Width == 0.0 || (double)rect.Height == 0.0)
			{
				throw new ArgumentException(string.Format("Rectangle '{0}' cannot have a width or height equal to 0.", rect.ToString()));
			}
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateLineBrushFromRect(ref rect, color1.ToArgb(), color2.ToArgb(), linearGradientMode, WrapMode.Tile, out intPtr));
			base.SetNativeBrush(intPtr);
			this.rectangle = rect;
		}

		public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle)
			: this(rect, color1, color2, angle, false)
		{
		}

		public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle, bool isAngleScaleable)
		{
			if (rect.Width == 0 || rect.Height == 0)
			{
				throw new ArgumentException(string.Format("Rectangle '{0}' cannot have a width or height equal to 0.", rect.ToString()));
			}
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateLineBrushFromRectWithAngleI(ref rect, color1.ToArgb(), color2.ToArgb(), angle, isAngleScaleable, WrapMode.Tile, out intPtr));
			base.SetNativeBrush(intPtr);
			this.rectangle = rect;
		}

		public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle, bool isAngleScaleable)
		{
			if (rect.Width == 0f || rect.Height == 0f)
			{
				throw new ArgumentException(string.Format("Rectangle '{0}' cannot have a width or height equal to 0.", rect.ToString()));
			}
			IntPtr intPtr;
			GDIPlus.CheckStatus(GDIPlus.GdipCreateLineBrushFromRectWithAngle(ref rect, color1.ToArgb(), color2.ToArgb(), angle, isAngleScaleable, WrapMode.Tile, out intPtr));
			base.SetNativeBrush(intPtr);
			this.rectangle = rect;
		}

		public Blend Blend
		{
			get
			{
				if (this._interpolationColorsWasSet)
				{
					return null;
				}
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetLineBlendCount(base.NativeBrush, out num));
				float[] array = new float[num];
				float[] array2 = new float[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetLineBlend(base.NativeBrush, array, array2, num));
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
				GDIPlus.CheckStatus(GDIPlus.GdipSetLineBlend(base.NativeBrush, factors, positions, num));
			}
		}

		[MonoTODO("The GammaCorrection value is ignored when using libgdiplus.")]
		public bool GammaCorrection
		{
			get
			{
				bool flag;
				GDIPlus.CheckStatus(GDIPlus.GdipGetLineGammaCorrection(base.NativeBrush, out flag));
				return flag;
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetLineGammaCorrection(base.NativeBrush, value));
			}
		}

		public ColorBlend InterpolationColors
		{
			get
			{
				if (!this._interpolationColorsWasSet)
				{
					throw new ArgumentException("Property must be set to a valid ColorBlend object to use interpolation colors.");
				}
				int num;
				GDIPlus.CheckStatus(GDIPlus.GdipGetLinePresetBlendCount(base.NativeBrush, out num));
				int[] array = new int[num];
				float[] array2 = new float[num];
				GDIPlus.CheckStatus(GDIPlus.GdipGetLinePresetBlend(base.NativeBrush, array, array2, num));
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
				GDIPlus.CheckStatus(GDIPlus.GdipSetLinePresetBlend(base.NativeBrush, array, positions, num));
				this._interpolationColorsWasSet = true;
			}
		}

		public Color[] LinearColors
		{
			get
			{
				int[] array = new int[2];
				GDIPlus.CheckStatus(GDIPlus.GdipGetLineColors(base.NativeBrush, array));
				return new Color[]
				{
					Color.FromArgb(array[0]),
					Color.FromArgb(array[1])
				};
			}
			set
			{
				GDIPlus.CheckStatus(GDIPlus.GdipSetLineColors(base.NativeBrush, value[0].ToArgb(), value[1].ToArgb()));
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
				GDIPlus.CheckStatus(GDIPlus.GdipGetLineTransform(base.NativeBrush, matrix.nativeMatrix));
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Transform");
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetLineTransform(base.NativeBrush, value.nativeMatrix));
			}
		}

		public WrapMode WrapMode
		{
			get
			{
				WrapMode wrapMode;
				GDIPlus.CheckStatus(GDIPlus.GdipGetLineWrapMode(base.NativeBrush, out wrapMode));
				return wrapMode;
			}
			set
			{
				if (value < WrapMode.Tile || value > WrapMode.Clamp)
				{
					throw new InvalidEnumArgumentException("WrapMode");
				}
				GDIPlus.CheckStatus(GDIPlus.GdipSetLineWrapMode(base.NativeBrush, value));
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
			GDIPlus.CheckStatus(GDIPlus.GdipMultiplyLineTransform(base.NativeBrush, matrix.nativeMatrix, order));
		}

		public void ResetTransform()
		{
			GDIPlus.CheckStatus(GDIPlus.GdipResetLineTransform(base.NativeBrush));
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipRotateLineTransform(base.NativeBrush, angle, order));
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipScaleLineTransform(base.NativeBrush, sx, sy, order));
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
			GDIPlus.CheckStatus(GDIPlus.GdipSetLineLinearBlend(base.NativeBrush, focus, scale));
			this._interpolationColorsWasSet = false;
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
			GDIPlus.CheckStatus(GDIPlus.GdipSetLineSigmaBlend(base.NativeBrush, focus, scale));
			this._interpolationColorsWasSet = false;
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			GDIPlus.CheckStatus(GDIPlus.GdipTranslateLineTransform(base.NativeBrush, dx, dy, order));
		}

		public override object Clone()
		{
			IntPtr intPtr;
			GDIPlus.CheckStatus((Status)GDIPlus.GdipCloneBrush(new HandleRef(this, base.NativeBrush), out intPtr));
			return new LinearGradientBrush(intPtr);
		}

		private RectangleF rectangle;

		private bool _interpolationColorsWasSet;
	}
}
