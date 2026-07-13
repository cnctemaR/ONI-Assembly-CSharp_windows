using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class CollectionViewDragger : DragEventsProcessor
	{
		private CollectionView targetView
		{
			get
			{
				return this.m_Target as CollectionView;
			}
		}

		private ScrollContainer targetScrollView
		{
			get
			{
				return this.targetView.scrollView;
			}
		}

		private bool enabled { get; set; } = true;

		public ICollectionDragAndDropController dragAndDropController { get; set; }

		public CollectionViewDragger(CollectionView listView)
			: base(listView)
		{
		}

		protected override bool CanStartDrag(Vector3 pointerPosition, EventModifiers modifiers)
		{
			bool flag = this.dragAndDropController == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = !this.targetScrollView.contentContainer.worldBound.Contains(pointerPosition);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					RecycledItem recycledItem = this.GetRecycledItem(pointerPosition);
					bool flag4 = recycledItem != null && this.targetView.HasCanStartDrag();
					if (flag4)
					{
						IEnumerable<int> enumerable2;
						if (!this.targetView.hasSelection)
						{
							IEnumerable<int> enumerable = new int[] { recycledItem.index };
							enumerable2 = enumerable;
						}
						else
						{
							enumerable2 = this.targetView.selectedIndices;
						}
						IEnumerable<int> enumerable3 = enumerable2;
						flag2 = this.targetView.RaiseCanStartDrag(recycledItem, enumerable3, modifiers);
					}
					else
					{
						bool hasSelection = this.targetView.hasSelection;
						if (hasSelection)
						{
							flag2 = this.dragAndDropController.CanStartDrag(this.targetView.selectedIndices);
						}
						else
						{
							flag2 = recycledItem != null && this.dragAndDropController.CanStartDrag(new int[] { recycledItem.index });
						}
					}
				}
			}
			return flag2;
		}

		protected internal override StartDragArgs StartDrag(Vector3 pointerPosition, EventModifiers modifiers)
		{
			RecycledItem recycledItem = this.GetRecycledItem(pointerPosition);
			bool flag = recycledItem != null;
			IEnumerable<int> enumerable;
			if (flag)
			{
				bool flag2 = this.targetView.selectionType == SelectionType.None;
				if (flag2)
				{
					enumerable = new int[] { recycledItem.index };
				}
				else
				{
					bool flag3 = !this.targetView.IsSelected(recycledItem.index);
					if (flag3)
					{
						this.targetView.SetSelection(recycledItem.index);
					}
					enumerable = this.targetView.selectedIndices;
				}
			}
			else
			{
				IEnumerable<int> enumerable3;
				if (!this.targetView.hasSelection)
				{
					IEnumerable<int> enumerable2 = Array.Empty<int>();
					enumerable3 = enumerable2;
				}
				else
				{
					enumerable3 = this.targetView.selectedIndices;
				}
				enumerable = enumerable3;
			}
			StartDragArgs startDragArgs = this.dragAndDropController.SetupDragAndDrop(enumerable, false);
			startDragArgs.modifiers = modifiers;
			startDragArgs = this.targetView.RaiseSetupDragAndDrop(recycledItem, this.dragAndDropController.GetSortedSelectedIds(), startDragArgs);
			startDragArgs.SetGenericData("__unity-drag-and-drop__source-view", this.targetView);
			return startDragArgs;
		}

		protected internal override void UpdateDrag(Vector3 pointerPosition, EventModifiers modifiers)
		{
			CollectionViewDragger.DragPosition dragPosition = default(CollectionViewDragger.DragPosition);
			DragVisualMode visualMode = this.GetVisualMode(pointerPosition, modifiers, ref dragPosition);
			bool flag = visualMode == DragVisualMode.Rejected;
			if (flag)
			{
				this.ClearDragAndDropUI(false);
			}
			else
			{
				this.HandleDragAndScroll(pointerPosition);
				this.ApplyDragAndDropUI(dragPosition);
			}
			base.dragAndDrop.SetVisualMode(visualMode);
			base.dragAndDrop.UpdateDrag(pointerPosition);
		}

		protected internal override void OnDrop(Vector3 pointerPosition, EventModifiers modifiers)
		{
			CollectionViewDragger.DragPosition dragPosition = default(CollectionViewDragger.DragPosition);
			bool flag = !this.TryGetDragPosition(pointerPosition, ref dragPosition);
			if (!flag)
			{
				DragAndDropArgs dragAndDropArgs = this.MakeDragAndDropArgs(dragPosition, modifiers);
				DragVisualMode dragVisualMode = this.targetView.RaiseDrop(pointerPosition, dragAndDropArgs);
				bool flag2 = dragVisualMode > DragVisualMode.None;
				if (flag2)
				{
					bool flag3 = dragVisualMode != DragVisualMode.Rejected;
					if (flag3)
					{
						base.dragAndDrop.AcceptDrag();
					}
					else
					{
						this.dragAndDropController.DragCleanup();
					}
				}
				else
				{
					bool flag4 = this.IsDraggingDisabled();
					if (!flag4)
					{
						bool flag5 = this.dragAndDropController.HandleDragAndDrop(dragAndDropArgs) != DragVisualMode.Rejected;
						if (flag5)
						{
							this.dragAndDropController.OnDrop(dragAndDropArgs);
							base.dragAndDrop.AcceptDrag();
						}
						else
						{
							this.dragAndDropController.DragCleanup();
						}
					}
				}
			}
		}

		protected override void ClearDragAndDropUI(bool dragCancelled)
		{
			if (dragCancelled)
			{
				this.dragAndDropController.DragCleanup();
			}
			this.targetView.elementPanel.cursorManager.ResetCursor();
			this.m_LastDragPosition = default(CollectionViewDragger.DragPosition);
			foreach (RecycledItem recycledItem in this.targetView.m_IndexToItemDictionary.Values)
			{
				recycledItem.element.RemoveFromClassList(BaseVerticalCollectionView.itemDragHoverUssClassName);
			}
			bool flag = this.m_DragHoverBar != null;
			if (flag)
			{
				this.m_DragHoverBar.style.visibility = Visibility.Hidden;
			}
		}

		internal void HandleDragAndScroll(Vector2 pointerPosition)
		{
			double num = 0.0;
			bool flag = pointerPosition.y < this.targetScrollView.worldBound.yMin + 10f;
			if (flag)
			{
				num = this.targetScrollView.verticalScroller.value + -20.0;
				bool flag2 = num <= (double)this.targetScrollView.worldBound.yMin;
				if (flag2)
				{
					num = 0.0;
				}
			}
			else
			{
				bool flag3 = pointerPosition.y > this.targetScrollView.worldBound.yMax - 10f;
				if (!flag3)
				{
					return;
				}
				float num2 = this.targetView.averageItemHeight * (float)this.targetView.itemsSource.Count;
				float height = this.targetScrollView.contentContainer.resolvedStyle.height;
				bool flag4 = num2 > height;
				if (flag4)
				{
					double value = this.targetScrollView.verticalScroller.value;
					num = ((value + 20.0 > (double)(num2 - height)) ? value : (value + 20.0));
				}
			}
			this.targetView.UpdateVerticalScrollValue(num);
		}

		private DragVisualMode GetVisualMode(Vector3 pointerPosition, EventModifiers modifiers, ref CollectionViewDragger.DragPosition dragPosition)
		{
			bool flag = this.dragAndDropController == null || !this.dragAndDropController.CanDrop();
			DragVisualMode dragVisualMode;
			if (flag)
			{
				dragVisualMode = DragVisualMode.Rejected;
			}
			else
			{
				bool flag2 = this.TryGetDragPosition(pointerPosition, ref dragPosition);
				DragAndDropArgs dragAndDropArgs = this.MakeDragAndDropArgs(dragPosition, modifiers);
				DragVisualMode dragVisualMode2 = this.targetView.RaiseHandleDragAndDrop(pointerPosition, dragAndDropArgs);
				bool flag3 = dragVisualMode2 > DragVisualMode.None;
				if (flag3)
				{
					dragVisualMode = dragVisualMode2;
				}
				else
				{
					dragVisualMode = (flag2 ? this.dragAndDropController.HandleDragAndDrop(dragAndDropArgs) : DragVisualMode.Rejected);
				}
			}
			return dragVisualMode;
		}

		private VisualElement CreateDragHoverBar()
		{
			VisualElement visualElement = new VisualElement
			{
				pickingMode = PickingMode.Ignore,
				style = 
				{
					width = this.targetView.localBound.width,
					visibility = Visibility.Hidden
				}
			};
			visualElement.AddToClassList(BaseVerticalCollectionView.dragHoverBarUssClassName);
			this.targetView.RegisterCallback<GeometryChangedEvent>(delegate(GeometryChangedEvent _)
			{
				this.m_DragHoverBar.style.width = this.targetView.localBound.width;
			}, TrickleDown.NoTrickleDown);
			return visualElement;
		}

		private void ApplyDragAndDropUI(CollectionViewDragger.DragPosition dragPosition)
		{
			bool flag = this.m_LastDragPosition.Equals(dragPosition) || this.IsDraggingDisabled();
			if (!flag)
			{
				if (this.m_DragHoverBar == null)
				{
					this.m_DragHoverBar = this.CreateDragHoverBar();
				}
				this.targetScrollView.viewport.Add(this.m_DragHoverBar);
				this.ClearDragAndDropUI(false);
				this.m_LastDragPosition = dragPosition;
				switch (dragPosition.dropPosition)
				{
				case DragAndDropPosition.OverItem:
					dragPosition.recycledItem.element.AddToClassList(BaseVerticalCollectionView.itemDragHoverUssClassName);
					break;
				case DragAndDropPosition.BetweenItems:
				{
					bool flag2 = dragPosition.insertAtIndex == 0;
					if (flag2)
					{
						this.PlaceHoverBarAt(0f);
					}
					else
					{
						VisualElement rootElementForIndex = this.targetView.GetRootElementForIndex(dragPosition.insertAtIndex - 1);
						this.PlaceHoverBarAtElement(rootElementForIndex ?? this.targetView.GetRootElementForIndex(dragPosition.insertAtIndex));
					}
					break;
				}
				case DragAndDropPosition.OutsideItems:
				{
					VisualElement rootElementForIndex2 = this.targetView.GetRootElementForIndex(this.targetView.itemsSource.Count - 1);
					bool flag3 = rootElementForIndex2 != null;
					if (flag3)
					{
						this.PlaceHoverBarAtElement(rootElementForIndex2);
					}
					else
					{
						this.PlaceHoverBarAt(0f);
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("dropPosition", dragPosition.dropPosition, "Unsupported dropPosition value");
				}
			}
		}

		private bool TryGetDragPosition(Vector2 pointerPosition, ref CollectionViewDragger.DragPosition dragPosition)
		{
			RecycledItem recycledItem = this.GetRecycledItem(pointerPosition);
			bool flag = recycledItem == null;
			bool flag3;
			if (flag)
			{
				bool flag2 = !this.targetView.worldBound.Contains(pointerPosition);
				if (flag2)
				{
					flag3 = false;
				}
				else
				{
					dragPosition.dropPosition = DragAndDropPosition.OutsideItems;
					bool flag4 = pointerPosition.y <= this.targetScrollView.contentContainer.worldBound.yMin;
					if (flag4)
					{
						dragPosition.insertAtIndex = 0;
					}
					else
					{
						dragPosition.insertAtIndex = this.targetView.itemsSource.Count;
					}
					flag3 = true;
				}
			}
			else
			{
				bool flag5 = recycledItem.element.worldBound.yMax - pointerPosition.y < 5f;
				if (flag5)
				{
					dragPosition.insertAtIndex = recycledItem.index + 1;
					dragPosition.dropPosition = DragAndDropPosition.BetweenItems;
				}
				else
				{
					bool flag6 = pointerPosition.y - recycledItem.element.worldBound.yMin > 5f;
					if (flag6)
					{
						Vector2 containerOffset = this.targetScrollView.containerOffset;
						this.targetView.ScrollToItem(recycledItem.index);
						bool flag7 = !Mathf.Approximately(containerOffset.x, this.targetScrollView.containerOffset.x) || !Mathf.Approximately(containerOffset.y, this.targetScrollView.containerOffset.y);
						if (flag7)
						{
							return this.TryGetDragPosition(pointerPosition, ref dragPosition);
						}
						dragPosition.recycledItem = recycledItem;
						dragPosition.insertAtIndex = recycledItem.index;
						dragPosition.dropPosition = DragAndDropPosition.OverItem;
					}
					else
					{
						dragPosition.insertAtIndex = recycledItem.index;
						dragPosition.dropPosition = DragAndDropPosition.BetweenItems;
					}
				}
				flag3 = true;
			}
			return flag3;
		}

		private DragAndDropArgs MakeDragAndDropArgs(CollectionViewDragger.DragPosition dragPosition, EventModifiers modifiers)
		{
			object obj = null;
			RecycledItem recycledItem = dragPosition.recycledItem;
			bool flag = recycledItem != null;
			if (flag)
			{
				obj = this.targetView.itemsSource[recycledItem.index];
			}
			return new DragAndDropArgs
			{
				target = obj,
				insertAtIndex = dragPosition.insertAtIndex,
				dragAndDropPosition = dragPosition.dropPosition,
				dragAndDropData = DragAndDropUtility.GetDragAndDrop(this.m_Target.panel).data,
				modifiers = modifiers
			};
		}

		private float GetHoverBarTopPosition(VisualElement item)
		{
			VisualElement viewport = this.targetScrollView.viewport;
			return Mathf.Min(viewport.WorldToLocal(item.worldBound).yMax, viewport.localBound.yMax - 2f);
		}

		private void PlaceHoverBarAtElement(VisualElement item)
		{
			this.PlaceHoverBarAt(this.GetHoverBarTopPosition(item));
		}

		private void PlaceHoverBarAt(float top)
		{
			this.m_DragHoverBar.style.top = top;
			this.m_DragHoverBar.style.visibility = Visibility.Visible;
			this.m_DragHoverBar.style.marginLeft = 0f;
			this.m_DragHoverBar.style.width = this.targetView.localBound.width;
		}

		private RecycledItem GetRecycledItem(Vector3 pointerPosition)
		{
			foreach (RecycledItem recycledItem in this.targetView.m_IndexToItemDictionary.Values)
			{
				bool flag = recycledItem.element.worldBound.Contains(pointerPosition);
				if (flag)
				{
					return recycledItem;
				}
			}
			return null;
		}

		private bool IsDraggingDisabled()
		{
			return this.targetView == base.dragAndDrop.data.source && !this.enabled;
		}

		private const int k_AutoScrollAreaSize = 10;

		private const int k_PanSpeed = 20;

		private const int k_BetweenElementsAreaSize = 5;

		private const int k_DragHoverBarHeight = 2;

		private const int k_DefaultCursorId = 8;

		private CollectionViewDragger.DragPosition m_LastDragPosition;

		private VisualElement m_DragHoverBar;

		internal struct DragPosition : IEquatable<CollectionViewDragger.DragPosition>
		{
			public bool Equals(CollectionViewDragger.DragPosition other)
			{
				return this.insertAtIndex == other.insertAtIndex && object.Equals(this.recycledItem, other.recycledItem) && this.dropPosition == other.dropPosition;
			}

			public override bool Equals(object obj)
			{
				bool flag;
				if (obj is CollectionViewDragger.DragPosition)
				{
					CollectionViewDragger.DragPosition dragPosition = (CollectionViewDragger.DragPosition)obj;
					flag = this.Equals(dragPosition);
				}
				else
				{
					flag = false;
				}
				return flag;
			}

			public override int GetHashCode()
			{
				return HashCode.Combine<int, RecycledItem, DragAndDropPosition>(this.insertAtIndex, this.recycledItem, this.dropPosition);
			}

			public int insertAtIndex;

			public RecycledItem recycledItem;

			public DragAndDropPosition dropPosition;
		}
	}
}
