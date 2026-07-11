using System;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class MouseEventBase<T> : EventBase<T>, IMouseEvent, IMouseEventInternal where T : MouseEventBase<T>, new()
	{
		protected MouseEventBase()
		{
			this.Init();
		}

		public EventModifiers modifiers { get; protected set; }

		public Vector2 mousePosition { get; protected set; }

		public Vector2 localMousePosition { get; internal set; }

		public Vector2 mouseDelta { get; protected set; }

		public int clickCount { get; protected set; }

		public int button { get; protected set; }

		public bool shiftKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Shift) != EventModifiers.None;
			}
		}

		public bool ctrlKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Control) != EventModifiers.None;
			}
		}

		public bool commandKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Command) != EventModifiers.None;
			}
		}

		public bool altKey
		{
			get
			{
				return (this.modifiers & EventModifiers.Alt) != EventModifiers.None;
			}
		}

		public bool actionKey
		{
			get
			{
				bool flag;
				if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
				{
					flag = this.commandKey;
				}
				else
				{
					flag = this.ctrlKey;
				}
				return flag;
			}
		}

		bool IMouseEventInternal.hasUnderlyingPhysicalEvent { get; set; }

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.TricklesDown | EventBase.EventFlags.Cancellable;
			this.modifiers = EventModifiers.None;
			this.mousePosition = Vector2.zero;
			this.localMousePosition = Vector2.zero;
			this.mouseDelta = Vector2.zero;
			this.clickCount = 0;
			this.button = 0;
			((IMouseEventInternal)this).hasUnderlyingPhysicalEvent = false;
		}

		public override IEventHandler currentTarget
		{
			get
			{
				return base.currentTarget;
			}
			internal set
			{
				base.currentTarget = value;
				VisualElement visualElement = this.currentTarget as VisualElement;
				if (visualElement != null)
				{
					this.localMousePosition = visualElement.WorldToLocal(this.mousePosition);
				}
			}
		}

		public static T GetPooled(Event systemEvent)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.imguiEvent = systemEvent;
			if (systemEvent != null)
			{
				pooled.modifiers = systemEvent.modifiers;
				pooled.mousePosition = systemEvent.mousePosition;
				pooled.localMousePosition = systemEvent.mousePosition;
				pooled.mouseDelta = systemEvent.delta;
				pooled.button = systemEvent.button;
				pooled.clickCount = systemEvent.clickCount;
				pooled.hasUnderlyingPhysicalEvent = true;
			}
			return pooled;
		}

		public static T GetPooled(Vector2 mousePosition)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.mousePosition = mousePosition;
			return pooled;
		}

		public static T GetPooled(IMouseEvent triggerEvent)
		{
			T pooled = EventBase<T>.GetPooled();
			if (triggerEvent != null)
			{
				pooled.modifiers = triggerEvent.modifiers;
				pooled.mousePosition = triggerEvent.mousePosition;
				pooled.localMousePosition = triggerEvent.mousePosition;
				pooled.mouseDelta = triggerEvent.mouseDelta;
				pooled.button = triggerEvent.button;
				pooled.clickCount = triggerEvent.clickCount;
				IMouseEventInternal mouseEventInternal = triggerEvent as IMouseEventInternal;
				if (mouseEventInternal != null)
				{
					pooled.hasUnderlyingPhysicalEvent = mouseEventInternal.hasUnderlyingPhysicalEvent;
				}
			}
			return pooled;
		}
	}
}
