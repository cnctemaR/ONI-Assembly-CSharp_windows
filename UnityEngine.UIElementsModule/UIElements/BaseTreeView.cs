using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	public abstract class BaseTreeView : BaseVerticalCollectionView
	{
		public new IList itemsSource
		{
			get
			{
				return this.viewController.itemsSource;
			}
			internal set
			{
				base.GetOrCreateViewController().itemsSource = value;
			}
		}

		public void SetRootItems<T>(IList<TreeViewItemData<T>> rootItems)
		{
			this.SetRootItemsInternal<T>(rootItems);
		}

		internal abstract void SetRootItemsInternal<T>(IList<TreeViewItemData<T>> rootItems);

		public IEnumerable<int> GetRootIds()
		{
			return this.viewController.GetRootItemIds();
		}

		public int GetTreeCount()
		{
			return this.viewController.GetTreeItemsCount();
		}

		public new BaseTreeViewController viewController
		{
			get
			{
				return base.viewController as BaseTreeViewController;
			}
		}

		private protected override void CreateVirtualizationController()
		{
			base.CreateVirtualizationController<ReusableTreeViewItem>();
		}

		public override void SetViewController(CollectionViewController controller)
		{
			bool flag = this.viewController != null;
			if (flag)
			{
				this.viewController.itemIndexChanged -= this.OnItemIndexChanged;
			}
			base.SetViewController(controller);
			bool flag2 = this.viewController != null;
			if (flag2)
			{
				this.viewController.itemIndexChanged += this.OnItemIndexChanged;
			}
		}

		private void OnItemIndexChanged(int srcIndex, int dstIndex)
		{
			base.RefreshItems();
		}

		internal override ICollectionDragAndDropController CreateDragAndDropController()
		{
			return new TreeViewReorderableDragAndDropController(this);
		}

		public bool autoExpand
		{
			get
			{
				return this.m_AutoExpand;
			}
			set
			{
				this.m_AutoExpand = value;
				BaseTreeViewController viewController = this.viewController;
				if (viewController != null)
				{
					viewController.RegenerateWrappers();
				}
				base.RefreshItems();
			}
		}

		internal List<int> expandedItemIds
		{
			get
			{
				return this.m_ExpandedItemIds;
			}
			set
			{
				this.m_ExpandedItemIds = value;
			}
		}

		public BaseTreeView()
			: this(-1)
		{
		}

		public BaseTreeView(int itemHeight)
			: base(null, (float)itemHeight)
		{
			this.m_ExpandedItemIds = new List<int>();
			base.AddToClassList(BaseTreeView.ussClassName);
			base.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnTreeViewPointerUp), TrickleDown.TrickleDown);
		}

		public int GetIdForIndex(int index)
		{
			return this.viewController.GetIdForIndex(index);
		}

		public int GetParentIdForIndex(int index)
		{
			return this.viewController.GetParentId(this.GetIdForIndex(index));
		}

		public IEnumerable<int> GetChildrenIdsForIndex(int index)
		{
			return this.viewController.GetChildrenIdsByIndex(index);
		}

		public IEnumerable<TreeViewItemData<T>> GetSelectedItems<T>()
		{
			return this.GetSelectedItemsInternal<T>();
		}

		private protected abstract IEnumerable<TreeViewItemData<T>> GetSelectedItemsInternal<T>();

		public T GetItemDataForIndex<T>(int index)
		{
			return this.GetItemDataForIndexInternal<T>(index);
		}

		private protected abstract T GetItemDataForIndexInternal<T>(int index);

		public T GetItemDataForId<T>(int id)
		{
			return this.GetItemDataForIdInternal<T>(id);
		}

		private protected abstract T GetItemDataForIdInternal<T>(int id);

		public void AddItem<T>(TreeViewItemData<T> item, int parentId = -1, int childIndex = -1, bool rebuildTree = true)
		{
			this.AddItemInternal<T>(item, parentId, childIndex, rebuildTree);
		}

		private protected abstract void AddItemInternal<T>(TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree);

		public bool TryRemoveItem(int id)
		{
			bool flag = this.viewController.TryRemoveItem(id, true);
			bool flag2;
			if (flag)
			{
				base.RefreshItems();
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			bool flag = this.viewController != null;
			if (flag)
			{
				this.viewController.RebuildTree();
				base.RefreshItems();
			}
		}

		private protected override bool HandleItemNavigation(bool moveIn, bool altPressed)
		{
			int num = 1;
			bool flag = false;
			foreach (int num2 in base.selectedIds)
			{
				int indexForId = this.viewController.GetIndexForId(num2);
				bool flag2 = !this.viewController.HasChildrenByIndex(indexForId);
				if (flag2)
				{
					break;
				}
				bool flag3 = moveIn && !this.IsExpandedByIndex(indexForId);
				if (flag3)
				{
					this.ExpandItemByIndex(indexForId, altPressed);
					flag = true;
				}
				else
				{
					bool flag4 = !moveIn && this.IsExpandedByIndex(indexForId);
					if (flag4)
					{
						this.CollapseItemByIndex(indexForId, altPressed);
						flag = true;
					}
				}
			}
			bool flag5 = flag;
			bool flag6;
			if (flag5)
			{
				flag6 = true;
			}
			else
			{
				bool flag7 = !moveIn;
				if (flag7)
				{
					int idForIndex = this.viewController.GetIdForIndex(base.selectedIndex);
					int parentId = this.viewController.GetParentId(idForIndex);
					bool flag8 = parentId != -1;
					if (flag8)
					{
						this.SetSelectionById(parentId);
						base.ScrollToItemById(parentId);
						return true;
					}
					num = -1;
				}
				int num3 = base.selectedIndex;
				bool flag9;
				do
				{
					num3 += num;
					flag9 = this.viewController.HasChildrenByIndex(num3);
				}
				while (!flag9 && num3 >= 0 && num3 < this.itemsSource.Count);
				bool flag10 = flag9;
				if (flag10)
				{
					base.SetSelection(num3);
					base.ScrollToItem(num3);
					flag6 = true;
				}
				else
				{
					flag6 = false;
				}
			}
			return flag6;
		}

		public void SetSelectionById(int id)
		{
			this.SetSelectionById(new int[] { id });
		}

		public void SetSelectionById(IEnumerable<int> ids)
		{
			this.SetSelectionInternalById(ids, true);
		}

		public void SetSelectionByIdWithoutNotify(IEnumerable<int> ids)
		{
			this.SetSelectionInternalById(ids, false);
		}

		internal void SetSelectionInternalById(IEnumerable<int> ids, bool sendNotification)
		{
			bool flag = ids == null;
			if (!flag)
			{
				List<int> list = ids.Select<int, int>((int id) => this.GetItemIndex(id, true)).ToList<int>();
				base.SetSelectionInternal(list, sendNotification);
			}
		}

		public void AddToSelectionById(int id)
		{
			int itemIndex = this.GetItemIndex(id, true);
			base.Rebuild();
			base.AddToSelection(itemIndex);
		}

		public void RemoveFromSelectionById(int id)
		{
			int itemIndex = this.GetItemIndex(id, false);
			base.RemoveFromSelection(itemIndex);
		}

		private int GetItemIndex(int id, bool expand = false)
		{
			if (expand)
			{
				for (int num = this.viewController.GetParentId(id); num != -1; num = this.viewController.GetParentId(num))
				{
					bool flag = !this.m_ExpandedItemIds.Contains(num);
					if (flag)
					{
						this.viewController.ExpandItem(num, false, true);
					}
				}
			}
			return this.viewController.GetIndexForId(id);
		}

		internal void CopyExpandedStates(int sourceId, int targetId)
		{
			bool flag = this.IsExpanded(sourceId);
			if (flag)
			{
				this.ExpandItem(targetId, false);
				bool flag2 = this.viewController.HasChildren(sourceId);
				if (flag2)
				{
					bool flag3 = this.viewController.GetChildrenIds(sourceId).Count<int>() != this.viewController.GetChildrenIds(targetId).Count<int>();
					if (flag3)
					{
						Debug.LogWarning("Source and target hierarchies are not the same");
					}
					else
					{
						for (int i = 0; i < this.viewController.GetChildrenIds(sourceId).Count<int>(); i++)
						{
							int num = this.viewController.GetChildrenIds(sourceId).ElementAt<int>(i);
							int num2 = this.viewController.GetChildrenIds(targetId).ElementAt<int>(i);
							this.CopyExpandedStates(num, num2);
						}
					}
				}
			}
			else
			{
				this.CollapseItem(targetId, false);
			}
		}

		public bool IsExpanded(int id)
		{
			return this.viewController.IsExpanded(id);
		}

		public void CollapseItem(int id, bool collapseAllChildren = false)
		{
			this.viewController.CollapseItem(id, collapseAllChildren);
			base.RefreshItems();
		}

		public void ExpandItem(int id, bool expandAllChildren = false)
		{
			this.viewController.ExpandItem(id, expandAllChildren, true);
		}

		public void ExpandRootItems()
		{
			foreach (int num in this.viewController.GetRootItemIds())
			{
				this.viewController.ExpandItem(num, false, false);
			}
			base.RefreshItems();
		}

		public void ExpandAll()
		{
			this.viewController.ExpandAll();
		}

		public void CollapseAll()
		{
			this.viewController.CollapseAll();
		}

		private void OnTreeViewPointerUp(PointerUpEvent evt)
		{
			base.scrollView.contentContainer.Focus();
		}

		private bool IsExpandedByIndex(int index)
		{
			return this.viewController.IsExpandedByIndex(index);
		}

		private void CollapseItemByIndex(int index, bool collapseAll)
		{
			bool flag = !this.viewController.HasChildrenByIndex(index);
			if (!flag)
			{
				this.viewController.CollapseItemByIndex(index, collapseAll);
				base.RefreshItems();
				base.SaveViewData();
			}
		}

		private void ExpandItemByIndex(int index, bool expandAll)
		{
			bool flag = !this.viewController.HasChildrenByIndex(index);
			if (!flag)
			{
				this.viewController.ExpandItemByIndex(index, expandAll, true);
				base.RefreshItems();
				base.SaveViewData();
			}
		}

		public new static readonly string ussClassName = "unity-tree-view";

		public new static readonly string itemUssClassName = BaseTreeView.ussClassName + "__item";

		public static readonly string itemToggleUssClassName = BaseTreeView.ussClassName + "__item-toggle";

		public static readonly string itemIndentsContainerUssClassName = BaseTreeView.ussClassName + "__item-indents";

		public static readonly string itemIndentUssClassName = BaseTreeView.ussClassName + "__item-indent";

		public static readonly string itemContentContainerUssClassName = BaseTreeView.ussClassName + "__item-content";

		private bool m_AutoExpand;

		[SerializeField]
		private List<int> m_ExpandedItemIds;

		public new class UxmlTraits : BaseVerticalCollectionView.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				BaseTreeView baseTreeView = (BaseTreeView)ve;
				baseTreeView.autoExpand = this.m_AutoExpand.GetValueFromBag(bag, cc);
			}

			private readonly UxmlBoolAttributeDescription m_AutoExpand = new UxmlBoolAttributeDescription
			{
				name = "auto-expand",
				defaultValue = false
			};
		}
	}
}
