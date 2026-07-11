using System;
using System.Diagnostics;
using UnityEngine.EventSystems;

public class KButtonDrag : KButton, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onBeginDrag;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onDrag;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onEndDrag;

	public void ClearOnDragEvents()
	{
		this.onBeginDrag = null;
		this.onDrag = null;
		this.onEndDrag = null;
	}

	public void OnBeginDrag(PointerEventData data)
	{
		this.onBeginDrag.Signal();
	}

	public void OnDrag(PointerEventData data)
	{
		this.onDrag.Signal();
	}

	public void OnEndDrag(PointerEventData data)
	{
		this.onEndDrag.Signal();
	}
}
