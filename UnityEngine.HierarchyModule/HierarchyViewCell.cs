using System;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal sealed class HierarchyViewCell : VisualElement
	{
		internal void BindCell()
		{
			bool isCellBound = this.m_IsCellBound;
			if (!isCellBound)
			{
				HierarchyViewCellDescriptor descriptor = this.Descriptor;
				if (descriptor != null)
				{
					Action<HierarchyViewCell> bindCell = descriptor.BindCell;
					if (bindCell != null)
					{
						bindCell(this);
					}
				}
				this.m_IsCellBound = true;
			}
		}

		internal unsafe void UnbindCell()
		{
			bool flag = !this.m_IsCellBound;
			if (!flag)
			{
				bool flag2 = this.Descriptor != null;
				if (flag2)
				{
					HierarchyViewCellDescriptor descriptor = this.Descriptor;
					if (descriptor != null)
					{
						Action<HierarchyViewCell> unbindCell = descriptor.UnbindCell;
						if (unbindCell != null)
						{
							unbindCell(this);
						}
					}
					bool clearCellContent = this.Descriptor.ClearCellContent;
					if (clearCellContent)
					{
						base.Clear();
					}
				}
				this.BoundObject = null;
				this.Node = *HierarchyNode.Null;
				this.NodeIndex = -1;
				this.Handler = null;
				this.Descriptor = null;
				this.m_IsCellBound = false;
			}
		}

		public HierarchyNodeTypeHandler Handler { get; internal set; }

		public HierarchyNode Node { get; internal set; }

		public int NodeIndex { get; internal set; }

		public HierarchyViewCellDescriptor Descriptor { get; internal set; }

		public object BoundObject { get; set; }

		public bool IsDefaultValue
		{
			get
			{
				return this.m_IsDefaultValue;
			}
			set
			{
				if (value)
				{
					base.RemoveFromClassList("non-default-value");
				}
				else
				{
					base.AddToClassList("non-default-value");
				}
				this.m_IsDefaultValue = value;
			}
		}

		internal HierarchyViewCell(HierarchyView view, HierarchyViewColumn column)
		{
			this.View = view;
			this.Column = column;
			base.name = "HierarchyViewCell";
		}

		public override string ToString()
		{
			string id = this.Column.Descriptor.Id;
			bool flag = this.Descriptor != null;
			string text;
			if (flag)
			{
				text = this.Descriptor.ToString();
			}
			else
			{
				text = string.Format("{0} - NoCellDesc", this.Column.Descriptor);
			}
			return text;
		}

		private bool m_IsCellBound;

		private bool m_IsDefaultValue;

		public readonly HierarchyViewColumn Column;

		public readonly HierarchyView View;
	}
}
