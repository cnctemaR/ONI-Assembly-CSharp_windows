using System;

namespace System.Drawing
{
	internal class CocoaContext : IMacContext
	{
		public CocoaContext(IntPtr focusHandle, IntPtr ctx, int width, int height)
		{
			this.focusHandle = focusHandle;
			this.ctx = ctx;
			this.width = width;
			this.height = height;
		}

		public void Synchronize()
		{
			MacSupport.CGContextSynchronize(this.ctx);
		}

		public void Release()
		{
			if (IntPtr.Zero != this.focusHandle)
			{
				MacSupport.CGContextFlush(this.ctx);
			}
			MacSupport.CGContextRestoreGState(this.ctx);
			if (IntPtr.Zero != this.focusHandle)
			{
				MacSupport.objc_msgSend(this.focusHandle, MacSupport.sel_registerName("unlockFocus"));
			}
		}

		public IntPtr focusHandle;

		public IntPtr ctx;

		public int width;

		public int height;
	}
}
