using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Internal
{
	[StructLayout(LayoutKind.Sequential)]
	internal class GPPOINT
	{
		internal GPPOINT()
		{
		}

		internal GPPOINT(Point pt)
		{
			this.X = pt.X;
			this.Y = pt.Y;
		}

		internal int X;

		internal int Y;
	}
}
