using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements
{
	public class MultiColumnController : IDisposable
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action columnSortingChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ContextualMenuPopulateEvent, Column> headerContextMenuPopulateEvent;

		internal MultiColumnCollectionHeader header
		{
			get
			{
				return this.m_MultiColumnHeader;
			}
		}

		internal ColumnSortingMode sortingMode
		{
			get
			{
				return this.m_SortingMode;
			}
			set
			{
				this.m_SortingMode = value;
				this.header.sortingEnabled = this.m_SortingMode > ColumnSortingMode.None;
			}
		}

		public MultiColumnController(Columns columns, SortColumnDescriptions sortDescriptions, List<SortColumnDescription> sortedColumns)
		{
			this.m_MultiColumnHeader = new MultiColumnCollectionHeader(columns, sortDescriptions, sortedColumns)
			{
				viewDataKey = MultiColumnController.k_HeaderViewDataKey
			};
			this.m_MultiColumnHeader.columnSortingChanged += this.OnColumnSortingChanged;
			this.m_MultiColumnHeader.contextMenuPopulateEvent += this.OnContextMenuPopulateEvent;
			this.m_MultiColumnHeader.columnResized += this.OnColumnResized;
			this.m_MultiColumnHeader.viewDataRestored += this.OnViewDataRestored;
			this.m_MultiColumnHeader.columns.columnAdded += this.OnColumnAdded;
			this.m_MultiColumnHeader.columns.columnRemoved += this.OnColumnRemoved;
			this.m_MultiColumnHeader.columns.columnReordered += this.OnColumnReordered;
			this.m_MultiColumnHeader.columns.columnChanged += this.OnColumnsChanged;
			this.m_MultiColumnHeader.columns.changed += this.OnColumnChanged;
		}

		private static void BindCellItem<T>(VisualElement ve, int rowIndex, Column column, T item)
		{
			bool flag = column.bindCell != null;
			if (flag)
			{
				column.bindCell(ve, rowIndex);
			}
			else
			{
				MultiColumnController.DefaultBindCellItem<T>(ve, item);
			}
		}

		private static void UnbindCellItem(VisualElement ve, int rowIndex, Column column)
		{
			Action<VisualElement, int> unbindCell = column.unbindCell;
			if (unbindCell != null)
			{
				unbindCell(ve, rowIndex);
			}
		}

		private static VisualElement DefaultMakeCellItem()
		{
			Label label = new Label();
			label.AddToClassList(MultiColumnController.cellLabelUssClassName);
			return label;
		}

		private static void DefaultBindCellItem<T>(VisualElement ve, T item)
		{
			Label label = ve as Label;
			bool flag = label != null;
			if (flag)
			{
				label.text = item.ToString();
			}
		}

		public VisualElement MakeItem()
		{
			VisualElement visualElement = new VisualElement
			{
				name = MultiColumnController.rowContainerUssClassName
			};
			visualElement.AddToClassList(MultiColumnController.rowContainerUssClassName);
			foreach (Column column in this.m_MultiColumnHeader.columns.visibleList)
			{
				VisualElement visualElement2 = new VisualElement();
				visualElement2.AddToClassList(MultiColumnController.cellUssClassName);
				Func<VisualElement> makeCell = column.makeCell;
				VisualElement visualElement3 = ((makeCell != null) ? makeCell() : null) ?? MultiColumnController.DefaultMakeCellItem();
				visualElement2.SetProperty(MultiColumnController.bindableElementPropertyName, visualElement3);
				visualElement2.Add(visualElement3);
				visualElement.Add(visualElement2);
			}
			return visualElement;
		}

		public void BindItem<T>(VisualElement element, int index, T item)
		{
			int num = 0;
			index = this.GetSourceIndex(index);
			foreach (Column column in this.m_MultiColumnHeader.columns.visibleList)
			{
				MultiColumnCollectionHeader.ColumnData columnData;
				bool flag = !this.m_MultiColumnHeader.columnDataMap.TryGetValue(column, out columnData);
				if (!flag)
				{
					VisualElement visualElement = element[num++];
					VisualElement visualElement2 = visualElement.GetProperty(MultiColumnController.bindableElementPropertyName) as VisualElement;
					MultiColumnController.BindCellItem<T>(visualElement2, index, column, item);
					visualElement.style.width = columnData.control.resolvedStyle.width;
					visualElement.SetProperty(MultiColumnController.k_BoundColumnVePropertyName, column);
				}
			}
		}

		public void UnbindItem(VisualElement element, int index)
		{
			index = this.GetSourceIndex(index);
			foreach (VisualElement visualElement in element.Children())
			{
				Column column = visualElement.GetProperty(MultiColumnController.k_BoundColumnVePropertyName) as Column;
				bool flag = column == null;
				if (!flag)
				{
					VisualElement visualElement2 = visualElement.GetProperty(MultiColumnController.bindableElementPropertyName) as VisualElement;
					MultiColumnController.UnbindCellItem(visualElement2, index, column);
				}
			}
		}

		public void DestroyItem(VisualElement element)
		{
			foreach (VisualElement visualElement in element.Children())
			{
				Column column = visualElement.GetProperty(MultiColumnController.k_BoundColumnVePropertyName) as Column;
				bool flag = column == null;
				if (!flag)
				{
					VisualElement visualElement2 = visualElement.GetProperty(MultiColumnController.bindableElementPropertyName) as VisualElement;
					Action<VisualElement> destroyCell = column.destroyCell;
					if (destroyCell != null)
					{
						destroyCell(visualElement2);
					}
					visualElement.ClearProperty(MultiColumnController.k_BoundColumnVePropertyName);
				}
			}
		}

		public void PrepareView(BaseVerticalCollectionView collectionView)
		{
			bool flag = this.m_View != null;
			if (flag)
			{
				Debug.LogWarning("Trying to initialize multi column view more than once. This shouldn't happen.");
			}
			else
			{
				this.m_View = collectionView;
				this.m_HeaderContainer = new VisualElement
				{
					name = MultiColumnController.headerContainerUssClassName
				};
				this.m_HeaderContainer.AddToClassList(MultiColumnController.headerContainerUssClassName);
				this.m_HeaderContainer.viewDataKey = MultiColumnController.k_HeaderContainerViewDataKey;
				collectionView.scrollView.hierarchy.Insert(0, this.m_HeaderContainer);
				this.m_HeaderContainer.Add(this.m_MultiColumnHeader);
				this.m_View.scrollView.horizontalScroller.valueChanged += this.OnHorizontalScrollerValueChanged;
				this.m_View.scrollView.contentViewport.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnViewportGeometryChanged), TrickleDown.NoTrickleDown);
				this.m_MultiColumnHeader.columnContainer.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnColumnContainerGeometryChanged), TrickleDown.NoTrickleDown);
			}
		}

		public void Dispose()
		{
			bool flag = this.m_View != null;
			if (flag)
			{
				this.m_View.scrollView.horizontalScroller.valueChanged -= this.OnHorizontalScrollerValueChanged;
				this.m_View.scrollView.contentViewport.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnViewportGeometryChanged), TrickleDown.NoTrickleDown);
				this.m_View = null;
			}
			this.m_MultiColumnHeader.columnContainer.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnColumnContainerGeometryChanged), TrickleDown.NoTrickleDown);
			this.m_MultiColumnHeader.columnSortingChanged -= this.OnColumnSortingChanged;
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

		private void OnHorizontalScrollerValueChanged(float v)
		{
			this.m_MultiColumnHeader.ScrollHorizontally(v);
		}

		private void OnViewportGeometryChanged(GeometryChangedEvent evt)
		{
			float num = this.m_MultiColumnHeader.resolvedStyle.paddingLeft + this.m_MultiColumnHeader.resolvedStyle.paddingRight;
			this.m_MultiColumnHeader.style.maxWidth = evt.newRect.width - num;
			this.m_MultiColumnHeader.style.maxWidth = evt.newRect.width - num;
			this.UpdateContentContainer(this.m_View);
		}

		private void OnColumnContainerGeometryChanged(GeometryChangedEvent evt)
		{
			this.UpdateContentContainer(this.m_View);
		}

		private void UpdateContentContainer(BaseVerticalCollectionView collectionView)
		{
			float width = this.m_MultiColumnHeader.columnContainer.layout.width;
			float num = Mathf.Max(width, collectionView.scrollView.contentViewport.resolvedStyle.width);
			collectionView.scrollView.contentContainer.style.width = num;
		}

		private void OnColumnSortingChanged()
		{
			this.UpdateDragger();
			bool flag = this.sortingMode == ColumnSortingMode.Default;
			if (flag)
			{
				this.m_View.RefreshItems();
			}
			Action action = this.columnSortingChanged;
			if (action != null)
			{
				action();
			}
		}

		internal void UpdateDragger()
		{
			bool flag = this.sortingMode == ColumnSortingMode.None;
			if (flag)
			{
				this.m_View.dragger.enabled = true;
			}
			else
			{
				this.m_View.dragger.enabled = this.header.sortedColumnReadonly.Count == 0;
			}
		}

		internal void SortIfNeeded()
		{
			this.UpdateDragger();
			bool flag = this.sortingMode == ColumnSortingMode.None || this.sortingMode != ColumnSortingMode.Default || this.m_View.itemsSource == null;
			if (!flag)
			{
				this.m_View.virtualizationController.UnbindAll();
				List<int> sortedToSourceIndex = this.m_SortedToSourceIndex;
				if (sortedToSourceIndex != null)
				{
					sortedToSourceIndex.Clear();
				}
				List<int> sourceToSortedIndex = this.m_SourceToSortedIndex;
				if (sourceToSortedIndex != null)
				{
					sourceToSortedIndex.Clear();
				}
				bool flag2 = this.header.sortedColumnReadonly.Count == 0;
				if (!flag2)
				{
					if (this.m_SortedToSourceIndex == null)
					{
						this.m_SortedToSourceIndex = new List<int>(this.m_View.itemsSource.Count);
					}
					if (this.m_SourceToSortedIndex == null)
					{
						this.m_SourceToSortedIndex = new List<int>(this.m_View.itemsSource.Count);
					}
					for (int i = 0; i < this.m_View.itemsSource.Count; i++)
					{
						this.m_SortedToSourceIndex.Add(i);
						this.m_SourceToSortedIndex.Add(-1);
					}
					this.m_SortedToSourceIndex.Sort(new Comparison<int>(this.CombinedComparison));
					for (int j = 0; j < this.m_SortedToSourceIndex.Count; j++)
					{
						this.m_SourceToSortedIndex[this.m_SortedToSourceIndex[j]] = j;
					}
				}
			}
		}

		private int CombinedComparison(int a, int b)
		{
			BaseTreeViewController baseTreeViewController = this.m_View.viewController as BaseTreeViewController;
			bool flag = baseTreeViewController != null;
			if (flag)
			{
				int num = baseTreeViewController.GetIdForIndex(a);
				int num2 = baseTreeViewController.GetIdForIndex(b);
				int num3 = baseTreeViewController.GetParentId(num);
				int num4 = baseTreeViewController.GetParentId(num2);
				bool flag2 = num3 != num4;
				if (flag2)
				{
					int i = baseTreeViewController.GetIndentationDepth(num);
					int j = baseTreeViewController.GetIndentationDepth(num2);
					int num5 = i;
					int num6 = j;
					while (i > j)
					{
						i--;
						num = num3;
						num3 = baseTreeViewController.GetParentId(num3);
					}
					while (j > i)
					{
						j--;
						num2 = num4;
						num4 = baseTreeViewController.GetParentId(num4);
					}
					while (num3 != num4)
					{
						num = num3;
						num2 = num4;
						num3 = baseTreeViewController.GetParentId(num3);
						num4 = baseTreeViewController.GetParentId(num4);
					}
					bool flag3 = num == num2;
					if (flag3)
					{
						return num5.CompareTo(num6);
					}
					a = baseTreeViewController.GetIndexForId(num);
					b = baseTreeViewController.GetIndexForId(num2);
				}
			}
			int num7 = 0;
			foreach (SortColumnDescription sortColumnDescription in this.header.sortedColumns)
			{
				Comparison<int> comparison = sortColumnDescription.column.comparison;
				num7 = ((comparison != null) ? comparison(a, b) : 0);
				bool flag4 = num7 != 0;
				if (flag4)
				{
					bool flag5 = sortColumnDescription.direction == SortDirection.Descending;
					if (flag5)
					{
						num7 = -num7;
					}
					break;
				}
			}
			return (num7 == 0) ? a.CompareTo(b) : num7;
		}

		internal int GetSourceIndex(int sortedIndex)
		{
			return MultiColumnController.GetIndexFromList(sortedIndex, this.m_SortedToSourceIndex);
		}

		internal int GetSortedIndex(int sourceIndex)
		{
			return MultiColumnController.GetIndexFromList(sourceIndex, this.m_SourceToSortedIndex);
		}

		private static int GetIndexFromList(int index, List<int> indices)
		{
			bool flag = indices == null;
			int num;
			if (flag)
			{
				num = index;
			}
			else
			{
				bool flag2 = index < 0 || index >= indices.Count;
				if (flag2)
				{
					num = index;
				}
				else
				{
					num = ((indices.Count > 0) ? indices[index] : index);
				}
			}
			return num;
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
				foreach (ReusableCollectionItem reusableCollectionItem in this.m_View.activeItems)
				{
					reusableCollectionItem.bindableElement.ElementAt(index).style.width = width;
				}
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

		private static readonly PropertyName k_BoundColumnVePropertyName = "__unity-multi-column-bound-column";

		internal static readonly PropertyName bindableElementPropertyName = "__unity-multi-column-bindable-element";

		internal static readonly string baseUssClassName = "unity-multi-column-view";

		private static readonly string k_HeaderContainerViewDataKey = "unity-multi-column-header-container";

		public static readonly string headerContainerUssClassName = MultiColumnController.baseUssClassName + "__header-container";

		public static readonly string rowContainerUssClassName = MultiColumnController.baseUssClassName + "__row-container";

		public static readonly string cellUssClassName = MultiColumnController.baseUssClassName + "__cell";

		public static readonly string cellLabelUssClassName = MultiColumnController.cellUssClassName + "__label";

		private static readonly string k_HeaderViewDataKey = "Header";

		private List<int> m_SortedToSourceIndex;

		private List<int> m_SourceToSortedIndex;

		private ColumnSortingMode m_SortingMode;

		private BaseVerticalCollectionView m_View;

		private VisualElement m_HeaderContainer;

		private MultiColumnCollectionHeader m_MultiColumnHeader;
	}
}
