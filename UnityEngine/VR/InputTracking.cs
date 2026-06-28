using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR
{
	public sealed class InputTracking
	{
		public static Vector3 GetLocalPosition(VRNode node)
		{
			Vector3 vector;
			InputTracking.INTERNAL_CALL_GetLocalPosition(node, out vector);
			return vector;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);

		public static Quaternion GetLocalRotation(VRNode node)
		{
			Quaternion quaternion;
			InputTracking.INTERNAL_CALL_GetLocalRotation(node, out quaternion);
			return quaternion;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Recenter();
	}
}
