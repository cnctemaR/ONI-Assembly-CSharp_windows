using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyPropertyStorageType.h")]
	public enum HierarchyPropertyStorageType
	{
		Sparse,
		Dense,
		Blob,
		Default = 1
	}
}
