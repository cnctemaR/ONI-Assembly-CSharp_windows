using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Event sent immediately before an element loses focus. Capturable, bubbles, non-cancellable.</para>
	/// </summary>
	public class FocusOutEvent : FocusEventBase<FocusOutEvent>
	{
		/// <summary>
		///   <para>Constructor. Avoid newing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
		/// </summary>
		public FocusOutEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>Reset the event members to their initial value.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.Capturable;
		}
	}
}
