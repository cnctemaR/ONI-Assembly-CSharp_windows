using System;

namespace UnityEngine.XR
{
	/// <summary>
	///   <para>Represents the current user presence state detected by the device.</para>
	/// </summary>
	public enum UserPresenceState
	{
		/// <summary>
		///   <para>The device does not support detecting user presence.</para>
		/// </summary>
		Unsupported = -1,
		/// <summary>
		///   <para>The device does not detect that the user is present.</para>
		/// </summary>
		NotPresent,
		/// <summary>
		///   <para>The device detects that the user is present.</para>
		/// </summary>
		Present,
		/// <summary>
		///   <para>The device is currently in a state where it cannot determine user presence.</para>
		/// </summary>
		Unknown
	}
}
