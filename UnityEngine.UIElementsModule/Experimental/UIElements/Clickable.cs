using System;
using System.Diagnostics;

namespace UnityEngine.Experimental.UIElements
{
	public class Clickable : MouseManipulator
	{
		public Clickable(Action handler, long delay, long interval)
			: this(handler)
		{
			this.m_Delay = delay;
			this.m_Interval = interval;
			this.m_Active = false;
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
			this.m_Active = false;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EventBase> clickedWithEventInfo;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action clicked;

		public Vector2 lastMousePosition { get; private set; }

		private void OnTimer(TimerState timerState)
		{
			if (this.clicked != null && this.IsRepeatable())
			{
				if (base.target.ContainsPoint(this.lastMousePosition))
				{
					this.clicked();
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
			base.target.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), Capture.NoCapture);
			base.target.RegisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), Capture.NoCapture);
			base.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), Capture.NoCapture);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown), Capture.NoCapture);
			base.target.UnregisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove), Capture.NoCapture);
			base.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp), Capture.NoCapture);
		}

		protected void OnMouseDown(MouseDownEvent evt)
		{
			if (base.CanStartManipulation(evt))
			{
				this.m_Active = true;
				base.target.TakeMouseCapture();
				this.lastMousePosition = evt.localMousePosition;
				if (this.IsRepeatable())
				{
					if (base.target.ContainsPoint(evt.localMousePosition))
					{
						if (this.clicked != null)
						{
							this.clicked();
						}
						else if (this.clickedWithEventInfo != null)
						{
							this.clickedWithEventInfo(evt);
						}
					}
					if (this.m_Repeater == null)
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
			if (this.m_Active)
			{
				this.lastMousePosition = evt.localMousePosition;
				evt.StopPropagation();
			}
		}

		protected void OnMouseUp(MouseUpEvent evt)
		{
			if (this.m_Active && base.CanStopManipulation(evt))
			{
				this.m_Active = false;
				base.target.ReleaseMouseCapture();
				if (this.IsRepeatable())
				{
					if (this.m_Repeater != null)
					{
						this.m_Repeater.Pause();
					}
				}
				else if (base.target.ContainsPoint(evt.localMousePosition))
				{
					if (this.clicked != null)
					{
						this.clicked();
					}
					else if (this.clickedWithEventInfo != null)
					{
						this.clickedWithEventInfo(evt);
					}
				}
				base.target.pseudoStates &= ~PseudoStates.Active;
				evt.StopPropagation();
			}
		}

		private readonly long m_Delay;

		private readonly long m_Interval;

		protected bool m_Active;

		private IVisualElementScheduledItem m_Repeater;
	}
}
