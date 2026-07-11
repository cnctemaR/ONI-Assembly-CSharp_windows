using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class EventDispatcher : IEventDispatcher
	{
		private void DispatchEnterLeave(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, Func<EventBase> getEnterEventFunc, Func<EventBase> getLeaveEventFunc)
		{
			if (previousTopElementUnderMouse != currentTopElementUnderMouse)
			{
				int i = 0;
				VisualElement visualElement;
				for (visualElement = previousTopElementUnderMouse; visualElement != null; visualElement = visualElement.shadow.parent)
				{
					i++;
				}
				int j = 0;
				VisualElement visualElement2;
				for (visualElement2 = currentTopElementUnderMouse; visualElement2 != null; visualElement2 = visualElement2.shadow.parent)
				{
					j++;
				}
				visualElement = previousTopElementUnderMouse;
				visualElement2 = currentTopElementUnderMouse;
				while (i > j)
				{
					using (EventBase eventBase = getLeaveEventFunc())
					{
						eventBase.target = visualElement;
						this.DispatchEvent(eventBase, visualElement.panel);
					}
					i--;
					visualElement = visualElement.shadow.parent;
				}
				List<VisualElement> list = VisualElementListPool.Get(j);
				while (j > i)
				{
					list.Add(visualElement2);
					j--;
					visualElement2 = visualElement2.shadow.parent;
				}
				while (visualElement != visualElement2)
				{
					using (EventBase eventBase2 = getLeaveEventFunc())
					{
						eventBase2.target = visualElement;
						this.DispatchEvent(eventBase2, visualElement.panel);
					}
					list.Add(visualElement2);
					visualElement = visualElement.shadow.parent;
					visualElement2 = visualElement2.shadow.parent;
				}
				for (int k = list.Count - 1; k >= 0; k--)
				{
					using (EventBase eventBase3 = getEnterEventFunc())
					{
						eventBase3.target = list[k];
						this.DispatchEvent(eventBase3, list[k].panel);
					}
				}
				VisualElementListPool.Release(list);
			}
		}

		private void DispatchDragEnterDragLeave(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent)
		{
			this.DispatchEnterLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, () => MouseEventBase<DragEnterEvent>.GetPooled(triggerEvent), () => MouseEventBase<DragLeaveEvent>.GetPooled(triggerEvent));
		}

		private void DispatchMouseEnterMouseLeave(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent)
		{
			this.DispatchEnterLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, () => MouseEventBase<MouseEnterEvent>.GetPooled(triggerEvent), () => MouseEventBase<MouseLeaveEvent>.GetPooled(triggerEvent));
		}

		private void DispatchMouseOverMouseOut(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent)
		{
			if (previousTopElementUnderMouse != currentTopElementUnderMouse)
			{
				if (previousTopElementUnderMouse != null)
				{
					using (MouseOutEvent pooled = MouseEventBase<MouseOutEvent>.GetPooled(triggerEvent))
					{
						pooled.target = previousTopElementUnderMouse;
						this.DispatchEvent(pooled, previousTopElementUnderMouse.panel);
					}
				}
				if (currentTopElementUnderMouse != null)
				{
					using (MouseOverEvent pooled2 = MouseEventBase<MouseOverEvent>.GetPooled(triggerEvent))
					{
						pooled2.target = currentTopElementUnderMouse;
						this.DispatchEvent(pooled2, currentTopElementUnderMouse.panel);
					}
				}
			}
		}

		public void DispatchEvent(EventBase evt, IPanel panel)
		{
			if (evt.GetEventTypeId() == EventBase<IMGUIEvent>.TypeId())
			{
				Event imguiEvent = evt.imguiEvent;
				if (imguiEvent.type == EventType.Repaint)
				{
					return;
				}
			}
			IMouseEvent mouseEvent = evt as IMouseEvent;
			IMouseEventInternal mouseEventInternal = evt as IMouseEventInternal;
			if (mouseEvent != null && mouseEventInternal != null && mouseEventInternal.hasUnderlyingPhysicalEvent)
			{
				this.m_LastMousePosition = mouseEvent.mousePosition;
			}
			bool flag = false;
			VisualElement visualElement = MouseCaptureController.mouseCapture as VisualElement;
			if (evt.GetEventTypeId() != EventBase<MouseCaptureOutEvent>.TypeId() && visualElement != null && visualElement.panel == null)
			{
				Event imguiEvent2 = evt.imguiEvent;
				Debug.Log(string.Format("Capture has no panel, forcing removal (capture={0} eventType={1})", MouseCaptureController.mouseCapture, (imguiEvent2 == null) ? "null" : imguiEvent2.type.ToString()));
				MouseCaptureController.ReleaseMouseCapture();
			}
			bool flag2 = false;
			bool flag3 = false;
			if (MouseCaptureController.mouseCapture != null)
			{
				if (evt.imguiEvent != null && evt.target == null)
				{
					flag2 = true;
					flag3 = false;
				}
				if (mouseEvent != null && (evt.target == null || evt.target == MouseCaptureController.mouseCapture))
				{
					flag2 = true;
					flag3 = true;
				}
				if (panel != null)
				{
					if (visualElement != null && visualElement.panel.contextType != panel.contextType)
					{
						flag2 = false;
						flag3 = false;
					}
				}
			}
			evt.skipElement = null;
			if (flag2)
			{
				IEventHandler mouseCapture = MouseCaptureController.mouseCapture;
				flag = true;
				evt.dispatch = true;
				evt.target = MouseCaptureController.mouseCapture;
				evt.currentTarget = MouseCaptureController.mouseCapture;
				evt.propagationPhase = PropagationPhase.AtTarget;
				MouseCaptureController.mouseCapture.HandleEvent(evt);
				if (!flag3)
				{
					evt.target = null;
				}
				evt.currentTarget = null;
				evt.propagationPhase = PropagationPhase.None;
				evt.dispatch = false;
				evt.skipElement = mouseCapture;
			}
			if (!flag3 && !evt.isPropagationStopped)
			{
				if (evt is IKeyboardEvent && panel != null)
				{
					flag = true;
					if (panel.focusController.focusedElement != null)
					{
						IMGUIContainer imguicontainer = panel.focusController.focusedElement as IMGUIContainer;
						if (imguicontainer != null)
						{
							if (imguicontainer != evt.skipElement && imguicontainer.HandleIMGUIEvent(evt.imguiEvent))
							{
								evt.StopPropagation();
								evt.PreventDefault();
							}
						}
						else
						{
							evt.target = panel.focusController.focusedElement;
							EventDispatcher.PropagateEvent(evt);
						}
					}
					else
					{
						evt.target = panel.visualTree;
						EventDispatcher.PropagateEvent(evt);
						if (!evt.isPropagationStopped)
						{
							EventDispatcher.PropagateToIMGUIContainer(panel.visualTree, evt);
						}
					}
				}
				else if (mouseEvent != null)
				{
					if (evt.GetEventTypeId() == EventBase<MouseLeaveWindowEvent>.TypeId())
					{
						VisualElement topElementUnderMouse = this.m_TopElementUnderMouse;
						this.m_TopElementUnderMouse = null;
						this.DispatchMouseEnterMouseLeave(topElementUnderMouse, this.m_TopElementUnderMouse, mouseEvent);
						this.DispatchMouseOverMouseOut(topElementUnderMouse, this.m_TopElementUnderMouse, mouseEvent);
					}
					else if (evt.GetEventTypeId() == EventBase<DragExitedEvent>.TypeId())
					{
						VisualElement topElementUnderMouse2 = this.m_TopElementUnderMouse;
						this.m_TopElementUnderMouse = null;
						this.DispatchDragEnterDragLeave(topElementUnderMouse2, this.m_TopElementUnderMouse, mouseEvent);
					}
					else
					{
						VisualElement topElementUnderMouse3 = this.m_TopElementUnderMouse;
						if (evt.target == null && panel != null)
						{
							this.m_TopElementUnderMouse = panel.Pick(mouseEvent.mousePosition);
							evt.target = this.m_TopElementUnderMouse;
						}
						if (evt.target != null)
						{
							flag = true;
							EventDispatcher.PropagateEvent(evt);
						}
						if (evt.GetEventTypeId() == EventBase<MouseMoveEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseDownEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseUpEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseEnterWindowEvent>.TypeId() || evt.GetEventTypeId() == EventBase<WheelEvent>.TypeId())
						{
							this.DispatchMouseEnterMouseLeave(topElementUnderMouse3, this.m_TopElementUnderMouse, mouseEvent);
							this.DispatchMouseOverMouseOut(topElementUnderMouse3, this.m_TopElementUnderMouse, mouseEvent);
						}
						else if (evt.GetEventTypeId() == EventBase<DragUpdatedEvent>.TypeId())
						{
							this.DispatchDragEnterDragLeave(topElementUnderMouse3, this.m_TopElementUnderMouse, mouseEvent);
						}
					}
				}
				else if (panel != null && evt is ICommandEvent)
				{
					IMGUIContainer imguicontainer2 = panel.focusController.focusedElement as IMGUIContainer;
					flag = true;
					if (imguicontainer2 != null)
					{
						if (imguicontainer2 != evt.skipElement && imguicontainer2.HandleIMGUIEvent(evt.imguiEvent))
						{
							evt.StopPropagation();
							evt.PreventDefault();
						}
					}
					else if (panel.focusController.focusedElement != null)
					{
						evt.target = panel.focusController.focusedElement;
						EventDispatcher.PropagateEvent(evt);
					}
					else
					{
						EventDispatcher.PropagateToIMGUIContainer(panel.visualTree, evt);
					}
				}
				else if (evt is IPropagatableEvent || evt is IFocusEvent || evt is IChangeEvent || evt.GetEventTypeId() == EventBase<InputEvent>.TypeId() || evt.GetEventTypeId() == EventBase<GeometryChangedEvent>.TypeId())
				{
					Debug.Assert(evt.target != null);
					flag = true;
					EventDispatcher.PropagateEvent(evt);
				}
			}
			if (!flag3 && !evt.isPropagationStopped && panel != null)
			{
				Event imguiEvent3 = evt.imguiEvent;
				if (!flag || (imguiEvent3 != null && imguiEvent3.type == EventType.Used) || evt.GetEventTypeId() == EventBase<MouseEnterWindowEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseLeaveWindowEvent>.TypeId())
				{
					EventDispatcher.PropagateToIMGUIContainer(panel.visualTree, evt);
				}
			}
			if (evt.target == null && panel != null)
			{
				evt.target = panel.visualTree;
			}
			EventDispatcher.ExecuteDefaultAction(evt);
		}

		internal void UpdateElementUnderMouse(IPanel panel)
		{
			Vector2 vector = panel.visualTree.WorldToLocal(this.m_LastMousePosition);
			VisualElement topElementUnderMouse = this.m_TopElementUnderMouse;
			this.m_TopElementUnderMouse = panel.Pick(vector);
			this.DispatchMouseEnterMouseLeave(topElementUnderMouse, this.m_TopElementUnderMouse, null);
			this.DispatchMouseOverMouseOut(topElementUnderMouse, this.m_TopElementUnderMouse, null);
		}

		private static void PropagateToIMGUIContainer(VisualElement root, EventBase evt)
		{
			if (evt.imguiEvent != null)
			{
				IMGUIContainer imguicontainer = root as IMGUIContainer;
				if (imguicontainer != null && (evt.imguiEvent.type == EventType.Used || root != evt.skipElement))
				{
					if (imguicontainer.HandleIMGUIEvent(evt.imguiEvent))
					{
						evt.StopPropagation();
						evt.PreventDefault();
					}
				}
				else if (root != null)
				{
					for (int i = 0; i < root.shadow.childCount; i++)
					{
						EventDispatcher.PropagateToIMGUIContainer(root.shadow[i], evt);
						if (evt.isPropagationStopped)
						{
							break;
						}
					}
				}
			}
		}

		private static void PropagateEvent(EventBase evt)
		{
			if (!evt.dispatch)
			{
				EventDispatcher.PropagationPaths.Type type = ((!evt.capturable) ? EventDispatcher.PropagationPaths.Type.None : EventDispatcher.PropagationPaths.Type.Capture);
				type |= ((!evt.bubbles) ? EventDispatcher.PropagationPaths.Type.None : EventDispatcher.PropagationPaths.Type.BubbleUp);
				using (EventDispatcher.PropagationPaths propagationPaths = EventDispatcher.BuildPropagationPath(evt.target as VisualElement, type))
				{
					evt.dispatch = true;
					if (evt.capturable && propagationPaths != null && propagationPaths.capturePath.Count > 0)
					{
						evt.propagationPhase = PropagationPhase.Capture;
						for (int i = propagationPaths.capturePath.Count - 1; i >= 0; i--)
						{
							if (evt.isPropagationStopped)
							{
								break;
							}
							if (propagationPaths.capturePath[i] != evt.skipElement)
							{
								evt.currentTarget = propagationPaths.capturePath[i];
								evt.currentTarget.HandleEvent(evt);
							}
						}
					}
					if (evt.target != evt.skipElement)
					{
						evt.propagationPhase = PropagationPhase.AtTarget;
						evt.currentTarget = evt.target;
						evt.currentTarget.HandleEvent(evt);
					}
					if (evt.bubbles && propagationPaths != null && propagationPaths.bubblePath.Count > 0)
					{
						evt.propagationPhase = PropagationPhase.BubbleUp;
						foreach (VisualElement visualElement in propagationPaths.bubblePath)
						{
							if (evt.isPropagationStopped)
							{
								break;
							}
							if (visualElement != evt.skipElement)
							{
								evt.currentTarget = visualElement;
								evt.currentTarget.HandleEvent(evt);
							}
						}
					}
					evt.dispatch = false;
					evt.propagationPhase = PropagationPhase.None;
					evt.currentTarget = null;
				}
			}
		}

		private static void ExecuteDefaultAction(EventBase evt)
		{
			if (evt.target != null)
			{
				evt.dispatch = true;
				evt.currentTarget = evt.target;
				evt.propagationPhase = PropagationPhase.DefaultAction;
				evt.currentTarget.HandleEvent(evt);
				evt.propagationPhase = PropagationPhase.None;
				evt.currentTarget = null;
				evt.dispatch = false;
			}
		}

		private static EventDispatcher.PropagationPaths BuildPropagationPath(VisualElement elem, EventDispatcher.PropagationPaths.Type pathTypesRequested)
		{
			EventDispatcher.PropagationPaths propagationPaths;
			if (elem == null || pathTypesRequested == EventDispatcher.PropagationPaths.Type.None)
			{
				propagationPaths = null;
			}
			else
			{
				EventDispatcher.PropagationPaths propagationPaths2 = EventDispatcher.PropagationPathsPool.Acquire();
				while (elem.shadow.parent != null)
				{
					if (elem.shadow.parent.enabledInHierarchy)
					{
						if ((pathTypesRequested & EventDispatcher.PropagationPaths.Type.Capture) == EventDispatcher.PropagationPaths.Type.Capture && elem.shadow.parent.HasCaptureHandlers())
						{
							propagationPaths2.capturePath.Add(elem.shadow.parent);
						}
						if ((pathTypesRequested & EventDispatcher.PropagationPaths.Type.BubbleUp) == EventDispatcher.PropagationPaths.Type.BubbleUp && elem.shadow.parent.HasBubbleHandlers())
						{
							propagationPaths2.bubblePath.Add(elem.shadow.parent);
						}
					}
					elem = elem.shadow.parent;
				}
				propagationPaths = propagationPaths2;
			}
			return propagationPaths;
		}

		private VisualElement m_TopElementUnderMouse;

		private Vector2 m_LastMousePosition;

		private const int k_DefaultPropagationDepth = 16;

		private class PropagationPaths : IDisposable
		{
			public PropagationPaths(int initialSize)
			{
				this.capturePath = new List<VisualElement>(initialSize);
				this.bubblePath = new List<VisualElement>(initialSize);
			}

			public void Dispose()
			{
				EventDispatcher.PropagationPathsPool.Release(this);
			}

			public void Clear()
			{
				this.bubblePath.Clear();
				this.capturePath.Clear();
			}

			public readonly List<VisualElement> capturePath;

			public readonly List<VisualElement> bubblePath;

			[Flags]
			public enum Type
			{
				None = 0,
				Capture = 1,
				BubbleUp = 2
			}
		}

		private static class PropagationPathsPool
		{
			public static EventDispatcher.PropagationPaths Acquire()
			{
				EventDispatcher.PropagationPaths propagationPaths2;
				if (EventDispatcher.PropagationPathsPool.s_Available.Count != 0)
				{
					EventDispatcher.PropagationPaths propagationPaths = EventDispatcher.PropagationPathsPool.s_Available[0];
					EventDispatcher.PropagationPathsPool.s_Available.RemoveAt(0);
					propagationPaths2 = propagationPaths;
				}
				else
				{
					EventDispatcher.PropagationPaths propagationPaths3 = new EventDispatcher.PropagationPaths(16);
					propagationPaths2 = propagationPaths3;
				}
				return propagationPaths2;
			}

			public static void Release(EventDispatcher.PropagationPaths po)
			{
				po.Clear();
				EventDispatcher.PropagationPathsPool.s_Available.Add(po);
			}

			private static readonly List<EventDispatcher.PropagationPaths> s_Available = new List<EventDispatcher.PropagationPaths>();
		}
	}
}
