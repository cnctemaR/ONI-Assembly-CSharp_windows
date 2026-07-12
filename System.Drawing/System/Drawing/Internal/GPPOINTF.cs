using System;
using System.Runtime.InteropServices;

namespace System.Drawing.Internal
{
	[StructLayout(LayoutKind.Sequential)]
	internal class GPPOINTF
	{
		internal GPPOINTF()
		{
		}

		internal GPPOINTF(PointF pt)
		{
			this.X = pt.X;
			this.Y = pt.Y;
		}

		internal PointF ToPoint()
		{
			return new PointF(this.X, this.Y);
		}

		internal float X;

		internal float Y;
	}
}
