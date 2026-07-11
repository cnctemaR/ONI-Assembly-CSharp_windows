using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Use the DragLeaveEvent class to manage events sent when dragging leaves an element or one of its descendants. The DragLeaveEvent can be cancelled, cannot be captured, and does not bubble.</para>
	/// </summary>
	public class DragLeaveEvent : DragAndDropEventBase<DragLeaveEvent>
	{
		/// <summary>
		///   <para>Constructor. Avoid renewing events. Instead, use GetPooled() to get an event from a pool of reusable events.</para>
		/// </summary>
		public DragLeaveEvent()
		{
			this.Init();
		}

		/// <summary>
		///   <para>Resets the event members to their initial values.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Capturable;
		}
	}
}
