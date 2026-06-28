using System;

namespace System.Drawing
{
	public abstract class Brush : MarshalByRefObject, IDisposable, ICloneable
	{
		internal Brush(IntPtr ptr)
		{
			this.nativeObject = ptr;
		}

		protected Brush()
		{
		}

		public abstract object Clone();

		internal IntPtr NativeObject
		{
			get
			{
				return this.nativeObject;
			}
			set
			{
				this.nativeObject = value;
			}
		}

		protected internal void SetNativeBrush(IntPtr brush)
		{
			this.nativeObject = brush;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.nativeObject != IntPtr.Zero)
			{
				Status status = GDIPlus.GdipDeleteBrush(this.nativeObject);
				this.nativeObject = IntPtr.Zero;
				GDIPlus.CheckStatus(status);
			}
		}

		~Brush()
		{
			this.Dispose(false);
		}

		internal IntPtr nativeObject;
	}
}
