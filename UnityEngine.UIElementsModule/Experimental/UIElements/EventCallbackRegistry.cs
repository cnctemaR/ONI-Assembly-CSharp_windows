using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class EventCallbackRegistry
	{
		public EventCallbackRegistry()
		{
			this.m_IsInvoking = 0;
		}

		private static EventCallbackList GetCallbackList(EventCallbackList initializer = null)
		{
			return EventCallbackRegistry.s_ListPool.Get(initializer);
		}

		private static void ReleaseCallbackList(EventCallbackList toRelease)
		{
			EventCallbackRegistry.s_ListPool.Release(toRelease);
		}

		private EventCallbackList GetCallbackListForWriting()
		{
			EventCallbackList eventCallbackList;
			if (this.m_IsInvoking > 0)
			{
				if (this.m_TemporaryCallbacks == null)
				{
					if (this.m_Callbacks != null)
					{
						this.m_TemporaryCallbacks = EventCallbackRegistry.GetCallbackList(this.m_Callbacks);
					}
					else
					{
						this.m_TemporaryCallbacks = EventCallbackRegistry.GetCallbackList(null);
					}
				}
				eventCallbackList = this.m_TemporaryCallbacks;
			}
			else
			{
				if (this.m_Callbacks == null)
				{
					this.m_Callbacks = EventCallbackRegistry.GetCallbackList(null);
				}
				eventCallbackList = this.m_Callbacks;
			}
			return eventCallbackList;
		}

		private EventCallbackList GetCallbackListForReading()
		{
			EventCallbackList eventCallbackList;
			if (this.m_TemporaryCallbacks != null)
			{
				eventCallbackList = this.m_TemporaryCallbacks;
			}
			else
			{
				eventCallbackList = this.m_Callbacks;
			}
			return eventCallbackList;
		}

		private bool ShouldRegisterCallback(long eventTypeId, Delegate callback, CallbackPhase phase)
		{
			bool flag;
			if (callback == null)
			{
				flag = false;
			}
			else
			{
				EventCallbackList callbackListForReading = this.GetCallbackListForReading();
				flag = callbackListForReading == null || !callbackListForReading.Contains(eventTypeId, callback, phase);
			}
			return flag;
		}

		private bool UnregisterCallback(long eventTypeId, Delegate callback, Capture useCapture)
		{
			bool flag;
			if (callback == null)
			{
				flag = false;
			}
			else
			{
				EventCallbackList callbackListForWriting = this.GetCallbackListForWriting();
				CallbackPhase callbackPhase = ((useCapture != Capture.Capture) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.CaptureAndTarget);
				flag = callbackListForWriting.Remove(eventTypeId, callback, callbackPhase);
			}
			return flag;
		}

		public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			CallbackPhase callbackPhase = ((useCapture != Capture.Capture) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.CaptureAndTarget);
			if (this.ShouldRegisterCallback(num, callback, callbackPhase))
			{
				EventCallbackList callbackListForWriting = this.GetCallbackListForWriting();
				callbackListForWriting.Add(new EventCallbackFunctor<TEventType>(callback, callbackPhase));
			}
		}

		public void RegisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			CallbackPhase callbackPhase = ((useCapture != Capture.Capture) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.CaptureAndTarget);
			if (this.ShouldRegisterCallback(num, callback, callbackPhase))
			{
				EventCallbackList callbackListForWriting = this.GetCallbackListForWriting();
				callbackListForWriting.Add(new EventCallbackFunctor<TEventType, TCallbackArgs>(callback, userArgs, callbackPhase));
			}
		}

		public bool UnregisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			return this.UnregisterCallback(num, callback, useCapture);
		}

		public bool UnregisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			return this.UnregisterCallback(num, callback, useCapture);
		}

		public void InvokeCallbacks(EventBase evt)
		{
			if (this.m_Callbacks != null)
			{
				this.m_IsInvoking++;
				for (int i = 0; i < this.m_Callbacks.Count; i++)
				{
					if (evt.isImmediatePropagationStopped)
					{
						break;
					}
					this.m_Callbacks[i].Invoke(evt);
				}
				this.m_IsInvoking--;
				if (this.m_IsInvoking == 0)
				{
					if (this.m_TemporaryCallbacks != null)
					{
						EventCallbackRegistry.ReleaseCallbackList(this.m_Callbacks);
						this.m_Callbacks = EventCallbackRegistry.GetCallbackList(this.m_TemporaryCallbacks);
						EventCallbackRegistry.ReleaseCallbackList(this.m_TemporaryCallbacks);
						this.m_TemporaryCallbacks = null;
					}
				}
			}
		}

		public bool HasCaptureHandlers()
		{
			return this.m_Callbacks != null && this.m_Callbacks.capturingCallbackCount > 0;
		}

		public bool HasBubbleHandlers()
		{
			return this.m_Callbacks != null && this.m_Callbacks.bubblingCallbackCount > 0;
		}

		private static readonly EventCallbackListPool s_ListPool = new EventCallbackListPool();

		private EventCallbackList m_Callbacks;

		private EventCallbackList m_TemporaryCallbacks;

		private int m_IsInvoking;
	}
}
