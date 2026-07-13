using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public abstract class CallbackEventHandler : IEventHandler
	{
		public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentException("callback parameter is null");
			}
			EventCallbackRegistry eventCallbackRegistry;
			if ((eventCallbackRegistry = this.m_CallbackRegistry) == null)
			{
				eventCallbackRegistry = (this.m_CallbackRegistry = new EventCallbackRegistry());
			}
			eventCallbackRegistry.RegisterCallback<TEventType>(callback, useTrickleDown, InvokePolicy.Default);
			this.AddEventCategories<TEventType>(useTrickleDown);
		}

		public void RegisterCallbackOnce<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentException("callback parameter is null");
			}
			EventCallbackRegistry eventCallbackRegistry;
			if ((eventCallbackRegistry = this.m_CallbackRegistry) == null)
			{
				eventCallbackRegistry = (this.m_CallbackRegistry = new EventCallbackRegistry());
			}
			eventCallbackRegistry.RegisterCallback<TEventType>(callback, useTrickleDown, InvokePolicy.Once);
			this.AddEventCategories<TEventType>(useTrickleDown);
		}

		private void AddEventCategories<TEventType>(TrickleDown useTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			VisualElement visualElement = this as VisualElement;
			bool flag = visualElement != null;
			if (flag)
			{
				visualElement.AddEventCallbackCategories(1 << (int)EventBase<TEventType>.EventCategory, useTrickleDown);
			}
		}

		public void RegisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentException("callback parameter is null");
			}
			EventCallbackRegistry eventCallbackRegistry;
			if ((eventCallbackRegistry = this.m_CallbackRegistry) == null)
			{
				eventCallbackRegistry = (this.m_CallbackRegistry = new EventCallbackRegistry());
			}
			eventCallbackRegistry.RegisterCallback<TEventType, TUserArgsType>(callback, userArgs, useTrickleDown, InvokePolicy.Default);
			this.AddEventCategories<TEventType>(useTrickleDown);
		}

		public void RegisterCallbackOnce<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentException("callback parameter is null");
			}
			EventCallbackRegistry eventCallbackRegistry;
			if ((eventCallbackRegistry = this.m_CallbackRegistry) == null)
			{
				eventCallbackRegistry = (this.m_CallbackRegistry = new EventCallbackRegistry());
			}
			eventCallbackRegistry.RegisterCallback<TEventType, TUserArgsType>(callback, userArgs, useTrickleDown, InvokePolicy.Once);
			this.AddEventCategories<TEventType>(useTrickleDown);
		}

		internal void RegisterCallback<TEventType>(EventCallback<TEventType> callback, InvokePolicy invokePolicy, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			EventCallbackRegistry eventCallbackRegistry;
			if ((eventCallbackRegistry = this.m_CallbackRegistry) == null)
			{
				eventCallbackRegistry = (this.m_CallbackRegistry = new EventCallbackRegistry());
			}
			eventCallbackRegistry.RegisterCallback<TEventType>(callback, useTrickleDown, invokePolicy);
			this.AddEventCategories<TEventType>(useTrickleDown);
		}

		public void UnregisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentException("callback parameter is null");
			}
			EventCallbackRegistry callbackRegistry = this.m_CallbackRegistry;
			if (callbackRegistry != null)
			{
				callbackRegistry.UnregisterCallback<TEventType>(callback, useTrickleDown);
			}
		}

		public void UnregisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentException("callback parameter is null");
			}
			EventCallbackRegistry callbackRegistry = this.m_CallbackRegistry;
			if (callbackRegistry != null)
			{
				callbackRegistry.UnregisterCallback<TEventType, TUserArgsType>(callback, useTrickleDown);
			}
		}

		public abstract void SendEvent(EventBase e);

		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		internal abstract void SendEvent(EventBase e, DispatchMode dispatchMode);

		internal abstract void HandleEvent(EventBase e);

		void IEventHandler.HandleEvent(EventBase evt)
		{
			bool flag = evt == null;
			if (!flag)
			{
				this.HandleEvent(evt);
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
		[Obsolete("Use HandleEventBubbleUp. Before proceeding, make sure you understand the latest changes to UIToolkit event propagation rules by visiting Unity's manual page https://docs.unity3d.com/Manual/UIE-Events-Dispatching.html")]
		protected virtual void ExecuteDefaultActionAtTarget(EventBase evt)
		{
		}

		[EventInterest(EventInterestOptions.Inherit)]
		protected virtual void HandleEventBubbleUp(EventBase evt)
		{
		}

		[EventInterest(EventInterestOptions.Inherit)]
		internal virtual void HandleEventBubbleUpDisabled(EventBase evt)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void HandleEventBubbleUpInternal(EventBase evt)
		{
			this.HandleEventBubbleUp(evt);
		}

		[EventInterest(EventInterestOptions.Inherit)]
		protected virtual void HandleEventTrickleDown(EventBase evt)
		{
		}

		[EventInterest(EventInterestOptions.Inherit)]
		internal virtual void HandleEventTrickleDownDisabled(EventBase evt)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void HandleEventTrickleDownInternal(EventBase evt)
		{
			this.HandleEventTrickleDown(evt);
		}

		[Obsolete("Use HandleEventBubbleUp. Before proceeding, make sure you understand the latest changes to UIToolkit event propagation rules by visiting Unity's manual page https://docs.unity3d.com/Manual/UIE-Events-Dispatching.html")]
		[EventInterest(EventInterestOptions.Inherit)]
		protected virtual void ExecuteDefaultAction(EventBase evt)
		{
		}

		[Obsolete("Use HandleEventBubbleUpDisabled.")]
		[EventInterest(EventInterestOptions.Inherit)]
		internal virtual void ExecuteDefaultActionDisabledAtTarget(EventBase evt)
		{
		}

		[EventInterest(EventInterestOptions.Inherit)]
		[Obsolete("Use HandleEventBubbleUpDisabled.")]
		internal virtual void ExecuteDefaultActionDisabled(EventBase evt)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ExecuteDefaultActionInternal(EventBase evt)
		{
			this.ExecuteDefaultAction(evt);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ExecuteDefaultActionDisabledInternal(EventBase evt)
		{
			this.ExecuteDefaultActionDisabled(evt);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ExecuteDefaultActionAtTargetInternal(EventBase evt)
		{
			this.ExecuteDefaultActionAtTarget(evt);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ExecuteDefaultActionDisabledAtTargetInternal(EventBase evt)
		{
			this.ExecuteDefaultActionDisabledAtTarget(evt);
		}

		protected void NotifyPropertyChanged(in BindingId property)
		{
			VisualElement visualElement = this as VisualElement;
			bool flag = ((visualElement != null) ? visualElement.elementPanel : null) == null;
			if (!flag)
			{
				using (PropertyChangedEvent pooled = PropertyChangedEvent.GetPooled(in property))
				{
					pooled.target = this;
					this.SendEvent(pooled);
				}
			}
		}

		internal bool isIMGUIContainer = false;

		internal EventCallbackRegistry m_CallbackRegistry;

		internal const string HandleEventBubbleUpName = "HandleEventBubbleUp";

		internal const string HandleEventTrickleDownName = "HandleEventTrickleDown";

		internal const string ExecuteDefaultActionName = "ExecuteDefaultAction";

		internal const string ExecuteDefaultActionAtTargetName = "ExecuteDefaultActionAtTarget";
	}
}
