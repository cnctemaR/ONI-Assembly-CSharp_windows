using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class EventCallbackFunctor<TEventType, TCallbackArgs> : EventCallbackFunctorBase where TEventType : EventBase<TEventType>, new()
	{
		public EventCallbackFunctor(EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, CallbackPhase phase)
			: base(phase)
		{
			this.userArgs = userArgs;
			this.m_Callback = callback;
			this.m_EventTypeId = EventBase<TEventType>.TypeId();
		}

		internal TCallbackArgs userArgs { get; set; }

		public override void Invoke(EventBase evt)
		{
			if (evt == null)
			{
				throw new ArgumentNullException();
			}
			if (evt.GetEventTypeId() == this.m_EventTypeId)
			{
				if (base.PhaseMatches(evt))
				{
					this.m_Callback(evt as TEventType, this.userArgs);
				}
			}
		}

		public override bool IsEquivalentTo(long eventTypeId, Delegate callback, CallbackPhase phase)
		{
			return this.m_EventTypeId == eventTypeId && this.m_Callback == callback && base.phase == phase;
		}

		private EventCallback<TEventType, TCallbackArgs> m_Callback;

		private long m_EventTypeId;
	}
}
