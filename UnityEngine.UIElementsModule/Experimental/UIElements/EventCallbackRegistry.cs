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

		private bool UnregisterCallback(long eventTypeId, Delegate callback, TrickleDown useTrickleDown)
		{
			bool flag;
			if (callback == null)
			{
				flag = false;
			}
			else
			{
				EventCallbackList callbackListForWriting = this.GetCallbackListForWriting();
				CallbackPhase callbackPhase = ((useTrickleDown != TrickleDown.TrickleDown) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.TrickleDownAndTarget);
				flag = callbackListForWriting.Remove(eventTypeId, callback, callbackPhase);
			}
			return flag;
		}

		[Obsolete("Use TrickleDown instead of Capture.")]
		public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			this.RegisterCallback<TEventType>(callback, (TrickleDown)useCapture);
		}

		public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			if (callback == null)
			{
				throw new ArgumentException("callback parameter is null");
			}
			long num = EventBase<TEventType>.TypeId();
			CallbackPhase callbackPhase = ((useTrickleDown != TrickleDown.TrickleDown) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.TrickleDownAndTarget);
			EventCallbackList eventCallbackList = this.GetCallbackListForReading();
			if (eventCallbackList == null || !eventCallbackList.Contains(num, callback, callbackPhase))
			{
				eventCallbackList = this.GetCallbackListForWriting();
				eventCallbackList.Add(new EventCallbackFunctor<TEventType>(callback, callbackPhase));
			}
		}

		[Obsolete("Use TrickleDown instead of Capture.")]
		public void RegisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			this.RegisterCallback<TEventType, TCallbackArgs>(callback, userArgs, (TrickleDown)useCapture);
		}

		public void RegisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			if (callback == null)
			{
				throw new ArgumentException("callback parameter is null");
			}
			long num = EventBase<TEventType>.TypeId();
			CallbackPhase callbackPhase = ((useTrickleDown != TrickleDown.TrickleDown) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.TrickleDownAndTarget);
			EventCallbackList eventCallbackList = this.GetCallbackListForReading();
			if (eventCallbackList != null)
			{
				EventCallbackFunctor<TEventType, TCallbackArgs> eventCallbackFunctor = eventCallbackList.Find(num, callback, callbackPhase) as EventCallbackFunctor<TEventType, TCallbackArgs>;
				if (eventCallbackFunctor != null)
				{
					eventCallbackFunctor.userArgs = userArgs;
					return;
				}
			}
			eventCallbackList = this.GetCallbackListForWriting();
			eventCallbackList.Add(new EventCallbackFunctor<TEventType, TCallbackArgs>(callback, userArgs, callbackPhase));
		}

		[Obsolete("Use TrickleDown instead of Capture.")]
		public bool UnregisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			return this.UnregisterCallback<TEventType>(callback, (TrickleDown)useCapture);
		}

		public bool UnregisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			return this.UnregisterCallback(num, callback, useTrickleDown);
		}

		[Obsolete("Use TrickleDown instead of Capture.")]
		public bool UnregisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			return this.UnregisterCallback<TEventType, TCallbackArgs>(callback, (TrickleDown)useCapture);
		}

		public bool UnregisterCallback<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			return this.UnregisterCallback(num, callback, useTrickleDown);
		}

		internal bool TryGetUserArgs<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TrickleDown useTrickleDown, out TCallbackArgs userArgs) where TEventType : EventBase<TEventType>, new()
		{
			userArgs = default(TCallbackArgs);
			bool flag;
			if (callback == null)
			{
				flag = false;
			}
			else
			{
				EventCallbackList callbackListForReading = this.GetCallbackListForReading();
				long num = EventBase<TEventType>.TypeId();
				CallbackPhase callbackPhase = ((useTrickleDown != TrickleDown.TrickleDown) ? CallbackPhase.TargetAndBubbleUp : CallbackPhase.TrickleDownAndTarget);
				EventCallbackFunctor<TEventType, TCallbackArgs> eventCallbackFunctor = callbackListForReading.Find(num, callback, callbackPhase) as EventCallbackFunctor<TEventType, TCallbackArgs>;
				if (eventCallbackFunctor == null)
				{
					flag = false;
				}
				else
				{
					userArgs = eventCallbackFunctor.userArgs;
					flag = true;
				}
			}
			return flag;
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

		[Obsolete("Use HasTrickleDownHandlers instead of HasCaptureHandlers.")]
		public bool HasCaptureHandlers()
		{
			return this.HasTrickleDownHandlers();
		}

		public bool HasTrickleDownHandlers()
		{
			return this.m_Callbacks != null && this.m_Callbacks.trickleDownCallbackCount > 0;
		}

		public bool HasBubbleHandlers()
		{
			return this.m_Callbacks != null && this.m_Callbacks.bubbleUpCallbackCount > 0;
		}

		private static readonly EventCallbackListPool s_ListPool = new EventCallbackListPool();

		private EventCallbackList m_Callbacks;

		private EventCallbackList m_TemporaryCallbacks;

		private int m_IsInvoking;
	}
}
