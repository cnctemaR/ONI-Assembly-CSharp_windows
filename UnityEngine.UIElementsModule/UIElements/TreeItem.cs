using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	internal readonly struct TreeItem
	{
		public int id { get; }

		public int parentId { get; }

		public IEnumerable<int> childrenIds { get; }

		public bool hasChildren
		{
			get
			{
				return this.childrenIds != null && this.childrenIds.Any<int>();
			}
		}

		public TreeItem(int id, int parentId = -1, IEnumerable<int> childrenIds = null)
		{
			this.id = id;
			this.parentId = parentId;
			this.childrenIds = childrenIds;
		}

		public const int invalidId = -1;
	}
}
