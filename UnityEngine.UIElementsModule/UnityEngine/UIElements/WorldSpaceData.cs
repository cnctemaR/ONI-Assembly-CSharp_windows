using System;

namespace UnityEngine.UIElements
{
	internal struct WorldSpaceData
	{
		public static readonly Bounds k_Empty3DBounds = new Bounds(Vector3.zero, Vector3.one * float.NegativeInfinity);

		public Bounds localBounds3D;

		public Bounds localBoundsPicking3D;

		public Bounds localBoundsWithoutNested3D;

		public Rect boundingBoxWithoutNested;
	}
}
