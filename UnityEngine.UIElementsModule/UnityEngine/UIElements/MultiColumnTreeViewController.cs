using System;
using System.Collections.Generic;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements
{
	public abstract class MultiColumnTreeViewController : BaseTreeViewController
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

		protected MultiColumnTreeViewController(Columns columns, SortColumnDescriptions sortDescriptions, List<SortColumnDescription> sortedColumns)
		{
			this.m_ColumnController = new MultiColumnController(columns, sortDescriptions, sortedColumns);
		}

		internal override void PreRefresh()
		{
			base.PreRefresh();
			this.m_ColumnController.SortIfNeeded();
		}

		internal override void InvokeMakeItem(ReusableCollectionItem reusableItem)
		{
			ReusableMultiColumnTreeViewItem reusableMultiColumnTreeViewItem = reusableItem as ReusableMultiColumnTreeViewItem;
			bool flag = reusableMultiColumnTreeViewItem != null;
			if (flag)
			{
				reusableMultiColumnTreeViewItem.Init(this.MakeItem(), this.m_ColumnController.header.columns);
				base.PostInitRegistration(reusableMultiColumnTreeViewItem);
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
		}

		public override void Dispose()
		{
			this.m_ColumnController.Dispose();
			this.m_ColumnController = null;
			base.Dispose();
		}

		private MultiColumnController m_ColumnController;
	}
}
