using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class OwnerMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
			this.netId = reader.ReadNetworkId();
			this.playerControllerId = (short)reader.ReadPackedUInt32();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(this.netId);
			writer.WritePackedUInt32((uint)this.playerControllerId);
		}

		public NetworkInstanceId netId;

		public short playerControllerId;
	}
}
