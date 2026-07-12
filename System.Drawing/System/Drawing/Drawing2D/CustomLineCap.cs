using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	public class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
	{
		internal static CustomLineCap CreateCustomLineCapObject(IntPtr cap)
		{
			return new CustomLineCap(cap);
		}

		internal CustomLineCap()
		{
		}

		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath)
			: this(fillPath, strokePath, LineCap.Flat)
		{
		}

		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap)
			: this(fillPath, strokePath, baseCap, 0f)
		{
		}

		public CustomLineCap(GraphicsPath fillPath, GraphicsPath strokePath, LineCap baseCap, float baseInset)
		{
			IntPtr intPtr;
			int num = GDIPlus.GdipCreateCustomLineCap(new HandleRef(fillPath, (fillPath == null) ? IntPtr.Zero : fillPath.nativePath), new HandleRef(strokePath, (strokePath == null) ? IntPtr.Zero : strokePath.nativePath), baseCap, baseInset, out intPtr);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			this.SetNativeLineCap(intPtr);
		}

		internal CustomLineCap(IntPtr nativeLineCap)
		{
			this.SetNativeLineCap(nativeLineCap);
		}

		internal void SetNativeLineCap(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("handle");
			}
			this.nativeCap = new SafeCustomLineCapHandle(handle);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			if (disposing && this.nativeCap != null)
			{
				this.nativeCap.Dispose();
			}
			this._disposed = true;
		}

		~CustomLineCap()
		{
			this.Dispose(false);
		}

		public object Clone()
		{
			return this.CoreClone();
		}

		internal virtual object CoreClone()
		{
			IntPtr intPtr;
			int num = GDIPlus.GdipCloneCustomLineCap(new HandleRef(this, this.nativeCap), out intPtr);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
			return CustomLineCap.CreateCustomLineCapObject(intPtr);
		}

		public void SetStrokeCaps(LineCap startCap, LineCap endCap)
		{
			int num = GDIPlus.GdipSetCustomLineCapStrokeCaps(new HandleRef(this, this.nativeCap), startCap, endCap);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
		{
			int num = GDIPlus.GdipGetCustomLineCapStrokeCaps(new HandleRef(this, this.nativeCap), out startCap, out endCap);
			if (num != 0)
			{
				throw SafeNativeMethods.Gdip.StatusException(num);
			}
		}

		public LineJoin StrokeJoin
		{
			get
			{
				LineJoin lineJoin;
				int num = GDIPlus.GdipGetCustomLineCapStrokeJoin(new HandleRef(this, this.nativeCap), out lineJoin);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return lineJoin;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapStrokeJoin(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		public LineCap BaseCap
		{
			get
			{
				LineCap lineCap;
				int num = GDIPlus.GdipGetCustomLineCapBaseCap(new HandleRef(this, this.nativeCap), out lineCap);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return lineCap;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapBaseCap(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		public float BaseInset
		{
			get
			{
				float num2;
				int num = GDIPlus.GdipGetCustomLineCapBaseInset(new HandleRef(this, this.nativeCap), out num2);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return num2;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapBaseInset(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		public float WidthScale
		{
			get
			{
				float num2;
				int num = GDIPlus.GdipGetCustomLineCapWidthScale(new HandleRef(this, this.nativeCap), out num2);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
				return num2;
			}
			set
			{
				int num = GDIPlus.GdipSetCustomLineCapWidthScale(new HandleRef(this, this.nativeCap), value);
				if (num != 0)
				{
					throw SafeNativeMethods.Gdip.StatusException(num);
				}
			}
		}

		internal SafeCustomLineCapHandle nativeCap;

		private bool _disposed;
	}
}
