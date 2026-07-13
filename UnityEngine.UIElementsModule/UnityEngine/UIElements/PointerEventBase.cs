using System;
using UnityEngine.InputForUI;

namespace UnityEngine.UIElements
{
	[EventCategory(EventCategory.Pointer)]
	public abstract class PointerEventBase<T> : EventBase<T>, IPointerEvent, IPointerEventInternal, IPointerOrMouseEvent where T : PointerEventBase<T>, new()
	{
		public int pointerId { get; protected set; }

		public string pointerType { get; protected set; }

		public bool isPrimary { get; protected set; }

		public int button { get; protected set; }

		public int pressedButtons { get; protected set; }

		public Vector3 position { get; protected set; }

		public Vector3 localPosition { get; protected set; }

		public Vector3 deltaPosition { get; protected set; }

		public float deltaTime { get; protected set; }

		public int clickCount { get; protected set; }

		public float pressure { get; protected set; }

		public float tangentialPressure { get; protected set; }

		public float altitudeAngle
		{
			get
			{
				bool altitudeNeedsConversion = this.m_AltitudeNeedsConversion;
				if (altitudeNeedsConversion)
				{
					this.m_AltitudeAngle = PointerEventBase<T>.TiltToAltitude(this.tilt);
					this.m_AltitudeNeedsConversion = false;
				}
				return this.m_AltitudeAngle;
			}
			protected set
			{
				this.m_AltitudeNeedsConversion = true;
				this.m_AltitudeAngle = value;
			}
		}

		public float azimuthAngle
		{
			get
			{
				bool azimuthNeedsConversion = this.m_AzimuthNeedsConversion;
				if (azimuthNeedsConversion)
				{
					this.m_AzimuthAngle = PointerEventBase<T>.TiltToAzimuth(this.tilt);
					this.m_AzimuthNeedsConversion = false;
				}
				return this.m_AzimuthAngle;
			}
			protected set
			{
				this.m_AzimuthNeedsConversion = true;
				this.m_AzimuthAngle = value;
			}
		}

		public float twist { get; protected set; }

		public Vector2 tilt
		{
			get
			{
				bool flag = Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer && this.pointerType == PointerType.touch && this.m_TiltNeeded;
				if (flag)
				{
					this.m_Tilt = PointerEventBase<T>.AzimuthAndAlitutudeToTilt(this.m_AltitudeAngle, this.m_AzimuthAngle);
					this.m_TiltNeeded = false;
				}
				return this.m_Tilt;
			}
			protected set
			{
				this.m_TiltNeeded = true;
				this.m_Tilt = value;
			}
		}

		public PenStatus penStatus { get; protected set; }

		public Vector2 radius { get; protected set; }

		public Vector2 radiusVariance { get; protected set; }

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

		internal IMouseEvent compatibilityMouseEvent { get; set; }

		internal int displayIndex { get; set; }

		internal bool recomputeTopElementUnderPointer { get; set; }

		IMouseEvent IPointerEventInternal.compatibilityMouseEvent
		{
			get
			{
				return this.compatibilityMouseEvent;
			}
		}

		int IPointerEventInternal.displayIndex
		{
			get
			{
				return this.displayIndex;
			}
		}

		bool IPointerEventInternal.recomputeTopElementUnderPointer
		{
			get
			{
				return this.recomputeTopElementUnderPointer;
			}
		}

		Vector3 IPointerOrMouseEvent.deltaPosition
		{
			get
			{
				return this.deltaPosition;
			}
			set
			{
				this.deltaPosition = value;
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
			this.pointerId = 0;
			this.pointerType = PointerType.unknown;
			this.isPrimary = false;
			this.button = -1;
			this.pressedButtons = 0;
			this.position = Vector3.zero;
			this.localPosition = Vector3.zero;
			this.deltaPosition = Vector3.zero;
			this.deltaTime = 0f;
			this.clickCount = 0;
			this.pressure = 0f;
			this.tangentialPressure = 0f;
			this.altitudeAngle = 0f;
			this.azimuthAngle = 0f;
			this.tilt = new Vector2(0f, 0f);
			this.twist = 0f;
			this.penStatus = PenStatus.None;
			this.radius = Vector2.zero;
			this.radiusVariance = Vector2.zero;
			this.modifiers = EventModifiers.None;
			IDisposable disposable = (IDisposable)this.compatibilityMouseEvent;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			this.compatibilityMouseEvent = null;
			this.displayIndex = 0;
			this.recomputeTopElementUnderPointer = false;
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
					this.localPosition = visualElement.WorldToLocal3D(this.position);
				}
				else
				{
					this.localPosition = this.position;
				}
			}
		}

		private static bool IsMouse(Event systemEvent)
		{
			EventType rawType = systemEvent.rawType;
			return rawType == EventType.MouseMove || rawType == EventType.MouseDown || rawType == EventType.MouseUp || rawType == EventType.MouseDrag || rawType == EventType.ContextClick || rawType == EventType.MouseEnterWindow || rawType == EventType.MouseLeaveWindow;
		}

		private static bool IsTouch(Event systemEvent)
		{
			EventType rawType = systemEvent.rawType;
			return rawType == EventType.TouchMove || rawType == EventType.TouchDown || rawType == EventType.TouchUp || rawType == EventType.TouchStationary || rawType == EventType.TouchEnter || rawType == EventType.TouchLeave;
		}

		private static float TiltToAzimuth(Vector2 tilt)
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

		private static Vector2 AzimuthAndAlitutudeToTilt(float altitude, float azimuth)
		{
			return new Vector2(0f, 0f)
			{
				x = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Cos(altitude) / Mathf.Sin(azimuth)),
				y = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Sin(altitude) / Mathf.Sin(azimuth))
			};
		}

		private static float TiltToAltitude(Vector2 tilt)
		{
			return 1.5707964f - Mathf.Acos(Mathf.Cos(tilt.x) * Mathf.Cos(tilt.y));
		}

		public static T GetPooled(Event systemEvent)
		{
			T pooled = EventBase<T>.GetPooled();
			bool flag = !PointerEventBase<T>.IsMouse(systemEvent) && !PointerEventBase<T>.IsTouch(systemEvent) && systemEvent.rawType != EventType.DragUpdated;
			if (flag)
			{
				Debug.Assert(false, string.Concat(new string[]
				{
					"Unexpected event type: ",
					systemEvent.rawType.ToString(),
					" (",
					systemEvent.type.ToString(),
					")"
				}));
			}
			PointerType pointerType = systemEvent.pointerType;
			PointerType pointerType2 = pointerType;
			if (pointerType2 != PointerType.Touch)
			{
				if (pointerType2 != PointerType.Pen)
				{
					pooled.pointerType = PointerType.mouse;
					pooled.pointerId = PointerId.mousePointerId;
				}
				else
				{
					pooled.pointerType = PointerType.pen;
					pooled.pointerId = PointerId.penPointerIdBase;
					bool flag2 = systemEvent.penStatus == PenStatus.Barrel;
					if (flag2)
					{
						PointerDeviceState.PressButton(pooled.pointerId, 1);
					}
					else
					{
						PointerDeviceState.ReleaseButton(pooled.pointerId, 1);
					}
					bool flag3 = systemEvent.penStatus == PenStatus.Eraser;
					if (flag3)
					{
						PointerDeviceState.PressButton(pooled.pointerId, 5);
					}
					else
					{
						PointerDeviceState.ReleaseButton(pooled.pointerId, 5);
					}
				}
			}
			else
			{
				pooled.pointerType = PointerType.touch;
				pooled.pointerId = PointerId.touchPointerIdBase;
			}
			pooled.isPrimary = true;
			pooled.altitudeAngle = 0f;
			pooled.azimuthAngle = 0f;
			pooled.radius = Vector2.zero;
			pooled.radiusVariance = Vector2.zero;
			pooled.imguiEvent = systemEvent;
			bool flag4 = systemEvent.rawType == EventType.MouseDown || systemEvent.rawType == EventType.TouchDown;
			if (flag4)
			{
				PointerDeviceState.PressButton(pooled.pointerId, systemEvent.button);
				pooled.button = systemEvent.button;
			}
			else
			{
				bool flag5 = systemEvent.rawType == EventType.MouseUp || systemEvent.rawType == EventType.TouchUp;
				if (flag5)
				{
					PointerDeviceState.ReleaseButton(pooled.pointerId, systemEvent.button);
					pooled.button = systemEvent.button;
				}
				else
				{
					bool flag6 = systemEvent.rawType == EventType.MouseMove || systemEvent.rawType == EventType.TouchMove;
					if (flag6)
					{
						pooled.button = -1;
					}
				}
			}
			pooled.pressedButtons = PointerDeviceState.GetPressedButtons(pooled.pointerId);
			pooled.position = systemEvent.mousePosition;
			pooled.localPosition = systemEvent.mousePosition;
			pooled.deltaPosition = systemEvent.delta;
			pooled.clickCount = systemEvent.clickCount;
			pooled.modifiers = systemEvent.modifiers;
			pooled.tilt = systemEvent.tilt;
			pooled.penStatus = systemEvent.penStatus;
			pooled.twist = systemEvent.twist;
			PointerType pointerType3 = systemEvent.pointerType;
			PointerType pointerType4 = pointerType3;
			if (pointerType4 != PointerType.Touch)
			{
				if (pointerType4 != PointerType.Pen)
				{
					pooled.pressure = ((pooled.pressedButtons == 0) ? 0f : 0.5f);
				}
				else
				{
					pooled.pressure = systemEvent.pressure;
				}
			}
			else
			{
				pooled.pressure = systemEvent.pressure;
			}
			pooled.tangentialPressure = 0f;
			return pooled;
		}

		internal static T GetPooled(EventType eventType, Vector3 mousePosition, Vector2 delta, int button, int clickCount, EventModifiers modifiers, int displayIndex)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.pointerId = PointerId.mousePointerId;
			pooled.pointerType = PointerType.mouse;
			pooled.isPrimary = true;
			pooled.displayIndex = displayIndex;
			bool flag = eventType == EventType.MouseDown;
			if (flag)
			{
				PointerDeviceState.PressButton(pooled.pointerId, button);
				pooled.button = button;
			}
			else
			{
				bool flag2 = eventType == EventType.MouseUp;
				if (flag2)
				{
					PointerDeviceState.ReleaseButton(pooled.pointerId, button);
					pooled.button = button;
				}
				else
				{
					pooled.button = -1;
				}
			}
			pooled.pressedButtons = PointerDeviceState.GetPressedButtons(pooled.pointerId);
			pooled.position = mousePosition;
			pooled.localPosition = mousePosition;
			pooled.deltaPosition = delta;
			pooled.clickCount = clickCount;
			pooled.modifiers = modifiers;
			pooled.pressure = ((pooled.pressedButtons == 0) ? 0f : 0.5f);
			return pooled;
		}

		public static T GetPooled(Touch touch, EventModifiers modifiers = EventModifiers.None)
		{
			return PointerEventBase<T>.GetPooled(touch, touch.fingerId + PointerId.touchPointerIdBase, modifiers, 0);
		}

		internal static T GetPooled(Touch touch, int pointerId, EventModifiers modifiers, int displayIndex)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.pointerId = pointerId;
			pooled.pointerType = PointerType.touch;
			pooled.displayIndex = displayIndex;
			pooled.isPrimary = pointerId == PointerId.touchPointerIdBase;
			bool flag = touch.phase == TouchPhase.Began;
			if (flag)
			{
				PointerDeviceState.PressButton(pooled.pointerId, 0);
				pooled.button = 0;
			}
			else
			{
				bool flag2 = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
				if (flag2)
				{
					PointerDeviceState.ReleaseButton(pooled.pointerId, 0);
					pooled.button = 0;
				}
				else
				{
					pooled.button = -1;
				}
			}
			pooled.pressedButtons = PointerDeviceState.GetPressedButtons(pooled.pointerId);
			pooled.position = touch.position;
			pooled.localPosition = touch.position;
			pooled.deltaPosition = touch.deltaPosition;
			pooled.deltaTime = touch.deltaTime;
			pooled.clickCount = touch.tapCount;
			pooled.pressure = ((Mathf.Abs(touch.maximumPossiblePressure) > 1E-30f) ? (touch.pressure / touch.maximumPossiblePressure) : 1f);
			pooled.tangentialPressure = 0f;
			pooled.altitudeAngle = touch.altitudeAngle;
			pooled.azimuthAngle = touch.azimuthAngle;
			pooled.twist = 0f;
			pooled.tilt = new Vector2(0f, 0f);
			pooled.penStatus = PenStatus.None;
			pooled.radius = new Vector2(touch.radius, touch.radius);
			pooled.radiusVariance = new Vector2(touch.radiusVariance, touch.radiusVariance);
			pooled.modifiers = modifiers;
			return pooled;
		}

		public static T GetPooled(PenData pen, EventModifiers modifiers = EventModifiers.None)
		{
			return PointerEventBase<T>.GetPooled(pen, modifiers, 0);
		}

		internal static T GetPooled(PenData pen, EventModifiers modifiers, int displayIndex)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.pointerId = PointerId.penPointerIdBase;
			pooled.pointerType = PointerType.pen;
			pooled.displayIndex = displayIndex;
			pooled.isPrimary = true;
			bool flag = pen.contactType == PenEventType.PenDown;
			if (flag)
			{
				PointerDeviceState.PressButton(pooled.pointerId, 0);
				pooled.button = 0;
			}
			else
			{
				bool flag2 = pen.contactType == PenEventType.PenUp;
				if (flag2)
				{
					PointerDeviceState.ReleaseButton(pooled.pointerId, 0);
					pooled.button = 0;
				}
				else
				{
					pooled.button = -1;
				}
			}
			bool flag3 = pen.penStatus == PenStatus.Barrel;
			if (flag3)
			{
				PointerDeviceState.PressButton(pooled.pointerId, 1);
			}
			else
			{
				PointerDeviceState.ReleaseButton(pooled.pointerId, 1);
			}
			bool flag4 = pen.penStatus == PenStatus.Eraser;
			if (flag4)
			{
				PointerDeviceState.PressButton(pooled.pointerId, 5);
			}
			else
			{
				PointerDeviceState.ReleaseButton(pooled.pointerId, 5);
			}
			pooled.pressedButtons = PointerDeviceState.GetPressedButtons(pooled.pointerId);
			pooled.position = pen.position;
			pooled.localPosition = pen.position;
			pooled.deltaPosition = pen.deltaPos;
			pooled.clickCount = 0;
			pooled.pressure = pen.pressure;
			pooled.tangentialPressure = 0f;
			pooled.twist = pen.twist;
			pooled.tilt = pen.tilt;
			pooled.penStatus = pen.penStatus;
			pooled.radius = Vector2.zero;
			pooled.radiusVariance = Vector2.zero;
			pooled.modifiers = modifiers;
			return pooled;
		}

		internal static T GetPooled(PointerEvent pointerEvent, Vector3 position, int pointerId, float deltaTime)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.position = position;
			pooled.localPosition = position;
			pooled.deltaPosition = PointerDeviceState.GetPointerDeltaPosition(pointerId, ContextType.Player, position);
			pooled.pointerId = pointerId;
			pooled.deltaTime = deltaTime;
			pooled.displayIndex = pointerEvent.displayIndex;
			pooled.isPrimary = pointerEvent.isPrimaryPointer;
			pooled.button = -1;
			bool flag = pointerEvent.eventSource == EventSource.Mouse;
			if (flag)
			{
				pooled.pointerType = PointerType.mouse;
				Debug.Assert(pointerEvent.isPrimaryPointer, "PointerEvent from Mouse source is expected to be a primary pointer.");
				Debug.Assert(pointerId == PointerId.mousePointerId, "PointerEvent from Mouse source is expected to have mouse pointer id.");
				bool flag2 = pointerEvent.button == PointerEvent.Button.Primary;
				if (flag2)
				{
					pooled.button = 0;
				}
				else
				{
					bool flag3 = pointerEvent.button == PointerEvent.Button.PenEraserInTouch;
					if (flag3)
					{
						pooled.button = 1;
					}
					else
					{
						bool flag4 = pointerEvent.button == PointerEvent.Button.PenBarrelButton;
						if (flag4)
						{
							pooled.button = 2;
						}
					}
				}
			}
			else
			{
				bool flag5 = pointerEvent.eventSource == EventSource.Touch;
				if (flag5)
				{
					pooled.pointerType = PointerType.touch;
					Debug.Assert(pooled.pointerId >= PointerId.touchPointerIdBase && pooled.pointerId < PointerId.touchPointerIdBase + PointerId.touchPointerCount, "PointerEvent from Touch source is expected to have touch-based pointer id.");
					bool flag6 = pointerEvent.button == PointerEvent.Button.Primary;
					if (flag6)
					{
						pooled.button = 0;
					}
				}
				else
				{
					bool flag7 = pointerEvent.eventSource == EventSource.Pen;
					if (flag7)
					{
						pooled.pointerType = PointerType.pen;
						Debug.Assert(pooled.pointerId >= PointerId.penPointerIdBase && pooled.pointerId < PointerId.penPointerIdBase + PointerId.penPointerCount, "PointerEvent from Pen source is expected to have pen-based pointer id.");
						bool flag8 = pointerEvent.button == PointerEvent.Button.Primary;
						if (flag8)
						{
							pooled.button = 0;
						}
						else
						{
							bool flag9 = pointerEvent.button == PointerEvent.Button.PenBarrelButton;
							if (flag9)
							{
								pooled.button = 1;
							}
							else
							{
								bool flag10 = pointerEvent.button == PointerEvent.Button.PenEraserInTouch;
								if (flag10)
								{
									pooled.button = 5;
								}
							}
						}
					}
					else
					{
						bool flag11 = pointerEvent.eventSource == EventSource.TrackedDevice;
						if (!flag11)
						{
							throw new ArgumentOutOfRangeException("pointerEvent", "Unsupported EventSource for pointer event");
						}
						pooled.pointerType = PointerType.tracked;
						Debug.Assert(pooled.pointerId >= PointerId.trackedPointerIdBase && pooled.pointerId < PointerId.trackedPointerIdBase + PointerId.trackedPointerCount, "PointerEvent from TrackedDevice source is expected to have tracked-based pointer id.");
						bool flag12 = pointerEvent.button == PointerEvent.Button.Primary;
						if (flag12)
						{
							pooled.button = 0;
						}
					}
				}
			}
			bool flag13 = pointerEvent.type == PointerEvent.Type.ButtonPressed;
			if (flag13)
			{
				Debug.Assert(pooled.button != -1, "PointerEvent of type ButtonPressed is expected to have button != -1.");
				PointerDeviceState.PressButton(pooled.pointerId, pooled.button);
			}
			else
			{
				bool flag14 = pointerEvent.type == PointerEvent.Type.ButtonReleased;
				if (flag14)
				{
					Debug.Assert(pooled.button != -1, "PointerEvent of type ButtonReleased is expected to have button != -1.");
					PointerDeviceState.ReleaseButton(pooled.pointerId, pooled.button);
				}
				else
				{
					bool flag15 = pointerEvent.type != PointerEvent.Type.TouchCanceled;
					if (flag15)
					{
						Debug.Assert(pooled.button == -1, "PointerEvent of type other than ButtonPressed, ButtonReleased, or TouchCanceled is expected to have button set to none.");
					}
				}
			}
			pooled.pressedButtons = PointerDeviceState.GetPressedButtons(pooled.pointerId);
			bool flag16 = pointerEvent.eventSource == EventSource.Pen;
			if (flag16)
			{
				pooled.penStatus = PenStatus.None;
				bool flag17 = (pooled.pressedButtons & 1) != 0;
				if (flag17)
				{
					ref T ptr = ref pooled;
					ptr.penStatus |= PenStatus.Contact;
				}
				bool flag18 = (pooled.pressedButtons & 2) != 0;
				if (flag18)
				{
					ref T ptr = ref pooled;
					ptr.penStatus |= PenStatus.Barrel;
				}
				bool flag19 = (pooled.pressedButtons & 32) != 0;
				if (flag19)
				{
					ref T ptr = ref pooled;
					ptr.penStatus |= PenStatus.Eraser;
				}
				bool isInverted = pointerEvent.isInverted;
				if (isInverted)
				{
					ref T ptr = ref pooled;
					ptr.penStatus |= PenStatus.Inverted;
				}
			}
			pooled.clickCount = pointerEvent.clickCount;
			pooled.pressure = pointerEvent.pressure;
			pooled.altitudeAngle = pointerEvent.altitude;
			pooled.azimuthAngle = pointerEvent.azimuth;
			pooled.twist = pointerEvent.twist;
			pooled.tilt = pointerEvent.tilt;
			EventModifiers eventModifiers = EventModifiers.None;
			bool isShiftPressed = pointerEvent.eventModifiers.isShiftPressed;
			if (isShiftPressed)
			{
				eventModifiers |= EventModifiers.Shift;
			}
			bool isCtrlPressed = pointerEvent.eventModifiers.isCtrlPressed;
			if (isCtrlPressed)
			{
				eventModifiers |= EventModifiers.Control;
			}
			bool isAltPressed = pointerEvent.eventModifiers.isAltPressed;
			if (isAltPressed)
			{
				eventModifiers |= EventModifiers.Alt;
			}
			bool isMetaPressed = pointerEvent.eventModifiers.isMetaPressed;
			if (isMetaPressed)
			{
				eventModifiers |= EventModifiers.Command;
			}
			pooled.modifiers = eventModifiers;
			return pooled;
		}

		internal static T GetPooled(IPointerEvent triggerEvent, Vector2 position, int pointerId)
		{
			bool flag = triggerEvent != null;
			T t;
			if (flag)
			{
				t = PointerEventBase<T>.GetPooled(triggerEvent);
			}
			else
			{
				T pooled = EventBase<T>.GetPooled();
				pooled.position = position;
				pooled.localPosition = position;
				pooled.pointerId = pointerId;
				pooled.pointerType = PointerType.GetPointerType(pointerId);
				t = pooled;
			}
			return t;
		}

		public static T GetPooled(IPointerEvent triggerEvent)
		{
			T pooled = EventBase<T>.GetPooled();
			bool flag = triggerEvent != null;
			if (flag)
			{
				pooled.pointerId = triggerEvent.pointerId;
				pooled.pointerType = triggerEvent.pointerType;
				pooled.isPrimary = triggerEvent.isPrimary;
				pooled.button = triggerEvent.button;
				pooled.pressedButtons = triggerEvent.pressedButtons;
				pooled.position = triggerEvent.position;
				pooled.localPosition = triggerEvent.localPosition;
				pooled.deltaPosition = triggerEvent.deltaPosition;
				pooled.deltaTime = triggerEvent.deltaTime;
				pooled.clickCount = triggerEvent.clickCount;
				pooled.pressure = triggerEvent.pressure;
				pooled.tangentialPressure = triggerEvent.tangentialPressure;
				pooled.altitudeAngle = triggerEvent.altitudeAngle;
				pooled.azimuthAngle = triggerEvent.azimuthAngle;
				pooled.twist = triggerEvent.twist;
				pooled.tilt = triggerEvent.tilt;
				pooled.penStatus = triggerEvent.penStatus;
				pooled.radius = triggerEvent.radius;
				pooled.radiusVariance = triggerEvent.radiusVariance;
				pooled.modifiers = triggerEvent.modifiers;
			}
			return pooled;
		}

		internal virtual IMouseEvent GetPooledCompatibilityMouseEvent()
		{
			return null;
		}

		protected internal override void PreDispatch(IPanel panel)
		{
			base.PreDispatch(panel);
			bool flag = panel.ShouldSendCompatibilityMouseEvents(this);
			if (flag)
			{
				this.compatibilityMouseEvent = this.GetPooledCompatibilityMouseEvent();
			}
			bool recomputeTopElementUnderPointer = this.recomputeTopElementUnderPointer;
			if (recomputeTopElementUnderPointer)
			{
				PointerDeviceState.SavePointerPosition(this.pointerId, this.position, panel, panel.contextType);
				((BaseVisualElementPanel)panel).RecomputeTopElementUnderPointer(this.pointerId, this.position, this);
			}
			EventBase eventBase = (EventBase)this.compatibilityMouseEvent;
			if (eventBase != null)
			{
				eventBase.PreDispatch(panel);
			}
		}

		protected internal override void PostDispatch(IPanel panel)
		{
			for (int i = 0; i < PointerId.maxPointers; i++)
			{
				panel.ProcessPointerCapture(i);
			}
			EventBase eventBase = (EventBase)this.compatibilityMouseEvent;
			if (eventBase != null)
			{
				eventBase.PostDispatch(panel);
			}
			bool recomputeTopElementUnderPointer = this.recomputeTopElementUnderPointer;
			if (recomputeTopElementUnderPointer)
			{
				BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
				if (baseVisualElementPanel != null)
				{
					baseVisualElementPanel.CommitElementUnderPointers();
				}
			}
			base.PostDispatch(panel);
		}

		internal override void Dispatch(BaseVisualElementPanel panel)
		{
			EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, this.pointerId, this.position);
		}

		protected PointerEventBase()
		{
			this.LocalInit();
		}

		private const float k_DefaultButtonPressure = 0.5f;

		private bool m_AltitudeNeedsConversion = true;

		private bool m_AzimuthNeedsConversion = true;

		private float m_AltitudeAngle = 0f;

		private float m_AzimuthAngle = 0f;

		private bool m_TiltNeeded = true;

		private Vector2 m_Tilt = new Vector2(0f, 0f);
	}
}
