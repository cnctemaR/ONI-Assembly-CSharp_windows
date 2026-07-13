using System;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal interface IHierarchyEntityIdConverter
	{
		HierarchyNode GetNode(EntityId entityId);

		void GetNodes(ReadOnlySpan<EntityId> entityIds, Span<HierarchyNode> outNodes);

		EntityId GetEntityId(in HierarchyNode node);

		void GetEntityIds(ReadOnlySpan<HierarchyNode> nodes, Span<EntityId> outEntityIds);
	}
}
