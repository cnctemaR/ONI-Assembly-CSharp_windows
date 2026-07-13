using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class CollectionView : VisualElement
	{
		private ICollectionDragAndDropController CreateDragAndDropController()
		{
			return new ReorderableDragAndDropController(this);
		}

		internal CollectionViewDragger dragger
		{
			get
			{
				return this.m_Dragger;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action reorderModeChanged;

		internal bool isRebuildScheduled
		{
			get
			{
				IVisualElementScheduledItem rebuildScheduled = this.m_RebuildScheduled;
				return rebuildScheduled != null && rebuildScheduled.isActive;
			}
		}

		internal bool processingPointerDownEvent
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
			get;
			private set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<CanStartDragArgs, bool> canStartDrag;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<SetupDragAndDropArgs, StartDragArgs> setupDragAndDrop;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<HandleDragAndDropArgs, DragVisualMode> dragAndDropUpdate;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<HandleDragAndDropArgs, DragVisualMode> handleDrop;

		[CreateProperty]
		public IList itemsSource
		{
			get
			{
				return this.m_ItemsSource;
			}
			set
			{
				bool flag = value == this.itemsSource;
				if (!flag)
				{
					this.m_ItemsSource = value;
					this.RefreshItems();
					base.NotifyPropertyChanged(in CollectionView.itemsSourceProperty);
				}
			}
		}

		public void SetItemsSourceWithoutNotify(IList source)
		{
			this.m_ItemsSource = source;
		}

		public CollectionViewLayoutConfiguration layoutConfiguration
		{
			get
			{
				return this.m_Configuration;
			}
			set
			{
				bool flag = value == null || value == this.m_Configuration;
				if (!flag)
				{
					this.m_Configuration = value;
					this.m_Configuration.m_View = this;
					MultiColumnLayoutConfiguration multiColumnLayoutConfiguration = value as MultiColumnLayoutConfiguration;
					bool flag2 = multiColumnLayoutConfiguration != null;
					if (flag2)
					{
						base.Insert(0, multiColumnLayoutConfiguration.CreateMultiColumnHeader());
					}
					this.UnbindAllItems();
					this.ResetAverageHeight();
					this.RefreshItems();
				}
			}
		}

		public ScrollContainer scrollView
		{
			get
			{
				return this.m_ScrollView;
			}
			private set
			{
				this.m_ScrollView = value;
			}
		}

		[CreateProperty]
		public float fixedItemHeight
		{
			get
			{
				return this.m_FixedItemHeight;
			}
			set
			{
				bool flag = Math.Abs(this.m_FixedItemHeight - value) > float.Epsilon;
				if (flag)
				{
					this.m_FixedItemHeight = value;
					base.NotifyPropertyChanged(in CollectionView.fixedItemHeightProperty);
				}
			}
		}

		[CreateProperty]
		public SelectionType selectionType
		{
			get
			{
				return this.m_SelectionType;
			}
			set
			{
				SelectionType selectionType = this.m_SelectionType;
				this.m_SelectionType = value;
				bool flag = this.m_SelectionType == SelectionType.None;
				if (flag)
				{
					this.ClearSelection();
				}
				else
				{
					bool flag2 = this.m_SelectionType == SelectionType.Single;
					if (flag2)
					{
						bool flag3 = this.m_Selection.indexCount > 1;
						if (flag3)
						{
							this.SetSelection(this.m_Selection.FirstIndex());
						}
					}
				}
				bool flag4 = selectionType != this.m_SelectionType;
				if (flag4)
				{
					base.NotifyPropertyChanged(in CollectionView.selectionTypeProperty);
				}
			}
		}

		internal float averageItemHeight
		{
			get
			{
				bool flag = this.fixedItemHeight > 0f;
				float num;
				if (flag)
				{
					num = this.fixedItemHeight;
				}
				else
				{
					bool flag2 = this.m_ComputedAverageHeight > 0f;
					if (flag2)
					{
						num = this.m_ComputedAverageHeight;
					}
					else
					{
						num = 22f;
					}
				}
				return num;
			}
		}

		[CreateProperty]
		public bool showBorder
		{
			get
			{
				return this.m_ScrollView.contentContainer.ClassListContains(BaseVerticalCollectionView.borderUssClassName);
			}
			set
			{
				bool showBorder = this.showBorder;
				this.m_ScrollView.contentContainer.EnableInClassList(BaseVerticalCollectionView.borderUssClassName, value);
				bool flag = showBorder != this.showBorder;
				if (flag)
				{
					base.NotifyPropertyChanged(in CollectionView.showBorderProperty);
				}
			}
		}

		[CreateProperty]
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
					this.RefreshItems();
					base.NotifyPropertyChanged(in CollectionView.showAlternatingRowBackgroundsProperty);
				}
			}
		}

		[CreateProperty]
		public bool reorderable
		{
			get
			{
				CollectionViewDragger dragger = this.m_Dragger;
				bool? flag;
				if (dragger == null)
				{
					flag = null;
				}
				else
				{
					ICollectionDragAndDropController dragAndDropController = dragger.dragAndDropController;
					flag = ((dragAndDropController != null) ? new bool?(dragAndDropController.enableReordering) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
			set
			{
				bool flag = value == this.reorderable;
				if (!flag)
				{
					bool reorderable = this.reorderable;
					ICollectionDragAndDropController dragAndDropController = this.m_Dragger.dragAndDropController;
					bool flag2 = dragAndDropController != null && dragAndDropController.enableReordering != value;
					if (flag2)
					{
						dragAndDropController.enableReordering = value;
						this.Rebuild();
					}
					bool flag3 = reorderable != this.reorderable;
					if (flag3)
					{
						base.NotifyPropertyChanged(in CollectionView.reorderableProperty);
					}
				}
			}
		}

		[CreateProperty]
		public ListViewReorderMode reorderMode
		{
			get
			{
				return this.m_ReorderMode;
			}
			set
			{
				bool flag = value == this.m_ReorderMode;
				if (!flag)
				{
					this.m_ReorderMode = value;
					this.InitializeDragAndDropController(this.reorderable);
					Action action = this.reorderModeChanged;
					if (action != null)
					{
						action();
					}
					this.Rebuild();
					base.NotifyPropertyChanged(in CollectionView.reorderModeProperty);
				}
			}
		}

		public CollectionView()
		{
			this.focusable = true;
			base.isCompositeRoot = true;
			base.delegatesFocus = true;
			this.selectionType = SelectionType.Single;
			base.AddToClassList(BaseVerticalCollectionView.ussClassName);
			this.m_ScrollView = new ScrollContainer
			{
				focusable = true
			};
			this.m_Container = this.m_ScrollView.contentContainer;
			this.m_VerticalScroller = this.m_ScrollView.verticalScroller;
			this.m_VerticalScroller.RegisterValueChangedCallback<double>(new EventCallback<ChangeEvent<double>>(this.OnVerticalScrollingChangeEvent));
			this.m_HorizontalScroller = this.m_ScrollView.horizontalScroller;
			this.m_HorizontalScroller.RegisterValueChangedCallback<double>(new EventCallback<ChangeEvent<double>>(this.OnHorizontalScrollerChangeEvent));
			base.Add(this.m_ScrollView);
			this.InitializeDragAndDropController(this.reorderable);
			base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanelEvent), TrickleDown.NoTrickleDown);
			base.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanelEvent), TrickleDown.NoTrickleDown);
			base.RegisterCallback<GeometryChangedEvent>(delegate(GeometryChangedEvent evt)
			{
				this.ContainerSizeChanged(evt.newRect.height, evt.newRect.width);
			}, TrickleDown.NoTrickleDown);
		}

		private void OnAttachToPanelEvent(AttachToPanelEvent evt)
		{
			bool flag = evt.destinationPanel == null;
			if (!flag)
			{
				this.AddManipulator(this.m_NavigationManipulator = new KeyboardNavigationManipulator(new Action<KeyboardNavigationOperation, EventBase>(this.Apply)));
				base.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				base.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
				base.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				base.RegisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
			}
		}

		private void OnDetachFromPanelEvent(DetachFromPanelEvent evt)
		{
			bool flag = evt.originPanel == null;
			if (!flag)
			{
				this.RemoveManipulator(this.m_NavigationManipulator);
				base.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				base.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
				base.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				base.UnregisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
				IVisualElementScheduledItem scrollScheduledItem = this.m_ScrollScheduledItem;
				bool flag2 = scrollScheduledItem != null && scrollScheduledItem.isActive;
				if (flag2)
				{
					this.m_ScrollScheduledItem.Pause();
					this.m_ScrollScheduledItem = null;
				}
			}
		}

		private void OnVerticalScrollingChangeEvent(ChangeEvent<double> evt)
		{
			bool isChangingScrollingParameters = this.m_IsChangingScrollingParameters;
			if (!isChangingScrollingParameters)
			{
				this.m_DelayedScrolledVerticalValue = this.m_VerticalScroller.value;
				this.ScheduleScroll();
				evt.StopImmediatePropagation();
			}
		}

		private void OnHorizontalScrollerChangeEvent(ChangeEvent<double> evt)
		{
			MultiColumnLayoutConfiguration multiColumnLayoutConfiguration = this.layoutConfiguration as MultiColumnLayoutConfiguration;
			bool flag = multiColumnLayoutConfiguration != null;
			if (flag)
			{
				multiColumnLayoutConfiguration.header.ScrollHorizontally((float)evt.newValue);
			}
		}

		internal void UpdateVerticalScrollValue(double value)
		{
			bool flag = !this.m_VerticalScroller.Approximately(value, this.m_ScrollValue);
			if (flag)
			{
				CollectionViewScroller verticalScroller = this.m_VerticalScroller;
				this.m_ScrollValue = value;
				verticalScroller.value = value;
				this.BindVisibleItems();
			}
		}

		private void SetScrollingParameters(double currentScrollOffset, double maxScrollRange)
		{
			bool isChangingScrollingParameters = this.m_IsChangingScrollingParameters;
			this.m_IsChangingScrollingParameters = true;
			currentScrollOffset = Math.Min(currentScrollOffset, maxScrollRange);
			try
			{
				using (new EventDispatcherGate(base.panel.dispatcher))
				{
					this.m_VerticalScroller.highValue = maxScrollRange;
					this.m_VerticalScroller.value = (this.m_ScrollValue = currentScrollOffset);
				}
			}
			finally
			{
				this.m_IsChangingScrollingParameters = isChangingScrollingParameters;
			}
		}

		private void ScheduleScroll()
		{
			bool flag = this.m_ScrollScheduledItem == null;
			if (flag)
			{
				this.m_ScrollScheduledItem = base.schedule.Execute(new Action(this.OnDelayedScroll));
			}
			else
			{
				bool flag2 = !this.m_ScrollScheduledItem.isActive;
				if (flag2)
				{
					this.m_ScrollScheduledItem.Resume();
				}
			}
		}

		private void OnDelayedScroll()
		{
			this.UpdateVerticalScrollValue(this.m_DelayedScrolledVerticalValue);
			this.m_DelayedScrolledVerticalValue = 0.0;
		}

		private void ContainerSizeChanged(float height, float width)
		{
			bool flag = !Mathf.Approximately(this.m_LastHeight, height);
			if (flag)
			{
				this.m_LastHeight = height;
				this.RefreshItems();
			}
		}

		private void UnbindItem(RecycledItem item)
		{
			bool flag = item == null;
			if (!flag)
			{
				int index = item.index;
				item.index = -1;
				this.m_IndexToItemDictionary.Remove(index);
				Action<VisualElement, int> unbindCell = this.layoutConfiguration.unbindCell;
				if (unbindCell != null)
				{
					unbindCell(item.element, index);
				}
			}
		}

		internal void OnDestroyItem(RecycledItem item)
		{
			Action<VisualElement> destroyCell = this.layoutConfiguration.destroyCell;
			if (destroyCell != null)
			{
				destroyCell(item.element);
			}
		}

		private void BindItem(RecycledItem item, int index)
		{
			int index2 = item.index;
			bool flag = this.m_IndexToItemDictionary.ContainsKey(item.index);
			if (flag)
			{
				this.UnbindItem(item);
			}
			bool flag2 = this.showAlternatingRowBackgrounds != AlternatingRowBackground.None && index % 2 == 1;
			item.element.EnableInClassList(BaseVerticalCollectionView.itemAlternativeBackgroundUssClassName, flag2);
			item.isLastItem = index == this.itemsSource.Count - 1;
			item.SetSelected(this.m_Selection.ContainsIndex(index));
			item.element.style.height = this.averageItemHeight;
			item.index = index;
			this.m_IndexToItemDictionary.Add(index, item);
			bool flag3 = index >= 0 && index < this.itemsSource.Count;
			if (flag3)
			{
				Action<VisualElement, int> bindCell = this.layoutConfiguration.bindCell;
				if (bindCell != null)
				{
					bindCell(item.element, index);
				}
			}
			this.HandleFocus(item, index2);
		}

		public void Rebuild()
		{
			IVisualElementScheduledItem rebuildScheduled = this.m_RebuildScheduled;
			if (rebuildScheduled != null)
			{
				rebuildScheduled.Pause();
			}
			this.ClearAllItems();
			this.ResetAverageHeight();
			this.RefreshItems();
		}

		internal void ScheduleRebuild()
		{
			bool flag = this.m_RebuildScheduled == null;
			if (flag)
			{
				this.m_RebuildScheduled = base.schedule.Execute(new Action(this.Rebuild));
			}
			else
			{
				bool flag2 = !this.m_RebuildScheduled.isActive;
				if (flag2)
				{
					this.m_RebuildScheduled.Resume();
				}
			}
		}

		public void RefreshItems()
		{
			bool flag;
			if (this.itemsSource != null)
			{
				CollectionViewLayoutConfiguration layoutConfiguration = this.layoutConfiguration;
				if (((layoutConfiguration != null) ? layoutConfiguration.makeCell : null) != null)
				{
					flag = this.itemsSource.Count == 0;
					goto IL_002E;
				}
			}
			flag = true;
			IL_002E:
			bool flag2 = flag;
			if (flag2)
			{
				this.m_VerticalScroller.style.display = DisplayStyle.None;
			}
			else
			{
				IVisualElementScheduledItem rebuildScheduled = this.m_RebuildScheduled;
				bool flag3 = rebuildScheduled != null && rebuildScheduled.isActive;
				if (flag3)
				{
					this.Rebuild();
				}
				else
				{
					this.m_VerticalScroller.style.display = DisplayStyle.Flex;
					float height = this.m_Container.resolvedStyle.height;
					bool flag4 = float.IsNaN(height);
					if (!flag4)
					{
						this.m_LastHeight = height;
						int num = (int)(this.m_Container.layout.height / this.averageItemHeight);
						bool flag5 = this.itemsSource.Count - 1 < num;
						if (flag5)
						{
							this.m_ScrollValue = 0.0;
						}
						double num2 = (double)this.averageItemHeight * (double)this.itemsSource.Count;
						this.m_VerticalScroller.style.display = ((num2 > (double)height) ? DisplayStyle.Flex : DisplayStyle.None);
						base.EnableInClassList(CollectionView.verticalScrollerVisibleUssClassName, num2 > (double)height);
						this.SetScrollingParameters(this.m_ScrollValue, Math.Abs(num2 - (double)height));
						this.BindVisibleItems();
						bool flag6 = this.m_IndexToItemDictionary.Count > this.m_DisplayedList.Count;
						if (flag6)
						{
							for (int i = this.m_IndexToItemDictionary.Count - 1; i >= this.m_DisplayedList.Count; i--)
							{
								this.UnbindItem(this.m_IndexToItemDictionary[i]);
							}
						}
					}
				}
			}
		}

		private void ClearAllItems()
		{
			while (this.m_DisplayedList.Count > 0)
			{
				this.ClearItem(this.m_DisplayedList.First);
			}
			while (this.m_FreeList.Count > 0)
			{
				this.ClearItem(this.m_FreeList.First);
			}
			RecycledItem.ClearItemPool();
		}

		[EventInterest(new Type[]
		{
			typeof(PointerUpEvent),
			typeof(FocusInEvent),
			typeof(FocusOutEvent),
			typeof(NavigationSubmitEvent)
		})]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool flag = evt.eventTypeId == EventBase<PointerUpEvent>.TypeId();
			if (flag)
			{
				CollectionViewDragger dragger = this.m_Dragger;
				if (dragger != null)
				{
					dragger.OnPointerUpEvent((PointerUpEvent)evt);
				}
			}
			else
			{
				bool flag2 = evt.eventTypeId == EventBase<FocusInEvent>.TypeId();
				if (flag2)
				{
					this.OnFocusIn(evt.elementTarget);
				}
				else
				{
					bool flag3 = evt.eventTypeId == EventBase<FocusOutEvent>.TypeId();
					if (flag3)
					{
						this.OnFocusOut(((FocusOutEvent)evt).relatedTarget as VisualElement);
					}
					else
					{
						bool flag4 = evt.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId();
						if (flag4)
						{
							bool flag5 = evt.target == this;
							if (flag5)
							{
								this.m_ScrollView.Focus();
							}
						}
					}
				}
			}
		}

		private void OnFocusIn(VisualElement leafTarget)
		{
			bool flag = leafTarget == this.m_ScrollView;
			if (!flag)
			{
				this.m_LastFocusedElementTreeChildIndexes.Clear();
				bool flag2 = this.m_ScrollView.contentContainer.FindElementInTree(leafTarget, this.m_LastFocusedElementTreeChildIndexes);
				if (flag2)
				{
					VisualElement visualElement = this.m_ScrollView.contentContainer[this.m_LastFocusedElementTreeChildIndexes[0]];
					foreach (RecycledItem recycledItem in this.m_IndexToItemDictionary.Values)
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

		private void OnFocusOut(VisualElement willFocus)
		{
			bool flag = willFocus == null || willFocus != this.m_ScrollView;
			if (flag)
			{
				this.m_LastFocusedElementTreeChildIndexes.Clear();
				this.m_LastFocusedElementIndex = -1;
			}
		}

		private void HandleFocus(RecycledItem recycledItem, int previousIndex)
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
						this.m_ScrollView.Focus();
					}
				}
			}
		}

		private void ClearItem(LinkedListNode<RecycledItem> item)
		{
			item.List.Remove(item);
			this.UnbindItem(item.Value);
			RecycledItem.Recycle(item.Value);
		}

		private void UnbindAllItems()
		{
			foreach (KeyValuePair<int, RecycledItem> keyValuePair in this.m_IndexToItemDictionary)
			{
				int num;
				RecycledItem recycledItem;
				keyValuePair.Deconstruct(out num, out recycledItem);
				RecycledItem recycledItem2 = recycledItem;
				this.UnbindItem(recycledItem2);
			}
		}

		private void UpdateVisibleRange()
		{
			int num = 0;
			int num2 = -1;
			bool flag = this.m_DisplayedList.Count > 0;
			if (flag)
			{
				num = this.m_DisplayedList.First.Value.index;
				LinkedListNode<RecycledItem> linkedListNode = this.m_DisplayedList.First;
				bool flag2 = linkedListNode.Value.renderedHeight < 0f;
				if (flag2)
				{
					return;
				}
				this.m_VerticalScroller.scrollSize = (double)(10f * this.averageItemHeight / linkedListNode.Value.renderedHeight);
				LinkedListNode<RecycledItem> linkedListNode2 = this.m_DisplayedList.First;
				float num3 = this.m_LastHeight - this.m_ScrollView.containerOffset.y;
				while (linkedListNode != null && linkedListNode.Value.verticalOffset + linkedListNode.Value.renderedHeight < num3)
				{
					linkedListNode2 = linkedListNode;
					linkedListNode = linkedListNode.Next;
				}
				bool flag3 = linkedListNode2 != null;
				if (flag3)
				{
					num2 = linkedListNode2.Value.index;
				}
			}
			int num4 = num2 - num + 1;
			bool flag4 = num4 > 0 && this.itemsSource != null;
			if (flag4)
			{
				float num5 = Mathf.Ceil(this.m_LastHeight / this.averageItemHeight);
				float num6 = num5 / (float)this.itemsSource.Count;
				this.m_VerticalScroller.Adjust(num6);
			}
		}

		private void ResetAverageHeight()
		{
			this.m_ComputedAverageHeight = -1f;
		}

		private void BindVisibleItems()
		{
			float lastHeight = this.m_LastHeight;
			int num = (int)Mathf.Ceil(lastHeight / this.averageItemHeight) + 3;
			this.m_FirstVisibleItemIndex = (int)(this.m_ScrollValue / (double)this.averageItemHeight);
			LinkedList<RecycledItem> refreshList = this.m_RefreshList;
			LinkedList<RecycledItem> displayedList = this.m_DisplayedList;
			this.m_DisplayedList = refreshList;
			this.m_RefreshList = displayedList;
			for (int i = 0; i < num; i++)
			{
				int num2 = this.m_FirstVisibleItemIndex + i;
				bool flag = num2 < 0 || num2 > this.itemsSource.Count - 1;
				if (!flag)
				{
					RecycledItem recycledItem;
					bool flag2 = this.m_IndexToItemDictionary.TryGetValue(num2, out recycledItem);
					if (flag2)
					{
						bool flag3 = recycledItem.node.List == this.m_RefreshList;
						if (flag3)
						{
							recycledItem.node.List.Remove(recycledItem.node);
						}
					}
				}
			}
			while (this.m_RefreshList.Count > 0)
			{
				LinkedListNode<RecycledItem> first = this.m_RefreshList.First;
				bool flag4 = first != null;
				if (flag4)
				{
					this.m_RefreshList.RemoveFirst();
					first.Value.element.style.display = DisplayStyle.None;
					this.m_FreeList.AddLast(first);
				}
			}
			this.AddElementsFromIndex(this.m_FirstVisibleItemIndex, num);
		}

		private void AddElementsFromIndex(int firstIndex, int itemCount)
		{
			int num = firstIndex + itemCount;
			for (int i = firstIndex; i < num; i++)
			{
				bool flag = i < 0 || i > this.itemsSource.Count - 1;
				if (!flag)
				{
					RecycledItem recycledItem;
					bool flag2 = this.m_IndexToItemDictionary.TryGetValue(i, out recycledItem);
					if (flag2)
					{
						LinkedList<RecycledItem> list = recycledItem.node.List;
						if (list != null)
						{
							list.Remove(recycledItem.node);
						}
					}
					else
					{
						bool flag3 = this.m_FreeList.Count > 0;
						if (flag3)
						{
							recycledItem = this.m_FreeList.First.Value;
							this.m_FreeList.RemoveFirst();
						}
						else
						{
							Func<VisualElement> makeCell = this.layoutConfiguration.makeCell;
							VisualElement visualElement = ((makeCell != null) ? makeCell() : null);
							recycledItem = RecycledItem.AllocateItem(visualElement, this);
							this.m_Container.Add(visualElement);
						}
					}
					this.BindItem(recycledItem, i);
					recycledItem.element.style.display = DisplayStyle.Flex;
					recycledItem.element.style.position = Position.Absolute;
					recycledItem.element.style.top = 0f;
					recycledItem.element.style.left = 0f;
					recycledItem.element.style.right = 0f;
					this.m_DisplayedList.AddLast(recycledItem.node);
				}
			}
			this.UpdateContainerOffset();
			bool flag4 = this.m_DisplayedList.Count > 0;
			if (flag4)
			{
				RecycledItem.UpdatePositions(this.m_DisplayedList.First.Value);
			}
		}

		private void UpdateContainerOffset()
		{
			float num = this.averageItemHeight;
			Vector2 containerOffset = this.m_ScrollView.containerOffset;
			bool flag = this.m_DisplayedList.Count > 0;
			if (flag)
			{
				bool flag2 = this.m_DisplayedList.First.Value.renderedHeight > 0f;
				if (flag2)
				{
					num = this.m_DisplayedList.First.Value.renderedHeight;
				}
			}
			float num2 = (float)(this.m_ScrollValue % (double)this.averageItemHeight);
			num2 *= num / this.averageItemHeight;
			containerOffset.y = num2;
			containerOffset.x = 0f;
			this.m_ScrollView.containerOffset = containerOffset;
		}

		private void UpdateScrollingRangeAfterLayout()
		{
			float num = 0f;
			MultiColumnLayoutConfiguration multiColumnLayoutConfiguration = this.layoutConfiguration as MultiColumnLayoutConfiguration;
			bool flag = multiColumnLayoutConfiguration != null;
			if (flag)
			{
				num = multiColumnLayoutConfiguration.header.worldBoundingBox.width;
			}
			else
			{
				float num2 = ((this.m_VerticalScroller.style.display == DisplayStyle.None) ? 0f : this.m_VerticalScroller.worldBound.width);
				foreach (RecycledItem recycledItem in this.m_DisplayedList)
				{
					num = Mathf.Max(num, recycledItem.element.worldBoundingBox.width - num2);
				}
			}
			this.m_HorizontalScroller.SetEnabled(num > this.m_Container.worldBound.width);
			this.m_HorizontalScroller.style.display = ((num > this.m_Container.rect.width) ? DisplayStyle.Flex : DisplayStyle.None);
			this.m_HorizontalScroller.lowValue = 0.0;
			this.m_HorizontalScroller.highValue = (double)(num - this.m_Container.rect.width);
			this.m_HorizontalScroller.scrollSize = (double)(10f * this.m_Container.rect.width);
			float num3 = ((num > 1E-30f) ? (this.m_Container.worldBound.width / num) : 1f);
			this.m_HorizontalScroller.Adjust(num3);
			LinkedListNode<RecycledItem> last = this.m_DisplayedList.Last;
			bool flag2 = last != null;
			if (flag2)
			{
				RecycledItem value = last.Value;
				float y = this.m_ScrollView.containerOffset.y;
				float num4 = value.verticalOffset + value.renderedHeight;
				float height = this.m_Container.resolvedStyle.height;
				bool isLastItem = value.isLastItem;
				if (isLastItem)
				{
					LinkedListNode<RecycledItem> linkedListNode = last;
					float num5 = height;
					float num6 = num5 - value.renderedHeight;
					while (num6 > 0f && linkedListNode.Previous != null)
					{
						linkedListNode = linkedListNode.Previous;
						num6 -= linkedListNode.Value.renderedHeight;
					}
					bool flag3 = num6 <= 0f;
					if (flag3)
					{
						double num7 = (double)(-(double)num6 / linkedListNode.Value.renderedHeight);
						int index = linkedListNode.Value.index;
						double num8 = ((double)index + num7) * (double)this.averageItemHeight;
						this.SetScrollingParameters(Math.Min(this.m_ScrollValue, num8), num8);
					}
				}
				else
				{
					bool flag4 = num4 + y < height - 1f;
					if (flag4)
					{
						float num9 = height - (num4 - y);
						int num10 = Mathf.CeilToInt(num9 / this.averageItemHeight);
						num10 = Math.Clamp(num10, 0, this.itemsSource.Count - value.index - 1);
						bool flag5 = num10 > 0;
						if (flag5)
						{
							this.AddElementsFromIndex(value.index + 1, num10);
							return;
						}
					}
				}
				this.UpdateVisibleRange();
			}
		}

		internal void ItemPositionUpdated(RecycledItem item)
		{
			this.UpdateScrollingRangeAfterLayout();
		}

		public int GetIndexFromPosition(Vector2 position)
		{
			float num = AlignmentUtils.RoundToPixelGrid(this.averageItemHeight, base.scaledPixelsPerPoint, 0.02f);
			double num2 = this.m_ScrollValue + (double)position.y;
			return (int)(num2 / (double)num);
		}

		public void ScrollToItem(int index)
		{
			bool flag = index < -1 || index > this.itemsSource.Count;
			if (!flag)
			{
				bool flag2 = index == -1;
				if (flag2)
				{
					index = this.itemsSource.Count - 1;
				}
				bool flag3 = this.m_FirstVisibleItemIndex >= index;
				if (flag3)
				{
					this.UpdateVerticalScrollValue((double)(this.averageItemHeight * (float)index));
				}
				else
				{
					int num = (int)(this.m_Container.layout.height / this.averageItemHeight);
					bool flag4 = index < this.m_FirstVisibleItemIndex + num;
					if (!flag4)
					{
						float num2 = this.averageItemHeight - (this.m_Container.layout.height - (float)num * this.averageItemHeight);
						float num3 = this.averageItemHeight * (float)(index - num) + num2;
						this.UpdateVerticalScrollValue((double)num3);
					}
				}
			}
		}

		public VisualElement GetRootElementForIndex(int index)
		{
			bool flag = this.m_DisplayedList == null || index < 0 || index >= this.m_DisplayedList.Count;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = null;
			}
			else
			{
				LinkedListNode<RecycledItem> linkedListNode = this.m_DisplayedList.First;
				for (int i = 0; i < index; i++)
				{
					linkedListNode = linkedListNode.Next;
				}
				visualElement = linkedListNode.Value.element;
			}
			return visualElement;
		}

		[CreateProperty]
		public int selectedIndex
		{
			get
			{
				return this.m_Selection.FirstIndex();
			}
			set
			{
				this.SetSelection(value);
			}
		}

		[CreateProperty(ReadOnly = true)]
		public IEnumerable<int> selectedIndices
		{
			get
			{
				return this.m_Selection.indices;
			}
		}

		public bool hasSelection
		{
			get
			{
				return this.m_Selection.indices.Count > 0;
			}
		}

		public bool IsSelected(int index)
		{
			return this.m_Selection.ContainsIndex(index);
		}

		private void NotifyOfSelectionChange()
		{
			Action action = this.selectedIndicesChanged;
			if (action != null)
			{
				action();
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action selectedIndicesChanged;

		private void OnPointerUp(IPointerEvent evt)
		{
			bool flag = !evt.isPrimary;
			if (!flag)
			{
				bool flag2 = evt.button != 0 && evt.button != 1;
				if (!flag2)
				{
					bool flag3 = evt.pointerType != PointerType.mouse;
					if (flag3)
					{
						bool flag4 = (evt.position - this.m_TouchDownPosition).sqrMagnitude <= 100f;
						if (flag4)
						{
							this.DoSelect(evt.localPosition, evt.actionKey, evt.shiftKey);
						}
					}
					else
					{
						Vector2 vector = default(Vector2);
						MultiColumnLayoutConfiguration multiColumnLayoutConfiguration = this.layoutConfiguration as MultiColumnLayoutConfiguration;
						bool flag5 = multiColumnLayoutConfiguration != null;
						if (flag5)
						{
							vector = new Vector2(0f, multiColumnLayoutConfiguration.headerContainer.rect.height);
						}
						int indexFromPosition = this.GetIndexFromPosition(evt.localPosition - vector);
						bool flag6 = this.selectionType == SelectionType.Multiple && evt.button == 0 && !evt.shiftKey && !evt.actionKey && this.m_Selection.indexCount > 1 && this.m_Selection.ContainsIndex(indexFromPosition);
						if (flag6)
						{
							this.SetSelection(indexFromPosition);
						}
					}
				}
			}
		}

		private void OnPointerCancel(PointerCancelEvent evt)
		{
			bool flag = !evt.isPrimary;
			if (!flag)
			{
				this.ClearSelection();
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
					this.OnPointerDown(evt);
				}
				else
				{
					this.OnPointerUp(evt);
				}
			}
		}

		private void OnPointerDown(IPointerEvent evt)
		{
			this.processingPointerDownEvent = true;
			try
			{
				bool flag = !evt.isPrimary;
				if (!flag)
				{
					bool flag2 = evt.button != 0 && evt.button != 1;
					if (!flag2)
					{
						bool flag3 = evt.pointerType != PointerType.mouse;
						if (flag3)
						{
							this.m_TouchDownPosition = evt.position;
						}
						else
						{
							this.DoSelect(evt.localPosition, evt.actionKey, evt.shiftKey);
						}
					}
				}
			}
			finally
			{
				this.processingPointerDownEvent = false;
			}
		}

		private void DoSelect(Vector2 localPosition, bool actionKey, bool shiftKey)
		{
			Vector2 vector = default(Vector2);
			MultiColumnLayoutConfiguration multiColumnLayoutConfiguration = this.layoutConfiguration as MultiColumnLayoutConfiguration;
			bool flag = multiColumnLayoutConfiguration != null;
			if (flag)
			{
				vector = new Vector2(0f, multiColumnLayoutConfiguration.headerContainer.rect.height);
			}
			int indexFromPosition = this.GetIndexFromPosition(localPosition - vector);
			bool flag2 = indexFromPosition > this.itemsSource.Count - 1;
			if (!flag2)
			{
				this.m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.None;
				switch (this.selectionType)
				{
				case SelectionType.None:
					break;
				case SelectionType.Single:
					if (!this.m_Selection.ContainsIndex(indexFromPosition))
					{
						goto IL_0112;
					}
					break;
				case SelectionType.Multiple:
					if (!actionKey)
					{
						if (!shiftKey)
						{
							if (!this.m_Selection.ContainsIndex(indexFromPosition))
							{
								goto IL_0112;
							}
						}
						else
						{
							bool flag3 = this.m_Selection.indexCount == 0;
							if (flag3)
							{
								this.SetSelection(indexFromPosition);
							}
							else
							{
								this.DoRangeSelection(indexFromPosition);
							}
						}
					}
					else
					{
						bool flag4 = this.m_Selection.ContainsIndex(indexFromPosition);
						if (flag4)
						{
							this.RemoveFromSelection(indexFromPosition);
						}
						else
						{
							this.AddToSelection(indexFromPosition);
						}
					}
					break;
				default:
					goto IL_0112;
				}
				return;
				IL_0112:
				this.SetSelection(indexFromPosition);
			}
		}

		private void DoRangeSelection(int rangeSelectionFinalIndex)
		{
			bool flag = rangeSelectionFinalIndex < 0 || rangeSelectionFinalIndex >= this.itemsSource.Count;
			if (!flag)
			{
				int num = this.m_Selection.minIndex;
				int num2 = this.m_Selection.maxIndex;
				CollectionView.RangeSelectionDirection rangeSelectionDirection = this.m_RangeSelectionDirection;
				CollectionView.RangeSelectionDirection rangeSelectionDirection2 = rangeSelectionDirection;
				if (rangeSelectionDirection2 != CollectionView.RangeSelectionDirection.Up)
				{
					if (rangeSelectionDirection2 != CollectionView.RangeSelectionDirection.Down)
					{
						num = Mathf.Min(num, rangeSelectionFinalIndex);
						num2 = Mathf.Max(num2, rangeSelectionFinalIndex);
					}
					else
					{
						num2 = rangeSelectionFinalIndex;
					}
				}
				else
				{
					num = rangeSelectionFinalIndex;
				}
				bool flag2 = num == num2;
				if (flag2)
				{
					this.m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.None;
				}
				int num3 = num2 - num + 1;
				bool flag3 = num3 <= 0;
				if (!flag3)
				{
					int[] array = ArrayPool<int>.Shared.Rent(num3);
					try
					{
						for (int i = 0; i < num3; i++)
						{
							array[i] = num + i;
						}
						this.ClearSelectionWithoutValidation();
						this.AddToSelection(array.AsSpan<int>(0, num3));
					}
					finally
					{
						ArrayPool<int>.Shared.Return(array, false);
					}
				}
			}
		}

		private unsafe void AddToSelection(ReadOnlySpan<int> indices)
		{
			bool flag = indices.Length == 0;
			if (!flag)
			{
				ReadOnlySpan<int> readOnlySpan = indices;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					int num = *readOnlySpan[i];
					this.AddToSelectionWithoutValidation(num);
				}
				this.NotifyOfSelectionChange();
				base.SaveViewData();
			}
		}

		private void AddToSelectionWithoutValidation(int index)
		{
			bool flag = this.m_Selection.ContainsIndex(index);
			if (!flag)
			{
				RecycledItem recycledItem;
				bool flag2 = this.m_IndexToItemDictionary.TryGetValue(index, out recycledItem);
				if (flag2)
				{
					recycledItem.SetSelected(true);
				}
				this.m_Selection.AddIndex(index);
			}
		}

		public unsafe void AddToSelection(int index)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)4];
			*intPtr = index;
			Span<int> span = new Span<int>(intPtr, 1);
			this.AddToSelection(span);
		}

		public void RemoveFromSelection(int index)
		{
			bool flag = !this.m_Selection.TryRemove(index);
			if (!flag)
			{
				RecycledItem recycledItem;
				bool flag2 = this.m_IndexToItemDictionary.TryGetValue(index, out recycledItem);
				if (flag2)
				{
					recycledItem.SetSelected(false);
				}
				this.m_Selection.TryRemove(index);
				this.NotifyOfSelectionChange();
				base.SaveViewData();
			}
		}

		public void SetSelection(IReadOnlyList<int> indices)
		{
			this.SetSelectionInternal(indices, true);
		}

		public void SetSelection(ReadOnlySpan<int> indices)
		{
			this.SetSelectionInternal(indices, true);
		}

		public unsafe void SetSelection(int index)
		{
			bool flag = index < 0;
			if (flag)
			{
				this.ClearSelection();
			}
			else
			{
				IntPtr intPtr = stackalloc byte[(UIntPtr)4];
				*intPtr = index;
				Span<int> span = new Span<int>(intPtr, 1);
				this.SetSelection(span);
			}
		}

		public void SetSelectionWithoutNotify(IReadOnlyList<int> indices)
		{
			this.SetSelectionInternal(indices, false);
		}

		public void SetSelectionWithoutNotify(ReadOnlySpan<int> indices)
		{
			this.SetSelectionInternal(indices, false);
		}

		private unsafe void SetSelectionInternal(IReadOnlyList<int> indices, bool sendNotification)
		{
			bool flag = indices == null;
			if (!flag)
			{
				int count = indices.Count;
				bool flag2 = count == 0;
				if (flag2)
				{
					this.SetSelectionInternal(ReadOnlySpan<int>.Empty, sendNotification);
				}
				else
				{
					bool flag3 = count < 16;
					if (flag3)
					{
						int num = count;
						Span<int> span2;
						int num2;
						checked
						{
							Span<int> span = new Span<int>(stackalloc byte[unchecked((UIntPtr)num) * 4], num);
							span2 = span;
							num2 = 0;
						}
						foreach (int num3 in indices)
						{
							*span2[num2++] = num3;
						}
						this.SetSelectionInternal(span2, sendNotification);
					}
					else
					{
						byte[] array = ArrayPool<byte>.Shared.Rent(count * 4);
						try
						{
							Span<int> span3 = MemoryMarshal.Cast<byte, int>(array);
							int num4 = 0;
							foreach (int num5 in indices)
							{
								*span3[num4++] = num5;
							}
							Span<int> span = span3;
							span3 = span.Slice(0, num4);
							this.SetSelectionInternal(span3, sendNotification);
						}
						finally
						{
							ArrayPool<byte>.Shared.Return(array, false);
						}
					}
				}
				base.SaveViewData();
			}
		}

		private unsafe void SetSelectionInternal(ReadOnlySpan<int> indices, bool sendNotification)
		{
			bool flag = this.MatchesExistingSelection(indices);
			if (!flag)
			{
				int selectedIndex = this.selectedIndex;
				this.ClearSelectionWithoutValidation();
				bool flag2 = this.m_Selection.capacity < indices.Length;
				if (flag2)
				{
					this.m_Selection.capacity = indices.Length;
				}
				ReadOnlySpan<int> readOnlySpan = indices;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					int num = *readOnlySpan[i];
					this.AddToSelectionWithoutValidation(num);
				}
				if (sendNotification)
				{
					bool flag3 = selectedIndex != this.selectedIndex;
					if (flag3)
					{
						base.NotifyPropertyChanged(in CollectionView.selectedIndexProperty);
					}
					this.NotifyOfSelectionChange();
				}
				base.SaveViewData();
			}
		}

		private bool MatchesExistingSelection(ReadOnlySpan<int> indices)
		{
			bool flag = indices.Length != this.m_Selection.indexCount;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ReadOnlySpan<int> readOnlySpan = NoAllocHelpers.CreateReadOnlySpan<int>(this.m_Selection.indices);
				flag2 = readOnlySpan.SequenceEqual<int>(indices);
			}
			return flag2;
		}

		public void ClearSelection()
		{
			bool flag = this.m_Selection.indices.Count == 0;
			if (!flag)
			{
				this.ClearSelectionWithoutValidation();
				this.NotifyOfSelectionChange();
			}
		}

		private void ClearSelectionWithoutValidation()
		{
			foreach (KeyValuePair<int, RecycledItem> keyValuePair in this.m_IndexToItemDictionary)
			{
				int num;
				RecycledItem recycledItem;
				keyValuePair.Deconstruct(out num, out recycledItem);
				RecycledItem recycledItem2 = recycledItem;
				recycledItem2.SetSelected(false);
			}
			this.m_Selection.ClearIndices();
		}

		internal virtual CollectionViewDragger CreateDragger()
		{
			return new CollectionViewDragger(this);
		}

		private void InitializeDragAndDropController(bool enableReordering)
		{
			bool flag = this.m_Dragger != null;
			if (flag)
			{
				this.m_Dragger.UnregisterCallbacksFromTarget(true);
				this.m_Dragger.dragAndDropController = null;
				this.m_Dragger = null;
			}
			this.m_Dragger = this.CreateDragger();
			this.m_Dragger.dragAndDropController = this.CreateDragAndDropController();
			bool flag2 = this.m_Dragger.dragAndDropController == null;
			if (!flag2)
			{
				this.m_Dragger.dragAndDropController.enableReordering = enableReordering;
			}
		}

		internal void SetDragAndDropController(ICollectionDragAndDropController dragAndDropController)
		{
			if (this.m_Dragger == null)
			{
				this.m_Dragger = this.CreateDragger();
			}
			this.m_Dragger.dragAndDropController = dragAndDropController;
		}

		internal bool HasCanStartDrag()
		{
			return this.canStartDrag != null;
		}

		internal bool RaiseCanStartDrag(RecycledItem item, IEnumerable<int> indices, EventModifiers modifiers)
		{
			Func<CanStartDragArgs, bool> func = this.canStartDrag;
			return func == null || func(new CanStartDragArgs(item.element, item.index, indices, modifiers));
		}

		internal StartDragArgs RaiseSetupDragAndDrop(RecycledItem item, IEnumerable<int> indices, StartDragArgs args)
		{
			Func<SetupDragAndDropArgs, StartDragArgs> func = this.setupDragAndDrop;
			return (func != null) ? func(new SetupDragAndDropArgs(item.element, indices, args)) : args;
		}

		internal DragVisualMode RaiseHandleDragAndDrop(Vector2 pointerPosition, DragAndDropArgs dragAndDropArgs)
		{
			Func<HandleDragAndDropArgs, DragVisualMode> func = this.dragAndDropUpdate;
			return (func != null) ? func(new HandleDragAndDropArgs(pointerPosition, dragAndDropArgs)) : DragVisualMode.None;
		}

		internal DragVisualMode RaiseDrop(Vector2 pointerPosition, DragAndDropArgs dragAndDropArgs)
		{
			Func<HandleDragAndDropArgs, DragVisualMode> func = this.handleDrop;
			return (func != null) ? func(new HandleDragAndDropArgs(pointerPosition, dragAndDropArgs)) : DragVisualMode.None;
		}

		internal void Move(int index, int newIndex)
		{
			bool flag = this.itemsSource == null;
			if (!flag)
			{
				bool flag2 = index == newIndex;
				if (!flag2)
				{
					int i = Mathf.Min(index, newIndex);
					int num = Mathf.Max(index, newIndex);
					bool flag3 = i < 0 || num >= this.itemsSource.Count;
					if (!flag3)
					{
						int num2 = ((newIndex < index) ? 1 : (-1));
						while (i < num)
						{
							this.Swap(index, newIndex);
							newIndex += num2;
							bool flag4 = index < newIndex;
							if (flag4)
							{
								i = index;
								num = newIndex;
							}
							else
							{
								num = index;
								i = newIndex;
							}
						}
					}
				}
			}
		}

		private void Swap(int lhs, int rhs)
		{
			IList itemsSource = this.itemsSource;
			IList itemsSource2 = this.itemsSource;
			object obj = this.itemsSource[rhs];
			object obj2 = this.itemsSource[lhs];
			itemsSource[lhs] = obj;
			itemsSource2[rhs] = obj2;
		}

		private void SelectAll()
		{
			bool flag = this.selectionType != SelectionType.Multiple;
			if (!flag)
			{
				for (int i = 0; i < this.itemsSource.Count; i++)
				{
					this.m_Selection.AddIndex(i);
				}
				foreach (RecycledItem recycledItem in this.m_IndexToItemDictionary.Values)
				{
					recycledItem.SetSelected(true);
				}
				this.NotifyOfSelectionChange();
				base.SaveViewData();
			}
		}

		private bool Apply(KeyboardNavigationOperation op, bool shiftKey)
		{
			CollectionView.<>c__DisplayClass174_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.shiftKey = shiftKey;
			bool flag = this.selectionType == SelectionType.None;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				switch (op)
				{
				case KeyboardNavigationOperation.SelectAll:
					this.SelectAll();
					return true;
				case KeyboardNavigationOperation.Cancel:
					this.ClearSelection();
					return true;
				case KeyboardNavigationOperation.Submit:
					this.ScrollToItem(this.selectedIndex);
					return true;
				case KeyboardNavigationOperation.Previous:
				{
					int num = ((this.m_Selection.indexCount == 0) ? (-1) : ((this.m_RangeSelectionDirection != CollectionView.RangeSelectionDirection.Down) ? this.m_Selection.minIndex : this.m_Selection.maxIndex)) - 1;
					bool flag3 = num >= 0;
					if (flag3)
					{
						bool flag4 = this.m_RangeSelectionDirection == CollectionView.RangeSelectionDirection.None;
						if (flag4)
						{
							this.m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.Up;
						}
						this.<Apply>g__HandleSelectionAndScroll|174_0(num, ref CS$<>8__locals1);
						return true;
					}
					break;
				}
				case KeyboardNavigationOperation.Next:
				{
					int num2 = ((this.m_Selection.indexCount == 0) ? (-1) : ((this.m_RangeSelectionDirection != CollectionView.RangeSelectionDirection.Up) ? this.m_Selection.maxIndex : this.m_Selection.minIndex)) + 1;
					bool flag5 = num2 < this.itemsSource.Count;
					if (flag5)
					{
						bool flag6 = this.m_RangeSelectionDirection == CollectionView.RangeSelectionDirection.None;
						if (flag6)
						{
							this.m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.Down;
						}
						this.<Apply>g__HandleSelectionAndScroll|174_0(num2, ref CS$<>8__locals1);
						return true;
					}
					break;
				}
				case KeyboardNavigationOperation.MoveRight:
				case KeyboardNavigationOperation.MoveLeft:
					break;
				case KeyboardNavigationOperation.PageUp:
				{
					bool flag7 = this.m_Selection.indexCount > 0;
					if (flag7)
					{
						bool flag8 = this.m_RangeSelectionDirection == CollectionView.RangeSelectionDirection.None;
						if (flag8)
						{
							this.m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.Up;
						}
						int num3 = ((this.m_RangeSelectionDirection == CollectionView.RangeSelectionDirection.Up) ? this.m_Selection.minIndex : this.m_Selection.maxIndex);
						this.<Apply>g__HandleSelectionAndScroll|174_0(Mathf.Max(0, num3 - (this.m_DisplayedList.Count - 1)), ref CS$<>8__locals1);
					}
					return true;
				}
				case KeyboardNavigationOperation.PageDown:
				{
					bool flag9 = this.m_Selection.indexCount > 0;
					if (flag9)
					{
						bool flag10 = this.m_RangeSelectionDirection == CollectionView.RangeSelectionDirection.None;
						if (flag10)
						{
							this.m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.Down;
						}
						int num4 = ((this.m_RangeSelectionDirection == CollectionView.RangeSelectionDirection.Up) ? this.m_Selection.minIndex : this.m_Selection.maxIndex);
						this.<Apply>g__HandleSelectionAndScroll|174_0(Mathf.Min(this.itemsSource.Count - 1, num4 + (this.m_DisplayedList.Count - 1)), ref CS$<>8__locals1);
					}
					return true;
				}
				case KeyboardNavigationOperation.Begin:
					this.<Apply>g__HandleSelectionAndScroll|174_0(0, ref CS$<>8__locals1);
					return true;
				case KeyboardNavigationOperation.End:
					this.<Apply>g__HandleSelectionAndScroll|174_0(this.itemsSource.Count - 1, ref CS$<>8__locals1);
					return true;
				default:
					throw new ArgumentOutOfRangeException("op", op, null);
				}
				flag2 = false;
			}
			return flag2;
		}

		private void Apply(KeyboardNavigationOperation op, EventBase sourceEvent)
		{
			KeyDownEvent keyDownEvent = sourceEvent as KeyDownEvent;
			bool flag;
			if (keyDownEvent == null || !keyDownEvent.shiftKey)
			{
				INavigationEvent navigationEvent = sourceEvent as INavigationEvent;
				if (navigationEvent != null)
				{
					if (navigationEvent.shiftKey)
					{
						goto IL_002D;
					}
				}
				flag = false;
				goto IL_0035;
			}
			IL_002D:
			flag = true;
			IL_0035:
			bool flag2 = flag;
			bool flag3 = this.Apply(op, flag2);
			if (flag3)
			{
				sourceEvent.StopPropagation();
			}
			FocusController focusController = this.focusController;
			if (focusController != null)
			{
				focusController.IgnoreEvent(sourceEvent);
			}
		}

		[CompilerGenerated]
		private void <Apply>g__HandleSelectionAndScroll|174_0(int index, ref CollectionView.<>c__DisplayClass174_0 A_2)
		{
			bool flag = index < 0 || index >= this.itemsSource.Count;
			if (!flag)
			{
				bool flag2 = ((this.selectionType == SelectionType.Multiple) & A_2.shiftKey) && this.m_Selection.indexCount != 0;
				if (flag2)
				{
					this.DoRangeSelection(index);
				}
				else
				{
					this.m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.None;
					this.selectedIndex = index;
				}
				this.ScrollToItem(index);
			}
		}

		internal static readonly BindingId itemsSourceProperty = "itemsSource";

		internal static readonly BindingId selectionTypeProperty = "selectionType";

		internal static readonly BindingId selectedIndexProperty = "selectedIndex";

		internal static readonly BindingId reorderableProperty = "reorderable";

		internal static readonly BindingId reorderModeProperty = "reorderMode";

		internal static readonly BindingId showBorderProperty = "showBorder";

		internal static readonly BindingId showAlternatingRowBackgroundsProperty = "showAlternatingRowBackgrounds";

		internal static readonly BindingId fixedItemHeightProperty = "fixedItemHeight";

		internal static readonly BindingId selectedIndicesProperty = "selectedIndices";

		private VisualElement m_Container;

		private ScrollContainer m_ScrollView;

		private CollectionViewScroller m_VerticalScroller;

		private CollectionViewScroller m_HorizontalScroller;

		private CollectionViewDragger m_Dragger;

		private CollectionViewLayoutConfiguration m_Configuration;

		private IList m_ItemsSource;

		private LinkedList<RecycledItem> m_RefreshList = new LinkedList<RecycledItem>();

		private LinkedList<RecycledItem> m_DisplayedList = new LinkedList<RecycledItem>();

		private KeyboardNavigationManipulator m_NavigationManipulator;

		private IVisualElementScheduledItem m_RebuildScheduled;

		private IVisualElementScheduledItem m_ScrollScheduledItem;

		private List<int> m_LastFocusedElementTreeChildIndexes = new List<int>();

		private readonly LinkedList<RecycledItem> m_FreeList = new LinkedList<RecycledItem>();

		private readonly CollectionViewSelection m_Selection = new CollectionViewSelection();

		private bool m_IsChangingScrollingParameters;

		private double m_DelayedScrolledVerticalValue = 0.0;

		private double m_ScrollValue;

		private float m_FixedItemHeight = -1f;

		private float m_ComputedAverageHeight = -1f;

		private float m_LastHeight = -1f;

		private int m_FirstVisibleItemIndex;

		private int m_LastFocusedElementIndex = -1;

		private Vector3 m_TouchDownPosition;

		private AlternatingRowBackground m_ShowAlternatingRowBackgrounds = AlternatingRowBackground.None;

		private CollectionView.RangeSelectionDirection m_RangeSelectionDirection = CollectionView.RangeSelectionDirection.None;

		private SelectionType m_SelectionType;

		private ListViewReorderMode m_ReorderMode;

		private const float k_DefaultItemHeight = 22f;

		private const float k_ScrollThresholdSquared = 100f;

		private const float k_DefaultScrollSize = 10f;

		private const float k_Buffer = 1f;

		internal readonly Dictionary<int, RecycledItem> m_IndexToItemDictionary = new Dictionary<int, RecycledItem>();

		public static readonly string verticalScrollerVisibleUssClassName = BaseVerticalCollectionView.ussClassName + "--vertical-scroller-visible";

		private enum RangeSelectionDirection
		{
			Up = -1,
			None,
			Down
		}
	}
}
