using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal abstract class VerticalVirtualizationController<T> : CollectionVirtualizationController where T : ReusableCollectionItem, new()
	{
		public override IEnumerable<ReusableCollectionItem> activeItems
		{
			get
			{
				return this.m_ActiveItems;
			}
		}

		internal int itemsCount
		{
			get
			{
				CollectionViewController viewController = this.m_CollectionView.viewController;
				return (viewController != null) ? viewController.GetItemsCount() : this.m_CollectionView.itemsSource.Count;
			}
		}

		protected virtual bool VisibleItemPredicate(T i)
		{
			return i.rootElement.style.display == DisplayStyle.Flex;
		}

		internal T firstVisibleItem
		{
			get
			{
				foreach (T t in this.m_ActiveItems)
				{
					bool flag = this.m_VisibleItemPredicateDelegate(t);
					if (flag)
					{
						return t;
					}
				}
				return default(T);
			}
		}

		internal T lastVisibleItem
		{
			get
			{
				int i = this.m_ActiveItems.Count;
				while (i > 0)
				{
					T t = this.m_ActiveItems[--i];
					bool flag = this.m_VisibleItemPredicateDelegate(t);
					if (flag)
					{
						return t;
					}
				}
				return default(T);
			}
		}

		public override int visibleItemCount
		{
			get
			{
				int num = 0;
				foreach (T t in this.m_ActiveItems)
				{
					bool flag = this.m_VisibleItemPredicateDelegate(t);
					if (flag)
					{
						num++;
					}
				}
				return num;
			}
		}

		protected SerializedVirtualizationData serializedData
		{
			get
			{
				return this.m_CollectionView.serializedVirtualizationData;
			}
		}

		public override int firstVisibleIndex
		{
			get
			{
				return Mathf.Min(this.serializedData.firstVisibleIndex, (this.m_CollectionView.viewController != null) ? (this.m_CollectionView.viewController.GetItemsCount() - 1) : this.serializedData.firstVisibleIndex);
			}
			protected set
			{
				this.serializedData.firstVisibleIndex = value;
			}
		}

		protected float lastHeight
		{
			get
			{
				return this.m_CollectionView.lastHeight;
			}
		}

		protected virtual bool alwaysRebindOnRefresh
		{
			get
			{
				return true;
			}
		}

		protected VerticalVirtualizationController(BaseVerticalCollectionView collectionView)
			: base(collectionView.scrollView)
		{
			this.m_CollectionView = collectionView;
			this.m_ActiveItems = new List<T>();
			this.m_VisibleItemPredicateDelegate = new Func<T, bool>(this.VisibleItemPredicate);
			this.m_PerformDeferredScrollToItem = new Action(this.PerformDeferredScrollToItem);
			this.m_ScrollCallback = new Action(this.OnScrollUpdate);
			this.m_ScrollView.contentContainer.disableClipping = false;
			collectionView.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanelEvent), TrickleDown.NoTrickleDown);
		}

		public override void Refresh(bool rebuild)
		{
			bool flag = this.m_CollectionView.HasValidDataAndBindings();
			BaseVerticalCollectionView collectionView = this.m_CollectionView;
			IList itemsSource = this.m_CollectionView.itemsSource;
			collectionView.m_PreviousRefreshedCount = ((itemsSource != null) ? itemsSource.Count : 0);
			for (int i = 0; i < this.m_ActiveItems.Count; i++)
			{
				int num = this.firstVisibleIndex + i;
				T t = this.m_ActiveItems[i];
				bool flag2 = t.rootElement.style.display == DisplayStyle.Flex;
				if (rebuild)
				{
					bool flag3 = flag && t.index != -1;
					if (flag3)
					{
						this.m_CollectionView.viewController.InvokeUnbindItem(t, t.index);
					}
					this.m_Pool.Release(t);
				}
				else
				{
					bool flag4 = this.m_CollectionView.itemsSource != null && num >= 0 && num < this.itemsCount;
					if (flag4)
					{
						bool flag5 = !flag;
						if (!flag5)
						{
							bool flag6 = t.index != -1;
							if (flag6)
							{
								this.m_CollectionView.viewController.InvokeUnbindItem(t, t.index);
							}
							bool flag7 = flag2 || this.alwaysRebindOnRefresh;
							if (flag7)
							{
								this.Setup(t, num);
							}
						}
					}
					else
					{
						this.ReleaseItem(i--);
					}
				}
			}
			if (rebuild)
			{
				this.m_Pool.Clear();
				this.m_ActiveItems.Clear();
				this.m_ScrollView.Clear();
			}
		}

		public override void UnbindAll()
		{
			bool flag = this.m_CollectionView.HasValidDataAndBindings();
			bool flag2 = !flag;
			if (!flag2)
			{
				foreach (T t in this.m_ActiveItems)
				{
					this.m_CollectionView.viewController.InvokeUnbindItem(t, t.index);
				}
			}
		}

		protected void Setup(T recycledItem, int newIndex)
		{
			bool isDragGhost = recycledItem.isDragGhost;
			bool flag = this.GetDraggedIndex() == newIndex;
			if (flag)
			{
				bool flag2 = recycledItem.index != -1;
				if (flag2)
				{
					this.m_CollectionView.viewController.InvokeUnbindItem(recycledItem, recycledItem.index);
				}
				recycledItem.SetDragGhost(true);
				recycledItem.index = this.m_DraggedItem.index;
				recycledItem.rootElement.style.display = DisplayStyle.Flex;
				this.m_CollectionView.viewController.SetBindingContext(recycledItem, recycledItem.index);
			}
			else
			{
				bool flag3 = isDragGhost;
				if (flag3)
				{
					recycledItem.SetDragGhost(false);
				}
				bool flag4 = newIndex >= this.itemsCount;
				if (flag4)
				{
					recycledItem.rootElement.style.display = DisplayStyle.None;
					bool flag5 = recycledItem.index >= 0 && recycledItem.index < this.itemsCount;
					if (flag5)
					{
						this.m_CollectionView.viewController.InvokeUnbindItem(recycledItem, recycledItem.index);
						recycledItem.index = -1;
					}
				}
				else
				{
					recycledItem.rootElement.style.display = DisplayStyle.Flex;
					int idForIndex = this.m_CollectionView.viewController.GetIdForIndex(newIndex);
					bool flag6 = recycledItem.index == newIndex && recycledItem.id == idForIndex;
					if (!flag6)
					{
						bool flag7 = this.m_CollectionView.showAlternatingRowBackgrounds != AlternatingRowBackground.None && newIndex % 2 == 1;
						recycledItem.rootElement.EnableInClassList(BaseVerticalCollectionView.itemAlternativeBackgroundUssClassName, flag7);
						int index = recycledItem.index;
						bool flag8 = recycledItem.index != -1;
						if (flag8)
						{
							this.m_CollectionView.viewController.InvokeUnbindItem(recycledItem, recycledItem.index);
						}
						recycledItem.index = newIndex;
						recycledItem.id = idForIndex;
						int num = newIndex - this.firstVisibleIndex;
						bool flag9 = num >= this.m_ScrollView.contentContainer.childCount;
						if (flag9)
						{
							recycledItem.rootElement.BringToFront();
						}
						else
						{
							bool flag10 = num >= 0;
							if (flag10)
							{
								recycledItem.rootElement.PlaceBehind(this.m_ScrollView.contentContainer[num]);
							}
							else
							{
								recycledItem.rootElement.SendToBack();
							}
						}
						this.m_CollectionView.viewController.InvokeBindItem(recycledItem, newIndex);
						this.HandleFocus(recycledItem, index);
					}
				}
			}
		}

		private bool IsContentContainerPanelDirtied()
		{
			IPanel panel = this.m_ScrollView.contentContainer.panel;
			return panel != null && panel.isDirty;
		}

		private bool ShouldStopDeferredScrollTo()
		{
			return !this.IsContentContainerPanelDirtied();
		}

		protected bool ShouldDeferScrollToItem(int index)
		{
			IVisualElementScheduledItem scheduleDeferredScrollToItem = this.m_ScheduleDeferredScrollToItem;
			bool flag = scheduleDeferredScrollToItem != null && scheduleDeferredScrollToItem.isActive;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.IsContentContainerPanelDirtied();
				if (flag3)
				{
					this.m_DeferredScrollToItemIndex = new int?(index);
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		protected void ScheduleDeferredScrollToItem()
		{
			bool flag = this.m_DeferredScrollToItemIndex == null;
			if (!flag)
			{
				bool flag2 = this.m_ScheduleDeferredScrollToItem == null;
				if (flag2)
				{
					this.m_ScheduleDeferredScrollToItem = this.m_CollectionView.schedule.Execute(this.m_PerformDeferredScrollToItem).Until(new Func<bool>(this.ShouldStopDeferredScrollTo));
				}
				else
				{
					bool flag3 = !this.m_ScheduleDeferredScrollToItem.isActive;
					if (flag3)
					{
						this.m_ScheduleDeferredScrollToItem.Resume();
					}
				}
			}
		}

		private void PerformDeferredScrollToItem()
		{
			bool flag = this.m_DeferredScrollToItemIndex != null;
			if (flag)
			{
				this.ScrollToItem(this.m_DeferredScrollToItemIndex.Value);
			}
			else
			{
				this.m_ScheduleDeferredScrollToItem.Pause();
			}
		}

		protected void StopDeferredScrollToItem()
		{
			this.m_DeferredScrollToItemIndex = null;
			bool flag = this.m_ScheduleDeferredScrollToItem == null;
			if (!flag)
			{
				bool isActive = this.m_ScheduleDeferredScrollToItem.isActive;
				if (isActive)
				{
					this.m_ScheduleDeferredScrollToItem.Pause();
				}
			}
		}

		private void OnDetachFromPanelEvent(DetachFromPanelEvent evt)
		{
			IVisualElementScheduledItem scrollScheduledItem = this.m_ScrollScheduledItem;
			bool flag = scrollScheduledItem != null && scrollScheduledItem.isActive;
			if (flag)
			{
				this.m_ScrollScheduledItem.Pause();
				this.m_ScrollScheduledItem = null;
			}
		}

		public override void OnFocusIn(VisualElement leafTarget)
		{
			bool flag = leafTarget == this.m_ScrollView.contentContainer;
			if (!flag)
			{
				this.m_LastFocusedElementTreeChildIndexes.Clear();
				bool flag2 = this.m_ScrollView.contentContainer.FindElementInTree(leafTarget, this.m_LastFocusedElementTreeChildIndexes);
				if (flag2)
				{
					VisualElement visualElement = this.m_ScrollView.contentContainer[this.m_LastFocusedElementTreeChildIndexes[0]];
					foreach (ReusableCollectionItem reusableCollectionItem in this.activeItems)
					{
						bool flag3 = reusableCollectionItem.rootElement == visualElement;
						if (flag3)
						{
							this.m_LastFocusedElementIndex = reusableCollectionItem.index;
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

		public override void OnFocusOut(VisualElement willFocus)
		{
			bool flag = willFocus == null || willFocus != this.m_ScrollView.contentContainer;
			if (flag)
			{
				this.m_LastFocusedElementTreeChildIndexes.Clear();
				this.m_LastFocusedElementIndex = -1;
			}
		}

		private void HandleFocus(ReusableCollectionItem recycledItem, int previousIndex)
		{
			bool flag = this.m_LastFocusedElementIndex == -1;
			if (!flag)
			{
				bool flag2 = this.m_LastFocusedElementIndex == recycledItem.index;
				if (flag2)
				{
					VisualElement visualElement = recycledItem.rootElement.ElementAtTreePath(this.m_LastFocusedElementTreeChildIndexes);
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
						VisualElement visualElement2 = recycledItem.rootElement.ElementAtTreePath(this.m_LastFocusedElementTreeChildIndexes);
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

		public override void UpdateBackground()
		{
			float num;
			bool flag = this.m_CollectionView.showAlternatingRowBackgrounds != AlternatingRowBackground.All || (num = this.m_ScrollView.contentViewport.resolvedStyle.height - this.GetExpectedContentHeight()) <= 0f;
			if (flag)
			{
				VisualElement emptyRows = this.m_EmptyRows;
				if (emptyRows != null)
				{
					emptyRows.RemoveFromHierarchy();
				}
			}
			else
			{
				bool flag2 = this.lastVisibleItem == null;
				if (!flag2)
				{
					bool flag3 = this.m_EmptyRows == null;
					if (flag3)
					{
						this.m_EmptyRows = new VisualElement
						{
							classList = { BaseVerticalCollectionView.backgroundFillUssClassName }
						};
					}
					bool flag4 = this.m_EmptyRows.parent == null;
					if (flag4)
					{
						this.m_ScrollView.contentViewport.Add(this.m_EmptyRows);
					}
					float expectedItemHeight = this.GetExpectedItemHeight(-1);
					int num2 = Mathf.FloorToInt(num / expectedItemHeight) + 1;
					bool flag5 = num2 > this.m_EmptyRows.childCount;
					if (flag5)
					{
						int num3 = num2 - this.m_EmptyRows.childCount;
						for (int i = 0; i < num3; i++)
						{
							VisualElement visualElement = new VisualElement();
							visualElement.style.flexShrink = 0f;
							this.m_EmptyRows.Add(visualElement);
						}
					}
					T t = this.lastVisibleItem;
					int num4 = ((t != null) ? t.index : (-1));
					int childCount = this.m_EmptyRows.hierarchy.childCount;
					for (int j = 0; j < childCount; j++)
					{
						VisualElement visualElement2 = this.m_EmptyRows.hierarchy[j];
						num4++;
						visualElement2.style.height = expectedItemHeight;
						visualElement2.EnableInClassList(BaseVerticalCollectionView.itemAlternativeBackgroundUssClassName, num4 % 2 == 1);
					}
				}
			}
		}

		internal override void StartDragItem(ReusableCollectionItem item)
		{
			this.m_DraggedItem = item as T;
			int num = this.m_ActiveItems.IndexOf(this.m_DraggedItem);
			this.m_ActiveItems.RemoveAt(num);
			T orMakeItemAtIndex = this.GetOrMakeItemAtIndex(num, num);
			this.Setup(orMakeItemAtIndex, this.m_DraggedItem.index);
		}

		internal override void EndDrag(int dropIndex)
		{
			ReusableCollectionItem recycledItemFromIndex = this.m_CollectionView.GetRecycledItemFromIndex(dropIndex);
			int num = ((recycledItemFromIndex != null) ? this.m_ScrollView.IndexOf(recycledItemFromIndex.rootElement) : this.m_ActiveItems.Count);
			this.m_ScrollView.Insert(num, this.m_DraggedItem.rootElement);
			this.m_ActiveItems.Insert(num, this.m_DraggedItem);
			for (int i = 0; i < this.m_ActiveItems.Count; i++)
			{
				T t = this.m_ActiveItems[i];
				bool isDragGhost = t.isDragGhost;
				if (isDragGhost)
				{
					t.index = -1;
					this.ReleaseItem(i);
					i--;
				}
			}
			bool flag = Math.Min(dropIndex, this.itemsCount - 1) != this.m_DraggedItem.index;
			if (flag)
			{
				bool flag2 = this.lastVisibleItem != null;
				if (flag2)
				{
					this.lastVisibleItem.rootElement.style.display = DisplayStyle.None;
				}
				bool flag3 = this.m_DraggedItem.index < dropIndex;
				if (flag3)
				{
					this.m_CollectionView.viewController.InvokeUnbindItem(this.m_DraggedItem, this.m_DraggedItem.index);
					this.m_DraggedItem.index = -1;
				}
				else
				{
					bool flag4 = recycledItemFromIndex != null;
					if (flag4)
					{
						this.m_CollectionView.viewController.InvokeUnbindItem(recycledItemFromIndex, recycledItemFromIndex.index);
						recycledItemFromIndex.index = -1;
					}
				}
			}
			this.m_DraggedItem = default(T);
		}

		internal virtual T GetOrMakeItemAtIndex(int activeItemIndex = -1, int scrollViewIndex = -1)
		{
			T t = this.m_Pool.Get();
			bool flag = t.rootElement == null;
			if (flag)
			{
				this.m_CollectionView.viewController.InvokeMakeItem(t);
				t.onDestroy += this.OnDestroyItem;
			}
			t.PreAttachElement();
			bool flag2 = activeItemIndex == -1;
			if (flag2)
			{
				this.m_ActiveItems.Add(t);
			}
			else
			{
				this.m_ActiveItems.Insert(activeItemIndex, t);
			}
			bool flag3 = scrollViewIndex == -1;
			if (flag3)
			{
				this.m_ScrollView.Add(t.rootElement);
			}
			else
			{
				this.m_ScrollView.Insert(scrollViewIndex, t.rootElement);
			}
			return t;
		}

		internal virtual void ReleaseItem(int activeItemsIndex)
		{
			T t = this.m_ActiveItems[activeItemsIndex];
			bool flag = t.index != -1;
			if (flag)
			{
				this.m_CollectionView.viewController.InvokeUnbindItem(t, t.index);
			}
			this.m_Pool.Release(t);
			this.m_ActiveItems.Remove(t);
		}

		private void OnDestroyItem(ReusableCollectionItem item)
		{
			this.m_CollectionView.viewController.InvokeDestroyItem(item);
			item.onDestroy -= this.OnDestroyItem;
		}

		protected virtual void OnScrollUpdate()
		{
		}

		protected int GetDraggedIndex()
		{
			ListViewDraggerAnimated listViewDraggerAnimated = this.m_CollectionView.dragger as ListViewDraggerAnimated;
			bool flag = listViewDraggerAnimated != null && listViewDraggerAnimated.isDragging;
			int num;
			if (flag)
			{
				num = listViewDraggerAnimated.draggedItem.index;
			}
			else
			{
				num = -1;
			}
			return num;
		}

		protected void ScheduleScroll()
		{
			bool flag = this.m_ScrollScheduledItem == null;
			if (flag)
			{
				this.m_ScrollScheduledItem = this.m_CollectionView.schedule.Execute(this.m_ScrollCallback);
			}
			else
			{
				this.m_ScrollScheduledItem.Pause();
				this.m_ScrollScheduledItem.Resume();
			}
		}

		private readonly ObjectPool<T> m_Pool = new ObjectPool<T>(() => new T(), null, delegate(T i)
		{
			i.DetachElement();
		}, delegate(T i)
		{
			i.DestroyElement();
		}, true, 10, 10000);

		protected BaseVerticalCollectionView m_CollectionView;

		protected const int k_ExtraVisibleItems = 2;

		protected List<T> m_ActiveItems;

		protected T m_DraggedItem;

		private int? m_DeferredScrollToItemIndex;

		private readonly Action m_PerformDeferredScrollToItem;

		private IVisualElementScheduledItem m_ScheduleDeferredScrollToItem;

		private IVisualElementScheduledItem m_ScrollScheduledItem;

		private Action m_ScrollCallback;

		private int m_LastFocusedElementIndex = -1;

		private List<int> m_LastFocusedElementTreeChildIndexes = new List<int>();

		protected readonly Func<T, bool> m_VisibleItemPredicateDelegate;

		protected List<T> m_ScrollInsertionList = new List<T>();

		private VisualElement m_EmptyRows;
	}
}
