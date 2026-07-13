using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal readonly ref struct HierarchyViewDragAndDropHandlingData
	{
		public HierarchyNode Parent { get; }

		public HierarchyNode Target { get; }

		public int InsertAtIndex { get; }

		public DragAndDropPosition DropPosition { get; }

		public HierarchyView View { get; }

		public IReadOnlyList<EntityId> EntityIds
		{
			get
			{
				return this.m_DragAndDropData.entityIds;
			}
		}

		public string[] Paths
		{
			get
			{
				return this.m_DragAndDropData.paths;
			}
		}

		public object Source
		{
			get
			{
				return this.m_DragAndDropData.source;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal EventModifiers EventModifiers { get; }

		public object GetGenericData(string key)
		{
			return this.m_DragAndDropData.GetGenericData(key);
		}

		internal HierarchyViewDragAndDropHandlingData(in HierarchyNode parent, in HierarchyNode target, int insertAtIndex, DragAndDropPosition dropPosition, DragAndDropData dragAndDropData, HierarchyView view, EventModifiers eventModifiers)
		{
			this.Parent = parent;
			this.Target = target;
			this.InsertAtIndex = insertAtIndex;
			this.DropPosition = dropPosition;
			this.m_DragAndDropData = dragAndDropData;
			this.View = view;
			this.EventModifiers = eventModifiers;
		}

		private readonly DragAndDropData m_DragAndDropData;
	}
}
