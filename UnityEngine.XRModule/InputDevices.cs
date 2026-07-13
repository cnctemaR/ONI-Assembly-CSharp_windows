using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputDevices.h")]
	[StaticAccessor("XRInputDevices::Get()", StaticAccessorType.Dot)]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_VR")]
	[StructLayout(LayoutKind.Sequential)]
	public class InputDevices
	{
		public static InputDevice GetDeviceAtXRNode(XRNode node)
		{
			ulong deviceIdAtXRNode = InputTracking.GetDeviceIdAtXRNode(node);
			return new InputDevice(deviceIdAtXRNode);
		}

		public static void GetDevicesAtXRNode(XRNode node, List<InputDevice> inputDevices)
		{
			bool flag = inputDevices == null;
			if (flag)
			{
				throw new ArgumentNullException("inputDevices");
			}
			List<ulong> list = new List<ulong>();
			InputTracking.GetDeviceIdsAtXRNode_Internal(node, list);
			inputDevices.Clear();
			foreach (ulong num in list)
			{
				InputDevice inputDevice = new InputDevice(num);
				bool isValid = inputDevice.isValid;
				if (isValid)
				{
					inputDevices.Add(inputDevice);
				}
			}
		}

		public static void GetDevices(List<InputDevice> inputDevices)
		{
			bool flag = inputDevices == null;
			if (flag)
			{
				throw new ArgumentNullException("inputDevices");
			}
			inputDevices.Clear();
			InputDevices.GetDevices_Internal(inputDevices);
		}

		[Obsolete("This API has been marked as deprecated and will be removed in future versions. Please use InputDevices.GetDevicesWithCharacteristics instead.")]
		public static void GetDevicesWithRole(InputDeviceRole role, List<InputDevice> inputDevices)
		{
			bool flag = inputDevices == null;
			if (flag)
			{
				throw new ArgumentNullException("inputDevices");
			}
			bool flag2 = InputDevices.s_InputDeviceList == null;
			if (flag2)
			{
				InputDevices.s_InputDeviceList = new List<InputDevice>();
			}
			InputDevices.GetDevices_Internal(InputDevices.s_InputDeviceList);
			inputDevices.Clear();
			foreach (InputDevice inputDevice in InputDevices.s_InputDeviceList)
			{
				bool flag3 = inputDevice.role == role;
				if (flag3)
				{
					inputDevices.Add(inputDevice);
				}
			}
		}

		public static void GetDevicesWithCharacteristics(InputDeviceCharacteristics desiredCharacteristics, List<InputDevice> inputDevices)
		{
			bool flag = inputDevices == null;
			if (flag)
			{
				throw new ArgumentNullException("inputDevices");
			}
			bool flag2 = InputDevices.s_InputDeviceList == null;
			if (flag2)
			{
				InputDevices.s_InputDeviceList = new List<InputDevice>();
			}
			InputDevices.GetDevices_Internal(InputDevices.s_InputDeviceList);
			inputDevices.Clear();
			foreach (InputDevice inputDevice in InputDevices.s_InputDeviceList)
			{
				bool flag3 = (inputDevice.characteristics & desiredCharacteristics) == desiredCharacteristics;
				if (flag3)
				{
					inputDevices.Add(inputDevice);
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<InputDevice> deviceConnected;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<InputDevice> deviceDisconnected;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<InputDevice> deviceConfigChanged;

		[RequiredByNativeCode]
		private static void InvokeConnectionEvent(ulong deviceId, ConnectionChangeType change)
		{
			switch (change)
			{
			case ConnectionChangeType.Connected:
			{
				bool flag = InputDevices.deviceConnected != null;
				if (flag)
				{
					InputDevices.deviceConnected(new InputDevice(deviceId));
				}
				break;
			}
			case ConnectionChangeType.Disconnected:
			{
				bool flag2 = InputDevices.deviceDisconnected != null;
				if (flag2)
				{
					InputDevices.deviceDisconnected(new InputDevice(deviceId));
				}
				break;
			}
			case ConnectionChangeType.ConfigChange:
			{
				bool flag3 = InputDevices.deviceConfigChanged != null;
				if (flag3)
				{
					InputDevices.deviceConfigChanged(new InputDevice(deviceId));
				}
				break;
			}
			}
		}

		private unsafe static void GetDevices_Internal([NotNull] List<InputDevice> inputDevices)
		{
			if (inputDevices == null)
			{
				ThrowHelper.ThrowArgumentNullException(inputDevices, "inputDevices");
			}
			try
			{
				fixed (InputDevice[] array = NoAllocHelpers.ExtractArrayFromList<InputDevice>(inputDevices))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, inputDevices.Count);
					InputDevices.GetDevices_Internal_Injected(ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<InputDevice>(inputDevices);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SendHapticImpulse(ulong deviceId, uint channel, float amplitude, float duration);

		internal unsafe static bool SendHapticBuffer(ulong deviceId, uint channel, [NotNull] byte[] buffer)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			Span<byte> span = new Span<byte>(buffer);
			bool flag;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				flag = InputDevices.SendHapticBuffer_Injected(deviceId, channel, ref managedSpanWrapper);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool TryGetHapticCapabilities(ulong deviceId, out HapticCapabilities capabilities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopHaptics(ulong deviceId);

		internal static bool TryGetFeatureUsages(ulong deviceId, [NotNull] List<InputFeatureUsage> featureUsages)
		{
			if (featureUsages == null)
			{
				ThrowHelper.ThrowArgumentNullException(featureUsages, "featureUsages");
			}
			return InputDevices.TryGetFeatureUsages_Injected(deviceId, featureUsages);
		}

		internal unsafe static bool TryGetFeatureValue_bool(ulong deviceId, string usage, out bool value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_bool_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_UInt32(ulong deviceId, string usage, out uint value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_UInt32_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_float(ulong deviceId, string usage, out float value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_float_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_Vector2f(ulong deviceId, string usage, out Vector2 value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_Vector2f_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_Vector3f(ulong deviceId, string usage, out Vector3 value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_Vector3f_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_Quaternionf(ulong deviceId, string usage, out Quaternion value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_Quaternionf_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_Custom(ulong deviceId, string usage, [Out] byte[] value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (value != null)
				{
					fixed (byte[] array = value)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				flag = InputDevices.TryGetFeatureValue_Custom_Injected(deviceId, ref managedSpanWrapper, out blittableArrayWrapper);
			}
			finally
			{
				char* ptr = null;
				byte[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<byte>(ref array);
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValueAtTime_bool(ulong deviceId, string usage, long time, out bool value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValueAtTime_bool_Injected(deviceId, ref managedSpanWrapper, time, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValueAtTime_UInt32(ulong deviceId, string usage, long time, out uint value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValueAtTime_UInt32_Injected(deviceId, ref managedSpanWrapper, time, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValueAtTime_float(ulong deviceId, string usage, long time, out float value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValueAtTime_float_Injected(deviceId, ref managedSpanWrapper, time, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValueAtTime_Vector2f(ulong deviceId, string usage, long time, out Vector2 value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValueAtTime_Vector2f_Injected(deviceId, ref managedSpanWrapper, time, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValueAtTime_Vector3f(ulong deviceId, string usage, long time, out Vector3 value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValueAtTime_Vector3f_Injected(deviceId, ref managedSpanWrapper, time, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValueAtTime_Quaternionf(ulong deviceId, string usage, long time, out Quaternion value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValueAtTime_Quaternionf_Injected(deviceId, ref managedSpanWrapper, time, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_XRHand(ulong deviceId, string usage, out Hand value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_XRHand_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_XRBone(ulong deviceId, string usage, out Bone value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_XRBone_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal unsafe static bool TryGetFeatureValue_XREyes(ulong deviceId, string usage, out Eyes value)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(usage, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = usage.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = InputDevices.TryGetFeatureValue_XREyes_Injected(deviceId, ref managedSpanWrapper, out value);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsDeviceValid(ulong deviceId);

		internal static string GetDeviceName(ulong deviceId)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				InputDevices.GetDeviceName_Injected(deviceId, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		internal static string GetDeviceManufacturer(ulong deviceId)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				InputDevices.GetDeviceManufacturer_Injected(deviceId, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		internal static string GetDeviceSerialNumber(ulong deviceId)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				InputDevices.GetDeviceSerialNumber_Injected(deviceId, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern InputDeviceCharacteristics GetDeviceCharacteristics(ulong deviceId);

		internal static InputDeviceRole GetDeviceRole(ulong deviceId)
		{
			InputDeviceCharacteristics deviceCharacteristics = InputDevices.GetDeviceCharacteristics(deviceId);
			bool flag = (deviceCharacteristics & (InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice)) == (InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice);
			InputDeviceRole inputDeviceRole;
			if (flag)
			{
				inputDeviceRole = InputDeviceRole.Generic;
			}
			else
			{
				bool flag2 = (deviceCharacteristics & (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Left)) == (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Left);
				if (flag2)
				{
					inputDeviceRole = InputDeviceRole.LeftHanded;
				}
				else
				{
					bool flag3 = (deviceCharacteristics & (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Right)) == (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Right);
					if (flag3)
					{
						inputDeviceRole = InputDeviceRole.RightHanded;
					}
					else
					{
						bool flag4 = (deviceCharacteristics & InputDeviceCharacteristics.Controller) == InputDeviceCharacteristics.Controller;
						if (flag4)
						{
							inputDeviceRole = InputDeviceRole.GameController;
						}
						else
						{
							bool flag5 = (deviceCharacteristics & (InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.TrackingReference)) == (InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.TrackingReference);
							if (flag5)
							{
								inputDeviceRole = InputDeviceRole.TrackingReference;
							}
							else
							{
								bool flag6 = (deviceCharacteristics & InputDeviceCharacteristics.TrackedDevice) == InputDeviceCharacteristics.TrackedDevice;
								if (flag6)
								{
									inputDeviceRole = InputDeviceRole.HardwareTracker;
								}
								else
								{
									inputDeviceRole = InputDeviceRole.Unknown;
								}
							}
						}
					}
				}
			}
			return inputDeviceRole;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDevices_Internal_Injected(ref BlittableListWrapper inputDevices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendHapticBuffer_Injected(ulong deviceId, uint channel, ref ManagedSpanWrapper buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureUsages_Injected(ulong deviceId, List<InputFeatureUsage> featureUsages);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_bool_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_UInt32_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_float_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_Vector2f_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_Vector3f_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_Quaternionf_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_Custom_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out BlittableArrayWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValueAtTime_bool_Injected(ulong deviceId, ref ManagedSpanWrapper usage, long time, out bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValueAtTime_UInt32_Injected(ulong deviceId, ref ManagedSpanWrapper usage, long time, out uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValueAtTime_float_Injected(ulong deviceId, ref ManagedSpanWrapper usage, long time, out float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValueAtTime_Vector2f_Injected(ulong deviceId, ref ManagedSpanWrapper usage, long time, out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValueAtTime_Vector3f_Injected(ulong deviceId, ref ManagedSpanWrapper usage, long time, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValueAtTime_Quaternionf_Injected(ulong deviceId, ref ManagedSpanWrapper usage, long time, out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_XRHand_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out Hand value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_XRBone_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out Bone value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFeatureValue_XREyes_Injected(ulong deviceId, ref ManagedSpanWrapper usage, out Eyes value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDeviceName_Injected(ulong deviceId, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDeviceManufacturer_Injected(ulong deviceId, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDeviceSerialNumber_Injected(ulong deviceId, out ManagedSpanWrapper ret);

		private static List<InputDevice> s_InputDeviceList;
	}
}
