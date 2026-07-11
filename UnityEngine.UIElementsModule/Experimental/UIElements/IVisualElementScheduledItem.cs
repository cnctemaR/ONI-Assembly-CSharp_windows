using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IVisualElementScheduledItem
	{
		/// <summary>
		///   <para>Returns the VisualElement this object is associated with.</para>
		/// </summary>
		VisualElement element { get; }

		/// <summary>
		///   <para>Will be true when this item is scheduled. Note that an item's callback will only be executed when it's VisualElement is attached to a panel.</para>
		/// </summary>
		bool isActive { get; }

		/// <summary>
		///   <para>If not already active, will schedule this item on its VisualElement's scheduler.</para>
		/// </summary>
		void Resume();

		/// <summary>
		///   <para>Removes this item from its VisualElement's scheduler.</para>
		/// </summary>
		void Pause();

		/// <summary>
		///   <para>Cancels any previously scheduled execution of this item and re-schedules the item.</para>
		/// </summary>
		/// <param name="delayMs">Minimum time in milliseconds before this item will be executed.</param>
		void ExecuteLater(long delayMs);

		/// <summary>
		///   <para>Adds a delay to the first invokation.</para>
		/// </summary>
		/// <param name="delayMs">The minimum number of milliseconds after activation where this item's action will be executed.</param>
		/// <returns>
		///   <para>This ScheduledItem.</para>
		/// </returns>
		IVisualElementScheduledItem StartingIn(long delayMs);

		/// <summary>
		///   <para>Repeats this action after a specified time.</para>
		/// </summary>
		/// <param name="intervalMs">Minimum amount of time in milliseconds between each action execution.</param>
		/// <returns>
		///   <para>This ScheduledItem.</para>
		/// </returns>
		IVisualElementScheduledItem Every(long intervalMs);

		IVisualElementScheduledItem Until(Func<bool> stopCondition);

		/// <summary>
		///   <para>After specified duration, the item will be automatically unscheduled.</para>
		/// </summary>
		/// <param name="durationMs">The total duration in milliseconds where this item will be active.</param>
		/// <returns>
		///   <para>This ScheduledItem.</para>
		/// </returns>
		IVisualElementScheduledItem ForDuration(long durationMs);
	}
}
