using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class MultiColumnLayoutConfiguration : CollectionViewLayoutConfiguration
	{
		internal MultiColumnCollectionHeader header
		{
			get
			{
				return this.m_MultiColumnHeader;
			}
		}

		public VisualElement headerContainer
		{
			get
			{
				return this.m_HeaderContainer;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ContextualMenuPopulateEvent, Column> headerContextMenuPopulateEvent;

		public MultiColumnLayoutConfiguration()
		{
			this.columns = new Columns();
			base.makeCell = (Func<VisualElement>)Delegate.Combine(base.makeCell, new Func<VisualElement>(this.MakeCell));
			base.bindCell = (Action<VisualElement, int>)Delegate.Combine(base.bindCell, new Action<VisualElement, int>(this.BindCell));
			base.unbindCell = (Action<VisualElement, int>)Delegate.Combine(base.unbindCell, new Action<VisualElement, int>(this.UnbindCell));
			base.destroyCell = (Action<VisualElement>)Delegate.Combine(base.destroyCell, new Action<VisualElement>(this.DestroyCell));
		}

		[CreateProperty]
		public Columns columns
		{
			get
			{
				return this.m_Columns;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_Columns.Clear();
				}
				else
				{
					this.m_Columns = value;
					bool flag2 = this.m_Columns.Count > 0;
					if (flag2)
					{
						this.CreateMultiColumnHeader();
					}
				}
			}
		}

		private VisualElement DefaultMakeCellItem()
		{
			Label label = new Label();
			label.AddToClassList(MultiColumnController.cellUssClassName);
			return label;
		}

		private VisualElement MakeCell()
		{
			bool flag = this.m_MultiColumnHeader == null;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = new Label();
			}
			else
			{
				VisualElement visualElement2 = new VisualElement
				{
					name = MultiColumnController.rowContainerUssClassName
				};
				visualElement2.AddToClassList(MultiColumnController.rowContainerUssClassName);
				foreach (Column column in this.m_MultiColumnHeader.columns.visibleList)
				{
					VisualElement visualElement3 = new VisualElement();
					visualElement3.AddToClassList(MultiColumnController.cellUssClassName);
					Func<VisualElement> makeCell = column.makeCell;
					VisualElement visualElement4 = ((makeCell != null) ? makeCell() : null) ?? this.DefaultMakeCellItem();
					visualElement3.SetProperty(this.bindableElementPropertyName, visualElement4);
					visualElement3.Add(visualElement4);
					visualElement2.Add(visualElement3);
				}
				visualElement = visualElement2;
			}
			return visualElement;
		}

		private void BindCell(VisualElement element, int index)
		{
			int num = 0;
			element.style.width = this.header.worldBoundingBox.width;
			foreach (Column column in this.m_MultiColumnHeader.columns.visibleList)
			{
				MultiColumnCollectionHeader.ColumnData columnData;
				bool flag = !this.m_MultiColumnHeader.columnDataMap.TryGetValue(column, out columnData);
				if (!flag)
				{
					VisualElement visualElement = element[num++];
					VisualElement visualElement2 = visualElement.GetProperty(this.bindableElementPropertyName) as VisualElement;
					bool flag2 = column.bindCell != null;
					if (flag2)
					{
						column.bindCell(visualElement2, index);
					}
					visualElement.style.width = columnData.control.resolvedStyle.width;
					visualElement.SetProperty(this.k_BoundColumnVePropertyName, column);
				}
			}
		}

		private void UnbindCell(VisualElement element, int index)
		{
			foreach (VisualElement visualElement in element.Children())
			{
				Column column = visualElement.GetProperty(this.k_BoundColumnVePropertyName) as Column;
				bool flag = column == null;
				if (!flag)
				{
					VisualElement visualElement2 = visualElement.GetProperty(this.bindableElementPropertyName) as VisualElement;
					Action<VisualElement, int> unbindCell = column.unbindCell;
					if (unbindCell != null)
					{
						unbindCell(visualElement2, index);
					}
				}
			}
		}

		private void DestroyCell(VisualElement element)
		{
			foreach (VisualElement visualElement in element.Children())
			{
				Column column = visualElement.GetProperty(this.k_BoundColumnVePropertyName) as Column;
				bool flag = column == null;
				if (!flag)
				{
					VisualElement visualElement2 = visualElement.GetProperty(this.bindableElementPropertyName) as VisualElement;
					Action<VisualElement> destroyCell = column.destroyCell;
					if (destroyCell != null)
					{
						destroyCell(visualElement2);
					}
					visualElement.ClearProperty(this.k_BoundColumnVePropertyName);
				}
			}
		}

		public VisualElement CreateMultiColumnHeader()
		{
			bool flag = this.m_MultiColumnHeader != null;
			if (flag)
			{
				this.Dispose();
			}
			this.m_MultiColumnHeader = new MultiColumnCollectionHeader(this.columns, new SortColumnDescriptions(), new List<SortColumnDescription>())
			{
				viewDataKey = "Header"
			};
			this.m_MultiColumnHeader.contextMenuPopulateEvent += this.OnContextMenuPopulateEvent;
			this.m_MultiColumnHeader.columnResized += this.OnColumnResized;
			this.m_MultiColumnHeader.viewDataRestored += this.OnViewDataRestored;
			this.m_MultiColumnHeader.columns.columnAdded += this.OnColumnAdded;
			this.m_MultiColumnHeader.columns.columnRemoved += this.OnColumnRemoved;
			this.m_MultiColumnHeader.columns.columnReordered += this.OnColumnReordered;
			this.m_MultiColumnHeader.columns.columnChanged += this.OnColumnsChanged;
			this.m_MultiColumnHeader.columns.changed += this.OnColumnChanged;
			this.m_HeaderContainer = new VisualElement
			{
				name = MultiColumnController.headerContainerUssClassName
			};
			this.m_HeaderContainer.AddToClassList(MultiColumnController.headerContainerUssClassName);
			this.m_HeaderContainer.viewDataKey = "unity-multi-column-header-container";
			this.m_HeaderContainer.Add(this.m_MultiColumnHeader);
			return this.m_HeaderContainer;
		}

		private void Dispose()
		{
			this.m_MultiColumnHeader.contextMenuPopulateEvent -= this.OnContextMenuPopulateEvent;
			this.m_MultiColumnHeader.columnResized -= this.OnColumnResized;
			this.m_MultiColumnHeader.viewDataRestored -= this.OnViewDataRestored;
			this.m_MultiColumnHeader.columns.columnAdded -= this.OnColumnAdded;
			this.m_MultiColumnHeader.columns.columnRemoved -= this.OnColumnRemoved;
			this.m_MultiColumnHeader.columns.columnReordered -= this.OnColumnReordered;
			this.m_MultiColumnHeader.columns.columnChanged -= this.OnColumnsChanged;
			this.m_MultiColumnHeader.columns.changed -= this.OnColumnChanged;
			this.m_MultiColumnHeader.RemoveFromHierarchy();
			this.m_MultiColumnHeader.Dispose();
			this.m_MultiColumnHeader = null;
			this.m_HeaderContainer.RemoveFromHierarchy();
			this.m_HeaderContainer = null;
		}

		private void OnContextMenuPopulateEvent(ContextualMenuPopulateEvent evt, Column column)
		{
			Action<ContextualMenuPopulateEvent, Column> action = this.headerContextMenuPopulateEvent;
			if (action != null)
			{
				action(evt, column);
			}
		}

		private void OnColumnResized(int index, float width)
		{
			bool isRebuildScheduled = this.m_View.isRebuildScheduled;
			if (!isRebuildScheduled)
			{
				this.m_View.Query<VisualElement>(null, MultiColumnController.rowContainerUssClassName).ForEach<StyleLength>((VisualElement element) => element[index].style.width = width);
			}
		}

		private void OnColumnAdded(Column column, int index)
		{
			this.m_View.Rebuild();
		}

		private void OnColumnRemoved(Column column)
		{
			this.m_View.Rebuild();
		}

		private void OnColumnReordered(Column column, int from, int to)
		{
			bool isApplyingViewState = this.m_MultiColumnHeader.isApplyingViewState;
			if (!isApplyingViewState)
			{
				this.m_View.Rebuild();
			}
		}

		private void OnColumnsChanged(Column column, ColumnDataType type)
		{
			bool isApplyingViewState = this.m_MultiColumnHeader.isApplyingViewState;
			if (!isApplyingViewState)
			{
				bool flag = type == ColumnDataType.Visibility;
				if (flag)
				{
					this.m_View.ScheduleRebuild();
				}
			}
		}

		private void OnColumnChanged(ColumnsDataType type)
		{
			bool isApplyingViewState = this.m_MultiColumnHeader.isApplyingViewState;
			if (!isApplyingViewState)
			{
				bool flag = type == ColumnsDataType.PrimaryColumn;
				if (flag)
				{
					this.m_View.ScheduleRebuild();
				}
			}
		}

		private void OnViewDataRestored()
		{
			this.m_View.Rebuild();
		}

		private Columns m_Columns;

		private MultiColumnCollectionHeader m_MultiColumnHeader;

		private VisualElement m_HeaderContainer;

		private const string k_HeaderViewDataKey = "Header";

		private const string k_HeaderContainerViewDataKey = "unity-multi-column-header-container";

		private readonly PropertyName k_BoundColumnVePropertyName = "__unity-multi-column-bound-column";

		private readonly PropertyName bindableElementPropertyName = "__unity-multi-column-bindable-element";
	}
}
