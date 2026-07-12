using System;

namespace UnityEngine.UIElements
{
	public abstract class CallbackEventHandler : IEventHandler
	{
		public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = this.m_CallbackRegistry == null;
			if (flag)
			{
				this.m_CallbackRegistry = new EventCallbackRegistry();
			}
			this.m_CallbackRegistry.RegisterCallback<TEventType>(callback, useTrickleDown, InvokePolicy.Default);
			this.AddEventCategories<TEventType>();
		}

		private void AddEventCategories<TEventType>() where TEventType : EventBase<TEventType>, new()
		{
			VisualElement visualElement = this as VisualElement;
			bool flag = visualElement != null;
			if (flag)
			{
				visualElement.eventCallbackCategories |= 1 << (int)EventBase<TEventType>.EventCategory;
			}
		}

		public void RegisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = this.m_CallbackRegistry == null;
			if (flag)
			{
				this.m_CallbackRegistry = new EventCallbackRegistry();
			}
			this.m_CallbackRegistry.RegisterCallback<TEventType, TUserArgsType>(callback, userArgs, useTrickleDown, InvokePolicy.Default);
			this.AddEventCategories<TEventType>();
		}

		internal void RegisterCallback<TEventType>(EventCallback<TEventType> callback, InvokePolicy invokePolicy, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = this.m_CallbackRegistry == null;
			if (flag)
			{
				this.m_CallbackRegistry = new EventCallbackRegistry();
			}
			this.m_CallbackRegistry.RegisterCallback<TEventType>(callback, useTrickleDown, invokePolicy);
			this.AddEventCategories<TEventType>();
		}

		public void UnregisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = this.m_CallbackRegistry != null;
			if (flag)
			{
				this.m_CallbackRegistry.UnregisterCallback<TEventType>(callback, useTrickleDown);
			}
		}

		public void UnregisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = this.m_CallbackRegistry != null;
			if (flag)
			{
				this.m_CallbackRegistry.UnregisterCallback<TEventType, TUserArgsType>(callback, useTrickleDown);
			}
		}

		internal bool TryGetUserArgs<TEventType, TCallbackArgs>(EventCallback<TEventType, TCallbackArgs> callback, TrickleDown useTrickleDown, out TCallbackArgs userData) where TEventType : EventBase<TEventType>, new()
		{
			userData = default(TCallbackArgs);
			bool flag = this.m_CallbackRegistry != null;
			return flag && this.m_CallbackRegistry.TryGetUserArgs<TEventType, TCallbackArgs>(callback, useTrickleDown, out userData);
		}

		public abstract void SendEvent(EventBase e);

		internal abstract void SendEvent(EventBase e, DispatchMode dispatchMode);

		internal void HandleEventAtTargetPhase(EventBase evt)
		{
			evt.currentTarget = evt.target;
			evt.propagationPhase = PropagationPhase.AtTarget;
			this.HandleEventAtCurrentTargetAndPhase(evt);
			evt.propagationPhase = PropagationPhase.DefaultActionAtTarget;
			this.HandleEventAtCurrentTargetAndPhase(evt);
		}

		internal void HandleEventAtTargetAndDefaultPhase(EventBase evt)
		{
			this.HandleEventAtTargetPhase(evt);
			evt.propagationPhase = PropagationPhase.DefaultAction;
			this.HandleEventAtCurrentTargetAndPhase(evt);
		}

		internal void HandleEventAtCurrentTargetAndPhase(EventBase evt)
		{
			this.HandleEvent(evt);
		}

		void IEventHandler.HandleEvent(EventBase evt)
		{
			this.HandleEventAtCurrentTargetAndPhase(evt);
		}

		[Obsolete("The virtual method CallbackEventHandler.HandleEvent is deprecated and will be removed in a future release. Please override ExecuteDefaultAction instead.")]
		public virtual void HandleEvent(EventBase evt)
		{
			bool flag = evt == null;
			if (!flag)
			{
				switch (evt.propagationPhase)
				{
				case PropagationPhase.TrickleDown:
				case PropagationPhase.BubbleUp:
				{
					bool flag2 = !evt.isPropagationStopped;
					if (flag2)
					{
						EventCallbackRegistry callbackRegistry = this.m_CallbackRegistry;
						if (callbackRegistry != null)
						{
							callbackRegistry.InvokeCallbacks(evt, evt.propagationPhase);
						}
					}
					bool flag3 = this.isIMGUIContainer && !evt.isPropagationStopped;
					if (flag3)
					{
						((IMGUIContainer)this).ProcessEvent(evt);
					}
					break;
				}
				case PropagationPhase.AtTarget:
				{
					bool flag4 = !evt.isPropagationStopped;
					if (flag4)
					{
						EventCallbackRegistry callbackRegistry2 = this.m_CallbackRegistry;
						if (callbackRegistry2 != null)
						{
							callbackRegistry2.InvokeCallbacks(evt, PropagationPhase.TrickleDown);
						}
					}
					bool flag5 = !evt.isPropagationStopped;
					if (flag5)
					{
						EventCallbackRegistry callbackRegistry3 = this.m_CallbackRegistry;
						if (callbackRegistry3 != null)
						{
							callbackRegistry3.InvokeCallbacks(evt, PropagationPhase.BubbleUp);
						}
					}
					bool flag6 = this.isIMGUIContainer && !evt.isPropagationStopped;
					if (flag6)
					{
						((IMGUIContainer)this).ProcessEvent(evt);
					}
					break;
				}
				case PropagationPhase.DefaultAction:
				{
					bool flag7 = !evt.isDefaultPrevented;
					if (flag7)
					{
						using (new EventDebuggerLogExecuteDefaultAction(evt))
						{
							bool flag8;
							if (evt.skipDisabledElements)
							{
								VisualElement visualElement = this as VisualElement;
								if (visualElement != null)
								{
									flag8 = !visualElement.enabledInHierarchy;
									goto IL_01AC;
								}
							}
							flag8 = false;
							IL_01AC:
							bool flag9 = flag8;
							if (flag9)
							{
								this.ExecuteDefaultActionDisabled(evt);
							}
							else
							{
								this.ExecuteDefaultAction(evt);
							}
						}
					}
					break;
				}
				case PropagationPhase.DefaultActionAtTarget:
				{
					bool flag10 = !evt.isDefaultPrevented;
					if (flag10)
					{
						using (new EventDebuggerLogExecuteDefaultAction(evt))
						{
							bool flag11;
							if (evt.skipDisabledElements)
							{
								VisualElement visualElement2 = this as VisualElement;
								if (visualElement2 != null)
								{
									flag11 = !visualElement2.enabledInHierarchy;
									goto IL_0144;
								}
							}
							flag11 = false;
							IL_0144:
							bool flag12 = flag11;
							if (flag12)
							{
								this.ExecuteDefaultActionDisabledAtTarget(evt);
							}
							else
							{
								this.ExecuteDefaultActionAtTarget(evt);
							}
						}
					}
					break;
				}
				}
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

		[EventInterest(EventInterestOptions.Inherit)]
		protected virtual void ExecuteDefaultActionAtTarget(EventBase evt)
		{
		}

		[EventInterest(EventInterestOptions.Inherit)]
		protected virtual void ExecuteDefaultAction(EventBase evt)
		{
		}

		[EventInterest(EventInterestOptions.Inherit)]
		internal virtual void ExecuteDefaultActionDisabledAtTarget(EventBase evt)
		{
		}

		[EventInterest(EventInterestOptions.Inherit)]
		internal virtual void ExecuteDefaultActionDisabled(EventBase evt)
		{
		}

		internal bool isIMGUIContainer = false;

		private EventCallbackRegistry m_CallbackRegistry;

		internal const string ExecuteDefaultActionName = "ExecuteDefaultAction";

		internal const string ExecuteDefaultActionAtTargetName = "ExecuteDefaultActionAtTarget";
	}
}
