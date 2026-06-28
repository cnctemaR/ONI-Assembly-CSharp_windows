using System;

namespace System.Drawing
{
	internal struct XVisualInfo
	{
		internal IntPtr visual;

		internal IntPtr visualid;

		internal int screen;

		internal uint depth;

		internal int klass;

		internal IntPtr red_mask;

		internal IntPtr green_mask;

		internal IntPtr blue_mask;

		internal int colormap_size;

		internal int bits_per_rgb;
	}
}
