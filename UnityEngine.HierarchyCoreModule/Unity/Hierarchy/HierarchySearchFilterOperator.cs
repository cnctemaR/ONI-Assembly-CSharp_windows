using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchySearch.h")]
	public enum HierarchySearchFilterOperator
	{
		Equal,
		Contains,
		Greater,
		GreaterOrEqual,
		Lesser,
		LesserOrEqual,
		NotEqual,
		Not
	}
}
