using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IScheduler
	{
		IScheduledItem ScheduleOnce(Action<TimerState> timerUpdateEvent, long delayMs);

		IScheduledItem ScheduleUntil(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, Func<bool> stopCondition = null);

		IScheduledItem ScheduleForDuration(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, long durationMs);

		/// <summary>
		///   <para>Manually unschedules a previously scheduled action.</para>
		/// </summary>
		/// <param name="item">The item to be removed from this scheduler.</param>
		void Unschedule(IScheduledItem item);

		/// <summary>
		///   <para>Add this item to the list of scheduled tasks.</para>
		/// </summary>
		/// <param name="item">The item to register.</param>
		void Schedule(IScheduledItem item);
	}
}
