using System;

namespace System.Drawing
{
	internal struct CarbonContext : IMacContext
	{
		public CarbonContext(IntPtr port, IntPtr ctx, int width, int height)
		{
			this.port = port;
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
			MacSupport.ReleaseContext(this.port, this.ctx);
		}

		public IntPtr port;

		public IntPtr ctx;

		public int width;

		public int height;
	}
}
