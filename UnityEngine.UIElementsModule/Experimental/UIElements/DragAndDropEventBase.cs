using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class DragAndDropEventBase<T> : MouseEventBase<T>, IDragAndDropEvent, IPropagatableEvent where T : DragAndDropEventBase<T>, new()
	{
	}
}
