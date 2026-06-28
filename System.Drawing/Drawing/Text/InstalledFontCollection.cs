using System;

namespace System.Drawing.Text
{
	public sealed class InstalledFontCollection : FontCollection
	{
		public InstalledFontCollection()
		{
			Status status = GDIPlus.GdipNewInstalledFontCollection(out this.nativeFontCollection);
			GDIPlus.CheckStatus(status);
		}
	}
}
