using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;

namespace UnityEngine.UIElements
{
	public class DefaultTreeViewController<T> : TreeViewController, IDefaultTreeViewController<T>
	{
		private TreeDataController<T> treeDataController
		{
			get
			{
				TreeDataController<T> treeDataController;
				if ((treeDataController = this.m_TreeDataController) == null)
				{
					treeDataController = (this.m_TreeDataController = new TreeDataController<T>());
				}
				return treeDataController;
			}
		}

		public override IList itemsSource
		{
			get
			{
				return base.itemsSource;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.SetRootItems(null);
				}
				else
				{
					IList<TreeViewItemData<T>> list = value as IList<TreeViewItemData<T>>;
					bool flag2 = list != null;
					if (flag2)
					{
						this.SetRootItems(list);
					}
					else
					{
						Debug.LogError(string.Format("Type does not match this tree view controller's data type ({0}).", typeof(T)));
					}
				}
			}
		}

		public void SetRootItems(IList<TreeViewItemData<T>> items)
		{
			bool flag = items == base.itemsSource;
			if (!flag)
			{
				bool isCreated = this.m_Hierarchy.IsCreated;
				if (isCreated)
				{
					base.ClearIdToNodeDictionary();
					this.treeDataController.ClearNodeToDataDictionary();
					base.hierarchy = new Hierarchy();
				}
				bool flag2 = items != null;
				if (flag2)
				{
					this.treeDataController.ConvertTreeViewItemDataToHierarchy(items, (HierarchyNode node) => base.CreateNode(in node), delegate(int id, HierarchyNode node)
					{
						base.UpdateIdToNodeDictionary(id, in node, true);
					});
					base.UpdateHierarchy();
					bool autoExpand = base.baseTreeView.autoExpand;
					if (autoExpand)
					{
						this.m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Expanded);
						base.UpdateHierarchy();
					}
					bool flag3 = base.IsViewDataKeyEnabled();
					if (flag3)
					{
						base.OnViewDataReadyUpdateNodes();
					}
				}
				base.SetHierarchyViewModelWithoutNotify(this.m_HierarchyViewModel);
				base.RaiseItemsSourceChanged();
			}
		}

		public virtual void AddItem(in TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree = true)
		{
			bool flag = parentId == BaseTreeView.invalidId;
			HierarchyNode hierarchyNode;
			if (flag)
			{
				hierarchyNode = base.CreateNode(HierarchyNode.Null);
			}
			else
			{
				HierarchyNode hierarchyNodeById = base.GetHierarchyNodeById(parentId);
				hierarchyNode = base.CreateNode(in hierarchyNodeById);
				TreeViewItemData<T> treeItemDataForNode = this.treeDataController.GetTreeItemDataForNode(hierarchyNodeById);
				bool flag2 = treeItemDataForNode.data != null;
				if (flag2)
				{
					treeItemDataForNode.InsertChild(item, childIndex);
				}
			}
			this.treeDataController.AddItem(in item, hierarchyNode);
			base.UpdateIdToNodeDictionary(item.id, in hierarchyNode, true);
			bool flag3 = item.children.GetCount() > 0;
			if (flag3)
			{
				HierarchyNode parentNode = base.GetHierarchyNodeById(item.id);
				this.treeDataController.ConvertTreeViewItemDataToHierarchy(item.children, delegate(HierarchyNode itemNode)
				{
					BaseTreeViewController <>4__this = this;
					HierarchyNode hierarchyNode2 = (((in itemNode) == HierarchyNode.Null) ? parentNode : itemNode);
					return <>4__this.CreateNode(in hierarchyNode2);
				}, delegate(int id, HierarchyNode newNode)
				{
					base.UpdateIdToNodeDictionary(id, in newNode, true);
				});
			}
			bool autoExpand = base.baseTreeView.autoExpand;
			if (autoExpand)
			{
				base.ExpandAncestorNodes(in hierarchyNode);
			}
			bool flag4 = childIndex != -1;
			if (flag4)
			{
				HierarchyNode parent = this.m_Hierarchy.GetParent(in hierarchyNode);
				base.UpdateSortOrder(in parent, in hierarchyNode, childIndex);
			}
			if (rebuildTree)
			{
				base.baseTreeView.RefreshItems();
			}
		}

		public virtual TreeViewItemData<T> GetTreeViewItemDataForId(int id)
		{
			return this.treeDataController.GetTreeItemDataForNode(base.GetHierarchyNodeById(id));
		}

		public virtual TreeViewItemData<T> GetTreeViewItemDataForIndex(int index)
		{
			int idForIndex = this.GetIdForIndex(index);
			return this.treeDataController.GetTreeItemDataForNode(base.GetHierarchyNodeById(idForIndex));
		}

		public override bool TryRemoveItem(int id, bool rebuildTree = true)
		{
			HierarchyNode node = base.GetHierarchyNodeById(id);
			bool flag = (in node) != HierarchyNode.Null;
			bool flag4;
			if (flag)
			{
				int parentId = this.GetParentId(id);
				bool flag2 = parentId != BaseTreeView.invalidId;
				if (flag2)
				{
					TreeViewItemData<T> treeItemDataForNode = this.treeDataController.GetTreeItemDataForNode(base.GetHierarchyNodeById(parentId));
					bool flag3 = treeItemDataForNode.data != null;
					if (flag3)
					{
						treeItemDataForNode.RemoveChild(id);
					}
				}
				base.RemoveAllChildrenItemsFromCollections(in node, delegate(HierarchyNode hierarchyNode, int itemId)
				{
					this.treeDataController.RemoveItem(hierarchyNode);
					this.UpdateIdToNodeDictionary(itemId, in node, false);
				});
				this.treeDataController.RemoveItem(node);
				base.UpdateIdToNodeDictionary(id, in node, false);
				this.m_Hierarchy.Remove(in node);
				if (rebuildTree)
				{
					base.baseTreeView.RefreshItems();
				}
				flag4 = true;
			}
			else
			{
				flag4 = false;
			}
			return flag4;
		}

		public override object GetItemForId(int id)
		{
			return this.treeDataController.GetTreeItemDataForNode(base.GetHierarchyNodeById(id)).data;
		}

		public virtual T GetDataForId(int id)
		{
			return this.treeDataController.GetDataForNode(base.GetHierarchyNodeById(id));
		}

		public virtual T GetDataForIndex(int index)
		{
			return this.treeDataController.GetDataForNode(base.GetHierarchyNodeByIndex(index));
		}

		public override object GetItemForIndex(int index)
		{
			return this.treeDataController.GetDataForNode(base.GetHierarchyNodeByIndex(index));
		}

		private TreeDataController<T> m_TreeDataController;
	}
}
