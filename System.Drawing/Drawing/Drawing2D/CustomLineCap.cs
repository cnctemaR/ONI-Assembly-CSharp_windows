using System;

namespace System.Drawing.Drawing2D
{
	public class CustomLineCap : MarshalByRefObject, IDisposable, ICloneable
	{
		internal CustomLineCap()
		{
		}

		internal CustomLineCap(IntPtr ptr)
		{
			this.nativeObject = ptr;
		}

		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath)
			: this(fillPath, strokePath, LineCap.Flat, 0f)
		{
		}

		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap)
			: this(fillPath, strokePath, baseCap, 0f)
		{
		}

		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap, float baseInset)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			if (fillPath != null)
			{
				intPtr = fillPath.nativePath;
			}
			if (strokePath != null)
			{
				intPtr2 = strokePath.nativePath;
			}
			Status status = GDIPlus.GdipCreateCustomLineCap(intPtr, intPtr2, baseCap, baseInset, out this.nativeObject);
			GDIPlus.CheckStatus(status);
		}

		public LineCap BaseCap
		{
			get
			{
				LineCap lineCap;
				Status status = GDIPlus.GdipGetCustomLineCapBaseCap(this.nativeObject, out lineCap);
				GDIPlus.CheckStatus(status);
				return lineCap;
			}
			set
			{
				Status status = GDIPlus.GdipSetCustomLineCapBaseCap(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public LineJoin StrokeJoin
		{
			get
			{
				LineJoin lineJoin;
				Status status = GDIPlus.GdipGetCustomLineCapStrokeJoin(this.nativeObject, out lineJoin);
				GDIPlus.CheckStatus(status);
				return lineJoin;
			}
			set
			{
				Status status = GDIPlus.GdipSetCustomLineCapStrokeJoin(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public float BaseInset
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetCustomLineCapBaseInset(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				Status status = GDIPlus.GdipSetCustomLineCapBaseInset(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public float WidthScale
		{
			get
			{
				float num;
				Status status = GDIPlus.GdipGetCustomLineCapWidthScale(this.nativeObject, out num);
				GDIPlus.CheckStatus(status);
				return num;
			}
			set
			{
				Status status = GDIPlus.GdipSetCustomLineCapWidthScale(this.nativeObject, value);
				GDIPlus.CheckStatus(status);
			}
		}

		public object Clone()
		{
			IntPtr intPtr;
			Status status = GDIPlus.GdipCloneCustomLineCap(this.nativeObject, out intPtr);
			GDIPlus.CheckStatus(status);
			return new CustomLineCap(intPtr);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				Status status = GDIPlus.GdipDeleteCustomLineCap(this.nativeObject);
				GDIPlus.CheckStatus(status);
				this.disposed = true;
				this.nativeObject = IntPtr.Zero;
			}
		}

		~CustomLineCap()
		{
			this.Dispose(false);
		}

		public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
		{
			Status status = GDIPlus.GdipGetCustomLineCapStrokeCaps(this.nativeObject, out startCap, out endCap);
			GDIPlus.CheckStatus(status);
		}

		public void SetStrokeCaps(LineCap startCap, LineCap endCap)
		{
			Status status = GDIPlus.GdipSetCustomLineCapStrokeCaps(this.nativeObject, startCap, endCap);
			GDIPlus.CheckStatus(status);
		}

		private bool disposed;

		internal IntPtr nativeObject;
	}
}
