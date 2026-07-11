using System;
using System.Diagnostics;
using System.Runtime.Diagnostics;
using System.Security;
using System.Threading;

namespace System.Runtime
{
	internal abstract class ActionItem
	{
		public bool LowPriority
		{
			get
			{
				return this.lowPriority;
			}
			protected set
			{
				this.lowPriority = value;
			}
		}

		public static void Schedule(Action<object> callback, object state)
		{
			ActionItem.Schedule(callback, state, false);
		}

		[SecuritySafeCritical]
		public static void Schedule(Action<object> callback, object state, bool lowPriority)
		{
			if (PartialTrustHelpers.ShouldFlowSecurityContext || WaitCallbackActionItem.ShouldUseActivity || Fx.Trace.IsEnd2EndActivityTracingEnabled)
			{
				new ActionItem.DefaultActionItem(callback, state, lowPriority).Schedule();
				return;
			}
			ActionItem.ScheduleCallback(callback, state, lowPriority);
		}

		[SecurityCritical]
		protected abstract void Invoke();

		[SecurityCritical]
		protected void Schedule()
		{
			if (this.isScheduled)
			{
				throw Fx.Exception.AsError(new InvalidOperationException("Action Item Is Already Scheduled"));
			}
			this.isScheduled = true;
			this.ScheduleCallback(ActionItem.CallbackHelper.InvokeWithoutContextCallback);
		}

		[SecurityCritical]
		protected void ScheduleWithoutContext()
		{
			if (this.isScheduled)
			{
				throw Fx.Exception.AsError(new InvalidOperationException("Action Item Is Already Scheduled"));
			}
			this.isScheduled = true;
			this.ScheduleCallback(ActionItem.CallbackHelper.InvokeWithoutContextCallback);
		}

		[SecurityCritical]
		private static void ScheduleCallback(Action<object> callback, object state, bool lowPriority)
		{
			if (lowPriority)
			{
				IOThreadScheduler.ScheduleCallbackLowPriNoFlow(callback, state);
				return;
			}
			IOThreadScheduler.ScheduleCallbackNoFlow(callback, state);
		}

		[SecurityCritical]
		private void ScheduleCallback(Action<object> callback)
		{
			ActionItem.ScheduleCallback(callback, this, this.lowPriority);
		}

		private bool isScheduled;

		private bool lowPriority;

		[SecurityCritical]
		private static class CallbackHelper
		{
			public static Action<object> InvokeWithoutContextCallback
			{
				get
				{
					if (ActionItem.CallbackHelper.invokeWithoutContextCallback == null)
					{
						ActionItem.CallbackHelper.invokeWithoutContextCallback = new Action<object>(ActionItem.CallbackHelper.InvokeWithoutContext);
					}
					return ActionItem.CallbackHelper.invokeWithoutContextCallback;
				}
			}

			public static ContextCallback OnContextAppliedCallback
			{
				get
				{
					if (ActionItem.CallbackHelper.onContextAppliedCallback == null)
					{
						ActionItem.CallbackHelper.onContextAppliedCallback = new ContextCallback(ActionItem.CallbackHelper.OnContextApplied);
					}
					return ActionItem.CallbackHelper.onContextAppliedCallback;
				}
			}

			private static void InvokeWithoutContext(object state)
			{
				((ActionItem)state).Invoke();
				((ActionItem)state).isScheduled = false;
			}

			private static void OnContextApplied(object o)
			{
				((ActionItem)o).Invoke();
				((ActionItem)o).isScheduled = false;
			}

			private static Action<object> invokeWithoutContextCallback;

			private static ContextCallback onContextAppliedCallback;
		}

		private class DefaultActionItem : ActionItem
		{
			[SecuritySafeCritical]
			public DefaultActionItem(Action<object> callback, object state, bool isLowPriority)
			{
				base.LowPriority = isLowPriority;
				this.callback = callback;
				this.state = state;
				if (WaitCallbackActionItem.ShouldUseActivity)
				{
					this.flowLegacyActivityId = true;
					this.activityId = DiagnosticTraceBase.ActivityId;
				}
				if (Fx.Trace.IsEnd2EndActivityTracingEnabled)
				{
					this.eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate(false);
					if (TraceCore.ActionItemScheduledIsEnabled(Fx.Trace))
					{
						TraceCore.ActionItemScheduled(Fx.Trace, this.eventTraceActivity);
					}
				}
			}

			[SecurityCritical]
			protected override void Invoke()
			{
				if (this.flowLegacyActivityId || Fx.Trace.IsEnd2EndActivityTracingEnabled)
				{
					this.TraceAndInvoke();
					return;
				}
				this.callback(this.state);
			}

			[SecurityCritical]
			private void TraceAndInvoke()
			{
				if (this.flowLegacyActivityId)
				{
					Guid guid = DiagnosticTraceBase.ActivityId;
					try
					{
						DiagnosticTraceBase.ActivityId = this.activityId;
						this.callback(this.state);
						return;
					}
					finally
					{
						DiagnosticTraceBase.ActivityId = guid;
					}
				}
				Guid empty = Guid.Empty;
				bool flag = false;
				try
				{
					if (this.eventTraceActivity != null)
					{
						empty = Trace.CorrelationManager.ActivityId;
						flag = true;
						Trace.CorrelationManager.ActivityId = this.eventTraceActivity.ActivityId;
						if (TraceCore.ActionItemCallbackInvokedIsEnabled(Fx.Trace))
						{
							TraceCore.ActionItemCallbackInvoked(Fx.Trace, this.eventTraceActivity);
						}
					}
					this.callback(this.state);
				}
				finally
				{
					if (flag)
					{
						Trace.CorrelationManager.ActivityId = empty;
					}
				}
			}

			[SecurityCritical]
			private Action<object> callback;

			[SecurityCritical]
			private object state;

			private bool flowLegacyActivityId;

			private Guid activityId;

			private EventTraceActivity eventTraceActivity;
		}
	}
}
