using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements.HierarchyV2
{
	internal class RecycledItem
	{
		public LinkedListNode<RecycledItem> node { get; set; }

		public VisualElement element
		{
			get
			{
				return this.m_Element;
			}
			private set
			{
				this.m_Element = value;
			}
		}

		public float verticalOffset
		{
			get
			{
				return this.m_Element.resolvedStyle.translate.y;
			}
			set
			{
				Vector3 translate = this.m_Element.resolvedStyle.translate;
				translate.y = value;
				this.m_Element.style.translate = translate;
			}
		}

		public static RecycledItem AllocateItem(VisualElement element, CollectionView parent)
		{
			RecycledItem recycledItem = RecycledItem.s_ItemPool.Get();
			recycledItem.Assign(element, parent);
			recycledItem.node = new LinkedListNode<RecycledItem>(recycledItem);
			return recycledItem;
		}

		public static void Recycle(RecycledItem item)
		{
			RecycledItem.s_ItemPool.Release(item);
		}

		public static void ClearItemPool()
		{
			RecycledItem.s_ItemPool.Clear();
		}

		public void Assign(VisualElement element, CollectionView parent)
		{
			this.m_CollectionView = parent;
			this.renderedHeight = -1f;
			this.element = element;
			this.index = -1;
			element.AddToClassList(BaseVerticalCollectionView.itemUssClassName);
			element.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChange), TrickleDown.NoTrickleDown);
		}

		private void OnSizeChange(GeometryChangedEvent evt)
		{
			this.renderedHeight = evt.newRect.height;
			bool flag = evt.layoutPass < 4;
			if (flag)
			{
				RecycledItem.UpdatePositions(this);
			}
		}

		public static void UpdatePositions(RecycledItem item)
		{
			for (LinkedListNode<RecycledItem> linkedListNode = item.node; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				float num = linkedListNode.Value.renderedHeight;
				bool flag = !float.IsNaN(num) && num > 0f;
				if (flag)
				{
					linkedListNode.Value.UpdatePosition();
					bool flag2 = linkedListNode.Next == null;
					if (flag2)
					{
						linkedListNode.Value.m_CollectionView.ItemPositionUpdated(linkedListNode.Value);
					}
				}
			}
		}

		private void UpdatePosition()
		{
			float num = 0f;
			bool flag = this.node.Previous != null;
			if (flag)
			{
				num = this.node.Previous.Value.verticalOffset + this.node.Previous.Value.renderedHeight;
			}
			bool flag2 = !Mathf.Approximately(num, this.verticalOffset);
			if (flag2)
			{
				this.verticalOffset = num;
			}
		}

		public void DetachElement()
		{
			bool flag = this.element == null;
			if (!flag)
			{
				this.element.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnSizeChange), TrickleDown.NoTrickleDown);
				this.element.RemoveFromClassList(BaseVerticalCollectionView.itemUssClassName);
				this.element.RemoveFromHierarchy();
				this.SetSelected(false);
				this.index = -1;
			}
		}

		private void DestroyElement()
		{
			this.m_CollectionView.OnDestroyItem(this);
		}

		public void SetSelected(bool selected)
		{
			bool flag = this.element != null;
			if (flag)
			{
				if (selected)
				{
					this.element.AddToClassList(BaseVerticalCollectionView.itemSelectedVariantUssClassName);
					this.element.pseudoStates |= PseudoStates.Checked;
				}
				else
				{
					this.element.RemoveFromClassList(BaseVerticalCollectionView.itemSelectedVariantUssClassName);
					this.element.pseudoStates &= ~PseudoStates.Checked;
				}
			}
		}

		private static ObjectPool<RecycledItem> s_ItemPool = new ObjectPool<RecycledItem>(() => new RecycledItem(), null, delegate(RecycledItem i)
		{
			i.DetachElement();
		}, delegate(RecycledItem i)
		{
			i.DestroyElement();
		}, true, 10, 10000);

		public int index;

		public float renderedHeight;

		public bool isLastItem;

		public const int k_UndefinedIndex = -1;

		private CollectionView m_CollectionView;

		private VisualElement m_Element;
	}
}
