using System;

namespace UnityEngine.Networking.PlayerConnection
{
	/// <summary>
	///   <para>Arguments passed to Action callbacks registered in PlayerConnection.</para>
	/// </summary>
	[Serializable]
	public class MessageEventArgs
	{
		/// <summary>
		///   <para>The Player ID that the data is received from.</para>
		/// </summary>
		public int playerId;

		/// <summary>
		///   <para>Data that is received.</para>
		/// </summary>
		public byte[] data;
	}
}
