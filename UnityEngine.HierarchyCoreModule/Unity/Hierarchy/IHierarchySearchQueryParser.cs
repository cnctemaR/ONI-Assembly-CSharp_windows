using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal interface IHierarchySearchQueryParser
	{
		HierarchySearchQueryDescriptor ParseQuery(string query);
	}
}
