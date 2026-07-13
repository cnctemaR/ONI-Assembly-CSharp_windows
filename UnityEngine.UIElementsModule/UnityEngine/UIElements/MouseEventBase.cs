using System;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Pointer)]
	public abstract class MouseEventBase<T> : EventBase<T>, IMouseEvent, IMouseEventInternal, IPointerOrMouseEvent where T : MouseEventBase<T>, new()
	{
		public EventModifiers modifiers { get; protected set; }

		public Vector2 mousePosition { get; protected set; }

		public Vector2 localMousePosition { get; internal set; }

		public Vector2 mouseDelta { get; protected set; }

		public int clickCount { get; protected set; }

		public int button { get; protected set; }

		public int pressedButtons { get; protected set; }

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

		internal IPointerEvent sourcePointerEvent { get; set; }

		internal bool recomputeTopElementUnderMouse { get; set; }

		IPointerEvent IMouseEventInternal.sourcePointerEvent
		{
			get
			{
				return this.sourcePointerEvent;
			}
		}

		bool IMouseEventInternal.recomputeTopElementUnderMouse
		{
			get
			{
				return this.recomputeTopElementUnderMouse;
			}
		}

		int IPointerOrMouseEvent.pointerId
		{
			get
			{
				return PointerId.mousePointerId;
			}
		}

		Vector3 IPointerOrMouseEvent.position
		{
			get
			{
				return this.mousePosition;
			}
		}

		Vector3 IPointerOrMouseEvent.deltaPosition
		{
			get
			{
				return this.mouseDelta;
			}
			set
			{
				this.mouseDelta = value;
			}
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
			base.propagation = EventBase.EventPropagation.BubblesOrTricklesDown;
			this.modifiers = EventModifiers.None;
			this.mousePosition = Vector2.zero;
			this.localMousePosition = Vector2.zero;
			this.mouseDelta = Vector2.zero;
			this.clickCount = 0;
			this.button = 0;
			this.pressedButtons = 0;
			this.sourcePointerEvent = null;
			this.recomputeTopElementUnderMouse = false;
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
				bool flag = visualElement != null;
				if (flag)
				{
					this.localMousePosition = visualElement.WorldToLocal3D(this.mousePosition);
				}
				else
				{
					this.localMousePosition = this.mousePosition;
				}
			}
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			bool recomputeTopElementUnderMouse = this.recomputeTopElementUnderMouse;
			if (recomputeTopElementUnderMouse)
			{
				bool flag = this.sourcePointerEvent == null;
				if (flag)
				{
					PointerDeviceState.SavePointerPosition(PointerId.mousePointerId, this.mousePosition, panel, panel.contextType);
					((BaseVisualElementPanel)panel).RecomputeTopElementUnderPointer(PointerId.mousePointerId, this.mousePosition, this);
				}
				else
				{
					bool flag2 = this.sourcePointerEvent.pointerId != PointerId.mousePointerId;
					if (flag2)
					{
						Vector2 s_OutsidePanelCoordinates = BaseVisualElementPanel.s_OutsidePanelCoordinates;
						PointerDeviceState.SavePointerPosition(PointerId.mousePointerId, s_OutsidePanelCoordinates, null, panel.contextType);
						((BaseVisualElementPanel)panel).SetTopElementUnderPointer(PointerId.mousePointerId, null, s_OutsidePanelCoordinates);
					}
				}
			}
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			EventBase eventBase = this.sourcePointerEvent as EventBase;
			bool flag = eventBase != null;
			if (flag)
			{
				Debug.Assert(!eventBase.processed, "!pointerEvent.processed");
				bool isPropagationStopped = base.isPropagationStopped;
				if (isPropagationStopped)
				{
					eventBase.StopPropagation();
				}
				bool isImmediatePropagationStopped = base.isImmediatePropagationStopped;
				if (isImmediatePropagationStopped)
				{
					eventBase.StopImmediatePropagation();
				}
				eventBase.processedByFocusController |= base.processedByFocusController;
			}
			else
			{
				bool recomputeTopElementUnderMouse = this.recomputeTopElementUnderMouse;
				if (recomputeTopElementUnderMouse)
				{
					BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
					if (baseVisualElementPanel != null)
					{
						baseVisualElementPanel.CommitElementUnderPointers();
					}
				}
			}
			base.PostDispatch(panel);
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, PointerId.mousePointerId, this.mousePosition);
		}

		public static T GetPooled(Event systemEvent)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.imguiEvent = systemEvent;
			bool flag = systemEvent != null;
			if (flag)
			{
				pooled.modifiers = systemEvent.modifiers;
				pooled.mousePosition = systemEvent.mousePosition;
				pooled.localMousePosition = systemEvent.mousePosition;
				pooled.mouseDelta = systemEvent.delta;
				pooled.button = systemEvent.button;
				pooled.pressedButtons = PointerDeviceState.GetPressedButtons(PointerId.mousePointerId);
				pooled.clickCount = systemEvent.clickCount;
			}
			return pooled;
		}

		public static T GetPooled(Vector2 position, int button, int clickCount, Vector2 delta, EventModifiers modifiers = EventModifiers.None)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.modifiers = modifiers;
			pooled.mousePosition = position;
			pooled.localMousePosition = position;
			pooled.mouseDelta = delta;
			pooled.button = button;
			pooled.pressedButtons = PointerDeviceState.GetPressedButtons(PointerId.mousePointerId);
			pooled.clickCount = clickCount;
			return pooled;
		}

		internal static T GetPooled(IMouseEvent triggerEvent, Vector2 mousePosition)
		{
			bool flag = triggerEvent != null;
			T t;
			if (flag)
			{
				t = MouseEventBase<T>.GetPooled(triggerEvent);
			}
			else
			{
				T pooled = EventBase<T>.GetPooled();
				pooled.mousePosition = mousePosition;
				pooled.localMousePosition = mousePosition;
				t = pooled;
			}
			return t;
		}

		public static T GetPooled(IMouseEvent triggerEvent)
		{
			T pooled = EventBase<T>.GetPooled(triggerEvent as EventBase);
			bool flag = triggerEvent != null;
			if (flag)
			{
				pooled.modifiers = triggerEvent.modifiers;
				pooled.mousePosition = triggerEvent.mousePosition;
				pooled.localMousePosition = triggerEvent.mousePosition;
				pooled.mouseDelta = triggerEvent.mouseDelta;
				pooled.button = triggerEvent.button;
				pooled.pressedButtons = triggerEvent.pressedButtons;
				pooled.clickCount = triggerEvent.clickCount;
			}
			return pooled;
		}

		protected static T GetPooled(IPointerEvent pointerEvent)
		{
			T pooled = EventBase<T>.GetPooled();
			EventBase eventBase = pooled;
			EventBase eventBase2 = pointerEvent as EventBase;
			eventBase.elementTarget = ((eventBase2 != null) ? eventBase2.elementTarget : null);
			EventBase eventBase3 = pooled;
			EventBase eventBase4 = pointerEvent as EventBase;
			eventBase3.imguiEvent = ((eventBase4 != null) ? eventBase4.imguiEvent : null);
			pooled.modifiers = pointerEvent.modifiers;
			pooled.mousePosition = pointerEvent.position;
			pooled.localMousePosition = pointerEvent.position;
			pooled.mouseDelta = pointerEvent.deltaPosition;
			pooled.button = ((pointerEvent.button == -1) ? 0 : pointerEvent.button);
			pooled.pressedButtons = pointerEvent.pressedButtons;
			pooled.clickCount = pointerEvent.clickCount;
			IPointerEventInternal pointerEventInternal = pointerEvent as IPointerEventInternal;
			bool flag = pointerEventInternal != null;
			if (flag)
			{
				pooled.sourcePointerEvent = pointerEvent;
			}
			return pooled;
		}

		protected MouseEventBase()
		{
			this.LocalInit();
		}
	}
}
