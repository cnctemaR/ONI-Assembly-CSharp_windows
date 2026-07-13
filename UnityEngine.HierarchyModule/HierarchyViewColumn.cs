using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	internal sealed class HierarchyViewColumn : Column
	{
		internal HierarchyView View
		{
			get
			{
				return this.m_View;
			}
		}

		public IReadOnlyCollection<HierarchyViewCellDescriptor> CellDescriptors
		{
			get
			{
				return this.m_CellDescriptors;
			}
		}

		public HierarchyViewColumn(HierarchyView view, HierarchyViewColumnDescriptor descriptor)
		{
			this.m_View = view;
			this.Descriptor = descriptor;
			base.name = this.Descriptor.Id;
			base.resizable = true;
			base.stretchable = false;
			base.sortable = false;
			base.title = this.Descriptor.Title;
			bool flag = this.Descriptor.MakeHeader != null;
			if (flag)
			{
				base.makeHeader = (Func<VisualElement>)Delegate.Combine(base.makeHeader, new Func<VisualElement>(this.MakeHeader));
				base.bindHeader = (Action<VisualElement>)Delegate.Combine(base.bindHeader, new Action<VisualElement>(this.BindHeader));
				base.unbindHeader = (Action<VisualElement>)Delegate.Combine(base.unbindHeader, new Action<VisualElement>(this.UnbindHeader));
				base.destroyHeader = (Action<VisualElement>)Delegate.Combine(base.destroyHeader, new Action<VisualElement>(this.DestroyHeader));
			}
			else
			{
				bool flag2 = this.Descriptor.Icon;
				if (flag2)
				{
					base.icon = Background.FromTexture2D(this.Descriptor.Icon);
				}
			}
			bool flag3 = descriptor.DefaultWidth > 0;
			if (flag3)
			{
				base.width = (float)descriptor.DefaultWidth;
			}
			base.makeCell = (Func<VisualElement>)Delegate.Combine(base.makeCell, new Func<VisualElement>(this.MakeCell));
			base.bindCell = (Action<VisualElement, int>)Delegate.Combine(base.bindCell, new Action<VisualElement, int>(this.BindCell));
			base.unbindCell = (Action<VisualElement, int>)Delegate.Combine(base.unbindCell, new Action<VisualElement, int>(this.UnbindCell));
		}

		public void AddCell(HierarchyViewCellDescriptor desc)
		{
			bool flag = !desc.ValidForColumn(this.Descriptor);
			if (flag)
			{
				Debug.LogError("Cannot register Cell: " + desc.ColumnId + " with Column: " + this.Descriptor.Id);
			}
			else
			{
				foreach (HierarchyViewCellDescriptor hierarchyViewCellDescriptor in this.CellDescriptors)
				{
					bool flag2 = hierarchyViewCellDescriptor.HandlerType == desc.HandlerType;
					if (flag2)
					{
						Debug.LogError(string.Format("Cell: for NodeType {0} is already registered.", desc.HandlerType));
						return;
					}
				}
				this.m_CellDescriptors.Add(desc);
			}
		}

		internal void ApplyDefaultColumnProperties()
		{
			bool flag = this.Descriptor.DefaultWidth > 0;
			if (flag)
			{
				HierarchyViewColumn.SetWidth(this, (float)this.Descriptor.DefaultWidth);
			}
			base.visible = this.Descriptor.DefaultVisibility;
		}

		internal static void SetWidth(Column col, float newWidth)
		{
			bool flag = newWidth <= 0f;
			if (!flag)
			{
				col.width = newWidth;
				bool flag2 = col.minWidth.value > newWidth;
				if (flag2)
				{
					col.minWidth = newWidth;
				}
			}
		}

		private VisualElement MakeHeader()
		{
			return this.Descriptor.MakeHeader();
		}

		private void BindHeader(VisualElement header)
		{
			HierarchyViewColumnDescriptor descriptor = this.Descriptor;
			if (descriptor != null)
			{
				descriptor.BindHeader(header, this.m_View);
			}
		}

		private void UnbindHeader(VisualElement header)
		{
			HierarchyViewColumnDescriptor descriptor = this.Descriptor;
			if (descriptor != null)
			{
				descriptor.UnbindHeader(header, this.m_View);
			}
		}

		private void DestroyHeader(VisualElement header)
		{
			HierarchyViewColumnDescriptor descriptor = this.Descriptor;
			if (descriptor != null)
			{
				descriptor.DestroyHeader(header, this.m_View);
			}
		}

		private VisualElement MakeCell()
		{
			return new HierarchyViewCell(this.m_View, this);
		}

		internal void BindColumn(HierarchyView view)
		{
			this.Descriptor.InvokeBindColumn(this, view);
			foreach (HierarchyViewCellDescriptor hierarchyViewCellDescriptor in this.CellDescriptors)
			{
				hierarchyViewCellDescriptor.InvokeBindColumn(this.Descriptor, view);
			}
		}

		internal void UnbindColumn(HierarchyView view)
		{
			foreach (HierarchyViewCellDescriptor hierarchyViewCellDescriptor in this.CellDescriptors)
			{
				hierarchyViewCellDescriptor.InvokeUnbindColumn(this.Descriptor, view);
			}
			bool flag = this.Descriptor.UnbindHeader != null || this.Descriptor.DestroyHeader != null;
			if (flag)
			{
				List<VisualElement> list = view.ListView.Query<VisualElement>(null, "unity-multi-column-header__column").ToList();
				foreach (VisualElement visualElement in list)
				{
					bool flag2 = visualElement.name == this.Descriptor.Id;
					if (flag2)
					{
						VisualElement visualElement2 = visualElement.Q(null, "unity-multi-column-header__column__content");
						Action<VisualElement, HierarchyView> unbindHeader = this.Descriptor.UnbindHeader;
						if (unbindHeader != null)
						{
							unbindHeader(visualElement2, view);
						}
						Action<VisualElement, HierarchyView> destroyHeader = this.Descriptor.DestroyHeader;
						if (destroyHeader != null)
						{
							destroyHeader(visualElement2, view);
						}
						break;
					}
				}
			}
			this.Descriptor.InvokeUnbindColumn(this, view);
		}

		private unsafe void BindCell(VisualElement cellElement, int index)
		{
			HierarchyViewCell hierarchyViewCell = cellElement as HierarchyViewCell;
			bool flag = hierarchyViewCell == null;
			if (!flag)
			{
				HierarchyNode hierarchyNode = *this.m_View.ViewModel[index];
				bool flag2 = (in hierarchyNode) == HierarchyNode.Null;
				if (!flag2)
				{
					bool flag3 = !this.m_View.Source.Exists(in hierarchyNode);
					if (!flag3)
					{
						hierarchyViewCell.Node = hierarchyNode;
						hierarchyViewCell.NodeIndex = index;
						hierarchyViewCell.Handler = this.m_View.Source.GetNodeTypeHandler(in hierarchyNode);
						bool flag4 = hierarchyViewCell.Handler == null;
						if (!flag4)
						{
							foreach (HierarchyViewCellDescriptor hierarchyViewCellDescriptor in this.CellDescriptors)
							{
								bool flag5 = hierarchyViewCellDescriptor.HandlerType == null || hierarchyViewCellDescriptor.HandlerType == hierarchyViewCell.Handler.GetType();
								if (flag5)
								{
									hierarchyViewCell.Descriptor = hierarchyViewCellDescriptor;
									break;
								}
							}
							bool flag6 = hierarchyViewCell.Descriptor == null;
							if (!flag6)
							{
								hierarchyViewCell.BindCell();
							}
						}
					}
				}
			}
		}

		private void UnbindCell(VisualElement cellElement, int index)
		{
			HierarchyViewCell hierarchyViewCell = cellElement as HierarchyViewCell;
			bool flag = hierarchyViewCell == null;
			if (!flag)
			{
				hierarchyViewCell.UnbindCell();
			}
		}

		public override string ToString()
		{
			return this.Descriptor.ToString();
		}

		internal const string k_NonDefaultValue = "non-default-value";

		private readonly HierarchyView m_View;

		private readonly List<HierarchyViewCellDescriptor> m_CellDescriptors = new List<HierarchyViewCellDescriptor>();

		public readonly HierarchyViewColumnDescriptor Descriptor;
	}
}
