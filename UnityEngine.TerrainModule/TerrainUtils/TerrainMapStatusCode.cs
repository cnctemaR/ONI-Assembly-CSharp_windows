using System;

namespace UnityEngine.TerrainUtils
{
	internal enum TerrainMapStatusCode
	{
		OK,
		Overlapping,
		SizeMismatch = 4,
		EdgeAlignmentMismatch = 8
	}
}
