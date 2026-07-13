using System;

namespace UnityEngine.UIElements
{
	internal enum ColliderUpdateMode
	{
		[InspectorName("Match 3-D bounding box")]
		MatchBoundingBox,
		[InspectorName("Keep existing colliders (if any)")]
		Keep,
		[InspectorName("Match 2-D document rect")]
		MatchDocumentRect
	}
}
