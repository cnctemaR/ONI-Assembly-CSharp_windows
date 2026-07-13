using System;
using UnityEngine.Pool;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements
{
	internal class EventCallbackFunctor<TEventType> : EventCallbackFunctorBase where TEventType : EventBase<TEventType>, new()
	{
		public static EventCallbackFunctor<TEventType> GetPooled(long eventTypeId, EventCallback<TEventType> callback, InvokePolicy invokePolicy = InvokePolicy.Default)
		{
			EventCallbackFunctor<TEventType> eventCallbackFunctor = GenericPool<EventCallbackFunctor<TEventType>>.Get();
			eventCallbackFunctor.eventTypeId = eventTypeId;
			eventCallbackFunctor.invokePolicy = invokePolicy;
			eventCallbackFunctor.m_Callback = callback;
			return eventCallbackFunctor;
		}

		public override void Dispose()
		{
			this.eventTypeId = 0L;
			this.invokePolicy = InvokePolicy.Default;
			this.m_Callback = null;
			GenericPool<EventCallbackFunctor<TEventType>>.Release(this);
		}

		public override void Invoke(EventBase evt)
		{
			using (new EventDebuggerLogCall(this.m_Callback, evt))
			{
				this.m_Callback(evt as TEventType);
			}
		}

		public override void UnregisterCallback(CallbackEventHandler target, TrickleDown useTrickleDown)
		{
			target.UnregisterCallback<TEventType>(this.m_Callback, useTrickleDown);
		}

		public override bool IsEquivalentTo(long eventTypeId, Delegate callback)
		{
			return this.eventTypeId == eventTypeId && this.m_Callback == callback;
		}

		private EventCallback<TEventType> m_Callback;
	}
}
