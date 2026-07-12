using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal readonly struct TreeViewItemWrapper
	{
		public int id
		{
			get
			{
				return this.item.id;
			}
		}

		public int parentId
		{
			get
			{
				return this.item.parentId;
			}
		}

		public IEnumerable<int> childrenIds
		{
			get
			{
				return this.item.childrenIds;
			}
		}

		public bool hasChildren
		{
			get
			{
				return this.item.hasChildren;
			}
		}

		public TreeViewItemWrapper(TreeItem item, int depth)
		{
			this.item = item;
			this.depth = depth;
		}

		public readonly TreeItem item;

		public readonly int depth;
	}
}
