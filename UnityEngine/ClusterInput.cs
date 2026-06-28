using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class ClusterInput
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAxis(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetButton(string name);

		public static Vector3 GetTrackerPosition(string name)
		{
			Vector3 vector;
			ClusterInput.INTERNAL_CALL_GetTrackerPosition(name, out vector);
			return vector;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerPosition(string name, out Vector3 value);

		public static Quaternion GetTrackerRotation(string name)
		{
			Quaternion quaternion;
			ClusterInput.INTERNAL_CALL_GetTrackerRotation(name, out quaternion);
			return quaternion;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetTrackerRotation(string name, out Quaternion value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAxis(string name, float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetButton(string name, bool value);

		public static void SetTrackerPosition(string name, Vector3 value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerPosition(name, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerPosition(string name, ref Vector3 value);

		public static void SetTrackerRotation(string name, Quaternion value)
		{
			ClusterInput.INTERNAL_CALL_SetTrackerRotation(name, ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTrackerRotation(string name, ref Quaternion value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool AddInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EditInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CheckConnectionToServer(string name);
	}
}
