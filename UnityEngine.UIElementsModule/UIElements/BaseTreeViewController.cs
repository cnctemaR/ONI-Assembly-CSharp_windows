using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine.Pool;

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

		public void RebuildTree()
		{
			this.m_TreeItems.Clear();
			this.m_RootIndices.Clear();
			foreach (int num in this.GetAllItemIds(null))
			{
				int parentId = this.GetParentId(num);
				bool flag = parentId == -1;
				if (flag)
				{
					this.m_RootIndices.Add(num);
				}
				this.m_TreeItems.Add(num, new TreeItem(num, parentId, this.GetChildrenIds(num)));
			}
			this.RegenerateWrappers();
		}

		public IEnumerable<int> GetRootItemIds()
		{
			return this.m_RootIndices;
		}

		public abstract IEnumerable<int> GetAllItemIds(IEnumerable<int> rootIds = null);

		public abstract int GetParentId(int id);

		public abstract IEnumerable<int> GetChildrenIds(int id);

		public abstract void Move(int id, int newParentId, int childIndex = -1, bool rebuildTree = true);

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
			bool autoExpand = this.baseTreeView.autoExpand;
			if (autoExpand)
			{
				this.baseTreeView.expandedItemIds.Remove(treeItem.id);
				this.baseTreeView.schedule.Execute(delegate
				{
					this.ExpandItem(treeItem.id, true, true);
				});
			}
		}

		private void OnItemPointerUp(PointerUpEvent evt)
		{
			bool flag = (evt.modifiers & EventModifiers.Alt) == EventModifiers.None;
			if (!flag)
			{
				VisualElement visualElement = evt.currentTarget as VisualElement;
				Toggle toggle = visualElement.Q<Toggle>(BaseTreeView.itemToggleUssClassName, null);
				int index = ((ReusableTreeViewItem)toggle.userData).index;
				int idForIndex = this.GetIdForIndex(index);
				bool flag2 = this.IsExpandedByIndex(index);
				bool flag3 = !this.HasChildrenByIndex(index);
				if (!flag3)
				{
					HashSet<int> hashSet = new HashSet<int>(this.baseTreeView.expandedItemIds);
					bool flag4 = flag2;
					if (flag4)
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
						bool flag5 = this.HasChildren(num);
						if (flag5)
						{
							bool flag6 = flag2;
							if (flag6)
							{
								hashSet.Remove(num);
							}
							else
							{
								hashSet.Add(num);
							}
						}
					}
					this.baseTreeView.expandedItemIds = hashSet.ToList<int>();
					this.RegenerateWrappers();
					this.baseTreeView.RefreshItems();
					evt.StopPropagation();
				}
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
				this.CollapseItemByIndex(index, false);
			}
			else
			{
				this.ExpandItemByIndex(index, false, true);
			}
			this.baseTreeView.scrollView.contentContainer.Focus();
		}

		public virtual int GetTreeItemsCount()
		{
			return this.m_TreeItems.Count;
		}

		public override int GetIndexForId(int id)
		{
			bool flag = this.m_TreeItemIdsWithItemWrappers.Contains(id);
			if (flag)
			{
				for (int i = 0; i < this.m_ItemWrappers.Count; i++)
				{
					bool flag2 = this.m_ItemWrappers[i].id == id;
					if (flag2)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public override int GetIdForIndex(int index)
		{
			return this.IsIndexValid(index) ? this.m_ItemWrappers[index].id : (-1);
		}

		public virtual bool HasChildren(int id)
		{
			TreeItem treeItem;
			bool flag = this.m_TreeItems.TryGetValue(id, out treeItem);
			return flag && treeItem.hasChildren;
		}

		internal bool Exists(int id)
		{
			return this.m_TreeItems.ContainsKey(id);
		}

		public bool HasChildrenByIndex(int index)
		{
			return this.IsIndexValid(index) && this.m_ItemWrappers[index].hasChildren;
		}

		public IEnumerable<int> GetChildrenIdsByIndex(int index)
		{
			return this.IsIndexValid(index) ? this.m_ItemWrappers[index].childrenIds : null;
		}

		public int GetChildIndexForId(int id)
		{
			TreeItem treeItem;
			bool flag = !this.m_TreeItems.TryGetValue(id, out treeItem);
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				int num2 = 0;
				TreeItem treeItem2;
				IEnumerable<int> enumerable;
				if (!this.m_TreeItems.TryGetValue(treeItem.parentId, out treeItem2))
				{
					IEnumerable<int> rootIndices = this.m_RootIndices;
					enumerable = rootIndices;
				}
				else
				{
					enumerable = treeItem2.childrenIds;
				}
				IEnumerable<int> enumerable2 = enumerable;
				foreach (int num3 in enumerable2)
				{
					bool flag2 = num3 == id;
					if (flag2)
					{
						return num2;
					}
					num2++;
				}
				num = -1;
			}
			return num;
		}

		internal int GetIndentationDepth(int id)
		{
			int num = 0;
			int num2 = this.GetParentId(id);
			while (num2 != -1)
			{
				num2 = this.GetParentId(num2);
				num++;
			}
			return num;
		}

		internal int GetIndentationDepthByIndex(int index)
		{
			int idForIndex = this.GetIdForIndex(index);
			return this.GetIndentationDepth(idForIndex);
		}

		internal virtual bool CanChangeExpandedState(int id)
		{
			return true;
		}

		public bool IsExpanded(int id)
		{
			return this.baseTreeView.expandedItemIds.Contains(id);
		}

		public bool IsExpandedByIndex(int index)
		{
			bool flag = !this.IsIndexValid(index);
			return !flag && this.IsExpanded(this.m_ItemWrappers[index].id);
		}

		public void ExpandItemByIndex(int index, bool expandAllChildren, bool refresh = true)
		{
			using (BaseTreeViewController.K_ExpandItemByIndex.Auto())
			{
				bool flag = !this.HasChildrenByIndex(index);
				if (!flag)
				{
					int idForIndex = this.GetIdForIndex(index);
					bool flag2 = !this.CanChangeExpandedState(idForIndex);
					if (!flag2)
					{
						bool flag3 = !this.baseTreeView.expandedItemIds.Contains(idForIndex) || expandAllChildren;
						if (flag3)
						{
							IEnumerable<int> childrenIdsByIndex = this.GetChildrenIdsByIndex(index);
							List<int> list = new List<int>();
							foreach (int num in childrenIdsByIndex)
							{
								bool flag4 = !this.m_TreeItemIdsWithItemWrappers.Contains(num);
								if (flag4)
								{
									list.Add(num);
								}
							}
							this.CreateWrappers(list, this.GetIndentationDepth(idForIndex) + 1, ref this.m_WrapperInsertionList);
							this.m_ItemWrappers.InsertRange(index + 1, this.m_WrapperInsertionList);
							bool flag5 = !this.baseTreeView.expandedItemIds.Contains(this.m_ItemWrappers[index].id);
							if (flag5)
							{
								this.baseTreeView.expandedItemIds.Add(this.m_ItemWrappers[index].id);
							}
							this.m_WrapperInsertionList.Clear();
						}
						if (expandAllChildren)
						{
							IEnumerable<int> childrenIds = this.GetChildrenIds(idForIndex);
							foreach (int num2 in this.GetAllItemIds(childrenIds))
							{
								bool flag6 = !this.baseTreeView.expandedItemIds.Contains(num2);
								if (flag6)
								{
									this.ExpandItemByIndex(this.GetIndexForId(num2), true, false);
								}
							}
						}
						if (refresh)
						{
							this.baseTreeView.RefreshItems();
						}
					}
				}
			}
		}

		public void ExpandItem(int id, bool expandAllChildren, bool refresh = true)
		{
			bool flag = !this.HasChildren(id) || !this.CanChangeExpandedState(id);
			if (!flag)
			{
				for (int i = 0; i < this.m_ItemWrappers.Count; i++)
				{
					bool flag2 = this.m_ItemWrappers[i].id == id;
					if (flag2)
					{
						bool flag3 = expandAllChildren || !this.IsExpandedByIndex(i);
						if (flag3)
						{
							this.ExpandItemByIndex(i, expandAllChildren, refresh);
							return;
						}
					}
				}
				bool flag4 = this.baseTreeView.expandedItemIds.Contains(id);
				if (!flag4)
				{
					this.baseTreeView.expandedItemIds.Add(id);
				}
			}
		}

		public void CollapseItemByIndex(int index, bool collapseAllChildren)
		{
			bool flag = !this.HasChildrenByIndex(index);
			if (!flag)
			{
				int idForIndex = this.GetIdForIndex(index);
				bool flag2 = !this.CanChangeExpandedState(idForIndex);
				if (!flag2)
				{
					if (collapseAllChildren)
					{
						IEnumerable<int> childrenIds = this.GetChildrenIds(idForIndex);
						foreach (int num in this.GetAllItemIds(childrenIds))
						{
							this.baseTreeView.expandedItemIds.Remove(num);
						}
					}
					this.baseTreeView.expandedItemIds.Remove(idForIndex);
					int num2 = 0;
					int num3 = index + 1;
					int indentationDepthByIndex = this.GetIndentationDepthByIndex(index);
					while (num3 < this.m_ItemWrappers.Count && this.GetIndentationDepthByIndex(num3) > indentationDepthByIndex)
					{
						num2++;
						num3++;
					}
					int num4 = index + 1 + num2;
					for (int i = index + 1; i < num4; i++)
					{
						this.m_TreeItemIdsWithItemWrappers.Remove(this.m_ItemWrappers[i].id);
					}
					this.m_ItemWrappers.RemoveRange(index + 1, num2);
					this.baseTreeView.RefreshItems();
				}
			}
		}

		public void CollapseItem(int id, bool collapseAllChildren)
		{
			bool flag = !this.CanChangeExpandedState(id);
			if (!flag)
			{
				int i = 0;
				while (i < this.m_ItemWrappers.Count)
				{
					bool flag2 = this.m_ItemWrappers[i].id == id;
					if (flag2)
					{
						bool flag3 = this.IsExpandedByIndex(i);
						if (flag3)
						{
							this.CollapseItemByIndex(i, collapseAllChildren);
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
				bool flag4 = !this.baseTreeView.expandedItemIds.Contains(id);
				if (!flag4)
				{
					this.baseTreeView.expandedItemIds.Remove(id);
				}
			}
		}

		public void ExpandAll()
		{
			foreach (int num in this.GetAllItemIds(null))
			{
				bool flag = !this.CanChangeExpandedState(num);
				if (!flag)
				{
					bool flag2 = !this.baseTreeView.expandedItemIds.Contains(num);
					if (flag2)
					{
						this.baseTreeView.expandedItemIds.Add(num);
					}
				}
			}
			this.RegenerateWrappers();
			this.baseTreeView.RefreshItems();
		}

		public void CollapseAll()
		{
			bool flag = this.baseTreeView.expandedItemIds.Count == 0;
			if (!flag)
			{
				List<int> list;
				using (CollectionPool<List<int>, int>.Get(out list))
				{
					foreach (int num in this.baseTreeView.expandedItemIds)
					{
						bool flag2 = !this.CanChangeExpandedState(num);
						if (flag2)
						{
							list.Add(num);
						}
					}
					this.baseTreeView.expandedItemIds.Clear();
					this.baseTreeView.expandedItemIds.AddRange(list);
				}
				this.RegenerateWrappers();
				this.baseTreeView.RefreshItems();
			}
		}

		internal void RegenerateWrappers()
		{
			this.m_ItemWrappers.Clear();
			this.m_TreeItemIdsWithItemWrappers.Clear();
			IEnumerable<int> rootItemIds = this.GetRootItemIds();
			bool flag = rootItemIds == null;
			if (!flag)
			{
				this.CreateWrappers(rootItemIds, 0, ref this.m_ItemWrappers);
				base.SetItemsSourceWithoutNotify(this.m_ItemWrappers);
			}
		}

		private void CreateWrappers(IEnumerable<int> treeViewItemIds, int depth, ref List<TreeViewItemWrapper> wrappers)
		{
			using (BaseTreeViewController.k_CreateWrappers.Auto())
			{
				bool flag = treeViewItemIds == null || wrappers == null || this.m_TreeItemIdsWithItemWrappers == null;
				if (!flag)
				{
					foreach (int num in treeViewItemIds)
					{
						TreeItem treeItem;
						bool flag2 = !this.m_TreeItems.TryGetValue(num, out treeItem);
						if (!flag2)
						{
							TreeViewItemWrapper treeViewItemWrapper = new TreeViewItemWrapper(treeItem, depth);
							wrappers.Add(treeViewItemWrapper);
							this.m_TreeItemIdsWithItemWrappers.Add(num);
							BaseTreeView baseTreeView = this.baseTreeView;
							bool flag3 = ((baseTreeView != null) ? baseTreeView.expandedItemIds : null) == null;
							if (!flag3)
							{
								bool flag4 = this.baseTreeView.expandedItemIds.Contains(treeViewItemWrapper.id) && treeViewItemWrapper.hasChildren;
								if (flag4)
								{
									this.CreateWrappers(this.GetChildrenIds(treeViewItemWrapper.id), depth + 1, ref wrappers);
								}
							}
						}
					}
				}
			}
		}

		private bool IsIndexValid(int index)
		{
			return index >= 0 && index < this.m_ItemWrappers.Count;
		}

		internal void RaiseItemParentChanged(int id, int newParentId)
		{
			base.RaiseItemIndexChanged(id, newParentId);
		}

		private Dictionary<int, TreeItem> m_TreeItems = new Dictionary<int, TreeItem>();

		private List<int> m_RootIndices = new List<int>();

		private List<TreeViewItemWrapper> m_ItemWrappers = new List<TreeViewItemWrapper>();

		private HashSet<int> m_TreeItemIdsWithItemWrappers = new HashSet<int>();

		private List<TreeViewItemWrapper> m_WrapperInsertionList = new List<TreeViewItemWrapper>();

		private static readonly ProfilerMarker K_ExpandItemByIndex = new ProfilerMarker(ProfilerCategory.Scripts, "BaseTreeViewController.ExpandItemByIndex");

		private static readonly ProfilerMarker k_CreateWrappers = new ProfilerMarker("BaseTreeViewController.CreateWrappers");
	}
}
