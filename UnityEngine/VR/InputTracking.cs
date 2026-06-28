using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR
{
	public static class InputTracking
	{
		public static Vector3 GetLocalPosition(VRNode node)
		{
			Vector3 vector;
			InputTracking.INTERNAL_CALL_GetLocalPosition(node, out vector);
			return vector;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);

		public static Quaternion GetLocalRotation(VRNode node)
		{
			Quaternion quaternion;
			InputTracking.INTERNAL_CALL_GetLocalRotation(node, out quaternion);
			return quaternion;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Recenter();

		public static extern bool disablePositionalTracking
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
