using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	public abstract class BaseVerticalCollectionView : BindableElement, ISerializationCallbackReceiver
	{
		[Obsolete("onItemsChosen is deprecated, use itemsChosen instead", false)]
		public event Action<IEnumerable<object>> onItemsChosen
		{
			add
			{
				this.itemsChosen += value;
			}
			remove
			{
				this.itemsChosen -= value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<object>> itemsChosen;

		[Obsolete("onSelectionChange is deprecated, use selectionChanged instead", false)]
		public event Action<IEnumerable<object>> onSelectionChange
		{
			add
			{
				this.selectionChanged += value;
			}
			remove
			{
				this.selectionChanged -= value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<object>> selectionChanged;

		[Obsolete("onSelectedIndicesChange is deprecated, use selectedIndicesChanged instead", false)]
		public event Action<IEnumerable<int>> onSelectedIndicesChange
		{
			add
			{
				this.selectedIndicesChanged += value;
			}
			remove
			{
				this.selectedIndicesChanged -= value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<IEnumerable<int>> selectedIndicesChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, int> itemIndexChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action itemsSourceChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action selectionNotChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Func<CanStartDragArgs, bool> canStartDrag;

		internal bool HasCanStartDrag()
		{
			return this.canStartDrag != null;
		}

		internal bool RaiseCanStartDrag(ReusableCollectionItem item, IEnumerable<int> ids)
		{
			Func<CanStartDragArgs, bool> func = this.canStartDrag;
			return func == null || func(new CanStartDragArgs((item != null) ? item.rootElement : null, (item != null) ? item.id : (-1), ids));
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Func<SetupDragAndDropArgs, StartDragArgs> setupDragAndDrop;

		internal StartDragArgs RaiseSetupDragAndDrop(ReusableCollectionItem item, IEnumerable<int> ids, StartDragArgs args)
		{
			Func<SetupDragAndDropArgs, StartDragArgs> func = this.setupDragAndDrop;
			return (func != null) ? func(new SetupDragAndDropArgs((item != null) ? item.rootElement : null, ids, args)) : args;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Func<HandleDragAndDropArgs, DragVisualMode> dragAndDropUpdate;

		internal DragVisualMode RaiseHandleDragAndDrop(Vector2 pointerPosition, DragAndDropArgs dragAndDropArgs)
		{
			Func<HandleDragAndDropArgs, DragVisualMode> func = this.dragAndDropUpdate;
			return (func != null) ? func(new HandleDragAndDropArgs(pointerPosition, dragAndDropArgs)) : DragVisualMode.None;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Func<HandleDragAndDropArgs, DragVisualMode> handleDrop;

		internal DragVisualMode RaiseDrop(Vector2 pointerPosition, DragAndDropArgs dragAndDropArgs)
		{
			Func<HandleDragAndDropArgs, DragVisualMode> func = this.handleDrop;
			return (func != null) ? func(new HandleDragAndDropArgs(pointerPosition, dragAndDropArgs)) : DragVisualMode.None;
		}

		public IList itemsSource
		{
			get
			{
				CollectionViewController viewController = this.viewController;
				return (viewController != null) ? viewController.itemsSource : null;
			}
			set
			{
				this.GetOrCreateViewController().itemsSource = value;
			}
		}

		[Obsolete("makeItem has been moved to ListView and TreeView. Use these ones instead.")]
		public Func<VisualElement> makeItem
		{
			get
			{
				throw new UnityException("makeItem has been moved to ListView and TreeView. Use these ones instead.");
			}
			set
			{
				throw new UnityException("makeItem has been moved to ListView and TreeView. Use these ones instead.");
			}
		}

		[Obsolete("bindItem has been moved to ListView and TreeView. Use these ones instead.")]
		public Action<VisualElement, int> bindItem
		{
			get
			{
				throw new UnityException("bindItem has been moved to ListView and TreeView. Use these ones instead.");
			}
			set
			{
				throw new UnityException("bindItem has been moved to ListView and TreeView. Use these ones instead.");
			}
		}

		[Obsolete("unbindItem has been moved to ListView and TreeView. Use these ones instead.")]
		public Action<VisualElement, int> unbindItem
		{
			get
			{
				throw new UnityException("unbindItem has been moved to ListView and TreeView. Use these ones instead.");
			}
			set
			{
				throw new UnityException("unbindItem has been moved to ListView and TreeView. Use these ones instead.");
			}
		}

		[Obsolete("destroyItem has been moved to ListView and TreeView. Use these ones instead.")]
		public Action<VisualElement> destroyItem
		{
			get
			{
				throw new UnityException("destroyItem has been moved to ListView and TreeView. Use these ones instead.");
			}
			set
			{
				throw new UnityException("destroyItem has been moved to ListView and TreeView. Use these ones instead.");
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
			}
		}

		public object selectedItem
		{
			get
			{
				return this.m_Selection.FirstObject();
			}
		}

		public IEnumerable<object> selectedItems
		{
			get
			{
				foreach (int index in this.m_Selection.indices)
				{
					object item;
					bool flag = this.m_Selection.items.TryGetValue(index, out item);
					if (flag)
					{
						yield return item;
					}
					else
					{
						yield return null;
					}
					item = null;
				}
				List<int>.Enumerator enumerator = default(List<int>.Enumerator);
				yield break;
				yield break;
			}
		}

		public int selectedIndex
		{
			get
			{
				return (this.m_Selection.indexCount == 0) ? (-1) : this.m_Selection.FirstIndex();
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
				return this.m_Selection.indices;
			}
		}

		internal IEnumerable<int> selectedIds
		{
			get
			{
				return this.m_Selection.selectedIds;
			}
		}

		internal IEnumerable<ReusableCollectionItem> activeItems
		{
			get
			{
				CollectionVirtualizationController virtualizationController = this.m_VirtualizationController;
				return ((virtualizationController != null) ? virtualizationController.activeItems : null) ?? BaseVerticalCollectionView.k_EmptyItems;
			}
		}

		internal ScrollView scrollView
		{
			get
			{
				return this.m_ScrollView;
			}
		}

		internal ListViewDragger dragger
		{
			get
			{
				return this.m_Dragger;
			}
		}

		internal CollectionVirtualizationController virtualizationController
		{
			get
			{
				return this.GetOrCreateVirtualizationController();
			}
		}

		public CollectionViewController viewController
		{
			get
			{
				return this.m_ViewController;
			}
		}

		[Obsolete("resolvedItemHeight is deprecated and will be removed from the API.", false)]
		public float resolvedItemHeight
		{
			get
			{
				return this.ResolveItemHeight(-1f);
			}
		}

		internal float ResolveItemHeight(float height = -1f)
		{
			float scaledPixelsPerPoint = base.scaledPixelsPerPoint;
			height = ((height < 0f) ? this.fixedItemHeight : height);
			return Mathf.Round(height * scaledPixelsPerPoint) / scaledPixelsPerPoint;
		}

		public bool showBorder
		{
			get
			{
				return this.m_ScrollView.ClassListContains(BaseVerticalCollectionView.borderUssClassName);
			}
			set
			{
				this.m_ScrollView.EnableInClassList(BaseVerticalCollectionView.borderUssClassName, value);
			}
		}

		public bool reorderable
		{
			get
			{
				ListViewDragger dragger = this.m_Dragger;
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
				ICollectionDragAndDropController dragAndDropController = this.m_Dragger.dragAndDropController;
				bool flag = dragAndDropController != null && dragAndDropController.enableReordering != value;
				if (flag)
				{
					dragAndDropController.enableReordering = value;
					this.Rebuild();
				}
			}
		}

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
					this.m_ScrollView.horizontalScrollerVisibility = (value ? ScrollerVisibility.Auto : ScrollerVisibility.Hidden);
					this.m_ScrollView.mode = (value ? ScrollViewMode.VerticalAndHorizontal : ScrollViewMode.Vertical);
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
					this.RefreshItems();
				}
			}
		}

		public CollectionVirtualizationMethod virtualizationMethod
		{
			get
			{
				return this.m_VirtualizationMethod;
			}
			set
			{
				CollectionVirtualizationMethod virtualizationMethod = this.m_VirtualizationMethod;
				this.m_VirtualizationMethod = value;
				bool flag = virtualizationMethod != value;
				if (flag)
				{
					this.CreateVirtualizationController();
					this.Rebuild();
				}
			}
		}

		[Obsolete("itemHeight is deprecated, use fixedItemHeight instead.", false)]
		public int itemHeight
		{
			get
			{
				return (int)this.fixedItemHeight;
			}
			set
			{
				this.fixedItemHeight = (float)value;
			}
		}

		public float fixedItemHeight
		{
			get
			{
				return this.m_FixedItemHeight;
			}
			set
			{
				bool flag = value < 0f;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("fixedItemHeight", "Value needs to be positive for virtualization.");
				}
				this.m_ItemHeightIsInline = true;
				bool flag2 = Math.Abs(this.m_FixedItemHeight - value) > float.Epsilon;
				if (flag2)
				{
					this.m_FixedItemHeight = value;
					this.RefreshItems();
				}
			}
		}

		internal float lastHeight
		{
			get
			{
				return this.m_LastHeight;
			}
		}

		private protected virtual void CreateVirtualizationController()
		{
			this.CreateVirtualizationController<ReusableCollectionItem>();
		}

		internal CollectionVirtualizationController GetOrCreateVirtualizationController()
		{
			bool flag = this.m_VirtualizationController == null;
			if (flag)
			{
				this.CreateVirtualizationController();
			}
			return this.m_VirtualizationController;
		}

		internal void CreateVirtualizationController<T>() where T : ReusableCollectionItem, new()
		{
			CollectionVirtualizationMethod virtualizationMethod = this.virtualizationMethod;
			CollectionVirtualizationMethod collectionVirtualizationMethod = virtualizationMethod;
			if (collectionVirtualizationMethod != CollectionVirtualizationMethod.FixedHeight)
			{
				if (collectionVirtualizationMethod != CollectionVirtualizationMethod.DynamicHeight)
				{
					throw new ArgumentOutOfRangeException("virtualizationMethod", this.virtualizationMethod, "Unsupported virtualizationMethod virtualization");
				}
				this.m_VirtualizationController = new DynamicHeightVirtualizationController<T>(this);
			}
			else
			{
				this.m_VirtualizationController = new FixedHeightVirtualizationController<T>(this);
			}
		}

		internal CollectionViewController GetOrCreateViewController()
		{
			bool flag = this.m_ViewController == null;
			if (flag)
			{
				this.SetViewController(this.CreateViewController());
			}
			return this.m_ViewController;
		}

		protected abstract CollectionViewController CreateViewController();

		public virtual void SetViewController(CollectionViewController controller)
		{
			bool flag = this.m_ViewController != null;
			if (flag)
			{
				this.m_ViewController.itemIndexChanged -= this.m_ItemIndexChangedCallback;
				this.m_ViewController.itemsSourceChanged -= this.m_ItemsSourceChangedCallback;
				this.m_ViewController.Dispose();
				this.m_ViewController = null;
			}
			this.m_ViewController = controller;
			bool flag2 = this.m_ViewController != null;
			if (flag2)
			{
				this.m_ViewController.SetView(this);
				this.m_ViewController.itemIndexChanged += this.m_ItemIndexChangedCallback;
				this.m_ViewController.itemsSourceChanged += this.m_ItemsSourceChangedCallback;
			}
		}

		internal virtual ListViewDragger CreateDragger()
		{
			return new ListViewDragger(this);
		}

		internal void InitializeDragAndDropController(bool enableReordering)
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

		internal abstract ICollectionDragAndDropController CreateDragAndDropController();

		internal void SetDragAndDropController(ICollectionDragAndDropController dragAndDropController)
		{
			if (this.m_Dragger == null)
			{
				this.m_Dragger = this.CreateDragger();
			}
			this.m_Dragger.dragAndDropController = dragAndDropController;
		}

		public BaseVerticalCollectionView()
		{
			base.AddToClassList(BaseVerticalCollectionView.ussClassName);
			this.m_Selection = new BaseVerticalCollectionView.Selection
			{
				selectedIds = this.m_SelectedIds
			};
			this.selectionType = SelectionType.Single;
			this.m_ScrollView = new ScrollView();
			this.m_ScrollView.AddToClassList(BaseVerticalCollectionView.listScrollViewUssClassName);
			this.m_ScrollView.verticalScroller.valueChanged += delegate(float v)
			{
				this.OnScroll(new Vector2(0f, v));
			};
			this.m_ScrollView.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChanged), TrickleDown.NoTrickleDown);
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
			this.m_ScrollView.contentContainer.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanel), TrickleDown.NoTrickleDown);
			base.hierarchy.Add(this.m_ScrollView);
			this.m_ScrollView.contentContainer.focusable = true;
			this.m_ScrollView.contentContainer.usageHints &= ~UsageHints.GroupTransform;
			this.m_ScrollView.viewDataKey = "unity-vertical-collection-scroll-view";
			this.m_ScrollView.verticalScroller.viewDataKey = null;
			this.m_ScrollView.horizontalScroller.viewDataKey = null;
			base.focusable = true;
			base.isCompositeRoot = true;
			base.delegatesFocus = true;
			this.m_ItemIndexChangedCallback = new Action<int, int>(this.OnItemIndexChanged);
			this.m_ItemsSourceChangedCallback = new Action(this.OnItemsSourceChanged);
			this.InitializeDragAndDropController(false);
		}

		public BaseVerticalCollectionView(IList itemsSource, float itemHeight = -1f)
			: this()
		{
			bool flag = Math.Abs(itemHeight - -1f) > float.Epsilon;
			if (flag)
			{
				this.m_FixedItemHeight = itemHeight;
				this.m_ItemHeightIsInline = true;
			}
			bool flag2 = itemsSource != null;
			if (flag2)
			{
				this.itemsSource = itemsSource;
			}
		}

		[Obsolete("makeItem and bindItem are now in ListView and TreeView directly, please use a constructor without these parameters.")]
		public BaseVerticalCollectionView(IList itemsSource, float itemHeight = -1f, Func<VisualElement> makeItem = null, Action<VisualElement, int> bindItem = null)
			: this()
		{
			bool flag = Math.Abs(itemHeight - -1f) > float.Epsilon;
			if (flag)
			{
				this.m_FixedItemHeight = itemHeight;
				this.m_ItemHeightIsInline = true;
			}
			this.itemsSource = itemsSource;
		}

		public VisualElement GetRootElementForId(int id)
		{
			ReusableCollectionItem reusableCollectionItem = this.activeItems.FirstOrDefault<ReusableCollectionItem>((ReusableCollectionItem t) => t.id == id);
			return (reusableCollectionItem != null) ? reusableCollectionItem.rootElement : null;
		}

		public VisualElement GetRootElementForIndex(int index)
		{
			return this.GetRootElementForId(this.viewController.GetIdForIndex(index));
		}

		internal virtual bool HasValidDataAndBindings()
		{
			return this.m_ViewController != null && this.itemsSource != null;
		}

		private void OnItemIndexChanged(int srcIndex, int dstIndex)
		{
			Action<int, int> action = this.itemIndexChanged;
			if (action != null)
			{
				action(srcIndex, dstIndex);
			}
			this.RefreshItems();
		}

		private void OnItemsSourceChanged()
		{
			Action action = this.itemsSourceChanged;
			if (action != null)
			{
				action();
			}
		}

		public void RefreshItem(int index)
		{
			foreach (ReusableCollectionItem reusableCollectionItem in this.activeItems)
			{
				int index2 = reusableCollectionItem.index;
				bool flag = index2 == index;
				if (flag)
				{
					this.viewController.InvokeUnbindItem(reusableCollectionItem, index2);
					this.viewController.InvokeBindItem(reusableCollectionItem, index2);
					break;
				}
			}
		}

		public void RefreshItems()
		{
			using (BaseVerticalCollectionView.k_RefreshMarker.Auto())
			{
				bool flag = this.m_ViewController == null;
				if (!flag)
				{
					IVisualElementScheduledItem rebuildScheduled = this.m_RebuildScheduled;
					bool flag2 = rebuildScheduled != null && rebuildScheduled.isActive;
					if (flag2)
					{
						this.Rebuild();
					}
					else
					{
						this.RefreshSelection();
						this.virtualizationController.Refresh(false);
						this.PostRefresh();
					}
				}
			}
		}

		[Obsolete("Refresh() has been deprecated. Use Rebuild() instead. (UnityUpgradable) -> Rebuild()", false)]
		public void Refresh()
		{
			this.Rebuild();
		}

		public void Rebuild()
		{
			using (BaseVerticalCollectionView.k_RebuildMarker.Auto())
			{
				bool flag = this.m_ViewController == null;
				if (!flag)
				{
					this.RefreshSelection();
					this.virtualizationController.Refresh(true);
					this.PostRefresh();
					IVisualElementScheduledItem rebuildScheduled = this.m_RebuildScheduled;
					if (rebuildScheduled != null)
					{
						rebuildScheduled.Pause();
					}
				}
			}
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

		private void RefreshSelection()
		{
			BaseVerticalCollectionView.<>c__DisplayClass172_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.selectedIndicesChanged = false;
			CS$<>8__locals1.previousSelectionCount = this.m_Selection.indexCount;
			this.m_Selection.items.Clear();
			CollectionViewController viewController = this.viewController;
			bool flag = ((viewController != null) ? viewController.itemsSource : null) == null;
			if (flag)
			{
				this.m_Selection.ClearIndices();
				this.<RefreshSelection>g__NotifyIfChanged|172_0(ref CS$<>8__locals1);
			}
			else
			{
				bool flag2 = this.m_Selection.idCount > 0;
				if (flag2)
				{
					List<int> list;
					using (CollectionPool<List<int>, int>.Get(out list))
					{
						foreach (int num in this.m_Selection.selectedIds)
						{
							int indexForId = this.viewController.GetIndexForId(num);
							bool flag3 = indexForId < 0;
							if (flag3)
							{
								CS$<>8__locals1.selectedIndicesChanged = true;
							}
							else
							{
								bool flag4 = !this.m_Selection.ContainsIndex(indexForId);
								if (flag4)
								{
									CS$<>8__locals1.selectedIndicesChanged = true;
								}
								list.Add(indexForId);
							}
						}
						this.m_Selection.ClearIndices();
						foreach (int num2 in list)
						{
							this.m_Selection.AddIndex(num2, this.viewController.GetItemForIndex(num2));
						}
					}
				}
				this.<RefreshSelection>g__NotifyIfChanged|172_0(ref CS$<>8__locals1);
			}
		}

		private protected virtual void PostRefresh()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				this.m_LastHeight = this.m_ScrollView.layout.height;
				bool flag2 = float.IsNaN(this.m_ScrollView.layout.height);
				if (!flag2)
				{
					this.Resize(this.m_ScrollView.layout.size);
				}
			}
		}

		public void ScrollTo(VisualElement visualElement)
		{
			this.m_ScrollView.ScrollTo(visualElement);
		}

		public void ScrollToItem(int index)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				this.virtualizationController.ScrollToItem(index);
			}
		}

		[Obsolete("ScrollToId() has been deprecated. Use ScrollToItemById() instead. (UnityUpgradable) -> ScrollToItemById(*)", false)]
		public void ScrollToId(int id)
		{
			this.ScrollToItemById(id);
		}

		public void ScrollToItemById(int id)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				int indexForId = this.viewController.GetIndexForId(id);
				this.virtualizationController.ScrollToItem(indexForId);
			}
		}

		private void OnScroll(Vector2 offset)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				this.virtualizationController.OnScroll(offset);
			}
		}

		private void Resize(Vector2 size)
		{
			this.virtualizationController.Resize(size);
			this.m_LastHeight = size.y;
			this.virtualizationController.UpdateBackground();
		}

		private void OnAttachToPanel(AttachToPanelEvent evt)
		{
			bool flag = evt.destinationPanel == null;
			if (!flag)
			{
				this.m_ScrollView.contentContainer.AddManipulator(this.m_NavigationManipulator = new KeyboardNavigationManipulator(new Action<KeyboardNavigationOperation, EventBase>(this.Apply)));
				this.m_ScrollView.contentContainer.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.RegisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			}
		}

		private void OnDetachFromPanel(DetachFromPanelEvent evt)
		{
			bool flag = evt.originPanel == null;
			if (!flag)
			{
				this.m_ScrollView.contentContainer.RemoveManipulator(this.m_NavigationManipulator);
				this.m_ScrollView.contentContainer.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.UnregisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
				this.m_ScrollView.contentContainer.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			}
		}

		[Obsolete("OnKeyDown is obsolete and will be removed from ListView. Use the event system instead, i.e. SendEvent(EventBase e).", true)]
		public void OnKeyDown(KeyDownEvent evt)
		{
			this.m_NavigationManipulator.OnKeyDown(evt);
		}

		private bool Apply(KeyboardNavigationOperation op, bool shiftKey, bool altKey)
		{
			BaseVerticalCollectionView.<>c__DisplayClass183_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.shiftKey = shiftKey;
			bool flag = this.selectionType == SelectionType.None || !this.HasValidDataAndBindings();
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
				{
					Action<IEnumerable<object>> action = this.itemsChosen;
					if (action != null)
					{
						action(this.selectedItems);
					}
					this.ScrollToItem(this.selectedIndex);
					return true;
				}
				case KeyboardNavigationOperation.Previous:
				{
					bool flag3 = this.selectedIndex > 0;
					if (flag3)
					{
						this.<Apply>g__HandleSelectionAndScroll|183_0(this.selectedIndex - 1, ref CS$<>8__locals1);
						return true;
					}
					break;
				}
				case KeyboardNavigationOperation.Next:
				{
					bool flag4 = this.selectedIndex + 1 < this.m_ViewController.itemsSource.Count;
					if (flag4)
					{
						this.<Apply>g__HandleSelectionAndScroll|183_0(this.selectedIndex + 1, ref CS$<>8__locals1);
						return true;
					}
					break;
				}
				case KeyboardNavigationOperation.MoveRight:
				{
					bool flag5 = this.m_Selection.indexCount > 0;
					if (flag5)
					{
						return this.HandleItemNavigation(true, altKey);
					}
					break;
				}
				case KeyboardNavigationOperation.MoveLeft:
				{
					bool flag6 = this.m_Selection.indexCount > 0;
					if (flag6)
					{
						return this.HandleItemNavigation(false, altKey);
					}
					break;
				}
				case KeyboardNavigationOperation.PageUp:
				{
					bool flag7 = this.m_Selection.indexCount > 0;
					if (flag7)
					{
						int num = (this.m_IsRangeSelectionDirectionUp ? this.m_Selection.minIndex : this.m_Selection.maxIndex);
						this.<Apply>g__HandleSelectionAndScroll|183_0(Mathf.Max(0, num - (this.virtualizationController.visibleItemCount - 1)), ref CS$<>8__locals1);
					}
					return true;
				}
				case KeyboardNavigationOperation.PageDown:
				{
					bool flag8 = this.m_Selection.indexCount > 0;
					if (flag8)
					{
						int num2 = (this.m_IsRangeSelectionDirectionUp ? this.m_Selection.minIndex : this.m_Selection.maxIndex);
						this.<Apply>g__HandleSelectionAndScroll|183_0(Mathf.Min(this.viewController.itemsSource.Count - 1, num2 + (this.virtualizationController.visibleItemCount - 1)), ref CS$<>8__locals1);
					}
					return true;
				}
				case KeyboardNavigationOperation.Begin:
					this.<Apply>g__HandleSelectionAndScroll|183_0(0, ref CS$<>8__locals1);
					return true;
				case KeyboardNavigationOperation.End:
					this.<Apply>g__HandleSelectionAndScroll|183_0(this.m_ViewController.itemsSource.Count - 1, ref CS$<>8__locals1);
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
						goto IL_0030;
					}
				}
				flag = false;
				goto IL_0038;
			}
			IL_0030:
			flag = true;
			IL_0038:
			bool flag2 = flag;
			keyDownEvent = sourceEvent as KeyDownEvent;
			bool flag3;
			if (keyDownEvent == null || !keyDownEvent.altKey)
			{
				INavigationEvent navigationEvent = sourceEvent as INavigationEvent;
				if (navigationEvent != null)
				{
					if (navigationEvent.altKey)
					{
						goto IL_006C;
					}
				}
				flag3 = false;
				goto IL_0072;
			}
			IL_006C:
			flag3 = true;
			IL_0072:
			bool flag4 = flag3;
			bool flag5 = this.Apply(op, flag2, flag4);
			if (flag5)
			{
				sourceEvent.StopPropagation();
				sourceEvent.PreventDefault();
			}
		}

		private protected virtual bool HandleItemNavigation(bool moveIn, bool altKey)
		{
			return false;
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
			bool flag = evt.pointerType != PointerType.mouse;
			if (flag)
			{
				this.ProcessPointerDown(evt);
				base.panel.PreventCompatibilityMouseEvents(evt.pointerId);
			}
			else
			{
				this.ProcessPointerDown(evt);
			}
		}

		private void OnPointerCancel(PointerCancelEvent evt)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = !evt.isPrimary;
				if (!flag2)
				{
					this.ClearSelection();
				}
			}
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			bool flag = evt.pointerType != PointerType.mouse;
			if (flag)
			{
				this.ProcessPointerUp(evt);
				base.panel.PreventCompatibilityMouseEvents(evt.pointerId);
			}
			else
			{
				this.ProcessPointerUp(evt);
			}
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
							bool flag5 = (evt.position - this.m_TouchDownPosition).sqrMagnitude <= 100f;
							if (flag5)
							{
								this.DoSelect(evt.localPosition, evt.clickCount, evt.actionKey, evt.shiftKey);
							}
						}
						else
						{
							int indexFromPosition = this.virtualizationController.GetIndexFromPosition(evt.localPosition);
							bool flag6 = this.selectionType == SelectionType.Multiple && !evt.shiftKey && !evt.actionKey && this.m_Selection.indexCount > 1 && this.m_Selection.ContainsIndex(indexFromPosition);
							if (flag6)
							{
								this.ProcessSingleClick(indexFromPosition);
							}
						}
					}
				}
			}
		}

		private void DoSelect(Vector2 localPosition, int clickCount, bool actionKey, bool shiftKey)
		{
			int indexFromPosition = this.virtualizationController.GetIndexFromPosition(localPosition);
			int num = ((this.m_Selection.indexCount > 0 && this.m_Selection.FirstIndex() != indexFromPosition) ? 1 : ((clickCount > 2) ? 2 : clickCount));
			bool flag = indexFromPosition > this.viewController.itemsSource.Count - 1;
			if (!flag)
			{
				bool flag2 = this.selectionType == SelectionType.None;
				if (!flag2)
				{
					int idForIndex = this.viewController.GetIdForIndex(indexFromPosition);
					int num2 = num;
					int num3 = num2;
					if (num3 != 1)
					{
						if (num3 == 2)
						{
							bool flag3 = this.itemsChosen == null;
							if (!flag3)
							{
								bool flag4 = false;
								foreach (int num4 in this.selectedIndices)
								{
									bool flag5 = indexFromPosition == num4;
									if (flag5)
									{
										flag4 = true;
										break;
									}
								}
								this.ProcessSingleClick(indexFromPosition);
								bool flag6 = !flag4;
								if (!flag6)
								{
									Action<IEnumerable<object>> action = this.itemsChosen;
									if (action != null)
									{
										action(this.selectedItems);
									}
								}
							}
						}
					}
					else
					{
						bool flag7 = this.selectionType == SelectionType.Multiple && actionKey;
						if (flag7)
						{
							bool flag8 = this.m_Selection.ContainsId(idForIndex);
							if (flag8)
							{
								this.RemoveFromSelection(indexFromPosition);
							}
							else
							{
								this.AddToSelection(indexFromPosition);
							}
						}
						else
						{
							bool flag9 = this.selectionType == SelectionType.Multiple && shiftKey;
							if (flag9)
							{
								bool flag10 = this.m_Selection.indexCount == 0;
								if (flag10)
								{
									this.SetSelection(indexFromPosition);
								}
								else
								{
									this.DoRangeSelection(indexFromPosition);
								}
							}
							else
							{
								bool flag11 = this.selectionType == SelectionType.Multiple && this.m_Selection.ContainsIndex(indexFromPosition);
								if (flag11)
								{
									Action action2 = this.selectionNotChanged;
									if (action2 != null)
									{
										action2();
									}
								}
								else
								{
									bool flag12 = this.selectionType == SelectionType.Single && this.m_Selection.ContainsIndex(indexFromPosition);
									if (flag12)
									{
										Action action3 = this.selectionNotChanged;
										if (action3 != null)
										{
											action3();
										}
									}
									this.SetSelection(indexFromPosition);
								}
							}
						}
					}
				}
			}
		}

		internal void DoRangeSelection(int rangeSelectionFinalIndex)
		{
			int num = (this.m_IsRangeSelectionDirectionUp ? this.m_Selection.maxIndex : this.m_Selection.minIndex);
			this.ClearSelectionWithoutValidation();
			List<int> list = new List<int>();
			this.m_IsRangeSelectionDirectionUp = rangeSelectionFinalIndex < num;
			bool isRangeSelectionDirectionUp = this.m_IsRangeSelectionDirectionUp;
			if (isRangeSelectionDirectionUp)
			{
				for (int i = rangeSelectionFinalIndex; i <= num; i++)
				{
					list.Add(i);
				}
			}
			else
			{
				for (int j = rangeSelectionFinalIndex; j >= num; j--)
				{
					list.Add(j);
				}
			}
			this.AddToSelection(list);
		}

		private void ProcessSingleClick(int clickedIndex)
		{
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
					for (int i = 0; i < this.m_ViewController.itemsSource.Count; i++)
					{
						int idForIndex = this.viewController.GetIdForIndex(i);
						object itemForIndex = this.viewController.GetItemForIndex(i);
						foreach (ReusableCollectionItem reusableCollectionItem in this.activeItems)
						{
							bool flag3 = reusableCollectionItem.id == idForIndex;
							if (flag3)
							{
								reusableCollectionItem.SetSelected(true);
							}
						}
						bool flag4 = !this.m_Selection.ContainsId(idForIndex);
						if (flag4)
						{
							this.m_Selection.AddId(idForIndex);
							this.m_Selection.AddIndex(i, itemForIndex);
						}
					}
					this.NotifyOfSelectionChange();
					base.SaveViewData();
				}
			}
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
			bool flag = this.m_Selection.ContainsIndex(index);
			if (!flag)
			{
				int idForIndex = this.viewController.GetIdForIndex(index);
				object itemForIndex = this.viewController.GetItemForIndex(index);
				foreach (ReusableCollectionItem reusableCollectionItem in this.activeItems)
				{
					bool flag2 = reusableCollectionItem.id == idForIndex;
					if (flag2)
					{
						reusableCollectionItem.SetSelected(true);
					}
				}
				this.m_Selection.AddId(idForIndex);
				this.m_Selection.AddIndex(index, itemForIndex);
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
			bool flag = !this.m_Selection.TryRemove(index);
			if (!flag)
			{
				int idForIndex = this.viewController.GetIdForIndex(index);
				foreach (ReusableCollectionItem reusableCollectionItem in this.activeItems)
				{
					bool flag2 = reusableCollectionItem.id == idForIndex;
					if (flag2)
					{
						reusableCollectionItem.SetSelected(false);
					}
				}
				this.m_Selection.RemoveId(idForIndex);
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
				bool flag2 = this.MatchesExistingSelection(indices);
				if (!flag2)
				{
					this.ClearSelectionWithoutValidation();
					ICollection collection = indices as ICollection;
					bool flag3 = collection != null && this.m_Selection.capacity < collection.Count;
					if (flag3)
					{
						this.m_Selection.capacity = collection.Count;
					}
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
		}

		private bool MatchesExistingSelection(IEnumerable<int> indices)
		{
			IList<int> list = indices as IList<int>;
			List<int> list2 = null;
			bool flag3;
			try
			{
				bool flag = list == null;
				if (flag)
				{
					list2 = CollectionPool<List<int>, int>.Get();
					list2.AddRange(indices);
					list = list2;
				}
				bool flag2 = list.Count != this.m_Selection.indexCount;
				if (flag2)
				{
					flag3 = false;
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						bool flag4 = list[i] != this.m_Selection.indices[i];
						if (flag4)
						{
							return false;
						}
					}
					flag3 = true;
				}
			}
			finally
			{
				bool flag5 = list2 != null;
				if (flag5)
				{
					CollectionPool<List<int>, int>.Release(list2);
				}
			}
			return flag3;
		}

		private void NotifyOfSelectionChange()
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				Action<IEnumerable<object>> action = this.selectionChanged;
				if (action != null)
				{
					action(this.selectedItems);
				}
				Action<IEnumerable<int>> action2 = this.selectedIndicesChanged;
				if (action2 != null)
				{
					action2(this.m_Selection.indices);
				}
			}
		}

		public void ClearSelection()
		{
			bool flag = !this.HasValidDataAndBindings() || this.m_Selection.idCount == 0;
			if (!flag)
			{
				this.ClearSelectionWithoutValidation();
				this.NotifyOfSelectionChange();
			}
		}

		private void ClearSelectionWithoutValidation()
		{
			foreach (ReusableCollectionItem reusableCollectionItem in this.activeItems)
			{
				reusableCollectionItem.SetSelected(false);
			}
			this.m_Selection.Clear();
		}

		internal override void OnViewDataReady()
		{
			base.OnViewDataReady();
			string fullHierarchicalViewDataKey = base.GetFullHierarchicalViewDataKey();
			base.OverwriteFromViewData(this, fullHierarchicalViewDataKey);
			this.m_ScrollView.UpdateContentViewTransform();
		}

		[EventInterest(new Type[]
		{
			typeof(PointerUpEvent),
			typeof(FocusEvent),
			typeof(NavigationSubmitEvent),
			typeof(BlurEvent)
		})]
		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			bool flag = evt.eventTypeId == EventBase<PointerUpEvent>.TypeId();
			if (flag)
			{
				ListViewDragger dragger = this.m_Dragger;
				if (dragger != null)
				{
					dragger.OnPointerUpEvent((PointerUpEvent)evt);
				}
			}
			else
			{
				bool flag2 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
				if (flag2)
				{
					CollectionVirtualizationController virtualizationController = this.m_VirtualizationController;
					if (virtualizationController != null)
					{
						virtualizationController.OnFocus(evt.leafTarget as VisualElement);
					}
				}
				else
				{
					bool flag3 = evt.eventTypeId == EventBase<BlurEvent>.TypeId();
					if (flag3)
					{
						BlurEvent blurEvent = evt as BlurEvent;
						CollectionVirtualizationController virtualizationController2 = this.m_VirtualizationController;
						if (virtualizationController2 != null)
						{
							virtualizationController2.OnBlur(((blurEvent != null) ? blurEvent.relatedTarget : null) as VisualElement);
						}
					}
					else
					{
						bool flag4 = evt.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId();
						if (flag4)
						{
							bool flag5 = evt.target == this;
							if (flag5)
							{
								this.m_ScrollView.contentContainer.Focus();
							}
						}
					}
				}
			}
		}

		private void OnSizeChanged(GeometryChangedEvent evt)
		{
			bool flag = !this.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = Mathf.Approximately(evt.newRect.width, evt.oldRect.width) && Mathf.Approximately(evt.newRect.height, evt.oldRect.height);
				if (!flag2)
				{
					this.Resize(evt.newRect.size);
				}
			}
		}

		private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			int num;
			bool flag = !this.m_ItemHeightIsInline && e.customStyle.TryGetValue(BaseVerticalCollectionView.s_ItemHeightProperty, out num);
			if (flag)
			{
				bool flag2 = Math.Abs(this.m_FixedItemHeight - (float)num) > float.Epsilon;
				if (flag2)
				{
					this.m_FixedItemHeight = (float)num;
					this.RefreshItems();
				}
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.m_Selection.selectedIds = this.m_SelectedIds;
			this.RefreshItems();
		}

		[CompilerGenerated]
		private void <RefreshSelection>g__NotifyIfChanged|172_0(ref BaseVerticalCollectionView.<>c__DisplayClass172_0 A_1)
		{
			bool flag = A_1.selectedIndicesChanged || this.m_Selection.indexCount != A_1.previousSelectionCount;
			if (flag)
			{
				this.NotifyOfSelectionChange();
			}
		}

		[CompilerGenerated]
		private void <Apply>g__HandleSelectionAndScroll|183_0(int index, ref BaseVerticalCollectionView.<>c__DisplayClass183_0 A_2)
		{
			bool flag = index < 0 || index >= this.m_ViewController.itemsSource.Count;
			if (!flag)
			{
				bool flag2 = ((this.selectionType == SelectionType.Multiple) & A_2.shiftKey) && this.m_Selection.indexCount != 0;
				if (flag2)
				{
					this.DoRangeSelection(index);
				}
				else
				{
					this.selectedIndex = index;
				}
				this.ScrollToItem(index);
			}
		}

		private static readonly ProfilerMarker k_RefreshMarker = new ProfilerMarker("BaseVerticalCollectionView.RefreshItems");

		private static readonly ProfilerMarker k_RebuildMarker = new ProfilerMarker("BaseVerticalCollectionView.Rebuild");

		internal const string internalBindingKey = "__unity-collection-view-internal-binding";

		private SelectionType m_SelectionType;

		private static readonly List<ReusableCollectionItem> k_EmptyItems = new List<ReusableCollectionItem>();

		private bool m_HorizontalScrollingEnabled;

		[SerializeField]
		private AlternatingRowBackground m_ShowAlternatingRowBackgrounds = AlternatingRowBackground.None;

		internal static readonly int s_DefaultItemHeight = 22;

		internal float m_FixedItemHeight = (float)BaseVerticalCollectionView.s_DefaultItemHeight;

		internal bool m_ItemHeightIsInline;

		private CollectionVirtualizationMethod m_VirtualizationMethod;

		private readonly ScrollView m_ScrollView;

		private CollectionViewController m_ViewController;

		private CollectionVirtualizationController m_VirtualizationController;

		private KeyboardNavigationManipulator m_NavigationManipulator;

		[SerializeField]
		internal SerializedVirtualizationData serializedVirtualizationData = new SerializedVirtualizationData();

		[SerializeField]
		private readonly List<int> m_SelectedIds = new List<int>();

		private readonly BaseVerticalCollectionView.Selection m_Selection;

		private float m_LastHeight;

		private bool m_IsRangeSelectionDirectionUp;

		private ListViewDragger m_Dragger;

		internal const float ItemHeightUnset = -1f;

		internal static CustomStyleProperty<int> s_ItemHeightProperty = new CustomStyleProperty<int>("--unity-item-height");

		private Action<int, int> m_ItemIndexChangedCallback;

		private Action m_ItemsSourceChangedCallback;

		internal IVisualElementScheduledItem m_RebuildScheduled;

		public static readonly string ussClassName = "unity-collection-view";

		public static readonly string borderUssClassName = BaseVerticalCollectionView.ussClassName + "--with-border";

		public static readonly string itemUssClassName = BaseVerticalCollectionView.ussClassName + "__item";

		public static readonly string dragHoverBarUssClassName = BaseVerticalCollectionView.ussClassName + "__drag-hover-bar";

		public static readonly string dragHoverMarkerUssClassName = BaseVerticalCollectionView.ussClassName + "__drag-hover-marker";

		public static readonly string itemDragHoverUssClassName = BaseVerticalCollectionView.itemUssClassName + "--drag-hover";

		public static readonly string itemSelectedVariantUssClassName = BaseVerticalCollectionView.itemUssClassName + "--selected";

		public static readonly string itemAlternativeBackgroundUssClassName = BaseVerticalCollectionView.itemUssClassName + "--alternative-background";

		public static readonly string listScrollViewUssClassName = BaseVerticalCollectionView.ussClassName + "__scroll-view";

		internal static readonly string backgroundFillUssClassName = BaseVerticalCollectionView.ussClassName + "__background-fill";

		private Vector3 m_TouchDownPosition;

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public UxmlTraits()
			{
				base.focusable.defaultValue = true;
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				int num = 0;
				BaseVerticalCollectionView baseVerticalCollectionView = (BaseVerticalCollectionView)ve;
				baseVerticalCollectionView.reorderable = this.m_Reorderable.GetValueFromBag(bag, cc);
				bool flag = this.m_FixedItemHeight.TryGetValueFromBag(bag, cc, ref num);
				if (flag)
				{
					baseVerticalCollectionView.fixedItemHeight = (float)num;
				}
				baseVerticalCollectionView.virtualizationMethod = this.m_VirtualizationMethod.GetValueFromBag(bag, cc);
				baseVerticalCollectionView.showBorder = this.m_ShowBorder.GetValueFromBag(bag, cc);
				baseVerticalCollectionView.selectionType = this.m_SelectionType.GetValueFromBag(bag, cc);
				baseVerticalCollectionView.showAlternatingRowBackgrounds = this.m_ShowAlternatingRowBackgrounds.GetValueFromBag(bag, cc);
				baseVerticalCollectionView.horizontalScrollingEnabled = this.m_HorizontalScrollingEnabled.GetValueFromBag(bag, cc);
			}

			private readonly UxmlIntAttributeDescription m_FixedItemHeight = new UxmlIntAttributeDescription
			{
				name = "fixed-item-height",
				obsoleteNames = new string[] { "itemHeight, item-height" },
				defaultValue = BaseVerticalCollectionView.s_DefaultItemHeight
			};

			private readonly UxmlEnumAttributeDescription<CollectionVirtualizationMethod> m_VirtualizationMethod = new UxmlEnumAttributeDescription<CollectionVirtualizationMethod>
			{
				name = "virtualization-method",
				defaultValue = CollectionVirtualizationMethod.FixedHeight
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

			private readonly UxmlBoolAttributeDescription m_HorizontalScrollingEnabled = new UxmlBoolAttributeDescription
			{
				name = "horizontal-scrolling",
				defaultValue = false
			};
		}

		private class Selection
		{
			public List<int> selectedIds { get; set; }

			public int indexCount
			{
				get
				{
					return this.indices.Count;
				}
			}

			public int idCount
			{
				get
				{
					return this.selectedIds.Count;
				}
			}

			public int minIndex
			{
				get
				{
					bool flag = this.m_MinIndex == -1;
					if (flag)
					{
						this.m_MinIndex = this.indices.Min();
					}
					return this.m_MinIndex;
				}
			}

			public int maxIndex
			{
				get
				{
					bool flag = this.m_MaxIndex == -1;
					if (flag)
					{
						this.m_MaxIndex = this.indices.Max();
					}
					return this.m_MaxIndex;
				}
			}

			public int capacity
			{
				get
				{
					return this.indices.Capacity;
				}
				set
				{
					this.indices.Capacity = value;
					bool flag = this.selectedIds.Capacity < value;
					if (flag)
					{
						this.selectedIds.Capacity = value;
					}
				}
			}

			public int FirstIndex()
			{
				return (this.indices.Count > 0) ? this.indices[0] : (-1);
			}

			public object FirstObject()
			{
				object obj;
				return this.items.TryGetValue(this.FirstIndex(), out obj) ? obj : null;
			}

			public bool ContainsIndex(int index)
			{
				return this.m_IndexLookup.Contains(index);
			}

			public bool ContainsId(int id)
			{
				return this.m_IdLookup.Contains(id);
			}

			public void AddId(int id)
			{
				this.selectedIds.Add(id);
				this.m_IdLookup.Add(id);
			}

			public void AddIndex(int index, object obj)
			{
				this.m_IndexLookup.Add(index);
				this.indices.Add(index);
				this.items[index] = obj;
				bool flag = index < this.m_MinIndex;
				if (flag)
				{
					this.m_MinIndex = index;
				}
				bool flag2 = index > this.m_MaxIndex;
				if (flag2)
				{
					this.m_MaxIndex = index;
				}
			}

			public bool TryRemove(int index)
			{
				bool flag = !this.m_IndexLookup.Remove(index);
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					int num = this.indices.IndexOf(index);
					bool flag3 = num >= 0;
					if (flag3)
					{
						this.indices.RemoveAt(num);
						this.items.Remove(index);
						bool flag4 = index == this.m_MinIndex;
						if (flag4)
						{
							this.m_MinIndex = -1;
						}
						bool flag5 = index == this.m_MaxIndex;
						if (flag5)
						{
							this.m_MaxIndex = -1;
						}
					}
					flag2 = true;
				}
				return flag2;
			}

			public void RemoveId(int id)
			{
				this.selectedIds.Remove(id);
				this.m_IdLookup.Remove(id);
			}

			public void ClearItems()
			{
				this.items.Clear();
			}

			public void ClearIds()
			{
				this.m_IdLookup.Clear();
				this.selectedIds.Clear();
			}

			public void ClearIndices()
			{
				this.m_IndexLookup.Clear();
				this.indices.Clear();
				this.m_MinIndex = -1;
				this.m_MaxIndex = -1;
			}

			public void Clear()
			{
				this.ClearItems();
				this.ClearIds();
				this.ClearIndices();
			}

			private readonly HashSet<int> m_IndexLookup = new HashSet<int>();

			private readonly HashSet<int> m_IdLookup = new HashSet<int>();

			private int m_MinIndex = -1;

			private int m_MaxIndex = -1;

			public readonly List<int> indices = new List<int>();

			public readonly Dictionary<int, object> items = new Dictionary<int, object>();
		}
	}
}
