using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	internal sealed class HierarchyViewColumnDescriptor
	{
		internal void InvokeBindColumn(HierarchyViewColumn column, HierarchyView view)
		{
			bool isBound = this.m_IsBound;
			if (!isBound)
			{
				Action<HierarchyViewColumn, HierarchyView> bindColumn = this.BindColumn;
				if (bindColumn != null)
				{
					bindColumn(column, view);
				}
				this.m_IsBound = true;
			}
		}

		internal void InvokeUnbindColumn(HierarchyViewColumn column, HierarchyView view)
		{
			bool flag = !this.m_IsBound;
			if (!flag)
			{
				Action<HierarchyViewColumn, HierarchyView> unbindColumn = this.UnbindColumn;
				if (unbindColumn != null)
				{
					unbindColumn(column, view);
				}
				this.m_IsBound = false;
			}
		}

		public HierarchyViewColumnDescriptor(string columnId)
		{
			this.Id = columnId;
		}

		public override string ToString()
		{
			return this.Id;
		}

		private bool m_IsBound;

		public readonly string Id;

		public string Title;

		public Texture2D Icon;

		public string Tooltip;

		public int DefaultPriority;

		public int DefaultWidth = -1;

		public bool DefaultVisibility;

		public object UserData;

		public Func<VisualElement> MakeHeader;

		public Action<VisualElement, HierarchyView> BindHeader;

		public Action<VisualElement, HierarchyView> UnbindHeader;

		public Action<VisualElement, HierarchyView> DestroyHeader;

		public Action<HierarchyViewColumn, HierarchyView> BindColumn;

		public Action<HierarchyViewColumn, HierarchyView> UnbindColumn;
	}
}
