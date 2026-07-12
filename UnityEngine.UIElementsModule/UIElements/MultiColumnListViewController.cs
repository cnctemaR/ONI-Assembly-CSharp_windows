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
