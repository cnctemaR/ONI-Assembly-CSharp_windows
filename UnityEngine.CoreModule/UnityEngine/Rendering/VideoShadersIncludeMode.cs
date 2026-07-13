using System;

namespace UnityEngine.Rendering
{
	public enum VideoShadersIncludeMode
	{
		[InspectorName("Don't include")]
		Never,
		[InspectorName("Include if referenced")]
		Referenced,
		[InspectorName("Always include")]
		Always
	}
}
