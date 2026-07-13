using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal sealed class HierarchyView : VisualElement, IDisposable
	{
		internal bool m_IsRenamingItem
		{
			get
			{
				return this.m_RenamingItem != null;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HierarchyView.SourceHierarchyChangingEventHandler SourceHierarchyChanging;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HierarchyView.SourceHierarchyChangedEventHandler SourceHierarchyChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<HierarchyViewItem> BindViewItem;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<HierarchyViewItem> UnbindViewItem;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HierarchyViewModel.FlagsChangedEventHandler FlagsChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HierarchyView.PopulateContextMenuEventHandler PopulateContextMenu;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HierarchyView.GetTooltipEventHandler GetTooltip;

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action Initializing;

		public Unity.Hierarchy.Hierarchy Source
		{
			get
			{
				return this.m_Hierarchy;
			}
		}

		public HierarchyFlattened Flattened
		{
			get
			{
				return this.m_HierarchyFlattened;
			}
		}

		public HierarchyViewModel ViewModel
		{
			get
			{
				return this.m_HierarchyViewModel;
			}
		}

		internal MultiColumnListView ListView
		{
			get
			{
				return this.m_MultiColumnListView;
			}
		}

		public VisualElement StyleContainer
		{
			get
			{
				return this.m_StyleContainer;
			}
		}

		public string Filter
		{
			get
			{
				return this.m_HierarchyViewModel.Query.ToString();
			}
			set
			{
				this.m_HierarchyViewModel.SetQuery(value);
			}
		}

		public bool Filtering
		{
			get
			{
				return this.m_HierarchyViewModel.Filtering;
			}
		}

		public bool Updating
		{
			get
			{
				bool flag = this.m_Hierarchy == null || !this.m_Hierarchy.IsCreated;
				return !flag && (this.m_UpdateStage != HierarchyView.UpdateStage.UpdatingHierarchy || this.m_Hierarchy.Updating || this.m_HierarchyFlattened.Updating || this.m_HierarchyViewModel.Updating);
			}
		}

		public bool UpdateNeeded
		{
			get
			{
				bool flag = this.m_Hierarchy == null || !this.m_Hierarchy.IsCreated;
				return !flag && (this.Updating || this.DataUpdateNeeded || this.DisplayUpdateNeeded || this.ExecutePostUpdateActionsNeeded);
			}
		}

		public float UpdateProgress
		{
			get
			{
				bool flag = !this.Updating;
				float num;
				if (flag)
				{
					num = 100f;
				}
				else
				{
					bool flag2 = this.m_UpdateStage == HierarchyView.UpdateStage.UpdatingHierarchyViewModel;
					if (flag2)
					{
						num = this.m_HierarchyViewModel.UpdateProgress;
					}
					else
					{
						num = 0f;
					}
				}
				return num;
			}
		}

		internal bool DataUpdateNeeded
		{
			get
			{
				return this.m_Hierarchy.UpdateNeeded || this.m_HierarchyFlattened.UpdateNeeded || this.m_HierarchyViewModel.UpdateNeeded;
			}
		}

		internal bool DisplayUpdateNeeded
		{
			get
			{
				return this.m_Version != this.m_HierarchyViewModel.Version;
			}
		}

		internal bool ExecutePostUpdateActionsNeeded
		{
			get
			{
				return this.m_PostUpdateActionQueue.Count > 0;
			}
		}

		internal HierarchyViewDragHandler DragHandler
		{
			get
			{
				return this.m_DragHandler;
			}
		}

		internal HierarchyViewItemColumn NameColumn
		{
			get
			{
				return this.m_NameColumn;
			}
		}

		public HierarchyView()
		{
			base.AddToClassList("hierarchy");
			this.AddManipulator(new ContextualMenuManipulator(new Action<ContextualMenuPopulateEvent>(this.InvokePopulateContextMenu)));
			this.m_MultiColumnListView = new MultiColumnListView
			{
				name = "unity-tree-view__list-view",
				fixedItemHeight = 20f,
				selectionType = SelectionType.Multiple,
				reorderMode = ListViewReorderMode.Simple,
				reorderable = true,
				itemsSource = null,
				columns = 
				{
					stretchMode = Columns.StretchMode.Grow
				}
			};
			this.m_NameColumn = new HierarchyViewItemColumn(this);
			this.m_DragHandler = new HierarchyViewDragHandler(this);
			this.m_MultiColumnListView.selectedIndicesChanged += this.OnSelectedIndicesChanged;
			this.m_MultiColumnListView.AddToClassList("unity-tree-view__list-view");
			this.m_MultiColumnListView.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			this.m_MultiColumnListView.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.TrickleDown);
			this.m_MultiColumnListView.RegisterCallback<NavigationMoveEvent>(new EventCallback<NavigationMoveEvent>(this.OnNavigationMove), TrickleDown.NoTrickleDown);
			this.m_MultiColumnListView.Q(null, ScrollView.contentAndVerticalScrollUssClassName).RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnListViewClick), TrickleDown.NoTrickleDown);
			this.m_MultiColumnListView.columns.Add(this.m_NameColumn);
			this.m_NameColumn.stretchable = true;
			this.m_NameColumn.OnBindItem += this.OnBindItem;
			this.m_NameColumn.OnUnbindItem += this.OnUnbindItem;
			ScrollView scrollView = this.m_MultiColumnListView.Q<ScrollView>(null, null);
			this.m_ListViewContentContainer = scrollView.contentContainer;
			scrollView.mode = ScrollViewMode.VerticalAndHorizontal;
			this.m_ListViewContentContainer.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(this.OnClickEvent), TrickleDown.NoTrickleDown);
			this.m_ListViewContentContainer.RegisterCallback<NavigationCancelEvent>(new EventCallback<NavigationCancelEvent>(this.OnNavigationCancel), TrickleDown.NoTrickleDown);
			this.m_StyleContainer = new VisualElement();
			this.m_StyleContainer.AddToClassList("hierarchy__container");
			this.m_StyleContainer.Add(this.m_MultiColumnListView);
			base.Add(this.m_StyleContainer);
			this.m_LastMouseUpSelectionIndex = -1;
			this.SetRenamingItem(null);
			this.m_RenameDelayMs = 500;
		}

		public void Dispose()
		{
			this.SetSourceHierarchy(null, HierarchyNodeFlags.None);
			this.BindViewItem = null;
			this.UnbindViewItem = null;
			this.PopulateContextMenu = null;
			this.GetTooltip = null;
		}

		public void SetSourceHierarchy(Unity.Hierarchy.Hierarchy hierarchy, HierarchyNodeFlags defaultFlags = HierarchyNodeFlags.None)
		{
			bool flag = this.m_Hierarchy == hierarchy;
			if (!flag)
			{
				bool flag2 = this.m_Hierarchy != null;
				if (flag2)
				{
					this.m_Hierarchy.HandlerCreated -= this.OnHandlerCreated;
				}
				bool flag3 = this.m_HierarchyViewModel != null;
				if (flag3)
				{
					this.m_HierarchyViewModel.FlagsChanged -= this.FlagsChanged;
				}
				HierarchyView.SourceHierarchyChangingEventHandler sourceHierarchyChanging = this.SourceHierarchyChanging;
				if (sourceHierarchyChanging != null)
				{
					sourceHierarchyChanging(this.m_Hierarchy, hierarchy, defaultFlags);
				}
				this.ClearColumns();
				this.Reset();
				this.SetRenamingItem(null);
				this.m_LastMouseUpSelectionIndex = -1;
				this.m_SelectedIndicesChangedFromPointerDown = false;
				this.m_SelectedIndices.Clear();
				this.m_ScheduledItem = null;
				this.m_MultiColumnListView.itemsSource = null;
				this.m_PostUpdateActionQueue.Clear();
				this.m_UpdateStage = HierarchyView.UpdateStage.UpdatingHierarchy;
				this.m_Version = 0;
				bool flag4 = this.m_HierarchyViewModel != null;
				if (flag4)
				{
					bool isCreated = this.m_HierarchyViewModel.IsCreated;
					if (isCreated)
					{
						this.m_HierarchyViewModel.Dispose();
					}
					this.m_HierarchyViewModel = null;
				}
				bool flag5 = this.m_HierarchyFlattened != null;
				if (flag5)
				{
					bool isCreated2 = this.m_HierarchyFlattened.IsCreated;
					if (isCreated2)
					{
						this.m_HierarchyFlattened.Dispose();
					}
					this.m_HierarchyFlattened = null;
				}
				this.m_Hierarchy = null;
				bool flag6 = hierarchy == null;
				if (!flag6)
				{
					this.m_Hierarchy = hierarchy;
					this.m_HierarchyFlattened = new HierarchyFlattened(this.m_Hierarchy);
					this.m_HierarchyViewModel = new HierarchyViewModel(this.m_HierarchyFlattened, defaultFlags);
					this.m_Hierarchy.Update();
					this.m_HierarchyFlattened.Update();
					this.m_HierarchyViewModel.Update();
					this.m_MultiColumnListView.itemsSource = this.m_HierarchyViewModel.AsReadOnlyList();
					this.BindColumns();
					this.Initialize();
					HierarchyView.SourceHierarchyChangedEventHandler sourceHierarchyChanged = this.SourceHierarchyChanged;
					if (sourceHierarchyChanged != null)
					{
						sourceHierarchyChanged(hierarchy, defaultFlags);
					}
					this.m_Hierarchy.HandlerCreated += this.OnHandlerCreated;
					this.m_HierarchyViewModel.FlagsChanged += this.FlagsChanged;
				}
			}
		}

		public void Update()
		{
			while (this.DoUpdate(HierarchyView.UpdateMode.Update, 0.0))
			{
			}
		}

		public bool UpdateIncremental()
		{
			return this.DoUpdate(HierarchyView.UpdateMode.UpdateIncremental, 0.0);
		}

		public bool UpdateIncrementalTimed(double milliseconds)
		{
			for (;;)
			{
				this.m_UpdateTimer.Restart();
				bool flag = !this.DoUpdate(HierarchyView.UpdateMode.UpdateIncrementalTimed, milliseconds);
				if (flag)
				{
					break;
				}
				milliseconds -= this.m_UpdateTimer.ElapsedMillisecondsPrecise();
				bool flag2 = milliseconds <= 0.0;
				if (flag2)
				{
					goto Block_2;
				}
			}
			return false;
			Block_2:
			return true;
		}

		public void Select(in HierarchyNode node)
		{
			this.m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void Select(ReadOnlySpan<HierarchyNode> nodes)
		{
			this.m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void SelectRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Selected, direction);
			this.Update();
		}

		public void SelectRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.SetFlagsRecursive(nodes, HierarchyNodeFlags.Selected, direction);
			this.Update();
		}

		public void SelectAll(bool exposedOnly)
		{
			if (exposedOnly)
			{
				this.m_HierarchyViewModel.SetFlags(this.m_HierarchyViewModel.AsReadOnlySpan(), HierarchyNodeFlags.Selected);
			}
			else
			{
				this.m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Selected);
			}
			this.Update();
		}

		public void SetSelection(in HierarchyNode node)
		{
			using (new HierarchyViewModelFlagsChangeScope(this.m_HierarchyViewModel))
			{
				this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
				this.m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Selected);
			}
			this.Update();
		}

		public void SetSelection(ReadOnlySpan<HierarchyNode> nodes)
		{
			using (new HierarchyViewModelFlagsChangeScope(this.m_HierarchyViewModel))
			{
				this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
				this.m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Selected);
			}
			this.Update();
		}

		public bool IsSelected(in HierarchyNode node)
		{
			return this.m_HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Selected);
		}

		public bool IsSelectedOrAnyAncestorSelected(in HierarchyNode node)
		{
			HierarchyNode hierarchyNode = node;
			for (;;)
			{
				bool flag = (in hierarchyNode) == this.m_Hierarchy.Root;
				if (flag)
				{
					break;
				}
				bool flag2 = this.IsSelected(in hierarchyNode);
				if (flag2)
				{
					goto Block_2;
				}
				hierarchyNode = this.m_HierarchyViewModel.GetParent(in hierarchyNode);
			}
			return false;
			Block_2:
			return true;
		}

		public void ToggleSelected(in HierarchyNode node)
		{
			this.m_HierarchyViewModel.ToggleFlags(in node, HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void ToggleSelected(ReadOnlySpan<HierarchyNode> nodes)
		{
			this.m_HierarchyViewModel.ToggleFlags(nodes, HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void ToggleSelectedRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ToggleFlagsRecursive(in node, HierarchyNodeFlags.Selected, direction);
			this.Update();
		}

		public void ToggleSelectedRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ToggleFlagsRecursive(nodes, HierarchyNodeFlags.Selected, direction);
			this.Update();
		}

		public void ToggleSelection()
		{
			this.m_HierarchyViewModel.ToggleFlags(HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void Deselect(in HierarchyNode node)
		{
			this.m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void Deselect(ReadOnlySpan<HierarchyNode> nodes)
		{
			this.m_HierarchyViewModel.ClearFlags(nodes, HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void DeselectRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Selected, direction);
			this.Update();
		}

		public void DeselectRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ClearFlagsRecursive(nodes, HierarchyNodeFlags.Selected, direction);
			this.Update();
		}

		public void DeselectAll()
		{
			this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
			this.Update();
		}

		public void Expand(in HierarchyNode node)
		{
			this.m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Expanded);
			this.Update();
		}

		public void Expand(ReadOnlySpan<HierarchyNode> nodes)
		{
			this.m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Expanded);
			this.Update();
		}

		public void ExpandRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Expanded, direction);
			this.Update();
		}

		public void ExpandRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.SetFlagsRecursive(nodes, HierarchyNodeFlags.Expanded, direction);
			this.Update();
		}

		public void ExpandAll()
		{
			this.m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Expanded);
			this.Update();
		}

		public bool IsExpanded(in HierarchyNode node)
		{
			return this.m_HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Expanded);
		}

		public void Collapse(in HierarchyNode node)
		{
			this.m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Expanded);
			this.Update();
		}

		public void Collapse(ReadOnlySpan<HierarchyNode> nodes)
		{
			this.m_HierarchyViewModel.ClearFlags(nodes, HierarchyNodeFlags.Expanded);
			this.Update();
		}

		public void CollapseRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Expanded, direction);
			this.Update();
		}

		public void CollapseRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ClearFlagsRecursive(nodes, HierarchyNodeFlags.Expanded, direction);
			this.Update();
		}

		public void CollapseAll()
		{
			this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Expanded);
			this.Update();
		}

		public bool IsCollapsed(in HierarchyNode node)
		{
			return this.m_HierarchyViewModel.DoesNotHaveAllFlags(in node, HierarchyNodeFlags.Expanded);
		}

		public void Show(in HierarchyNode node)
		{
			this.m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Hidden);
			this.Update();
		}

		public void Show(ReadOnlySpan<HierarchyNode> nodes)
		{
			this.m_HierarchyViewModel.ClearFlags(nodes, HierarchyNodeFlags.Hidden);
			this.Update();
		}

		public void ShowRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Hidden, direction);
			this.Update();
		}

		public void ShowRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.ClearFlagsRecursive(nodes, HierarchyNodeFlags.Hidden, direction);
			this.Update();
		}

		public void ShowAll()
		{
			this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Hidden);
			this.Update();
		}

		public bool IsShown(in HierarchyNode node)
		{
			return this.m_HierarchyViewModel.DoesNotHaveAllFlags(in node, HierarchyNodeFlags.Hidden);
		}

		public void Hide(in HierarchyNode node)
		{
			this.m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Hidden);
			this.Update();
		}

		public void Hide(ReadOnlySpan<HierarchyNode> nodes)
		{
			this.m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Hidden);
			this.Update();
		}

		public void HideRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Hidden, direction);
			this.Update();
		}

		public void HideRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
		{
			this.m_HierarchyViewModel.SetFlagsRecursive(nodes, HierarchyNodeFlags.Hidden, direction);
			this.Update();
		}

		public void HideAll()
		{
			this.m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Hidden);
			this.Update();
		}

		public bool IsHidden(in HierarchyNode node)
		{
			return this.m_HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Hidden);
		}

		public void Frame(in HierarchyNode node)
		{
			bool flag = (in node) == HierarchyNode.Null || (in node) == this.m_Hierarchy.Root;
			if (!flag)
			{
				this.ExpandParents(in node);
				this.m_HierarchyViewModel.Update();
				this.UpdateListView();
				this.ScrollToNode(in node);
			}
		}

		public void Frame(ReadOnlySpan<HierarchyNode> nodes)
		{
			bool flag = nodes.Length == 0;
			if (!flag)
			{
				this.ExpandParents(nodes);
				this.m_HierarchyViewModel.Update();
				this.UpdateListView();
				this.ScrollToNode(nodes[0]);
			}
		}

		public void SetColumns(List<Column> columns, HierarchyViewState state = null)
		{
			bool flag = state != null && state.Columns != null && state.Columns.Length != 0;
			if (flag)
			{
				foreach (HierarchyViewColumnState hierarchyViewColumnState in state.Columns)
				{
					Column columnWithId = HierarchyViewColumnUtility.GetColumnWithId(columns, hierarchyViewColumnState.ColumnId);
					bool flag2 = columnWithId == null;
					if (!flag2)
					{
						columnWithId.visible = hierarchyViewColumnState.Visible;
						HierarchyViewColumn.SetWidth(columnWithId, hierarchyViewColumnState.Width);
					}
				}
				columns.Sort(delegate(Column c1, Column c2)
				{
					int visibleIndex = HierarchyViewColumnUtility.GetVisibleIndex(state, c1);
					int visibleIndex2 = HierarchyViewColumnUtility.GetVisibleIndex(state, c2);
					return visibleIndex - visibleIndex2;
				});
			}
			else
			{
				foreach (Column column in columns)
				{
					HierarchyViewColumn hierarchyViewColumn = column as HierarchyViewColumn;
					bool flag3 = hierarchyViewColumn != null;
					if (flag3)
					{
						hierarchyViewColumn.ApplyDefaultColumnProperties();
					}
					else
					{
						HierarchyViewItemColumn hierarchyViewItemColumn = column as HierarchyViewItemColumn;
						bool flag4 = hierarchyViewItemColumn != null;
						if (flag4)
						{
							hierarchyViewItemColumn.ApplyDefaultColumnProperties();
						}
					}
				}
			}
			this.m_MultiColumnListView.columns.Clear();
			foreach (Column column2 in columns)
			{
				this.m_MultiColumnListView.columns.Add(column2);
			}
			this.BindColumns();
		}

		public void SetColumnDescriptors(IEnumerable<HierarchyViewColumnDescriptor> columnDescriptors, IEnumerable<HierarchyViewCellDescriptor> cellDescriptors, HierarchyViewState state = null)
		{
			List<Column> list = new List<Column> { this.NameColumn };
			foreach (HierarchyViewColumnDescriptor hierarchyViewColumnDescriptor in columnDescriptors)
			{
				HierarchyViewColumn hierarchyViewColumn = new HierarchyViewColumn(this, hierarchyViewColumnDescriptor);
				foreach (HierarchyViewCellDescriptor hierarchyViewCellDescriptor in cellDescriptors)
				{
					bool flag = hierarchyViewCellDescriptor.ValidForColumn(hierarchyViewColumnDescriptor);
					if (flag)
					{
						hierarchyViewColumn.AddCell(hierarchyViewCellDescriptor);
					}
				}
				list.Add(hierarchyViewColumn);
			}
			list.Sort(delegate(Column c1, Column c2)
			{
				int columnDefaultPriority = HierarchyViewColumnUtility.GetColumnDefaultPriority(c1);
				int columnDefaultPriority2 = HierarchyViewColumnUtility.GetColumnDefaultPriority(c2);
				return columnDefaultPriority - columnDefaultPriority2;
			});
			this.SetColumns(list, state);
		}

		public void SetState(HierarchyViewState viewState)
		{
			bool flag = (viewState.ValidContent & (HierarchyViewState.Content.ViewModelState | HierarchyViewState.Content.SearchText | HierarchyViewState.Content.Columns)) > HierarchyViewState.Content.Invalid;
			if (flag)
			{
				this.EnqueuePostUpdateAction(delegate
				{
					bool flag3 = viewState.ValidContent.HasFlag(HierarchyViewState.Content.Columns);
					if (flag3)
					{
						this.SetColumnState(viewState);
					}
					bool flag4 = viewState.ValidContent.HasFlag(HierarchyViewState.Content.SearchText);
					if (flag4)
					{
						this.Filter = viewState.SearchText;
					}
					bool flag5 = viewState.ValidContent.HasFlag(HierarchyViewState.Content.ViewModelState);
					if (flag5)
					{
						this.m_HierarchyViewModel.SetState(viewState.ViewModelState);
					}
				});
			}
			bool flag2 = viewState.ValidContent.HasFlag(HierarchyViewState.Content.ScrollPosition);
			if (flag2)
			{
				this.m_MultiColumnListView.scrollView.scrollOffset = new Vector2(viewState.ScrollPositionX, viewState.ScrollPositionY);
			}
		}

		public HierarchyViewState GetState(HierarchyViewState.Content content = HierarchyViewState.Content.All)
		{
			HierarchyViewState hierarchyViewState = new HierarchyViewState(content);
			bool flag = hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.ViewModelState);
			if (flag)
			{
				hierarchyViewState.ViewModelState = this.m_HierarchyViewModel.GetState();
			}
			bool flag2 = hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.SearchText);
			if (flag2)
			{
				hierarchyViewState.SearchText = this.Filter;
			}
			bool flag3 = hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.ScrollPosition);
			if (flag3)
			{
				ScrollView scrollView = this.m_MultiColumnListView.Q<ScrollView>(null, null);
				Vector2 vector = ((scrollView != null) ? scrollView.scrollOffset : new Vector2(-1f, -1f));
				hierarchyViewState.ScrollPositionX = vector.x;
				hierarchyViewState.ScrollPositionY = vector.y;
			}
			bool flag4 = hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.Columns);
			if (flag4)
			{
				hierarchyViewState.Columns = new HierarchyViewColumnState[this.m_MultiColumnListView.columns.Count];
				List<VisualElement> list = this.m_MultiColumnListView.Query<VisualElement>(null, "unity-multi-column-header__column").ToList();
				int num = 0;
				foreach (Column column in this.m_MultiColumnListView.columns)
				{
					string columnId = HierarchyViewColumnUtility.GetColumnId(column);
					int num2 = list.FindIndex((VisualElement header) => header.name == columnId);
					hierarchyViewState.Columns[num] = new HierarchyViewColumnState
					{
						ColumnId = columnId,
						Width = column.width.value,
						Visible = column.visible,
						Index = ((num2 != -1) ? num2 : num)
					};
					num++;
				}
				Array.Sort<HierarchyViewColumnState>(hierarchyViewState.Columns, (HierarchyViewColumnState c1, HierarchyViewColumnState c2) => c1.Index - c2.Index);
			}
			return hierarchyViewState;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal void Initialize()
		{
			this.BindHandlers();
			try
			{
				Action initializing = this.Initializing;
				if (initializing != null)
				{
					initializing();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		internal void Reset()
		{
			this.UnbindHandlers();
			this.m_StyleContainer.Remove(this.m_MultiColumnListView);
			this.m_StyleContainer.RemoveFromHierarchy();
			this.m_StyleContainer = new VisualElement();
			this.m_StyleContainer.AddToClassList("hierarchy__container");
			this.m_StyleContainer.Add(this.m_MultiColumnListView);
			base.Add(this.m_StyleContainer);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal void EnqueuePostUpdateAction(Action action)
		{
			bool locked = this.m_PostUpdateActionQueue.Locked;
			if (locked)
			{
				throw new InvalidOperationException("Cannot enqueue post update action while processing post update actions.");
			}
			this.m_PostUpdateActionQueue.PushBack(in action);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal void BeginRename(in HierarchyNode node)
		{
			int num = this.m_HierarchyViewModel.IndexOf(in node);
			bool flag = num < 0;
			if (!flag)
			{
				HierarchyViewItem hierarchyViewItemFromIndex = this.GetHierarchyViewItemFromIndex(num);
				bool flag2 = hierarchyViewItemFromIndex == null;
				if (!flag2)
				{
					hierarchyViewItemFromIndex.BeginRename();
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal void OnLostFocus()
		{
			this.m_LastMouseUpSelectionIndex = -1;
		}

		internal int GetIndexFromLocalPosition(Vector2 pos)
		{
			return this.m_MultiColumnListView.virtualizationController.GetIndexFromPosition(pos);
		}

		internal int GetIndexFromWorldPosition(Vector2 worldPos, float offset = 0f)
		{
			Vector3 vector = new Vector3(worldPos.x, worldPos.y - offset, 0f);
			Vector2 vector2 = this.m_ListViewContentContainer.WorldToLocal(vector);
			return this.GetIndexFromLocalPosition(vector2);
		}

		internal void InvokeBindViewItem(HierarchyViewItem item)
		{
			HierarchyNodeTypeHandler handler = item.Handler;
			if (handler != null)
			{
				handler.Internal_BindItem(item);
			}
			Action<HierarchyViewItem> bindViewItem = this.BindViewItem;
			if (bindViewItem != null)
			{
				bindViewItem(item);
			}
		}

		internal void InvokeUnbindViewItem(HierarchyViewItem item)
		{
			HierarchyNodeTypeHandler handler = item.Handler;
			if (handler != null)
			{
				handler.Internal_UnbindItem(item);
			}
			Action<HierarchyViewItem> unbindViewItem = this.UnbindViewItem;
			if (unbindViewItem != null)
			{
				unbindViewItem(item);
			}
		}

		internal void InvokePopulateContextMenu(ContextualMenuPopulateEvent evt)
		{
			HierarchyView hierarchyView = evt.target as HierarchyView;
			bool flag = hierarchyView == null;
			if (!flag)
			{
				bool isRenamingItem = this.m_IsRenamingItem;
				if (isRenamingItem)
				{
					HierarchyViewItemName hierarchyViewItemName = this.m_RenamingItem.Q<HierarchyViewItemName>(null, null);
					if (hierarchyViewItemName != null)
					{
						hierarchyViewItemName.CancelRename();
					}
					this.SetRenamingItem(null);
				}
				evt.StopImmediatePropagation();
				Vector2 vector = hierarchyView.ChangeCoordinatesTo(this.m_ListViewContentContainer, evt.localMousePosition);
				int indexFromLocalPosition = this.GetIndexFromLocalPosition(vector);
				HierarchyViewItem hierarchyViewItemFromIndex = this.GetHierarchyViewItemFromIndex(indexFromLocalPosition);
				bool flag2 = hierarchyViewItemFromIndex == null;
				if (flag2)
				{
					this.m_MultiColumnListView.ClearSelection();
					foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in this.m_Hierarchy.EnumerateNodeTypeHandlers())
					{
						IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
						bool flag3 = hierarchyEditorNodeTypeHandler != null;
						if (flag3)
						{
							hierarchyEditorNodeTypeHandler.PopulateContextMenu(this, null, evt.menu);
						}
					}
				}
				else
				{
					IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler2 = hierarchyViewItemFromIndex.Handler as IHierarchyEditorNodeTypeHandler;
					bool flag4 = hierarchyEditorNodeTypeHandler2 != null;
					if (flag4)
					{
						hierarchyEditorNodeTypeHandler2.PopulateContextMenu(this, hierarchyViewItemFromIndex, evt.menu);
					}
				}
				HierarchyView.PopulateContextMenuEventHandler populateContextMenu = this.PopulateContextMenu;
				if (populateContextMenu != null)
				{
					populateContextMenu(hierarchyViewItemFromIndex, evt.menu);
				}
			}
		}

		internal void InvokeGetTooltip(HierarchyViewItem item, bool filtering, StringBuilder tooltip)
		{
			IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = item.Handler as IHierarchyEditorNodeTypeHandler;
			bool flag = hierarchyEditorNodeTypeHandler != null;
			if (flag)
			{
				hierarchyEditorNodeTypeHandler.GetTooltip(item, filtering, tooltip);
			}
			HierarchyView.GetTooltipEventHandler getTooltip = this.GetTooltip;
			if (getTooltip != null)
			{
				getTooltip(item, filtering, tooltip);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal void PingNode(HierarchyNode node)
		{
			bool flag = (in node) == HierarchyNode.Null || (in node) == this.m_Hierarchy.Root;
			if (!flag)
			{
				bool flag2 = !this.m_Hierarchy.Exists(in node);
				if (!flag2)
				{
					this.ExpandParents(in node);
					this.Update();
					int num = this.m_HierarchyViewModel.IndexOf(in node);
					bool flag3 = num < 0;
					if (!flag3)
					{
						this.m_MultiColumnListView.ScrollToItem(num);
						Action <>9__1;
						this.EnqueuePostUpdateAction(delegate
						{
							IVisualElementScheduler schedule = this.schedule;
							Action action;
							if ((action = <>9__1) == null)
							{
								action = (<>9__1 = delegate
								{
									this.DoPingAnimation(node);
								});
							}
							schedule.Execute(action);
						});
					}
				}
			}
		}

		private void DoPingAnimation(HierarchyNode node)
		{
			int num = this.m_HierarchyViewModel.IndexOf(in node);
			bool flag = num < 0;
			if (!flag)
			{
				HierarchyViewItem hierarchyViewItemFromIndex = this.GetHierarchyViewItemFromIndex(num);
				bool flag2 = hierarchyViewItemFromIndex == null;
				if (!flag2)
				{
					VisualElement rowContainer = hierarchyViewItemFromIndex.RowContainer;
					bool flag3 = rowContainer == null;
					if (!flag3)
					{
						bool flag4 = rowContainer.ClassListContains("hierarchy - item__ping-base");
						if (!flag4)
						{
							rowContainer.AddToClassList("hierarchy - item__ping-base");
							EventCallback<TransitionEndEvent> <>9__2;
							EventCallback<TransitionEndEvent> <>9__1;
							rowContainer.schedule.Execute(delegate
							{
								rowContainer.AddToClassList("hierarchy-item__ping-ramp-in-style");
								rowContainer.AddToClassList("hierarchy-item__ping-ramp-in-start");
								CallbackEventHandler rowContainer3 = rowContainer;
								EventCallback<TransitionEndEvent> eventCallback;
								if ((eventCallback = <>9__1) == null)
								{
									eventCallback = (<>9__1 = delegate(TransitionEndEvent _)
									{
										rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-in-start");
										rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-in-style");
										rowContainer.AddToClassList("hierarchy-item__ping-ramp-out-start");
										rowContainer.AddToClassList("hierarchy-item__ping-ramp-out-style");
										CallbackEventHandler rowContainer2 = rowContainer;
										EventCallback<TransitionEndEvent> eventCallback2;
										if ((eventCallback2 = <>9__2) == null)
										{
											eventCallback2 = (<>9__2 = delegate(TransitionEndEvent _)
											{
												rowContainer.RemoveFromClassList("hierarchy - item__ping-base");
												rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-out-start");
												rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-out-style");
											});
										}
										rowContainer2.RegisterCallbackOnce<TransitionEndEvent>(eventCallback2, TrickleDown.NoTrickleDown);
									});
								}
								rowContainer3.RegisterCallbackOnce<TransitionEndEvent>(eventCallback, TrickleDown.NoTrickleDown);
							});
						}
					}
				}
			}
		}

		internal void ScrollToNode(in HierarchyNode node)
		{
			bool flag = (in node) == HierarchyNode.Null || (in node) == this.m_Hierarchy.Root;
			if (!flag)
			{
				int num = this.m_HierarchyViewModel.IndexOf(in node);
				bool flag2 = num >= 0;
				if (flag2)
				{
					this.m_MultiColumnListView.ScrollToItem(num);
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal void ExpandParents(in HierarchyNode node)
		{
			bool flag = (in node) == HierarchyNode.Null || (in node) == this.m_Hierarchy.Root;
			if (!flag)
			{
				HierarchyNode parent = this.m_Hierarchy.GetParent(in node);
				bool flag2 = (in parent) == HierarchyNode.Null || (in parent) == this.m_Hierarchy.Root;
				if (!flag2)
				{
					this.m_HierarchyViewModel.SetFlagsRecursive(in parent, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Parents);
				}
			}
		}

		internal unsafe void ExpandParents(ReadOnlySpan<HierarchyNode> nodes)
		{
			using (RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(nodes.Length, true))
			{
				int i = 0;
				int length = nodes.Length;
				while (i < length)
				{
					readonly ref HierarchyNode ptr = ref nodes[i];
					bool flag = (in ptr) == HierarchyNode.Null || (in ptr) == this.m_Hierarchy.Root;
					if (!flag)
					{
						*rentSpanUnmanaged.Span[i] = this.m_Hierarchy.GetParent(in ptr);
					}
					i++;
				}
				this.m_HierarchyViewModel.SetFlagsRecursive(rentSpanUnmanaged.Span, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Parents);
			}
		}

		internal void SelectChildrenAndExpandRecursive()
		{
			int num = this.m_HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
			bool flag = num == 0;
			if (!flag)
			{
				RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(num, false);
				try
				{
					this.m_HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, in rentSpanUnmanaged);
					this.m_HierarchyViewModel.SetFlagsRecursive(in rentSpanUnmanaged, HierarchyNodeFlags.Expanded | HierarchyNodeFlags.Selected, HierarchyTraversalDirection.Children);
					this.Update();
				}
				finally
				{
					rentSpanUnmanaged.Dispose();
				}
			}
		}

		internal void SetRenamingItem(HierarchyViewItem item)
		{
			this.m_RenamingItem = item;
		}

		private void BindHandlers()
		{
			bool flag = this.m_Hierarchy == null || !this.m_Hierarchy.IsCreated;
			if (!flag)
			{
				foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in this.m_Hierarchy.EnumerateNodeTypeHandlers())
				{
					hierarchyNodeTypeHandler.Internal_BindView(this);
				}
			}
		}

		private void UnbindHandlers()
		{
			bool flag = this.m_Hierarchy == null || !this.m_Hierarchy.IsCreated;
			if (!flag)
			{
				foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in this.m_Hierarchy.EnumerateNodeTypeHandlers())
				{
					hierarchyNodeTypeHandler.Internal_UnbindView(this);
				}
			}
		}

		private void OnClickEvent(ClickEvent evt)
		{
			IVisualElementScheduledItem scheduledItem = this.m_ScheduledItem;
			if (scheduledItem != null)
			{
				scheduledItem.Pause();
			}
			bool flag = evt.button != 0;
			if (!flag)
			{
				int indexFromLocalPosition = this.GetIndexFromLocalPosition(evt.localPosition);
				HierarchyViewItem item = this.GetHierarchyViewItemFromIndex(indexFromLocalPosition);
				bool flag2 = item == null;
				if (!flag2)
				{
					Vector3 position = evt.position;
					HierarchyViewItemName hierarchyViewItemName = item.Q<HierarchyViewItemName>(null, null);
					bool flag3 = indexFromLocalPosition == this.m_LastMouseUpSelectionIndex && evt.clickCount == 1 && hierarchyViewItemName != null && hierarchyViewItemName.worldBound.Contains(position);
					if (flag3)
					{
						bool flag4 = this.m_RenameDelayMs == 0;
						if (flag4)
						{
							item.BeginRename();
						}
						else
						{
							this.m_ScheduledItem = base.schedule.Execute(delegate
							{
								item.BeginRename();
								this.m_ScheduledItem = null;
							}).StartingIn((long)this.m_RenameDelayMs);
						}
					}
					else
					{
						bool flag5 = evt.clickCount == 2;
						if (flag5)
						{
							readonly ref HierarchyNode ptr = ref this.m_HierarchyViewModel[indexFromLocalPosition];
							HierarchyNodeTypeHandler nodeTypeHandler = this.m_Hierarchy.GetNodeTypeHandler(in ptr);
							IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = nodeTypeHandler as IHierarchyEditorNodeTypeHandler;
							bool flag6 = hierarchyEditorNodeTypeHandler != null;
							if (flag6)
							{
								hierarchyEditorNodeTypeHandler.OnDoubleClick(this, in ptr);
							}
						}
					}
					this.m_LastMouseUpSelectionIndex = indexFromLocalPosition;
				}
			}
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			bool flag = !this.m_SelectedIndicesChangedFromPointerDown;
			if (!flag)
			{
				HierarchyViewModel.FlagsChangedEventHandler flagsChanged = this.FlagsChanged;
				if (flagsChanged != null)
				{
					flagsChanged(HierarchyNodeFlags.Selected);
				}
				this.m_SelectedIndicesChangedFromPointerDown = false;
			}
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			bool isRenamingItem = this.m_IsRenamingItem;
			if (!isRenamingItem)
			{
				bool flag = true;
				KeyCode keyCode = evt.keyCode;
				KeyCode keyCode2 = keyCode;
				if (keyCode2 != KeyCode.Escape)
				{
					switch (keyCode2)
					{
					case KeyCode.Home:
					case KeyCode.PageUp:
						this.m_MultiColumnListView.SetSelection(0);
						break;
					case KeyCode.End:
					case KeyCode.PageDown:
						this.m_MultiColumnListView.SetSelection(this.m_MultiColumnListView.itemsSource.Count - 1);
						break;
					default:
						flag = false;
						break;
					}
				}
				else
				{
					this.m_SelectedIndices.Clear();
					this.m_SelectedIndicesChangedFromPointerDown = false;
				}
				this.m_ListViewContentContainer.Focus();
				bool flag2 = flag;
				if (flag2)
				{
					evt.StopPropagation();
				}
			}
		}

		private void OnNavigationMove(NavigationMoveEvent evt)
		{
			bool isRenamingItem = this.m_IsRenamingItem;
			if (!isRenamingItem)
			{
				bool flag = true;
				int selectedIndex = this.m_MultiColumnListView.selectedIndex;
				bool flag2 = selectedIndex == -1;
				if (flag2)
				{
					NavigationMoveEvent.Direction direction = evt.direction;
					NavigationMoveEvent.Direction direction2 = direction;
					if (direction2 != NavigationMoveEvent.Direction.Up && direction2 != NavigationMoveEvent.Direction.Down)
					{
						flag = false;
					}
					else
					{
						this.m_MultiColumnListView.SetSelection(0);
					}
					this.m_ListViewContentContainer.Focus();
				}
				else
				{
					NavigationMoveEvent.Direction direction3 = evt.direction;
					NavigationMoveEvent.Direction direction4 = direction3;
					if (direction4 != NavigationMoveEvent.Direction.Left && direction4 != NavigationMoveEvent.Direction.Right)
					{
						flag = false;
					}
					else
					{
						int num = this.m_HierarchyViewModel.HasAnyFlagsCount(HierarchyNodeFlags.Selected);
						RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(num, false);
						try
						{
							this.m_HierarchyViewModel.GetNodesWithAnyFlags(HierarchyNodeFlags.Selected, in rentSpanUnmanaged);
							this.SetExpandedState(in rentSpanUnmanaged, evt.direction == NavigationMoveEvent.Direction.Right, evt.altKey);
						}
						finally
						{
							rentSpanUnmanaged.Dispose();
						}
					}
				}
				bool flag3 = flag;
				if (flag3)
				{
					evt.StopPropagation();
				}
			}
		}

		private void OnNavigationCancel(NavigationCancelEvent evt)
		{
			this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Cut);
			this.Update();
			evt.StopImmediatePropagation();
		}

		private void OnListViewClick(ClickEvent evt)
		{
			VisualElement visualElement = evt.target as VisualElement;
			bool flag = visualElement != this.m_MultiColumnListView.Q(null, ScrollView.contentAndVerticalScrollUssClassName);
			if (!flag)
			{
				this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
				this.Update();
				this.m_LastMouseUpSelectionIndex = -1;
				evt.StopImmediatePropagation();
			}
		}

		private void OnUnbindItem(HierarchyViewItem element)
		{
			element.ExpandedStateChanged -= new HierarchyViewItem.ExpandedStateChangedEventHandler(this.SetExpandedState);
		}

		private void OnHandlerCreated(HierarchyNodeTypeHandlerBase handler)
		{
			this.Reset();
			this.Initialize();
		}

		private HierarchyViewItem GetHierarchyViewItemFromIndex(int index)
		{
			bool flag = index == -1;
			HierarchyViewItem hierarchyViewItem;
			if (flag)
			{
				hierarchyViewItem = null;
			}
			else
			{
				VisualElement rootElementForIndex = this.m_MultiColumnListView.GetRootElementForIndex(index);
				HierarchyViewItem hierarchyViewItem2 = ((rootElementForIndex != null) ? rootElementForIndex.Q<HierarchyViewItem>(null, null) : null);
				hierarchyViewItem = hierarchyViewItem2;
			}
			return hierarchyViewItem;
		}

		private unsafe void OnSelectedIndicesChanged(IEnumerable<int> indices)
		{
			bool flag = false;
			this.m_SelectedIndices.Clear();
			foreach (int num in indices)
			{
				bool flag2 = num < 0;
				if (!flag2)
				{
					bool flag3 = num == this.m_LastMouseUpSelectionIndex;
					if (flag3)
					{
						flag = true;
					}
					this.m_SelectedIndices.Add(num);
				}
			}
			bool flag4 = !flag;
			if (flag4)
			{
				this.m_LastMouseUpSelectionIndex = -1;
			}
			using (RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(this.m_SelectedIndices.Count, true))
			{
				for (int i = 0; i < this.m_SelectedIndices.Count; i++)
				{
					int num2 = this.m_SelectedIndices[i];
					bool flag5 = num2 < 0 || num2 >= this.m_HierarchyViewModel.Count;
					if (!flag5)
					{
						*rentSpanUnmanaged.Span[i] = *this.m_HierarchyViewModel[num2];
					}
				}
				this.m_SelectedIndices.Clear();
				using (new HierarchyViewModelFlagsChangeScope(this.m_HierarchyViewModel, false))
				{
					this.m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
					this.m_HierarchyViewModel.SetFlags(rentSpanUnmanaged.Span, HierarchyNodeFlags.Selected);
				}
				bool flag6 = this.m_MultiColumnListView.pointerProcessingState == BaseVerticalCollectionView.pointerProcessingStateEnum.PointerDown && this.m_MultiColumnListView.currentPointerButton != 1;
				if (flag6)
				{
					this.m_SelectedIndicesChangedFromPointerDown = true;
				}
				else
				{
					HierarchyViewModel.FlagsChangedEventHandler flagsChanged = this.FlagsChanged;
					if (flagsChanged != null)
					{
						flagsChanged(HierarchyNodeFlags.Selected);
					}
				}
			}
		}

		private void OnBindItem(HierarchyViewItem item)
		{
			item.ExpandedStateChanged += new HierarchyViewItem.ExpandedStateChangedEventHandler(this.SetExpandedState);
		}

		private void SetExpandedState(in HierarchyNode node, bool isExpanded, bool recurse)
		{
			if (isExpanded)
			{
				if (recurse)
				{
					this.ExpandRecursive(in node, HierarchyTraversalDirection.Children);
				}
				else
				{
					this.Expand(in node);
				}
			}
			else if (recurse)
			{
				this.CollapseRecursive(in node, HierarchyTraversalDirection.Children);
			}
			else
			{
				this.Collapse(in node);
			}
		}

		private void SetExpandedState(ReadOnlySpan<HierarchyNode> nodes, bool isExpanded, bool recurse)
		{
			if (isExpanded)
			{
				if (recurse)
				{
					this.ExpandRecursive(nodes, HierarchyTraversalDirection.Children);
				}
				else
				{
					this.Expand(nodes);
				}
			}
			else if (recurse)
			{
				this.CollapseRecursive(nodes, HierarchyTraversalDirection.Children);
			}
			else
			{
				this.Collapse(nodes);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal bool UpdateListView()
		{
			bool flag = this.m_Version == this.m_HierarchyViewModel.Version;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.m_MultiColumnListView.RefreshItems();
				int num = this.m_HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
				bool flag3 = num == 0;
				if (flag3)
				{
					this.SetListViewSelectionWithoutNotify(Array.Empty<int>());
				}
				else
				{
					RentSpanUnmanaged<int> rentSpanUnmanaged = new RentSpanUnmanaged<int>(num, false);
					try
					{
						this.m_HierarchyViewModel.GetIndicesWithAllFlags(HierarchyNodeFlags.Selected, in rentSpanUnmanaged);
						this.SetListViewSelectionWithoutNotify(in rentSpanUnmanaged);
					}
					finally
					{
						rentSpanUnmanaged.Dispose();
					}
				}
				this.m_Version = this.m_HierarchyViewModel.Version;
				flag2 = false;
			}
			return flag2;
		}

		private unsafe void SetListViewSelectionWithoutNotify(Span<int> selection)
		{
			using (RentSpanUnmanaged<int> rentSpanUnmanaged = new RentSpanUnmanaged<int>(selection.Length, false))
			{
				int num = 0;
				for (int i = 0; i < selection.Length; i++)
				{
					int num2 = *selection[i];
					bool flag = num2 >= 0 && num2 < this.m_HierarchyViewModel.Count;
					if (flag)
					{
						*rentSpanUnmanaged.Span[num++] = num2;
					}
				}
				this.m_MultiColumnListView.SetSelectionWithoutNotify(rentSpanUnmanaged.Span.Slice(0, num));
			}
		}

		private void SetColumnState(HierarchyViewState state)
		{
			List<Column> list = CollectionPool<List<Column>, Column>.Get();
			foreach (Column column in this.m_MultiColumnListView.columns)
			{
				list.Add(column);
			}
			this.SetColumns(list, state);
		}

		private void ClearColumns()
		{
			List<VisualElement> list = this.m_MultiColumnListView.Query<VisualElement>("unity-multi-column-view__row-container", null).ToList();
			foreach (VisualElement visualElement in list)
			{
				List<HierarchyViewCell> list2 = visualElement.Query<HierarchyViewCell>("HierarchyViewCell", null).ToList();
				foreach (HierarchyViewCell hierarchyViewCell in list2)
				{
					bool flag = hierarchyViewCell.Descriptor == null;
					if (!flag)
					{
						hierarchyViewCell.UnbindCell();
					}
				}
			}
			foreach (Column column in this.m_MultiColumnListView.columns)
			{
				HierarchyViewColumn hierarchyViewColumn = column as HierarchyViewColumn;
				bool flag2 = hierarchyViewColumn != null;
				if (flag2)
				{
					hierarchyViewColumn.UnbindColumn(this);
				}
			}
		}

		private void BindColumns()
		{
			foreach (Column column in this.m_MultiColumnListView.columns)
			{
				HierarchyViewColumn hierarchyViewColumn = column as HierarchyViewColumn;
				bool flag = hierarchyViewColumn != null;
				if (flag)
				{
					hierarchyViewColumn.BindColumn(this);
				}
			}
		}

		private bool DoUpdate(HierarchyView.UpdateMode mode, double milliseconds = 0.0)
		{
			bool flag = this.<DoUpdate>g__DoUpdateStage|185_4(mode, milliseconds);
			bool flag2 = !flag;
			if (flag2)
			{
				this.<DoUpdate>g__IncrementUpdateStage|185_5();
			}
			return flag || this.UpdateNeeded;
		}

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool <DoUpdate>g__UpdateHierarchy|185_0(HierarchyView.UpdateMode mode, double milliseconds)
		{
			bool flag = this.m_Hierarchy == null || !this.m_Hierarchy.IsCreated || !this.m_Hierarchy.UpdateNeeded;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				switch (mode)
				{
				case HierarchyView.UpdateMode.Update:
					this.m_Hierarchy.Update();
					flag2 = false;
					break;
				case HierarchyView.UpdateMode.UpdateIncremental:
					flag2 = this.m_Hierarchy.UpdateIncremental();
					break;
				case HierarchyView.UpdateMode.UpdateIncrementalTimed:
					flag2 = this.m_Hierarchy.UpdateIncrementalTimed(milliseconds);
					break;
				default:
					throw new NotImplementedException(mode.ToString());
				}
			}
			return flag2;
		}

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool <DoUpdate>g__UpdateHierarchyFlattened|185_1(HierarchyView.UpdateMode mode, double milliseconds)
		{
			bool flag = this.m_HierarchyFlattened == null || !this.m_HierarchyFlattened.IsCreated || !this.m_HierarchyFlattened.UpdateNeeded;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				switch (mode)
				{
				case HierarchyView.UpdateMode.Update:
					this.m_HierarchyFlattened.Update();
					flag2 = false;
					break;
				case HierarchyView.UpdateMode.UpdateIncremental:
					flag2 = this.m_HierarchyFlattened.UpdateIncremental();
					break;
				case HierarchyView.UpdateMode.UpdateIncrementalTimed:
					flag2 = this.m_HierarchyFlattened.UpdateIncrementalTimed(milliseconds);
					break;
				default:
					throw new NotImplementedException(mode.ToString());
				}
			}
			return flag2;
		}

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool <DoUpdate>g__UpdateHierarchyViewModel|185_2(HierarchyView.UpdateMode mode, double milliseconds)
		{
			bool flag = this.m_HierarchyViewModel == null || !this.m_HierarchyViewModel.IsCreated || !this.m_HierarchyViewModel.UpdateNeeded;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				switch (mode)
				{
				case HierarchyView.UpdateMode.Update:
					this.m_HierarchyViewModel.Update();
					flag2 = false;
					break;
				case HierarchyView.UpdateMode.UpdateIncremental:
					flag2 = this.m_HierarchyViewModel.UpdateIncremental();
					break;
				case HierarchyView.UpdateMode.UpdateIncrementalTimed:
					flag2 = this.m_HierarchyViewModel.UpdateIncrementalTimed(milliseconds);
					break;
				default:
					throw new NotImplementedException(mode.ToString());
				}
			}
			return flag2;
		}

		[CompilerGenerated]
		internal static void <DoUpdate>g__ExecuteActions|185_3(CircularBuffer<Action> actions)
		{
			bool flag = !actions.IsEmpty;
			if (flag)
			{
			}
			while (!actions.IsEmpty)
			{
				Action action = actions.Front();
				try
				{
					actions.Locked = true;
					if (action != null)
					{
						action();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				finally
				{
					actions.Locked = false;
					actions.PopFront();
				}
			}
		}

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool <DoUpdate>g__DoUpdateStage|185_4(HierarchyView.UpdateMode mode, double milliseconds)
		{
			bool flag;
			switch (this.m_UpdateStage)
			{
			case HierarchyView.UpdateStage.UpdatingHierarchy:
				flag = this.<DoUpdate>g__UpdateHierarchy|185_0(mode, milliseconds);
				break;
			case HierarchyView.UpdateStage.UpdatingHierarchyFlattened:
				flag = this.<DoUpdate>g__UpdateHierarchyFlattened|185_1(mode, milliseconds);
				break;
			case HierarchyView.UpdateStage.UpdatingHierarchyViewModel:
				flag = this.<DoUpdate>g__UpdateHierarchyViewModel|185_2(mode, milliseconds);
				break;
			case HierarchyView.UpdateStage.UpdatingListView:
				flag = this.UpdateListView();
				break;
			case HierarchyView.UpdateStage.ExecutePostUpdateActions:
				HierarchyView.<DoUpdate>g__ExecuteActions|185_3(this.m_PostUpdateActionQueue);
				flag = false;
				break;
			default:
				throw new NotImplementedException(this.m_UpdateStage.ToString());
			}
			return flag;
		}

		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void <DoUpdate>g__IncrementUpdateStage|185_5()
		{
			this.m_UpdateStage = (this.m_UpdateStage + 1) % HierarchyView.UpdateStage.Count;
		}

		internal const int k_ItemHeight = 20;

		private const string k_ListViewName = "unity-tree-view__list-view";

		private const string k_HierarchyViewRootStyleName = "hierarchy";

		private const string k_HierarchyViewStyleContainerStyleName = "hierarchy__container";

		private const int k_RenamingDelayMs = 500;

		internal const string k_HierarchyPingBase = "hierarchy - item__ping-base";

		private const string k_HierarchyPingRampIn_Style = "hierarchy-item__ping-ramp-in-style";

		private const string k_HierarchyPingRampIn_Start = "hierarchy-item__ping-ramp-in-start";

		private const string k_HierarchyPingRampOut_Style = "hierarchy-item__ping-ramp-out-style";

		private const string k_HierarchyPingRampOut_Start = "hierarchy-item__ping-ramp-out-start";

		private Unity.Hierarchy.Hierarchy m_Hierarchy;

		private HierarchyFlattened m_HierarchyFlattened;

		private HierarchyViewModel m_HierarchyViewModel;

		private int m_Version;

		private HierarchyView.UpdateStage m_UpdateStage = HierarchyView.UpdateStage.UpdatingHierarchy;

		private readonly Stopwatch m_UpdateTimer = new Stopwatch();

		private readonly CircularBuffer<Action> m_PostUpdateActionQueue = new CircularBuffer<Action>(16);

		private readonly MultiColumnListView m_MultiColumnListView;

		private readonly HierarchyViewItemColumn m_NameColumn;

		private readonly HierarchyViewDragHandler m_DragHandler;

		private readonly VisualElement m_ListViewContentContainer;

		private VisualElement m_StyleContainer;

		private IVisualElementScheduledItem m_ScheduledItem;

		private readonly List<int> m_SelectedIndices = new List<int>();

		private bool m_SelectedIndicesChangedFromPointerDown;

		private int m_LastMouseUpSelectionIndex;

		private HierarchyViewItem m_RenamingItem;

		internal int m_RenameDelayMs;

		private enum UpdateStage
		{
			UpdatingHierarchy,
			UpdatingHierarchyFlattened,
			UpdatingHierarchyViewModel,
			UpdatingListView,
			ExecutePostUpdateActions,
			Count,
			First = 0,
			Last = 4
		}

		private enum UpdateMode
		{
			Update,
			UpdateIncremental,
			UpdateIncrementalTimed
		}

		internal class TestHelper
		{
			public static int FirstUpdateStage
			{
				get
				{
					return 0;
				}
			}

			public static int LastUpdateStage
			{
				get
				{
					return 4;
				}
			}

			public static int HierarchyUpdateStage
			{
				get
				{
					return 0;
				}
			}

			public static int CurrentUpdateStage(HierarchyView view)
			{
				return (int)view.m_UpdateStage;
			}

			public static bool ViewUpdateNeeded(HierarchyView view)
			{
				return view.UpdateNeeded;
			}

			public static bool HierarchyUpdateNeeded(HierarchyView view)
			{
				return view.m_Hierarchy.UpdateNeeded;
			}
		}

		public delegate void SourceHierarchyChangingEventHandler(Unity.Hierarchy.Hierarchy oldHierarchy, Unity.Hierarchy.Hierarchy newHierarchy, HierarchyNodeFlags defaultFlags);

		public delegate void SourceHierarchyChangedEventHandler(Unity.Hierarchy.Hierarchy hierarchy, HierarchyNodeFlags defaultFlags);

		public delegate void PopulateContextMenuEventHandler(HierarchyViewItem item, DropdownMenu menu);

		public delegate void GetTooltipEventHandler(HierarchyViewItem item, bool filtering, StringBuilder tooltip);
	}
}
