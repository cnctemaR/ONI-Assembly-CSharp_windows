using System;

namespace UnityEngine.XR.WSA
{
	/// <summary>
	///   <para>he Holographic Remoting interface allows you to connect an application to a remote holographic device, and stream data between the application and that device.</para>
	/// </summary>
	public class HolographicRemoting
	{
		/// <summary>
		///   <para>Whether the app is displaying protected content.</para>
		/// </summary>
		public static HolographicStreamerConnectionState ConnectionState
		{
			get
			{
				return HolographicStreamerConnectionState.Disconnected;
			}
		}

		public static void Connect(string clientName, int maxBitRate = 9999)
		{
		}

		public static void Disconnect()
		{
		}
	}
}
