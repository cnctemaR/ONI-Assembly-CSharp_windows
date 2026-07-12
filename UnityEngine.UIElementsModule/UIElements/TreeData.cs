using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal readonly struct TreeData<T>
	{
		public IEnumerable<int> rootItemIds
		{
			get
			{
				return this.m_RootItemIds;
			}
		}

		public TreeData(IList<TreeViewItemData<T>> rootItems)
		{
			this.m_RootItemIds = new List<int>();
			this.m_Tree = new Dictionary<int, TreeViewItemData<T>>();
			this.m_ParentIds = new Dictionary<int, int>();
			this.m_ChildrenIds = new Dictionary<int, List<int>>();
			this.RefreshTree(rootItems);
		}

		public TreeViewItemData<T> GetDataForId(int id)
		{
			TreeViewItemData<T> treeViewItemData;
			bool flag = this.m_Tree.TryGetValue(id, out treeViewItemData);
			TreeViewItemData<T> treeViewItemData2;
			if (flag)
			{
				treeViewItemData2 = treeViewItemData;
			}
			else
			{
				treeViewItemData2 = default(TreeViewItemData<T>);
			}
			return treeViewItemData2;
		}

		public int GetParentId(int id)
		{
			int num;
			bool flag = this.m_ParentIds.TryGetValue(id, out num);
			int num2;
			if (flag)
			{
				num2 = num;
			}
			else
			{
				num2 = -1;
			}
			return num2;
		}

		public void AddItem(TreeViewItemData<T> item, int parentId, int childIndex)
		{
			List<TreeViewItemData<T>> list = CollectionPool<List<TreeViewItemData<T>>, TreeViewItemData<T>>.Get();
			list.Add(item);
			this.BuildTree(list, false);
			this.AddItemToParent(item, parentId, childIndex);
			CollectionPool<List<TreeViewItemData<T>>, TreeViewItemData<T>>.Release(list);
		}

		public bool TryRemove(int id)
		{
			int num;
			bool flag = this.m_ParentIds.TryGetValue(id, out num);
			if (flag)
			{
				this.RemoveFromParent(id, num);
			}
			else
			{
				this.m_RootItemIds.Remove(id);
			}
			return this.TryRemoveChildrenIds(id);
		}

		public void Move(int id, int newParentId, int childIndex)
		{
			TreeViewItemData<T> treeViewItemData;
			bool flag = !this.m_Tree.TryGetValue(id, out treeViewItemData);
			if (!flag)
			{
				int num;
				bool flag2 = this.m_ParentIds.TryGetValue(id, out num);
				if (flag2)
				{
					bool flag3 = num == newParentId;
					if (flag3)
					{
						int childIndex2 = this.m_Tree[num].GetChildIndex(id);
						bool flag4 = childIndex2 < childIndex;
						if (flag4)
						{
							childIndex--;
						}
					}
					this.RemoveFromParent(treeViewItemData.id, num);
				}
				else
				{
					int num2 = this.m_RootItemIds.IndexOf(id);
					bool flag5 = newParentId == -1 && num2 < childIndex;
					if (flag5)
					{
						childIndex--;
					}
					this.m_RootItemIds.Remove(id);
				}
				this.AddItemToParent(treeViewItemData, newParentId, childIndex);
			}
		}

		public bool HasAncestor(int childId, int ancestorId)
		{
			bool flag = childId == -1 || ancestorId == -1;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				int num = childId;
				int parentId;
				while ((parentId = this.GetParentId(num)) != -1)
				{
					bool flag3 = ancestorId == parentId;
					if (flag3)
					{
						return true;
					}
					num = parentId;
				}
				flag2 = false;
			}
			return flag2;
		}

		private void AddItemToParent(TreeViewItemData<T> item, int parentId, int childIndex)
		{
			bool flag = parentId == -1;
			if (flag)
			{
				this.m_ParentIds.Remove(item.id);
				bool flag2 = childIndex < 0 || childIndex >= this.m_RootItemIds.Count;
				if (flag2)
				{
					this.m_RootItemIds.Add(item.id);
				}
				else
				{
					this.m_RootItemIds.Insert(childIndex, item.id);
				}
			}
			else
			{
				TreeViewItemData<T> treeViewItemData = this.m_Tree[parentId];
				treeViewItemData.InsertChild(item, childIndex);
				this.m_Tree[parentId] = treeViewItemData;
				this.m_ParentIds[item.id] = parentId;
				this.UpdateParentTree(treeViewItemData);
			}
		}

		private void RemoveFromParent(int id, int parentId)
		{
			TreeViewItemData<T> treeViewItemData = this.m_Tree[parentId];
			treeViewItemData.RemoveChild(id);
			this.m_Tree[parentId] = treeViewItemData;
			List<int> list;
			bool flag = this.m_ChildrenIds.TryGetValue(parentId, out list);
			if (flag)
			{
				list.Remove(id);
			}
			this.UpdateParentTree(treeViewItemData);
		}

		private void UpdateParentTree(TreeViewItemData<T> current)
		{
			for (;;)
			{
				int num;
				bool flag = this.m_ParentIds.TryGetValue(current.id, out num);
				if (!flag)
				{
					break;
				}
				TreeViewItemData<T> treeViewItemData = this.m_Tree[num];
				treeViewItemData.ReplaceChild(current);
				this.m_Tree[num] = treeViewItemData;
				current = treeViewItemData;
			}
		}

		private bool TryRemoveChildrenIds(int id)
		{
			TreeViewItemData<T> treeViewItemData;
			bool flag = this.m_Tree.TryGetValue(id, out treeViewItemData) && treeViewItemData.children != null;
			if (flag)
			{
				foreach (TreeViewItemData<T> treeViewItemData2 in treeViewItemData.children)
				{
					this.TryRemoveChildrenIds(treeViewItemData2.id);
				}
			}
			List<int> list;
			bool flag2 = this.m_ChildrenIds.TryGetValue(id, out list);
			if (flag2)
			{
				CollectionPool<List<int>, int>.Release(list);
			}
			bool flag3 = false;
			flag3 |= this.m_RootItemIds.Remove(id);
			flag3 |= this.m_ChildrenIds.Remove(id);
			flag3 |= this.m_ParentIds.Remove(id);
			flag3 |= this.m_Tree.Remove(id);
			return flag3 | this.m_RootItemIds.Remove(id);
		}

		private void RefreshTree(IList<TreeViewItemData<T>> rootItems)
		{
			this.m_Tree.Clear();
			this.m_ParentIds.Clear();
			this.m_ChildrenIds.Clear();
			this.m_RootItemIds.Clear();
			this.BuildTree(rootItems, true);
		}

		private void BuildTree(IEnumerable<TreeViewItemData<T>> items, bool isRoot)
		{
			bool flag = items == null;
			if (!flag)
			{
				foreach (TreeViewItemData<T> treeViewItemData in items)
				{
					this.m_Tree.Add(treeViewItemData.id, treeViewItemData);
					if (isRoot)
					{
						this.m_RootItemIds.Add(treeViewItemData.id);
					}
					bool flag2 = treeViewItemData.children != null;
					if (flag2)
					{
						List<int> list;
						bool flag3 = !this.m_ChildrenIds.TryGetValue(treeViewItemData.id, out list);
						if (flag3)
						{
							this.m_ChildrenIds.Add(treeViewItemData.id, list = CollectionPool<List<int>, int>.Get());
						}
						foreach (TreeViewItemData<T> treeViewItemData2 in treeViewItemData.children)
						{
							this.m_ParentIds.Add(treeViewItemData2.id, treeViewItemData.id);
							list.Add(treeViewItemData2.id);
						}
						this.BuildTree(treeViewItemData.children, false);
					}
				}
			}
		}

		private readonly IList<int> m_RootItemIds;

		private readonly Dictionary<int, TreeViewItemData<T>> m_Tree;

		private readonly Dictionary<int, int> m_ParentIds;

		private readonly Dictionary<int, List<int>> m_ChildrenIds;
	}
}
