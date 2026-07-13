using System;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	public class Clickable : PointerManipulator
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EventBase> clickedWithEventInfo;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action clicked;

		protected bool active { get; set; }

		public Vector2 lastMousePosition { get; private set; }

		internal bool acceptClicksIfDisabled
		{
			get
			{
				return this.m_AcceptClicksIfDisabled;
			}
			set
			{
				bool flag = this.m_AcceptClicksIfDisabled == value;
				if (!flag)
				{
					bool flag2 = base.target != null;
					if (flag2)
					{
						this.UnregisterCallbacksFromTarget();
					}
					this.m_AcceptClicksIfDisabled = value;
					bool flag3 = base.target != null;
					if (flag3)
					{
						this.RegisterCallbacksOnTarget();
					}
				}
			}
		}

		private InvokePolicy invokePolicy
		{
			get
			{
				return this.acceptClicksIfDisabled ? InvokePolicy.IncludeDisabled : InvokePolicy.Default;
			}
		}

		public Clickable(Action handler, long delay, long interval)
			: this(handler)
		{
			this.m_Delay = delay;
			this.m_Interval = interval;
			this.active = false;
		}

		public Clickable(Action<EventBase> handler)
		{
			this.m_ActivePointerId = -1;
			base..ctor();
			this.clickedWithEventInfo = handler;
			base.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse
			});
		}

		public Clickable(Action handler)
		{
			this.m_ActivePointerId = -1;
			base..ctor();
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
				bool flag2 = this.ContainsPointer(this.m_ActivePointerId) && (base.target.enabledInHierarchy || this.acceptClicksIfDisabled);
				if (flag2)
				{
					this.Invoke(null);
					base.target.SetActivePseudoState(true);
				}
				else
				{
					base.target.SetActivePseudoState(false);
				}
			}
		}

		private bool IsRepeatable()
		{
			return this.m_Delay > 0L || this.m_Interval > 0L;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			base.target.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), this.invokePolicy, TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), this.invokePolicy, TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), InvokePolicy.IncludeDisabled, TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), InvokePolicy.IncludeDisabled, TrickleDown.NoTrickleDown);
			base.target.RegisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), InvokePolicy.IncludeDisabled, TrickleDown.NoTrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.target.UnregisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerCancelEvent>(new EventCallback<PointerCancelEvent>(this.OnPointerCancel), TrickleDown.NoTrickleDown);
			base.target.UnregisterCallback<PointerCaptureOutEvent>(new EventCallback<PointerCaptureOutEvent>(this.OnPointerCaptureOut), TrickleDown.NoTrickleDown);
			this.ResetActivePseudoState();
		}

		[Obsolete("OnMouseDown has been removed and replaced by its pointer-based equivalent. Please use OnPointerDown.", false)]
		protected void OnMouseDown(MouseDownEvent evt)
		{
			bool flag = !this.active && base.CanStartManipulation(evt);
			if (flag)
			{
				this.ProcessDownEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
			}
		}

		[Obsolete("OnMouseMove has been removed and replaced by its pointer-based equivalent. Please use OnPointerMove.", false)]
		protected void OnMouseMove(MouseMoveEvent evt)
		{
			bool active = this.active;
			if (active)
			{
				this.ProcessMoveEvent(evt, evt.localMousePosition);
			}
		}

		[Obsolete("OnMouseUp has been removed and replaced by its pointer-based equivalent. Please use OnPointerUp.", false)]
		protected void OnMouseUp(MouseUpEvent evt)
		{
			bool flag = this.active && base.CanStopManipulation(evt);
			if (flag)
			{
				this.ProcessUpEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
			}
		}

		protected void OnPointerDown(PointerDownEvent evt)
		{
			bool flag = this.active || !base.CanStartManipulation(evt);
			if (!flag)
			{
				this.ProcessDownEvent(evt, evt.localPosition, evt.pointerId);
			}
		}

		protected void OnPointerMove(PointerMoveEvent evt)
		{
			bool flag = !this.active;
			if (!flag)
			{
				this.ProcessMoveEvent(evt, evt.localPosition);
			}
		}

		protected void OnPointerUp(PointerUpEvent evt)
		{
			bool flag = !this.active || !base.CanStopManipulation(evt);
			if (!flag)
			{
				this.ProcessUpEvent(evt, evt.localPosition, evt.pointerId);
			}
		}

		private void OnPointerCancel(PointerCancelEvent evt)
		{
			bool flag = !this.active || !base.CanStopManipulation(evt);
			if (!flag)
			{
				this.ProcessCancelEvent(evt, evt.pointerId);
			}
		}

		private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
		{
			bool flag = !this.active;
			if (!flag)
			{
				this.ProcessCancelEvent(evt, evt.pointerId);
			}
		}

		private bool ContainsPointer(int pointerId)
		{
			VisualElement topElementUnderPointer = base.target.elementPanel.GetTopElementUnderPointer(pointerId);
			return base.target == topElementUnderPointer || base.target.Contains(topElementUnderPointer);
		}

		protected void Invoke(EventBase evt)
		{
			Action action = this.clicked;
			if (action != null)
			{
				action();
			}
			Action<EventBase> action2 = this.clickedWithEventInfo;
			if (action2 != null)
			{
				action2(evt);
			}
		}

		internal void SimulateSingleClick(EventBase evt, int delayMs = 100)
		{
			base.target.SetActivePseudoState(true);
			this.m_PendingActivePseudoStateReset = base.target.schedule.Execute(new Action(this.ResetActivePseudoState));
			this.m_PendingActivePseudoStateReset.ExecuteLater((long)delayMs);
			this.Invoke(evt);
		}

		private void ResetActivePseudoState()
		{
			bool flag = this.m_PendingActivePseudoStateReset == null;
			if (!flag)
			{
				base.target.SetActivePseudoState(false);
				this.m_PendingActivePseudoStateReset = null;
			}
		}

		protected virtual void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
		{
			this.active = true;
			this.m_ActivePointerId = pointerId;
			base.target.CapturePointer(pointerId);
			bool flag = !(evt is IPointerEvent);
			if (flag)
			{
				base.target.panel.ProcessPointerCapture(pointerId);
			}
			this.lastMousePosition = localPosition;
			bool flag2 = this.IsRepeatable();
			if (flag2)
			{
				bool flag3 = this.ContainsPointer(pointerId) && (base.target.enabledInHierarchy || this.acceptClicksIfDisabled);
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
			base.target.SetActivePseudoState(true);
			evt.StopImmediatePropagation();
		}

		protected virtual void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
		{
			this.lastMousePosition = localPosition;
			bool flag = this.ContainsPointer(this.m_ActivePointerId);
			if (flag)
			{
				base.target.SetActivePseudoState(true);
			}
			else
			{
				base.target.SetActivePseudoState(false);
			}
			evt.StopPropagation();
		}

		protected virtual void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
		{
			this.active = false;
			this.m_ActivePointerId = -1;
			base.target.ReleasePointer(pointerId);
			bool flag = !(evt is IPointerEvent);
			if (flag)
			{
				base.target.panel.ProcessPointerCapture(pointerId);
			}
			base.target.SetActivePseudoState(false);
			bool flag2 = this.IsRepeatable();
			if (flag2)
			{
				IVisualElementScheduledItem repeater = this.m_Repeater;
				if (repeater != null)
				{
					repeater.Pause();
				}
			}
			else
			{
				bool flag3 = this.ContainsPointer(pointerId) && (base.target.enabledInHierarchy || this.acceptClicksIfDisabled);
				if (flag3)
				{
					this.Invoke(evt);
				}
			}
			evt.StopPropagation();
		}

		protected virtual void ProcessCancelEvent(EventBase evt, int pointerId)
		{
			this.active = false;
			this.m_ActivePointerId = -1;
			base.target.ReleasePointer(pointerId);
			bool flag = !(evt is IPointerEvent);
			if (flag)
			{
				base.target.panel.ProcessPointerCapture(pointerId);
			}
			base.target.SetActivePseudoState(false);
			bool flag2 = this.IsRepeatable();
			if (flag2)
			{
				IVisualElementScheduledItem repeater = this.m_Repeater;
				if (repeater != null)
				{
					repeater.Pause();
				}
			}
			evt.StopPropagation();
		}

		private readonly long m_Delay;

		private readonly long m_Interval;

		private int m_ActivePointerId;

		private bool m_AcceptClicksIfDisabled;

		private IVisualElementScheduledItem m_Repeater;

		private IVisualElementScheduledItem m_PendingActivePseudoStateReset;
	}
}
