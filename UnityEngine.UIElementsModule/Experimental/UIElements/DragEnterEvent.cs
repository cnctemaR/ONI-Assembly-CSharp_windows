using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Use the DragEnterEvent class to manage events that occur when dragging enters an element or one of its descendants. The DragEnterEvent can be cancelled, cannot be captured, and does not bubble.</para>
	/// </summary>
	public class DragEnterEvent : DragAndDropEventBase<DragEnterEvent>
	{
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
