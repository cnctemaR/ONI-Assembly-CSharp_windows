using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal abstract class CollectionVirtualizationController
	{
		public abstract int firstVisibleIndex { get; protected set; }

		public abstract int visibleItemCount { get; }

		protected CollectionVirtualizationController(ScrollView scrollView)
		{
			this.m_ScrollView = scrollView;
		}

		public abstract void Refresh(bool rebuild);

		public abstract void ScrollToItem(int id);

		public abstract void Resize(Vector2 size);

		public abstract void OnScroll(Vector2 offset);

		public abstract int GetIndexFromPosition(Vector2 position);

		public abstract float GetExpectedItemHeight(int index);

		public abstract float GetExpectedContentHeight();

		public abstract void OnFocus(VisualElement leafTarget);

		public abstract void OnBlur(VisualElement willFocus);

		public abstract void UpdateBackground();

		public abstract IEnumerable<ReusableCollectionItem> activeItems { get; }

		internal abstract void StartDragItem(ReusableCollectionItem item);

		internal abstract void EndDrag(int dropIndex);

		protected readonly ScrollView m_ScrollView;
	}
}
