using System;
using System.Collections.Generic;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements
{
	public class MultiColumnListViewController : BaseListViewController
	{
		public MultiColumnController columnController
		{
			get
			{
				return this.m_ColumnController;
			}
		}

		internal MultiColumnCollectionHeader header
		{
			get
			{
				MultiColumnController columnController = this.m_ColumnController;
				return (columnController != null) ? columnController.header : null;
			}
		}

		public MultiColumnListViewController(Columns columns, SortColumnDescriptions sortDescriptions, List<SortColumnDescription> sortedColumns)
		{
			this.m_ColumnController = new MultiColumnController(columns, sortDescriptions, sortedColumns);
			base.itemsSourceSizeChanged += this.SortIfNeeded;
			base.itemsSourceChanged += this.SortIfNeeded;
		}

		internal override void PreRefresh()
		{
			base.PreRefresh();
			this.m_ColumnController.SortIfNeeded();
		}

		private void SortIfNeeded()
		{
			this.m_ColumnController.UpdateDragger();
			bool flag = this.m_ColumnController.sortingMode == ColumnSortingMode.Default;
			if (flag)
			{
				base.view.RefreshItems();
			}
		}

		internal override void InvokeMakeItem(ReusableCollectionItem reusableItem)
		{
			ReusableMultiColumnListViewItem reusableMultiColumnListViewItem = reusableItem as ReusableMultiColumnListViewItem;
			bool flag = reusableMultiColumnListViewItem != null;
			if (flag)
			{
				reusableMultiColumnListViewItem.Init(this.MakeItem(), this.m_ColumnController.header.columns, base.baseListView.reorderMode == ListViewReorderMode.Animated);
				base.PostInitRegistration(reusableMultiColumnListViewItem);
			}
			else
			{
				base.InvokeMakeItem(reusableItem);
			}
		}

		internal override void InvokeBindItem(ReusableCollectionItem reusableItem, int index)
		{
			base.InvokeBindItem(reusableItem, index);
			ReusableListViewItem reusableListViewItem = reusableItem as ReusableListViewItem;
			bool flag = reusableListViewItem != null;
			if (flag)
			{
				bool flag2 = this.m_ColumnController.header.sortingEnabled && this.m_ColumnController.header.sortedColumnReadonly.Count > 0;
				reusableListViewItem.SetDragHandleEnabled(!flag2);
			}
		}

		public override object GetItemForIndex(int index)
		{
			int sourceIndex = this.columnController.GetSourceIndex(index);
			return base.GetItemForIndex(sourceIndex);
		}

		public override int GetIndexForId(int id)
		{
			int indexForId = base.GetIndexForId(id);
			return this.columnController.GetSortedIndex(indexForId);
		}

		public override int GetIdForIndex(int index)
		{
			int sourceIndex = this.columnController.GetSourceIndex(index);
			return base.GetIdForIndex(sourceIndex);
		}

		protected override VisualElement MakeItem()
		{
			return this.m_ColumnController.MakeItem();
		}

		protected override void BindItem(VisualElement element, int index)
		{
			this.m_ColumnController.BindItem<object>(element, index, this.GetItemForIndex(index));
		}

		protected override void UnbindItem(VisualElement element, int index)
		{
			this.m_ColumnController.UnbindItem(element, index);
		}

		protected override void DestroyItem(VisualElement element)
		{
			this.m_ColumnController.DestroyItem(element);
		}

		protected override void PrepareView()
		{
			this.m_ColumnController.PrepareView(base.view);
			base.baseListView.reorderModeChanged += this.UpdateReorderClassList;
		}

		public override void Dispose()
		{
			base.baseListView.reorderModeChanged -= this.UpdateReorderClassList;
			this.m_ColumnController.Dispose();
			this.m_ColumnController = null;
			base.Dispose();
		}

		private void UpdateReorderClassList()
		{
			this.m_ColumnController.header.EnableInClassList(MultiColumnCollectionHeader.reorderableUssClassName, base.baseListView.reorderable && base.baseListView.reorderMode == ListViewReorderMode.Animated);
		}

		private MultiColumnController m_ColumnController;
	}
}
