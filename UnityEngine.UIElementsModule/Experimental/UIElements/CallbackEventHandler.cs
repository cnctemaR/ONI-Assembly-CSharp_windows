using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class CallbackEventHandler : IEventHandler
	{
		[Obsolete("Use TrickleDown instead of Capture.")]
		public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			this.RegisterCallback<TEventType>(callback, (TrickleDown)useCapture);
		}

		public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			if (this.m_CallbackRegistry == null)
			{
				this.m_CallbackRegistry = new EventCallbackRegistry();
			}
			this.m_CallbackRegistry.RegisterCallback<TEventType>(callback, useTrickleDown);
		}

		[Obsolete("Use TrickleDown instead of Capture.")]
		public void RegisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			this.RegisterCallback<TEventType, TUserArgsType>(callback, userArgs, (TrickleDown)useCapture);
		}

		public void RegisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			if (this.m_CallbackRegistry == null)
			{
				this.m_CallbackRegistry = new EventCallbackRegistry();
			}
			this.m_CallbackRegistry.RegisterCallback<TEventType, TUserArgsType>(callback, userArgs, useTrickleDown);
		}

		[Obsolete("Use TrickleDown instead of Capture.")]
		public void UnregisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			this.UnregisterCallback<TEventType>(callback, (TrickleDown)useCapture);
		}

		public void UnregisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			if (this.m_CallbackRegistry != null)
			{
				this.m_CallbackRegistry.UnregisterCallback<TEventType>(callback, useTrickleDown);
			}
		}

		[Obsolete("Use TrickleDown instead of Capture.")]
		public void UnregisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, Capture useCapture) where TEventType : EventBase<TEventType>, new()
		{
			this.UnregisterCallback<TEventType, TUserArgsType>(callback, (TrickleDown)useCapture);
		}

		public void UnregisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			if (this.m_CallbackRegistry != null)
			{
				this.m_CallbackRegistry.UnregisterCallback<TEventType, TUserArgsType>(callback, useTrickleDown);
			}
		}

		internal bool TryGetUserArgs<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TrickleDown useTrickleDown, out TCallbackArgs userData) where TEventType : EventBase<TEventType>, new()
		{
			userData = default(TCallbackArgs);
			return this.m_CallbackRegistry != null && this.m_CallbackRegistry.TryGetUserArgs<TEventType, TCallbackArgs>(callback, useTrickleDown, out userData);
		}

		public abstract void SendEvent(EventBase e);

		public virtual void HandleEvent(EventBase evt)
		{
			if (evt.propagationPhase != PropagationPhase.DefaultAction)
			{
				if (!evt.isPropagationStopped)
				{
					if (this.m_CallbackRegistry != null)
					{
						this.m_CallbackRegistry.InvokeCallbacks(evt);
					}
				}
				if (evt.propagationPhase == PropagationPhase.AtTarget && !evt.isDefaultPrevented)
				{
					this.ExecuteDefaultActionAtTarget(evt);
				}
			}
			else if (!evt.isDefaultPrevented)
			{
				this.ExecuteDefaultAction(evt);
			}
		}

		public bool HasTrickleDownHandlers()
		{
			return this.m_CallbackRegistry != null && this.m_CallbackRegistry.HasTrickleDownHandlers();
		}

		public bool HasBubbleUpHandlers()
		{
			return this.m_CallbackRegistry != null && this.m_CallbackRegistry.HasBubbleHandlers();
		}

		[Obsolete("Use HasTrickleDownHandlers instead of HasCaptureHandlers.")]
		public bool HasCaptureHandlers()
		{
			return this.HasTrickleDownHandlers();
		}

		[Obsolete("Use HasBubbleUpHandlers instead of HasBubbleHandlers.")]
		public bool HasBubbleHandlers()
		{
			return this.HasBubbleUpHandlers();
		}

		protected internal virtual void ExecuteDefaultActionAtTarget(EventBase evt)
		{
		}

		protected internal virtual void ExecuteDefaultAction(EventBase evt)
		{
		}

		private EventCallbackRegistry m_CallbackRegistry;
	}
}
