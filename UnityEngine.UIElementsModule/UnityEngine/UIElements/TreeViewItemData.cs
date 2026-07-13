using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public readonly struct TreeViewItemData<T>
	{
		public int id { get; }

		public T data
		{
			get
			{
				return this.m_Data;
			}
		}

		public IEnumerable<TreeViewItemData<T>> children
		{
			get
			{
				return this.m_Children;
			}
		}

		public bool hasChildren
		{
			get
			{
				return this.m_Children != null && this.m_Children.Count > 0;
			}
		}

		public TreeViewItemData(int id, T data, List<TreeViewItemData<T>> children = null)
		{
			this.id = id;
			this.m_Data = data;
			this.m_Children = children ?? new List<TreeViewItemData<T>>();
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void AddChild(TreeViewItemData<T> child)
		{
			this.m_Children.Add(child);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal void AddChildren(IList<TreeViewItemData<T>> children)
		{
			foreach (TreeViewItemData<T> treeViewItemData in children)
			{
				this.AddChild(treeViewItemData);
			}
		}

		internal void InsertChild(TreeViewItemData<T> child, int index)
		{
			bool flag = index < 0 || index >= this.m_Children.Count;
			if (flag)
			{
				this.m_Children.Add(child);
			}
			else
			{
				this.m_Children.Insert(index, child);
			}
		}

		internal void RemoveChild(int childId)
		{
			bool flag = this.m_Children == null;
			if (!flag)
			{
				for (int i = 0; i < this.m_Children.Count; i++)
				{
					bool flag2 = childId == this.m_Children[i].id;
					if (flag2)
					{
						this.m_Children.RemoveAt(i);
						break;
					}
				}
			}
		}

		internal int GetChildIndex(int itemId)
		{
			int num = 0;
			foreach (TreeViewItemData<T> treeViewItemData in this.m_Children)
			{
				bool flag = treeViewItemData.id == itemId;
				if (flag)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		internal void ReplaceChild(TreeViewItemData<T> newChild)
		{
			bool flag = !this.hasChildren;
			if (!flag)
			{
				int num = 0;
				foreach (TreeViewItemData<T> treeViewItemData in this.m_Children)
				{
					bool flag2 = treeViewItemData.id == newChild.id;
					if (flag2)
					{
						this.m_Children.RemoveAt(num);
						this.m_Children.Insert(num, newChild);
						break;
					}
					num++;
				}
			}
		}

		[CreateProperty]
		private readonly T m_Data;

		private readonly IList<TreeViewItemData<T>> m_Children;
	}
}
