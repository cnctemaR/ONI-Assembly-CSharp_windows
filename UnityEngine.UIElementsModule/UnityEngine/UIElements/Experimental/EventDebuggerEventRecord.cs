using System;

namespace UnityEngine.UIElements.Experimental
{
	[Serializable]
	internal class EventDebuggerEventRecord
	{
		public string eventBaseName { get; private set; }

		public long eventTypeId { get; private set; }

		public ulong eventId { get; private set; }

		private ulong triggerEventId { get; set; }

		internal long timestamp { get; private set; }

		public IEventHandler target { get; set; }

		private bool isPropagationStopped { get; set; }

		private bool isImmediatePropagationStopped { get; set; }

		public PropagationPhase propagationPhase { get; private set; }

		private IEventHandler currentTarget { get; set; }

		private bool dispatch { get; set; }

		private Vector2 originalMousePosition { get; set; }

		public EventModifiers modifiers { get; private set; }

		public Vector2 mousePosition { get; private set; }

		public int clickCount { get; private set; }

		public int button { get; private set; }

		public int pressedButtons { get; private set; }

		public int pointerId { get; private set; }

		public Vector3 delta { get; private set; }

		public char character { get; private set; }

		public KeyCode keyCode { get; private set; }

		public string commandName { get; private set; }

		public NavigationDeviceType deviceType { get; private set; }

		public NavigationMoveEvent.Direction navigationDirection { get; private set; }

		private void Init(EventBase evt)
		{
			Type type = evt.GetType();
			this.eventBaseName = EventDebugger.GetTypeDisplayName(type);
			this.eventTypeId = evt.eventTypeId;
			this.eventId = evt.eventId;
			this.triggerEventId = evt.triggerEventId;
			this.timestamp = evt.timestamp;
			this.target = evt.target;
			this.isPropagationStopped = evt.isPropagationStopped;
			this.isImmediatePropagationStopped = evt.isImmediatePropagationStopped;
			this.propagationPhase = evt.propagationPhase;
			this.originalMousePosition = evt.originalMousePosition;
			this.currentTarget = evt.currentTarget;
			this.dispatch = evt.dispatch;
			IMouseEvent mouseEvent = evt as IMouseEvent;
			bool flag = mouseEvent != null;
			if (flag)
			{
				this.modifiers = mouseEvent.modifiers;
				this.mousePosition = mouseEvent.mousePosition;
				this.button = mouseEvent.button;
				this.pressedButtons = mouseEvent.pressedButtons;
				this.clickCount = mouseEvent.clickCount;
				WheelEvent wheelEvent = mouseEvent as WheelEvent;
				bool flag2 = wheelEvent != null;
				if (flag2)
				{
					this.delta = wheelEvent.delta;
				}
			}
			IPointerEvent pointerEvent = evt as IPointerEvent;
			bool flag3 = pointerEvent != null;
			if (flag3)
			{
				this.modifiers = pointerEvent.modifiers;
				this.mousePosition = pointerEvent.position;
				this.button = pointerEvent.button;
				this.pressedButtons = pointerEvent.pressedButtons;
				this.clickCount = pointerEvent.clickCount;
				this.pointerId = pointerEvent.pointerId;
			}
			IKeyboardEvent keyboardEvent = evt as IKeyboardEvent;
			bool flag4 = keyboardEvent != null;
			if (flag4)
			{
				this.modifiers = keyboardEvent.modifiers;
				this.character = keyboardEvent.character;
				this.keyCode = keyboardEvent.keyCode;
			}
			ICommandEvent commandEvent = evt as ICommandEvent;
			bool flag5 = commandEvent != null;
			if (flag5)
			{
				this.commandName = commandEvent.commandName;
			}
			INavigationEvent navigationEvent = evt as INavigationEvent;
			bool flag6 = navigationEvent != null;
			if (flag6)
			{
				this.deviceType = navigationEvent.deviceType;
				NavigationMoveEvent navigationMoveEvent = navigationEvent as NavigationMoveEvent;
				bool flag7 = navigationMoveEvent != null;
				if (flag7)
				{
					this.navigationDirection = navigationMoveEvent.direction;
				}
			}
		}

		public EventDebuggerEventRecord(EventBase evt)
		{
			this.Init(evt);
		}

		public string TimestampString()
		{
			long num = (long)((float)this.timestamp / 1000f * 10000000f);
			return new DateTime(num).ToString("HH:mm:ss.ffffff");
		}
	}
}
