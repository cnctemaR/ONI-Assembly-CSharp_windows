using System;

namespace Unity.VectorGraphics
{
	public struct PathProperties
	{
		public Stroke Stroke { readonly get; set; }

		public PathEnding Head { readonly get; set; }

		public PathEnding Tail { readonly get; set; }

		public PathCorner Corners { readonly get; set; }
	}
}
