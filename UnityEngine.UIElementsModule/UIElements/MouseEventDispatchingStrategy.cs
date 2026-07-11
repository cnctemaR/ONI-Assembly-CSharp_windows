using System;

namespace UnityEngine.UIElements
{
	internal class MouseEventDispatchingStrategy : IEventDispatchingStrategy
	{
		public bool CanDispatchEvent(EventBase evt)
		{
			return evt is IMouseEvent;
		}

		public void DispatchEvent(EventBase evt, IPanel panel)
		{
			IMouseEvent mouseEvent = evt as IMouseEvent;
			bool flag = mouseEvent == null;
			if (!flag)
			{
				BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
				bool flag2 = true;
				bool flag3 = (IMouseEventInternal)mouseEvent != null;
				if (flag3)
				{
					flag2 = ((IMouseEventInternal)mouseEvent).recomputeTopElementUnderMouse;
				}
				VisualElement visualElement = (flag2 ? ((baseVisualElementPanel != null) ? baseVisualElementPanel.Pick(mouseEvent.mousePosition) : null) : ((baseVisualElementPanel != null) ? baseVisualElementPanel.GetTopElementUnderPointer(PointerId.mousePointerId) : null));
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
				bool flag7 = baseVisualElementPanel != null;
				if (flag7)
				{
					bool flag8 = evt.eventTypeId == EventBase<MouseLeaveWindowEvent>.TypeId() && (evt as MouseLeaveWindowEvent).pressedButtons == 0;
					if (flag8)
					{
						baseVisualElementPanel.ClearCachedElementUnderPointer(evt);
					}
					else
					{
						bool flag9 = flag2;
						if (flag9)
						{
							baseVisualElementPanel.SetElementUnderPointer(visualElement, evt);
						}
					}
				}
				bool flag10 = evt.target != null;
				if (flag10)
				{
					EventDispatchUtilities.PropagateEvent(evt);
				}
				IMGUIContainer imguicontainer = ((baseVisualElementPanel != null) ? baseVisualElementPanel.rootIMGUIContainer : null);
				bool flag11 = !evt.isPropagationStopped && panel != null && evt.imguiEvent != null && imguicontainer != null;
				if (flag11)
				{
					bool flag12 = evt.propagateToIMGUI || evt.eventTypeId == EventBase<MouseEnterWindowEvent>.TypeId() || evt.eventTypeId == EventBase<MouseLeaveWindowEvent>.TypeId() || evt.target == imguicontainer;
					if (flag12)
					{
						evt.skipElements.Add(evt.target);
						EventDispatchUtilities.PropagateToIMGUIContainer(panel.visualTree, evt);
					}
					else
					{
						evt.skipElements.Add(evt.target);
						bool flag13 = !evt.Skip(imguicontainer);
						if (flag13)
						{
							Focusable focusable = evt.target as Focusable;
							bool flag14 = focusable != null && !focusable.focusable && focusable.isIMGUIContainer;
							imguicontainer.SendEventToIMGUI(evt, flag14);
						}
					}
					bool flag15 = evt.imguiEvent.rawType == EventType.Used;
					if (flag15)
					{
						evt.StopPropagation();
					}
				}
				evt.stopDispatch = true;
			}
		}
	}
}
