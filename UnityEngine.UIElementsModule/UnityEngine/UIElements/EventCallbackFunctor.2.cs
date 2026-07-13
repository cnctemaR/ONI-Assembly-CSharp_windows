using System;
using UnityEngine.Pool;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements
{
	internal class EventCallbackFunctor<TEventType, TCallbackArgs> : EventCallbackFunctorBase where TEventType : EventBase<TEventType>, new()
	{
		internal TCallbackArgs userArgs { get; set; }

		public static EventCallbackFunctor<TEventType, TCallbackArgs> GetPooled(long eventTypeId, EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, InvokePolicy invokePolicy = InvokePolicy.Default)
		{
			EventCallbackFunctor<TEventType, TCallbackArgs> eventCallbackFunctor = GenericPool<EventCallbackFunctor<TEventType, TCallbackArgs>>.Get();
			eventCallbackFunctor.eventTypeId = eventTypeId;
			eventCallbackFunctor.invokePolicy = invokePolicy;
			eventCallbackFunctor.userArgs = userArgs;
			eventCallbackFunctor.m_Callback = callback;
			return eventCallbackFunctor;
		}

		public override void Dispose()
		{
			this.eventTypeId = 0L;
			this.invokePolicy = InvokePolicy.Default;
			this.userArgs = default(TCallbackArgs);
			this.m_Callback = null;
			GenericPool<EventCallbackFunctor<TEventType, TCallbackArgs>>.Release(this);
		}

		public override void Invoke(EventBase evt)
		{
			using (new EventDebuggerLogCall(this.m_Callback, evt))
			{
				this.m_Callback(evt as TEventType, this.userArgs);
			}
		}

		public override void UnregisterCallback(CallbackEventHandler target, TrickleDown useTrickleDown)
		{
			target.UnregisterCallback<TEventType, TCallbackArgs>(this.m_Callback, useTrickleDown);
		}

		public override bool IsEquivalentTo(long eventTypeId, Delegate callback)
		{
			return this.eventTypeId == eventTypeId && this.m_Callback == callback;
		}

		private EventCallback<TEventType, TCallbackArgs> m_Callback;
	}
}
