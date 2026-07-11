using System;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	public class Clickable : MouseManipulator
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EventBase> clickedWithEventInfo;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action clicked;

		protected bool active { get; set; }

		public Vector2 lastMousePosition { get; private set; }

		public Clickable(Action handler, long delay, long interval)
			: this(handler)
		{
			this.m_Delay = delay;
			this.m_Interval = interval;
			this.active = false;
		}

		public Clickable(Action<EventBase> handler)
		{
			this.clickedWithEventInfo = handler;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
		}

		public Clickable(Action handler)
		{
			this.clicked = handler;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
			this.active = false;
		}

		private void OnTimer(TimerState timerState)
		{
			bool flag = (this.clicked != null || this.clickedWithEventInfo != null) && this.IsRepeatable();
			if (flag)
			{
				bool flag2 = base.target.ContainsPoint(this.lastMousePosition);
				if (flag2)
				{
					this.Invoke(null);
					base.target.pseudoStates |= PseudoStates.Active;
				}
				else
				{
					base.target.pseudoStates &= ~PseudoStates.Active;
				}
			}
		}

		private bool IsRepeatable()
		{
			return this.m_Delay > 0L || this.m_Interval > 0L;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), TrickleDown.NoTrickleDown);
		}

		private void Invoke(EventBase evt)
		{
			bool flag = this.clicked != null;
			if (flag)
			{
				this.clicked();
			}
			Action<EventBase> action = this.clickedWithEventInfo;
			if (action != null)
			{
				action(evt);
			}
		}

		protected void OnMouseDown(MouseDownEvent evt)
		{
			bool flag = evt != null && base.CanStartManipulation(evt);
			if (flag)
			{
				this.active = true;
				base.target.CaptureMouse();
				this.lastMousePosition = evt.localMousePosition;
				bool flag2 = this.IsRepeatable();
				if (flag2)
				{
					bool flag3 = base.target.ContainsPoint(evt.localMousePosition);
					if (flag3)
					{
						this.Invoke(evt);
					}
					bool flag4 = this.m_Repeater == null;
					if (flag4)
					{
						this.m_Repeater = base.target.schedule.Execute(new Action<TimerState>(this.OnTimer)).Every(this.m_Interval).StartingIn(this.m_Delay);
					}
					else
					{
						this.m_Repeater.ExecuteLater(this.m_Delay);
					}
				}
				base.target.pseudoStates |= PseudoStates.Active;
				evt.StopImmediatePropagation();
			}
		}

		protected void OnMouseMove(MouseMoveEvent evt)
		{
			bool flag = evt != null && this.active;
			if (flag)
			{
				this.lastMousePosition = evt.localMousePosition;
				bool flag2 = base.target.ContainsPoint(evt.localMousePosition);
				if (flag2)
				{
					base.target.pseudoStates |= PseudoStates.Active;
				}
				else
				{
					base.target.pseudoStates &= ~PseudoStates.Active;
				}
				evt.StopPropagation();
			}
		}

		protected void OnMouseUp(MouseUpEvent evt)
		{
			bool flag = evt != null && this.active && base.CanStopManipulation(evt);
			if (flag)
			{
				this.active = false;
				base.target.ReleaseMouse();
				base.target.pseudoStates &= ~PseudoStates.Active;
				bool flag2 = this.IsRepeatable();
				if (flag2)
				{
					bool flag3 = this.m_Repeater != null;
					if (flag3)
					{
						this.m_Repeater.Pause();
					}
				}
				else
				{
					bool flag4 = base.target.ContainsPoint(evt.localMousePosition);
					if (flag4)
					{
						this.Invoke(evt);
					}
				}
				evt.StopPropagation();
			}
		}

		private readonly long m_Delay;

		private readonly long m_Interval;

		private IVisualElementScheduledItem m_Repeater;
	}
}
