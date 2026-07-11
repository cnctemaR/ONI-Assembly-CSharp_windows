using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>The propagation phases of an event.</para>
	/// </summary>
	public enum PropagationPhase
	{
		/// <summary>
		///   <para>The event is not being propagated.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>The event is being sent to the root element down to the event target parent element.</para>
		/// </summary>
		Capture,
		/// <summary>
		///   <para>The event is being sent to the event target.</para>
		/// </summary>
		AtTarget,
		/// <summary>
		///   <para>The event is being sent to the event target parent element up to the root element.</para>
		/// </summary>
		BubbleUp,
		/// <summary>
		///   <para>The event is being sent to the target element for it to execute its default actions for this event. Event handlers do not get the events in this phase. Instead, ExecuteDefaultAction is called on the target.</para>
		/// </summary>
		DefaultAction
	}
}
