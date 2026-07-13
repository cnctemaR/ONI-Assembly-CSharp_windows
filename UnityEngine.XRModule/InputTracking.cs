using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[StaticAccessor("XRInputTrackingFacade::Get()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputTrackingFacade.h")]
	[NativeConditional("ENABLE_VR")]
	[RequiredByNativeCode]
	public static class InputTracking
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<XRNodeState> trackingAcquired;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<XRNodeState> trackingLost;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<XRNodeState> nodeAdded;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<XRNodeState> nodeRemoved;

		[RequiredByNativeCode]
		private static void InvokeTrackingEvent(InputTracking.TrackingStateEventType eventType, XRNode nodeType, long uniqueID, bool tracked)
		{
			XRNodeState xrnodeState = default(XRNodeState);
			xrnodeState.uniqueID = (ulong)uniqueID;
			xrnodeState.nodeType = nodeType;
			xrnodeState.tracked = tracked;
			Action<XRNodeState> action;
			switch (eventType)
			{
			case InputTracking.TrackingStateEventType.NodeAdded:
				action = InputTracking.nodeAdded;
				break;
			case InputTracking.TrackingStateEventType.NodeRemoved:
				action = InputTracking.nodeRemoved;
				break;
			case InputTracking.TrackingStateEventType.TrackingAcquired:
				action = InputTracking.trackingAcquired;
				break;
			case InputTracking.TrackingStateEventType.TrackingLost:
				action = InputTracking.trackingLost;
				break;
			default:
				throw new ArgumentException("TrackingEventHandler - Invalid EventType: " + eventType.ToString());
			}
			bool flag = action != null;
			if (flag)
			{
				action(xrnodeState);
			}
		}

		[NativeConditional("ENABLE_VR", "Vector3f::zero")]
		[Obsolete("This API is obsolete, and should no longer be used. Please use InputDevice.TryGetFeatureValue with the CommonUsages.devicePosition usage instead.")]
		public static Vector3 GetLocalPosition(XRNode node)
		{
			Vector3 vector;
			InputTracking.GetLocalPosition_Injected(node, out vector);
			return vector;
		}

		[NativeConditional("ENABLE_VR", "Quaternionf::identity()")]
		[Obsolete("This API is obsolete, and should no longer be used. Please use InputDevice.TryGetFeatureValue with the CommonUsages.deviceRotation usage instead.")]
		public static Quaternion GetLocalRotation(XRNode node)
		{
			Quaternion quaternion;
			InputTracking.GetLocalRotation_Injected(node, out quaternion);
			return quaternion;
		}

		[Obsolete("This API is obsolete, and should no longer be used. Please use XRInputSubsystem.TryRecenter() instead.")]
		[NativeConditional("ENABLE_VR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Recenter();

		[NativeConditional("ENABLE_VR")]
		[Obsolete("This API is obsolete, and should no longer be used. Please use InputDevice.name with the device associated with that tracking data instead.")]
		public static string GetNodeName(ulong uniqueId)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				InputTracking.GetNodeName_Injected(uniqueId, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static void GetNodeStates(List<XRNodeState> nodeStates)
		{
			bool flag = nodeStates == null;
			if (flag)
			{
				throw new ArgumentNullException("nodeStates");
			}
			nodeStates.Clear();
			InputTracking.GetNodeStates_Internal(nodeStates);
		}

		[NativeConditional("ENABLE_VR")]
		private unsafe static void GetNodeStates_Internal([NotNull] List<XRNodeState> nodeStates)
		{
			if (nodeStates == null)
			{
				ThrowHelper.ThrowArgumentNullException(nodeStates, "nodeStates");
			}
			try
			{
				fixed (XRNodeState[] array = NoAllocHelpers.ExtractArrayFromList<XRNodeState>(nodeStates))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, nodeStates.Count);
					InputTracking.GetNodeStates_Internal_Injected(ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<XRNodeState>(nodeStates);
			}
		}

		[NativeConditional("ENABLE_VR")]
		[Obsolete("This API is obsolete, and should no longer be used. Please use the TrackedPoseDriver in the Legacy Input Helpers package for controlling a camera in XR.")]
		public static extern bool disablePositionalTracking
		{
			[NativeName("GetPositionalTrackingDisabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetPositionalTrackingDisabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("XRInputTracking::Get()", StaticAccessorType.Dot)]
		[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputTracking.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ulong GetDeviceIdAtXRNode(XRNode node);

		[StaticAccessor("XRInputTracking::Get()", StaticAccessorType.Dot)]
		[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputTracking.h")]
		internal unsafe static void GetDeviceIdsAtXRNode_Internal(XRNode node, [NotNull] List<ulong> deviceIds)
		{
			if (deviceIds == null)
			{
				ThrowHelper.ThrowArgumentNullException(deviceIds, "deviceIds");
			}
			try
			{
				fixed (ulong[] array = NoAllocHelpers.ExtractArrayFromList<ulong>(deviceIds))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, deviceIds.Count);
					InputTracking.GetDeviceIdsAtXRNode_Internal_Injected(node, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ulong>(deviceIds);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalPosition_Injected(XRNode node, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalRotation_Injected(XRNode node, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeName_Injected(ulong uniqueId, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeStates_Internal_Injected(ref BlittableListWrapper nodeStates);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDeviceIdsAtXRNode_Internal_Injected(XRNode node, ref BlittableListWrapper deviceIds);

		private enum TrackingStateEventType
		{
			NodeAdded,
			NodeRemoved,
			TrackingAcquired,
			TrackingLost
		}
	}
}
