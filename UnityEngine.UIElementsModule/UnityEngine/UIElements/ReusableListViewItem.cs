using System;

namespace UnityEngine.UIElements
{
	internal class ReusableListViewItem : ReusableCollectionItem
	{
		public override VisualElement rootElement
		{
			get
			{
				return this.m_Container ?? base.bindableElement;
			}
		}

		public void Init(VisualElement item, bool usesAnimatedDragger)
		{
			base.Init(item);
			VisualElement visualElement = new VisualElement
			{
				name = BaseListView.reorderableItemUssClassName
			};
			this.UpdateHierarchy(visualElement, base.bindableElement, usesAnimatedDragger);
		}

		protected void UpdateHierarchy(VisualElement root, VisualElement item, bool usesAnimatedDragger)
		{
			if (usesAnimatedDragger)
			{
				bool flag = this.m_Container != null;
				if (!flag)
				{
					this.m_Container = root;
					this.m_Container.AddToClassList(BaseListView.reorderableItemUssClassName);
					this.m_DragHandle = new VisualElement
					{
						name = BaseListView.reorderableItemHandleUssClassName
					};
					this.m_DragHandle.AddToClassList(BaseListView.reorderableItemHandleUssClassName);
					VisualElement visualElement = new VisualElement
					{
						name = BaseListView.reorderableItemHandleBarUssClassName
					};
					visualElement.AddToClassList(BaseListView.reorderableItemHandleBarUssClassName);
					this.m_DragHandle.Add(visualElement);
					VisualElement visualElement2 = new VisualElement
					{
						name = BaseListView.reorderableItemHandleBarUssClassName
					};
					visualElement2.AddToClassList(BaseListView.reorderableItemHandleBarUssClassName);
					this.m_DragHandle.Add(visualElement2);
					this.m_ItemContainer = new VisualElement
					{
						name = BaseListView.reorderableItemContainerUssClassName
					};
					this.m_ItemContainer.AddToClassList(BaseListView.reorderableItemContainerUssClassName);
					this.m_ItemContainer.Add(item);
					this.m_Container.Add(this.m_DragHandle);
					this.m_Container.Add(this.m_ItemContainer);
				}
			}
			else
			{
				bool flag2 = this.m_Container == null;
				if (!flag2)
				{
					this.m_Container.RemoveFromHierarchy();
					this.m_Container = null;
				}
			}
		}

		public void UpdateDragHandle(bool needsDragHandle)
		{
			if (needsDragHandle)
			{
				bool flag = this.m_DragHandle.parent == null;
				if (flag)
				{
					this.rootElement.Insert(0, this.m_DragHandle);
					this.rootElement.AddToClassList(BaseListView.reorderableItemUssClassName);
				}
			}
			else
			{
				VisualElement dragHandle = this.m_DragHandle;
				bool flag2 = ((dragHandle != null) ? dragHandle.parent : null) != null;
				if (flag2)
				{
					this.m_DragHandle.RemoveFromHierarchy();
					this.rootElement.RemoveFromClassList(BaseListView.reorderableItemUssClassName);
				}
			}
		}

		public void SetDragHandleEnabled(bool enabled)
		{
			bool flag = this.m_DragHandle != null;
			if (flag)
			{
				this.m_DragHandle.SetEnabled(enabled);
				this.m_DragHandle.tooltip = (enabled ? null : ReusableListViewItem.k_SortingDisablesReorderingTooltip);
			}
		}

		public override void PreAttachElement()
		{
			base.PreAttachElement();
			this.rootElement.AddToClassList(BaseListView.itemUssClassName);
		}

		public override void DetachElement()
		{
			base.DetachElement();
			this.rootElement.RemoveFromClassList(BaseListView.itemUssClassName);
		}

		public override void SetDragGhost(bool dragGhost)
		{
			base.SetDragGhost(dragGhost);
			bool flag = this.m_DragHandle != null;
			if (flag)
			{
				this.m_DragHandle.EnableInClassList("unity-hidden", base.isDragGhost);
			}
		}

		protected override void OnGeometryChanged(GeometryChangedEvent evt)
		{
			base.OnGeometryChanged(evt);
			VisualElement itemContainer = this.m_ItemContainer;
			if (itemContainer != null)
			{
				itemContainer.UpdateWorldTransform();
			}
		}

		private static readonly string k_SortingDisablesReorderingTooltip = "Reordering is disabled when the collection is being sorted.";

		private VisualElement m_Container;

		private VisualElement m_DragHandle;

		private VisualElement m_ItemContainer;
	}
}
