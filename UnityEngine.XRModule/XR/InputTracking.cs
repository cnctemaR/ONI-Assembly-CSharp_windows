using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>A collection of methods and properties for interacting with the XR tracking system.</para>
	/// </summary>
	[RequiredByNativeCode]
	public static class InputTracking
	{
		/// <summary>
		///   <para>Gets the position of a specific node.</para>
		/// </summary>
		/// <param name="node">Specifies which node's position should be returned.</param>
		/// <returns>
		///   <para>The position of the node in its local tracking space.</para>
		/// </returns>
		public static Vector3 GetLocalPosition(XRNode node)
		{
			Vector3 vector;
			InputTracking.INTERNAL_CALL_GetLocalPosition(node, out vector);
			return vector;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalPosition(XRNode node, out Vector3 value);

		/// <summary>
		///   <para>Gets the rotation of a specific node.</para>
		/// </summary>
		/// <param name="node">Specifies which node's rotation should be returned.</param>
		/// <returns>
		///   <para>The rotation of the node in its local tracking space.</para>
		/// </returns>
		public static Quaternion GetLocalRotation(XRNode node)
		{
			Quaternion quaternion;
			InputTracking.INTERNAL_CALL_GetLocalRotation(node, out quaternion);
			return quaternion;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalRotation(XRNode node, out Quaternion value);

		/// <summary>
		///   <para>Center tracking to the current position and orientation of the HMD.</para>
		/// </summary>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Recenter();

		/// <summary>
		///   <para>Accepts the unique identifier for a tracked node and returns a friendly name for it.</para>
		/// </summary>
		/// <param name="uniqueID">The unique identifier for the Node index.</param>
		/// <returns>
		///   <para>The name of the tracked node if the given 64-bit identifier maps to a currently tracked node. Empty string otherwise.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetNodeName(ulong uniqueID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeStatesInternal(object nodeStates);

		public static void GetNodeStates(List<XRNodeState> nodeStates)
		{
			if (nodeStates == null)
			{
				throw new ArgumentNullException("nodeStates");
			}
			nodeStates.Clear();
			InputTracking.GetNodeStatesInternal(nodeStates);
		}

		/// <summary>
		///   <para>Disables positional tracking in XR. This takes effect the next time the head pose is sampled.  If set to true the camera only tracks headset rotation state.</para>
		/// </summary>
		public static extern bool disablePositionalTracking
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		// Note: this type is marked as 'beforefieldinit'.
		static InputTracking()
		{
			InputTracking.trackingAcquired = null;
			InputTracking.trackingLost = null;
			InputTracking.nodeAdded = null;
			InputTracking.nodeRemoved = null;
		}

		private enum TrackingStateEventType
		{
			NodeAdded,
			NodeRemoved,
			TrackingAcquired,
			TrackingLost
		}
	}
}
