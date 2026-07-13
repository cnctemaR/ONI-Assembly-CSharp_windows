using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class FixedHeightVirtualizationController<T> : VerticalVirtualizationController<T> where T : ReusableCollectionItem, new()
	{
		private float resolvedItemHeight
		{
			get
			{
				return this.m_CollectionView.ResolveItemHeight(-1f);
			}
		}

		protected override bool VisibleItemPredicate(T i)
		{
			return true;
		}

		public FixedHeightVirtualizationController(BaseVerticalCollectionView collectionView)
			: base(collectionView)
		{
		}

		public override int GetIndexFromPosition(Vector2 position)
		{
			return (int)(position.y / this.resolvedItemHeight);
		}

		public override float GetExpectedItemHeight(int index)
		{
			return this.resolvedItemHeight;
		}

		public override float GetExpectedContentHeight()
		{
			return (float)base.itemsCount * this.resolvedItemHeight;
		}

		public override void ScrollToItem(int index)
		{
			bool flag = this.visibleItemCount == 0 || index < -1;
			if (!flag)
			{
				float resolvedItemHeight = this.resolvedItemHeight;
				bool flag2 = index == -1;
				if (flag2)
				{
					int num = (int)(base.lastHeight / resolvedItemHeight);
					bool flag3 = base.itemsCount < num;
					if (flag3)
					{
						this.m_ScrollView.scrollOffset = new Vector2(0f, 0f);
					}
					else
					{
						this.m_ScrollView.scrollOffset = new Vector2(0f, (float)(base.itemsCount + 1) * resolvedItemHeight);
					}
				}
				else
				{
					bool flag4 = this.firstVisibleIndex >= index;
					if (flag4)
					{
						this.m_ScrollView.scrollOffset = Vector2.up * (resolvedItemHeight * (float)index);
					}
					else
					{
						int num2 = (int)(base.lastHeight / resolvedItemHeight);
						bool flag5 = index < this.firstVisibleIndex + num2;
						if (!flag5)
						{
							int num3 = index - num2 + 1;
							float num4 = resolvedItemHeight - (base.lastHeight - (float)num2 * resolvedItemHeight);
							float num5 = resolvedItemHeight * (float)num3 + num4;
							this.m_ScrollView.scrollOffset = new Vector2(this.m_ScrollView.scrollOffset.x, num5);
						}
					}
				}
			}
		}

		public override void Resize(Vector2 size)
		{
			float expectedContentHeight = this.GetExpectedContentHeight();
			this.m_ScrollView.contentContainer.style.height = expectedContentHeight;
			float num = Mathf.Max(0f, expectedContentHeight - this.m_ScrollView.contentViewport.layout.height);
			float num2 = Mathf.Min(base.serializedData.scrollOffset.y, num);
			this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num);
			this.m_ScrollView.verticalScroller.slider.SetValueWithoutNotify(num2);
			int num3 = 0;
			float num4 = size.y / this.resolvedItemHeight;
			bool flag = num4 > 0f;
			if (flag)
			{
				num3 = (int)num4 + 2;
			}
			int num5 = Mathf.Min(num3, base.itemsCount);
			bool flag2 = this.visibleItemCount != num5;
			if (flag2)
			{
				int visibleItemCount = this.visibleItemCount;
				bool flag3 = this.visibleItemCount > num5;
				if (flag3)
				{
					int num6 = visibleItemCount - num5;
					for (int i = 0; i < num6; i++)
					{
						int num7 = this.m_ActiveItems.Count - 1;
						this.ReleaseItem(num7);
					}
				}
				else
				{
					int num8 = num5 - this.visibleItemCount;
					for (int j = 0; j < num8; j++)
					{
						int num9 = j + this.firstVisibleIndex + visibleItemCount;
						T orMakeItemAtIndex = this.GetOrMakeItemAtIndex(-1, -1);
						base.Setup(orMakeItemAtIndex, num9);
					}
				}
			}
			this.OnScroll(new Vector2(0f, num2));
		}

		public override void OnScroll(Vector2 scrollOffset)
		{
			float num = Mathf.Max(0f, scrollOffset.y);
			float resolvedItemHeight = this.resolvedItemHeight;
			int num2 = (int)(num / resolvedItemHeight);
			this.m_ScrollView.contentContainer.style.paddingTop = (float)num2 * resolvedItemHeight;
			this.m_ScrollView.contentContainer.style.height = (float)base.itemsCount * resolvedItemHeight;
			base.serializedData.scrollOffset.y = scrollOffset.y;
			bool flag = num2 != this.firstVisibleIndex;
			if (flag)
			{
				this.firstVisibleIndex = num2;
				bool flag2 = this.m_ActiveItems.Count > 0;
				if (flag2)
				{
					bool flag3 = this.firstVisibleIndex < this.m_ActiveItems[0].index;
					if (flag3)
					{
						int num3 = this.m_ActiveItems[0].index - this.firstVisibleIndex;
						List<T> scrollInsertionList = this.m_ScrollInsertionList;
						int num4 = 0;
						while (num4 < num3 && this.m_ActiveItems.Count > 0)
						{
							List<T> activeItems = this.m_ActiveItems;
							T t = activeItems[activeItems.Count - 1];
							scrollInsertionList.Add(t);
							this.m_ActiveItems.RemoveAt(this.m_ActiveItems.Count - 1);
							t.rootElement.SendToBack();
							num4++;
						}
						this.m_ActiveItems.InsertRange(0, scrollInsertionList);
						this.m_ScrollInsertionList.Clear();
					}
					else
					{
						int firstVisibleIndex = this.firstVisibleIndex;
						List<T> activeItems2 = this.m_ActiveItems;
						bool flag4 = firstVisibleIndex < activeItems2[activeItems2.Count - 1].index;
						if (flag4)
						{
							List<T> scrollInsertionList2 = this.m_ScrollInsertionList;
							int num5 = 0;
							while (this.firstVisibleIndex > this.m_ActiveItems[num5].index)
							{
								T t2 = this.m_ActiveItems[num5];
								scrollInsertionList2.Add(t2);
								num5++;
								t2.rootElement.BringToFront();
							}
							this.m_ActiveItems.RemoveRange(0, num5);
							this.m_ActiveItems.AddRange(scrollInsertionList2);
							scrollInsertionList2.Clear();
						}
					}
					for (int i = 0; i < this.m_ActiveItems.Count; i++)
					{
						int num6 = i + this.firstVisibleIndex;
						base.Setup(this.m_ActiveItems[i], num6);
					}
				}
			}
		}

		internal override T GetOrMakeItemAtIndex(int activeItemIndex = -1, int scrollViewIndex = -1)
		{
			T orMakeItemAtIndex = base.GetOrMakeItemAtIndex(activeItemIndex, scrollViewIndex);
			orMakeItemAtIndex.rootElement.style.height = this.resolvedItemHeight;
			return orMakeItemAtIndex;
		}

		internal override void EndDrag(int dropIndex)
		{
			this.m_DraggedItem.rootElement.style.height = this.resolvedItemHeight;
			bool flag = this.firstVisibleIndex > this.m_DraggedItem.index;
			if (flag)
			{
				this.m_ScrollView.verticalScroller.value = base.serializedData.scrollOffset.y - this.resolvedItemHeight;
			}
			base.EndDrag(dropIndex);
		}
	}
}
