using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IEventDispatcher
	{
		/// <summary>
		///   <para>Dispatch an event to the panel.</para>
		/// </summary>
		/// <param name="evt">The event to dispatch.</param>
		/// <param name="panel">The panel where the event will be dispatched.</param>
		void DispatchEvent(EventBase evt, IPanel panel);
	}
}
