using System;
using System.Security.Permissions;

namespace System.Drawing
{
	public sealed class BufferedGraphics : IDisposable
	{
		private BufferedGraphics()
		{
		}

		internal BufferedGraphics(Graphics targetGraphics, Rectangle targetRectangle)
		{
			this.size = targetRectangle;
			this.target = targetGraphics;
			this.membmp = new Bitmap(this.size.Width, this.size.Height);
		}

		~BufferedGraphics()
		{
			this.Dispose(false);
		}

		public Graphics Graphics
		{
			get
			{
				if (this.source == null)
				{
					this.source = Graphics.FromImage(this.membmp);
				}
				return this.source;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this.membmp != null)
			{
				this.membmp.Dispose();
				this.membmp = null;
			}
			if (this.source != null)
			{
				this.source.Dispose();
				this.source = null;
			}
			this.target = null;
		}

		public void Render()
		{
			this.Render(this.target);
		}

		public void Render(Graphics target)
		{
			if (target == null)
			{
				return;
			}
			target.DrawImage(this.membmp, this.size);
		}

		[MonoTODO("The targetDC parameter has no equivalent in libgdiplus.")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public void Render(IntPtr targetDC)
		{
			throw new NotImplementedException();
		}

		private Rectangle size;

		private Bitmap membmp;

		private Graphics target;

		private Graphics source;
	}
}
