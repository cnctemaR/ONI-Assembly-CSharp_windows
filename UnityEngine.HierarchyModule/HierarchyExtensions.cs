using System;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal static class HierarchyExtensions
	{
		public static T GetNodeTypeHandler<T>(this Hierarchy hierarchy) where T : HierarchyNodeTypeHandler
		{
			return hierarchy.GetNodeTypeHandlerBase<T>();
		}

		public static HierarchyNodeTypeHandler GetNodeTypeHandler(this Hierarchy hierarchy, in HierarchyNode node)
		{
			HierarchyNodeTypeHandlerBase nodeTypeHandlerBase = hierarchy.GetNodeTypeHandlerBase(in node);
			HierarchyNodeTypeHandler hierarchyNodeTypeHandler = nodeTypeHandlerBase as HierarchyNodeTypeHandler;
			return (hierarchyNodeTypeHandler != null) ? hierarchyNodeTypeHandler : null;
		}

		public static HierarchyNodeTypeHandler GetNodeTypeHandler(this Hierarchy hierarchy, string nodeTypeName)
		{
			HierarchyNodeTypeHandlerBase nodeTypeHandlerBase = hierarchy.GetNodeTypeHandlerBase(nodeTypeName);
			HierarchyNodeTypeHandler hierarchyNodeTypeHandler = nodeTypeHandlerBase as HierarchyNodeTypeHandler;
			return (hierarchyNodeTypeHandler != null) ? hierarchyNodeTypeHandler : null;
		}

		public static HierarchyNodeTypeHandlerEnumerable EnumerateNodeTypeHandlers(this Hierarchy hierarchy)
		{
			return new HierarchyNodeTypeHandlerEnumerable(hierarchy);
		}

		public unsafe static HierarchyNode GetNode(this Hierarchy hierarchy, EntityId entityId)
		{
			bool flag = entityId == EntityId.None;
			HierarchyNode hierarchyNode;
			if (flag)
			{
				hierarchyNode = *HierarchyNode.Null;
			}
			else
			{
				foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in hierarchy.EnumerateNodeTypeHandlers())
				{
					IHierarchyEntityIdConverter hierarchyEntityIdConverter = hierarchyNodeTypeHandler as IHierarchyEntityIdConverter;
					bool flag2 = hierarchyEntityIdConverter != null;
					if (flag2)
					{
						HierarchyNode node = hierarchyEntityIdConverter.GetNode(entityId);
						bool flag3 = (in node) != HierarchyNode.Null;
						if (flag3)
						{
							return node;
						}
					}
				}
				hierarchyNode = *HierarchyNode.Null;
			}
			return hierarchyNode;
		}

		public static void GetNodes(this Hierarchy hierarchy, ReadOnlySpan<EntityId> entityIds, Span<HierarchyNode> outNodes)
		{
			bool flag = outNodes.Length != entityIds.Length;
			if (flag)
			{
				throw new ArgumentException("entityIds and outNodes must have the same length.");
			}
			outNodes.Clear();
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in hierarchy.EnumerateNodeTypeHandlers())
			{
				IHierarchyEntityIdConverter hierarchyEntityIdConverter = hierarchyNodeTypeHandler as IHierarchyEntityIdConverter;
				bool flag2 = hierarchyEntityIdConverter != null;
				if (flag2)
				{
					hierarchyEntityIdConverter.GetNodes(entityIds, outNodes);
				}
			}
		}

		public static EntityId GetEntityId(this Hierarchy hierarchy, in HierarchyNode node)
		{
			bool flag = (in node) == HierarchyNode.Null;
			EntityId entityId;
			if (flag)
			{
				entityId = EntityId.None;
			}
			else
			{
				foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in hierarchy.EnumerateNodeTypeHandlers())
				{
					IHierarchyEntityIdConverter hierarchyEntityIdConverter = hierarchyNodeTypeHandler as IHierarchyEntityIdConverter;
					bool flag2 = hierarchyEntityIdConverter != null;
					if (flag2)
					{
						EntityId entityId2 = hierarchyEntityIdConverter.GetEntityId(in node);
						bool flag3 = entityId2 != EntityId.None;
						if (flag3)
						{
							return entityId2;
						}
					}
				}
				entityId = EntityId.None;
			}
			return entityId;
		}

		public static void GetEntityIds(this Hierarchy hierarchy, ReadOnlySpan<HierarchyNode> nodes, Span<EntityId> outEntityIds)
		{
			bool flag = outEntityIds.Length != nodes.Length;
			if (flag)
			{
				throw new ArgumentException("nodes and outEntityIds must have the same length.");
			}
			outEntityIds.Clear();
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in hierarchy.EnumerateNodeTypeHandlers())
			{
				IHierarchyEntityIdConverter hierarchyEntityIdConverter = hierarchyNodeTypeHandler as IHierarchyEntityIdConverter;
				bool flag2 = hierarchyEntityIdConverter != null;
				if (flag2)
				{
					hierarchyEntityIdConverter.GetEntityIds(nodes, outEntityIds);
				}
			}
		}
	}
}
