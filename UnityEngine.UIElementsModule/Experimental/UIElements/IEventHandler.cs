using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IEventHandler
	{
		/// <summary>
		///   <para>Handle an event.</para>
		/// </summary>
		/// <param name="evt">The event to handle.</param>
		void HandleEvent(EventBase evt);

		/// <summary>
		///   <para>Return true if event handlers for the event propagation capture phase have been attached on this object.</para>
		/// </summary>
		/// <returns>
		///   <para>True if object has event handlers for the capture phase.</para>
		/// </returns>
		bool HasCaptureHandlers();

		/// <summary>
		///   <para>Return true if event handlers for the event propagation bubble up phase have been attached on this object.</para>
		/// </summary>
		/// <returns>
		///   <para>True if object has event handlers for the bubble up phase.</para>
		/// </returns>
		bool HasBubbleHandlers();
	}
}
