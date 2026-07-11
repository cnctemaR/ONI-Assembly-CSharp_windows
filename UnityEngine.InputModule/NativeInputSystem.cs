using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngineInternal.Input
{
	[NativeConditional("ENABLE_NEW_INPUT_SYSTEM")]
	[NativeHeader("Modules/Input/Private/InputInternal.h")]
	[NativeHeader("Modules/Input/Private/InputModuleBindings.h")]
	public class NativeInputSystem
	{
		public static Action<int, string> onDeviceDiscovered
		{
			get
			{
				return NativeInputSystem.s_OnDeviceDiscoveredCallback;
			}
			set
			{
				NativeInputSystem.s_OnDeviceDiscoveredCallback = value;
				NativeInputSystem.hasDeviceDiscoveredCallback = NativeInputSystem.s_OnDeviceDiscoveredCallback != null;
			}
		}

		[RequiredByNativeCode]
		internal static void NotifyBeforeUpdate(NativeInputUpdateType updateType)
		{
			Action<NativeInputUpdateType> action = NativeInputSystem.onBeforeUpdate;
			if (action != null)
			{
				action(updateType);
			}
		}

		[RequiredByNativeCode]
		internal static void NotifyUpdate(NativeInputUpdateType updateType, int eventCount, IntPtr eventData)
		{
			Action<NativeInputUpdateType, int, IntPtr> action = NativeInputSystem.onUpdate;
			if (action != null)
			{
				action(updateType, eventCount, eventData);
			}
		}

		[RequiredByNativeCode]
		internal static void NotifyDeviceDiscovered(int deviceId, string deviceDescriptor)
		{
			Action<int, string> action = NativeInputSystem.s_OnDeviceDiscoveredCallback;
			if (action != null)
			{
				action(deviceId, deviceDescriptor);
			}
		}

		internal static extern bool hasDeviceDiscoveredCallback
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		} = false;

		public static extern double currentTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern double currentTimeOffsetToRealtimeSinceStartup
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("AllocateInputDeviceId")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int AllocateDeviceId();

		public static void QueueInputEvent<TInputEvent>(ref TInputEvent inputEvent) where TInputEvent : struct
		{
			NativeInputSystem.QueueInputEvent((IntPtr)UnsafeUtility.AddressOf<TInputEvent>(ref inputEvent));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void QueueInputEvent(IntPtr inputEvent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long IOCTL(int deviceId, int code, IntPtr data, int sizeInBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetPollingFrequency(float hertz);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Update(NativeInputUpdateType updateType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetUpdateMask(NativeInputUpdateType mask);

		public static Action<NativeInputUpdateType, int, IntPtr> onUpdate;

		public static Action<NativeInputUpdateType> onBeforeUpdate;

		private static Action<int, string> s_OnDeviceDiscoveredCallback;
	}
}
