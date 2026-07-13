using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[Flags]
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyTraversalDirection.h")]
	public enum HierarchyTraversalDirection : uint
	{
		Parents = 0U,
		Children = 1U
	}
}
