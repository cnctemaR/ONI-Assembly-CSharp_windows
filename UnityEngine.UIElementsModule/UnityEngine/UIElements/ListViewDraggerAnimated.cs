using System;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements
{
	internal class ListViewDraggerAnimated : ListViewDragger
	{
		public bool isDragging { get; private set; }

		public ReusableCollectionItem draggedItem
		{
			get
			{
				return this.m_Item;
			}
		}

		protected override bool supportsDragEvents
		{
			get
			{
				return false;
			}
		}

		public ListViewDraggerAnimated(BaseVerticalCollectionView listView)
			: base(listView)
		{
		}

		protected internal override StartDragArgs StartDrag(Vector3 pointerPosition, EventModifiers modifiers)
		{
			bool flag = !base.enabled;
			StartDragArgs startDragArgs;
			if (flag)
			{
				startDragArgs = base.StartDrag(pointerPosition, modifiers);
			}
			else
			{
				base.targetView.ClearSelection();
				ReusableCollectionItem recycledItem = base.GetRecycledItem(pointerPosition);
				bool flag2 = recycledItem == null;
				if (flag2)
				{
					startDragArgs = new StartDragArgs(string.Empty, DragVisualMode.Rejected, modifiers);
				}
				else
				{
					bool flag3 = base.targetView.selectionType > SelectionType.None;
					if (flag3)
					{
						base.targetView.SetSelection(recycledItem.index);
					}
					this.isDragging = true;
					this.m_Item = recycledItem;
					base.targetView.virtualizationController.StartDragItem(this.m_Item);
					float y = this.m_Item.rootElement.layout.y;
					this.m_SelectionHeight = this.m_Item.rootElement.layout.height;
					this.m_Item.rootElement.style.position = Position.Absolute;
					this.m_Item.rootElement.style.height = this.m_Item.rootElement.layout.height;
					this.m_Item.rootElement.style.width = this.m_Item.rootElement.layout.width;
					this.m_Item.rootElement.style.top = y;
					this.m_DragStartIndex = this.m_Item.index;
					this.m_CurrentIndex = this.m_DragStartIndex;
					this.m_CurrentPointerPosition = pointerPosition;
					this.m_LocalOffsetOnStart = base.targetScrollView.contentContainer.WorldToLocal(pointerPosition).y - y;
					ReusableCollectionItem recycledItemFromIndex = base.targetView.GetRecycledItemFromIndex(this.m_CurrentIndex + 1);
					bool flag4 = recycledItemFromIndex != null;
					if (flag4)
					{
						this.m_OffsetItem = recycledItemFromIndex;
						this.Animate(this.m_OffsetItem, this.m_SelectionHeight);
						this.m_OffsetItem.rootElement.style.paddingTop = this.m_SelectionHeight;
						bool flag5 = base.targetView.virtualizationMethod == CollectionVirtualizationMethod.FixedHeight;
						if (flag5)
						{
							this.m_OffsetItem.rootElement.style.height = base.targetView.fixedItemHeight + this.m_SelectionHeight;
						}
					}
					StartDragArgs startDragArgs2 = base.dragAndDropController.SetupDragAndDrop(new int[] { this.m_Item.index }, true);
					startDragArgs2.modifiers = modifiers;
					startDragArgs = startDragArgs2;
				}
			}
			return startDragArgs;
		}

		protected internal override void UpdateDrag(Vector3 pointerPosition, EventModifiers modifiers)
		{
			bool flag = !base.enabled;
			if (flag)
			{
				base.UpdateDrag(pointerPosition, modifiers);
			}
			else
			{
				bool flag2 = this.m_Item == null;
				if (!flag2)
				{
					base.HandleDragAndScroll(pointerPosition);
					this.m_CurrentPointerPosition = pointerPosition;
					Vector2 vector = base.targetScrollView.contentContainer.WorldToLocal(this.m_CurrentPointerPosition);
					Rect layout = this.m_Item.rootElement.layout;
					float height = base.targetScrollView.contentContainer.layout.height;
					layout.y = Mathf.Clamp(vector.y - this.m_LocalOffsetOnStart, 0f, height - this.m_SelectionHeight);
					float num = base.targetScrollView.contentContainer.resolvedStyle.paddingTop;
					this.m_CurrentIndex = -1;
					foreach (ReusableCollectionItem reusableCollectionItem in base.targetView.activeItems)
					{
						bool flag3 = reusableCollectionItem.index < 0 || (reusableCollectionItem.rootElement.style.display == DisplayStyle.None && !reusableCollectionItem.isDragGhost);
						if (!flag3)
						{
							bool flag4 = reusableCollectionItem.index == this.m_Item.index && reusableCollectionItem.index < base.targetView.itemsSource.Count - 1;
							if (flag4)
							{
								float expectedItemHeight = base.targetView.virtualizationController.GetExpectedItemHeight(reusableCollectionItem.index + 1);
								bool flag5 = Mathf.Approximately(layout.y + expectedItemHeight, height);
								if (!flag5)
								{
									bool flag6 = layout.y <= num + expectedItemHeight * 0.5f;
									if (flag6)
									{
										this.m_CurrentIndex = reusableCollectionItem.index;
									}
								}
							}
							else
							{
								float expectedItemHeight2 = base.targetView.virtualizationController.GetExpectedItemHeight(reusableCollectionItem.index);
								bool flag7 = layout.y <= num + expectedItemHeight2 * 0.5f;
								if (flag7)
								{
									bool flag8 = this.m_CurrentIndex == -1;
									if (flag8)
									{
										this.m_CurrentIndex = reusableCollectionItem.index;
									}
									bool flag9 = this.m_OffsetItem == reusableCollectionItem;
									if (flag9)
									{
										break;
									}
									this.Animate(this.m_OffsetItem, 0f);
									this.Animate(reusableCollectionItem, this.m_SelectionHeight);
									this.m_OffsetItem = reusableCollectionItem;
									break;
								}
								else
								{
									num += expectedItemHeight2;
								}
							}
						}
					}
					bool flag10 = this.m_CurrentIndex == -1;
					if (flag10)
					{
						this.m_CurrentIndex = base.targetView.itemsSource.Count;
						this.Animate(this.m_OffsetItem, 0f);
						this.m_OffsetItem = null;
					}
					this.m_Item.rootElement.layout = layout;
					this.m_Item.rootElement.BringToFront();
				}
			}
		}

		private void Animate(ReusableCollectionItem element, float paddingTop)
		{
			bool flag = element == null;
			if (!flag)
			{
				bool flag2 = element.animator != null;
				if (flag2)
				{
					bool flag3 = (element.animator.isRunning && element.animator.to.paddingTop == paddingTop) || (!element.animator.isRunning && element.rootElement.style.paddingTop == paddingTop);
					if (flag3)
					{
						return;
					}
				}
				ValueAnimation<StyleValues> animator = element.animator;
				if (animator != null)
				{
					animator.Stop();
				}
				ValueAnimation<StyleValues> animator2 = element.animator;
				if (animator2 != null)
				{
					animator2.Recycle();
				}
				StyleValues styleValues = ((base.targetView.virtualizationMethod == CollectionVirtualizationMethod.FixedHeight) ? new StyleValues
				{
					paddingTop = paddingTop,
					height = base.targetView.ResolveItemHeight(-1f) + paddingTop
				} : new StyleValues
				{
					paddingTop = paddingTop
				});
				element.animator = element.rootElement.experimental.animation.Start(styleValues, 500);
				element.animator.KeepAlive();
			}
		}

		protected internal override void OnDrop(Vector3 pointerPosition, EventModifiers modifiers)
		{
			bool flag = !base.enabled;
			if (flag)
			{
				base.OnDrop(pointerPosition, modifiers);
			}
			else
			{
				bool flag2 = this.m_Item == null;
				if (!flag2)
				{
					this.isDragging = false;
					this.m_Item.rootElement.ClearManualLayout();
					base.targetView.virtualizationController.EndDrag(this.m_CurrentIndex);
					bool flag3 = this.m_OffsetItem != null;
					if (flag3)
					{
						ValueAnimation<StyleValues> animator = this.m_OffsetItem.animator;
						if (animator != null)
						{
							animator.Stop();
						}
						ValueAnimation<StyleValues> animator2 = this.m_OffsetItem.animator;
						if (animator2 != null)
						{
							animator2.Recycle();
						}
						this.m_OffsetItem.animator = null;
						this.m_OffsetItem.rootElement.style.paddingTop = 0f;
						bool flag4 = base.targetView.virtualizationMethod == CollectionVirtualizationMethod.FixedHeight;
						if (flag4)
						{
							this.m_OffsetItem.rootElement.style.height = base.targetView.ResolveItemHeight(-1f);
						}
					}
					ListViewDragger.DragPosition dragPosition = new ListViewDragger.DragPosition
					{
						recycledItem = this.m_Item,
						insertAtIndex = this.m_CurrentIndex,
						dropPosition = DragAndDropPosition.BetweenItems
					};
					DragAndDropArgs dragAndDropArgs = base.MakeDragAndDropArgs(dragPosition, modifiers);
					base.dragAndDropController.OnDrop(dragAndDropArgs);
					base.dragAndDrop.AcceptDrag();
					this.m_Item = null;
					this.m_OffsetItem = null;
				}
			}
		}

		protected override void ClearDragAndDropUI(bool dragCancelled)
		{
		}

		protected override bool TryGetDragPosition(Vector2 pointerPosition, ref ListViewDragger.DragPosition dragPosition)
		{
			dragPosition.recycledItem = this.m_Item;
			dragPosition.insertAtIndex = this.m_CurrentIndex;
			dragPosition.dropPosition = DragAndDropPosition.BetweenItems;
			return true;
		}

		private int m_DragStartIndex;

		private int m_CurrentIndex;

		private float m_SelectionHeight;

		private float m_LocalOffsetOnStart;

		private Vector3 m_CurrentPointerPosition;

		private ReusableCollectionItem m_Item;

		private ReusableCollectionItem m_OffsetItem;
	}
}
