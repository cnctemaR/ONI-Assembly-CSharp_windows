using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Base class for drag and drop events.</para>
	/// </summary>
	public abstract class DragAndDropEventBase<T> : MouseEventBase<T>, IDragAndDropEvent, IPropagatableEvent where T : DragAndDropEventBase<T>, new()
	{
	}
}
