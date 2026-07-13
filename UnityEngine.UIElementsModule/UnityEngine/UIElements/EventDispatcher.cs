using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace UnityEngine.UIElements
{
	public sealed class EventDispatcher
	{
		internal PointerDispatchState pointerState { get; } = new PointerDispatchState();

		internal uint GateDepth
		{
			get
			{
				return this.m_GateDepth;
			}
		}

		internal static EventDispatcher CreateDefault()
		{
			return new EventDispatcher();
		}

		[Obsolete("Please use EventDispatcher.CreateDefault().")]
		internal EventDispatcher()
		{
			this.m_Queue = EventDispatcher.k_EventQueuePool.Get();
		}

		private bool dispatchImmediately
		{
			get
			{
				return this.m_Immediate || this.m_GateCount == 0U;
			}
		}

		internal bool processingEvents { get; private set; }

		internal void Dispatch(EventBase evt, [NotNull] BaseVisualElementPanel panel, DispatchMode dispatchMode)
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
				bool flag4 = this.HandleRecursiveState(evt);
				if (!flag4)
				{
					evt.Acquire();
					this.m_Queue.Enqueue(new EventDispatcher.EventRecord
					{
						m_Event = evt,
						m_Panel = panel
					});
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool HandleRecursiveState(EventBase evt)
		{
			bool flag = this.m_GateDepth <= 400U;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.m_DispatchStackFrame != 0;
				if (flag3)
				{
					StackTrace stackTrace = new StackTrace(1, true);
					StringBuilder stringBuilder = new StringBuilder();
					int num = stackTrace.FrameCount - this.m_DispatchStackFrame;
					stringBuilder.AppendLine(string.Format("Recursively dispatching event {0} from another event {1} (depth = {2})", evt, this.m_CurrentEvent, this.m_GateDepth));
					for (int i = 0; i < num; i++)
					{
						StackFrame frame = stackTrace.GetFrame(i);
						stringBuilder.Append(frame.GetMethod()).AppendFormat("({0}:{1}", frame.GetFileName(), frame.GetFileLineNumber()).AppendLine(")");
					}
					Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, stringBuilder.ToString(), Array.Empty<object>());
				}
				else
				{
					Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, string.Format("Recursively dispatching event {0} from another event {1} (depth = {2})", evt, this.m_CurrentEvent, this.m_GateDepth), Array.Empty<object>());
				}
				bool flag4 = this.m_GateDepth > 500U;
				if (flag4)
				{
					Debug.LogErrorFormat("Ignoring event {0}: too many events dispatched recurively", new object[] { evt });
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		internal void PushDispatcherContext()
		{
			this.ProcessEventQueue();
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
			this.m_GateDepth += 1U;
		}

		internal void OpenGate()
		{
			Debug.Assert(this.m_GateCount > 0U, "m_GateCount > 0");
			bool flag = this.m_GateCount > 0U;
			if (flag)
			{
				this.m_GateCount -= 1U;
			}
			try
			{
				bool flag2 = this.m_GateCount == 0U;
				if (flag2)
				{
					this.ProcessEventQueue();
				}
			}
			finally
			{
				Debug.Assert(this.m_GateDepth > 0U, "m_GateDepth > 0");
				bool flag3 = this.m_GateDepth > 0U;
				if (flag3)
				{
					this.m_GateDepth -= 1U;
				}
			}
		}

		private void ProcessEventQueue()
		{
			Queue<EventDispatcher.EventRecord> queue = this.m_Queue;
			this.m_Queue = EventDispatcher.k_EventQueuePool.Get();
			ExitGUIException ex = null;
			try
			{
				this.processingEvents = true;
				while (queue.Count > 0)
				{
					EventDispatcher.EventRecord eventRecord = queue.Dequeue();
					EventBase @event = eventRecord.m_Event;
					BaseVisualElementPanel panel = eventRecord.m_Panel;
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
				this.processingEvents = false;
				EventDispatcher.k_EventQueuePool.Release(queue);
			}
			bool flag = ex != null;
			if (flag)
			{
				throw ex;
			}
		}

		private void ProcessEvent(EventBase evt, [NotNull] BaseVisualElementPanel panel)
		{
			bool disposed = panel.disposed;
			if (!disposed)
			{
				Event imguiEvent = evt.imguiEvent;
				bool flag = imguiEvent != null && imguiEvent.rawType == EventType.Used;
				using (new EventDispatcherGate(this))
				{
					evt.PreDispatch(panel);
					try
					{
						this.m_CurrentEvent = evt;
						this.m_DispatchStackFrame = ((this.m_GateDepth > 490U) ? new StackTrace().FrameCount : 0);
						evt.Dispatch(panel);
					}
					finally
					{
						this.m_CurrentEvent = null;
					}
					evt.PostDispatch(panel);
					Debug.Assert(flag || evt.isPropagationStopped || imguiEvent == null || imguiEvent.rawType != EventType.Used, "Event is used but not stopped.");
				}
			}
		}

		internal ClickDetector m_ClickDetector = new ClickDetector();

		private static readonly ObjectPool<Queue<EventDispatcher.EventRecord>> k_EventQueuePool = new ObjectPool<Queue<EventDispatcher.EventRecord>>(() => new Queue<EventDispatcher.EventRecord>(), 100);

		private Queue<EventDispatcher.EventRecord> m_Queue;

		private uint m_GateCount;

		private uint m_GateDepth = 0U;

		internal const int k_MaxGateDepth = 500;

		internal const int k_NumberOfEventsWithStackInfo = 10;

		internal const int k_NumberOfEventsWithEventInfo = 100;

		private int m_DispatchStackFrame = 0;

		private EventBase m_CurrentEvent;

		private Stack<EventDispatcher.DispatchContext> m_DispatchContexts = new Stack<EventDispatcher.DispatchContext>();

		private bool m_Immediate = false;

		private struct EventRecord
		{
			public EventBase m_Event;

			public BaseVisualElementPanel m_Panel;
		}

		private struct DispatchContext
		{
			public uint m_GateCount;

			public Queue<EventDispatcher.EventRecord> m_Queue;
		}
	}
}
