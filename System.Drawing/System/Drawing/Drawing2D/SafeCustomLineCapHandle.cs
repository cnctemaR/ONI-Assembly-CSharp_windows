using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D
{
	internal class SafeCustomLineCapHandle : SafeHandle
	{
		internal SafeCustomLineCapHandle(IntPtr h)
			: base(IntPtr.Zero, true)
		{
			base.SetHandle(h);
		}

		protected override bool ReleaseHandle()
		{
			int num = 0;
			if (!this.IsInvalid)
			{
				try
				{
					num = GDIPlus.GdipDeleteCustomLineCap(new HandleRef(this, this.handle));
				}
				catch (Exception ex)
				{
					if (ClientUtils.IsSecurityOrCriticalException(ex))
					{
						throw;
					}
				}
				finally
				{
					this.handle = IntPtr.Zero;
				}
			}
			return num == 0;
		}

		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero;
			}
		}

		public static implicit operator IntPtr(SafeCustomLineCapHandle handle)
		{
			if (handle == null)
			{
				return IntPtr.Zero;
			}
			return handle.handle;
		}

		public static explicit operator SafeCustomLineCapHandle(IntPtr handle)
		{
			return new SafeCustomLineCapHandle(handle);
		}
	}
}
