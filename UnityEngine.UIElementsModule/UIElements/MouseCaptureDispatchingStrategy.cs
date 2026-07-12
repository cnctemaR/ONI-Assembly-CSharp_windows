using System;

namespace UnityEngine.UIElements
{
	internal class MouseCaptureDispatchingStrategy : IEventDispatchingStrategy
	{
		public bool CanDispatchEvent(EventBase evt)
		{
			return evt is IMouseEvent || evt.imguiEvent != null;
		}

		public void DispatchEvent(EventBase evt, IPanel panel)
		{
			MouseCaptureDispatchingStrategy.EventBehavior eventBehavior = MouseCaptureDispatchingStrategy.EventBehavior.None;
			IEventHandler eventHandler = ((panel != null) ? panel.GetCapturingElement(PointerId.mousePointerId) : null);
			bool flag = eventHandler == null;
			if (!flag)
			{
				VisualElement visualElement = eventHandler as VisualElement;
				bool flag2 = evt.eventTypeId != EventBase<MouseCaptureOutEvent>.TypeId() && visualElement != null && visualElement.panel == null;
				if (flag2)
				{
					visualElement.ReleaseMouse();
				}
				else
				{
					bool flag3 = panel != null && visualElement != null && visualElement.panel != panel;
					if (!flag3)
					{
						IMouseEvent mouseEvent = evt as IMouseEvent;
						bool flag4 = mouseEvent != null && (evt.target == null || evt.target == eventHandler);
						if (flag4)
						{
							eventBehavior = MouseCaptureDispatchingStrategy.EventBehavior.IsCapturable;
							eventBehavior |= MouseCaptureDispatchingStrategy.EventBehavior.IsSentExclusivelyToCapturingElement;
						}
						else
						{
							bool flag5 = evt.imguiEvent != null && evt.target == null;
							if (flag5)
							{
								eventBehavior = MouseCaptureDispatchingStrategy.EventBehavior.IsCapturable;
							}
						}
						bool flag6 = evt.eventTypeId == EventBase<MouseEnterWindowEvent>.TypeId() || evt.eventTypeId == EventBase<MouseLeaveWindowEvent>.TypeId() || evt.eventTypeId == EventBase<WheelEvent>.TypeId();
						if (flag6)
						{
							eventBehavior = MouseCaptureDispatchingStrategy.EventBehavior.None;
						}
						bool flag7 = (eventBehavior & MouseCaptureDispatchingStrategy.EventBehavior.IsCapturable) == MouseCaptureDispatchingStrategy.EventBehavior.IsCapturable;
						if (flag7)
						{
							BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
							bool flag8 = mouseEvent != null && baseVisualElementPanel != null;
							if (flag8)
							{
								IMouseEventInternal mouseEventInternal = mouseEvent as IMouseEventInternal;
								bool flag9 = mouseEventInternal == null || mouseEventInternal.recomputeTopElementUnderMouse;
								bool flag10 = flag9;
								if (flag10)
								{
									baseVisualElementPanel.RecomputeTopElementUnderPointer(PointerId.mousePointerId, mouseEvent.mousePosition, evt);
								}
							}
							evt.dispatch = true;
							evt.target = eventHandler;
							bool skipDisabledElements = evt.skipDisabledElements;
							evt.skipDisabledElements = false;
							CallbackEventHandler callbackEventHandler = eventHandler as CallbackEventHandler;
							if (callbackEventHandler != null)
							{
								callbackEventHandler.HandleEventAtTargetPhase(evt);
							}
							bool flag11 = (eventBehavior & MouseCaptureDispatchingStrategy.EventBehavior.IsSentExclusivelyToCapturingElement) != MouseCaptureDispatchingStrategy.EventBehavior.IsSentExclusivelyToCapturingElement;
							if (flag11)
							{
								evt.target = null;
								evt.skipDisabledElements = skipDisabledElements;
							}
							evt.currentTarget = null;
							evt.propagationPhase = PropagationPhase.None;
							evt.dispatch = false;
							evt.skipElements.Add(eventHandler);
							evt.stopDispatch = (eventBehavior & MouseCaptureDispatchingStrategy.EventBehavior.IsSentExclusivelyToCapturingElement) == MouseCaptureDispatchingStrategy.EventBehavior.IsSentExclusivelyToCapturingElement;
							bool flag12 = evt.target is IMGUIContainer;
							if (flag12)
							{
								evt.propagateToIMGUI = true;
								evt.skipElements.Add(evt.target);
							}
							else
							{
								evt.propagateToIMGUI = false;
							}
						}
					}
				}
			}
		}

		[Flags]
		private enum EventBehavior
		{
			None = 0,
			IsCapturable = 1,
			IsSentExclusivelyToCapturingElement = 2
		}
	}
}
