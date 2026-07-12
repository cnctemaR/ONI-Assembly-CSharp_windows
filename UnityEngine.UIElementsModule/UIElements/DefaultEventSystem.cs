using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal class DefaultEventSystem
	{
		private bool isAppFocused
		{
			get
			{
				return Application.isFocused;
			}
		}

		internal DefaultEventSystem.IInput input
		{
			get
			{
				DefaultEventSystem.IInput input;
				if ((input = this.m_Input) == null)
				{
					input = (this.m_Input = this.GetDefaultInput());
				}
				return input;
			}
			set
			{
				this.m_Input = value;
			}
		}

		private DefaultEventSystem.IInput GetDefaultInput()
		{
			DefaultEventSystem.IInput input = new DefaultEventSystem.Input();
			try
			{
				input.GetAxisRaw(this.m_HorizontalAxis);
			}
			catch (InvalidOperationException)
			{
				input = new DefaultEventSystem.NoInput();
				Debug.LogWarning("UI Toolkit is currently relying on the legacy Input Manager for its active input source, but the legacy Input Manager is not available using your current Project Settings. Some UI Toolkit functionality might be missing or not working properly as a result. To fix this problem, you can enable \"Input Manager (old)\" or \"Both\" in the Active Input Source setting of the Player section. UI Toolkit is using its internal default event system to process input. Alternatively, you may activate new Input System support with UI Toolkit by adding an EventSystem component to your active scene.");
			}
			return input;
		}

		private bool ShouldIgnoreEventsOnAppNotFocused()
		{
			OperatingSystemFamily operatingSystemFamily = SystemInfo.operatingSystemFamily;
			OperatingSystemFamily operatingSystemFamily2 = operatingSystemFamily;
			return operatingSystemFamily2 - OperatingSystemFamily.MacOSX <= 2;
		}

		public BaseRuntimePanel focusedPanel
		{
			get
			{
				return this.m_FocusedPanel;
			}
			set
			{
				bool flag = this.m_FocusedPanel != value;
				if (flag)
				{
					BaseRuntimePanel focusedPanel = this.m_FocusedPanel;
					if (focusedPanel != null)
					{
						focusedPanel.Blur();
					}
					this.m_FocusedPanel = value;
					BaseRuntimePanel focusedPanel2 = this.m_FocusedPanel;
					if (focusedPanel2 != null)
					{
						focusedPanel2.Focus();
					}
				}
			}
		}

		public void Reset()
		{
			this.m_LastMousePressButton = -1;
			this.m_NextMousePressTime = 0f;
			this.m_LastMouseClickCount = 0;
			this.m_LastMousePosition = Vector2.zero;
			this.m_MouseProcessedAtLeastOnce = false;
			this.m_ConsecutiveMoveCount = 0;
			this.m_IsMoveFromKeyboard = false;
			this.m_FocusedPanel = null;
		}

		public void Update(DefaultEventSystem.UpdateMode updateMode = DefaultEventSystem.UpdateMode.Always)
		{
			bool flag = !this.isAppFocused && this.ShouldIgnoreEventsOnAppNotFocused() && updateMode == DefaultEventSystem.UpdateMode.IgnoreIfAppNotFocused;
			if (!flag)
			{
				this.m_SendingPenEvent = this.ProcessPenEvents();
				bool flag2 = !this.m_SendingPenEvent;
				if (flag2)
				{
					this.m_SendingTouchEvents = this.ProcessTouchEvents();
				}
				bool flag3 = !this.m_SendingPenEvent && !this.m_SendingTouchEvents;
				if (flag3)
				{
					this.ProcessMouseEvents();
				}
				else
				{
					this.m_MouseProcessedAtLeastOnce = false;
				}
				using (this.FocusBasedEventSequence())
				{
					this.SendIMGUIEvents();
					this.SendInputEvents();
				}
			}
		}

		internal DefaultEventSystem.FocusBasedEventSequenceContext FocusBasedEventSequence()
		{
			return new DefaultEventSystem.FocusBasedEventSequenceContext(this);
		}

		private void SendIMGUIEvents()
		{
			bool flag = true;
			while (Event.PopEvent(this.m_Event))
			{
				bool flag2 = this.m_Event.type == EventType.Ignore || this.m_Event.type == EventType.Repaint || this.m_Event.type == EventType.Layout;
				if (!flag2)
				{
					this.m_CurrentModifiers = (flag ? this.m_Event.modifiers : (this.m_CurrentModifiers | this.m_Event.modifiers));
					flag = false;
					bool flag3 = this.m_Event.type == EventType.KeyUp || this.m_Event.type == EventType.KeyDown;
					if (flag3)
					{
						this.SendFocusBasedEvent<DefaultEventSystem>((DefaultEventSystem self) => UIElementsRuntimeUtility.CreateEvent(self.m_Event), this);
						this.ProcessTabEvent(this.m_Event, this.m_CurrentModifiers);
					}
					else
					{
						bool flag4 = this.m_Event.type == EventType.ScrollWheel;
						if (flag4)
						{
							int? num;
							Vector2 vector = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(this.input.mousePosition, out num);
							Vector2 vector2 = vector - this.m_LastMousePosition;
							Vector2 delta = this.m_Event.delta;
							this.SendPositionBasedEvent<ValueTuple<EventModifiers, Vector2>>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 _, [TupleElementNames(new string[] { "modifiers", "scrollDelta" })] ValueTuple<EventModifiers, Vector2> t) => WheelEvent.GetPooled(t.Item2, panelPosition, t.Item1), new ValueTuple<EventModifiers, Vector2>(this.m_CurrentModifiers, delta), false);
						}
						else
						{
							bool flag5 = (!this.m_SendingTouchEvents && !this.m_SendingPenEvent && this.m_Event.pointerType != PointerType.Mouse) || this.m_Event.type == EventType.MouseEnterWindow || this.m_Event.type == EventType.MouseLeaveWindow;
							if (flag5)
							{
								int num2 = ((this.m_Event.pointerType == PointerType.Mouse) ? PointerId.mousePointerId : ((this.m_Event.pointerType == PointerType.Touch) ? PointerId.touchPointerIdBase : PointerId.penPointerIdBase));
								int? num3;
								Vector3 vector3 = UIElementsRuntimeUtility.MultiDisplayToLocalScreenPosition(this.m_Event.mousePosition, out num3);
								Vector2 delta2 = this.m_Event.delta;
								this.SendPositionBasedEvent<Event>(vector3, delta2, num2, num3, delegate(Vector3 panelPosition, Vector3 panelDelta, Event evt)
								{
									evt.mousePosition = panelPosition;
									evt.delta = panelDelta;
									return UIElementsRuntimeUtility.CreateEvent(evt);
								}, this.m_Event, this.m_Event.type == EventType.MouseDown || this.m_Event.type == EventType.TouchDown);
							}
						}
					}
				}
			}
		}

		private void ProcessMouseEvents()
		{
			bool flag = !this.input.mousePresent;
			if (!flag)
			{
				int? num;
				Vector2 vector = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(this.input.mousePosition, out num);
				Vector2 vector2 = vector - this.m_LastMousePosition;
				bool flag2 = !this.m_MouseProcessedAtLeastOnce;
				if (flag2)
				{
					vector2 = Vector2.zero;
					this.m_LastMousePosition = vector;
					this.m_MouseProcessedAtLeastOnce = true;
				}
				else
				{
					bool flag3 = !Mathf.Approximately(vector2.x, 0f) || !Mathf.Approximately(vector2.y, 0f);
					if (flag3)
					{
						this.m_LastMousePosition = vector;
						this.SendPositionBasedEvent<DefaultEventSystem>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 panelDelta, DefaultEventSystem self) => PointerEventBase<PointerMoveEvent>.GetPooled(EventType.MouseMove, panelPosition, panelDelta, -1, 0, self.m_CurrentModifiers), this, false);
					}
				}
				int mouseButtonCount = this.input.mouseButtonCount;
				for (int i = 0; i < mouseButtonCount; i++)
				{
					bool mouseButtonDown = this.input.GetMouseButtonDown(i);
					if (mouseButtonDown)
					{
						bool flag4 = this.m_LastMousePressButton != i || this.input.unscaledTime >= this.m_NextMousePressTime;
						if (flag4)
						{
							this.m_LastMousePressButton = i;
							this.m_LastMouseClickCount = 0;
						}
						int num2 = this.m_LastMouseClickCount + 1;
						this.m_LastMouseClickCount = num2;
						int num3 = num2;
						this.m_NextMousePressTime = this.input.unscaledTime + this.input.doubleClickTime;
						this.SendPositionBasedEvent<ValueTuple<int, int, EventModifiers>>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 panelDelta, [TupleElementNames(new string[] { "button", "clickCount", "modifiers" })] ValueTuple<int, int, EventModifiers> t) => PointerEventHelper.GetPooled(EventType.MouseDown, panelPosition, panelDelta, t.Item1, t.Item2, t.Item3), new ValueTuple<int, int, EventModifiers>(i, num3, this.m_CurrentModifiers), true);
					}
					bool mouseButtonUp = this.input.GetMouseButtonUp(i);
					if (mouseButtonUp)
					{
						int lastMouseClickCount = this.m_LastMouseClickCount;
						this.SendPositionBasedEvent<ValueTuple<int, int, EventModifiers>>(vector, vector2, PointerId.mousePointerId, num, (Vector3 panelPosition, Vector3 panelDelta, [TupleElementNames(new string[] { "button", "clickCount", "modifiers" })] ValueTuple<int, int, EventModifiers> t) => PointerEventHelper.GetPooled(EventType.MouseUp, panelPosition, panelDelta, t.Item1, t.Item2, t.Item3), new ValueTuple<int, int, EventModifiers>(i, lastMouseClickCount, this.m_CurrentModifiers), false);
					}
				}
			}
		}

		private void SendInputEvents()
		{
			bool flag = this.ShouldSendMoveFromInput();
			bool flag2 = flag;
			if (flag2)
			{
				this.SendFocusBasedEvent<DefaultEventSystem>((DefaultEventSystem self) => NavigationMoveEvent.GetPooled(self.GetRawMoveVector(), self.m_IsMoveFromKeyboard ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
			}
			bool buttonDown = this.input.GetButtonDown(this.m_SubmitButton);
			if (buttonDown)
			{
				this.SendFocusBasedEvent<DefaultEventSystem>((DefaultEventSystem self) => NavigationEventBase<NavigationSubmitEvent>.GetPooled(self.input.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
			}
			bool buttonDown2 = this.input.GetButtonDown(this.m_CancelButton);
			if (buttonDown2)
			{
				this.SendFocusBasedEvent<DefaultEventSystem>((DefaultEventSystem self) => NavigationEventBase<NavigationCancelEvent>.GetPooled(self.input.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
			}
		}

		internal void OnFocusEvent(RuntimePanel panel, FocusEvent evt)
		{
			this.focusedPanel = panel;
		}

		internal void SendFocusBasedEvent<TArg>(Func<TArg, EventBase> evtFactory, TArg arg)
		{
			bool flag = this.m_PreviousFocusedPanel != null;
			if (flag)
			{
				using (EventBase eventBase = evtFactory(arg))
				{
					eventBase.target = this.m_PreviousFocusedElement ?? this.m_PreviousFocusedPanel.visualTree;
					this.m_PreviousFocusedPanel.visualTree.SendEvent(eventBase);
					this.UpdateFocusedPanel(this.m_PreviousFocusedPanel);
					return;
				}
			}
			List<Panel> sortedPlayerPanels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
			for (int i = sortedPlayerPanels.Count - 1; i >= 0; i--)
			{
				Panel panel = sortedPlayerPanels[i];
				BaseRuntimePanel baseRuntimePanel = panel as BaseRuntimePanel;
				bool flag2 = baseRuntimePanel != null;
				if (flag2)
				{
					using (EventBase eventBase2 = evtFactory(arg))
					{
						eventBase2.target = baseRuntimePanel.visualTree;
						baseRuntimePanel.visualTree.SendEvent(eventBase2);
						bool flag3 = baseRuntimePanel.focusController.focusedElement != null;
						if (flag3)
						{
							this.focusedPanel = baseRuntimePanel;
							break;
						}
						bool isPropagationStopped = eventBase2.isPropagationStopped;
						if (isPropagationStopped)
						{
							break;
						}
					}
				}
			}
		}

		internal void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId, Func<Vector3, Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
		{
			this.SendPositionBasedEvent<TArg>(mousePosition, delta, pointerId, null, evtFactory, arg, deselectIfNoTarget);
		}

		private void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId, int? targetDisplay, Func<Vector3, Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
		{
			bool flag = this.focusedPanel != null;
			if (flag)
			{
				this.UpdateFocusedPanel(this.focusedPanel);
			}
			IPanel panel = PointerDeviceState.GetPlayerPanelWithSoftPointerCapture(pointerId);
			IEventHandler capturingElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId);
			VisualElement visualElement = capturingElement as VisualElement;
			bool flag2 = visualElement != null;
			if (flag2)
			{
				panel = visualElement.panel;
			}
			BaseRuntimePanel baseRuntimePanel = null;
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			BaseRuntimePanel baseRuntimePanel2 = panel as BaseRuntimePanel;
			bool flag3 = baseRuntimePanel2 != null;
			if (flag3)
			{
				baseRuntimePanel = baseRuntimePanel2;
				baseRuntimePanel.ScreenToPanel(mousePosition, delta, out zero, out zero2, false);
			}
			else
			{
				List<Panel> sortedPlayerPanels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
				for (int i = sortedPlayerPanels.Count - 1; i >= 0; i--)
				{
					BaseRuntimePanel baseRuntimePanel3 = sortedPlayerPanels[i] as BaseRuntimePanel;
					bool flag4;
					if (baseRuntimePanel3 != null)
					{
						if (targetDisplay != null)
						{
							int targetDisplay2 = baseRuntimePanel3.targetDisplay;
							int? num = targetDisplay;
							flag4 = (targetDisplay2 == num.GetValueOrDefault()) & (num != null);
						}
						else
						{
							flag4 = true;
						}
					}
					else
					{
						flag4 = false;
					}
					bool flag5 = flag4;
					if (flag5)
					{
						bool flag6 = baseRuntimePanel3.ScreenToPanel(mousePosition, delta, out zero, out zero2, false) && baseRuntimePanel3.Pick(zero) != null;
						if (flag6)
						{
							baseRuntimePanel = baseRuntimePanel3;
							break;
						}
					}
				}
			}
			BaseRuntimePanel baseRuntimePanel4 = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as BaseRuntimePanel;
			bool flag7 = baseRuntimePanel4 != baseRuntimePanel;
			if (flag7)
			{
				if (baseRuntimePanel4 != null)
				{
					baseRuntimePanel4.PointerLeavesPanel(pointerId, baseRuntimePanel4.ScreenToPanel(mousePosition));
				}
				if (baseRuntimePanel != null)
				{
					baseRuntimePanel.PointerEntersPanel(pointerId, zero);
				}
			}
			bool flag8 = baseRuntimePanel != null;
			if (flag8)
			{
				using (EventBase eventBase = evtFactory(zero, zero2, arg))
				{
					baseRuntimePanel.visualTree.SendEvent(eventBase);
					bool processedByFocusController = eventBase.processedByFocusController;
					if (processedByFocusController)
					{
						this.UpdateFocusedPanel(baseRuntimePanel);
					}
					bool flag9 = eventBase.eventTypeId == EventBase<PointerDownEvent>.TypeId();
					if (flag9)
					{
						PointerDeviceState.SetPlayerPanelWithSoftPointerCapture(pointerId, baseRuntimePanel);
					}
					else
					{
						bool flag10 = eventBase.eventTypeId == EventBase<PointerUpEvent>.TypeId() && ((PointerUpEvent)eventBase).pressedButtons == 0;
						if (flag10)
						{
							PointerDeviceState.SetPlayerPanelWithSoftPointerCapture(pointerId, null);
						}
					}
				}
			}
			else if (deselectIfNoTarget)
			{
				this.focusedPanel = null;
			}
		}

		private void UpdateFocusedPanel(BaseRuntimePanel runtimePanel)
		{
			bool flag = runtimePanel.focusController.focusedElement != null;
			if (flag)
			{
				this.focusedPanel = runtimePanel;
			}
			else
			{
				bool flag2 = this.focusedPanel == runtimePanel;
				if (flag2)
				{
					this.focusedPanel = null;
				}
			}
		}

		private static EventBase MakeTouchEvent(Touch touch, EventModifiers modifiers)
		{
			EventBase eventBase;
			switch (touch.phase)
			{
			case TouchPhase.Began:
				eventBase = PointerEventBase<PointerDownEvent>.GetPooled(touch, modifiers);
				break;
			case TouchPhase.Moved:
				eventBase = PointerEventBase<PointerMoveEvent>.GetPooled(touch, modifiers);
				break;
			case TouchPhase.Stationary:
				eventBase = PointerEventBase<PointerStationaryEvent>.GetPooled(touch, modifiers);
				break;
			case TouchPhase.Ended:
				eventBase = PointerEventBase<PointerUpEvent>.GetPooled(touch, modifiers);
				break;
			case TouchPhase.Canceled:
				eventBase = PointerEventBase<PointerCancelEvent>.GetPooled(touch, modifiers);
				break;
			default:
				eventBase = null;
				break;
			}
			return eventBase;
		}

		private static EventBase MakePenEvent(PenData pen, EventModifiers modifiers)
		{
			switch (pen.contactType)
			{
			case PenEventType.PenDown:
				return PointerEventBase<PointerDownEvent>.GetPooled(pen, modifiers);
			case PenEventType.PenUp:
				return PointerEventBase<PointerUpEvent>.GetPooled(pen, modifiers);
			}
			return null;
		}

		private bool ProcessTouchEvents()
		{
			for (int i = 0; i < this.input.touchCount; i++)
			{
				Touch touch = this.input.GetTouch(i);
				bool flag = touch.type == TouchType.Indirect;
				if (!flag)
				{
					int? num;
					touch.position = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.position, out num);
					int? num2;
					touch.rawPosition = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.rawPosition, out num2);
					touch.deltaPosition = UIElementsRuntimeUtility.ScreenBottomLeftToPanelDelta(touch.deltaPosition);
					this.SendPositionBasedEvent<Touch>(touch.position, touch.deltaPosition, PointerId.touchPointerIdBase + touch.fingerId, num, delegate(Vector3 panelPosition, Vector3 panelDelta, Touch _touch)
					{
						_touch.position = panelPosition;
						_touch.deltaPosition = panelDelta;
						return DefaultEventSystem.MakeTouchEvent(_touch, EventModifiers.None);
					}, touch, false);
				}
			}
			return this.input.touchCount > 0;
		}

		private bool ProcessPenEvents()
		{
			PenData lastPenContactEvent = this.input.GetLastPenContactEvent();
			bool flag = lastPenContactEvent.contactType == PenEventType.NoContact;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.SendPositionBasedEvent<PenData>(lastPenContactEvent.position, lastPenContactEvent.deltaPos, PointerId.penPointerIdBase, null, delegate(Vector3 panelPosition, Vector3 panelDelta, PenData _pen)
				{
					_pen.position = panelPosition;
					_pen.deltaPos = panelDelta;
					return DefaultEventSystem.MakePenEvent(_pen, EventModifiers.None);
				}, lastPenContactEvent, false);
				this.input.ClearLastPenContactEvent();
				flag2 = true;
			}
			return flag2;
		}

		private Vector2 GetRawMoveVector()
		{
			Vector2 zero = Vector2.zero;
			zero.x = this.input.GetAxisRaw(this.m_HorizontalAxis);
			zero.y = this.input.GetAxisRaw(this.m_VerticalAxis);
			bool buttonDown = this.input.GetButtonDown(this.m_HorizontalAxis);
			if (buttonDown)
			{
				bool flag = zero.x < 0f;
				if (flag)
				{
					zero.x = -1f;
				}
				bool flag2 = zero.x > 0f;
				if (flag2)
				{
					zero.x = 1f;
				}
			}
			bool buttonDown2 = this.input.GetButtonDown(this.m_VerticalAxis);
			if (buttonDown2)
			{
				bool flag3 = zero.y < 0f;
				if (flag3)
				{
					zero.y = -1f;
				}
				bool flag4 = zero.y > 0f;
				if (flag4)
				{
					zero.y = 1f;
				}
			}
			return zero;
		}

		private bool ShouldSendMoveFromInput()
		{
			float unscaledTime = this.input.unscaledTime;
			Vector2 rawMoveVector = this.GetRawMoveVector();
			bool flag = Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f);
			bool flag2;
			if (flag)
			{
				this.m_ConsecutiveMoveCount = 0;
				this.m_IsMoveFromKeyboard = false;
				flag2 = false;
			}
			else
			{
				bool flag3 = this.input.GetButtonDown(this.m_HorizontalAxis) || this.input.GetButtonDown(this.m_VerticalAxis);
				bool flag4 = Vector2.Dot(rawMoveVector, this.m_LastMoveVector) > 0f;
				bool flag5 = !flag3;
				if (flag5)
				{
					bool flag6 = flag4 && this.m_ConsecutiveMoveCount == 1;
					if (flag6)
					{
						flag3 = unscaledTime > this.m_PrevActionTime + this.m_RepeatDelay;
					}
					else
					{
						flag3 = unscaledTime > this.m_PrevActionTime + 1f / this.m_InputActionsPerSecond;
					}
				}
				bool flag7 = !flag3;
				if (flag7)
				{
					flag2 = false;
				}
				else
				{
					NavigationMoveEvent.Direction direction = NavigationMoveEvent.DetermineMoveDirection(rawMoveVector.x, rawMoveVector.y, 0.6f);
					bool flag8 = direction > NavigationMoveEvent.Direction.None;
					if (flag8)
					{
						bool flag9 = !flag4;
						if (flag9)
						{
							this.m_ConsecutiveMoveCount = 0;
						}
						this.m_ConsecutiveMoveCount++;
						this.m_PrevActionTime = unscaledTime;
						this.m_LastMoveVector = rawMoveVector;
						this.m_IsMoveFromKeyboard |= this.input.anyKey;
					}
					else
					{
						this.m_ConsecutiveMoveCount = 0;
						this.m_IsMoveFromKeyboard = false;
					}
					flag2 = direction > NavigationMoveEvent.Direction.None;
				}
			}
			return flag2;
		}

		private void ProcessTabEvent(Event e, EventModifiers modifiers)
		{
			bool flag = e.ShouldSendNavigationMoveEventRuntime();
			if (flag)
			{
				NavigationMoveEvent.Direction direction = (e.shift ? NavigationMoveEvent.Direction.Previous : NavigationMoveEvent.Direction.Next);
				this.SendFocusBasedEvent<ValueTuple<NavigationMoveEvent.Direction, EventModifiers, DefaultEventSystem.IInput>>(([TupleElementNames(new string[] { "direction", "modifiers", "input" })] ValueTuple<NavigationMoveEvent.Direction, EventModifiers, DefaultEventSystem.IInput> t) => NavigationMoveEvent.GetPooled(t.Item1, t.Item3.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, t.Item2), new ValueTuple<NavigationMoveEvent.Direction, EventModifiers, DefaultEventSystem.IInput>(direction, modifiers, this.input));
			}
		}

		internal static Func<bool> IsEditorRemoteConnected = () => false;

		private DefaultEventSystem.IInput m_Input;

		private readonly string m_HorizontalAxis = "Horizontal";

		private readonly string m_VerticalAxis = "Vertical";

		private readonly string m_SubmitButton = "Submit";

		private readonly string m_CancelButton = "Cancel";

		private readonly float m_InputActionsPerSecond = 10f;

		private readonly float m_RepeatDelay = 0.5f;

		private bool m_SendingTouchEvents;

		private bool m_SendingPenEvent;

		private Event m_Event = new Event();

		private BaseRuntimePanel m_FocusedPanel;

		private BaseRuntimePanel m_PreviousFocusedPanel;

		private Focusable m_PreviousFocusedElement;

		private EventModifiers m_CurrentModifiers;

		private int m_LastMousePressButton = -1;

		private float m_NextMousePressTime = 0f;

		private int m_LastMouseClickCount = 0;

		private Vector2 m_LastMousePosition = Vector2.zero;

		private bool m_MouseProcessedAtLeastOnce;

		private int m_ConsecutiveMoveCount;

		private Vector2 m_LastMoveVector;

		private float m_PrevActionTime;

		private bool m_IsMoveFromKeyboard;

		public enum UpdateMode
		{
			Always,
			IgnoreIfAppNotFocused
		}

		internal struct FocusBasedEventSequenceContext : IDisposable
		{
			public FocusBasedEventSequenceContext(DefaultEventSystem es)
			{
				this.es = es;
				es.m_PreviousFocusedPanel = es.focusedPanel;
				BaseRuntimePanel focusedPanel = es.focusedPanel;
				es.m_PreviousFocusedElement = ((focusedPanel != null) ? focusedPanel.focusController.GetLeafFocusedElement() : null);
			}

			public void Dispose()
			{
				this.es.m_PreviousFocusedPanel = null;
				this.es.m_PreviousFocusedElement = null;
			}

			private DefaultEventSystem es;
		}

		internal interface IInput
		{
			bool GetButtonDown(string button);

			float GetAxisRaw(string axis);

			void ResetPenEvents();

			void ClearLastPenContactEvent();

			int penEventCount { get; }

			PenData GetPenEvent(int index);

			PenData GetLastPenContactEvent();

			int touchCount { get; }

			Touch GetTouch(int index);

			bool mousePresent { get; }

			bool GetMouseButtonDown(int button);

			bool GetMouseButtonUp(int button);

			Vector3 mousePosition { get; }

			Vector2 mouseScrollDelta { get; }

			int mouseButtonCount { get; }

			bool anyKey { get; }

			float unscaledTime { get; }

			float doubleClickTime { get; }
		}

		private class Input : DefaultEventSystem.IInput
		{
			public bool GetButtonDown(string button)
			{
				return UnityEngine.Input.GetButtonDown(button);
			}

			public float GetAxisRaw(string axis)
			{
				return UnityEngine.Input.GetAxis(axis);
			}

			public void ResetPenEvents()
			{
				UnityEngine.Input.ResetPenEvents();
			}

			public void ClearLastPenContactEvent()
			{
				UnityEngine.Input.ClearLastPenContactEvent();
			}

			public int penEventCount
			{
				get
				{
					return UnityEngine.Input.penEventCount;
				}
			}

			public PenData GetPenEvent(int index)
			{
				return UnityEngine.Input.GetPenEvent(index);
			}

			public PenData GetLastPenContactEvent()
			{
				return UnityEngine.Input.GetLastPenContactEvent();
			}

			public int touchCount
			{
				get
				{
					return UnityEngine.Input.touchCount;
				}
			}

			public Touch GetTouch(int index)
			{
				return UnityEngine.Input.GetTouch(index);
			}

			public bool mousePresent
			{
				get
				{
					return UnityEngine.Input.mousePresent;
				}
			}

			public bool GetMouseButtonDown(int button)
			{
				return UnityEngine.Input.GetMouseButtonDown(button);
			}

			public bool GetMouseButtonUp(int button)
			{
				return UnityEngine.Input.GetMouseButtonUp(button);
			}

			public Vector3 mousePosition
			{
				get
				{
					return UnityEngine.Input.mousePosition;
				}
			}

			public Vector2 mouseScrollDelta
			{
				get
				{
					return UnityEngine.Input.mouseScrollDelta;
				}
			}

			public int mouseButtonCount
			{
				get
				{
					return 3;
				}
			}

			public bool anyKey
			{
				get
				{
					return UnityEngine.Input.anyKey;
				}
			}

			public float unscaledTime
			{
				get
				{
					return Time.unscaledTime;
				}
			}

			public float doubleClickTime
			{
				get
				{
					return (float)Event.GetDoubleClickTime() * 0.001f;
				}
			}
		}

		private class NoInput : DefaultEventSystem.IInput
		{
			public bool GetButtonDown(string button)
			{
				return false;
			}

			public float GetAxisRaw(string axis)
			{
				return 0f;
			}

			public int touchCount
			{
				get
				{
					return 0;
				}
			}

			public Touch GetTouch(int index)
			{
				return default(Touch);
			}

			public void ResetPenEvents()
			{
			}

			public void ClearLastPenContactEvent()
			{
			}

			public int penEventCount
			{
				get
				{
					return 0;
				}
			}

			public PenData GetPenEvent(int index)
			{
				return default(PenData);
			}

			public PenData GetLastPenContactEvent()
			{
				return default(PenData);
			}

			public bool mousePresent
			{
				get
				{
					return false;
				}
			}

			public bool GetMouseButtonDown(int button)
			{
				return false;
			}

			public bool GetMouseButtonUp(int button)
			{
				return false;
			}

			public Vector3 mousePosition
			{
				get
				{
					return default(Vector3);
				}
			}

			public Vector2 mouseScrollDelta
			{
				get
				{
					return default(Vector2);
				}
			}

			public int mouseButtonCount
			{
				get
				{
					return 0;
				}
			}

			public bool anyKey
			{
				get
				{
					return false;
				}
			}

			public float unscaledTime
			{
				get
				{
					return 0f;
				}
			}

			public float doubleClickTime
			{
				get
				{
					return float.PositiveInfinity;
				}
			}
		}
	}
}
