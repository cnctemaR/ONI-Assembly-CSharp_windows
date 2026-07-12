using System;

namespace System.Drawing.Drawing2D
{
	public enum PathPointType
	{
		Start,
		Line,
		Bezier = 3,
		PathTypeMask = 7,
		DashMode = 16,
		PathMarker = 32,
		CloseSubpath = 128,
		Bezier3 = 3
	}
}
