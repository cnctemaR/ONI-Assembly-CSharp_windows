using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public sealed class EventDispatcher
	{
		internal PointerDispatchState pointerState { get; } = new PointerDispatchState();

		internal EventDispatcher()
		{
			this.m_DispatchingStrategies = new List<IEventDispatchingStrategy>();
			this.m_DispatchingStrategies.Add(new PointerCaptureDispatchingStrategy());
			this.m_DispatchingStrategies.Add(new MouseCaptureDispatchingStrategy());
			this.m_DispatchingStrategies.Add(new KeyboardEventDispatchingStrategy());
			this.m_DispatchingStrategies.Add(new PointerEventDispatchingStrategy());
			this.m_DispatchingStrategies.Add(new MouseEventDispatchingStrategy());
			this.m_DispatchingStrategies.Add(new CommandEventDispatchingStrategy());
			this.m_DispatchingStrategies.Add(new IMGUIEventDispatchingStrategy());
			this.m_DispatchingStrategies.Add(new DefaultDispatchingStrategy());
			this.m_Queue = EventDispatcher.k_EventQueuePool.Get();
		}

		private bool dispatchImmediately
		{
			get
			{
				return this.m_Immediate || this.m_GateCount == 0U;
			}
		}

		internal void Dispatch(EventBase evt, IPanel panel, DispatchMode dispatchMode)
		{
			evt.MarkReceivedByDispatcher();
			bool flag = evt.eventTypeId == EventBase<IMGUIEvent>.TypeId();
			if (flag)
			{
				Event imguiEvent = evt.imguiEvent;
				bool flag2 = imguiEvent.rawType == EventType.Repaint;
				if (flag2)
				{
					return;
				}
			}
			bool flag3 = this.dispatchImmediately || dispatchMode == DispatchMode.Immediate;
			if (flag3)
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
			bool flag = this.m_GateCount > 0U;
			if (flag)
			{
				this.m_GateCount -= 1U;
			}
			bool flag2 = this.m_GateCount == 0U;
			if (flag2)
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
			bool flag = ex != null;
			if (flag)
			{
				throw ex;
			}
		}

		private void ProcessEvent(EventBase evt, IPanel panel)
		{
			Event imguiEvent = evt.imguiEvent;
			bool flag = imguiEvent != null && imguiEvent.rawType == EventType.Used;
			using (new EventDispatcherGate(this))
			{
				evt.PreDispatch(panel);
				bool flag2 = !evt.stopDispatch && !evt.isPropagationStopped;
				if (flag2)
				{
					this.ApplyDispatchingStrategies(evt, panel, flag);
				}
				bool flag3 = evt.path != null;
				if (flag3)
				{
					foreach (VisualElement visualElement in evt.path.targetElements)
					{
						evt.target = visualElement;
						EventDispatchUtilities.ExecuteDefaultAction(evt, panel);
					}
					evt.target = evt.leafTarget;
				}
				else
				{
					EventDispatchUtilities.ExecuteDefaultAction(evt, panel);
				}
				evt.PostDispatch(panel);
				Debug.Assert(flag || evt.isPropagationStopped || imguiEvent == null || imguiEvent.rawType != EventType.Used, "Event is used but not stopped.");
			}
		}

		private void ApplyDispatchingStrategies(EventBase evt, IPanel panel, bool imguiEventIsInitiallyUsed)
		{
			foreach (IEventDispatchingStrategy eventDispatchingStrategy in this.m_DispatchingStrategies)
			{
				bool flag = eventDispatchingStrategy.CanDispatchEvent(evt);
				if (flag)
				{
					eventDispatchingStrategy.DispatchEvent(evt, panel);
					Debug.Assert(imguiEventIsInitiallyUsed || evt.isPropagationStopped || evt.imguiEvent == null || evt.imguiEvent.rawType != EventType.Used, "Unexpected condition: !evt.isPropagationStopped && evt.imguiEvent.rawType == EventType.Used.");
					bool flag2 = evt.stopDispatch || evt.isPropagationStopped;
					if (flag2)
					{
						break;
					}
				}
			}
		}

		private List<IEventDispatchingStrategy> m_DispatchingStrategies;

		private static readonly ObjectPool<Queue<EventDispatcher.EventRecord>> k_EventQueuePool = new ObjectPool<Queue<EventDispatcher.EventRecord>>(100);

		private Queue<EventDispatcher.EventRecord> m_Queue;

		private uint m_GateCount;

		private Stack<EventDispatcher.DispatchContext> m_DispatchContexts = new Stack<EventDispatcher.DispatchContext>();

		private bool m_Immediate = false;

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
	}
}
