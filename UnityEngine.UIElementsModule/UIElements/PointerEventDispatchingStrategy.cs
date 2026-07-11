using System;

namespace UnityEngine.UIElements
{
	internal class PointerEventDispatchingStrategy : IEventDispatchingStrategy
	{
		public bool CanDispatchEvent(EventBase evt)
		{
			return evt is IPointerEvent;
		}

		public virtual void DispatchEvent(EventBase evt, IPanel panel)
		{
			IPointerEvent pointerEvent = evt as IPointerEvent;
			bool flag = pointerEvent == null;
			if (!flag)
			{
				BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
				bool flag2 = true;
				bool flag3 = evt is IPointerEventInternal;
				if (flag3)
				{
					flag2 = ((IPointerEventInternal)pointerEvent).recomputeTopElementUnderPointer;
				}
				VisualElement visualElement = (flag2 ? ((baseVisualElementPanel != null) ? baseVisualElementPanel.Pick(pointerEvent.position) : null) : ((baseVisualElementPanel != null) ? baseVisualElementPanel.GetTopElementUnderPointer(pointerEvent.pointerId) : null));
				bool flag4 = evt.target == null && visualElement != null;
				if (flag4)
				{
					evt.propagateToIMGUI = false;
					evt.target = visualElement;
				}
				else
				{
					bool flag5 = evt.target == null && visualElement == null;
					if (flag5)
					{
						evt.target = ((panel != null) ? panel.visualTree : null);
					}
					else
					{
						bool flag6 = evt.target != null;
						if (flag6)
						{
							evt.propagateToIMGUI = false;
						}
					}
				}
				bool flag7 = baseVisualElementPanel != null && flag2;
				if (flag7)
				{
					baseVisualElementPanel.SetElementUnderPointer(visualElement, evt);
				}
				bool flag8 = evt.target != null;
				if (flag8)
				{
					EventDispatchUtilities.PropagateEvent(evt);
				}
				evt.stopDispatch = true;
			}
		}
	}
}
