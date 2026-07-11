using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Event sent when the mouse pointer enters an element or one of its descendent elements. The event is cancellable, non-capturable, and does not bubble.</para>
	/// </summary>
	public class MouseEnterEvent : MouseEventBase<MouseEnterEvent>
	{
		/// <summary>
		///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
		/// </summary>
		public MouseEnterEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>Resets the event members to their initial value.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Capturable;
		}
	}
}
