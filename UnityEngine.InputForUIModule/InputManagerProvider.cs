using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IntegerTime;

namespace UnityEngine.InputForUI
{
	internal class InputManagerProvider : IEventProviderImpl
	{
		private EventModifiers _eventModifiers
		{
			get
			{
				return this._inputEventPartialProvider._eventModifiers;
			}
		}

		public InputManagerProvider()
		{
		}

		internal InputManagerProvider(InputManagerProvider.IInput inputOverride, InputManagerProvider.ITime timeOverride)
		{
			this._input = inputOverride;
			this._time = timeOverride;
		}

		public void Initialize()
		{
			if (this._inputEventPartialProvider == null)
			{
				this._inputEventPartialProvider = new InputEventPartialProvider();
			}
			this._inputEventPartialProvider.Initialize();
			this._inputEventPartialProvider._sendNavigationEventOnTabKey = true;
			this._mouseState.Reset();
			this._isPenPresent = false;
			this._seenAtLeastOnePenPosition = false;
			this._lastSeenPenPositionForDetection = default(Vector2);
			this._penState.Reset();
			this._lastPenData = default(PenData);
			this._touchFingerIdToFingerIndex.Clear();
			this._touchNextFingerIndex = 0;
			this._touchState.Reset();
		}

		public void Shutdown()
		{
		}

		public void Update()
		{
			this._inputEventPartialProvider.Update();
			DiscreteTime discreteTime = (DiscreteTime)this._time.timeAsRational;
			this.DetectPen();
			bool flag = false;
			bool touchSupported = this._input.touchSupported;
			if (touchSupported)
			{
				flag = this.CheckTouchEvents(discreteTime);
			}
			bool flag2 = false;
			bool flag3 = !flag && this._isPenPresent;
			if (flag3)
			{
				DiscreteTime discreteTime2 = discreteTime;
				PenData lastPenContactEvent = this._input.GetLastPenContactEvent();
				flag2 = this.CheckPenEvent(discreteTime2, in lastPenContactEvent);
			}
			else
			{
				this._penState.Reset();
			}
			bool flag4 = !flag2 && !flag && this._input.mousePresent;
			if (flag4)
			{
				this.CheckMouseEvents(discreteTime, false);
			}
			else
			{
				this.CheckMouseEvents(discreteTime, true);
				this._mouseState.LastPositionValid = false;
			}
			bool mousePresent = this._input.mousePresent;
			if (mousePresent)
			{
				this.CheckMouseScroll(discreteTime);
			}
			this.CheckIfIMEChanged(discreteTime);
			this.DirectionNavigation(discreteTime);
			this.SubmitCancelNavigation(discreteTime);
			this.NextPreviousNavigation(discreteTime);
		}

		private bool CheckTouchEvents(DiscreteTime currentTime)
		{
			bool flag = true;
			bool flag2 = false;
			for (int i = 0; i < this._input.touchCount; i++)
			{
				Touch touch = this._input.GetTouch(i);
				bool flag3 = touch.type == TouchType.Indirect || touch.phase == TouchPhase.Stationary;
				if (!flag3)
				{
					int num;
					bool flag4 = !this._touchFingerIdToFingerIndex.TryGetValue(touch.fingerId, out num);
					if (flag4)
					{
						int touchNextFingerIndex = this._touchNextFingerIndex;
						this._touchNextFingerIndex = touchNextFingerIndex + 1;
						num = touchNextFingerIndex;
						this._touchFingerIdToFingerIndex.Add(touch.fingerId, num);
					}
					int num2;
					Vector2 vector = InputManagerProvider.MultiDisplayBottomLeftToPanelPosition(touch.position, out num2);
					Vector2 vector2 = InputManagerProvider.ScreenBottomLeftToPanelDelta(touch.deltaPosition);
					PointerEvent.Type type = PointerEvent.Type.PointerMoved;
					PointerEvent.Button button = PointerEvent.Button.None;
					switch (touch.phase)
					{
					case TouchPhase.Began:
						type = PointerEvent.Type.ButtonPressed;
						button = PointerEvent.Button.Primary;
						flag = false;
						this._touchState.OnButtonDown(currentTime, button);
						break;
					case TouchPhase.Moved:
						flag = false;
						break;
					case TouchPhase.Ended:
						type = PointerEvent.Type.ButtonReleased;
						button = PointerEvent.Button.Primary;
						this._touchState.OnButtonUp(currentTime, button);
						break;
					case TouchPhase.Canceled:
						type = PointerEvent.Type.TouchCanceled;
						button = PointerEvent.Button.Primary;
						this._touchState.OnButtonUp(currentTime, button);
						break;
					}
					Event @event = Event.From(new PointerEvent
					{
						type = type,
						pointerIndex = num,
						position = vector,
						deltaPosition = vector2,
						scroll = Vector2.zero,
						displayIndex = num2,
						tilt = InputManagerProvider.AzimuthAndAlitutudeToTilt(touch.altitudeAngle, touch.azimuthAngle),
						twist = 0f,
						pressure = ((Mathf.Abs(touch.maximumPossiblePressure) > Mathf.Epsilon) ? (touch.pressure / touch.maximumPossiblePressure) : 1f),
						isInverted = false,
						button = button,
						buttonsState = this._touchState.ButtonsState,
						clickCount = this._touchState.ClickCount,
						timestamp = currentTime,
						eventSource = EventSource.Touch,
						playerId = 0U,
						eventModifiers = this._eventModifiers
					});
					EventProvider.Dispatch(in @event);
					flag2 = true;
				}
			}
			bool flag5 = flag;
			if (flag5)
			{
				this._touchNextFingerIndex = 0;
				this._touchFingerIdToFingerIndex.Clear();
			}
			return flag2;
		}

		private void DetectPen()
		{
			bool isPenPresent = this._isPenPresent;
			if (!isPenPresent)
			{
				Vector2 position = this._input.GetLastPenContactEvent().position;
				bool seenAtLeastOnePenPosition = this._seenAtLeastOnePenPosition;
				if (seenAtLeastOnePenPosition)
				{
					float sqrMagnitude = (position - this._lastSeenPenPositionForDetection).sqrMagnitude;
					this._isPenPresent = sqrMagnitude >= 0.01f;
				}
				else
				{
					this._lastSeenPenPositionForDetection = position;
					this._seenAtLeastOnePenPosition = true;
				}
			}
		}

		private static PointerEvent.Button PenStatusToButton(PenStatus status)
		{
			bool flag = (status & PenStatus.Eraser) > PenStatus.None;
			PointerEvent.Button button;
			if (flag)
			{
				button = PointerEvent.Button.PenEraserInTouch;
			}
			else
			{
				bool flag2 = (status & PenStatus.Barrel) > PenStatus.None;
				if (flag2)
				{
					button = PointerEvent.Button.PenBarrelButton;
				}
				else
				{
					button = PointerEvent.Button.Primary;
				}
			}
			return button;
		}

		private bool CheckPenEvent(DiscreteTime currentTime, in PenData currentPenData)
		{
			Vector2 position = currentPenData.position;
			int num = 0;
			Vector2 vector = (this._penState.LastPositionValid ? (position - this._penState.LastPosition) : Vector2.zero);
			PointerEvent.Button button = PointerEvent.Button.None;
			bool flag = currentPenData.contactType != this._lastPenData.contactType;
			PointerEvent.Type type;
			if (flag)
			{
				PenEventType contactType = currentPenData.contactType;
				PenEventType penEventType = contactType;
				if (penEventType != PenEventType.PenDown)
				{
					if (penEventType != PenEventType.PenUp)
					{
						type = PointerEvent.Type.PointerMoved;
					}
					else
					{
						type = PointerEvent.Type.ButtonReleased;
						button = InputManagerProvider.PenStatusToButton(this._lastPenData.penStatus);
						this._penState.OnButtonUp(currentTime, button);
					}
				}
				else
				{
					type = PointerEvent.Type.ButtonPressed;
					button = InputManagerProvider.PenStatusToButton(currentPenData.penStatus);
					this._penState.OnButtonDown(currentTime, button);
				}
			}
			else
			{
				type = PointerEvent.Type.PointerMoved;
			}
			this._lastPenData = currentPenData;
			bool flag2 = false;
			bool flag3 = type != PointerEvent.Type.PointerMoved || !this._penState.LastPositionValid || vector.sqrMagnitude >= 0.01f;
			if (flag3)
			{
				Event @event = Event.From(new PointerEvent
				{
					type = type,
					pointerIndex = 0,
					position = position,
					deltaPosition = vector,
					scroll = Vector2.zero,
					displayIndex = num,
					tilt = currentPenData.tilt,
					twist = currentPenData.twist,
					pressure = currentPenData.pressure,
					isInverted = ((currentPenData.penStatus & PenStatus.Inverted) > PenStatus.None),
					button = button,
					buttonsState = this._penState.ButtonsState,
					clickCount = this._penState.ClickCount,
					timestamp = currentTime,
					eventSource = EventSource.Pen,
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(in @event);
				flag2 = true;
			}
			this._penState.OnMove(currentTime, position, num);
			return flag2;
		}

		private void CheckMouseEvents(DiscreteTime currentTime, bool muted = false)
		{
			int num;
			Vector2 vector = InputManagerProvider.MultiDisplayBottomLeftToPanelPosition(this._input.mousePosition, out num);
			bool lastPositionValid = this._mouseState.LastPositionValid;
			if (lastPositionValid)
			{
				Vector2 vector2 = vector - this._mouseState.LastPosition;
				bool flag = vector2.sqrMagnitude >= 0.01f;
				if (flag)
				{
					bool flag2 = !muted;
					if (flag2)
					{
						PointerEvent pointerEvent = new PointerEvent
						{
							type = PointerEvent.Type.PointerMoved,
							pointerIndex = 0,
							position = vector,
							deltaPosition = vector2,
							scroll = Vector2.zero,
							displayIndex = num,
							tilt = Vector2.zero,
							twist = 0f,
							pressure = 0f,
							isInverted = false,
							button = PointerEvent.Button.None,
							buttonsState = this._mouseState.ButtonsState,
							clickCount = 0,
							timestamp = currentTime,
							eventSource = EventSource.Mouse,
							playerId = 0U,
							eventModifiers = this._eventModifiers
						};
						Event @event = Event.From(pointerEvent);
						EventProvider.Dispatch(in @event);
					}
					this._mouseState.OnMove(currentTime, vector, num);
				}
			}
			else
			{
				this._mouseState.OnMove(currentTime, vector, num);
			}
			for (int i = 0; i < 5; i++)
			{
				PointerEvent.Button button = PointerEvent.ButtonFromButtonIndex(i);
				bool flag3 = this._mouseState.ButtonsState.Get(button);
				bool mouseButtonDown = this._input.GetMouseButtonDown(i);
				bool mouseButtonUp = this._input.GetMouseButtonUp(i);
				bool mouseButton = this._input.GetMouseButton(i);
				InputManagerProvider.ButtonEventsIterator buttonEventsIterator = InputManagerProvider.ButtonEventsIterator.FromState(flag3, mouseButtonDown, mouseButtonUp, mouseButton);
				bool flag4 = flag3;
				while (buttonEventsIterator.MoveNext())
				{
					bool flag5 = buttonEventsIterator.Current;
					this._mouseState.OnButtonChange(currentTime, button, flag4, flag5);
					flag4 = buttonEventsIterator.Current;
					bool flag6 = !muted;
					if (flag6)
					{
						PointerEvent pointerEvent = new PointerEvent
						{
							type = (buttonEventsIterator.Current ? PointerEvent.Type.ButtonPressed : PointerEvent.Type.ButtonReleased),
							pointerIndex = 0,
							position = this._mouseState.LastPosition,
							deltaPosition = Vector2.zero,
							scroll = Vector2.zero,
							displayIndex = this._mouseState.LastDisplayIndex,
							tilt = Vector2.zero,
							twist = 0f,
							pressure = 0f,
							isInverted = false,
							button = button,
							buttonsState = this._mouseState.ButtonsState,
							clickCount = this._mouseState.ClickCount,
							timestamp = currentTime,
							eventSource = EventSource.Mouse,
							playerId = 0U,
							eventModifiers = this._eventModifiers
						};
						Event @event = Event.From(pointerEvent);
						EventProvider.Dispatch(in @event);
					}
				}
			}
		}

		private void CheckMouseScroll(DiscreteTime currentTime)
		{
			Vector2 mouseScrollDelta = this._input.mouseScrollDelta;
			bool flag = mouseScrollDelta.sqrMagnitude < 0.01f;
			if (!flag)
			{
				int num = 0;
				bool lastPositionValid = this._mouseState.LastPositionValid;
				Vector2 vector;
				if (lastPositionValid)
				{
					vector = this._mouseState.LastPosition;
					num = this._mouseState.LastDisplayIndex;
				}
				else
				{
					vector = InputManagerProvider.MultiDisplayBottomLeftToPanelPosition(this._input.mousePosition, out num);
				}
				mouseScrollDelta.x *= 3f;
				mouseScrollDelta.y *= -3f;
				Event @event = Event.From(new PointerEvent
				{
					type = PointerEvent.Type.Scroll,
					pointerIndex = 0,
					position = vector,
					deltaPosition = Vector2.zero,
					scroll = mouseScrollDelta,
					displayIndex = num,
					tilt = Vector2.zero,
					twist = 0f,
					pressure = 0f,
					isInverted = false,
					button = PointerEvent.Button.None,
					buttonsState = this._mouseState.ButtonsState,
					clickCount = 0,
					timestamp = currentTime,
					eventSource = EventSource.Mouse,
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(in @event);
			}
		}

		private PointerEvent ToPointerStateEvent(DiscreteTime currentTime, in PointerState state, EventSource eventSource)
		{
			PointerEvent pointerEvent = default(PointerEvent);
			pointerEvent.type = PointerEvent.Type.State;
			pointerEvent.pointerIndex = 0;
			pointerEvent.position = state.LastPosition;
			pointerEvent.deltaPosition = Vector2.zero;
			pointerEvent.scroll = Vector2.zero;
			pointerEvent.displayIndex = state.LastDisplayIndex;
			pointerEvent.tilt = ((eventSource == EventSource.Pen) ? this._lastPenData.tilt : Vector2.zero);
			pointerEvent.twist = ((eventSource == EventSource.Pen) ? this._lastPenData.twist : 0f);
			pointerEvent.pressure = ((eventSource == EventSource.Pen) ? this._lastPenData.pressure : 0f);
			pointerEvent.isInverted = eventSource == EventSource.Pen && (this._lastPenData.penStatus & PenStatus.Inverted) > PenStatus.None;
			pointerEvent.button = PointerEvent.Button.None;
			PointerState pointerState = state;
			pointerEvent.buttonsState = pointerState.ButtonsState;
			pointerEvent.clickCount = 0;
			pointerEvent.timestamp = currentTime;
			pointerEvent.eventSource = eventSource;
			pointerEvent.playerId = 0U;
			pointerEvent.eventModifiers = this._eventModifiers;
			return pointerEvent;
		}

		private void NextPreviousNavigation(DiscreteTime currentTime)
		{
			int num = (this.InputManagerGetButtonDownOrDefault(this._configuration.NavigateNextButton) ? 1 : 0) + (this.InputManagerGetButtonDownOrDefault(this._configuration.NavigatePreviousButton) ? (-1) : 0);
			bool flag = num != 0;
			if (flag)
			{
				bool isShiftPressed = this._eventModifiers.isShiftPressed;
				if (isShiftPressed)
				{
					num = -num;
				}
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Move,
					direction = ((num >= 0) ? NavigationEvent.Direction.Next : NavigationEvent.Direction.Previous),
					timestamp = currentTime,
					eventSource = this.GetEventSourceFromPressedKey(),
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(in @event);
			}
		}

		private void SubmitCancelNavigation(DiscreteTime currentTime)
		{
			bool flag = this.InputManagerGetButtonDownOrDefault(this._configuration.SubmitButton);
			if (flag)
			{
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Submit,
					direction = NavigationEvent.Direction.None,
					timestamp = currentTime,
					eventSource = this.GetEventSourceFromPressedKey(),
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(in @event);
			}
			bool flag2 = this.InputManagerGetButtonDownOrDefault(this._configuration.CancelButton);
			if (flag2)
			{
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Cancel,
					direction = NavigationEvent.Direction.None,
					timestamp = currentTime,
					eventSource = this.GetEventSourceFromPressedKey(),
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(in @event);
			}
		}

		private void DirectionNavigation(DiscreteTime currentTime)
		{
			ValueTuple<Vector2, bool> valueTuple = this.ReadCurrentNavigationMoveVector();
			Vector2 item = valueTuple.Item1;
			bool item2 = valueTuple.Item2;
			NavigationEvent.Direction direction = NavigationEvent.DetermineMoveDirection(item, 0.6f);
			bool flag = direction == NavigationEvent.Direction.None;
			if (flag)
			{
				this._navigationEventRepeatHelper.Reset();
			}
			else
			{
				bool flag2 = this._navigationEventRepeatHelper.ShouldSendMoveEvent(currentTime, direction, item2);
				if (flag2)
				{
					EventSource eventSource = this.GetEventSourceFromPressedKey();
					bool flag3 = eventSource == EventSource.Unspecified && !item2;
					if (flag3)
					{
						eventSource = EventSource.Gamepad;
					}
					Event @event = Event.From(new NavigationEvent
					{
						type = NavigationEvent.Type.Move,
						direction = direction,
						timestamp = currentTime,
						eventSource = eventSource,
						playerId = 0U,
						eventModifiers = this._eventModifiers
					});
					EventProvider.Dispatch(in @event);
				}
			}
		}

		private void CheckIfIMEChanged(DiscreteTime currentTime)
		{
			string compositionString = this._input.compositionString;
			bool flag = this._compositionString != compositionString;
			if (flag)
			{
				this._compositionString = compositionString;
				Event @event = Event.From(this.ToIMECompositionEvent(currentTime, this._compositionString));
				EventProvider.Dispatch(in @event);
			}
		}

		public void OnFocusChanged(bool focus)
		{
			this._inputEventPartialProvider.OnFocusChanged(focus);
		}

		public bool RequestCurrentState(Event.Type type)
		{
			bool flag = this._inputEventPartialProvider.RequestCurrentState(type);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				DiscreteTime discreteTime = (DiscreteTime)this._time.timeAsRational;
				if (type != Event.Type.PointerEvent)
				{
					if (type != Event.Type.IMECompositionEvent)
					{
						flag2 = false;
					}
					else
					{
						Event @event = Event.From(this.ToIMECompositionEvent(discreteTime, this._compositionString));
						EventProvider.Dispatch(in @event);
						flag2 = true;
					}
				}
				else
				{
					bool lastPositionValid = this._touchState.LastPositionValid;
					if (lastPositionValid)
					{
						Event @event = Event.From(this.ToPointerStateEvent(discreteTime, in this._touchState, EventSource.Touch));
						EventProvider.Dispatch(in @event);
					}
					bool lastPositionValid2 = this._penState.LastPositionValid;
					if (lastPositionValid2)
					{
						Event @event = Event.From(this.ToPointerStateEvent(discreteTime, in this._penState, EventSource.Pen));
						EventProvider.Dispatch(in @event);
					}
					bool lastPositionValid3 = this._mouseState.LastPositionValid;
					if (lastPositionValid3)
					{
						Event @event = Event.From(this.ToPointerStateEvent(discreteTime, in this._mouseState, EventSource.Mouse));
						EventProvider.Dispatch(in @event);
					}
					flag2 = this._touchState.LastPositionValid || this._penState.LastPositionValid || this._mouseState.LastPositionValid;
				}
			}
			return flag2;
		}

		public uint playerCount
		{
			get
			{
				return 1U;
			}
		}

		private EventSource GetEventSourceFromPressedKey()
		{
			bool flag = this.InputManagerKeyboardWasPressed();
			EventSource eventSource;
			if (flag)
			{
				eventSource = EventSource.Keyboard;
			}
			else
			{
				bool flag2 = this.InputManagerJoystickWasPressed();
				if (flag2)
				{
					eventSource = EventSource.Gamepad;
				}
				else
				{
					eventSource = EventSource.Unspecified;
				}
			}
			return eventSource;
		}

		private bool InputManagerJoystickWasPressed()
		{
			for (KeyCode keyCode = KeyCode.Joystick1Button0; keyCode <= KeyCode.Joystick8Button19; keyCode++)
			{
				bool key = this._input.GetKey(keyCode);
				if (key)
				{
					return true;
				}
			}
			return false;
		}

		private bool InputManagerKeyboardWasPressed()
		{
			for (KeyCode keyCode = KeyCode.None; keyCode <= KeyCode.Menu; keyCode++)
			{
				bool key = this._input.GetKey(keyCode);
				if (key)
				{
					return true;
				}
			}
			return false;
		}

		private float InputManagerGetAxisRawOrDefault(string axisName)
		{
			float num;
			try
			{
				num = ((!string.IsNullOrEmpty(axisName)) ? this._input.GetAxisRaw(axisName) : 0f);
			}
			catch
			{
				num = 0f;
			}
			return num;
		}

		private bool InputManagerGetButtonDownOrDefault(string axisName)
		{
			bool flag;
			try
			{
				flag = !string.IsNullOrEmpty(axisName) && this._input.GetButtonDown(axisName);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private ValueTuple<Vector2, bool> ReadCurrentNavigationMoveVector()
		{
			Vector2 vector = new Vector2(this.InputManagerGetAxisRawOrDefault(this._configuration.HorizontalAxis), this.InputManagerGetAxisRawOrDefault(this._configuration.VerticalAxis));
			bool flag = false;
			bool flag2 = this.InputManagerGetButtonDownOrDefault(this._configuration.HorizontalAxis);
			if (flag2)
			{
				bool flag3 = vector.x < 0f;
				if (flag3)
				{
					vector.x = -1f;
				}
				else
				{
					bool flag4 = vector.x > 0f;
					if (flag4)
					{
						vector.x = 1f;
					}
				}
				flag = true;
			}
			bool flag5 = this.InputManagerGetButtonDownOrDefault(this._configuration.VerticalAxis);
			if (flag5)
			{
				bool flag6 = vector.y < 0f;
				if (flag6)
				{
					vector.y = -1f;
				}
				else
				{
					bool flag7 = vector.y > 0f;
					if (flag7)
					{
						vector.y = 1f;
					}
				}
				flag = true;
			}
			return new ValueTuple<Vector2, bool>(vector, flag);
		}

		private IMECompositionEvent ToIMECompositionEvent(DiscreteTime currentTime, string compositionString)
		{
			return new IMECompositionEvent
			{
				compositionString = compositionString,
				timestamp = currentTime,
				eventSource = EventSource.Unspecified,
				playerId = 0U,
				eventModifiers = this._eventModifiers
			};
		}

		internal static float TiltToAzimuth(Vector2 tilt)
		{
			float num = 0f;
			bool flag = tilt.x != 0f;
			if (flag)
			{
				num = 1.5707964f - Mathf.Atan2(-Mathf.Cos(tilt.x) * Mathf.Sin(tilt.y), Mathf.Cos(tilt.y) * Mathf.Sin(tilt.x));
				bool flag2 = num < 0f;
				if (flag2)
				{
					num += 6.2831855f;
				}
				bool flag3 = num >= 1.5707964f;
				if (flag3)
				{
					num -= 1.5707964f;
				}
				else
				{
					num += 4.712389f;
				}
			}
			return num;
		}

		internal static Vector2 AzimuthAndAlitutudeToTilt(float altitude, float azimuth)
		{
			return new Vector2(0f, 0f)
			{
				x = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Cos(altitude) / Mathf.Sin(azimuth)),
				y = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Sin(altitude) / Mathf.Sin(azimuth))
			};
		}

		internal static float TiltToAltitude(Vector2 tilt)
		{
			return 1.5707964f - Mathf.Acos(Mathf.Cos(tilt.x) * Mathf.Cos(tilt.y));
		}

		private static Vector2 MultiDisplayBottomLeftToPanelPosition(Vector2 position, out int targetDisplay)
		{
			int? num;
			Vector2 vector = InputManagerProvider.MultiDisplayToLocalScreenPosition(position, out num);
			targetDisplay = num.GetValueOrDefault();
			return InputManagerProvider.ScreenBottomLeftToPanelPosition(vector, targetDisplay);
		}

		private static Vector2 MultiDisplayToLocalScreenPosition(Vector2 position, out int? targetDisplay)
		{
			Vector3 vector = Display.RelativeMouseAt(position);
			bool flag = vector != Vector3.zero;
			Vector2 vector2;
			if (flag)
			{
				targetDisplay = new int?((int)vector.z);
				vector2 = vector;
			}
			else
			{
				targetDisplay = null;
				vector2 = position;
			}
			return vector2;
		}

		private static Vector2 ScreenBottomLeftToPanelPosition(Vector2 position, int targetDisplay)
		{
			int num = Screen.height;
			bool flag = targetDisplay > 0 && targetDisplay < Display.displays.Length;
			if (flag)
			{
				num = Display.displays[targetDisplay].systemHeight;
			}
			position.y = (float)num - position.y;
			return position;
		}

		private static Vector2 ScreenBottomLeftToPanelDelta(Vector2 delta)
		{
			delta.y = -delta.y;
			return delta;
		}

		private InputEventPartialProvider _inputEventPartialProvider;

		private const int kDefaultPlayerId = 0;

		private string _compositionString = string.Empty;

		private InputManagerProvider.Configuration _configuration = InputManagerProvider.Configuration.GetDefaultConfiguration();

		private InputManagerProvider.IInput _input = new InputManagerProvider.Input();

		private InputManagerProvider.ITime _time = new InputManagerProvider.Time();

		private NavigationEventRepeatHelper _navigationEventRepeatHelper = new NavigationEventRepeatHelper();

		private const int kMaxMouseButtons = 5;

		private PointerState _mouseState;

		private bool _isPenPresent;

		private bool _seenAtLeastOnePenPosition;

		private Vector2 _lastSeenPenPositionForDetection;

		private PointerState _penState;

		private PenData _lastPenData;

		private Dictionary<int, int> _touchFingerIdToFingerIndex = new Dictionary<int, int>();

		private int _touchNextFingerIndex;

		private PointerState _touchState;

		private const float kSmallestReportedMovementSqrDist = 0.01f;

		private const float kScrollUGUIScaleFactor = 3f;

		private struct ButtonEventsIterator : IEnumerator
		{
			public bool Current
			{
				get
				{
					return this._bit % 2 == 0;
				}
			}

			public bool MoveNext()
			{
				for (;;)
				{
					this._bit++;
					bool flag = (this._mask & (1U << this._bit)) > 0U;
					if (flag)
					{
						break;
					}
					if (this._bit >= 4)
					{
						goto Block_1;
					}
				}
				return true;
				Block_1:
				return false;
			}

			public void Reset()
			{
				this._bit = -1;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public static InputManagerProvider.ButtonEventsIterator FromState(bool previous, bool down, bool up, bool current)
			{
				uint num = ((!previous && current) ? 1U : ((previous && !current) ? 2U : 0U));
				return new InputManagerProvider.ButtonEventsIterator
				{
					_mask = num,
					_bit = -1
				};
			}

			private uint _mask;

			private int _bit;

			private const uint kWasPressed = 1U;

			private const uint kWasReleased = 2U;

			private const int kMaxBits = 4;
		}

		public struct Configuration
		{
			public static InputManagerProvider.Configuration GetDefaultConfiguration()
			{
				return new InputManagerProvider.Configuration
				{
					HorizontalAxis = "Horizontal",
					VerticalAxis = "Vertical",
					SubmitButton = "Submit",
					CancelButton = "Cancel",
					NavigateNextButton = "Next",
					NavigatePreviousButton = "Previous",
					InputActionsPerSecond = 10f,
					RepeatDelay = 0.5f
				};
			}

			public string HorizontalAxis;

			public string VerticalAxis;

			public string SubmitButton;

			public string CancelButton;

			public string NavigateNextButton;

			public string NavigatePreviousButton;

			public float InputActionsPerSecond;

			public float RepeatDelay;
		}

		internal interface IInput
		{
			string compositionString { get; }

			bool GetKey(KeyCode keyCode);

			bool GetKeyDown(KeyCode keyCode);

			bool GetButtonDown(string button);

			float GetAxisRaw(string axis);

			PenData GetPenEvent(int index);

			PenData GetLastPenContactEvent();

			bool touchSupported { get; }

			int touchCount { get; }

			Touch GetTouch(int index);

			bool mousePresent { get; }

			bool GetMouseButton(int button);

			bool GetMouseButtonDown(int button);

			bool GetMouseButtonUp(int button);

			Vector3 mousePosition { get; }

			Vector2 mouseScrollDelta { get; }
		}

		private class Input : InputManagerProvider.IInput
		{
			public string compositionString
			{
				get
				{
					return UnityEngine.Input.compositionString;
				}
			}

			public bool GetKey(KeyCode key)
			{
				return UnityEngine.Input.GetKey(key);
			}

			public bool GetKeyDown(KeyCode key)
			{
				return UnityEngine.Input.GetKeyDown(key);
			}

			public bool GetButtonDown(string button)
			{
				return UnityEngine.Input.GetButtonDown(button);
			}

			public float GetAxisRaw(string axis)
			{
				return UnityEngine.Input.GetAxisRaw(axis);
			}

			public PenData GetPenEvent(int index)
			{
				return UnityEngine.Input.GetPenEvent(index);
			}

			public PenData GetLastPenContactEvent()
			{
				return UnityEngine.Input.GetLastPenContactEvent();
			}

			public bool touchSupported
			{
				get
				{
					return UnityEngine.Input.touchSupported;
				}
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

			public bool GetMouseButton(int button)
			{
				return UnityEngine.Input.GetMouseButton(button);
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
		}

		internal interface ITime
		{
			RationalTime timeAsRational { get; }
		}

		private class Time : InputManagerProvider.ITime
		{
			public RationalTime timeAsRational
			{
				get
				{
					return UnityEngine.Time.timeAsRational;
				}
			}
		}
	}
}
