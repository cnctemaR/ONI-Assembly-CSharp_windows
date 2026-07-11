using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Interface for reading and writing inputs in a Unity Cluster.</para>
	/// </summary>
	[NativeConditional("ENABLE_CLUSTERINPUT")]
	[NativeHeader("Modules/ClusterInput/ClusterInput.h")]
	public class ClusterInput
	{
		/// <summary>
		///   <para>Returns the axis value as a continous float.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.c.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAxis(string name);

		/// <summary>
		///   <para>Returns the binary value of a button.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetButton(string name);

		/// <summary>
		///   <para>Return the position of a tracker as a Vector3.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.</param>
		[NativeConditional("ENABLE_CLUSTERINPUT", "Vector3f(0.0f, 0.0f, 0.0f)")]
		public static Vector3 GetTrackerPosition(string name)
		{
			Vector3 vector;
			ClusterInput.GetTrackerPosition_Injected(name, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Returns the rotation of a tracker as a Quaternion.</para>
		/// </summary>
		/// <param name="name">Name of input to poll.</param>
		[NativeConditional("ENABLE_CLUSTERINPUT", "Quartenion::identity")]
		public static Quaternion GetTrackerRotation(string name)
		{
			Quaternion quaternion;
			ClusterInput.GetTrackerRotation_Injected(name, out quaternion);
			return quaternion;
		}

		/// <summary>
		///   <para>Sets the axis value for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAxis(string name, float value);

		/// <summary>
		///   <para>Sets the button value for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetButton(string name, bool value);

		/// <summary>
		///   <para>Sets the tracker position for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		public static void SetTrackerPosition(string name, Vector3 value)
		{
			ClusterInput.SetTrackerPosition_Injected(name, ref value);
		}

		/// <summary>
		///   <para>Sets the tracker rotation for this input. Only works for input typed Custom.</para>
		/// </summary>
		/// <param name="name">Name of input to modify.</param>
		/// <param name="value">Value to set.</param>
		public static void SetTrackerRotation(string name, Quaternion value)
		{
			ClusterInput.SetTrackerRotation_Injected(name, ref value);
		}

		/// <summary>
		///   <para>Add a new VRPN input entry.</para>
		/// </summary>
		/// <param name="name">Name of the input entry. This has to be unique.</param>
		/// <param name="deviceName">Device name registered to VRPN server.</param>
		/// <param name="serverUrl">URL to the vrpn server.</param>
		/// <param name="index">Index of the Input entry, refer to vrpn.cfg if unsure.</param>
		/// <param name="type">Type of the input.</param>
		/// <returns>
		///   <para>True if the operation succeed.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool AddInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		/// <summary>
		///   <para>Edit an input entry which added via ClusterInput.AddInput.</para>
		/// </summary>
		/// <param name="name">Name of the input entry. This has to be unique.</param>
		/// <param name="deviceName">Device name registered to VRPN server.</param>
		/// <param name="serverUrl">URL to the vrpn server.</param>
		/// <param name="index">Index of the Input entry, refer to vrpn.cfg if unsure.</param>
		/// <param name="type">Type of the ClusterInputType as follow.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool EditInput(string name, string deviceName, string serverUrl, int index, ClusterInputType type);

		/// <summary>
		///   <para>Check the connection status of the device to the VRPN server it connected to.</para>
		/// </summary>
		/// <param name="name">Name of the input entry.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CheckConnectionToServer(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrackerPosition_Injected(string name, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrackerRotation_Injected(string name, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTrackerPosition_Injected(string name, ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTrackerRotation_Injected(string name, ref Quaternion value);
	}
}
