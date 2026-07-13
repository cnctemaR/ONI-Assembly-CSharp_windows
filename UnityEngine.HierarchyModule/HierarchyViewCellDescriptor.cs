using System;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal sealed class HierarchyViewCellDescriptor
	{
		public HierarchyViewCellDescriptor(string columnId, Type handlerType = null)
		{
			this.ColumnId = columnId;
			this.HandlerType = handlerType;
			this.ClearCellContent = true;
		}

		internal void InvokeBindColumn(HierarchyViewColumnDescriptor descriptor, HierarchyView view)
		{
			bool isColumnBound = this.m_IsColumnBound;
			if (!isColumnBound)
			{
				Action<HierarchyViewColumnDescriptor, HierarchyView> bindColumn = this.BindColumn;
				if (bindColumn != null)
				{
					bindColumn(descriptor, view);
				}
				this.m_IsColumnBound = true;
			}
		}

		internal void InvokeUnbindColumn(HierarchyViewColumnDescriptor descriptor, HierarchyView view)
		{
			bool flag = !this.m_IsColumnBound;
			if (!flag)
			{
				Action<HierarchyViewColumnDescriptor, HierarchyView> unbindColumn = this.UnbindColumn;
				if (unbindColumn != null)
				{
					unbindColumn(descriptor, view);
				}
				this.m_IsColumnBound = false;
			}
		}

		public bool ValidForColumn(HierarchyViewColumnDescriptor colDesc)
		{
			return this.ColumnId == colDesc.Id;
		}

		public override string ToString()
		{
			string text = ((this.HandlerType != null) ? this.HandlerType.Name : "<GenericNodeHandler>");
			return this.ColumnId + " - " + text;
		}

		private bool m_IsColumnBound;

		public readonly string ColumnId;

		public readonly Type HandlerType;

		public Action<HierarchyViewCell> BindCell;

		public Action<HierarchyViewCell> UnbindCell;

		public Action<HierarchyViewColumnDescriptor, HierarchyView> BindColumn;

		public Action<HierarchyViewColumnDescriptor, HierarchyView> UnbindColumn;

		public bool ClearCellContent;

		public object UserData;
	}
}
