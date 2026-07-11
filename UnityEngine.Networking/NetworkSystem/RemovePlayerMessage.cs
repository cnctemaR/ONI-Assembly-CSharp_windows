using System;

namespace UnityEngine.Networking.NetworkSystem
{
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class RemovePlayerMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
			this.playerControllerId = (short)reader.ReadUInt16();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write((ushort)this.playerControllerId);
		}

		public short playerControllerId;
	}
}
