using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Hierarchy;
using Unity.Profiling;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public abstract class BaseTreeViewController : CollectionViewController
	{
		protected BaseTreeView baseTreeView
		{
			get
			{
				return base.view as BaseTreeView;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<TreeViewExpansionChangedArgs> itemExpandedChanged;

		protected BaseTreeViewController()
		{
			this.hierarchy = new Hierarchy();
		}

		~BaseTreeViewController()
		{
			this.DisposeHierarchy();
		}

		private protected Hierarchy hierarchy
		{
			get
			{
				return this.m_Hierarchy;
			}
			set
			{
				bool flag = this.hierarchy == value;
				if (!flag)
				{
					this.DisposeHierarchy();
					bool flag2 = value == null;
					if (!flag2)
					{
						this.m_Hierarchy = value;
						this.m_HierarchyFlattened = new HierarchyFlattened(this.m_Hierarchy);
						this.m_HierarchyViewModel = new HierarchyViewModel(this.m_HierarchyFlattened, HierarchyNodeFlags.None);
						this.m_TreeViewDataProperty = this.m_Hierarchy.GetOrCreatePropertyUnmanaged<int>("TreeViewDataProperty", HierarchyPropertyStorageType.Dense);
					}
				}
			}
		}

		internal void DisposeHierarchy()
		{
			bool flag = this.m_HierarchyViewModel != null;
			if (flag)
			{
				bool isCreated = this.m_HierarchyViewModel.IsCreated;
				if (isCreated)
				{
					this.m_HierarchyViewModel.Dispose();
				}
				this.m_HierarchyViewModel = null;
			}
			bool flag2 = this.m_HierarchyFlattened != null;
			if (flag2)
			{
				bool isCreated2 = this.m_HierarchyFlattened.IsCreated;
				if (isCreated2)
				{
					this.m_HierarchyFlattened.Dispose();
				}
				this.m_HierarchyFlattened = null;
			}
			bool flag3 = this.m_Hierarchy != null;
			if (flag3)
			{
				bool isCreated3 = this.m_Hierarchy.IsCreated;
				if (isCreated3)
				{
					this.m_Hierarchy.Dispose();
				}
				this.m_Hierarchy = null;
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
				throw new InvalidOperationException("Can't set itemsSource directly. Override this controller to manage tree data.");
			}
		}

		[Obsolete("RebuildTree is no longer supported and will be removed.", false)]
		public void RebuildTree()
		{
		}

		public IEnumerable<int> GetRootItemIds()
		{
			foreach (ref HierarchyNode ptr in this.m_Hierarchy.EnumerateChildren(this.m_Hierarchy.Root))
			{
				HierarchyNode node = ptr;
				yield return this.m_TreeViewDataProperty.GetValue(in node);
				node = default(HierarchyNode);
			}
			HierarchyNodeChildren.Enumerator enumerator = default(HierarchyNodeChildren.Enumerator);
			yield break;
		}

		public virtual IEnumerable<int> GetAllItemIds(IEnumerable<int> rootIds = null)
		{
			bool flag = rootIds == null;
			if (flag)
			{
				foreach (ref HierarchyFlattenedNode ptr in this.m_HierarchyFlattened)
				{
					HierarchyFlattenedNode flattenedNode = ptr;
					HierarchyNode node = flattenedNode.Node;
					bool flag2 = (in node) == this.m_Hierarchy.Root || !this.m_Hierarchy.Exists(in node);
					if (!flag2)
					{
						yield return this.m_TreeViewDataProperty.GetValue(in node);
						node = default(HierarchyNode);
						flattenedNode = default(HierarchyFlattenedNode);
					}
				}
				HierarchyFlattened.Enumerator enumerator = default(HierarchyFlattened.Enumerator);
				yield break;
			}
			foreach (int id in rootIds)
			{
				HierarchyNode parentNode = this.m_IdToNodeDictionary[id];
				bool flag3 = !this.m_Hierarchy.Exists(in parentNode);
				if (!flag3)
				{
					HierarchyFlattenedChildrenEnumerable flattenedNodeChildren = this.m_HierarchyFlattened.EnumerateChildren(in parentNode);
					foreach (ref HierarchyFlattenedNode ptr2 in flattenedNodeChildren)
					{
						HierarchyFlattenedNode flattenedNode2 = ptr2;
						IHierarchyProperty<int> treeViewDataProperty = this.m_TreeViewDataProperty;
						HierarchyNode node2 = flattenedNode2.Node;
						yield return treeViewDataProperty.GetValue(in node2);
						flattenedNode2 = default(HierarchyFlattenedNode);
					}
					HierarchyFlattenedChildrenEnumerable.Enumerator enumerator3 = default(HierarchyFlattenedChildrenEnumerable.Enumerator);
					yield return id;
					parentNode = default(HierarchyNode);
					flattenedNodeChildren = default(HierarchyFlattenedChildrenEnumerable);
				}
			}
			IEnumerator<int> enumerator2 = null;
			yield break;
			yield break;
		}

		public virtual int GetParentId(int id)
		{
			HierarchyNode hierarchyNodeById = this.GetHierarchyNodeById(id);
			bool flag = (in hierarchyNodeById) == HierarchyNode.Null || !this.m_Hierarchy.Exists(in hierarchyNodeById);
			int num;
			if (flag)
			{
				num = BaseTreeView.invalidId;
			}
			else
			{
				HierarchyNode parent = this.m_Hierarchy.GetParent(in hierarchyNodeById);
				bool flag2 = (in parent) == this.m_Hierarchy.Root;
				if (flag2)
				{
					num = BaseTreeView.invalidId;
				}
				else
				{
					num = this.m_TreeViewDataProperty.GetValue(in parent);
				}
			}
			return num;
		}

		public virtual IEnumerable<int> GetChildrenIds(int id)
		{
			HierarchyNode nodeById = this.GetHierarchyNodeById(id);
			bool flag = (in nodeById) == HierarchyNode.Null || !this.m_Hierarchy.Exists(in nodeById);
			if (flag)
			{
				yield break;
			}
			foreach (ref HierarchyNode ptr in this.m_Hierarchy.EnumerateChildren(in nodeById))
			{
				HierarchyNode node = ptr;
				yield return this.m_TreeViewDataProperty.GetValue(in node);
				node = default(HierarchyNode);
			}
			HierarchyNodeChildren.Enumerator enumerator = default(HierarchyNodeChildren.Enumerator);
			yield break;
		}

		public unsafe virtual void Move(int id, int newParentId, int childIndex = -1, bool rebuildTree = true)
		{
			bool flag = id == newParentId;
			if (!flag)
			{
				bool flag2 = this.IsChildOf(newParentId, id);
				if (!flag2)
				{
					HierarchyNode hierarchyNode;
					bool flag3 = !this.m_IdToNodeDictionary.TryGetValue(id, out hierarchyNode);
					if (!flag3)
					{
						HierarchyNode hierarchyNode2 = ((newParentId == BaseTreeView.invalidId) ? (*this.m_Hierarchy.Root) : this.GetHierarchyNodeById(newParentId));
						HierarchyNode parent = this.m_Hierarchy.GetParent(in hierarchyNode);
						bool flag4 = (in parent) == (in hierarchyNode2);
						if (flag4)
						{
							int childIndexForId = this.GetChildIndexForId(id);
							bool flag5 = childIndexForId < childIndex;
							if (flag5)
							{
								childIndex--;
							}
						}
						else
						{
							this.m_Hierarchy.SetParent(in hierarchyNode, in hierarchyNode2);
						}
						this.UpdateSortOrder(in hierarchyNode2, in hierarchyNode, childIndex);
						if (rebuildTree)
						{
							this.RaiseItemParentChanged(id, newParentId);
						}
					}
				}
			}
		}

		public abstract bool TryRemoveItem(int id, bool rebuildTree = true);

		internal override void InvokeMakeItem(ReusableCollectionItem reusableItem)
		{
			ReusableTreeViewItem reusableTreeViewItem = reusableItem as ReusableTreeViewItem;
			bool flag = reusableTreeViewItem != null;
			if (flag)
			{
				reusableTreeViewItem.Init(this.MakeItem());
				this.PostInitRegistration(reusableTreeViewItem);
			}
		}

		internal override void InvokeBindItem(ReusableCollectionItem reusableItem, int index)
		{
			ReusableTreeViewItem reusableTreeViewItem = reusableItem as ReusableTreeViewItem;
			bool flag = reusableTreeViewItem != null;
			if (flag)
			{
				reusableTreeViewItem.customIndentWidth = this.baseTreeView.customIdent;
				reusableTreeViewItem.Indent(this.GetIndentationDepthByIndex(index));
				reusableTreeViewItem.SetExpandedWithoutNotify(this.IsExpandedByIndex(index));
				reusableTreeViewItem.SetToggleVisibility(this.HasChildrenByIndex(index));
			}
			base.InvokeBindItem(reusableItem, index);
		}

		internal override void InvokeDestroyItem(ReusableCollectionItem reusableItem)
		{
			ReusableTreeViewItem reusableTreeViewItem = reusableItem as ReusableTreeViewItem;
			bool flag = reusableTreeViewItem != null;
			if (flag)
			{
				reusableTreeViewItem.onPointerUp -= this.OnItemPointerUp;
				reusableTreeViewItem.onToggleValueChanged -= this.OnToggleValueChanged;
			}
			base.InvokeDestroyItem(reusableItem);
		}

		internal void PostInitRegistration(ReusableTreeViewItem treeItem)
		{
			treeItem.onPointerUp += this.OnItemPointerUp;
			treeItem.onToggleValueChanged += this.OnToggleValueChanged;
		}

		private void OnItemPointerUp(PointerUpEvent evt)
		{
			bool flag = (evt.modifiers & EventModifiers.Alt) == EventModifiers.None;
			if (!flag)
			{
				VisualElement visualElement = evt.currentTarget as VisualElement;
				Toggle toggle = visualElement.Q<Toggle>(BaseTreeView.itemToggleUssClassName, null);
				int index = ((ReusableTreeViewItem)toggle.userData).index;
				bool flag2 = !this.HasChildrenByIndex(index);
				if (!flag2)
				{
					bool flag3 = this.IsExpandedByIndex(index);
					bool flag4 = this.IsViewDataKeyEnabled();
					if (flag4)
					{
						int idForIndex = this.GetIdForIndex(index);
						HashSet<int> hashSet = new HashSet<int>(this.baseTreeView.expandedItemIds);
						bool flag5 = flag3;
						if (flag5)
						{
							hashSet.Remove(idForIndex);
						}
						else
						{
							hashSet.Add(idForIndex);
						}
						IEnumerable<int> childrenIdsByIndex = this.GetChildrenIdsByIndex(index);
						foreach (int num in this.GetAllItemIds(childrenIdsByIndex))
						{
							bool flag6 = this.HasChildren(num);
							if (flag6)
							{
								bool flag7 = flag3;
								if (flag7)
								{
									hashSet.Remove(num);
								}
								else
								{
									hashSet.Add(num);
								}
							}
						}
						this.baseTreeView.expandedItemIds = new List<int>(hashSet);
					}
					bool flag8 = flag3;
					if (flag8)
					{
						HierarchyViewModel hierarchyViewModel = this.m_HierarchyViewModel;
						HierarchyNode hierarchyNode = this.GetHierarchyNodeByIndex(index);
						hierarchyViewModel.ClearFlagsRecursive(in hierarchyNode, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
					}
					else
					{
						HierarchyViewModel hierarchyViewModel2 = this.m_HierarchyViewModel;
						HierarchyNode hierarchyNode = this.GetHierarchyNodeByIndex(index);
						hierarchyViewModel2.SetFlagsRecursive(in hierarchyNode, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
					}
					this.UpdateHierarchy();
					this.baseTreeView.RefreshItems();
					this.RaiseItemExpandedChanged(this.GetIdForIndex(index), !flag3, true);
					evt.StopPropagation();
				}
			}
		}

		private void RaiseItemExpandedChanged(int id, bool isExpanded, bool isAppliedToAllChildren)
		{
			Action<TreeViewExpansionChangedArgs> action = this.itemExpandedChanged;
			if (action != null)
			{
				action(new TreeViewExpansionChangedArgs
				{
					id = id,
					isExpanded = isExpanded,
					isAppliedToAllChildren = isAppliedToAllChildren
				});
			}
		}

		private void OnToggleValueChanged(ChangeEvent<bool> evt)
		{
			Toggle toggle = evt.target as Toggle;
			int index = ((ReusableTreeViewItem)toggle.userData).index;
			bool flag = this.IsExpandedByIndex(index);
			bool flag2 = flag;
			if (flag2)
			{
				this.CollapseItemByIndex(index, false, true);
			}
			else
			{
				this.ExpandItemByIndex(index, false, true);
			}
			this.baseTreeView.scrollView.contentContainer.Focus();
		}

		public virtual int GetTreeItemsCount()
		{
			return this.m_Hierarchy.Count;
		}

		public override int GetIndexForId(int id)
		{
			HierarchyNode hierarchyNode;
			return this.m_IdToNodeDictionary.TryGetValue(id, out hierarchyNode) ? this.m_HierarchyViewModel.IndexOf(in hierarchyNode) : BaseTreeView.invalidId;
		}

		public override int GetIdForIndex(int index)
		{
			int count = this.m_HierarchyViewModel.Count;
			bool flag = index == count && count > 0;
			int num;
			if (flag)
			{
				IHierarchyProperty<int> treeViewDataProperty = this.m_TreeViewDataProperty;
				HierarchyViewModel hierarchyViewModel = this.m_HierarchyViewModel;
				num = treeViewDataProperty.GetValue(hierarchyViewModel[hierarchyViewModel.Count - 1]);
			}
			else
			{
				num = ((!this.IsIndexValid(index)) ? BaseTreeView.invalidId : this.m_TreeViewDataProperty.GetValue(this.m_HierarchyViewModel[index]));
			}
			return num;
		}

		public virtual bool HasChildren(int id)
		{
			HierarchyNode hierarchyNode;
			bool flag = this.m_IdToNodeDictionary.TryGetValue(id, out hierarchyNode) && this.m_Hierarchy.Exists(in hierarchyNode);
			return flag && this.m_Hierarchy.GetChildrenCount(in hierarchyNode) > 0;
		}

		public bool Exists(int id)
		{
			return this.m_IdToNodeDictionary.ContainsKey(id);
		}

		public bool HasChildrenByIndex(int index)
		{
			bool flag = !this.IsIndexValid(index);
			return !flag && this.m_HierarchyViewModel.GetChildrenCount(this.m_HierarchyViewModel[index]) > 0;
		}

		public IEnumerable<int> GetChildrenIdsByIndex(int index)
		{
			bool flag = !this.IsIndexValid(index);
			if (flag)
			{
				yield break;
			}
			foreach (ref HierarchyNode ptr in this.m_Hierarchy.EnumerateChildren(this.m_HierarchyViewModel[index]))
			{
				HierarchyNode node = ptr;
				yield return this.m_TreeViewDataProperty.GetValue(in node);
				node = default(HierarchyNode);
			}
			HierarchyNodeChildren.Enumerator enumerator = default(HierarchyNodeChildren.Enumerator);
			yield break;
		}

		public int GetChildIndexForId(int id)
		{
			HierarchyNode hierarchyNode;
			bool flag = this.m_IdToNodeDictionary.TryGetValue(id, out hierarchyNode);
			int num;
			if (flag)
			{
				HierarchyNode parent = this.m_Hierarchy.GetParent(in hierarchyNode);
				bool flag2 = (in parent) == HierarchyNode.Null;
				if (flag2)
				{
					num = BaseTreeView.invalidId;
				}
				else
				{
					HierarchyNodeChildren hierarchyNodeChildren = this.m_Hierarchy.EnumerateChildren(in parent);
					int num2 = 0;
					foreach (ref HierarchyNode ptr in hierarchyNodeChildren)
					{
						HierarchyNode hierarchyNode2 = ptr;
						bool flag3 = (in hierarchyNode2) == (in hierarchyNode);
						if (flag3)
						{
							break;
						}
						num2++;
					}
					num = num2;
				}
			}
			else
			{
				num = BaseTreeView.invalidId;
			}
			return num;
		}

		public int GetIndentationDepth(int id)
		{
			int num = 0;
			int num2 = this.GetParentId(id);
			while (num2 != BaseTreeView.invalidId)
			{
				num2 = this.GetParentId(num2);
				num++;
			}
			return num;
		}

		public int GetIndentationDepthByIndex(int index)
		{
			int idForIndex = this.GetIdForIndex(index);
			return this.GetIndentationDepth(idForIndex);
		}

		public virtual bool CanChangeExpandedState(int id)
		{
			return true;
		}

		public bool IsExpanded(int id)
		{
			if (this.m_IdToNodeDictionary.ContainsKey(id))
			{
				Hierarchy hierarchy = this.m_Hierarchy;
				HierarchyNode hierarchyNode = this.m_IdToNodeDictionary[id];
				if (hierarchy.Exists(in hierarchyNode))
				{
					HierarchyViewModel hierarchyViewModel = this.m_HierarchyViewModel;
					HierarchyNode hierarchyNode2 = this.m_IdToNodeDictionary[id];
					return hierarchyViewModel.HasAllFlags(in hierarchyNode2, HierarchyNodeFlags.Expanded);
				}
			}
			return false;
		}

		public bool IsExpandedByIndex(int index)
		{
			bool flag = !this.IsIndexValid(index);
			return !flag && this.IsExpanded(this.GetIdForIndex(index));
		}

		public void ExpandItemByIndex(int index, bool expandAllChildren, bool refresh = true)
		{
			using (BaseTreeViewController.K_ExpandItemByIndex.Auto())
			{
				bool flag = !this.HasChildrenByIndex(index);
				if (!flag)
				{
					HierarchyNode hierarchyNodeById = this.GetHierarchyNodeById(this.GetIdForIndex(index));
					this.ExpandItemByNode(in hierarchyNodeById, expandAllChildren, refresh);
				}
			}
		}

		public void ExpandItem(int id, bool expandAllChildren, bool refresh = true)
		{
			bool flag = !this.HasChildren(id) || !this.CanChangeExpandedState(id);
			if (!flag)
			{
				HierarchyNode hierarchyNode;
				bool flag2 = this.m_IdToNodeDictionary.TryGetValue(id, out hierarchyNode);
				if (flag2)
				{
					this.ExpandItemByNode(in hierarchyNode, expandAllChildren, refresh);
				}
			}
		}

		public void CollapseItemByIndex(int index, bool collapseAllChildren, bool refresh = true)
		{
			bool flag = !this.HasChildrenByIndex(index);
			if (!flag)
			{
				HierarchyNode hierarchyNodeById = this.GetHierarchyNodeById(this.GetIdForIndex(index));
				this.CollapseItemByNode(in hierarchyNodeById, collapseAllChildren, refresh);
			}
		}

		public void CollapseItem(int id, bool collapseAllChildren, bool refresh = true)
		{
			bool flag = !this.HasChildren(id) || !this.CanChangeExpandedState(id);
			if (!flag)
			{
				HierarchyNode hierarchyNode;
				bool flag2 = this.m_IdToNodeDictionary.TryGetValue(id, out hierarchyNode);
				if (flag2)
				{
					this.CollapseItemByNode(in hierarchyNode, collapseAllChildren, refresh);
				}
			}
		}

		public void ExpandAll()
		{
			this.m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Expanded);
			this.UpdateHierarchy();
			bool flag = this.IsViewDataKeyEnabled();
			if (flag)
			{
				this.baseTreeView.expandedItemIds.Clear();
				foreach (ref HierarchyNode ptr in this.m_HierarchyViewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Expanded))
				{
					HierarchyNode hierarchyNode = ptr;
					this.baseTreeView.expandedItemIds.Add(this.m_TreeViewDataProperty.GetValue(in hierarchyNode));
				}
				this.baseTreeView.SaveViewData();
			}
			this.baseTreeView.RefreshItems();
			this.RaiseItemExpandedChanged(-1, true, true);
		}

		public void CollapseAll()
		{
			this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Expanded);
			this.UpdateHierarchy();
			bool flag = this.IsViewDataKeyEnabled();
			if (flag)
			{
				this.baseTreeView.expandedItemIds.Clear();
				this.baseTreeView.SaveViewData();
			}
			this.baseTreeView.RefreshItems();
			this.RaiseItemExpandedChanged(-1, false, true);
		}

		private void ExpandItemByNode(in HierarchyNode node, bool expandAllChildren, bool refresh)
		{
			int value = this.m_TreeViewDataProperty.GetValue(in node);
			bool flag = !this.CanChangeExpandedState(value);
			if (!flag)
			{
				if (expandAllChildren)
				{
					this.m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
				}
				else
				{
					this.m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Expanded);
				}
				this.m_HierarchyHasPendingChanged = true;
				bool flag2 = this.IsViewDataKeyEnabled();
				if (flag2)
				{
					HashSet<int> hashSet = new HashSet<int>(this.baseTreeView.expandedItemIds) { value };
					if (expandAllChildren)
					{
						this.UpdateHierarchy();
						IEnumerable<int> childrenIds = this.GetChildrenIds(value);
						foreach (int num in this.GetAllItemIds(childrenIds))
						{
							hashSet.Add(num);
						}
					}
					this.baseTreeView.expandedItemIds.Clear();
					this.baseTreeView.expandedItemIds.AddRange(hashSet);
					this.baseTreeView.SaveViewData();
				}
				if (refresh)
				{
					this.baseTreeView.RefreshItems();
				}
				this.RaiseItemExpandedChanged(value, true, expandAllChildren);
			}
		}

		private void CollapseItemByNode(in HierarchyNode node, bool collapseAllChildren, bool refresh)
		{
			int value = this.m_TreeViewDataProperty.GetValue(in node);
			bool flag = !this.CanChangeExpandedState(value);
			if (!flag)
			{
				bool flag2 = this.IsViewDataKeyEnabled();
				if (flag2)
				{
					if (collapseAllChildren)
					{
						IEnumerable<int> childrenIds = this.GetChildrenIds(value);
						foreach (int num in this.GetAllItemIds(childrenIds))
						{
							this.baseTreeView.expandedItemIds.Remove(num);
						}
					}
					this.baseTreeView.expandedItemIds.Remove(value);
					this.baseTreeView.SaveViewData();
				}
				if (collapseAllChildren)
				{
					this.m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
				}
				else
				{
					this.m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Expanded);
				}
				this.m_HierarchyHasPendingChanged = true;
				if (refresh)
				{
					this.baseTreeView.RefreshItems();
				}
				this.RaiseItemExpandedChanged(value, false, collapseAllChildren);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void GetExpandedItemIds(List<int> list)
		{
			bool flag = list.Count > 0;
			if (flag)
			{
				list.Clear();
			}
			bool flag2 = this.IsViewDataKeyEnabled();
			if (flag2)
			{
				list.AddRange(this.baseTreeView.expandedItemIds);
			}
			foreach (ref HierarchyNode ptr in this.m_HierarchyViewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Expanded))
			{
				HierarchyNode hierarchyNode = ptr;
				list.Add(this.m_TreeViewDataProperty.GetValue(in hierarchyNode));
			}
		}

		internal bool IsViewDataKeyEnabled()
		{
			return this.baseTreeView.enableViewDataPersistence && !string.IsNullOrEmpty(this.baseTreeView.viewDataKey);
		}

		internal void ExpandAncestorNodes(in HierarchyNode node)
		{
			HierarchyNode hierarchyNode = this.m_Hierarchy.GetParent(in node);
			for (;;)
			{
				bool flag;
				if ((in hierarchyNode) != this.m_Hierarchy.Root)
				{
					HierarchyNode hierarchyNodeById;
					hierarchyNode = (hierarchyNodeById = this.GetHierarchyNodeById(this.m_TreeViewDataProperty.GetValue(in hierarchyNode)));
					flag = (in hierarchyNodeById) != this.m_Hierarchy.Root;
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					break;
				}
				int value = this.m_TreeViewDataProperty.GetValue(in hierarchyNode);
				bool flag2 = !this.m_HierarchyViewModel.HasAllFlags(in hierarchyNode, HierarchyNodeFlags.Expanded) && this.CanChangeExpandedState(value);
				if (flag2)
				{
					bool flag3 = this.IsViewDataKeyEnabled();
					if (flag3)
					{
						this.baseTreeView.expandedItemIds.Add(value);
					}
					this.m_HierarchyViewModel.SetFlags(in hierarchyNode, HierarchyNodeFlags.Expanded);
					this.m_HierarchyViewModel.Update();
				}
				hierarchyNode = this.m_Hierarchy.GetParent(in hierarchyNode);
			}
		}

		internal override void PreRefresh()
		{
			bool flag = !this.m_HierarchyHasPendingChanged;
			if (!flag)
			{
				this.UpdateHierarchy();
			}
		}

		private bool IsIndexValid(int index)
		{
			return index >= 0 && index < this.m_HierarchyViewModel.Count;
		}

		private bool IsChildOf(int childId, int id)
		{
			bool flag = childId == BaseTreeView.invalidId || id == BaseTreeView.invalidId;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				HierarchyNode hierarchyNode = this.GetHierarchyNodeById(childId);
				HierarchyNode hierarchyNodeById = this.GetHierarchyNodeById(id);
				bool flag3 = (in hierarchyNodeById) == (in hierarchyNode);
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					for (;;)
					{
						HierarchyNode parent;
						HierarchyNode hierarchyNode2 = (parent = this.m_Hierarchy.GetParent(in hierarchyNode));
						if (!((in parent) != this.m_Hierarchy.Root))
						{
							goto Block_5;
						}
						bool flag4 = (in hierarchyNodeById) == (in hierarchyNode2);
						if (flag4)
						{
							break;
						}
						hierarchyNode = hierarchyNode2;
					}
					return true;
					Block_5:
					flag2 = false;
				}
			}
			return flag2;
		}

		internal void RaiseItemParentChanged(int id, int newParentId)
		{
			base.RaiseItemIndexChanged(id, newParentId);
		}

		internal unsafe HierarchyNode CreateNode(in HierarchyNode parent)
		{
			Hierarchy hierarchy = this.m_Hierarchy;
			HierarchyNode hierarchyNode = (((in parent) == HierarchyNode.Null) ? (*this.m_Hierarchy.Root) : parent);
			return hierarchy.Add(in hierarchyNode);
		}

		internal void UpdateIdToNodeDictionary(int id, in HierarchyNode node, bool isAdd = true)
		{
			this.m_HierarchyHasPendingChanged = true;
			if (isAdd)
			{
				this.m_TreeViewDataProperty.SetValue(in node, id);
				this.m_IdToNodeDictionary[id] = node;
			}
			else
			{
				this.m_IdToNodeDictionary.Remove(id);
			}
		}

		internal unsafe void RemoveAllChildrenItemsFromCollections(in HierarchyNode node, Action<HierarchyNode, int> removeCallback)
		{
			bool flag = (in node) == HierarchyNode.Null;
			if (!flag)
			{
				int num = this.m_HierarchyFlattened.IndexOf(in node);
				bool flag2 = num == -1;
				if (!flag2)
				{
					int num2 = num + 1;
					int childrenCountRecursive = this.m_HierarchyFlattened.GetChildrenCountRecursive(in node);
					for (int i = num2; i < num2 + childrenCountRecursive; i++)
					{
						HierarchyFlattenedNode hierarchyFlattenedNode = *this.m_HierarchyFlattened[i];
						HierarchyNode node2 = hierarchyFlattenedNode.Node;
						IHierarchyProperty<int> treeViewDataProperty = this.m_TreeViewDataProperty;
						HierarchyNode node3 = hierarchyFlattenedNode.Node;
						removeCallback(node2, treeViewDataProperty.GetValue(in node3));
					}
				}
			}
		}

		internal void ClearIdToNodeDictionary()
		{
			this.m_IdToNodeDictionary.Clear();
		}

		internal unsafe void UpdateSortOrder(in HierarchyNode newParent, in HierarchyNode insertedNode, int insertedIndex)
		{
			Span<HierarchyNode> span = this.m_Hierarchy.GetChildren(in newParent);
			bool flag = insertedIndex == -1;
			if (flag)
			{
				insertedIndex = span.Length;
			}
			int num = 0;
			int num2 = 0;
			while (num2 < insertedIndex && num2 < span.Length)
			{
				bool flag2 = (in insertedNode) == span[num2];
				if (!flag2)
				{
					this.m_Hierarchy.SetSortIndex(span[num2], num++);
				}
				num2++;
			}
			this.m_Hierarchy.SetSortIndex(in insertedNode, insertedIndex);
			bool flag3 = insertedIndex == num;
			if (flag3)
			{
				num++;
			}
			for (int i = insertedIndex; i < span.Length; i++)
			{
				bool flag4 = (in insertedNode) == span[i];
				if (!flag4)
				{
					this.m_Hierarchy.SetSortIndex(span[i], num++);
				}
			}
			this.m_Hierarchy.SortChildren(in newParent);
			this.UpdateHierarchy();
			Span<HierarchyNode> span2 = this.m_Hierarchy.GetChildren(in newParent);
			Span<HierarchyNode> span3 = span2;
			for (int j = 0; j < span3.Length; j++)
			{
				HierarchyNode hierarchyNode = *span3[j];
				this.m_Hierarchy.SetSortIndex(in hierarchyNode, 0);
			}
		}

		internal void OnViewDataReadyUpdateNodes()
		{
			foreach (int num in this.baseTreeView.expandedItemIds)
			{
				HierarchyNode hierarchyNode;
				bool flag = !this.m_IdToNodeDictionary.TryGetValue(num, out hierarchyNode);
				if (!flag)
				{
					this.m_HierarchyViewModel.SetFlags(in hierarchyNode, HierarchyNodeFlags.Expanded);
				}
			}
			this.UpdateHierarchy();
		}

		internal void UpdateHierarchy()
		{
			bool updateNeeded = this.m_Hierarchy.UpdateNeeded;
			if (updateNeeded)
			{
				this.m_Hierarchy.Update();
			}
			bool updateNeeded2 = this.m_HierarchyFlattened.UpdateNeeded;
			if (updateNeeded2)
			{
				this.m_HierarchyFlattened.Update();
			}
			bool updateNeeded3 = this.m_HierarchyViewModel.UpdateNeeded;
			if (updateNeeded3)
			{
				this.m_HierarchyViewModel.Update();
			}
			this.m_HierarchyHasPendingChanged = false;
		}

		internal unsafe HierarchyNode GetHierarchyNodeById(int id)
		{
			HierarchyNode hierarchyNode;
			return (this.m_IdToNodeDictionary.TryGetValue(id, out hierarchyNode) && this.m_Hierarchy.Exists(in hierarchyNode)) ? hierarchyNode : (*HierarchyNode.Null);
		}

		internal unsafe HierarchyNode GetHierarchyNodeByIndex(int index)
		{
			bool flag = !this.IsIndexValid(index);
			HierarchyNode hierarchyNode;
			if (flag)
			{
				hierarchyNode = *HierarchyNode.Null;
			}
			else
			{
				hierarchyNode = *this.m_HierarchyViewModel[index];
			}
			return hierarchyNode;
		}

		private protected Hierarchy m_Hierarchy;

		private protected HierarchyFlattened m_HierarchyFlattened;

		private protected HierarchyViewModel m_HierarchyViewModel;

		private protected Dictionary<int, HierarchyNode> m_IdToNodeDictionary = new Dictionary<int, HierarchyNode>();

		private const string k_HierarchyPropertyName = "TreeViewDataProperty";

		private IHierarchyProperty<int> m_TreeViewDataProperty;

		private bool m_HierarchyHasPendingChanged;

		private static readonly ProfilerMarker K_ExpandItemByIndex = new ProfilerMarker(ProfilerCategory.Scripts, "BaseTreeViewController.ExpandItemByIndex");
	}
}
