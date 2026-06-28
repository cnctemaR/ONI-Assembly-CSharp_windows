using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace System.Drawing.Design
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class PaintValueEventArgs : EventArgs
	{
		public PaintValueEventArgs(ITypeDescriptorContext context, object value, Graphics graphics, Rectangle bounds)
		{
			if (graphics == null)
			{
				throw new ArgumentNullException("graphics");
			}
			this.context = context;
			this.value = value;
			this.graphics = graphics;
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
				return this.value;
			}
		}

		private ITypeDescriptorContext context;

		private object value;

		private Graphics graphics;

		private Rectangle bounds;
	}
}
