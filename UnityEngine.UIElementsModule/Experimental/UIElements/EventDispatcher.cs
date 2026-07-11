using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public sealed class EventDispatcher
	{
		private EventDispatcher()
		{
			this.m_Queue = EventDispatcher.k_EventQueuePool.Get();
		}

		internal static EventDispatcher instance
		{
			get
			{
				if (EventDispatcher.s_EventDispatcher == null)
				{
					EventDispatcher.s_EventDispatcher = new EventDispatcher();
				}
				return EventDispatcher.s_EventDispatcher;
			}
		}

		internal static void ClearDispatcher()
		{
			EventDispatcher.s_EventDispatcher = null;
		}

		private bool dispatchImmediately
		{
			get
			{
				return this.m_GateCount == 0U;
			}
		}

		private void DispatchEnterLeave(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, Func<EventBase> getEnterEventFunc, Func<EventBase> getLeaveEventFunc)
		{
			if (previousTopElementUnderMouse != currentTopElementUnderMouse)
			{
				if (previousTopElementUnderMouse != null && previousTopElementUnderMouse.panel == null)
				{
					previousTopElementUnderMouse = null;
				}
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
						visualElement.SendEvent(eventBase);
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
						visualElement.SendEvent(eventBase2);
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
						list[k].SendEvent(eventBase3);
					}
				}
				VisualElementListPool.Release(list);
			}
		}

		private void DispatchDragEnterDragLeave(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent)
		{
			if (triggerEvent != null)
			{
				this.DispatchEnterLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, () => MouseEventBase<DragEnterEvent>.GetPooled(triggerEvent), () => MouseEventBase<DragLeaveEvent>.GetPooled(triggerEvent));
			}
			else
			{
				this.DispatchEnterLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, () => MouseEventBase<DragEnterEvent>.GetPooled(this.m_LastMousePosition), () => MouseEventBase<DragLeaveEvent>.GetPooled(this.m_LastMousePosition));
			}
		}

		private void DispatchMouseEnterMouseLeave(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent)
		{
			if (triggerEvent != null)
			{
				this.DispatchEnterLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, () => MouseEventBase<MouseEnterEvent>.GetPooled(triggerEvent), () => MouseEventBase<MouseLeaveEvent>.GetPooled(triggerEvent));
			}
			else
			{
				this.DispatchEnterLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, () => MouseEventBase<MouseEnterEvent>.GetPooled(this.m_LastMousePosition), () => MouseEventBase<MouseLeaveEvent>.GetPooled(this.m_LastMousePosition));
			}
		}

		private void DispatchMouseOverMouseOut(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent)
		{
			if (previousTopElementUnderMouse != currentTopElementUnderMouse)
			{
				if (previousTopElementUnderMouse != null && previousTopElementUnderMouse.panel != null)
				{
					using (MouseOutEvent mouseOutEvent = ((triggerEvent != null) ? MouseEventBase<MouseOutEvent>.GetPooled(triggerEvent) : MouseEventBase<MouseOutEvent>.GetPooled(this.m_LastMousePosition)))
					{
						mouseOutEvent.target = previousTopElementUnderMouse;
						previousTopElementUnderMouse.SendEvent(mouseOutEvent);
					}
				}
				if (currentTopElementUnderMouse != null)
				{
					using (MouseOverEvent mouseOverEvent = ((triggerEvent != null) ? MouseEventBase<MouseOverEvent>.GetPooled(triggerEvent) : MouseEventBase<MouseOverEvent>.GetPooled(this.m_LastMousePosition)))
					{
						mouseOverEvent.target = currentTopElementUnderMouse;
						currentTopElementUnderMouse.SendEvent(mouseOverEvent);
					}
				}
			}
		}

		private void DispatchEnterLeaveEvents(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, EventBase triggerEvent)
		{
			IMouseEvent mouseEvent = triggerEvent as IMouseEvent;
			if (mouseEvent != null)
			{
				if (triggerEvent.GetEventTypeId() == EventBase<MouseMoveEvent>.TypeId() || triggerEvent.GetEventTypeId() == EventBase<MouseDownEvent>.TypeId() || triggerEvent.GetEventTypeId() == EventBase<MouseUpEvent>.TypeId() || triggerEvent.GetEventTypeId() == EventBase<MouseEnterWindowEvent>.TypeId() || triggerEvent.GetEventTypeId() == EventBase<WheelEvent>.TypeId())
				{
					this.DispatchMouseEnterMouseLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, mouseEvent);
					this.DispatchMouseOverMouseOut(previousTopElementUnderMouse, currentTopElementUnderMouse, mouseEvent);
				}
				else if (triggerEvent.GetEventTypeId() == EventBase<DragUpdatedEvent>.TypeId())
				{
					this.DispatchDragEnterDragLeave(previousTopElementUnderMouse, currentTopElementUnderMouse, mouseEvent);
				}
			}
		}

		internal void Dispatch(EventBase evt, IPanel panel, DispatchMode dispatchMode)
		{
			evt.MarkReceivedByDispatcher();
			if (evt.GetEventTypeId() == EventBase<IMGUIEvent>.TypeId())
			{
				Event imguiEvent = evt.imguiEvent;
				if (imguiEvent.type == EventType.Repaint)
				{
					return;
				}
			}
			if (this.dispatchImmediately || dispatchMode == DispatchMode.Immediate)
			{
				this.ProcessEvent(evt, panel);
			}
			else
			{
				evt.Acquire();
				this.m_Queue.Enqueue(new EventDispatcher.EventRecord
				{
					m_Event = evt,
					m_Panel = panel
				});
			}
		}

		internal void PushDispatcherContext()
		{
			this.m_DispatchContexts.Push(new EventDispatcher.DispatchContext
			{
				m_GateCount = this.m_GateCount,
				m_Queue = this.m_Queue
			});
			this.m_GateCount = 0U;
			this.m_Queue = EventDispatcher.k_EventQueuePool.Get();
		}

		internal void PopDispatcherContext()
		{
			Debug.Assert(this.m_GateCount == 0U, "All gates should have been opened before popping dispatch context.");
			Debug.Assert(this.m_Queue.Count == 0, "Queue should be empty when popping dispatch context.");
			EventDispatcher.k_EventQueuePool.Release(this.m_Queue);
			this.m_GateCount = this.m_DispatchContexts.Peek().m_GateCount;
			this.m_Queue = this.m_DispatchContexts.Peek().m_Queue;
			this.m_DispatchContexts.Pop();
		}

		internal void CloseGate()
		{
			this.m_GateCount += 1U;
		}

		internal void OpenGate()
		{
			Debug.Assert(this.m_GateCount > 0U);
			if (this.m_GateCount > 0U)
			{
				this.m_GateCount -= 1U;
			}
			if (this.m_GateCount == 0U)
			{
				this.ProcessEventQueue();
			}
		}

		private void ProcessEventQueue()
		{
			Queue<EventDispatcher.EventRecord> queue = this.m_Queue;
			this.m_Queue = EventDispatcher.k_EventQueuePool.Get();
			ExitGUIException ex = null;
			try
			{
				while (queue.Count > 0)
				{
					EventDispatcher.EventRecord eventRecord = queue.Dequeue();
					EventBase @event = eventRecord.m_Event;
					IPanel panel = eventRecord.m_Panel;
					try
					{
						this.ProcessEvent(@event, panel);
					}
					catch (ExitGUIException ex2)
					{
						Debug.Assert(ex == null);
						ex = ex2;
					}
					finally
					{
						@event.Dispose();
					}
				}
			}
			finally
			{
				EventDispatcher.k_EventQueuePool.Release(queue);
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		private void ProcessEvent(EventBase evt, IPanel panel)
		{
			using (new EventDispatcher.Gate(this))
			{
				evt.PreDispatch();
				IMouseEvent mouseEvent = evt as IMouseEvent;
				IMouseEventInternal mouseEventInternal = evt as IMouseEventInternal;
				if (mouseEvent != null && mouseEventInternal != null && mouseEventInternal.hasUnderlyingPhysicalEvent)
				{
					this.m_LastMousePositionPanel = panel;
					this.m_LastMousePosition = mouseEvent.mousePosition;
				}
				bool flag = false;
				VisualElement visualElement = MouseCaptureController.mouseCapture as VisualElement;
				if (evt.GetEventTypeId() != EventBase<MouseCaptureOutEvent>.TypeId() && visualElement != null && visualElement.panel == null)
				{
					Event imguiEvent = evt.imguiEvent;
					Debug.Log(string.Format("Capture has no panel, forcing removal (capture={0} eventType={1})", MouseCaptureController.mouseCapture, (imguiEvent == null) ? "null" : imguiEvent.type.ToString()));
					MouseCaptureController.ReleaseMouse();
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
					if (evt.GetEventTypeId() == EventBase<WheelEvent>.TypeId())
					{
						flag2 = false;
						flag3 = false;
					}
				}
				evt.skipElement = null;
				if (flag2)
				{
					BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
					if (mouseEvent != null && baseVisualElementPanel != null)
					{
						VisualElement topElementUnderMouse = baseVisualElementPanel.topElementUnderMouse;
						if (evt.target == null)
						{
							baseVisualElementPanel.topElementUnderMouse = baseVisualElementPanel.Pick(mouseEvent.mousePosition);
						}
						this.DispatchEnterLeaveEvents(topElementUnderMouse, baseVisualElementPanel.topElementUnderMouse, evt);
					}
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
						BaseVisualElementPanel baseVisualElementPanel2 = panel as BaseVisualElementPanel;
						if (baseVisualElementPanel2 != null && evt.GetEventTypeId() == EventBase<MouseLeaveWindowEvent>.TypeId())
						{
							VisualElement topElementUnderMouse2 = baseVisualElementPanel2.topElementUnderMouse;
							baseVisualElementPanel2.topElementUnderMouse = null;
							this.DispatchMouseEnterMouseLeave(topElementUnderMouse2, baseVisualElementPanel2.topElementUnderMouse, mouseEvent);
							this.DispatchMouseOverMouseOut(topElementUnderMouse2, baseVisualElementPanel2.topElementUnderMouse, mouseEvent);
						}
						else if (baseVisualElementPanel2 != null && evt.GetEventTypeId() == EventBase<DragExitedEvent>.TypeId())
						{
							VisualElement topElementUnderMouse3 = baseVisualElementPanel2.topElementUnderMouse;
							baseVisualElementPanel2.topElementUnderMouse = null;
							this.DispatchDragEnterDragLeave(topElementUnderMouse3, baseVisualElementPanel2.topElementUnderMouse, mouseEvent);
						}
						else
						{
							VisualElement visualElement2 = null;
							if (evt.target == null && baseVisualElementPanel2 != null)
							{
								visualElement2 = baseVisualElementPanel2.topElementUnderMouse;
								baseVisualElementPanel2.topElementUnderMouse = panel.Pick(mouseEvent.mousePosition);
								evt.target = baseVisualElementPanel2.topElementUnderMouse;
							}
							if (evt.target != null)
							{
								flag = true;
								EventDispatcher.PropagateEvent(evt);
							}
							if (baseVisualElementPanel2 != null)
							{
								this.DispatchEnterLeaveEvents(visualElement2, baseVisualElementPanel2.topElementUnderMouse, evt);
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
					Event imguiEvent2 = evt.imguiEvent;
					if (!flag || (imguiEvent2 != null && imguiEvent2.type == EventType.Used) || evt.GetEventTypeId() == EventBase<MouseEnterWindowEvent>.TypeId() || evt.GetEventTypeId() == EventBase<MouseLeaveWindowEvent>.TypeId())
					{
						EventDispatcher.PropagateToIMGUIContainer(panel.visualTree, evt);
					}
				}
				if (evt.target == null && panel != null)
				{
					evt.target = panel.visualTree;
				}
				EventDispatcher.ExecuteDefaultAction(evt);
				evt.PostDispatch();
			}
		}

		internal void UpdateElementUnderMouse(BaseVisualElementPanel panel)
		{
			if (panel == this.m_LastMousePositionPanel)
			{
				Vector2 vector = panel.visualTree.WorldToLocal(this.m_LastMousePosition);
				VisualElement topElementUnderMouse = panel.topElementUnderMouse;
				panel.topElementUnderMouse = panel.Pick(vector);
				this.DispatchMouseEnterMouseLeave(topElementUnderMouse, panel.topElementUnderMouse, null);
				this.DispatchMouseOverMouseOut(topElementUnderMouse, panel.topElementUnderMouse, null);
			}
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
				EventDispatcher.PropagationPaths.Type type = ((!evt.tricklesDown) ? EventDispatcher.PropagationPaths.Type.None : EventDispatcher.PropagationPaths.Type.TrickleDown);
				type |= ((!evt.bubbles) ? EventDispatcher.PropagationPaths.Type.None : EventDispatcher.PropagationPaths.Type.BubbleUp);
				using (EventDispatcher.PropagationPaths propagationPaths = EventDispatcher.BuildPropagationPath(evt.target as VisualElement, type))
				{
					evt.dispatch = true;
					if (evt.tricklesDown && propagationPaths != null && propagationPaths.trickleDownPath.Count > 0)
					{
						evt.propagationPhase = PropagationPhase.TrickleDown;
						for (int i = propagationPaths.trickleDownPath.Count - 1; i >= 0; i--)
						{
							if (evt.isPropagationStopped)
							{
								break;
							}
							if (propagationPaths.trickleDownPath[i] != evt.skipElement)
							{
								evt.currentTarget = propagationPaths.trickleDownPath[i];
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
						if ((pathTypesRequested & EventDispatcher.PropagationPaths.Type.TrickleDown) == EventDispatcher.PropagationPaths.Type.TrickleDown && elem.shadow.parent.HasTrickleDownHandlers())
						{
							propagationPaths2.trickleDownPath.Add(elem.shadow.parent);
						}
						if ((pathTypesRequested & EventDispatcher.PropagationPaths.Type.BubbleUp) == EventDispatcher.PropagationPaths.Type.BubbleUp && elem.shadow.parent.HasBubbleUpHandlers())
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

		private static readonly ObjectPool<Queue<EventDispatcher.EventRecord>> k_EventQueuePool = new ObjectPool<Queue<EventDispatcher.EventRecord>>(100);

		private Queue<EventDispatcher.EventRecord> m_Queue;

		private uint m_GateCount;

		private Stack<EventDispatcher.DispatchContext> m_DispatchContexts = new Stack<EventDispatcher.DispatchContext>();

		private static EventDispatcher s_EventDispatcher;

		private IPanel m_LastMousePositionPanel;

		private Vector2 m_LastMousePosition;

		private const int k_DefaultPropagationDepth = 16;

		public struct Gate : IDisposable
		{
			public Gate(EventDispatcher d)
			{
				this.m_Dispatcher = d;
				this.m_Dispatcher.CloseGate();
			}

			public void Dispose()
			{
				this.m_Dispatcher.OpenGate();
			}

			private EventDispatcher m_Dispatcher;
		}

		private struct EventRecord
		{
			public EventBase m_Event;

			public IPanel m_Panel;
		}

		private struct DispatchContext
		{
			public uint m_GateCount;

			public Queue<EventDispatcher.EventRecord> m_Queue;
		}

		private class PropagationPaths : IDisposable
		{
			public PropagationPaths(int initialSize)
			{
				this.trickleDownPath = new List<VisualElement>(initialSize);
				this.bubblePath = new List<VisualElement>(initialSize);
			}

			[Obsolete("Use trickleDownPath instead of capturePath.")]
			public List<VisualElement> capturePath
			{
				get
				{
					return this.trickleDownPath;
				}
			}

			public void Dispose()
			{
				EventDispatcher.PropagationPathsPool.Release(this);
			}

			public void Clear()
			{
				this.bubblePath.Clear();
				this.trickleDownPath.Clear();
			}

			public readonly List<VisualElement> trickleDownPath;

			public readonly List<VisualElement> bubblePath;

			[Flags]
			public enum Type
			{
				None = 0,
				TrickleDown = 1,
				[Obsolete("Use TrickleDown instead of Capture.")]
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
