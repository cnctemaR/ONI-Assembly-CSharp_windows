using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IVisualElementScheduler
	{
		IVisualElementScheduledItem Execute(Action<TimerState> timerUpdateEvent);

		/// <summary>
		///   <para>Schedule this action to be executed later.</para>
		/// </summary>
		/// <param name="timerUpdateEvent">The action to be executed.</param>
		/// <param name="updateEvent">The action to be executed.</param>
		/// <returns>
		///   <para>Reference to the scheduled action.</para>
		/// </returns>
		IVisualElementScheduledItem Execute(Action updateEvent);
	}
}
