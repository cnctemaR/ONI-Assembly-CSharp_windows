using System;

namespace UnityEngine.UIElements.UIR
{
	internal enum VertexFlags
	{
		IsSolid,
		IsText,
		IsTextured,
		IsDynamic,
		IsSvgGradients,
		[Obsolete("Enum member VertexFlags.LastType has been deprecated. Use VertexFlags.IsGraphViewEdge instead.")]
		LastType = 10,
		IsGraphViewEdge = 10
	}
}
