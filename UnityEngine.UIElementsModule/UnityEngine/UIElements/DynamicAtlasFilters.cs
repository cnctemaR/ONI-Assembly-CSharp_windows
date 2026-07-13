using System;

namespace UnityEngine.UIElements
{
	[Flags]
	public enum DynamicAtlasFilters
	{
		None = 0,
		Readability = 1,
		Size = 2,
		Format = 4,
		ColorSpace = 8,
		FilterMode = 16
	}
}
