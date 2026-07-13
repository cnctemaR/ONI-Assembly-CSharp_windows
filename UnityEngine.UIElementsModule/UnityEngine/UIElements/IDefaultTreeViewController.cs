using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal interface IDefaultTreeViewController<T>
	{
		void SetRootItems(IList<TreeViewItemData<T>> items);

		void AddItem(in TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree = true);

		TreeViewItemData<T> GetTreeViewItemDataForId(int id);

		TreeViewItemData<T> GetTreeViewItemDataForIndex(int index);

		T GetDataForId(int id);

		T GetDataForIndex(int index);
	}
}
