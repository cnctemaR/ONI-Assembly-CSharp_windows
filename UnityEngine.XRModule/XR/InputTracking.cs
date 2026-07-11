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
	[RequiredByNativeCode]
	[NativeConditional("ENABLE_VR")]
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
				throw new ArgumentException("TrackingEventHandler - Invalid EventType: " + eventType);
			}
			if (action != null)
			{
				action(xrnodeState);
			}
		}

		[NativeConditional("ENABLE_VR", "Vector3f::zero")]
		public static Vector3 GetLocalPosition(XRNode node)
		{
			Vector3 vector;
			InputTracking.GetLocalPosition_Injected(node, out vector);
			return vector;
		}

		[NativeConditional("ENABLE_VR", "Quaternionf::identity()")]
		public static Quaternion GetLocalRotation(XRNode node)
		{
			Quaternion quaternion;
			InputTracking.GetLocalRotation_Injected(node, out quaternion);
			return quaternion;
		}

		[NativeConditional("ENABLE_VR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Recenter();

		[NativeConditional("ENABLE_VR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetNodeName(ulong uniqueId);

		public static void GetNodeStates(List<XRNodeState> nodeStates)
		{
			if (nodeStates == null)
			{
				throw new ArgumentNullException("nodeStates");
			}
			nodeStates.Clear();
			InputTracking.GetNodeStates_Internal(nodeStates);
		}

		[NativeConditional("ENABLE_VR && !ENABLE_DOTNET")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeStates_Internal(List<XRNodeState> nodeStates);

		[NativeConditional("ENABLE_VR && ENABLE_DOTNET")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern XRNodeState[] GetNodeStates_Internal_WinRT();

		[NativeConditional("ENABLE_VR")]
		public static extern bool disablePositionalTracking
		{
			[NativeName("GetPositionalTrackingDisabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetPositionalTrackingDisabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SendHapticImpulse(ulong deviceId, uint channel, float amplitude, float duration);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SendHapticBuffer(ulong deviceId, uint channel, byte[] buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool TryGetHapticCapabilities(ulong deviceId, out HapticCapabilities capabilities);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StopHaptics(ulong deviceId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsDeviceValid(ulong deviceId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ulong GetDeviceIdAtXRNode(XRNode node);

		// Note: this type is marked as 'beforefieldinit'.
		static InputTracking()
		{
			InputTracking.trackingAcquired = null;
			InputTracking.trackingLost = null;
			InputTracking.nodeAdded = null;
			InputTracking.nodeRemoved = null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalPosition_Injected(XRNode node, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalRotation_Injected(XRNode node, out Quaternion ret);

		private enum TrackingStateEventType
		{
			NodeAdded,
			NodeRemoved,
			TrackingAcquired,
			TrackingLost
		}
	}
}
