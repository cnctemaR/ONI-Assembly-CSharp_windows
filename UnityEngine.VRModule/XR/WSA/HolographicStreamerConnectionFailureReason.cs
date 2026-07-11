using System;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	/// <summary>
	///   <para>Enum indicating the reason why connection to remote device has failed.</para>
	/// </summary>
	[NativeHeader("Runtime/VR/HoloLens/PerceptionRemoting.h")]
	public enum HolographicStreamerConnectionFailureReason
	{
		/// <summary>
		///   <para>No failure.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Couldn't identify the reason why connection failed.</para>
		/// </summary>
		Unknown,
		/// <summary>
		///   <para>Remove device is not reachable.</para>
		/// </summary>
		Unreachable,
		/// <summary>
		///   <para>Handskahe failed while traying to establish connection with remote device.</para>
		/// </summary>
		HandshakeFailed,
		/// <summary>
		///   <para>Protocol used by the app does not match remoting app running on remote device.</para>
		/// </summary>
		ProtocolVersionMismatch,
		/// <summary>
		///   <para>Enum indicating the reason why remote connection failed.</para>
		/// </summary>
		ConnectionLost
	}
}
