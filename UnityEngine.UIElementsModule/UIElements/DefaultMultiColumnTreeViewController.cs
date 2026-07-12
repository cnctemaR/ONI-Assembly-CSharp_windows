using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class DefaultMultiColumnTreeViewController<T> : MultiColumnTreeViewController, IDefaultTreeViewController<T>
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

		public DefaultMultiColumnTreeViewController(Columns columns, SortColumnDescriptions sortDescriptions, List<SortColumnDescription> sortedColumns)
			: base(columns, sortDescriptions, sortedColumns)
		{
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
				this.treeDataController.SetRootItems(items);
				base.RebuildTree();
				base.RaiseItemsSourceChanged();
			}
		}

		public virtual void AddItem(in TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree = true)
		{
			this.treeDataController.AddItem(in item, parentId, childIndex);
			if (rebuildTree)
			{
				base.RebuildTree();
			}
		}

		public virtual TreeViewItemData<T> GetTreeViewItemDataForId(int id)
		{
			return this.treeDataController.GetTreeItemDataForId(id);
		}

		public virtual TreeViewItemData<T> GetTreeViewItemDataForIndex(int index)
		{
			int idForIndex = this.GetIdForIndex(index);
			return this.treeDataController.GetTreeItemDataForId(idForIndex);
		}

		public override bool TryRemoveItem(int id, bool rebuildTree = true)
		{
			bool flag = this.treeDataController.TryRemoveItem(id);
			bool flag2;
			if (flag)
			{
				if (rebuildTree)
				{
					base.RebuildTree();
				}
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		public T GetDataForId(int id)
		{
			return this.treeDataController.GetDataForId(id);
		}

		public T GetDataForIndex(int index)
		{
			return this.treeDataController.GetDataForId(this.GetIdForIndex(index));
		}

		public override object GetItemForIndex(int index)
		{
			return this.treeDataController.GetDataForId(this.GetIdForIndex(index));
		}

		public override int GetParentId(int id)
		{
			return this.treeDataController.GetParentId(id);
		}

		public override bool HasChildren(int id)
		{
			return this.treeDataController.HasChildren(id);
		}

		public override IEnumerable<int> GetChildrenIds(int id)
		{
			return this.treeDataController.GetChildrenIds(id);
		}

		public override void Move(int id, int newParentId, int childIndex = -1, bool rebuildTree = true)
		{
			bool flag = id == newParentId;
			if (!flag)
			{
				bool flag2 = this.IsChildOf(newParentId, id);
				if (!flag2)
				{
					this.treeDataController.Move(id, newParentId, childIndex);
					if (rebuildTree)
					{
						base.RebuildTree();
						base.RaiseItemIndexChanged(id, newParentId);
					}
				}
			}
		}

		private bool IsChildOf(int childId, int id)
		{
			return this.treeDataController.IsChildOf(childId, id);
		}

		public override IEnumerable<int> GetAllItemIds(IEnumerable<int> rootIds = null)
		{
			return this.treeDataController.GetAllItemIds(rootIds);
		}

		private TreeDataController<T> m_TreeDataController;
	}
}
