using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnityEngine.UIElements
{
	public class ListView : BindableElement, ISerializationCallbackReceiver
	{
		[Obsolete("onItemChosen is obsolete, use onItemsChosen instead")]
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<object> onItemChosen;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<object>> onItemsChosen;

		[Obsolete("onSelectionChanged is obsolete, use onSelectionChange instead")]
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<List<object>> onSelectionChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<object>> onSelectionChange;

		public IList itemsSource
		{
			get
			{
				return this.m_ItemsSource;
			}
			set
			{
				this.m_ItemsSource = value;
				this.Refresh();
			}
		}

		public Func<VisualElement> makeItem
		{
			get
			{
				return this.m_MakeItem;
			}
			set
			{
				bool flag = this.m_MakeItem == value;
				if (!flag)
				{
					this.m_MakeItem = value;
					this.Refresh();
				}
			}
		}

		public Action<VisualElement, int> unbindItem { get; set; }

		public Action<VisualElement, int> bindItem
		{
			get
			{
				return this.m_BindItem;
			}
			set
			{
				this.m_BindItem = value;
				this.Refresh();
			}
		}

		internal Func<int, int> getItemId
		{
			get
			{
				return this.m_GetItemId;
			}
			set
			{
				this.m_GetItemId = value;
				this.Refresh();
			}
		}

		public float resolvedItemHeight
		{
			get
			{
				float scaledPixelsPerPoint = base.scaledPixelsPerPoint;
				return Mathf.Round((float)this.itemHeight * scaledPixelsPerPoint) / scaledPixelsPerPoint;
			}
		}

		internal List<ListView.RecycledItem> Pool
		{
			get
			{
				return this.m_Pool;
			}
		}

		public int itemHeight
		{
			get
			{
				return this.m_ItemHeight;
			}
			set
			{
				this.m_ItemHeightIsInline = true;
				bool flag = this.m_ItemHeight != value;
				if (flag)
				{
					this.m_ItemHeight = value;
					this.Refresh();
				}
			}
		}

		public bool showBorder
		{
			get
			{
				return base.ClassListContains(ListView.borderUssClassName);
			}
			set
			{
				base.EnableInClassList(ListView.borderUssClassName, value);
			}
		}

		public bool reorderable
		{
			get
			{
				ListViewDragger dragger = this.m_Dragger;
				IListViewDragAndDropController listViewDragAndDropController = ((dragger != null) ? dragger.dragAndDropController : null);
				return listViewDragAndDropController != null && listViewDragAndDropController.enableReordering;
			}
			set
			{
				ListViewDragger dragger = this.m_Dragger;
				bool flag = ((dragger != null) ? dragger.dragAndDropController : null) == null;
				if (flag)
				{
					if (value)
					{
						this.SetDragAndDropController(new ListViewReorderableDragAndDropController(this));
					}
				}
				else
				{
					IListViewDragAndDropController dragAndDropController = this.m_Dragger.dragAndDropController;
					bool flag2 = dragAndDropController != null;
					if (flag2)
					{
						dragAndDropController.enableReordering = value;
					}
				}
			}
		}

		internal List<int> currentSelectionIds
		{
			get
			{
				return this.m_SelectedIds;
			}
		}

		public int selectedIndex
		{
			get
			{
				return (this.m_SelectedIndices.Count == 0) ? (-1) : this.m_SelectedIndices.First<int>();
			}
			set
			{
				this.SetSelection(value);
			}
		}

		public IEnumerable<int> selectedIndices
		{
			get
			{
				return this.m_SelectedIndices;
			}
		}

		public object selectedItem
		{
			get
			{
				return (this.m_SelectedItems.Count == 0) ? null : this.m_SelectedItems.First<object>();
			}
		}

		public IEnumerable<object> selectedItems
		{
			get
			{
				return this.m_SelectedItems;
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return null;
			}
		}

		public SelectionType selectionType
		{
			get
			{
				return this.m_SelectionType;
			}
			set
			{
				this.m_SelectionType = value;
				bool flag = this.m_SelectionType == SelectionType.None;
				if (flag)
				{
					this.ClearSelection();
				}
			}
		}

		public AlternatingRowBackground showAlternatingRowBackgrounds
		{
			get
			{
				return this.m_ShowAlternatingRowBackgrounds;
			}
			set
			{
				bool flag = this.m_ShowAlternatingRowBackgrounds == value;
				if (!flag)
				{
					this.m_ShowAlternatingRowBackgrounds = value;
					this.Refresh();
				}
			}
		}

		public bool showBoundCollectionSize { get; set; } = true;

		public bool horizontalScrollingEnabled
		{
			get
			{
				return this.m_HorizontalScrollingEnabled;
			}
			set
			{
				bool flag = this.m_HorizontalScrollingEnabled == value;
				if (!flag)
				{
					this.m_HorizontalScrollingEnabled = value;
					this.m_ScrollView.SetScrollViewMode(value ? ScrollViewMode.VerticalAndHorizontal : ScrollViewMode.Vertical);
				}
			}
		}

		public ListView()
		{
			base.AddToClassList(ListView.ussClassName);
			this.selectionType = SelectionType.Single;
			this.m_ScrollOffset = 0f;
			this.m_ScrollView = new ScrollView();
			this.m_ScrollView.viewDataKey = "list-view__scroll-view";
			this.m_ScrollView.StretchToParentSize();
			this.m_ScrollView.verticalScroller.valueChanged += this.OnScroll;
			base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChanged), TrickleDown.NoTrickleDown);
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
			base.hierarchy.Add(this.m_ScrollView);
			this.m_ScrollView.contentContainer.focusable = true;
			this.m_ScrollView.contentContainer.usageHints &= ~UsageHints.GroupTransform;
			this.m_EmptyRows = new VisualElement();
			this.m_EmptyRows.AddToClassList(ListView.s_BackgroundFillUssClassName);
			base.focusable = true;
			base.isCompositeRoot = true;
			base.delegatesFocus = true;
		}

		public ListView(IList itemsSource, int itemHeight, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
			: this()
		{
			this.m_ItemsSource = itemsSource;
			this.m_ItemHeight = itemHeight;
			this.m_ItemHeightIsInline = true;
			this.m_MakeItem = makeItem;
			this.m_BindItem = bindItem;
		}

		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			bool flag = evt.destinationPanel == null;
			if (!flag)
			{
				this.m_ScrollView.contentContainer.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
			}
		}

		private void OnDetachFromPanel(DetachFromPanelEvent evt)
		{
			bool flag = evt.originPanel == null;
			if (!flag)
			{
				this.m_ScrollView.contentContainer.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.UnregisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown), TrickleDown.NoTrickleDown);
			}
		}

		private bool ProcessNavigationEvent(EventBase evt, out bool shouldScroll)
		{
			shouldScroll = false;
			bool flag = evt == null || !this.HasValidDataAndBindings();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = ListView.IsSelectAllEvent(evt);
				if (flag3)
				{
					this.SelectAll();
					flag2 = true;
				}
				else
				{
					bool flag4 = base.eventInterpreter.IsCancellationEvent(evt);
					if (flag4)
					{
						this.ClearSelection();
						flag2 = true;
					}
					else
					{
						bool flag5 = base.eventInterpreter.IsActivationEvent(evt);
						if (flag5)
						{
							bool flag6 = this.selectedIndex >= 0 && this.selectedIndex < this.m_ItemsSource.Count;
							if (flag6)
							{
								Action<object> action = this.onItemChosen;
								if (action != null)
								{
									action(this.m_ItemsSource[this.selectedIndex]);
								}
							}
							Action<IEnumerable<object>> action2 = this.onItemsChosen;
							if (action2 != null)
							{
								action2(this.m_SelectedItems);
							}
							shouldScroll = true;
							flag2 = true;
						}
						else
						{
							NavigationDirection navigationDirection;
							bool flag7 = base.eventInterpreter.IsNavigationEvent(evt, out navigationDirection);
							if (flag7)
							{
								shouldScroll = true;
								switch (navigationDirection)
								{
								case NavigationDirection.Up:
								{
									bool flag8 = this.selectedIndex > 0;
									if (flag8)
									{
										int num = this.selectedIndex;
										this.selectedIndex = num - 1;
										return true;
									}
									break;
								}
								case NavigationDirection.Down:
								{
									bool flag9 = this.selectedIndex + 1 < this.itemsSource.Count;
									if (flag9)
									{
										int num = this.selectedIndex;
										this.selectedIndex = num + 1;
										return true;
									}
									break;
								}
								case NavigationDirection.PageUp:
									this.selectedIndex = Math.Max(0, this.selectedIndex - (int)(this.m_LastHeight / this.resolvedItemHeight));
									return true;
								case NavigationDirection.PageDown:
									this.selectedIndex = Math.Min(this.itemsSource.Count - 1, this.selectedIndex + (int)(this.m_LastHeight / this.resolvedItemHeight));
									return true;
								case NavigationDirection.Home:
									this.selectedIndex = 0;
									return true;
								case NavigationDirection.End:
									this.selectedIndex = this.itemsSource.Count - 1;
									return true;
								}
							}
							flag2 = false;
						}
					}
				}
			}
			return flag2;
		}

		private static bool IsSelectAllEvent(EventBase evt)
		{
			bool flag = evt.eventTypeId == EventBase<KeyDownEvent>.TypeId();
			if (flag)
			{
				KeyDownEvent keyDownEvent = (KeyDownEvent)evt;
				bool flag2 = keyDownEvent.keyCode == KeyCode.A && keyDownEvent.actionKey;
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		private void ProcessAnyEvent(EventBase evt)
		{
			bool flag2;
			bool flag = this.ProcessNavigationEvent(evt, out flag2);
			if (flag)
			{
				evt.StopPropagation();
				evt.PreventDefault();
				bool flag3 = flag2;
				if (flag3)
				{
					this.ScrollToItem(this.selectedIndex);
				}
			}
		}

		public void OnKeyDown(KeyDownEvent evt)
		{
			this.ProcessAnyEvent(evt);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			this.ProcessAnyEvent(evt);
			bool flag = !evt.isPropagationStopped;
			if (flag)
			{
				base.ExecuteDefaultActionAtTarget(evt);
			}
		}

		public void ScrollToItem(int index)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (flag)
			{
				throw new InvalidOperationException("Can't scroll without valid source, bind method, or factory method.");
			}
			bool flag2 = this.m_VisibleItemCount == 0 || index < -1;
			if (!flag2)
			{
				float resolvedItemHeight = this.resolvedItemHeight;
				bool flag3 = index == -1;
				if (flag3)
				{
					int num = (int)(this.m_LastHeight / resolvedItemHeight);
					bool flag4 = this.itemsSource.Count < num;
					if (flag4)
					{
						this.m_ScrollView.scrollOffset = new Vector2(0f, 0f);
					}
					else
					{
						this.m_ScrollView.scrollOffset = new Vector2(0f, (float)this.itemsSource.Count * resolvedItemHeight);
					}
				}
				else
				{
					bool flag5 = this.m_FirstVisibleIndex >= index;
					if (flag5)
					{
						this.m_ScrollView.scrollOffset = Vector2.up * (resolvedItemHeight * (float)index);
					}
					else
					{
						int num2 = (int)(this.m_LastHeight / resolvedItemHeight);
						bool flag6 = index < this.m_FirstVisibleIndex + num2;
						if (!flag6)
						{
							int num3 = index - num2;
							float num4 = resolvedItemHeight - (this.m_LastHeight - (float)num2 * resolvedItemHeight);
							float num5 = resolvedItemHeight * (float)num3 + num4;
							this.m_ScrollView.scrollOffset = new Vector2(this.m_ScrollView.scrollOffset.x, num5);
						}
					}
				}
			}
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			bool flag = evt.button == 0;
			if (flag)
			{
				bool flag2 = (evt.pressedButtons & 1) == 0;
				if (flag2)
				{
					this.ProcessPointerUp(evt);
				}
				else
				{
					this.ProcessPointerDown(evt);
				}
			}
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			this.ProcessPointerDown(evt);
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			this.ProcessPointerUp(evt);
		}

		private void ProcessPointerDown(IPointerEvent evt)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = !evt.isPrimary;
				if (!flag2)
				{
					bool flag3 = evt.button != 0;
					if (!flag3)
					{
						bool flag4 = evt.pointerType != PointerType.mouse;
						if (flag4)
						{
							this.m_TouchDownTime = ((EventBase)evt).timestamp;
							this.m_TouchDownPosition = evt.position;
						}
						else
						{
							this.DoSelect(evt.localPosition, evt.clickCount, evt.actionKey, evt.shiftKey);
						}
					}
				}
			}
		}

		private void ProcessPointerUp(IPointerEvent evt)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = !evt.isPrimary;
				if (!flag2)
				{
					bool flag3 = evt.button != 0;
					if (!flag3)
					{
						bool flag4 = evt.pointerType != PointerType.mouse;
						if (flag4)
						{
							long num = ((EventBase)evt).timestamp - this.m_TouchDownTime;
							Vector3 vector = evt.position - this.m_TouchDownPosition;
							bool flag5 = num < 500L && vector.sqrMagnitude <= 100f;
							if (flag5)
							{
								this.DoSelect(evt.localPosition, evt.clickCount, evt.actionKey, evt.shiftKey);
							}
						}
						else
						{
							int num2 = (int)(evt.localPosition.y / (float)this.itemHeight);
							bool flag6 = this.selectionType == SelectionType.Multiple && !evt.shiftKey && !evt.actionKey && this.m_SelectedIndices.Count > 1 && this.m_SelectedIndices.Contains(num2);
							if (flag6)
							{
								this.ProcessSingleClick(num2);
							}
						}
					}
				}
			}
		}

		private void DoSelect(Vector2 localPosition, int clickCount, bool actionKey, bool shiftKey)
		{
			int num = (int)(localPosition.y / this.resolvedItemHeight);
			bool flag = num > this.m_ItemsSource.Count - 1;
			if (!flag)
			{
				int idFromIndex = this.GetIdFromIndex(num);
				if (clickCount != 1)
				{
					if (clickCount == 2)
					{
						bool flag2 = this.onItemsChosen != null;
						if (flag2)
						{
							this.ProcessSingleClick(num);
						}
						Action<IEnumerable<object>> action = this.onItemsChosen;
						if (action != null)
						{
							action(this.m_SelectedItems);
						}
					}
				}
				else
				{
					bool flag3 = this.selectionType == SelectionType.None;
					if (!flag3)
					{
						bool flag4 = this.selectionType == SelectionType.Multiple && actionKey;
						if (flag4)
						{
							this.m_RangeSelectionOrigin = num;
							bool flag5 = this.m_SelectedIds.Contains(idFromIndex);
							if (flag5)
							{
								this.RemoveFromSelection(num);
							}
							else
							{
								this.AddToSelection(num);
							}
						}
						else
						{
							bool flag6 = this.selectionType == SelectionType.Multiple && shiftKey;
							if (flag6)
							{
								bool flag7 = this.m_RangeSelectionOrigin == -1;
								if (flag7)
								{
									this.m_RangeSelectionOrigin = num;
									this.SetSelection(num);
								}
								else
								{
									this.ClearSelectionWithoutValidation();
									List<int> list = new List<int>();
									bool flag8 = num < this.m_RangeSelectionOrigin;
									if (flag8)
									{
										for (int i = num; i <= this.m_RangeSelectionOrigin; i++)
										{
											list.Add(i);
										}
									}
									else
									{
										for (int j = this.m_RangeSelectionOrigin; j <= num; j++)
										{
											list.Add(j);
										}
									}
									this.AddToSelection(list);
								}
							}
							else
							{
								bool flag9 = this.selectionType == SelectionType.Multiple && this.m_SelectedIndices.Contains(num);
								if (!flag9)
								{
									this.m_RangeSelectionOrigin = num;
									this.SetSelection(num);
								}
							}
						}
					}
				}
			}
		}

		private void ProcessSingleClick(int clickedIndex)
		{
			this.m_RangeSelectionOrigin = clickedIndex;
			this.SetSelection(clickedIndex);
		}

		internal void SelectAll()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = this.selectionType != SelectionType.Multiple;
				if (!flag2)
				{
					for (int i = 0; i < this.itemsSource.Count; i++)
					{
						int idFromIndex = this.GetIdFromIndex(i);
						object obj = this.m_ItemsSource[i];
						foreach (ListView.RecycledItem recycledItem in this.m_Pool)
						{
							bool flag3 = recycledItem.id == idFromIndex;
							if (flag3)
							{
								recycledItem.SetSelected(true);
							}
						}
						bool flag4 = !this.m_SelectedIds.Contains(idFromIndex);
						if (flag4)
						{
							this.m_SelectedIds.Add(idFromIndex);
							this.m_SelectedIndices.Add(i);
							this.m_SelectedItems.Add(obj);
						}
					}
					this.NotifyOfSelectionChange();
					base.SaveViewData();
				}
			}
		}

		private int GetIdFromIndex(int index)
		{
			bool flag = this.m_GetItemId == null;
			int num;
			if (flag)
			{
				num = index;
			}
			else
			{
				num = this.m_GetItemId(index);
			}
			return num;
		}

		public void AddToSelection(int index)
		{
			this.AddToSelection(new int[] { index });
		}

		internal void AddToSelection(IList<int> indexes)
		{
			bool flag = !this.HasValidDataAndBindings() || indexes == null || indexes.Count == 0;
			if (!flag)
			{
				foreach (int num in indexes)
				{
					this.AddToSelectionWithoutValidation(num);
				}
				this.NotifyOfSelectionChange();
				base.SaveViewData();
			}
		}

		private void AddToSelectionWithoutValidation(int index)
		{
			bool flag = this.m_SelectedIndices.Contains(index);
			if (!flag)
			{
				int idFromIndex = this.GetIdFromIndex(index);
				object obj = this.m_ItemsSource[index];
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					bool flag2 = recycledItem.id == idFromIndex;
					if (flag2)
					{
						recycledItem.SetSelected(true);
					}
				}
				this.m_SelectedIds.Add(idFromIndex);
				this.m_SelectedIndices.Add(index);
				this.m_SelectedItems.Add(obj);
			}
		}

		public void RemoveFromSelection(int index)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				this.RemoveFromSelectionWithoutValidation(index);
				this.NotifyOfSelectionChange();
				base.SaveViewData();
			}
		}

		private void RemoveFromSelectionWithoutValidation(int index)
		{
			bool flag = !this.m_SelectedIndices.Contains(index);
			if (!flag)
			{
				int idFromIndex = this.GetIdFromIndex(index);
				object obj = this.m_ItemsSource[index];
				foreach (ListView.RecycledItem recycledItem in this.m_Pool)
				{
					bool flag2 = recycledItem.id == idFromIndex;
					if (flag2)
					{
						recycledItem.SetSelected(false);
					}
				}
				this.m_SelectedIds.Remove(idFromIndex);
				this.m_SelectedIndices.Remove(index);
				this.m_SelectedItems.Remove(obj);
			}
		}

		public void SetSelection(int index)
		{
			bool flag = index < 0;
			if (flag)
			{
				this.ClearSelection();
			}
			else
			{
				this.SetSelection(new int[] { index });
			}
		}

		public void SetSelection(IEnumerable<int> indices)
		{
			this.SetSelectionInternal(indices, true);
		}

		public void SetSelectionWithoutNotify(IEnumerable<int> indices)
		{
			this.SetSelectionInternal(indices, false);
		}

		internal void SetSelectionInternal(IEnumerable<int> indices, bool sendNotification)
		{
			bool flag = !this.HasValidDataAndBindings() || indices == null;
			if (!flag)
			{
				this.ClearSelectionWithoutValidation();
				foreach (int num in indices)
				{
					this.AddToSelectionWithoutValidation(num);
				}
				if (sendNotification)
				{
					this.NotifyOfSelectionChange();
				}
				base.SaveViewData();
			}
		}

		private void NotifyOfSelectionChange()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				Action<IEnumerable<object>> action = this.onSelectionChange;
				if (action != null)
				{
					action(this.m_SelectedItems);
				}
				Action<List<object>> action2 = this.onSelectionChanged;
				if (action2 != null)
				{
					action2(this.m_SelectedItems);
				}
			}
		}

		public void ClearSelection()
		{
			bool flag = !this.HasValidDataAndBindings() || this.m_SelectedIds.Count == 0;
			if (!flag)
			{
				this.ClearSelectionWithoutValidation();
				this.NotifyOfSelectionChange();
			}
		}

		private void ClearSelectionWithoutValidation()
		{
			foreach (ListView.RecycledItem recycledItem in this.m_Pool)
			{
				recycledItem.SetSelected(false);
			}
			this.m_SelectedIds.Clear();
			this.m_SelectedIndices.Clear();
			this.m_SelectedItems.Clear();
		}

		public void ScrollTo(VisualElement visualElement)
		{
			this.m_ScrollView.ScrollTo(visualElement);
		}

		internal void SetDragAndDropController(IListViewDragAndDropController dragAndDropController)
		{
			bool flag = this.m_Dragger == null;
			if (flag)
			{
				this.m_Dragger = new ListViewDragger(this);
			}
			this.m_Dragger.dragAndDropController = dragAndDropController;
		}

		internal IListViewDragAndDropController GetDragAndDropController()
		{
			ListViewDragger dragger = this.m_Dragger;
			return (dragger != null) ? dragger.dragAndDropController : null;
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
		}

		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			bool flag = evt.eventTypeId == EventBase<PointerUpEvent>.TypeId();
			if (flag)
			{
				ListViewDragger dragger = this.m_Dragger;
				if (dragger != null)
				{
					dragger.OnPointerUp();
				}
			}
			else
			{
				bool flag2 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
				if (flag2)
				{
					this.OnFocus(evt.leafTarget as VisualElement);
				}
			}
		}

		private void OnScroll(float offset)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				this.m_ScrollOffset = offset;
				float resolvedItemHeight = this.resolvedItemHeight;
				int num = (int)(offset / resolvedItemHeight);
				this.m_ScrollView.contentContainer.style.paddingTop = (float)num * resolvedItemHeight;
				this.m_ScrollView.contentContainer.style.height = (float)this.itemsSource.Count * resolvedItemHeight;
				bool flag2 = num != this.m_FirstVisibleIndex;
				if (flag2)
				{
					this.m_FirstVisibleIndex = num;
					bool flag3 = this.m_Pool.Count > 0;
					if (flag3)
					{
						bool flag4 = this.m_FirstVisibleIndex < this.m_Pool[0].index;
						if (flag4)
						{
							int num2 = this.m_Pool[0].index - this.m_FirstVisibleIndex;
							List<ListView.RecycledItem> scrollInsertionList = this.m_ScrollInsertionList;
							int num3 = 0;
							while (num3 < num2 && this.m_Pool.Count > 0)
							{
								ListView.RecycledItem recycledItem = this.m_Pool[this.m_Pool.Count - 1];
								scrollInsertionList.Add(recycledItem);
								this.m_Pool.RemoveAt(this.m_Pool.Count - 1);
								recycledItem.element.SendToBack();
								num3++;
							}
							this.m_ScrollInsertionList = this.m_Pool;
							this.m_Pool = scrollInsertionList;
							this.m_Pool.AddRange(this.m_ScrollInsertionList);
							this.m_ScrollInsertionList.Clear();
						}
						else
						{
							bool flag5 = this.m_FirstVisibleIndex < this.m_Pool[this.m_Pool.Count - 1].index;
							if (flag5)
							{
								List<ListView.RecycledItem> scrollInsertionList2 = this.m_ScrollInsertionList;
								int num4 = 0;
								while (this.m_FirstVisibleIndex > this.m_Pool[num4].index)
								{
									ListView.RecycledItem recycledItem2 = this.m_Pool[num4];
									scrollInsertionList2.Add(recycledItem2);
									num4++;
									recycledItem2.element.BringToFront();
								}
								this.m_Pool.RemoveRange(0, num4);
								this.m_Pool.AddRange(scrollInsertionList2);
								scrollInsertionList2.Clear();
							}
						}
						for (int i = 0; i < this.m_Pool.Count; i++)
						{
							int num5 = i + this.m_FirstVisibleIndex;
							bool flag6 = num5 < this.itemsSource.Count;
							if (flag6)
							{
								this.Setup(this.m_Pool[i], num5);
							}
							else
							{
								this.m_Pool[i].element.style.display = DisplayStyle.None;
							}
						}
					}
				}
			}
		}

		private bool HasValidDataAndBindings()
		{
			return this.itemsSource != null && this.makeItem != null && this.bindItem != null;
		}

		public void Refresh()
		{
			foreach (ListView.RecycledItem recycledItem in this.m_Pool)
			{
				recycledItem.DetachElement();
			}
			this.m_Pool.Clear();
			this.m_ScrollView.Clear();
			this.m_VisibleItemCount = 0;
			this.m_SelectedIndices.Clear();
			this.m_SelectedItems.Clear();
			bool flag = this.m_SelectedIds.Count > 0;
			if (flag)
			{
				for (int i = 0; i < this.m_ItemsSource.Count; i++)
				{
					bool flag2 = !this.m_SelectedIds.Contains(this.GetIdFromIndex(i));
					if (!flag2)
					{
						this.m_SelectedIndices.Add(i);
						this.m_SelectedItems.Add(this.m_ItemsSource[i]);
					}
				}
			}
			bool flag3 = !this.HasValidDataAndBindings();
			if (!flag3)
			{
				this.m_LastHeight = this.m_ScrollView.layout.height;
				bool flag4 = float.IsNaN(this.m_LastHeight);
				if (!flag4)
				{
					this.m_FirstVisibleIndex = (int)(this.m_ScrollOffset / this.resolvedItemHeight);
					this.ResizeHeight(this.m_LastHeight);
				}
			}
		}

		private void ResizeHeight(float height)
		{
			float resolvedItemHeight = this.resolvedItemHeight;
			float num = (float)this.itemsSource.Count * resolvedItemHeight;
			this.m_ScrollView.contentContainer.style.height = num;
			float num2 = Mathf.Max(0f, num - this.m_ScrollView.contentViewport.layout.height);
			this.m_ScrollView.verticalScroller.highValue = Mathf.Min(Mathf.Max(this.m_ScrollOffset, this.m_ScrollView.verticalScroller.highValue), num2);
			this.m_ScrollView.verticalScroller.value = Mathf.Min(this.m_ScrollOffset, this.m_ScrollView.verticalScroller.highValue);
			int num3 = Math.Min((int)(height / resolvedItemHeight) + 2, this.itemsSource.Count);
			bool flag = this.m_VisibleItemCount != num3;
			if (flag)
			{
				bool flag2 = this.m_VisibleItemCount > num3;
				if (flag2)
				{
					int num4 = this.m_VisibleItemCount - num3;
					for (int i = 0; i < num4; i++)
					{
						int num5 = this.m_Pool.Count - 1;
						ListView.RecycledItem recycledItem = this.m_Pool[num5];
						recycledItem.element.RemoveFromHierarchy();
						recycledItem.DetachElement();
						this.m_Pool.RemoveAt(num5);
					}
				}
				else
				{
					int num6 = num3 - this.m_VisibleItemCount;
					for (int j = 0; j < num6; j++)
					{
						int num7 = j + this.m_FirstVisibleIndex + this.m_VisibleItemCount;
						VisualElement visualElement = this.makeItem();
						ListView.RecycledItem recycledItem2 = new ListView.RecycledItem(visualElement);
						this.m_Pool.Add(recycledItem2);
						visualElement.AddToClassList("unity-listview-item");
						visualElement.style.position = Position.Relative;
						visualElement.style.flexBasis = StyleKeyword.Initial;
						visualElement.style.marginTop = 0f;
						visualElement.style.marginBottom = 0f;
						visualElement.style.flexGrow = 0f;
						visualElement.style.flexShrink = 0f;
						visualElement.style.height = resolvedItemHeight;
						bool flag3 = num7 < this.itemsSource.Count;
						if (flag3)
						{
							this.Setup(recycledItem2, num7);
						}
						else
						{
							visualElement.style.display = DisplayStyle.None;
						}
						this.m_ScrollView.Add(visualElement);
					}
				}
				this.m_VisibleItemCount = num3;
			}
			this.m_LastHeight = height;
			this.UpdateBackground();
		}

		private void Setup(ListView.RecycledItem recycledItem, int newIndex)
		{
			int index = recycledItem.index;
			int idFromIndex = this.GetIdFromIndex(newIndex);
			recycledItem.element.style.display = DisplayStyle.Flex;
			bool flag = recycledItem.index == newIndex;
			if (!flag)
			{
				this.m_LastItemIndex = newIndex;
				bool flag2 = this.showAlternatingRowBackgrounds != AlternatingRowBackground.None && newIndex % 2 == 1;
				if (flag2)
				{
					recycledItem.element.AddToClassList(ListView.itemAlternativeBackgroundUssClassName);
				}
				else
				{
					recycledItem.element.RemoveFromClassList(ListView.itemAlternativeBackgroundUssClassName);
				}
				bool flag3 = recycledItem.index != -1;
				if (flag3)
				{
					Action<VisualElement, int> unbindItem = this.unbindItem;
					if (unbindItem != null)
					{
						unbindItem(recycledItem.element, recycledItem.index);
					}
				}
				recycledItem.index = newIndex;
				recycledItem.id = idFromIndex;
				int num = newIndex - this.m_FirstVisibleIndex;
				bool flag4 = num == this.m_ScrollView.contentContainer.childCount;
				if (flag4)
				{
					recycledItem.element.BringToFront();
				}
				else
				{
					recycledItem.element.PlaceBehind(this.m_ScrollView.contentContainer[num]);
				}
				this.bindItem(recycledItem.element, recycledItem.index);
				recycledItem.SetSelected(this.m_SelectedIds.Contains(idFromIndex));
				this.HandleFocus(recycledItem, index);
			}
		}

		private void OnFocus(VisualElement leafTarget)
		{
			bool flag = leafTarget == this.m_ScrollView.contentContainer;
			if (!flag)
			{
				this.m_LastFocusedElementTreeChildIndexes.Clear();
				bool flag2 = this.m_ScrollView.contentContainer.FindElementInTree(leafTarget, this.m_LastFocusedElementTreeChildIndexes);
				if (flag2)
				{
					VisualElement visualElement = this.m_ScrollView.contentContainer[this.m_LastFocusedElementTreeChildIndexes[0]];
					foreach (ListView.RecycledItem recycledItem in this.m_Pool)
					{
						bool flag3 = recycledItem.element == visualElement;
						if (flag3)
						{
							this.m_LastFocusedElementIndex = recycledItem.index;
							break;
						}
					}
					this.m_LastFocusedElementTreeChildIndexes.RemoveAt(0);
				}
				else
				{
					this.m_LastFocusedElementIndex = -1;
				}
			}
		}

		private void HandleFocus(ListView.RecycledItem recycledItem, int previousIndex)
		{
			bool flag = this.m_LastFocusedElementIndex == -1;
			if (!flag)
			{
				bool flag2 = this.m_LastFocusedElementIndex == recycledItem.index;
				if (flag2)
				{
					VisualElement visualElement = recycledItem.element.ElementAtTreePath(this.m_LastFocusedElementTreeChildIndexes);
					if (visualElement != null)
					{
						visualElement.Focus();
					}
				}
				else
				{
					bool flag3 = this.m_LastFocusedElementIndex != previousIndex;
					if (flag3)
					{
						VisualElement visualElement2 = recycledItem.element.ElementAtTreePath(this.m_LastFocusedElementTreeChildIndexes);
						if (visualElement2 != null)
						{
							visualElement2.Blur();
						}
					}
					else
					{
						this.m_ScrollView.contentContainer.Focus();
					}
				}
			}
		}

		private void UpdateBackground()
		{
			float num = this.m_ScrollView.contentViewport.layout.size.y - this.m_ScrollView.contentContainer.layout.size.y;
			bool flag = this.showAlternatingRowBackgrounds != AlternatingRowBackground.All || num <= 0f;
			if (flag)
			{
				this.m_EmptyRows.RemoveFromHierarchy();
			}
			else
			{
				bool flag2 = this.m_EmptyRows.parent == null;
				if (flag2)
				{
					this.m_ScrollView.contentViewport.Add(this.m_EmptyRows);
				}
				float resolvedItemHeight = this.resolvedItemHeight;
				int num2 = Mathf.FloorToInt(num / resolvedItemHeight) + 1;
				bool flag3 = num2 > this.m_EmptyRows.childCount;
				if (flag3)
				{
					int num3 = num2 - this.m_EmptyRows.childCount;
					for (int i = 0; i < num3; i++)
					{
						VisualElement visualElement = new VisualElement();
						visualElement.style.flexShrink = 0f;
						this.m_EmptyRows.Add(visualElement);
					}
				}
				int num4 = this.m_LastItemIndex;
				int childCount = this.m_EmptyRows.hierarchy.childCount;
				for (int j = 0; j < childCount; j++)
				{
					VisualElement visualElement2 = this.m_EmptyRows.hierarchy[j];
					num4++;
					visualElement2.style.height = resolvedItemHeight;
					visualElement2.EnableInClassList(ListView.itemAlternativeBackgroundUssClassName, num4 % 2 == 1);
				}
			}
		}

		private void OnSizeChanged(GeometryChangedEvent evt)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = Mathf.Approximately(evt.newRect.height, evt.oldRect.height);
				if (!flag2)
				{
					this.ResizeHeight(evt.newRect.height);
				}
			}
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			int num;
			bool flag = !this.m_ItemHeightIsInline && e.customStyle.TryGetValue(ListView.s_ItemHeightProperty, out num);
			if (flag)
			{
				bool flag2 = this.m_ItemHeight != num;
				if (flag2)
				{
					this.m_ItemHeight = num;
					this.Refresh();
				}
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.Refresh();
		}

		private IList m_ItemsSource;

		private Func<VisualElement> m_MakeItem;

		private Action<VisualElement, int> m_BindItem;

		private Func<int, int> m_GetItemId;

		internal int m_ItemHeight = ListView.s_DefaultItemHeight;

		internal bool m_ItemHeightIsInline;

		[SerializeField]
		private float m_ScrollOffset;

		[SerializeField]
		private readonly List<int> m_SelectedIds = new List<int>();

		private readonly List<int> m_SelectedIndices = new List<int>();

		private readonly List<object> m_SelectedItems = new List<object>();

		private int m_LastFocusedElementIndex = -1;

		private List<int> m_LastFocusedElementTreeChildIndexes = new List<int>();

		private int m_RangeSelectionOrigin = -1;

		private ListViewDragger m_Dragger;

		private SelectionType m_SelectionType;

		[SerializeField]
		private AlternatingRowBackground m_ShowAlternatingRowBackgrounds = AlternatingRowBackground.None;

		private bool m_HorizontalScrollingEnabled;

		internal static readonly int s_DefaultItemHeight = 30;

		internal static CustomStyleProperty<int> s_ItemHeightProperty = new CustomStyleProperty<int>("--unity-item-height");

		private int m_FirstVisibleIndex;

		private float m_LastHeight;

		private List<ListView.RecycledItem> m_Pool = new List<ListView.RecycledItem>();

		internal readonly ScrollView m_ScrollView;

		private readonly VisualElement m_EmptyRows;

		private int m_LastItemIndex;

		private List<ListView.RecycledItem> m_ScrollInsertionList = new List<ListView.RecycledItem>();

		private const int k_ExtraVisibleItems = 2;

		private int m_VisibleItemCount;

		public static readonly string ussClassName = "unity-list-view";

		public static readonly string borderUssClassName = ListView.ussClassName + "--with-border";

		public static readonly string itemUssClassName = ListView.ussClassName + "__item";

		public static readonly string dragHoverBarUssClassName = ListView.ussClassName + "__drag-hover-bar";

		public static readonly string itemDragHoverUssClassName = ListView.itemUssClassName + "--drag-hover";

		public static readonly string itemSelectedVariantUssClassName = ListView.itemUssClassName + "--selected";

		public static readonly string itemAlternativeBackgroundUssClassName = ListView.itemUssClassName + "--alternative-background";

		internal static readonly string s_BackgroundFillUssClassName = ListView.ussClassName + "__background";

		private long m_TouchDownTime = 0L;

		private Vector3 m_TouchDownPosition;

		public new class UxmlFactory : UxmlFactory<ListView, ListView.UxmlTraits>
		{
		}

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				int num = 0;
				ListView listView = (ListView)ve;
				listView.reorderable = this.m_Reorderable.GetValueFromBag(bag, cc);
				bool flag = this.m_ItemHeight.TryGetValueFromBag(bag, cc, ref num);
				if (flag)
				{
					listView.itemHeight = num;
				}
				listView.showBorder = this.m_ShowBorder.GetValueFromBag(bag, cc);
				listView.selectionType = this.m_SelectionType.GetValueFromBag(bag, cc);
				listView.showAlternatingRowBackgrounds = this.m_ShowAlternatingRowBackgrounds.GetValueFromBag(bag, cc);
				listView.showBoundCollectionSize = this.m_ShowBoundCollectionSize.GetValueFromBag(bag, cc);
				listView.horizontalScrollingEnabled = this.m_HorizontalScrollingEnabled.GetValueFromBag(bag, cc);
			}

			private readonly UxmlIntAttributeDescription m_ItemHeight = new UxmlIntAttributeDescription
			{
				name = "item-height",
				obsoleteNames = new string[] { "itemHeight" },
				defaultValue = ListView.s_DefaultItemHeight
			};

			private readonly UxmlBoolAttributeDescription m_ShowBorder = new UxmlBoolAttributeDescription
			{
				name = "show-border",
				defaultValue = false
			};

			private readonly UxmlEnumAttributeDescription<SelectionType> m_SelectionType = new UxmlEnumAttributeDescription<SelectionType>
			{
				name = "selection-type",
				defaultValue = SelectionType.Single
			};

			private readonly UxmlEnumAttributeDescription<AlternatingRowBackground> m_ShowAlternatingRowBackgrounds = new UxmlEnumAttributeDescription<AlternatingRowBackground>
			{
				name = "show-alternating-row-backgrounds",
				defaultValue = AlternatingRowBackground.None
			};

			private readonly UxmlBoolAttributeDescription m_Reorderable = new UxmlBoolAttributeDescription
			{
				name = "reorderable",
				defaultValue = false
			};

			private readonly UxmlBoolAttributeDescription m_ShowBoundCollectionSize = new UxmlBoolAttributeDescription
			{
				name = "show-bound-collection-size",
				defaultValue = true
			};

			private readonly UxmlBoolAttributeDescription m_HorizontalScrollingEnabled = new UxmlBoolAttributeDescription
			{
				name = "horizontal-scrolling",
				defaultValue = false
			};
		}

		internal class RecycledItem
		{
			public VisualElement element { get; private set; }

			public RecycledItem(VisualElement element)
			{
				this.element = element;
				this.index = (this.id = -1);
				element.AddToClassList(ListView.itemUssClassName);
			}

			public void DetachElement()
			{
				this.element.RemoveFromClassList(ListView.itemUssClassName);
				this.element = null;
			}

			public void SetSelected(bool selected)
			{
				bool flag = this.element != null;
				if (flag)
				{
					if (selected)
					{
						this.element.AddToClassList(ListView.itemSelectedVariantUssClassName);
						this.element.pseudoStates |= PseudoStates.Checked;
					}
					else
					{
						this.element.RemoveFromClassList(ListView.itemSelectedVariantUssClassName);
						this.element.pseudoStates &= ~PseudoStates.Checked;
					}
				}
			}

			public const int kUndefinedIndex = -1;

			public int index;

			public int id;
		}
	}
}
