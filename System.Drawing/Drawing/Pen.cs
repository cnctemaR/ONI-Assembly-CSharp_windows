using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace System.Drawing
{
	public sealed class Pen : MarshalByRefObject, IDisposable, ICloneable
	{
		internal Pen(IntPtr p)
		{
			this.nativeObject = p;
		}

		public Pen(Brush brush)
			: this(brush, 1f)
		{
		}

		public Pen(Color color)
			: this(color, 1f)
		{
		}

		public Pen(Brush brush, float width)
		{
			if (brush == null)
			{
				throw new ArgumentNullException("brush");
			}
			Status status = GDIPlus.GdipCreatePen2(brush.nativeObject, width, GraphicsUnit.World, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			this.color = Color.Empty;
		}

		public Pen(Color color, float width)
		{
			Status status = GDIPlus.GdipCreatePen1(color.ToArgb(), width, GraphicsUnit.World, out this.nativeObject);
			GDIPlus.CheckStatus(status);
			this.color = color;
		}

		[MonoLimitation("Libgdiplus doesn't use this property for rendering")]
		public PenAlignment Alignment
		{
			get
			{
				PenAlignment penAlignment;
				Status status = GDIPlus.GdipGetPenMode(this.nativeObject, out penAlignment);
				GDIPlus.CheckStatus(status);
				return penAlignment;
			}
			set
			{
				if (value < PenAlignment.Center || value > PenAlignment.Right)
				{
					throw new InvalidEnumArgumentException("Alignment", (int)value, typeof(PenAlignment));
				}
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenMode(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public Brush Brush
		{
			get
			{
				IntPtr intPtr;
				Status status = GDIPlus.GdipGetPenBrushFill(this.nativeObject, out intPtr);
				GDIPlus.CheckStatus(status);
				return new SolidBrush(intPtr);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Brush");
				}
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				Status status = GDIPlus.GdipSetPenBrushFill(this.nativeObject, value.nativeObject);
				GDIPlus.CheckStatus(status);
				this.color = Color.Empty;
			}
		}

		public Color Color
		{
			get
			{
				if (this.color.Equals(Color.Empty))
				{
					int num;
					Status status = GDIPlus.GdipGetPenColor(this.nativeObject, out num);
					GDIPlus.CheckStatus(status);
					this.color = Color.FromArgb(num);
				}
				return this.color;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				Status status = GDIPlus.GdipSetPenColor(this.nativeObject, value.ToArgb());
				GDIPlus.CheckStatus(status);
				this.color = value;
			}
		}

		public float[] CompoundArray
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPenCompoundCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				float[] array = new float[num];
				status = GDIPlus.GdipGetPenCompoundArray(this.nativeObject, array, num);
				GDIPlus.CheckStatus(status);
				return array;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				int num = value.Length;
				if (num < 2)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				for (int i = 0; i < value.Length; i++)
				{
					float num2 = value[i];
					if (num2 < 0f || num2 > 1f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				Status status = GDIPlus.GdipSetPenCompoundArray(this.nativeObject, value, value.Length);
				GDIPlus.CheckStatus(status);
			}
		}

		public CustomLineCap CustomEndCap
		{
			get
			{
				return this.endCap;
			}
			set
			{
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenCustomEndCap(this.nativeObject, value.nativeObject);
					GDIPlus.CheckStatus(status);
					this.endCap = value;
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public CustomLineCap CustomStartCap
		{
			get
			{
				return this.startCap;
			}
			set
			{
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenCustomStartCap(this.nativeObject, value.nativeObject);
					GDIPlus.CheckStatus(status);
					this.startCap = value;
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public DashCap DashCap
		{
			get
			{
				DashCap dashCap;
				Status status = GDIPlus.GdipGetPenDashCap197819(this.nativeObject, out dashCap);
				GDIPlus.CheckStatus(status);
				return dashCap;
			}
			set
			{
				if (value < DashCap.Flat || value > DashCap.Triangle)
				{
					throw new InvalidEnumArgumentException("DashCap", (int)value, typeof(DashCap));
				}
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenDashCap197819(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float DashOffset
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetPenDashOffset(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenDashOffset(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float[] DashPattern
		{
			get
			{
				int num;
				Status status = GDIPlus.GdipGetPenDashCount(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				float[] array;
				if (num > 0)
				{
					array = new float[num];
					status = GDIPlus.GdipGetPenDashArray(this.nativeObject, array, num);
					GDIPlus.CheckStatus(status);
				}
				else if (this.DashStyle == DashStyle.Custom)
				{
					array = new float[] { 1f };
				}
				else
				{
					array = new float[0];
				}
				return array;
			}
			set
			{
				if (!this.isModifiable)
				{
					throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				for (int i = 0; i < value.Length; i++)
				{
					float num = value[i];
					if (num <= 0f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				Status status = GDIPlus.GdipSetPenDashArray(this.nativeObject, value, value.Length);
				GDIPlus.CheckStatus(status);
			}
		}

		public DashStyle DashStyle
		{
			get
			{
				DashStyle dashStyle;
				Status status = GDIPlus.GdipGetPenDashStyle(this.nativeObject, out dashStyle);
				GDIPlus.CheckStatus(status);
				return dashStyle;
			}
			set
			{
				if (value < DashStyle.Solid || value > DashStyle.Custom)
				{
					throw new InvalidEnumArgumentException("DashStyle", (int)value, typeof(DashStyle));
				}
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenDashStyle(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public LineCap StartCap
		{
			get
			{
				LineCap lineCap;
				Status status = GDIPlus.GdipGetPenStartCap(this.nativeObject, out lineCap);
				GDIPlus.CheckStatus(status);
				return lineCap;
			}
			set
			{
				if (value < LineCap.Flat || value > LineCap.Custom)
				{
					throw new InvalidEnumArgumentException("StartCap", (int)value, typeof(LineCap));
				}
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenStartCap(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public LineCap EndCap
		{
			get
			{
				LineCap lineCap;
				Status status = GDIPlus.GdipGetPenEndCap(this.nativeObject, out lineCap);
				GDIPlus.CheckStatus(status);
				return lineCap;
			}
			set
			{
				if (value < LineCap.Flat || value > LineCap.Custom)
				{
					throw new InvalidEnumArgumentException("EndCap", (int)value, typeof(LineCap));
				}
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenEndCap(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public LineJoin LineJoin
		{
			get
			{
				LineJoin lineJoin;
				Status status = GDIPlus.GdipGetPenLineJoin(this.nativeObject, out lineJoin);
				GDIPlus.CheckStatus(status);
				return lineJoin;
			}
			set
			{
				if (value < LineJoin.Miter || value > LineJoin.MiterClipped)
				{
					throw new InvalidEnumArgumentException("LineJoin", (int)value, typeof(LineJoin));
				}
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenLineJoin(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float MiterLimit
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetPenMiterLimit(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenMiterLimit(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public PenType PenType
		{
			get
			{
				PenType penType;
				Status status = GDIPlus.GdipGetPenFillType(this.nativeObject, out penType);
				GDIPlus.CheckStatus(status);
				return penType;
			}
		}

		public Matrix Transform
		{
			get
			{
				Matrix matrix = new Matrix();
				Status status = GDIPlus.GdipGetPenTransform(this.nativeObject, matrix.nativeMatrix);
				GDIPlus.CheckStatus(status);
				return matrix;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Transform");
				}
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenTransform(this.nativeObject, value.nativeMatrix);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public float Width
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetPenWidth(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				if (this.isModifiable)
				{
					Status status = GDIPlus.GdipSetPenWidth(this.nativeObject, value);
					GDIPlus.CheckStatus(status);
					return;
				}
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
		}

		public object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipClonePen(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new Pen(intPtr)
			{
				startCap = this.startCap,
				endCap = this.endCap
			};
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.isModifiable)
			{
				throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
			}
			if (this.nativeObject != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeletePen(this.nativeObject);
				this.nativeObject = IntPtr.Zero;
				GDIPlus.CheckStatus(status);
			}
		}

		~Pen()
		{
			this.Dispose(false);
		}

		public void MultiplyTransform(Matrix matrix)
		{
			this.MultiplyTransform(matrix, MatrixOrder.Prepend);
		}

		public void MultiplyTransform(Matrix matrix, MatrixOrder order)
		{
			Status status = GDIPlus.GdipMultiplyPenTransform(this.nativeObject, matrix.nativeMatrix, order);
			GDIPlus.CheckStatus(status);
		}

		public void ResetTransform()
		{
			Status status = GDIPlus.GdipResetPenTransform(this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public void RotateTransform(float angle)
		{
			this.RotateTransform(angle, MatrixOrder.Prepend);
		}

		public void RotateTransform(float angle, MatrixOrder order)
		{
			Status status = GDIPlus.GdipRotatePenTransform(this.nativeObject, angle, order);
			GDIPlus.CheckStatus(status);
		}

		public void ScaleTransform(float sx, float sy)
		{
			this.ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}

		public void ScaleTransform(float sx, float sy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipScalePenTransform(this.nativeObject, sx, sy, order);
			GDIPlus.CheckStatus(status);
		}

		public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
		{
			if (this.isModifiable)
			{
				Status status = GDIPlus.GdipSetPenLineCap197819(this.nativeObject, startCap, endCap, dashCap);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(Locale.GetText("This Pen object can't be modified."));
		}

		public void TranslateTransform(float dx, float dy)
		{
			this.TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}

		public void TranslateTransform(float dx, float dy, MatrixOrder order)
		{
			Status status = GDIPlus.GdipTranslatePenTransform(this.nativeObject, dx, dy, order);
			GDIPlus.CheckStatus(status);
		}

		internal IntPtr nativeObject;

		internal bool isModifiable = true;

		private Color color;

		private CustomLineCap startCap;

		private CustomLineCap endCap;
	}
}
