using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>The event sent to a dragged element when the drag and drop process ends.</para>
	/// </summary>
	public class DragExitedEvent : DragAndDropEventBase<DragExitedEvent>
	{
		/// <summary>
		///   <para>Resets the event members to their initial values.</para>
		/// </summary>
		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.Capturable;
		}
	}
}
