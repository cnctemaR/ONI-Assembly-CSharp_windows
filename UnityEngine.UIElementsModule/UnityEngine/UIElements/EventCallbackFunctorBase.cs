using System;

namespace UnityEngine.UIElements
{
	internal abstract class EventCallbackFunctorBase : IDisposable
	{
		public abstract void Invoke(EventBase evt);

		public abstract void UnregisterCallback(CallbackEventHandler target, TrickleDown useTrickleDown);

		public abstract void Dispose();

		public abstract bool IsEquivalentTo(long eventTypeId, Delegate callback);

		public long eventTypeId;

		public InvokePolicy invokePolicy;
	}
}
