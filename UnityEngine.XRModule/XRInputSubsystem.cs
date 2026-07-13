using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeType(Header = "Modules/XR/Subsystems/Input/XRInputSubsystem.h")]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	public class XRInputSubsystem : IntegratedSubsystem<XRInputSubsystemDescriptor>
	{
		internal uint GetIndex()
		{
			IntPtr intPtr = XRInputSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRInputSubsystem.GetIndex_Injected(intPtr);
		}

		public bool TryRecenter()
		{
			IntPtr intPtr = XRInputSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRInputSubsystem.TryRecenter_Injected(intPtr);
		}

		public bool TryGetInputDevices(List<InputDevice> devices)
		{
			bool flag = devices == null;
			if (flag)
			{
				throw new ArgumentNullException("devices");
			}
			devices.Clear();
			bool flag2 = this.m_DeviceIdsCache == null;
			if (flag2)
			{
				this.m_DeviceIdsCache = new List<ulong>();
			}
			this.m_DeviceIdsCache.Clear();
			this.TryGetDeviceIds_AsList(this.m_DeviceIdsCache);
			for (int i = 0; i < this.m_DeviceIdsCache.Count; i++)
			{
				devices.Add(new InputDevice(this.m_DeviceIdsCache[i]));
			}
			return true;
		}

		public bool TrySetTrackingOriginMode(TrackingOriginModeFlags origin)
		{
			IntPtr intPtr = XRInputSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRInputSubsystem.TrySetTrackingOriginMode_Injected(intPtr, origin);
		}

		public TrackingOriginModeFlags GetTrackingOriginMode()
		{
			IntPtr intPtr = XRInputSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRInputSubsystem.GetTrackingOriginMode_Injected(intPtr);
		}

		public TrackingOriginModeFlags GetSupportedTrackingOriginModes()
		{
			IntPtr intPtr = XRInputSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRInputSubsystem.GetSupportedTrackingOriginModes_Injected(intPtr);
		}

		public bool TryGetBoundaryPoints(List<Vector3> boundaryPoints)
		{
			bool flag = boundaryPoints == null;
			if (flag)
			{
				throw new ArgumentNullException("boundaryPoints");
			}
			return this.TryGetBoundaryPoints_AsList(boundaryPoints);
		}

		private unsafe bool TryGetBoundaryPoints_AsList(List<Vector3> boundaryPoints)
		{
			bool flag;
			try
			{
				IntPtr intPtr = XRInputSubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (boundaryPoints != null)
				{
					fixed (Vector3[] array = NoAllocHelpers.ExtractArrayFromList<Vector3>(boundaryPoints))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, boundaryPoints.Count);
					}
				}
				flag = XRInputSubsystem.TryGetBoundaryPoints_AsList_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<Vector3>(boundaryPoints);
			}
			return flag;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<XRInputSubsystem> trackingOriginUpdated;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<XRInputSubsystem> boundaryChanged;

		[RequiredByNativeCode(GenerateProxy = true)]
		private static void InvokeTrackingOriginUpdatedEvent(IntPtr internalPtr)
		{
			IntegratedSubsystem integratedSubsystemByPtr = SubsystemManager.GetIntegratedSubsystemByPtr(internalPtr);
			XRInputSubsystem xrinputSubsystem = integratedSubsystemByPtr as XRInputSubsystem;
			bool flag = xrinputSubsystem != null && xrinputSubsystem.trackingOriginUpdated != null;
			if (flag)
			{
				xrinputSubsystem.trackingOriginUpdated(xrinputSubsystem);
			}
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		private static void InvokeBoundaryChangedEvent(IntPtr internalPtr)
		{
			IntegratedSubsystem integratedSubsystemByPtr = SubsystemManager.GetIntegratedSubsystemByPtr(internalPtr);
			XRInputSubsystem xrinputSubsystem = integratedSubsystemByPtr as XRInputSubsystem;
			bool flag = xrinputSubsystem != null && xrinputSubsystem.boundaryChanged != null;
			if (flag)
			{
				xrinputSubsystem.boundaryChanged(xrinputSubsystem);
			}
		}

		internal unsafe void TryGetDeviceIds_AsList(List<ulong> deviceIds)
		{
			try
			{
				IntPtr intPtr = XRInputSubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableListWrapper blittableListWrapper;
				if (deviceIds != null)
				{
					fixed (ulong[] array = NoAllocHelpers.ExtractArrayFromList<ulong>(deviceIds))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, deviceIds.Count);
					}
				}
				XRInputSubsystem.TryGetDeviceIds_AsList_Injected(intPtr, ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ulong>(deviceIds);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryRecenter_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetTrackingOriginMode_Injected(IntPtr _unity_self, TrackingOriginModeFlags origin);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TrackingOriginModeFlags GetTrackingOriginMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TrackingOriginModeFlags GetSupportedTrackingOriginModes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetBoundaryPoints_AsList_Injected(IntPtr _unity_self, ref BlittableListWrapper boundaryPoints);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TryGetDeviceIds_AsList_Injected(IntPtr _unity_self, ref BlittableListWrapper deviceIds);

		private List<ulong> m_DeviceIdsCache;

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(XRInputSubsystem xrInputSubsystem)
			{
				return xrInputSubsystem.m_Ptr;
			}
		}
	}
}
