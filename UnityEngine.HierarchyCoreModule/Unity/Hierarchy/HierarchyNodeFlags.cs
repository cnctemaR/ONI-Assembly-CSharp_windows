using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[Flags]
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeFlags.h")]
	public enum HierarchyNodeFlags : uint
	{
		None = 0U,
		Expanded = 1U,
		Selected = 2U,
		Cut = 4U,
		Hidden = 8U
	}
}
