using System;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA
{
	/// <summary>
	///   <para>Current state of the holographis streamer remote connection.</para>
	/// </summary>
	[NativeHeader("Runtime/VR/HoloLens/PerceptionRemoting.h")]
	public enum HolographicStreamerConnectionState
	{
		/// <summary>
		///   <para>Indicates app being currently disconnected from any other remote device.</para>
		/// </summary>
		Disconnected,
		/// <summary>
		///   <para>Indicates app trying to connect to remote device.</para>
		/// </summary>
		Connecting,
		/// <summary>
		///   <para>Indicates app being connected to remote device.</para>
		/// </summary>
		Connected
	}
}
