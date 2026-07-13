using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Input/InputBindings.h")]
	public class Input
	{
		public static float GetAxis(string axisName)
		{
			return InputUnsafeUtility.GetAxis(axisName);
		}

		public static float GetAxisRaw(string axisName)
		{
			return InputUnsafeUtility.GetAxisRaw(axisName);
		}

		public static bool GetButton(string buttonName)
		{
			return InputUnsafeUtility.GetButton(buttonName);
		}

		public static bool GetButtonDown(string buttonName)
		{
			return InputUnsafeUtility.GetButtonDown(buttonName);
		}

		public static bool GetButtonUp(string buttonName)
		{
			return InputUnsafeUtility.GetButtonUp(buttonName);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeyInt(KeyCode key);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeyUpInt(KeyCode key);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeyDownInt(KeyCode key);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetMouseButton(int button);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetMouseButtonDown(int button);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetMouseButtonUp(int button);

		[FreeFunction("ResetInput")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetInputAxes();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetJoystickNames();

		[NativeThrows]
		public static Touch GetTouch(int index)
		{
			Touch touch;
			Input.GetTouch_Injected(index, out touch);
			return touch;
		}

		[NativeThrows]
		public static PenData GetPenEvent(int index)
		{
			PenData penData;
			Input.GetPenEvent_Injected(index, out penData);
			return penData;
		}

		[NativeThrows]
		public static PenData GetLastPenContactEvent()
		{
			PenData penData;
			Input.GetLastPenContactEvent_Injected(out penData);
			return penData;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetPenEvents();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearLastPenContactEvent();

		[NativeThrows]
		public static AccelerationEvent GetAccelerationEvent(int index)
		{
			AccelerationEvent accelerationEvent;
			Input.GetAccelerationEvent_Injected(index, out accelerationEvent);
			return accelerationEvent;
		}

		public static bool GetKey(KeyCode key)
		{
			return Input.GetKeyInt(key);
		}

		public static bool GetKey(string name)
		{
			return InputUnsafeUtility.GetKeyString(name);
		}

		public static bool GetKeyUp(KeyCode key)
		{
			return Input.GetKeyUpInt(key);
		}

		public static bool GetKeyUp(string name)
		{
			return InputUnsafeUtility.GetKeyUpString(name);
		}

		public static bool GetKeyDown(KeyCode key)
		{
			return Input.GetKeyDownInt(key);
		}

		public static bool GetKeyDown(string name)
		{
			return InputUnsafeUtility.GetKeyDownString(name);
		}

		[Conditional("UNITY_EDITOR")]
		internal static void SimulateTouch(Touch touch)
		{
		}

		[FreeFunction("SimulateTouch")]
		[NativeConditional("UNITY_EDITOR")]
		[Conditional("UNITY_EDITOR")]
		private static void SimulateTouchInternal(Touch touch, long timestamp)
		{
			Input.SimulateTouchInternal_Injected(ref touch, timestamp);
		}

		public static extern bool simulateMouseWithTouches
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeThrows]
		public static extern bool anyKey
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeThrows]
		public static extern bool anyKeyDown
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeThrows]
		public static string inputString
		{
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Input.get_inputString_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[NativeThrows]
		public static Vector3 mousePosition
		{
			get
			{
				Vector3 vector;
				Input.get_mousePosition_Injected(out vector);
				return vector;
			}
		}

		[NativeThrows]
		public static Vector3 mousePositionDelta
		{
			get
			{
				Vector3 vector;
				Input.get_mousePositionDelta_Injected(out vector);
				return vector;
			}
		}

		[NativeThrows]
		public static Vector2 mouseScrollDelta
		{
			get
			{
				Vector2 vector;
				Input.get_mouseScrollDelta_Injected(out vector);
				return vector;
			}
		}

		public static extern IMECompositionMode imeCompositionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static string compositionString
		{
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					Input.get_compositionString_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public static extern bool imeIsSelected
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Vector2 compositionCursorPos
		{
			get
			{
				Vector2 vector;
				Input.get_compositionCursorPos_Injected(out vector);
				return vector;
			}
			set
			{
				Input.set_compositionCursorPos_Injected(ref value);
			}
		}

		[Obsolete("eatKeyPressOnTextFieldFocus property is deprecated, and only provided to support legacy behavior.")]
		public static extern bool eatKeyPressOnTextFieldFocus
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static bool simulateTouchEnabled { get; set; }

		[FreeFunction("GetMousePresent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetMousePresentInternal();

		[FreeFunction("IsTouchSupported")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetTouchSupportedInternal();

		public static bool mousePresent
		{
			get
			{
				return !Input.simulateTouchEnabled && Input.GetMousePresentInternal();
			}
		}

		public static bool touchSupported
		{
			get
			{
				return Input.simulateTouchEnabled || Input.GetTouchSupportedInternal();
			}
		}

		public static extern int penEventCount
		{
			[FreeFunction("GetPenEventCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern int touchCount
		{
			[FreeFunction("GetTouchCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool touchPressureSupported
		{
			[FreeFunction("IsTouchPressureSupported")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool stylusTouchSupported
		{
			[FreeFunction("IsStylusTouchSupported")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool multiTouchEnabled
		{
			[FreeFunction("IsMultiTouchEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetMultiTouchEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("isGyroAvailable property is deprecated. Please use SystemInfo.supportsGyroscope instead.")]
		public static extern bool isGyroAvailable
		{
			[FreeFunction("IsGyroAvailable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern DeviceOrientation deviceOrientation
		{
			[FreeFunction("GetDeviceOrientation")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Vector3 acceleration
		{
			[FreeFunction("GetAcceleration")]
			get
			{
				Vector3 vector;
				Input.get_acceleration_Injected(out vector);
				return vector;
			}
		}

		public static extern bool compensateSensors
		{
			[FreeFunction("IsCompensatingSensors")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetCompensatingSensors")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int accelerationEventCount
		{
			[FreeFunction("GetAccelerationCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool backButtonLeavesApp
		{
			[FreeFunction("GetBackButtonLeavesApp")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("SetBackButtonLeavesApp")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static LocationService location
		{
			get
			{
				bool flag = Input.locationServiceInstance == null;
				if (flag)
				{
					Input.locationServiceInstance = new LocationService();
				}
				return Input.locationServiceInstance;
			}
		}

		public static Compass compass
		{
			get
			{
				bool flag = Input.compassInstance == null;
				if (flag)
				{
					Input.compassInstance = new Compass();
				}
				return Input.compassInstance;
			}
		}

		[FreeFunction("GetGyro")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetGyroInternal();

		public static Gyroscope gyro
		{
			get
			{
				bool flag = Input.s_MainGyro == null;
				if (flag)
				{
					Input.s_MainGyro = new Gyroscope(Input.GetGyroInternal());
				}
				return Input.s_MainGyro;
			}
		}

		public static Touch[] touches
		{
			get
			{
				int touchCount = Input.touchCount;
				Touch[] array = new Touch[touchCount];
				for (int i = 0; i < touchCount; i++)
				{
					array[i] = Input.GetTouch(i);
				}
				return array;
			}
		}

		public static AccelerationEvent[] accelerationEvents
		{
			get
			{
				int accelerationEventCount = Input.accelerationEventCount;
				AccelerationEvent[] array = new AccelerationEvent[accelerationEventCount];
				for (int i = 0; i < accelerationEventCount; i++)
				{
					array[i] = Input.GetAccelerationEvent(i);
				}
				return array;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CheckDisabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTouch_Injected(int index, out Touch ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPenEvent_Injected(int index, out PenData ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLastPenContactEvent_Injected(out PenData ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAccelerationEvent_Injected(int index, out AccelerationEvent ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SimulateTouchInternal_Injected([In] ref Touch touch, long timestamp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_inputString_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_mousePosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_mousePositionDelta_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_mouseScrollDelta_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_compositionString_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_compositionCursorPos_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_compositionCursorPos_Injected([In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_acceleration_Injected(out Vector3 ret);

		private static LocationService locationServiceInstance;

		private static Compass compassInstance;

		private static Gyroscope s_MainGyro;
	}
}
