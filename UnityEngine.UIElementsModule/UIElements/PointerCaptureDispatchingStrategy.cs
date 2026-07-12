using System;

namespace UnityEngine.UIElements
{
	internal class PointerCaptureDispatchingStrategy : IEventDispatchingStrategy
	{
		public bool CanDispatchEvent(EventBase evt)
		{
			return evt is IPointerEvent;
		}

		public void DispatchEvent(EventBase evt, IPanel panel)
		{
			IPointerEvent pointerEvent = evt as IPointerEvent;
			bool flag = pointerEvent == null;
			if (!flag)
			{
				IEventHandler capturingElement = panel.GetCapturingElement(pointerEvent.pointerId);
				bool flag2 = capturingElement == null;
				if (!flag2)
				{
					VisualElement visualElement = capturingElement as VisualElement;
					bool flag3 = evt.eventTypeId != EventBase<PointerCaptureOutEvent>.TypeId() && visualElement != null && visualElement.panel == null;
					if (flag3)
					{
						panel.ReleasePointer(pointerEvent.pointerId);
					}
					else
					{
						bool flag4 = evt.target != null && evt.target != capturingElement;
						if (!flag4)
						{
							bool flag5 = panel != null && visualElement != null && visualElement.panel != panel;
							if (!flag5)
							{
								bool flag6 = evt.eventTypeId != EventBase<PointerCaptureEvent>.TypeId() && evt.eventTypeId != EventBase<PointerCaptureOutEvent>.TypeId();
								if (flag6)
								{
									panel.ProcessPointerCapture(pointerEvent.pointerId);
								}
								BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
								bool flag7 = baseVisualElementPanel != null;
								if (flag7)
								{
									IPointerEventInternal pointerEventInternal = pointerEvent as IPointerEventInternal;
									bool flag8 = pointerEventInternal == null || pointerEventInternal.recomputeTopElementUnderPointer;
									bool flag9 = flag8;
									if (flag9)
									{
										baseVisualElementPanel.RecomputeTopElementUnderPointer(pointerEvent.pointerId, pointerEvent.position, evt);
									}
								}
								evt.dispatch = true;
								evt.target = capturingElement;
								evt.skipDisabledElements = false;
								CallbackEventHandler callbackEventHandler = capturingElement as CallbackEventHandler;
								if (callbackEventHandler != null)
								{
									callbackEventHandler.HandleEventAtTargetPhase(evt);
								}
								evt.currentTarget = null;
								evt.propagationPhase = PropagationPhase.None;
								evt.dispatch = false;
								evt.stopDispatch = true;
								evt.propagateToIMGUI = false;
							}
						}
					}
				}
			}
		}
	}
}
