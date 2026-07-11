using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>An event sent when a contextual menu needs to be filled with menu item.</para>
	/// </summary>
	public class ContextualMenuPopulateEvent : MouseEventBase<ContextualMenuPopulateEvent>
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public ContextualMenuPopulateEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>The menu to populate.</para>
		/// </summary>
		public ContextualMenu menu { get; private set; }

		/// <summary>
		///   <para>Retrieves an event from the event pool. Use this method to retrieve a mouse event and initialize the event, instead of creating a new mouse event.</para>
		/// </summary>
		/// <param name="triggerEvent">The event that triggered the display of the contextual menu.</param>
		/// <param name="menu">The menu to populate.</param>
		/// <param name="target">The element that triggered the display of the contextual menu.</param>
		/// <returns>
		///   <para>The event.</para>
		/// </returns>
		public static ContextualMenuPopulateEvent GetPooled(EventBase triggerEvent, ContextualMenu menu, IEventHandler target)
		{
			ContextualMenuPopulateEvent pooled = EventBase<ContextualMenuPopulateEvent>.GetPooled();
			if (triggerEvent != null)
			{
				IMouseEvent mouseEvent = triggerEvent as IMouseEvent;
				if (mouseEvent != null)
				{
					pooled.modifiers = mouseEvent.modifiers;
					pooled.mousePosition = mouseEvent.mousePosition;
					pooled.localMousePosition = mouseEvent.mousePosition;
					pooled.mouseDelta = mouseEvent.mouseDelta;
					pooled.button = mouseEvent.button;
					pooled.clickCount = mouseEvent.clickCount;
				}
				IMouseEventInternal mouseEventInternal = triggerEvent as IMouseEventInternal;
				if (mouseEventInternal != null)
				{
					((IMouseEventInternal)pooled).hasUnderlyingPhysicalEvent = mouseEventInternal.hasUnderlyingPhysicalEvent;
				}
				pooled.target = target;
				pooled.menu = menu;
			}
			return pooled;
		}

		/// <summary>
		///   <para>Reset the event members to their initial value.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			this.menu = null;
		}
	}
}
