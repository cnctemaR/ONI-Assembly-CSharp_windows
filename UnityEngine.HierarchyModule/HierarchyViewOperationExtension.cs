using System;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal static class HierarchyViewOperationExtension
	{
		public static void OnCut(this HierarchyView view)
		{
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in view.Source.EnumerateNodeTypeHandlers())
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
				bool flag = hierarchyEditorNodeTypeHandler != null && hierarchyEditorNodeTypeHandler.CanCut(view);
				if (flag)
				{
					hierarchyEditorNodeTypeHandler.OnCut(view);
				}
			}
		}

		public static void OnCopy(this HierarchyView view)
		{
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in view.Source.EnumerateNodeTypeHandlers())
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
				bool flag = hierarchyEditorNodeTypeHandler != null && hierarchyEditorNodeTypeHandler.CanCopy(view);
				if (flag)
				{
					hierarchyEditorNodeTypeHandler.OnCopy(view);
				}
			}
		}

		public static void OnPaste(this HierarchyView view)
		{
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in view.Source.EnumerateNodeTypeHandlers())
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
				bool flag = hierarchyEditorNodeTypeHandler != null && hierarchyEditorNodeTypeHandler.CanPaste(view);
				if (flag)
				{
					hierarchyEditorNodeTypeHandler.OnPaste(view);
				}
			}
		}

		public static void OnPasteAsChild(this HierarchyView view, bool keepWorldPos)
		{
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in view.Source.EnumerateNodeTypeHandlers())
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
				bool flag = hierarchyEditorNodeTypeHandler != null && hierarchyEditorNodeTypeHandler.CanPasteAsChild(view);
				if (flag)
				{
					hierarchyEditorNodeTypeHandler.OnPasteAsChild(view, keepWorldPos);
				}
			}
		}

		public static void OnDuplicate(this HierarchyView view)
		{
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in view.Source.EnumerateNodeTypeHandlers())
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
				bool flag = hierarchyEditorNodeTypeHandler != null && hierarchyEditorNodeTypeHandler.CanDuplicate(view);
				if (flag)
				{
					hierarchyEditorNodeTypeHandler.OnDuplicate(view);
				}
			}
		}

		public static void OnDelete(this HierarchyView view)
		{
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in view.Source.EnumerateNodeTypeHandlers())
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
				bool flag = hierarchyEditorNodeTypeHandler != null && hierarchyEditorNodeTypeHandler.CanDelete(view);
				if (flag)
				{
					hierarchyEditorNodeTypeHandler.OnDelete(view);
				}
			}
		}

		public static void OnSetName(this HierarchyView view, in HierarchyNode node)
		{
			view.BeginRename(in node);
		}

		public static HierarchyViewItem GetHierarchyViewItemForNode(this HierarchyView view, in HierarchyNode node)
		{
			bool flag = (in node) == HierarchyNode.Null;
			HierarchyViewItem hierarchyViewItem;
			if (flag)
			{
				hierarchyViewItem = null;
			}
			else
			{
				int num = view.ViewModel.IndexOf(in node);
				bool flag2 = num < 0;
				if (flag2)
				{
					hierarchyViewItem = null;
				}
				else
				{
					VisualElement rootElementForIndex = view.ListView.GetRootElementForIndex(num);
					bool flag3 = rootElementForIndex == null;
					if (flag3)
					{
						hierarchyViewItem = null;
					}
					else
					{
						hierarchyViewItem = rootElementForIndex.Q<HierarchyViewItem>(null, null);
					}
				}
			}
			return hierarchyViewItem;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal static bool DoesSelectedNodesHaveChildren(this HierarchyView view)
		{
			HierarchyViewModel viewModel = view.ViewModel;
			foreach (ref HierarchyNode ptr in viewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Selected))
			{
				bool flag = viewModel.GetChildrenCount(in ptr) > 0;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
