using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
	[VisibleToOtherModules]
	internal readonly ref struct HierarchyViewDragAndDropSetupData
	{
		public ReadOnlySpan<HierarchyNode> Nodes { get; }

		public List<EntityId> EntityIds { get; }

		public List<string> Paths { get; }

		public HierarchyView View { get; }

		public void SetGenericData(string key, object value)
		{
			this.m_GenericData[key] = value;
		}

		internal HierarchyViewDragAndDropSetupData(ReadOnlySpan<HierarchyNode> nodes, List<EntityId> entityIds, List<string> paths, HierarchyView view, Dictionary<string, object> genericData)
		{
			this.Nodes = nodes;
			this.EntityIds = entityIds;
			this.Paths = paths;
			this.View = view;
			this.m_GenericData = genericData;
		}

		private readonly Dictionary<string, object> m_GenericData;
	}
}
