using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal sealed class TreeDataController<T>
	{
		public void SetRootItems(IList<TreeViewItemData<T>> rootItems)
		{
			this.m_TreeData = new TreeData<T>(rootItems);
		}

		public void AddItem(in TreeViewItemData<T> item, int parentId, int childIndex)
		{
			this.m_TreeData.AddItem(item, parentId, childIndex);
		}

		public bool TryRemoveItem(int id)
		{
			return this.m_TreeData.TryRemove(id);
		}

		public TreeViewItemData<T> GetTreeItemDataForId(int id)
		{
			return this.m_TreeData.GetDataForId(id);
		}

		public T GetDataForId(int id)
		{
			return this.m_TreeData.GetDataForId(id).data;
		}

		public int GetParentId(int id)
		{
			return this.m_TreeData.GetParentId(id);
		}

		public bool HasChildren(int id)
		{
			return this.m_TreeData.GetDataForId(id).hasChildren;
		}

		private static IEnumerable<int> GetItemIds(IEnumerable<TreeViewItemData<T>> items)
		{
			bool flag = items == null;
			if (flag)
			{
				yield break;
			}
			foreach (TreeViewItemData<T> item in items)
			{
				yield return item.id;
				item = default(TreeViewItemData<T>);
			}
			IEnumerator<TreeViewItemData<T>> enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<int> GetChildrenIds(int id)
		{
			return TreeDataController<T>.GetItemIds(this.m_TreeData.GetDataForId(id).children);
		}

		public void Move(int id, int newParentId, int childIndex = -1)
		{
			bool flag = id == newParentId;
			if (!flag)
			{
				bool flag2 = this.IsChildOf(newParentId, id);
				if (!flag2)
				{
					this.m_TreeData.Move(id, newParentId, childIndex);
				}
			}
		}

		public bool IsChildOf(int childId, int id)
		{
			return this.m_TreeData.HasAncestor(childId, id);
		}

		public IEnumerable<int> GetAllItemIds(IEnumerable<int> rootIds = null)
		{
			this.m_IteratorStack.Clear();
			bool flag = rootIds == null;
			if (flag)
			{
				bool flag2 = this.m_TreeData.rootItemIds == null;
				if (flag2)
				{
					yield break;
				}
				rootIds = this.m_TreeData.rootItemIds;
			}
			IEnumerator<int> currentIterator = rootIds.GetEnumerator();
			for (;;)
			{
				bool hasNext = currentIterator.MoveNext();
				bool flag3 = !hasNext;
				if (flag3)
				{
					bool flag4 = this.m_IteratorStack.Count > 0;
					if (!flag4)
					{
						break;
					}
					currentIterator = this.m_IteratorStack.Pop();
				}
				else
				{
					int currentItemId = currentIterator.Current;
					yield return currentItemId;
					bool flag5 = this.HasChildren(currentItemId);
					if (flag5)
					{
						this.m_IteratorStack.Push(currentIterator);
						currentIterator = this.GetChildrenIds(currentItemId).GetEnumerator();
					}
				}
			}
			yield break;
		}

		private TreeData<T> m_TreeData;

		private Stack<IEnumerator<int>> m_IteratorStack = new Stack<IEnumerator<int>>();
	}
}
