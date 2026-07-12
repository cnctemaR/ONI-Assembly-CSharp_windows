using System;
using System.ComponentModel;

namespace System.Drawing.Design
{
	public class PaintValueEventArgs : EventArgs
	{
		public PaintValueEventArgs(ITypeDescriptorContext context, object value, Graphics graphics, Rectangle bounds)
		{
			this.context = context;
			this.valueToPaint = value;
			this.graphics = graphics;
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			this.bounds = bounds;
		}

		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public ITypeDescriptorContext Context
		{
			get
			{
				return this.context;
			}
		}

		public Graphics Graphics
		{
			get
			{
				return this.graphics;
			}
		}

		public object Value
		{
			get
			{
				return this.valueToPaint;
			}
		}

		private readonly ITypeDescriptorContext context;

		private readonly object valueToPaint;

		private readonly Graphics graphics;

		private readonly Rectangle bounds;
	}
}
