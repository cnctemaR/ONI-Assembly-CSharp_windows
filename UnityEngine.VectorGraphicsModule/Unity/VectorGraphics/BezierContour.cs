using System;

namespace Unity.VectorGraphics
{
	public struct BezierContour
	{
		public BezierPathSegment[] Segments { readonly get; set; }

		public bool Closed { readonly get; set; }
	}
}
