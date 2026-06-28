using System;
using System.IO;
using System.Security.Permissions;

namespace System.Drawing.Text
{
	public sealed class PrivateFontCollection : FontCollection
	{
		public PrivateFontCollection()
		{
			Status status = GDIPlus.GdipNewPrivateFontCollection(out this.nativeFontCollection);
			GDIPlus.CheckStatus(status);
		}

		public void AddFontFile(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			string fullPath = Path.GetFullPath(filename);
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException();
			}
			Status status = GDIPlus.GdipPrivateAddFontFile(this.nativeFontCollection, fullPath);
			GDIPlus.CheckStatus(status);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public void AddMemoryFont(IntPtr memory, int length)
		{
			Status status = GDIPlus.GdipPrivateAddMemoryFont(this.nativeFontCollection, memory, length);
			GDIPlus.CheckStatus(status);
		}

		protected override void Dispose(bool disposing)
		{
			if (this.nativeFontCollection != IntPtr.Zero)
			{
				GDIPlus.GdipDeletePrivateFontCollection(ref this.nativeFontCollection);
				this.nativeFontCollection = IntPtr.Zero;
			}
			base.Dispose(disposing);
		}
	}
}
