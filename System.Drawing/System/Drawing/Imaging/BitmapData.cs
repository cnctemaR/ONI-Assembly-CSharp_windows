using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class BitmapData
	{
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public PixelFormat PixelFormat
		{
			get
			{
				return this.pixel_format;
			}
			set
			{
				this.pixel_format = value;
			}
		}

		public int Reserved
		{
			get
			{
				return this.reserved;
			}
			set
			{
				this.reserved = value;
			}
		}

		public IntPtr Scan0
		{
			get
			{
				return this.scan0;
			}
			set
			{
				this.scan0 = value;
			}
		}

		public int Stride
		{
			get
			{
				return this.stride;
			}
			set
			{
				this.stride = value;
			}
		}

		private int width;

		private int height;

		private int stride;

		private PixelFormat pixel_format;

		private IntPtr scan0;

		private int reserved;

		private IntPtr palette;

		private int property_count;

		private IntPtr property;

		private float dpi_horz;

		private float dpi_vert;

		private int image_flags;

		private int left;

		private int top;

		private int x;

		private int y;

		private int transparent;
	}
}
