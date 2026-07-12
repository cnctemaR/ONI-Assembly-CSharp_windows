using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class DynamicHeightVirtualizationController<T> : VerticalVirtualizationController<T> where T : ReusableCollectionItem, new()
	{
		internal IReadOnlyDictionary<int, float> itemHeightCache
		{
			get
			{
				return this.m_ItemHeightCache;
			}
		}

		private float defaultExpectedHeight
		{
			get
			{
				bool flag = this.m_MinimumItemHeight > 0f;
				float num;
				if (flag)
				{
					num = this.m_MinimumItemHeight;
				}
				else
				{
					bool flag2 = this.m_CollectionView.m_ItemHeightIsInline && this.m_CollectionView.fixedItemHeight > 0f;
					if (flag2)
					{
						num = this.m_CollectionView.fixedItemHeight;
					}
					else
					{
						num = (float)BaseVerticalCollectionView.s_DefaultItemHeight;
					}
				}
				return num;
			}
		}

		private float contentPadding
		{
			get
			{
				return base.serializedData.contentPadding;
			}
			set
			{
				this.m_CollectionView.scrollView.contentContainer.style.paddingTop = value;
				base.serializedData.contentPadding = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private float contentHeight
		{
			get
			{
				return base.serializedData.contentHeight;
			}
			set
			{
				this.m_CollectionView.scrollView.contentContainer.style.height = value;
				base.serializedData.contentHeight = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private int anchoredIndex
		{
			get
			{
				return base.serializedData.anchoredItemIndex;
			}
			set
			{
				base.serializedData.anchoredItemIndex = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private float anchorOffset
		{
			get
			{
				return base.serializedData.anchorOffset;
			}
			set
			{
				base.serializedData.anchorOffset = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private float viewportMaxOffset
		{
			get
			{
				return base.serializedData.scrollOffset.y + this.m_ScrollView.contentViewport.layout.height;
			}
		}

		protected override bool alwaysRebindOnRefresh
		{
			get
			{
				return false;
			}
		}

		public DynamicHeightVirtualizationController(BaseVerticalCollectionView collectionView)
			: base(collectionView)
		{
			this.m_FillCallback = new Action(this.Fill);
			this.m_ScrollCallback = new Action(this.OnScrollUpdate);
			this.m_GeometryChangedCallback = new Action<ReusableCollectionItem>(this.OnRecycledItemGeometryChanged);
			this.m_IndexOutOfBoundsPredicate = new Predicate<int>(this.IsIndexOutOfBounds);
			this.m_ScrollResetCallback = new Action(this.ResetScroll);
		}

		public override void Refresh(bool rebuild)
		{
			this.CleanItemHeightCache();
			int count = this.m_ActiveItems.Count;
			bool flag = false;
			if (rebuild)
			{
				this.m_WaitingCache.Clear();
			}
			else
			{
				flag |= this.m_WaitingCache.RemoveWhere(this.m_IndexOutOfBoundsPredicate) > 0;
			}
			base.Refresh(rebuild);
			this.m_ScrollDirection = DynamicHeightVirtualizationController<T>.ScrollDirection.Idle;
			this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.None;
			bool flag2 = this.m_CollectionView.HasValidDataAndBindings();
			if (flag2)
			{
				bool flag3 = flag || count != this.m_ActiveItems.Count;
				if (flag3)
				{
					this.contentHeight = this.GetExpectedContentHeight();
					float num = Mathf.Max(0f, this.contentHeight - this.m_ScrollView.contentViewport.layout.height);
					this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num);
					this.m_ScrollView.verticalScroller.value = base.serializedData.scrollOffset.y;
					base.serializedData.scrollOffset.y = this.m_ScrollView.verticalScroller.value;
				}
				this.ScheduleFill();
			}
		}

		public override void ScrollToItem(int index)
		{
			bool flag = index < -1;
			if (!flag)
			{
				float height = this.m_ScrollView.contentContainer.layout.height;
				float height2 = this.m_ScrollView.contentViewport.layout.height;
				bool flag2 = index == -1;
				if (flag2)
				{
					this.m_ForcedLastVisibleItem = base.itemsCount - 1;
					this.m_ForcedFirstVisibleItem = -1;
					this.m_StickToBottom = true;
					this.m_ScrollView.scrollOffset = new Vector2(0f, (height2 >= height) ? 0f : height);
				}
				else
				{
					bool flag3 = this.firstVisibleIndex >= index;
					if (flag3)
					{
						this.m_ForcedFirstVisibleItem = index;
						this.m_ForcedLastVisibleItem = -1;
						this.m_ScrollView.scrollOffset = new Vector2(0f, this.GetContentHeightForIndex(index - 1));
					}
					else
					{
						float contentHeightForIndex = this.GetContentHeightForIndex(index);
						bool flag4 = contentHeightForIndex < this.contentPadding + height2;
						if (!flag4)
						{
							float num = contentHeightForIndex - height2 + (float)BaseVerticalCollectionView.s_DefaultItemHeight;
							this.m_ForcedLastVisibleItem = index;
							this.m_ForcedFirstVisibleItem = -1;
							this.m_ScrollView.scrollOffset = new Vector2(0f, num);
						}
					}
				}
			}
		}

		public override void Resize(Vector2 size)
		{
			float expectedContentHeight = this.GetExpectedContentHeight();
			this.contentHeight = Mathf.Max(expectedContentHeight, this.contentHeight);
			float height = this.m_ScrollView.contentViewport.layout.height;
			float num = Mathf.Max(0f, this.contentHeight - height);
			float num2 = Mathf.Min(base.serializedData.scrollOffset.y, num);
			this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num);
			this.m_ScrollView.verticalScroller.slider.SetValueWithoutNotify(num2);
			base.serializedData.scrollOffset.y = this.m_ScrollView.verticalScroller.value;
			float num3 = this.m_CollectionView.ResolveItemHeight(size.y);
			int num4 = Mathf.CeilToInt(num3 / this.defaultExpectedHeight);
			int num5 = num4;
			bool flag = num5 <= 0;
			if (!flag)
			{
				num5 += 2;
				int num6 = Mathf.Min(num5, base.itemsCount);
				bool flag2 = this.m_ActiveItems.Count != num6;
				if (flag2)
				{
					int count = this.m_ActiveItems.Count;
					bool flag3 = count > num6;
					if (flag3)
					{
						int num7 = count - num6;
						for (int i = 0; i < num7; i++)
						{
							int num8 = this.m_ActiveItems.Count - 1;
							this.ReleaseItem(num8);
						}
					}
					else
					{
						int num9 = num6 - this.m_ActiveItems.Count;
						int num10 = ((this.firstVisibleIndex < 0) ? 0 : this.firstVisibleIndex);
						for (int j = 0; j < num9; j++)
						{
							int num11 = j + num10 + count;
							T orMakeItemAtIndex = this.GetOrMakeItemAtIndex(-1, -1);
							bool flag4 = this.IsIndexOutOfBounds(num11);
							if (flag4)
							{
								this.HideItem(this.m_ActiveItems.Count - 1);
							}
							else
							{
								base.Setup(orMakeItemAtIndex, num11);
								this.MarkWaitingForLayout(orMakeItemAtIndex);
							}
						}
					}
				}
				this.ScheduleFill();
				this.ScheduleScrollDirectionReset();
				this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
			}
		}

		public override void OnScroll(Vector2 scrollOffset)
		{
			bool flag = this.m_DelayedScrollOffset == scrollOffset;
			if (!flag)
			{
				this.m_DelayedScrollOffset = scrollOffset;
				bool flag2 = this.m_ForcedFirstVisibleItem != -1 || this.m_ForcedLastVisibleItem != -1;
				if (flag2)
				{
					this.OnScrollUpdate();
					this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.ForcedScroll;
				}
				else
				{
					bool flag3 = this.m_ScheduledItem == null;
					if (flag3)
					{
						DynamicHeightVirtualizationController<T>.VirtualizationChange lastChange = this.m_LastChange;
						bool flag4 = lastChange == DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize || lastChange == DynamicHeightVirtualizationController<T>.VirtualizationChange.ForcedScroll;
						if (flag4)
						{
							this.m_ScheduledItem = this.m_CollectionView.schedule.Execute(this.m_FillCallback);
							float height = this.m_ScrollView.contentViewport.layout.height;
							float num = Mathf.Max(0f, this.contentHeight - height);
							float num2 = Mathf.Min(base.serializedData.scrollOffset.y, num);
							this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num);
							this.m_ScrollView.verticalScroller.slider.SetValueWithoutNotify(num2);
							base.serializedData.scrollOffset.y = this.m_ScrollView.verticalScroller.value;
							return;
						}
					}
					this.ScheduleScroll();
				}
			}
		}

		private void OnScrollUpdate()
		{
			Vector2 vector = (float.IsNegativeInfinity(this.m_DelayedScrollOffset.y) ? base.serializedData.scrollOffset : this.m_DelayedScrollOffset);
			bool flag = float.IsNaN(this.m_ScrollView.contentViewport.layout.height) || float.IsNaN(vector.y);
			if (!flag)
			{
				this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.Scroll;
				float expectedContentHeight = this.GetExpectedContentHeight();
				this.contentHeight = Mathf.Max(expectedContentHeight, this.contentHeight);
				this.m_ScrollDirection = ((vector.y < base.serializedData.scrollOffset.y) ? DynamicHeightVirtualizationController<T>.ScrollDirection.Up : DynamicHeightVirtualizationController<T>.ScrollDirection.Down);
				float num = Mathf.Max(0f, this.contentHeight - this.m_ScrollView.contentViewport.layout.height);
				bool flag2 = vector.y <= 0f;
				if (flag2)
				{
					this.m_ForcedFirstVisibleItem = 0;
				}
				this.m_StickToBottom = num > 0f && Math.Abs(vector.y - this.m_ScrollView.verticalScroller.highValue) < float.Epsilon;
				base.serializedData.scrollOffset = vector;
				this.m_CollectionView.SaveViewData();
				int num2 = ((this.m_ForcedFirstVisibleItem != -1) ? this.m_ForcedFirstVisibleItem : this.GetFirstVisibleItem(base.serializedData.scrollOffset.y));
				float contentHeightForIndex = this.GetContentHeightForIndex(num2 - 1);
				this.contentPadding = contentHeightForIndex;
				this.m_ForcedFirstVisibleItem = -1;
				bool flag3 = num2 != this.firstVisibleIndex;
				if (flag3)
				{
					this.CycleItems(num2);
				}
				else
				{
					this.Fill();
				}
				this.ScheduleScrollDirectionReset();
				this.m_DelayedScrollOffset = Vector2.negativeInfinity;
			}
		}

		private void CycleItems(int firstIndex)
		{
			bool flag = firstIndex == this.firstVisibleIndex;
			if (!flag)
			{
				T firstVisibleItem = base.firstVisibleItem;
				this.contentPadding = this.GetContentHeightForIndex(firstIndex - 1);
				this.firstVisibleIndex = firstIndex;
				bool flag2 = this.m_ActiveItems.Count > 0;
				if (flag2)
				{
					bool flag3 = firstVisibleItem == null || this.m_ActiveItems.Count <= Mathf.Abs(this.firstVisibleIndex - firstVisibleItem.index);
					if (!flag3)
					{
						bool flag4 = this.firstVisibleIndex < firstVisibleItem.index;
						if (flag4)
						{
							int num = firstVisibleItem.index - this.firstVisibleIndex;
							List<T> scrollInsertionList = this.m_ScrollInsertionList;
							for (int i = 0; i < num; i++)
							{
								List<T> activeItems = this.m_ActiveItems;
								T t = activeItems[activeItems.Count - 1];
								scrollInsertionList.Insert(0, t);
								this.m_ActiveItems.RemoveAt(this.m_ActiveItems.Count - 1);
								t.rootElement.SendToBack();
							}
							this.m_ActiveItems.InsertRange(0, scrollInsertionList);
							this.m_ScrollInsertionList.Clear();
						}
						else
						{
							List<T> scrollInsertionList2 = this.m_ScrollInsertionList;
							int num2 = 0;
							while (this.firstVisibleIndex > this.m_ActiveItems[num2].index)
							{
								T t2 = this.m_ActiveItems[num2];
								scrollInsertionList2.Add(t2);
								num2++;
								t2.rootElement.BringToFront();
							}
							this.m_ActiveItems.RemoveRange(0, num2);
							this.m_ActiveItems.AddRange(scrollInsertionList2);
							this.m_ScrollInsertionList.Clear();
						}
					}
					float num3 = this.contentPadding;
					for (int j = 0; j < this.m_ActiveItems.Count; j++)
					{
						T t3 = this.m_ActiveItems[j];
						int num4 = this.firstVisibleIndex + j;
						int index = t3.index;
						bool flag5 = t3.rootElement.style.display == DisplayStyle.Flex;
						this.m_WaitingCache.Remove(index);
						bool flag6 = this.IsIndexOutOfBounds(num4);
						if (flag6)
						{
							this.HideItem(j);
						}
						else
						{
							base.Setup(t3, num4);
							bool flag7 = num3 > this.viewportMaxOffset;
							bool flag8 = flag7;
							if (flag8)
							{
								this.HideItem(j);
							}
							else
							{
								bool flag9 = num4 != index || !flag5;
								if (flag9)
								{
									this.MarkWaitingForLayout(t3);
								}
							}
							num3 += this.GetExpectedItemHeight(num4);
						}
					}
				}
				bool flag10 = this.m_LastChange != DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
				if (flag10)
				{
					this.UpdateAnchor();
				}
				this.ScheduleFill();
			}
		}

		private bool NeedsFill()
		{
			bool flag = this.m_LastChange != DynamicHeightVirtualizationController<T>.VirtualizationChange.None || this.anchoredIndex < 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				T t = base.lastVisibleItem;
				int num = ((t != null) ? t.index : (-1));
				float num2 = this.contentPadding;
				bool flag3 = num2 > base.serializedData.scrollOffset.y;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					for (int i = this.firstVisibleIndex; i < base.itemsCount; i++)
					{
						bool flag4 = num2 > this.viewportMaxOffset || (num2 == this.viewportMaxOffset && !this.m_StickToBottom);
						if (flag4)
						{
							break;
						}
						num2 += this.GetExpectedItemHeight(i);
						bool flag5 = i > num;
						if (flag5)
						{
							return true;
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		private void Fill()
		{
			bool flag = !this.m_CollectionView.HasValidDataAndBindings();
			if (!flag)
			{
				bool flag2 = this.m_ActiveItems.Count == 0;
				if (flag2)
				{
					this.contentHeight = 0f;
					this.contentPadding = 0f;
				}
				else
				{
					bool flag3 = this.anchoredIndex < 0;
					if (!flag3)
					{
						bool flag4 = this.contentPadding > this.contentHeight;
						if (flag4)
						{
							this.OnScrollUpdate();
						}
						else
						{
							float num = this.contentPadding;
							float num2 = this.contentPadding;
							int num3 = 0;
							for (int i = this.firstVisibleIndex; i < base.itemsCount; i++)
							{
								bool flag5 = num2 > this.viewportMaxOffset || (num2 == this.viewportMaxOffset && !this.m_StickToBottom);
								if (flag5)
								{
									break;
								}
								num2 += this.GetExpectedItemHeight(i);
								T t = this.m_ActiveItems[num3++];
								bool flag6 = t.index != i || t.rootElement.style.display == DisplayStyle.None;
								if (flag6)
								{
									base.Setup(t, i);
									this.MarkWaitingForLayout(t);
								}
								bool flag7 = num3 >= this.m_ActiveItems.Count;
								if (flag7)
								{
									break;
								}
							}
							bool flag8 = this.firstVisibleIndex > 0 && this.contentPadding > base.serializedData.scrollOffset.y;
							if (flag8)
							{
								List<T> scrollInsertionList = this.m_ScrollInsertionList;
								for (int j = this.m_ActiveItems.Count - 1; j >= num3; j--)
								{
									bool flag9 = this.firstVisibleIndex == 0;
									if (flag9)
									{
										break;
									}
									T t2 = this.m_ActiveItems[j];
									scrollInsertionList.Insert(0, t2);
									this.m_ActiveItems.RemoveAt(this.m_ActiveItems.Count - 1);
									t2.rootElement.SendToBack();
									int num4 = this.firstVisibleIndex - 1;
									this.firstVisibleIndex = num4;
									int num5 = num4;
									base.Setup(t2, num5);
									this.MarkWaitingForLayout(t2);
									num -= this.GetExpectedItemHeight(num5);
									bool flag10 = num < base.serializedData.scrollOffset.y;
									if (flag10)
									{
										break;
									}
								}
								this.m_ActiveItems.InsertRange(0, scrollInsertionList);
								this.m_ScrollInsertionList.Clear();
							}
							this.contentPadding = num;
							this.contentHeight = this.GetExpectedContentHeight();
							bool flag11 = this.m_LastChange != DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
							if (flag11)
							{
								this.UpdateAnchor();
							}
							bool flag12 = this.m_WaitingCache.Count == 0;
							if (flag12)
							{
								this.ResetScroll();
								this.ApplyScrollViewUpdate(true);
							}
						}
					}
				}
			}
		}

		private void UpdateScrollViewContainer(float previousHeight, float newHeight)
		{
			bool stickToBottom = this.m_StickToBottom;
			if (!stickToBottom)
			{
				bool flag = this.m_ForcedLastVisibleItem >= 0;
				if (flag)
				{
					float contentHeightForIndex = this.GetContentHeightForIndex(this.m_ForcedLastVisibleItem);
					base.serializedData.scrollOffset.y = contentHeightForIndex + (float)BaseVerticalCollectionView.s_DefaultItemHeight - this.m_ScrollView.contentViewport.layout.height;
				}
				else
				{
					bool flag2 = this.m_ScrollDirection == DynamicHeightVirtualizationController<T>.ScrollDirection.Up;
					if (flag2)
					{
						SerializedVirtualizationData serializedData = base.serializedData;
						serializedData.scrollOffset.y = serializedData.scrollOffset.y + (newHeight - previousHeight);
					}
				}
			}
		}

		private void ApplyScrollViewUpdate(bool dimensionsOnly = false)
		{
			float contentPadding = this.contentPadding;
			float y = base.serializedData.scrollOffset.y;
			float num = y - contentPadding;
			bool flag = this.anchoredIndex >= 0;
			if (flag)
			{
				bool flag2 = this.firstVisibleIndex != this.anchoredIndex;
				if (flag2)
				{
					this.CycleItems(this.anchoredIndex);
					this.ScheduleFill();
				}
				this.firstVisibleIndex = this.anchoredIndex;
				num = this.anchorOffset;
			}
			float expectedContentHeight = this.GetExpectedContentHeight();
			this.contentHeight = expectedContentHeight;
			this.contentPadding = this.GetContentHeightForIndex(this.firstVisibleIndex - 1);
			float num2 = Mathf.Max(0f, this.m_ScrollView.RoundToPanelPixelSize(expectedContentHeight - this.m_ScrollView.contentViewport.layout.height));
			float num3 = Mathf.Min(this.contentPadding + num, num2);
			bool flag3 = this.m_StickToBottom && num2 > 0f;
			if (flag3)
			{
				num3 = num2;
			}
			else
			{
				bool flag4 = this.m_ForcedLastVisibleItem != -1;
				if (flag4)
				{
					float contentHeightForIndex = this.GetContentHeightForIndex(this.m_ForcedLastVisibleItem);
					float num4 = contentHeightForIndex + (float)BaseVerticalCollectionView.s_DefaultItemHeight - this.m_ScrollView.contentViewport.layout.height;
					num3 = num4;
				}
			}
			this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num2);
			this.m_ScrollView.verticalScroller.slider.SetValueWithoutNotify(num3);
			base.serializedData.scrollOffset.y = this.m_ScrollView.verticalScroller.slider.value;
			bool flag5 = dimensionsOnly || this.m_LastChange == DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
			if (flag5)
			{
				this.ScheduleScrollDirectionReset();
			}
			else
			{
				bool flag6 = this.NeedsFill();
				if (flag6)
				{
					this.Fill();
				}
				else
				{
					float num5 = this.contentPadding;
					int firstVisibleIndex = this.firstVisibleIndex;
					List<T> scrollInsertionList = this.m_ScrollInsertionList;
					int num6 = 0;
					for (int i = 0; i < this.m_ActiveItems.Count; i++)
					{
						T t = this.m_ActiveItems[i];
						int index = t.index;
						bool flag7 = index < 0;
						if (flag7)
						{
							break;
						}
						float expectedItemHeight = this.GetExpectedItemHeight(index);
						bool flag8 = this.m_ActiveItems[i].rootElement.style.display == DisplayStyle.Flex;
						if (flag8)
						{
							bool flag9 = num5 + expectedItemHeight <= base.serializedData.scrollOffset.y;
							if (flag9)
							{
								t.rootElement.BringToFront();
								this.HideItem(i);
								scrollInsertionList.Add(t);
								num6++;
								int firstVisibleIndex2 = this.firstVisibleIndex;
								this.firstVisibleIndex = firstVisibleIndex2 + 1;
							}
							else
							{
								bool flag10 = num5 > this.viewportMaxOffset;
								if (flag10)
								{
									this.HideItem(i);
								}
							}
						}
						num5 += this.GetExpectedItemHeight(index);
					}
					this.m_ActiveItems.RemoveRange(0, num6);
					this.m_ActiveItems.AddRange(scrollInsertionList);
					this.m_ScrollInsertionList.Clear();
					bool flag11 = this.firstVisibleIndex != firstVisibleIndex;
					if (flag11)
					{
						this.contentPadding = this.GetContentHeightForIndex(this.firstVisibleIndex - 1);
						this.UpdateAnchor();
					}
					this.ScheduleScrollDirectionReset();
					this.m_ForcedLastVisibleItem = -1;
					this.m_CollectionView.SaveViewData();
				}
			}
		}

		private void UpdateAnchor()
		{
			this.anchoredIndex = this.firstVisibleIndex;
			this.anchorOffset = base.serializedData.scrollOffset.y - this.contentPadding;
		}

		private void ScheduleFill()
		{
			bool flag = this.m_ScheduledItem == null;
			if (flag)
			{
				this.m_ScheduledItem = this.m_CollectionView.schedule.Execute(this.m_FillCallback);
			}
			else
			{
				this.m_ScheduledItem.Pause();
				this.m_ScheduledItem.Resume();
			}
		}

		private void ScheduleScroll()
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

		private void ScheduleScrollDirectionReset()
		{
			bool flag = this.m_ScrollResetScheduledItem == null;
			if (flag)
			{
				this.m_ScrollResetScheduledItem = this.m_CollectionView.schedule.Execute(this.m_ScrollResetCallback);
			}
			else
			{
				this.m_ScrollResetScheduledItem.Pause();
				this.m_ScrollResetScheduledItem.Resume();
			}
		}

		private void ResetScroll()
		{
			this.m_ScrollDirection = DynamicHeightVirtualizationController<T>.ScrollDirection.Idle;
			this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.None;
			this.m_ScrollView.UpdateContentViewTransform();
			this.UpdateAnchor();
			this.m_CollectionView.SaveViewData();
		}

		public override int GetIndexFromPosition(Vector2 position)
		{
			int num = 0;
			for (float num2 = 0f; num2 < position.y; num2 += this.GetExpectedItemHeight(num++))
			{
			}
			return num - 1;
		}

		public override float GetExpectedItemHeight(int index)
		{
			int draggedIndex = base.GetDraggedIndex();
			bool flag = draggedIndex >= 0 && index == draggedIndex;
			float num;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				float num2;
				num = (this.m_ItemHeightCache.TryGetValue(index, out num2) ? num2 : this.defaultExpectedHeight);
			}
			return num;
		}

		private int GetFirstVisibleItem(float offset)
		{
			bool flag = offset <= 0f;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				int num2 = -1;
				while (offset > 0f)
				{
					num2++;
					float expectedItemHeight = this.GetExpectedItemHeight(num2);
					offset -= expectedItemHeight;
				}
				num = num2;
			}
			return num;
		}

		public override float GetExpectedContentHeight()
		{
			return this.m_AccumulatedHeight + (float)(base.itemsCount - this.m_ItemHeightCache.Count) * this.defaultExpectedHeight;
		}

		private float GetContentHeightForIndex(int lastIndex)
		{
			bool flag = lastIndex < 0;
			float num;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo;
				bool flag2 = this.m_ContentHeightCache.TryGetValue(lastIndex, out contentHeightCacheInfo);
				if (flag2)
				{
					int draggedIndex = base.GetDraggedIndex();
					bool flag3 = draggedIndex >= 0 && lastIndex >= draggedIndex;
					if (flag3)
					{
						num = contentHeightCacheInfo.sum + (float)(lastIndex - contentHeightCacheInfo.count + 1) * this.defaultExpectedHeight - this.m_DraggedItem.rootElement.layout.height;
					}
					else
					{
						num = contentHeightCacheInfo.sum + (float)(lastIndex - contentHeightCacheInfo.count + 1) * this.defaultExpectedHeight;
					}
				}
				else
				{
					num = this.GetContentHeightForIndex(lastIndex - 1) + this.GetExpectedItemHeight(lastIndex);
				}
			}
			return num;
		}

		private DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo GetCachedContentHeight(int index)
		{
			while (index >= 0)
			{
				DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo;
				bool flag = this.m_ContentHeightCache.TryGetValue(index, out contentHeightCacheInfo);
				if (flag)
				{
					return contentHeightCacheInfo;
				}
				index--;
			}
			return default(DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo);
		}

		private void RegisterItemHeight(int index, float height)
		{
			bool flag = height <= 0f;
			if (!flag)
			{
				float num = this.m_CollectionView.ResolveItemHeight(height);
				float num2;
				bool flag2 = this.m_ItemHeightCache.TryGetValue(index, out num2);
				if (flag2)
				{
					this.m_AccumulatedHeight -= num2;
				}
				this.m_AccumulatedHeight += num;
				this.m_ItemHeightCache[index] = num;
				bool flag3 = index > this.m_HighestCachedIndex;
				if (flag3)
				{
					this.m_HighestCachedIndex = index;
				}
				bool flag4 = num2 == 0f;
				DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo cachedContentHeight = this.GetCachedContentHeight(index - 1);
				this.m_ContentHeightCache[index] = new DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo(cachedContentHeight.sum + num, cachedContentHeight.count + 1);
				foreach (KeyValuePair<int, float> keyValuePair in this.m_ItemHeightCache)
				{
					bool flag5 = keyValuePair.Key > index;
					if (flag5)
					{
						DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo = this.m_ContentHeightCache[keyValuePair.Key];
						this.m_ContentHeightCache[keyValuePair.Key] = new DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo(contentHeightCacheInfo.sum - num2 + num, flag4 ? (contentHeightCacheInfo.count + 1) : contentHeightCacheInfo.count);
					}
				}
			}
		}

		private void UnregisterItemHeight(int index)
		{
			float num;
			bool flag = !this.m_ItemHeightCache.TryGetValue(index, out num);
			if (!flag)
			{
				this.m_AccumulatedHeight -= num;
				this.m_ItemHeightCache.Remove(index);
				this.m_ContentHeightCache.Remove(index);
				int num2 = -1;
				foreach (KeyValuePair<int, float> keyValuePair in this.m_ItemHeightCache)
				{
					bool flag2 = keyValuePair.Key > index;
					if (flag2)
					{
						DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo = this.m_ContentHeightCache[keyValuePair.Key];
						this.m_ContentHeightCache[keyValuePair.Key] = new DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo(contentHeightCacheInfo.sum - num, contentHeightCacheInfo.count - 1);
					}
					bool flag3 = keyValuePair.Key > num2;
					if (flag3)
					{
						num2 = keyValuePair.Key;
					}
				}
				this.m_HighestCachedIndex = num2;
			}
		}

		private void CleanItemHeightCache()
		{
			bool flag = !this.IsIndexOutOfBounds(this.m_HighestCachedIndex);
			if (!flag)
			{
				List<int> list = CollectionPool<List<int>, int>.Get();
				try
				{
					foreach (int num in this.m_ItemHeightCache.Keys)
					{
						bool flag2 = this.IsIndexOutOfBounds(num);
						if (flag2)
						{
							list.Add(num);
						}
					}
					foreach (int num2 in list)
					{
						this.UnregisterItemHeight(num2);
					}
				}
				finally
				{
					CollectionPool<List<int>, int>.Release(list);
				}
				this.m_MinimumItemHeight = -1f;
			}
		}

		private void OnRecycledItemGeometryChanged(ReusableCollectionItem item)
		{
			bool flag = item.index == -1 || item.isDragGhost || float.IsNaN(item.rootElement.layout.height) || item.rootElement.layout.height == 0f;
			if (!flag)
			{
				bool flag2 = this.UpdateRegisteredHeight(item);
				if (flag2)
				{
					this.ApplyScrollViewUpdate(false);
				}
			}
		}

		private bool UpdateRegisteredHeight(ReusableCollectionItem item)
		{
			bool flag = item.index == -1 || item.isDragGhost || float.IsNaN(item.rootElement.layout.height) || item.rootElement.layout.height == 0f;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = item.rootElement.layout.height < this.defaultExpectedHeight;
				if (flag3)
				{
					this.m_MinimumItemHeight = item.rootElement.layout.height;
					this.Resize(this.m_ScrollView.layout.size);
				}
				float num = item.rootElement.layout.height - item.rootElement.resolvedStyle.paddingTop;
				float num2;
				bool flag4 = this.m_ItemHeightCache.TryGetValue(item.index, out num2);
				float num3 = (flag4 ? this.GetExpectedItemHeight(item.index) : this.defaultExpectedHeight);
				bool flag5 = this.m_WaitingCache.Count == 0;
				if (flag5)
				{
					bool flag6 = num > num3;
					if (flag6)
					{
						this.m_StickToBottom = false;
					}
					else
					{
						float num4 = num - num3;
						float num5 = Mathf.Max(0f, this.contentHeight - this.m_ScrollView.contentViewport.layout.height);
						this.m_StickToBottom = num5 > 0f && base.serializedData.scrollOffset.y >= this.m_ScrollView.verticalScroller.highValue + num4;
					}
				}
				bool flag7 = !flag4 || !Mathf.Approximately(num, num2);
				if (flag7)
				{
					this.RegisterItemHeight(item.index, num);
					this.UpdateScrollViewContainer(num3, num);
					bool flag8 = this.m_WaitingCache.Count == 0;
					if (flag8)
					{
						return true;
					}
				}
				flag2 = this.m_WaitingCache.Remove(item.index) && this.m_WaitingCache.Count == 0;
			}
			return flag2;
		}

		internal override T GetOrMakeItemAtIndex(int activeItemIndex = -1, int scrollViewIndex = -1)
		{
			T orMakeItemAtIndex = base.GetOrMakeItemAtIndex(activeItemIndex, scrollViewIndex);
			orMakeItemAtIndex.onGeometryChanged += this.m_GeometryChangedCallback;
			return orMakeItemAtIndex;
		}

		internal override void ReleaseItem(int activeItemsIndex)
		{
			T t = this.m_ActiveItems[activeItemsIndex];
			t.onGeometryChanged -= this.m_GeometryChangedCallback;
			int index = t.index;
			this.UnregisterItemHeight(index);
			base.ReleaseItem(activeItemsIndex);
			this.m_WaitingCache.Remove(index);
		}

		internal override void StartDragItem(ReusableCollectionItem item)
		{
			this.m_WaitingCache.Remove(item.index);
			base.StartDragItem(item);
			this.m_DraggedItem.onGeometryChanged -= this.m_GeometryChangedCallback;
		}

		internal override void EndDrag(int dropIndex)
		{
			bool flag = this.m_DraggedItem.index < dropIndex;
			int index = this.m_DraggedItem.index;
			int num = (flag ? 1 : (-1));
			float expectedItemHeight = this.GetExpectedItemHeight(index);
			for (int num2 = index; num2 != dropIndex; num2 += num)
			{
				float expectedItemHeight2 = this.GetExpectedItemHeight(num2);
				float expectedItemHeight3 = this.GetExpectedItemHeight(num2 + num);
				bool flag2 = Mathf.Approximately(expectedItemHeight2, expectedItemHeight3);
				if (!flag2)
				{
					this.RegisterItemHeight(num2, expectedItemHeight3);
				}
			}
			this.RegisterItemHeight(flag ? (dropIndex - 1) : dropIndex, expectedItemHeight);
			bool flag3 = this.firstVisibleIndex > this.m_DraggedItem.index;
			if (flag3)
			{
				this.firstVisibleIndex = this.GetFirstVisibleItem(base.serializedData.scrollOffset.y);
				this.UpdateAnchor();
			}
			this.m_DraggedItem.onGeometryChanged += this.m_GeometryChangedCallback;
			base.EndDrag(dropIndex);
		}

		private void HideItem(int activeItemsIndex)
		{
			T t = this.m_ActiveItems[activeItemsIndex];
			t.rootElement.style.display = DisplayStyle.None;
			this.m_WaitingCache.Remove(t.index);
		}

		private void MarkWaitingForLayout(T item)
		{
			bool isDragGhost = item.isDragGhost;
			if (!isDragGhost)
			{
				this.m_WaitingCache.Add(item.index);
				item.rootElement.lastLayout = Rect.zero;
				item.rootElement.MarkDirtyRepaint();
			}
		}

		private bool IsIndexOutOfBounds(int i)
		{
			return this.m_CollectionView.itemsSource == null || i >= base.itemsCount;
		}

		private int m_HighestCachedIndex = -1;

		private readonly Dictionary<int, float> m_ItemHeightCache = new Dictionary<int, float>(32);

		private readonly Dictionary<int, DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo> m_ContentHeightCache = new Dictionary<int, DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo>(32);

		private readonly HashSet<int> m_WaitingCache = new HashSet<int>(32);

		private int m_ForcedFirstVisibleItem = -1;

		private int m_ForcedLastVisibleItem = -1;

		private bool m_StickToBottom;

		private DynamicHeightVirtualizationController<T>.VirtualizationChange m_LastChange;

		private DynamicHeightVirtualizationController<T>.ScrollDirection m_ScrollDirection;

		private Vector2 m_DelayedScrollOffset = Vector2.negativeInfinity;

		private float m_AccumulatedHeight;

		private float m_MinimumItemHeight = -1f;

		private Action m_FillCallback;

		private Action m_ScrollCallback;

		private Action m_ScrollResetCallback;

		private Action<ReusableCollectionItem> m_GeometryChangedCallback;

		private IVisualElementScheduledItem m_ScheduledItem;

		private IVisualElementScheduledItem m_ScrollScheduledItem;

		private IVisualElementScheduledItem m_ScrollResetScheduledItem;

		private Predicate<int> m_IndexOutOfBoundsPredicate;

		private readonly struct ContentHeightCacheInfo
		{
			public ContentHeightCacheInfo(float sum, int count)
			{
				this.sum = sum;
				this.count = count;
			}

			public readonly float sum;

			public readonly int count;
		}

		private enum VirtualizationChange
		{
			None,
			Resize,
			Scroll,
			ForcedScroll
		}

		private enum ScrollDirection
		{
			Idle,
			Up,
			Down
		}
	}
}
