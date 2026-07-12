using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal class ListViewDragger : DragEventsProcessor
	{
		protected BaseVerticalCollectionView targetView
		{
			get
			{
				return this.m_Target as BaseVerticalCollectionView;
			}
		}

		protected ScrollView targetScrollView
		{
			get
			{
				return this.targetView.scrollView;
			}
		}

		public ICollectionDragAndDropController dragAndDropController { get; set; }

		public ListViewDragger(BaseVerticalCollectionView listView)
			: base(listView)
		{
		}

		protected override bool CanStartDrag(Vector3 pointerPosition)
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
					ReusableCollectionItem recycledItem = this.GetRecycledItem(pointerPosition);
					bool flag4 = recycledItem != null && this.targetView.HasCanStartDrag();
					if (flag4)
					{
						IEnumerable<int> enumerable2;
						if (!this.targetView.selectedIds.Any<int>())
						{
							IEnumerable<int> enumerable = new int[] { recycledItem.id };
							enumerable2 = enumerable;
						}
						else
						{
							enumerable2 = this.targetView.selectedIds;
						}
						IEnumerable<int> enumerable3 = enumerable2;
						flag2 = this.targetView.RaiseCanStartDrag(recycledItem, enumerable3);
					}
					else
					{
						bool flag5 = this.targetView.selectedIds.Any<int>();
						if (flag5)
						{
							flag2 = this.dragAndDropController.CanStartDrag(this.targetView.selectedIds);
						}
						else
						{
							flag2 = recycledItem != null && this.dragAndDropController.CanStartDrag(new int[] { recycledItem.id });
						}
					}
				}
			}
			return flag2;
		}

		protected internal override StartDragArgs StartDrag(Vector3 pointerPosition)
		{
			ReusableCollectionItem recycledItem = this.GetRecycledItem(pointerPosition);
			bool flag = recycledItem != null;
			IEnumerable<int> enumerable;
			if (flag)
			{
				bool flag2 = !this.targetView.selectedIndices.Contains(recycledItem.index);
				if (flag2)
				{
					this.targetView.SetSelection(recycledItem.index);
				}
				enumerable = this.targetView.selectedIds;
			}
			else
			{
				enumerable = (this.targetView.selectedIds.Any<int>() ? this.targetView.selectedIds : Enumerable.Empty<int>());
			}
			StartDragArgs startDragArgs = this.dragAndDropController.SetupDragAndDrop(enumerable, false);
			startDragArgs = this.targetView.RaiseSetupDragAndDrop(recycledItem, this.dragAndDropController.GetSortedSelectedIds(), startDragArgs);
			startDragArgs.SetGenericData("__unity-drag-and-drop__source-view", this.targetView);
			return startDragArgs;
		}

		protected internal override void UpdateDrag(Vector3 pointerPosition)
		{
			ListViewDragger.DragPosition dragPosition = default(ListViewDragger.DragPosition);
			DragVisualMode visualMode = this.GetVisualMode(pointerPosition, ref dragPosition);
			bool flag = visualMode == DragVisualMode.Rejected;
			if (flag)
			{
				this.ClearDragAndDropUI(false);
			}
			else
			{
				this.HandleDragAndScroll(pointerPosition);
				this.HandleAutoExpansion(pointerPosition);
				this.ApplyDragAndDropUI(dragPosition);
			}
			base.dragAndDrop.SetVisualMode(visualMode);
			base.dragAndDrop.UpdateDrag(pointerPosition);
		}

		private DragVisualMode GetVisualMode(Vector3 pointerPosition, ref ListViewDragger.DragPosition dragPosition)
		{
			bool flag = this.dragAndDropController == null;
			DragVisualMode dragVisualMode;
			if (flag)
			{
				dragVisualMode = DragVisualMode.Rejected;
			}
			else
			{
				bool flag2 = this.TryGetDragPosition(pointerPosition, ref dragPosition);
				DragAndDropArgs dragAndDropArgs = this.MakeDragAndDropArgs(dragPosition);
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

		protected internal override void OnDrop(Vector3 pointerPosition)
		{
			ListViewDragger.DragPosition dragPosition = default(ListViewDragger.DragPosition);
			bool flag = !this.TryGetDragPosition(pointerPosition, ref dragPosition);
			if (!flag)
			{
				DragAndDropArgs dragAndDropArgs = this.MakeDragAndDropArgs(dragPosition);
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
					bool flag4 = this.dragAndDropController.HandleDragAndDrop(dragAndDropArgs) != DragVisualMode.Rejected;
					if (flag4)
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

		internal void HandleDragAndScroll(Vector2 pointerPosition)
		{
			bool flag = pointerPosition.y < this.targetScrollView.worldBound.yMin + 5f;
			bool flag2 = pointerPosition.y > this.targetScrollView.worldBound.yMax - 5f;
			bool flag3 = flag || flag2;
			if (flag3)
			{
				Vector2 vector = this.targetScrollView.scrollOffset + (flag ? Vector2.down : Vector2.up) * 20f;
				vector.y = Mathf.Clamp(vector.y, 0f, Mathf.Max(0f, this.targetScrollView.contentContainer.worldBound.height - this.targetScrollView.contentViewport.worldBound.height));
				this.targetScrollView.scrollOffset = vector;
			}
		}

		private void HandleAutoExpansion(Vector2 pointerPosition)
		{
			ReusableCollectionItem recycledItem = this.GetRecycledItem(pointerPosition);
			bool flag = recycledItem == null;
			if (!flag)
			{
				this.dragAndDropController.HandleAutoExpand(recycledItem, pointerPosition);
			}
		}

		private void ApplyDragAndDropUI(ListViewDragger.DragPosition dragPosition)
		{
			bool flag = this.m_LastDragPosition.Equals(dragPosition);
			if (!flag)
			{
				bool flag2 = this.m_DragHoverBar == null;
				if (flag2)
				{
					this.m_DragHoverBar = new VisualElement();
					this.m_DragHoverBar.AddToClassList(BaseVerticalCollectionView.dragHoverBarUssClassName);
					this.m_DragHoverBar.style.width = this.targetView.localBound.width;
					this.m_DragHoverBar.style.visibility = Visibility.Hidden;
					this.m_DragHoverBar.pickingMode = PickingMode.Ignore;
					this.targetView.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.<ApplyDragAndDropUI>g__GeometryChangedCallback|27_0), TrickleDown.NoTrickleDown);
					this.targetScrollView.contentViewport.Add(this.m_DragHoverBar);
				}
				bool flag3 = this.m_DragHoverItemMarker == null && this.targetView is BaseTreeView;
				if (flag3)
				{
					this.m_DragHoverItemMarker = new VisualElement();
					this.m_DragHoverItemMarker.AddToClassList(BaseVerticalCollectionView.dragHoverMarkerUssClassName);
					this.m_DragHoverItemMarker.style.visibility = Visibility.Hidden;
					this.m_DragHoverItemMarker.pickingMode = PickingMode.Ignore;
					this.m_DragHoverBar.Add(this.m_DragHoverItemMarker);
					this.m_DragHoverSiblingMarker = new VisualElement();
					this.m_DragHoverSiblingMarker.AddToClassList(BaseVerticalCollectionView.dragHoverMarkerUssClassName);
					this.m_DragHoverSiblingMarker.style.visibility = Visibility.Hidden;
					this.m_DragHoverSiblingMarker.pickingMode = PickingMode.Ignore;
					this.targetScrollView.contentViewport.Add(this.m_DragHoverSiblingMarker);
				}
				this.ClearDragAndDropUI(false);
				this.m_LastDragPosition = dragPosition;
				switch (dragPosition.dropPosition)
				{
				case DragAndDropPosition.OverItem:
					dragPosition.recycledItem.rootElement.AddToClassList(BaseVerticalCollectionView.itemDragHoverUssClassName);
					break;
				case DragAndDropPosition.BetweenItems:
				{
					bool flag4 = dragPosition.insertAtIndex == 0;
					if (flag4)
					{
						this.PlaceHoverBarAt(0f, -1f, -1f);
					}
					else
					{
						ReusableCollectionItem recycledItemFromIndex = this.targetView.GetRecycledItemFromIndex(dragPosition.insertAtIndex - 1);
						ReusableCollectionItem recycledItemFromIndex2 = this.targetView.GetRecycledItemFromIndex(dragPosition.insertAtIndex);
						this.PlaceHoverBarAtElement(recycledItemFromIndex ?? recycledItemFromIndex2);
					}
					break;
				}
				case DragAndDropPosition.OutsideItems:
				{
					ReusableCollectionItem recycledItemFromIndex3 = this.targetView.GetRecycledItemFromIndex(this.targetView.itemsSource.Count - 1);
					bool flag5 = recycledItemFromIndex3 != null;
					if (flag5)
					{
						this.PlaceHoverBarAtElement(recycledItemFromIndex3);
					}
					else
					{
						this.PlaceHoverBarAt(0f, -1f, -1f);
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("dropPosition", dragPosition.dropPosition, "Unsupported dropPosition value");
				}
			}
		}

		protected virtual bool TryGetDragPosition(Vector2 pointerPosition, ref ListViewDragger.DragPosition dragPosition)
		{
			ReusableCollectionItem recycledItem = this.GetRecycledItem(pointerPosition);
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
					bool flag4 = pointerPosition.y >= this.targetScrollView.contentContainer.worldBound.yMax;
					if (flag4)
					{
						dragPosition.insertAtIndex = this.targetView.itemsSource.Count;
					}
					else
					{
						dragPosition.insertAtIndex = 0;
					}
					this.HandleTreePosition(pointerPosition, ref dragPosition);
					flag3 = true;
				}
			}
			else
			{
				bool flag5 = recycledItem.rootElement.worldBound.yMax - pointerPosition.y < 5f;
				if (flag5)
				{
					dragPosition.insertAtIndex = recycledItem.index + 1;
					dragPosition.dropPosition = DragAndDropPosition.BetweenItems;
				}
				else
				{
					bool flag6 = pointerPosition.y - recycledItem.rootElement.worldBound.yMin > 5f;
					if (flag6)
					{
						Vector2 scrollOffset = this.targetScrollView.scrollOffset;
						this.targetScrollView.ScrollTo(recycledItem.rootElement);
						bool flag7 = !Mathf.Approximately(scrollOffset.x, this.targetScrollView.scrollOffset.x) || !Mathf.Approximately(scrollOffset.y, this.targetScrollView.scrollOffset.y);
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
				this.HandleTreePosition(pointerPosition, ref dragPosition);
				flag3 = true;
			}
			return flag3;
		}

		private void HandleTreePosition(Vector2 pointerPosition, ref ListViewDragger.DragPosition dragPosition)
		{
			dragPosition.parentId = -1;
			dragPosition.childIndex = -1;
			this.m_LeftIndentation = -1f;
			this.m_SiblingBottom = -1f;
			BaseTreeView baseTreeView = this.targetView as BaseTreeView;
			bool flag = baseTreeView == null;
			if (!flag)
			{
				bool flag2 = dragPosition.insertAtIndex < 0;
				if (!flag2)
				{
					BaseTreeViewController viewController = baseTreeView.viewController;
					bool flag3 = dragPosition.dropPosition == DragAndDropPosition.OverItem;
					if (flag3)
					{
						dragPosition.parentId = viewController.GetIdForIndex(dragPosition.insertAtIndex);
						dragPosition.childIndex = -1;
					}
					else
					{
						bool flag4 = dragPosition.insertAtIndex <= 0;
						if (flag4)
						{
							dragPosition.childIndex = 0;
						}
						else
						{
							this.HandleSiblingInsertionAtAvailableDepthsAndChangeTargetIfNeeded(ref dragPosition, pointerPosition);
						}
					}
				}
			}
		}

		private void HandleSiblingInsertionAtAvailableDepthsAndChangeTargetIfNeeded(ref ListViewDragger.DragPosition dragPosition, Vector2 pointerPosition)
		{
			BaseTreeView baseTreeView = this.targetView as BaseTreeView;
			bool flag = baseTreeView == null;
			if (!flag)
			{
				BaseTreeViewController viewController = baseTreeView.viewController;
				int insertAtIndex = dragPosition.insertAtIndex;
				int idForIndex = viewController.GetIdForIndex(insertAtIndex);
				int num;
				int num2;
				this.GetPreviousAndNextItemsIgnoringDraggedItems(dragPosition.insertAtIndex, out num, out num2);
				bool flag2 = num == -1;
				if (!flag2)
				{
					bool flag3 = viewController.HasChildren(num) && baseTreeView.IsExpanded(num);
					int indentationDepth = viewController.GetIndentationDepth(num);
					int indentationDepth2 = viewController.GetIndentationDepth(num2);
					int num3 = ((num2 != -1) ? indentationDepth2 : 0);
					int num4 = viewController.GetIndentationDepth(num) + (flag3 ? 1 : 0);
					int num5 = num;
					float num6 = 15f;
					float num7 = 15f;
					bool flag4 = indentationDepth > 0;
					if (flag4)
					{
						VisualElement rootElementForId = baseTreeView.GetRootElementForId(num);
						VisualElement visualElement = rootElementForId.Q(BaseTreeView.itemIndentUssClassName, null);
						VisualElement visualElement2 = rootElementForId.Q(BaseTreeView.itemToggleUssClassName, null);
						num6 = visualElement2.layout.width;
						num7 = visualElement.layout.width / (float)indentationDepth;
					}
					else
					{
						int indentationDepth3 = baseTreeView.viewController.GetIndentationDepth(idForIndex);
						bool flag5 = indentationDepth3 > 0;
						if (flag5)
						{
							VisualElement rootElementForId2 = baseTreeView.GetRootElementForId(idForIndex);
							VisualElement visualElement3 = rootElementForId2.Q(BaseTreeView.itemIndentUssClassName, null);
							VisualElement visualElement4 = rootElementForId2.Q(BaseTreeView.itemToggleUssClassName, null);
							num6 = visualElement4.layout.width;
							num7 = visualElement3.layout.width / (float)indentationDepth3;
						}
					}
					bool flag6 = num4 <= num3;
					if (flag6)
					{
						this.m_LeftIndentation = num6 + num7 * (float)num3;
						bool flag7 = flag3;
						if (flag7)
						{
							dragPosition.parentId = num;
							dragPosition.childIndex = 0;
						}
						else
						{
							dragPosition.parentId = viewController.GetParentId(num);
							dragPosition.childIndex = viewController.GetChildIndexForId(num2);
						}
					}
					else
					{
						Vector2 vector = baseTreeView.scrollView.contentContainer.WorldToLocal(pointerPosition);
						int num8 = Mathf.FloorToInt((vector.x - num6) / num7);
						bool flag8 = num8 >= num4;
						if (flag8)
						{
							this.m_LeftIndentation = num6 + num7 * (float)num4;
							bool flag9 = flag3;
							if (flag9)
							{
								dragPosition.parentId = num;
								dragPosition.childIndex = 0;
							}
							else
							{
								dragPosition.parentId = viewController.GetParentId(num);
								dragPosition.childIndex = viewController.GetChildIndexForId(num) + 1;
							}
						}
						else
						{
							int i;
							for (i = viewController.GetIndentationDepth(num5); i > num3; i--)
							{
								bool flag10 = i == num8;
								if (flag10)
								{
									break;
								}
								num5 = viewController.GetParentId(num5);
							}
							bool flag11 = num5 != idForIndex;
							bool flag12 = flag11;
							if (flag12)
							{
								VisualElement rootElementForId3 = baseTreeView.GetRootElementForId(num5);
								bool flag13 = rootElementForId3 != null;
								if (flag13)
								{
									VisualElement contentViewport = this.targetScrollView.contentViewport;
									Rect rect = contentViewport.WorldToLocal(rootElementForId3.worldBound);
									bool flag14 = contentViewport.localBound.yMin < rect.yMax && rect.yMax < contentViewport.localBound.yMax;
									if (flag14)
									{
										this.m_SiblingBottom = rect.yMax;
									}
								}
							}
							dragPosition.parentId = viewController.GetParentId(num5);
							dragPosition.childIndex = viewController.GetChildIndexForId(num5) + 1;
							this.m_LeftIndentation = num6 + num7 * (float)i;
						}
					}
				}
			}
		}

		private void GetPreviousAndNextItemsIgnoringDraggedItems(int insertAtIndex, out int previousItemId, out int nextItemId)
		{
			previousItemId = (nextItemId = -1);
			int i = insertAtIndex - 1;
			int j = insertAtIndex;
			while (i >= 0)
			{
				int idForIndex = this.targetView.viewController.GetIdForIndex(i);
				bool flag = !this.dragAndDropController.GetSortedSelectedIds().Contains(idForIndex);
				if (flag)
				{
					previousItemId = idForIndex;
					break;
				}
				i--;
			}
			while (j < this.targetView.itemsSource.Count)
			{
				int idForIndex2 = this.targetView.viewController.GetIdForIndex(j);
				bool flag2 = !this.dragAndDropController.GetSortedSelectedIds().Contains(idForIndex2);
				if (flag2)
				{
					nextItemId = idForIndex2;
					break;
				}
				j++;
			}
		}

		protected DragAndDropArgs MakeDragAndDropArgs(ListViewDragger.DragPosition dragPosition)
		{
			object obj = null;
			ReusableCollectionItem recycledItem = dragPosition.recycledItem;
			bool flag = recycledItem != null;
			if (flag)
			{
				obj = this.targetView.viewController.GetItemForIndex(recycledItem.index);
			}
			return new DragAndDropArgs
			{
				target = obj,
				insertAtIndex = dragPosition.insertAtIndex,
				parentId = dragPosition.parentId,
				childIndex = dragPosition.childIndex,
				dragAndDropPosition = dragPosition.dropPosition,
				dragAndDropData = DragAndDropUtility.GetDragAndDrop(this.m_Target.panel).data
			};
		}

		private float GetHoverBarTopPosition(ReusableCollectionItem item)
		{
			VisualElement contentViewport = this.targetScrollView.contentViewport;
			return Mathf.Min(contentViewport.WorldToLocal(item.rootElement.worldBound).yMax, contentViewport.localBound.yMax - 2f);
		}

		private void PlaceHoverBarAtElement(ReusableCollectionItem item)
		{
			this.PlaceHoverBarAt(this.GetHoverBarTopPosition(item), this.m_LeftIndentation, this.m_SiblingBottom);
		}

		private void PlaceHoverBarAt(float top, float indentationPadding = -1f, float siblingBottom = -1f)
		{
			this.m_DragHoverBar.style.top = top;
			this.m_DragHoverBar.style.visibility = Visibility.Visible;
			bool flag = this.m_DragHoverItemMarker != null;
			if (flag)
			{
				this.m_DragHoverItemMarker.style.visibility = Visibility.Visible;
			}
			bool flag2 = indentationPadding >= 0f;
			if (flag2)
			{
				this.m_DragHoverBar.style.marginLeft = indentationPadding;
				this.m_DragHoverBar.style.width = this.targetView.localBound.width - indentationPadding;
				bool flag3 = siblingBottom > 0f && this.m_DragHoverSiblingMarker != null;
				if (flag3)
				{
					this.m_DragHoverSiblingMarker.style.top = siblingBottom;
					this.m_DragHoverSiblingMarker.style.visibility = Visibility.Visible;
					this.m_DragHoverSiblingMarker.style.marginLeft = indentationPadding;
				}
			}
			else
			{
				this.m_DragHoverBar.style.marginLeft = 0f;
				this.m_DragHoverBar.style.width = this.targetView.localBound.width;
			}
		}

		protected override void ClearDragAndDropUI(bool dragCancelled)
		{
			if (dragCancelled)
			{
				this.dragAndDropController.DragCleanup();
			}
			this.targetView.elementPanel.cursorManager.ResetCursor();
			this.m_LastDragPosition = default(ListViewDragger.DragPosition);
			foreach (ReusableCollectionItem reusableCollectionItem in this.targetView.activeItems)
			{
				reusableCollectionItem.rootElement.RemoveFromClassList(BaseVerticalCollectionView.itemDragHoverUssClassName);
			}
			bool flag = this.m_DragHoverBar != null;
			if (flag)
			{
				this.m_DragHoverBar.style.visibility = Visibility.Hidden;
			}
			bool flag2 = this.m_DragHoverItemMarker != null;
			if (flag2)
			{
				this.m_DragHoverItemMarker.style.visibility = Visibility.Hidden;
			}
			bool flag3 = this.m_DragHoverSiblingMarker != null;
			if (flag3)
			{
				this.m_DragHoverSiblingMarker.style.visibility = Visibility.Hidden;
			}
		}

		protected ReusableCollectionItem GetRecycledItem(Vector3 pointerPosition)
		{
			foreach (ReusableCollectionItem reusableCollectionItem in this.targetView.activeItems)
			{
				bool flag = reusableCollectionItem.rootElement.worldBound.Contains(pointerPosition);
				if (flag)
				{
					return reusableCollectionItem;
				}
			}
			return null;
		}

		[CompilerGenerated]
		private void <ApplyDragAndDropUI>g__GeometryChangedCallback|27_0(GeometryChangedEvent e)
		{
			this.m_DragHoverBar.style.width = this.targetView.localBound.width;
		}

		private ListViewDragger.DragPosition m_LastDragPosition;

		private VisualElement m_DragHoverBar;

		private VisualElement m_DragHoverItemMarker;

		private VisualElement m_DragHoverSiblingMarker;

		private float m_LeftIndentation = -1f;

		private float m_SiblingBottom = -1f;

		private const int k_AutoScrollAreaSize = 5;

		private const int k_BetweenElementsAreaSize = 5;

		private const int k_PanSpeed = 20;

		private const int k_DragHoverBarHeight = 2;

		internal struct DragPosition : IEquatable<ListViewDragger.DragPosition>
		{
			public bool Equals(ListViewDragger.DragPosition other)
			{
				return this.insertAtIndex == other.insertAtIndex && this.parentId == other.parentId && this.childIndex == other.childIndex && object.Equals(this.recycledItem, other.recycledItem) && this.dropPosition == other.dropPosition;
			}

			public override bool Equals(object obj)
			{
				bool flag;
				if (obj is ListViewDragger.DragPosition)
				{
					ListViewDragger.DragPosition dragPosition = (ListViewDragger.DragPosition)obj;
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
				int num = this.insertAtIndex;
				num = (num * 397) ^ this.parentId;
				num = (num * 397) ^ this.childIndex;
				int num2 = num * 397;
				ReusableCollectionItem reusableCollectionItem = this.recycledItem;
				num = num2 ^ ((reusableCollectionItem != null) ? reusableCollectionItem.GetHashCode() : 0);
				return (num * 397) ^ (int)this.dropPosition;
			}

			public int insertAtIndex;

			public int parentId;

			public int childIndex;

			public ReusableCollectionItem recycledItem;

			public DragAndDropPosition dropPosition;
		}
	}
}
