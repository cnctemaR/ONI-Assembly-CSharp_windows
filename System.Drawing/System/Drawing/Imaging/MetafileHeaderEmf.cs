using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	internal class MetafileHeaderEmf
	{
		public MetafileType type;

		public int size;

		public int version;

		public EmfPlusFlags emfPlusFlags;

		public float dpiX;

		public float dpiY;

		public int X;

		public int Y;

		public int Width;

		public int Height;

		public SafeNativeMethods.ENHMETAHEADER EmfHeader;

		public int EmfPlusHeaderSize;

		public int LogicalDpiX;

		public int LogicalDpiY;
	}
}
