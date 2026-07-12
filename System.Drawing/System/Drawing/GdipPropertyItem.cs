using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Drawing
{
	internal struct GdipPropertyItem
	{
		internal static void MarshalTo(GdipPropertyItem gdipProp, PropertyItem prop)
		{
			prop.Id = gdipProp.id;
			prop.Len = gdipProp.len;
			prop.Type = gdipProp.type;
			prop.Value = new byte[gdipProp.len];
			Marshal.Copy(gdipProp.value, prop.Value, 0, gdipProp.len);
		}

		internal int id;

		internal int len;

		internal short type;

		internal IntPtr value;
	}
}
