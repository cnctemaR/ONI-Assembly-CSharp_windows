using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Navigation)]
	public abstract class NavigationEventBase<T> : EventBase<T>, INavigationEvent where T : NavigationEventBase<T>, new()
	{
		public EventModifiers modifiers { get; protected set; }

		public bool shiftKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Shift) > EventModifiers.None;
			}
		}

		public bool ctrlKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Control) > EventModifiers.None;
			}
		}

		public bool commandKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Command) > EventModifiers.None;
			}
		}

		public bool altKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Alt) > EventModifiers.None;
			}
		}

		public bool actionKey
		{
			get
			{
				bool flag = Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer;
				bool flag2;
				if (flag)
				{
					flag2 = this.commandKey;
				}
				else
				{
					flag2 = this.ctrlKey;
				}
				return flag2;
			}
		}

		NavigationDeviceType INavigationEvent.deviceType
		{
			get
			{
				return this.deviceType;
			}
		}

		internal NavigationDeviceType deviceType { get; private set; }

		protected NavigationEventBase()
		{
			this.LocalInit();
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.Bubbles | EventBase.EventPropagation.TricklesDown | EventBase.EventPropagation.SkipDisabledElements;
			this.modifiers = EventModifiers.None;
			this.deviceType = NavigationDeviceType.Unknown;
		}

		public static T GetPooled(EventModifiers modifiers = EventModifiers.None)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.modifiers = modifiers;
			pooled.deviceType = NavigationDeviceType.Unknown;
			return pooled;
		}

		internal static T GetPooled(NavigationDeviceType deviceType, EventModifiers modifiers = EventModifiers.None)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.modifiers = modifiers;
			pooled.deviceType = deviceType;
			return pooled;
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToFocusedElementOrPanelRoot(this, panel);
		}
	}
}
