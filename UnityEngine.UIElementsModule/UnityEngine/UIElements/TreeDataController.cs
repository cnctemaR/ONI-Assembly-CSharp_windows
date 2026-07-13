using System;
using System.Collections.Generic;
using Unity.Hierarchy;

namespace UnityEngine.UIElements
{
	internal sealed class TreeDataController<T>
	{
		public void AddItem(in TreeViewItemData<T> item, HierarchyNode node)
		{
			this.m_NodeToItemDataDictionary.TryAdd(node, item);
		}

		public void RemoveItem(HierarchyNode node)
		{
			this.m_NodeToItemDataDictionary.Remove(node);
		}

		public TreeViewItemData<T> GetTreeItemDataForNode(HierarchyNode node)
		{
			TreeViewItemData<T> treeViewItemData;
			bool flag = this.m_NodeToItemDataDictionary.TryGetValue(node, out treeViewItemData);
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

		public T GetDataForNode(HierarchyNode node)
		{
			TreeViewItemData<T> treeViewItemData;
			bool flag = this.m_NodeToItemDataDictionary.TryGetValue(node, out treeViewItemData);
			T t;
			if (flag)
			{
				t = treeViewItemData.data;
			}
			else
			{
				t = default(T);
			}
			return t;
		}

		internal unsafe void ConvertTreeViewItemDataToHierarchy(IEnumerable<TreeViewItemData<T>> list, Func<HierarchyNode, HierarchyNode> createNode, Action<int, HierarchyNode> updateDictionary)
		{
			bool flag = list == null;
			if (!flag)
			{
				this.m_ItemStack.Clear();
				this.m_NodeStack.Clear();
				IEnumerator<TreeViewItemData<T>> enumerator = list.GetEnumerator();
				HierarchyNode hierarchyNode = *HierarchyNode.Null;
				for (;;)
				{
					bool flag2 = enumerator.MoveNext();
					bool flag3 = !flag2;
					if (flag3)
					{
						bool flag4 = this.m_ItemStack.Count > 0;
						if (!flag4)
						{
							break;
						}
						hierarchyNode = this.m_NodeStack.Pop();
						enumerator = this.m_ItemStack.Pop();
					}
					else
					{
						TreeViewItemData<T> treeViewItemData = enumerator.Current;
						HierarchyNode hierarchyNode2 = createNode(hierarchyNode);
						this.UpdateNodeToDataDictionary(hierarchyNode2, treeViewItemData);
						updateDictionary(treeViewItemData.id, hierarchyNode2);
						bool flag5 = treeViewItemData.children != null && ((IList<TreeViewItemData<T>>)treeViewItemData.children).Count > 0;
						if (flag5)
						{
							this.m_NodeStack.Push(hierarchyNode);
							hierarchyNode = hierarchyNode2;
							this.m_ItemStack.Push(enumerator);
							enumerator = treeViewItemData.children.GetEnumerator();
						}
					}
				}
			}
		}

		internal void UpdateNodeToDataDictionary(HierarchyNode node, TreeViewItemData<T> item)
		{
			this.m_NodeToItemDataDictionary.TryAdd(node, item);
		}

		internal void ClearNodeToDataDictionary()
		{
			this.m_NodeToItemDataDictionary.Clear();
		}

		private Dictionary<HierarchyNode, TreeViewItemData<T>> m_NodeToItemDataDictionary = new Dictionary<HierarchyNode, TreeViewItemData<T>>();

		private Stack<IEnumerator<TreeViewItemData<T>>> m_ItemStack = new Stack<IEnumerator<TreeViewItemData<T>>>();

		private Stack<HierarchyNode> m_NodeStack = new Stack<HierarchyNode>();
	}
}
