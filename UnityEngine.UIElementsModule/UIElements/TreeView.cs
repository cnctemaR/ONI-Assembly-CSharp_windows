using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class TreeView : BaseTreeView
	{
		public new Func<VisualElement> makeItem
		{
			get
			{
				return this.m_MakeItem;
			}
			set
			{
				bool flag = value != this.m_MakeItem;
				if (flag)
				{
					this.m_MakeItem = value;
					base.Rebuild();
				}
			}
		}

		public new Action<VisualElement, int> bindItem
		{
			get
			{
				return this.m_BindItem;
			}
			set
			{
				bool flag = value != this.m_BindItem;
				if (flag)
				{
					this.m_BindItem = value;
					base.RefreshItems();
				}
			}
		}

		public new Action<VisualElement, int> unbindItem { get; set; }

		public new Action<VisualElement> destroyItem { get; set; }

		internal override void SetRootItemsInternal<T>(IList<TreeViewItemData<T>> rootItems)
		{
			TreeViewHelpers<T, DefaultTreeViewController<T>>.SetRootItems(this, rootItems, () => new DefaultTreeViewController<T>());
		}

		internal override bool HasValidDataAndBindings()
		{
			return base.HasValidDataAndBindings() && this.makeItem != null == (this.bindItem != null);
		}

		public new TreeViewController viewController
		{
			get
			{
				return base.viewController as TreeViewController;
			}
		}

		protected override CollectionViewController CreateViewController()
		{
			return new DefaultTreeViewController<object>();
		}

		public TreeView()
			: this(null, null)
		{
		}

		public TreeView(Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
			: base(-1)
		{
			this.makeItem = makeItem;
			this.bindItem = bindItem;
		}

		public TreeView(int itemHeight, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
			: this(makeItem, bindItem)
		{
			base.fixedItemHeight = (float)itemHeight;
		}

		private protected override IEnumerable<TreeViewItemData<T>> GetSelectedItemsInternal<T>()
		{
			return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetSelectedItems(this);
		}

		private protected override T GetItemDataForIndexInternal<T>(int index)
		{
			return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetItemDataForIndex(this, index);
		}

		private protected override T GetItemDataForIdInternal<T>(int id)
		{
			return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetItemDataForId(this, id);
		}

		private protected override void AddItemInternal<T>(TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree)
		{
			TreeViewHelpers<T, DefaultTreeViewController<T>>.AddItem(this, item, parentId, childIndex, rebuildTree);
		}

		private Func<VisualElement> m_MakeItem;

		private Action<VisualElement, int> m_BindItem;

		public new class UxmlFactory : UxmlFactory<TreeView, TreeView.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseTreeView.UxmlTraits
		{
		}
	}
}
