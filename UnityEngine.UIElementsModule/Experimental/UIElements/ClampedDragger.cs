using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Experimental.UIElements
{
	internal class ClampedDragger<T> : Clickable where T : IComparable<T>
	{
		public ClampedDragger(BaseSlider<T> slider, Action clickHandler, Action dragHandler)
			: base(clickHandler, 250L, 30L)
		{
			this.dragDirection = ClampedDragger<T>.DragDirection.None;
			this.slider = slider;
			this.dragging += dragHandler;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action dragging;

		public ClampedDragger<T>.DragDirection dragDirection { get; set; }

		private BaseSlider<T> slider { get; set; }

		public Vector2 startMousePosition { get; private set; }

		public Vector2 delta
		{
			[CompilerGenerated]
			get
			{
				return base.lastMousePosition - this.startMousePosition;
			}
		}

		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(base.OnMouseUp), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(base.OnMouseUp), TrickleDown.NoTrickleDown);
		}

		private new void OnMouseDown(MouseDownEvent evt)
		{
			if (base.CanStartManipulation(evt))
			{
				this.startMousePosition = evt.localMousePosition;
				this.dragDirection = ClampedDragger<T>.DragDirection.None;
				base.OnMouseDown(evt);
			}
		}

		private new void OnMouseMove(MouseMoveEvent evt)
		{
			if (this.m_Active)
			{
				base.OnMouseMove(evt);
				if (this.dragDirection == ClampedDragger<T>.DragDirection.None)
				{
					this.dragDirection = ClampedDragger<T>.DragDirection.Free;
				}
				if (this.dragDirection == ClampedDragger<T>.DragDirection.Free)
				{
					if (this.dragging != null)
					{
						this.dragging();
					}
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
