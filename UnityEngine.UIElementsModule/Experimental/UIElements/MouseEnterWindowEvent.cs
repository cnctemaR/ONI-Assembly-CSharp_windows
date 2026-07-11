using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Event sent when the mouse pointer enters a window. Cancellable, non-capturable, does not bubbles.</para>
	/// </summary>
	public class MouseEnterWindowEvent : MouseEventBase<MouseEnterWindowEvent>
	{
		/// <summary>
		///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
		/// </summary>
		public MouseEnterWindowEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>Resets the event members to their initial value.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Cancellable;
		}
	}
}
