using System;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	internal class ClampedDragger<T> : Clickable where T : IComparable<T>
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action dragging;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action draggingEnded;

		public ClampedDragger<T>.DragDirection dragDirection { get; set; }

		private BaseSlider<T> slider { get; set; }

		public Vector2 startMousePosition { get; private set; }

		public Vector2 delta
		{
			get
			{
				return base.lastMousePosition - this.startMousePosition;
			}
		}

		public ClampedDragger(BaseSlider<T> slider, Action clickHandler, Action dragHandler)
			: base(clickHandler, 250L, 30L)
		{
			this.dragDirection = ClampedDragger<T>.DragDirection.None;
			this.slider = slider;
			this.dragging += dragHandler;
		}

		protected override void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
		{
			this.startMousePosition = localPosition;
			this.dragDirection = ClampedDragger<T>.DragDirection.None;
			base.ProcessDownEvent(evt, localPosition, pointerId);
			Action action = this.dragging;
			if (action != null)
			{
				action();
			}
		}

		protected override void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
		{
			base.ProcessUpEvent(evt, localPosition, pointerId);
			Action action = this.draggingEnded;
			if (action != null)
			{
				action();
			}
		}

		protected override void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
		{
			base.ProcessMoveEvent(evt, localPosition);
			bool flag = this.dragDirection == ClampedDragger<T>.DragDirection.None;
			if (flag)
			{
				this.dragDirection = ClampedDragger<T>.DragDirection.Free;
			}
			bool flag2 = this.dragDirection == ClampedDragger<T>.DragDirection.Free;
			if (flag2)
			{
				bool flag3 = evt.eventTypeId == EventBase<PointerMoveEvent>.TypeId();
				if (flag3)
				{
					PointerMoveEvent pointerMoveEvent = (PointerMoveEvent)evt;
					bool flag4 = pointerMoveEvent.pointerId != PointerId.mousePointerId;
					if (flag4)
					{
						pointerMoveEvent.isHandledByDraggable = true;
					}
				}
				Action action = this.dragging;
				if (action != null)
				{
					action();
				}
			}
		}

		[Flags]
		public enum DragDirection
		{
			None = 0,
			LowToHigh = 1,
			HighToLow = 2,
			Free = 4
		}
	}
}
