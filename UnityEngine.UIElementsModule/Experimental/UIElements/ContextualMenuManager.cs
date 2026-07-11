using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Use this class to display a contextual menu.</para>
	/// </summary>
	public abstract class ContextualMenuManager
	{
		/// <summary>
		///   <para>Check if the event is an event that triggers the display of the menu and display it if it needs to.</para>
		/// </summary>
		/// <param name="eventHandler">The element for which the menu is displayed.</param>
		/// <param name="evt">The event to inspect.</param>
		public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

		/// <summary>
		///   <para>Display the contextual menu.</para>
		/// </summary>
		/// <param name="triggerEvent">The event that triggered the display of the menu.</param>
		/// <param name="target">The element for which the menu is displayed.</param>
		public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
		{
			ContextualMenu contextualMenu = new ContextualMenu();
			bool flag;
			using (ContextualMenuPopulateEvent pooled = ContextualMenuPopulateEvent.GetPooled(triggerEvent, contextualMenu, target))
			{
				UIElementsUtility.eventDispatcher.DispatchEvent(pooled, null);
				flag = !pooled.isDefaultPrevented;
			}
			if (flag)
			{
				contextualMenu.PrepareForDisplay(triggerEvent);
				this.DoDisplayMenu(contextualMenu, triggerEvent);
			}
		}

		/// <summary>
		///   <para>Display the contextual menu.</para>
		/// </summary>
		/// <param name="menu">The menu to display.</param>
		/// <param name="triggerEvent">The event that triggered the display of the contextual menu.</param>
		protected abstract void DoDisplayMenu(ContextualMenu menu, EventBase triggerEvent);
	}
}
