using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Pool;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnCollectionHeader : VisualElement, IDisposable
	{
		internal bool isApplyingViewState
		{
			get
			{
				return this.m_ApplyingViewState;
			}
		}

		public Dictionary<Column, MultiColumnCollectionHeader.ColumnData> columnDataMap { get; } = new Dictionary<Column, MultiColumnCollectionHeader.ColumnData>();

		public ColumnLayout columnLayout { get; }

		public VisualElement columnContainer { get; }

		public VisualElement resizeHandleContainer { get; }

		public IEnumerable<SortColumnDescription> sortedColumns
		{
			get
			{
				return this.m_SortedColumns;
			}
		}

		public SortColumnDescriptions sortDescriptions
		{
			get
			{
				return this.m_SortDescriptions;
			}
			protected internal set
			{
				this.m_SortDescriptions = value;
				this.m_SortDescriptions.changed += this.UpdateSortedColumns;
				this.UpdateSortedColumns();
			}
		}

		public Columns columns { get; }

		public bool sortingEnabled
		{
			get
			{
				return this.m_SortingEnabled;
			}
			set
			{
				bool flag = this.m_SortingEnabled == value;
				if (!flag)
				{
					this.m_SortingEnabled = value;
					this.UpdateSortingStatus();
					this.UpdateSortedColumns();
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, float> columnResized;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action columnSortingChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ContextualMenuPopulateEvent, Column> contextMenuPopulateEvent;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action viewDataRestored;

		public MultiColumnCollectionHeader()
			: this(new Columns(), new SortColumnDescriptions(), new List<SortColumnDescription>())
		{
		}

		public MultiColumnCollectionHeader(Columns columns, SortColumnDescriptions sortDescriptions, List<SortColumnDescription> sortedColumns)
		{
			base.AddToClassList(MultiColumnCollectionHeader.ussClassName);
			this.columns = columns;
			this.m_SortedColumns = sortedColumns;
			this.sortDescriptions = sortDescriptions;
			this.columnContainer = new VisualElement
			{
				pickingMode = PickingMode.Ignore
			};
			this.columnContainer.AddToClassList(MultiColumnCollectionHeader.columnContainerUssClassName);
			base.Add(this.columnContainer);
			this.resizeHandleContainer = new VisualElement
			{
				pickingMode = PickingMode.Ignore
			};
			this.resizeHandleContainer.AddToClassList(MultiColumnCollectionHeader.handleContainerUssClassName);
			this.resizeHandleContainer.StretchToParentSize();
			base.Add(this.resizeHandleContainer);
			this.columnLayout = new ColumnLayout(columns);
			this.columnLayout.layoutRequested += this.ScheduleDoLayout;
			foreach (Column column in columns.visibleList)
			{
				this.OnColumnAdded(column);
			}
			this.columns.columnAdded += this.OnColumnAdded;
			this.columns.columnRemoved += this.OnColumnRemoved;
			this.columns.columnChanged += this.OnColumnChanged;
			this.columns.columnReordered += this.OnColumnReordered;
			this.columns.columnResized += this.OnColumnResized;
			this.AddManipulator(new ContextualMenuManipulator(new Action<ContextualMenuPopulateEvent>(this.OnContextualMenuManipulator)));
			base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
		}

		private void ScheduleDoLayout()
		{
			bool doLayoutScheduled = this.m_DoLayoutScheduled;
			if (!doLayoutScheduled)
			{
				base.schedule.Execute(new Action(this.DoLayout));
				this.m_DoLayoutScheduled = true;
			}
		}

		private void ResizeToFit()
		{
			this.columnLayout.ResizeToFit(base.layout.width);
		}

		private void UpdateSortedColumns()
		{
			bool sortingUpdatesTemporarilyDisabled = this.m_SortingUpdatesTemporarilyDisabled;
			if (!sortingUpdatesTemporarilyDisabled)
			{
				List<MultiColumnCollectionHeader.SortedColumnState> list;
				using (CollectionPool<List<MultiColumnCollectionHeader.SortedColumnState>, MultiColumnCollectionHeader.SortedColumnState>.Get(out list))
				{
					bool sortingEnabled = this.sortingEnabled;
					if (sortingEnabled)
					{
						foreach (SortColumnDescription sortColumnDescription in this.sortDescriptions)
						{
							Column column = null;
							bool flag = sortColumnDescription.columnIndex != -1;
							if (flag)
							{
								column = this.columns[sortColumnDescription.columnIndex];
							}
							else
							{
								bool flag2 = !string.IsNullOrEmpty(sortColumnDescription.columnName);
								if (flag2)
								{
									column = this.columns[sortColumnDescription.columnName];
								}
							}
							bool flag3 = column != null && column.sortable;
							if (flag3)
							{
								sortColumnDescription.column = column;
								list.Add(new MultiColumnCollectionHeader.SortedColumnState(sortColumnDescription, sortColumnDescription.direction));
							}
							else
							{
								sortColumnDescription.column = null;
							}
						}
					}
					bool flag4 = this.m_OldSortedColumnStates.SequenceEqual<MultiColumnCollectionHeader.SortedColumnState>(list);
					if (flag4)
					{
						return;
					}
					this.m_SortedColumns.Clear();
					foreach (MultiColumnCollectionHeader.SortedColumnState sortedColumnState in list)
					{
						this.m_SortedColumns.Add(sortedColumnState.columnDesc);
					}
					this.m_OldSortedColumnStates.CopyFrom<MultiColumnCollectionHeader.SortedColumnState>(list);
				}
				this.SaveViewState();
				this.RaiseColumnSortingChanged();
			}
		}

		private void UpdateColumnControls()
		{
			bool flag = false;
			Column column = null;
			foreach (Column column2 in this.columns.visibleList)
			{
				flag |= column2.stretchable;
				MultiColumnCollectionHeader.ColumnData columnData = null;
				bool flag2 = this.columnDataMap.TryGetValue(column2, out columnData);
				if (flag2)
				{
					columnData.control.style.minWidth = column2.minWidth;
					columnData.control.style.maxWidth = column2.maxWidth;
					columnData.resizeHandle.style.display = ((this.columns.resizable && column2.resizable) ? DisplayStyle.Flex : DisplayStyle.None);
				}
				column = column2;
			}
			bool flag3 = flag;
			if (flag3)
			{
				this.columnContainer.style.flexGrow = 1f;
				MultiColumnCollectionHeader.ColumnData columnData2;
				bool flag4 = this.columns.stretchMode == Columns.StretchMode.GrowAndFill && this.columnDataMap.TryGetValue(column, out columnData2);
				if (flag4)
				{
					columnData2.resizeHandle.style.display = DisplayStyle.None;
				}
			}
			else
			{
				this.columnContainer.style.flexGrow = 0f;
			}
			this.UpdateSortingStatus();
		}

		private void OnColumnAdded(Column column, int index)
		{
			this.OnColumnAdded(column);
		}

		private void OnColumnAdded(Column column)
		{
			bool flag = this.columnDataMap.ContainsKey(column);
			if (!flag)
			{
				MultiColumnHeaderColumn multiColumnHeaderColumn = new MultiColumnHeaderColumn(column);
				MultiColumnHeaderColumnResizeHandle multiColumnHeaderColumnResizeHandle = new MultiColumnHeaderColumnResizeHandle();
				multiColumnHeaderColumn.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnColumnControlGeometryChanged), TrickleDown.NoTrickleDown);
				multiColumnHeaderColumn.clickable.clickedWithEventInfo += this.OnColumnClicked;
				multiColumnHeaderColumn.mover.activeChanged += this.OnMoveManipulatorActivated;
				multiColumnHeaderColumnResizeHandle.dragArea.AddManipulator(new ColumnResizer(column));
				this.columnDataMap[column] = new MultiColumnCollectionHeader.ColumnData
				{
					control = multiColumnHeaderColumn,
					resizeHandle = multiColumnHeaderColumnResizeHandle
				};
				bool visible = column.visible;
				if (visible)
				{
					this.columnContainer.Insert(column.visibleIndex, multiColumnHeaderColumn);
					this.resizeHandleContainer.Insert(column.visibleIndex, multiColumnHeaderColumnResizeHandle);
				}
				else
				{
					this.OnColumnRemoved(column);
				}
				this.UpdateColumnControls();
				this.SaveViewState();
			}
		}

		private void OnColumnRemoved(Column column)
		{
			MultiColumnCollectionHeader.ColumnData columnData;
			bool flag = !this.columnDataMap.TryGetValue(column, out columnData);
			if (!flag)
			{
				this.CleanupColumnData(columnData);
				this.columnDataMap.Remove(column);
				this.UpdateColumnControls();
				this.SaveViewState();
			}
		}

		private void OnColumnChanged(Column column, ColumnDataType type)
		{
			bool flag = type == ColumnDataType.Visibility;
			if (flag)
			{
				bool visible = column.visible;
				if (visible)
				{
					this.OnColumnAdded(column);
				}
				else
				{
					this.OnColumnRemoved(column);
				}
				this.ApplyColumnSorting();
			}
			this.UpdateColumnControls();
			bool flag2 = type == ColumnDataType.Visibility;
			if (flag2)
			{
				this.SaveViewState();
			}
		}

		private void OnColumnReordered(Column column, int from, int to)
		{
			bool flag = !column.visible || from == to;
			if (!flag)
			{
				MultiColumnCollectionHeader.ColumnData columnData;
				bool flag2 = this.columnDataMap.TryGetValue(column, out columnData);
				if (flag2)
				{
					int num = column.visibleIndex;
					bool flag3 = num == this.columns.visibleList.Count<Column>() - 1;
					if (flag3)
					{
						columnData.control.BringToFront();
					}
					else
					{
						bool flag4 = to > from;
						if (flag4)
						{
							num++;
						}
						columnData.control.PlaceBehind(this.columnContainer[num]);
						columnData.resizeHandle.PlaceBehind(this.resizeHandleContainer[num]);
					}
				}
				this.UpdateColumnControls();
				this.SaveViewState();
			}
		}

		private void OnColumnResized(Column column)
		{
			this.SaveViewState();
		}

		private void OnContextualMenuManipulator(ContextualMenuPopulateEvent evt)
		{
			Column column3 = null;
			bool flag = this.columns.visibleList.Count<Column>() > 0;
			foreach (Column column2 in this.columns.visibleList)
			{
				bool flag2 = this.columns.stretchMode == Columns.StretchMode.GrowAndFill && flag && column2.stretchable;
				if (flag2)
				{
					flag = false;
				}
				bool flag3 = column3 == null;
				if (flag3)
				{
					MultiColumnCollectionHeader.ColumnData columnData;
					bool flag4 = this.columnDataMap.TryGetValue(column2, out columnData);
					if (flag4)
					{
						bool flag5 = columnData.control.layout.Contains(evt.localMousePosition);
						if (flag5)
						{
							column3 = column2;
						}
					}
				}
			}
			evt.menu.AppendAction("Resize To Fit", delegate(DropdownMenuAction a)
			{
				this.ResizeToFit();
			}, flag ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
			evt.menu.AppendSeparator(null);
			using (IEnumerator<Column> enumerator2 = this.columns.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Column column = enumerator2.Current;
					string text = column.title;
					bool flag6 = string.IsNullOrEmpty(text);
					if (flag6)
					{
						text = column.name;
					}
					bool flag7 = string.IsNullOrEmpty(text);
					if (flag7)
					{
						text = "Unnamed Column_" + column.index.ToString();
					}
					evt.menu.AppendAction(text, delegate(DropdownMenuAction a)
					{
						column.visible = !column.visible;
					}, delegate(DropdownMenuAction a)
					{
						bool flag8 = !string.IsNullOrEmpty(column.name) && this.columns.primaryColumnName == column.name;
						DropdownMenuAction.Status status;
						if (flag8)
						{
							status = DropdownMenuAction.Status.Disabled;
						}
						else
						{
							bool flag9 = !column.optional;
							if (flag9)
							{
								status = DropdownMenuAction.Status.Disabled;
							}
							else
							{
								bool visible = column.visible;
								if (visible)
								{
									status = DropdownMenuAction.Status.Checked;
								}
								else
								{
									status = DropdownMenuAction.Status.Normal;
								}
							}
						}
						return status;
					}, null);
				}
			}
			Action<ContextualMenuPopulateEvent, Column> action = this.contextMenuPopulateEvent;
			if (action != null)
			{
				action(evt, column3);
			}
		}

		private void OnMoveManipulatorActivated(ColumnMover mover)
		{
			this.resizeHandleContainer.style.display = (mover.active ? DisplayStyle.None : DisplayStyle.Flex);
		}

		private void OnGeometryChanged(GeometryChangedEvent e)
		{
			bool flag = float.IsNaN(e.newRect.width) || float.IsNaN(e.newRect.height);
			if (!flag)
			{
				this.columnLayout.Dirty();
				bool flag2 = e.layoutPass > 2;
				if (flag2)
				{
					this.ScheduleDoLayout();
				}
				else
				{
					this.DoLayout();
				}
			}
		}

		private void DoLayout()
		{
			this.columnLayout.DoLayout(base.layout.width);
			this.m_DoLayoutScheduled = false;
		}

		private void OnColumnControlGeometryChanged(GeometryChangedEvent evt)
		{
			MultiColumnHeaderColumn multiColumnHeaderColumn = evt.target as MultiColumnHeaderColumn;
			bool flag = multiColumnHeaderColumn == null;
			if (!flag)
			{
				MultiColumnCollectionHeader.ColumnData columnData = this.columnDataMap[multiColumnHeaderColumn.column];
				columnData.resizeHandle.style.left = multiColumnHeaderColumn.layout.xMax;
				bool flag2 = Math.Abs(evt.newRect.width - evt.oldRect.width) < float.Epsilon;
				if (!flag2)
				{
					this.RaiseColumnResized(this.columnContainer.IndexOf(evt.target as VisualElement));
				}
			}
		}

		private void OnColumnClicked(EventBase evt)
		{
			bool flag = !this.sortingEnabled;
			if (!flag)
			{
				MultiColumnHeaderColumn multiColumnHeaderColumn = evt.currentTarget as MultiColumnHeaderColumn;
				bool flag2 = multiColumnHeaderColumn == null || !multiColumnHeaderColumn.column.sortable;
				if (!flag2)
				{
					IPointerEvent pointerEvent = evt as IPointerEvent;
					bool flag3 = pointerEvent != null;
					EventModifiers eventModifiers;
					if (flag3)
					{
						eventModifiers = pointerEvent.modifiers;
					}
					else
					{
						IMouseEvent mouseEvent = evt as IMouseEvent;
						bool flag4 = mouseEvent != null;
						if (!flag4)
						{
							return;
						}
						eventModifiers = mouseEvent.modifiers;
					}
					this.m_SortingUpdatesTemporarilyDisabled = true;
					try
					{
						this.UpdateSortColumnDescriptionsOnClick(multiColumnHeaderColumn.column, eventModifiers);
					}
					finally
					{
						this.m_SortingUpdatesTemporarilyDisabled = false;
					}
					this.UpdateSortedColumns();
				}
			}
		}

		private void UpdateSortColumnDescriptionsOnClick(Column column, EventModifiers modifiers)
		{
			SortColumnDescription sortColumnDescription = this.sortDescriptions.FirstOrDefault<SortColumnDescription>((SortColumnDescription d) => d.column == column || (!string.IsNullOrEmpty(column.name) && d.columnName == column.name) || d.columnIndex == column.index);
			bool flag = sortColumnDescription != null;
			if (flag)
			{
				bool flag2 = modifiers == EventModifiers.Shift;
				if (flag2)
				{
					this.sortDescriptions.Remove(sortColumnDescription);
					return;
				}
				sortColumnDescription.direction = ((sortColumnDescription.direction == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending);
			}
			else
			{
				sortColumnDescription = (string.IsNullOrEmpty(column.name) ? new SortColumnDescription(column.index, SortDirection.Ascending) : new SortColumnDescription(column.name, SortDirection.Ascending));
			}
			EventModifiers eventModifiers = EventModifiers.Control;
			RuntimePlatform platform = Application.platform;
			bool flag3 = platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer;
			if (flag3)
			{
				eventModifiers = EventModifiers.Command;
			}
			bool flag4 = modifiers != eventModifiers;
			if (flag4)
			{
				this.sortDescriptions.Clear();
			}
			bool flag5 = !this.sortDescriptions.Contains(sortColumnDescription);
			if (flag5)
			{
				this.sortDescriptions.Add(sortColumnDescription);
			}
		}

		public void ScrollHorizontally(float horizontalOffset)
		{
			base.transform.position = new Vector3(-horizontalOffset, base.transform.position.y, base.transform.position.z);
		}

		private void RaiseColumnResized(int columnIndex)
		{
			Action<int, float> action = this.columnResized;
			if (action != null)
			{
				action(columnIndex, this.columnContainer[columnIndex].resolvedStyle.width);
			}
		}

		private void RaiseColumnSortingChanged()
		{
			this.ApplyColumnSorting();
			bool flag = !this.m_ApplyingViewState;
			if (flag)
			{
				Action action = this.columnSortingChanged;
				if (action != null)
				{
					action();
				}
			}
		}

		private void ApplyColumnSorting()
		{
			foreach (Column column in this.columns.visibleList)
			{
				MultiColumnCollectionHeader.ColumnData columnData;
				bool flag = !this.columnDataMap.TryGetValue(column, out columnData);
				if (!flag)
				{
					columnData.control.sortOrderLabel = "";
					columnData.control.RemoveFromClassList(MultiColumnHeaderColumn.sortedAscendingUssClassName);
					columnData.control.RemoveFromClassList(MultiColumnHeaderColumn.sortedDescendingUssClassName);
				}
			}
			List<MultiColumnCollectionHeader.ColumnData> list = new List<MultiColumnCollectionHeader.ColumnData>();
			foreach (SortColumnDescription sortColumnDescription in this.sortedColumns)
			{
				MultiColumnCollectionHeader.ColumnData columnData2;
				bool flag2 = this.columnDataMap.TryGetValue(sortColumnDescription.column, out columnData2);
				if (flag2)
				{
					list.Add(columnData2);
					bool flag3 = sortColumnDescription.direction == SortDirection.Ascending;
					if (flag3)
					{
						columnData2.control.AddToClassList(MultiColumnHeaderColumn.sortedAscendingUssClassName);
					}
					else
					{
						columnData2.control.AddToClassList(MultiColumnHeaderColumn.sortedDescendingUssClassName);
					}
				}
			}
			bool flag4 = list.Count > 1;
			if (flag4)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].control.sortOrderLabel = (i + 1).ToString();
				}
			}
		}

		private void UpdateSortingStatus()
		{
			bool flag = false;
			foreach (Column column in this.columns.visibleList)
			{
				MultiColumnCollectionHeader.ColumnData columnData;
				bool flag2 = !this.columnDataMap.TryGetValue(column, out columnData);
				if (!flag2)
				{
					bool flag3 = this.sortingEnabled && column.sortable;
					if (flag3)
					{
						flag = true;
					}
				}
			}
			foreach (Column column2 in this.columns.visibleList)
			{
				MultiColumnCollectionHeader.ColumnData columnData2;
				bool flag4 = !this.columnDataMap.TryGetValue(column2, out columnData2);
				if (!flag4)
				{
					bool flag5 = flag;
					if (flag5)
					{
						columnData2.control.AddToClassList(MultiColumnHeaderColumn.sortableUssClassName);
					}
					else
					{
						columnData2.control.RemoveFromClassList(MultiColumnHeaderColumn.sortableUssClassName);
					}
				}
			}
		}

		internal override void OnViewDataReady()
		{
			try
			{
				this.m_ApplyingViewState = true;
				base.OnViewDataReady();
				string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
				this.m_ViewState = base.GetOrCreateViewData<MultiColumnCollectionHeader.ViewState>(this.m_ViewState, fullHierarchicalViewDataKey);
				this.m_ViewState.Apply(this);
				Action action = this.viewDataRestored;
				if (action != null)
				{
					action();
				}
			}
			finally
			{
				this.m_ApplyingViewState = false;
			}
		}

		private void SaveViewState()
		{
			bool applyingViewState = this.m_ApplyingViewState;
			if (!applyingViewState)
			{
				MultiColumnCollectionHeader.ViewState viewState = this.m_ViewState;
				if (viewState != null)
				{
					viewState.Save(this);
				}
				base.SaveViewData();
			}
		}

		private void CleanupColumnData(MultiColumnCollectionHeader.ColumnData data)
		{
			data.control.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnColumnControlGeometryChanged), TrickleDown.NoTrickleDown);
			data.control.clickable.clickedWithEventInfo -= this.OnColumnClicked;
			data.control.mover.activeChanged -= this.OnMoveManipulatorActivated;
			data.control.RemoveFromHierarchy();
			data.control.Dispose();
			data.resizeHandle.RemoveFromHierarchy();
		}

		public void Dispose()
		{
			this.sortDescriptions.changed -= this.UpdateSortedColumns;
			this.columnLayout.layoutRequested -= this.ScheduleDoLayout;
			this.columns.columnAdded -= this.OnColumnAdded;
			this.columns.columnRemoved -= this.OnColumnRemoved;
			this.columns.columnChanged -= this.OnColumnChanged;
			this.columns.columnReordered -= this.OnColumnReordered;
			this.columns.columnResized -= this.OnColumnResized;
			foreach (MultiColumnCollectionHeader.ColumnData columnData in this.columnDataMap.Values)
			{
				this.CleanupColumnData(columnData);
			}
			this.columnDataMap.Clear();
		}

		private const int kMaxStableLayoutPassCount = 2;

		public static readonly string ussClassName = "unity-multi-column-header";

		public static readonly string columnContainerUssClassName = MultiColumnCollectionHeader.ussClassName + "__column-container";

		public static readonly string handleContainerUssClassName = MultiColumnCollectionHeader.ussClassName + "__resize-handle-container";

		public static readonly string reorderableUssClassName = MultiColumnCollectionHeader.ussClassName + "__header";

		private bool m_SortingEnabled;

		private List<SortColumnDescription> m_SortedColumns;

		private SortColumnDescriptions m_SortDescriptions;

		private List<MultiColumnCollectionHeader.SortedColumnState> m_OldSortedColumnStates = new List<MultiColumnCollectionHeader.SortedColumnState>();

		private bool m_SortingUpdatesTemporarilyDisabled;

		private MultiColumnCollectionHeader.ViewState m_ViewState;

		private bool m_ApplyingViewState;

		private bool m_DoLayoutScheduled;

		[Serializable]
		private class ViewState
		{
			internal void Save(MultiColumnCollectionHeader header)
			{
				this.m_SortDescriptions.Clear();
				this.m_OrderedColumnStates.Clear();
				foreach (SortColumnDescription sortColumnDescription in header.sortDescriptions)
				{
					this.m_SortDescriptions.Add(sortColumnDescription);
				}
				foreach (Column column in header.columns.displayList)
				{
					MultiColumnCollectionHeader.ViewState.ColumnState columnState = new MultiColumnCollectionHeader.ViewState.ColumnState
					{
						index = column.index,
						name = column.name,
						actualWidth = column.desiredWidth,
						width = column.width,
						visible = column.visible
					};
					this.m_OrderedColumnStates.Add(columnState);
				}
				this.m_HasPersistedData = true;
			}

			internal void Apply(MultiColumnCollectionHeader header)
			{
				bool flag = !this.m_HasPersistedData;
				if (!flag)
				{
					int num = Math.Min(this.m_OrderedColumnStates.Count, header.columns.Count);
					int num2 = 0;
					int num3 = 0;
					while (num3 < this.m_OrderedColumnStates.Count && num2 < num)
					{
						MultiColumnCollectionHeader.ViewState.ColumnState columnState = this.m_OrderedColumnStates[num3];
						Column column = null;
						bool flag2 = !string.IsNullOrEmpty(columnState.name);
						if (flag2)
						{
							bool flag3 = header.columns.Contains(columnState.name);
							if (flag3)
							{
								column = header.columns[columnState.name];
							}
							goto IL_00E2;
						}
						bool flag4 = columnState.index > header.columns.Count - 1;
						if (!flag4)
						{
							column = header.columns[columnState.index];
							bool flag5 = !string.IsNullOrEmpty(column.name);
							if (flag5)
							{
								column = null;
							}
							goto IL_00E2;
						}
						IL_0135:
						num3++;
						continue;
						IL_00E2:
						bool flag6 = column == null;
						if (flag6)
						{
							goto IL_0135;
						}
						header.columns.ReorderDisplay(column.displayIndex, num2++);
						column.visible = columnState.visible;
						column.width = columnState.width;
						column.desiredWidth = columnState.actualWidth;
						goto IL_0135;
					}
					header.sortDescriptions.Clear();
					foreach (SortColumnDescription sortColumnDescription in this.m_SortDescriptions)
					{
						header.sortDescriptions.Add(sortColumnDescription);
					}
				}
			}

			[SerializeField]
			private bool m_HasPersistedData;

			[SerializeField]
			private List<SortColumnDescription> m_SortDescriptions = new List<SortColumnDescription>();

			[SerializeField]
			private List<MultiColumnCollectionHeader.ViewState.ColumnState> m_OrderedColumnStates = new List<MultiColumnCollectionHeader.ViewState.ColumnState>();

			[Serializable]
			private struct ColumnState
			{
				public int index;

				public string name;

				public float actualWidth;

				public Length width;

				public bool visible;
			}
		}

		internal class ColumnData
		{
			public MultiColumnHeaderColumn control { get; set; }

			public MultiColumnHeaderColumnResizeHandle resizeHandle { get; set; }
		}

		private struct SortedColumnState
		{
			public SortedColumnState(SortColumnDescription desc, SortDirection dir)
			{
				this.columnDesc = desc;
				this.direction = dir;
			}

			public SortColumnDescription columnDesc;

			public SortDirection direction;
		}
	}
}
