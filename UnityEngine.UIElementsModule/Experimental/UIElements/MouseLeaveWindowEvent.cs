using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Event sent when the mouse pointer exits a window. Cancellable, non-capturable, does not bubbles.</para>
	/// </summary>
	public class MouseLeaveWindowEvent : MouseEventBase<MouseLeaveWindowEvent>
	{
		/// <summary>
		///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
		/// </summary>
		public MouseLeaveWindowEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>Reset the event members to their initial value.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Cancellable;
		}
	}
}
