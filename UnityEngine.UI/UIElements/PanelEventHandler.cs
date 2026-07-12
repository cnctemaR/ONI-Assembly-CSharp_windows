using System;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UIElements
{
	[AddComponentMenu("UI Toolkit/Panel Event Handler (UI Toolkit)")]
	public class PanelEventHandler : UIBehaviour, IPointerMoveHandler, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler, ISubmitHandler, ICancelHandler, IMoveHandler, IScrollHandler, ISelectHandler, IDeselectHandler, IPointerExitHandler, IPointerEnterHandler, IRuntimePanelComponent, IPointerClickHandler
	{
		public IPanel panel
		{
			get
			{
				return this.m_Panel;
			}
			set
			{
				BaseRuntimePanel baseRuntimePanel = (BaseRuntimePanel)value;
				if (this.m_Panel != baseRuntimePanel)
				{
					this.UnregisterCallbacks();
					this.m_Panel = baseRuntimePanel;
					this.RegisterCallbacks();
				}
			}
		}

		private GameObject selectableGameObject
		{
			get
			{
				BaseRuntimePanel panel = this.m_Panel;
				if (panel == null)
				{
					return null;
				}
				return panel.selectableGameObject;
			}
		}

		private EventSystem eventSystem
		{
			get
			{
				return UIElementsRuntimeUtility.activeEventSystem as EventSystem;
			}
		}

		private bool isCurrentFocusedPanel
		{
			get
			{
				return this.m_Panel != null && this.eventSystem != null && this.eventSystem.currentSelectedGameObject == this.selectableGameObject;
			}
		}

		private Focusable currentFocusedElement
		{
			get
			{
				BaseRuntimePanel panel = this.m_Panel;
				if (panel == null)
				{
					return null;
				}
				return panel.focusController.GetLeafFocusedElement();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.RegisterCallbacks();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.UnregisterCallbacks();
		}

		private void RegisterCallbacks()
		{
			if (this.m_Panel != null)
			{
				this.m_Panel.destroyed += this.OnPanelDestroyed;
				this.m_Panel.visualTree.RegisterCallback<FocusEvent>(new EventCallback<FocusEvent>(this.OnElementFocus), TrickleDown.TrickleDown);
				this.m_Panel.visualTree.RegisterCallback<BlurEvent>(new EventCallback<BlurEvent>(this.OnElementBlur), TrickleDown.TrickleDown);
			}
		}

		private void UnregisterCallbacks()
		{
			if (this.m_Panel != null)
			{
				this.m_Panel.destroyed -= this.OnPanelDestroyed;
				this.m_Panel.visualTree.UnregisterCallback<FocusEvent>(new EventCallback<FocusEvent>(this.OnElementFocus), TrickleDown.TrickleDown);
				this.m_Panel.visualTree.UnregisterCallback<BlurEvent>(new EventCallback<BlurEvent>(this.OnElementBlur), TrickleDown.TrickleDown);
			}
		}

		private void OnPanelDestroyed()
		{
			this.panel = null;
		}

		private void OnElementFocus(FocusEvent e)
		{
			if (!this.m_Selecting && this.eventSystem != null)
			{
				this.eventSystem.SetSelectedGameObject(this.selectableGameObject);
			}
		}

		private void OnElementBlur(BlurEvent e)
		{
		}

		public void OnSelect(BaseEventData eventData)
		{
			this.m_Selecting = true;
			try
			{
				BaseRuntimePanel panel = this.m_Panel;
				if (panel != null)
				{
					panel.Focus();
				}
			}
			finally
			{
				this.m_Selecting = false;
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			BaseRuntimePanel panel = this.m_Panel;
			if (panel == null)
			{
				return;
			}
			panel.Blur();
		}

		public void OnPointerMove(PointerEventData eventData)
		{
			if (this.m_Panel == null || !this.ReadPointerData(this.m_PointerEvent, eventData, PanelEventHandler.PointerEventType.Default))
			{
				return;
			}
			using (PointerMoveEvent pooled = PointerEventBase<PointerMoveEvent>.GetPooled(this.m_PointerEvent))
			{
				this.SendEvent(pooled, eventData);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this.m_Panel == null || !this.ReadPointerData(this.m_PointerEvent, eventData, PanelEventHandler.PointerEventType.Up))
			{
				return;
			}
			using (PointerUpEvent pooled = PointerEventBase<PointerUpEvent>.GetPooled(this.m_PointerEvent))
			{
				this.SendEvent(pooled, eventData);
				if (pooled.pressedButtons == 0)
				{
					PointerDeviceState.SetPlayerPanelWithSoftPointerCapture(pooled.pointerId, null);
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (this.m_Panel == null || !this.ReadPointerData(this.m_PointerEvent, eventData, PanelEventHandler.PointerEventType.Down))
			{
				return;
			}
			if (this.eventSystem != null)
			{
				this.eventSystem.SetSelectedGameObject(this.selectableGameObject);
			}
			using (PointerDownEvent pooled = PointerEventBase<PointerDownEvent>.GetPooled(this.m_PointerEvent))
			{
				this.SendEvent(pooled, eventData);
				PointerDeviceState.SetPlayerPanelWithSoftPointerCapture(pooled.pointerId, this.m_Panel);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.m_Panel == null || !this.ReadPointerData(this.m_PointerEvent, eventData, PanelEventHandler.PointerEventType.Default))
			{
				return;
			}
			if (eventData.pointerCurrentRaycast.gameObject == base.gameObject && eventData.pointerPressRaycast.gameObject != base.gameObject && this.m_PointerEvent.pointerId != PointerId.mousePointerId)
			{
				using (PointerCancelEvent pooled = PointerEventBase<PointerCancelEvent>.GetPooled(this.m_PointerEvent))
				{
					this.SendEvent(pooled, eventData);
					if (pooled.pressedButtons == 0)
					{
						PointerDeviceState.SetPlayerPanelWithSoftPointerCapture(pooled.pointerId, null);
					}
				}
			}
			this.m_Panel.PointerLeavesPanel(this.m_PointerEvent.pointerId, this.m_PointerEvent.position);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.m_Panel == null || !this.ReadPointerData(this.m_PointerEvent, eventData, PanelEventHandler.PointerEventType.Default))
			{
				return;
			}
			this.m_Panel.PointerEntersPanel(this.m_PointerEvent.pointerId, this.m_PointerEvent.position);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			this.m_LastClickTime = Time.unscaledTime;
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (this.m_Panel == null)
			{
				return;
			}
			Focusable focusable = this.currentFocusedElement ?? this.m_Panel.visualTree;
			this.ProcessImguiEvents(focusable);
			using (NavigationSubmitEvent pooled = NavigationEventBase<NavigationSubmitEvent>.GetPooled(PanelEventHandler.s_Modifiers))
			{
				pooled.target = focusable;
				this.SendEvent(pooled, eventData);
			}
		}

		public void OnCancel(BaseEventData eventData)
		{
			if (this.m_Panel == null)
			{
				return;
			}
			Focusable focusable = this.currentFocusedElement ?? this.m_Panel.visualTree;
			this.ProcessImguiEvents(focusable);
			using (NavigationCancelEvent pooled = NavigationEventBase<NavigationCancelEvent>.GetPooled(PanelEventHandler.s_Modifiers))
			{
				pooled.target = focusable;
				this.SendEvent(pooled, eventData);
			}
		}

		public void OnMove(AxisEventData eventData)
		{
			if (this.m_Panel == null)
			{
				return;
			}
			Focusable focusable = this.currentFocusedElement ?? this.m_Panel.visualTree;
			this.ProcessImguiEvents(focusable);
			using (NavigationMoveEvent pooled = NavigationMoveEvent.GetPooled(eventData.moveVector, PanelEventHandler.s_Modifiers))
			{
				pooled.target = focusable;
				this.SendEvent(pooled, eventData);
			}
		}

		public void OnScroll(PointerEventData eventData)
		{
			if (this.m_Panel == null || !this.ReadPointerData(this.m_PointerEvent, eventData, PanelEventHandler.PointerEventType.Default))
			{
				return;
			}
			Vector2 vector = eventData.scrollDelta;
			vector.y = -vector.y;
			vector /= 20f;
			using (WheelEvent pooled = WheelEvent.GetPooled(vector, this.m_PointerEvent))
			{
				this.SendEvent(pooled, eventData);
			}
		}

		private void SendEvent(EventBase e, BaseEventData sourceEventData)
		{
			this.m_Panel.SendEvent(e, DispatchMode.Default);
			if (e.isPropagationStopped)
			{
				sourceEventData.Use();
			}
		}

		private void SendEvent(EventBase e, Event sourceEvent)
		{
			this.m_Panel.SendEvent(e, DispatchMode.Default);
		}

		internal void Update()
		{
			if (this.isCurrentFocusedPanel)
			{
				this.ProcessImguiEvents(this.currentFocusedElement ?? this.m_Panel.visualTree);
			}
		}

		private void LateUpdate()
		{
			this.ProcessImguiEvents(null);
		}

		private void ProcessImguiEvents(Focusable target)
		{
			bool flag = true;
			while (Event.PopEvent(this.m_Event))
			{
				if (this.m_Event.type != EventType.Ignore && this.m_Event.type != EventType.Repaint && this.m_Event.type != EventType.Layout)
				{
					PanelEventHandler.s_Modifiers = (flag ? this.m_Event.modifiers : (PanelEventHandler.s_Modifiers | this.m_Event.modifiers));
					flag = false;
					if (target != null)
					{
						this.ProcessKeyboardEvent(this.m_Event, target);
						if (this.eventSystem.sendNavigationEvents)
						{
							this.ProcessTabEvent(this.m_Event, target);
						}
					}
				}
			}
		}

		private void ProcessKeyboardEvent(Event e, Focusable target)
		{
			if (e.type == EventType.KeyUp)
			{
				this.SendKeyUpEvent(e, target);
				return;
			}
			if (e.type == EventType.KeyDown)
			{
				this.SendKeyDownEvent(e, target);
			}
		}

		private void ProcessTabEvent(Event e, Focusable target)
		{
			if (e.ShouldSendNavigationMoveEventRuntime())
			{
				this.SendTabEvent(e, e.shift ? NavigationMoveEvent.Direction.Previous : NavigationMoveEvent.Direction.Next, target);
			}
		}

		private void SendTabEvent(Event e, NavigationMoveEvent.Direction direction, Focusable target)
		{
			using (NavigationMoveEvent pooled = NavigationMoveEvent.GetPooled(direction, PanelEventHandler.s_Modifiers))
			{
				pooled.target = target;
				this.SendEvent(pooled, e);
			}
		}

		private void SendKeyUpEvent(Event e, Focusable target)
		{
			using (KeyUpEvent keyUpEvent = (KeyUpEvent)UIElementsRuntimeUtility.CreateEvent(e))
			{
				keyUpEvent.target = target;
				this.SendEvent(keyUpEvent, e);
			}
		}

		private void SendKeyDownEvent(Event e, Focusable target)
		{
			using (KeyDownEvent keyDownEvent = (KeyDownEvent)UIElementsRuntimeUtility.CreateEvent(e))
			{
				keyDownEvent.target = target;
				this.SendEvent(keyDownEvent, e);
			}
		}

		private bool ReadPointerData(PanelEventHandler.PointerEvent pe, PointerEventData eventData, PanelEventHandler.PointerEventType eventType = PanelEventHandler.PointerEventType.Default)
		{
			if (this.eventSystem == null || this.eventSystem.currentInputModule == null)
			{
				return false;
			}
			pe.Read(this, eventData, eventType);
			Vector2 vector;
			Vector2 vector2;
			this.m_Panel.ScreenToPanel(pe.position, pe.deltaPosition, out vector, out vector2, true);
			pe.SetPosition(vector, vector2);
			return true;
		}

		private BaseRuntimePanel m_Panel;

		private readonly PanelEventHandler.PointerEvent m_PointerEvent = new PanelEventHandler.PointerEvent();

		private float m_LastClickTime;

		private bool m_Selecting;

		private Event m_Event = new Event();

		private static EventModifiers s_Modifiers;

		private enum PointerEventType
		{
			Default,
			Down,
			Up
		}

		private class PointerEvent : IPointerEvent
		{
			public int pointerId { get; private set; }

			public string pointerType { get; private set; }

			public bool isPrimary { get; private set; }

			public int button { get; private set; }

			public int pressedButtons { get; private set; }

			public Vector3 position { get; private set; }

			public Vector3 localPosition { get; private set; }

			public Vector3 deltaPosition { get; private set; }

			public float deltaTime { get; private set; }

			public int clickCount { get; private set; }

			public float pressure { get; private set; }

			public float tangentialPressure { get; private set; }

			public float altitudeAngle { get; private set; }

			public float azimuthAngle { get; private set; }

			public float twist { get; private set; }

			public Vector2 tilt { get; private set; }

			public PenStatus penStatus { get; private set; }

			public Vector2 radius { get; private set; }

			public Vector2 radiusVariance { get; private set; }

			public EventModifiers modifiers { get; private set; }

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
					if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer)
					{
						return this.ctrlKey;
					}
					return this.commandKey;
				}
			}

			public void Read(PanelEventHandler self, PointerEventData eventData, PanelEventHandler.PointerEventType eventType)
			{
				this.pointerId = self.eventSystem.currentInputModule.ConvertUIToolkitPointerId(eventData);
				this.pointerType = (PanelEventHandler.PointerEvent.<Read>g__InRange|90_0(this.pointerId, PointerId.touchPointerIdBase, PointerId.touchPointerCount) ? PointerType.touch : (PanelEventHandler.PointerEvent.<Read>g__InRange|90_0(this.pointerId, PointerId.penPointerIdBase, PointerId.penPointerCount) ? PointerType.pen : PointerType.mouse));
				this.isPrimary = this.pointerId == PointerId.mousePointerId || this.pointerId == PointerId.touchPointerIdBase || this.pointerId == PointerId.penPointerIdBase;
				int num = Screen.height;
				Vector3 relativeMousePositionForRaycast = MultipleDisplayUtilities.GetRelativeMousePositionForRaycast(eventData);
				int num2 = (int)relativeMousePositionForRaycast.z;
				if (num2 > 0 && num2 < Display.displays.Length)
				{
					num = Display.displays[num2].systemHeight;
				}
				Vector2 delta = eventData.delta;
				relativeMousePositionForRaycast.y = (float)num - relativeMousePositionForRaycast.y;
				delta.y = -delta.y;
				this.localPosition = (this.position = relativeMousePositionForRaycast);
				this.deltaPosition = delta;
				this.deltaTime = 0f;
				this.pressure = eventData.pressure;
				this.tangentialPressure = eventData.tangentialPressure;
				this.altitudeAngle = eventData.altitudeAngle;
				this.azimuthAngle = eventData.azimuthAngle;
				this.twist = eventData.twist;
				this.tilt = eventData.tilt;
				this.penStatus = eventData.penStatus;
				this.radius = eventData.radius;
				this.radiusVariance = eventData.radiusVariance;
				this.modifiers = PanelEventHandler.s_Modifiers;
				if (eventType == PanelEventHandler.PointerEventType.Default)
				{
					this.button = -1;
					this.clickCount = 0;
				}
				else
				{
					this.button = Mathf.Max(0, (int)eventData.button);
					this.clickCount = eventData.clickCount;
					if (eventType == PanelEventHandler.PointerEventType.Down)
					{
						if (Time.unscaledTime > self.m_LastClickTime + (float)ClickDetector.s_DoubleClickTime * 0.001f)
						{
							this.clickCount = 0;
						}
						int clickCount = this.clickCount;
						this.clickCount = clickCount + 1;
						PointerDeviceState.PressButton(this.pointerId, this.button);
					}
					else if (eventType == PanelEventHandler.PointerEventType.Up)
					{
						PointerDeviceState.ReleaseButton(this.pointerId, this.button);
					}
					this.clickCount = Mathf.Max(1, this.clickCount);
				}
				this.pressedButtons = PointerDeviceState.GetPressedButtons(this.pointerId);
			}

			public void SetPosition(Vector3 positionOverride, Vector3 deltaOverride)
			{
				this.position = positionOverride;
				this.localPosition = positionOverride;
				this.deltaPosition = deltaOverride;
			}

			[CompilerGenerated]
			internal static bool <Read>g__InRange|90_0(int i, int start, int count)
			{
				return i >= start && i < start + count;
			}
		}
	}
}
