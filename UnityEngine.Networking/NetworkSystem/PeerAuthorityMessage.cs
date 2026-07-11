using System;

namespace UnityEngine.Networking.NetworkSystem
{
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class PeerAuthorityMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
			this.connectionId = (int)reader.ReadPackedUInt32();
			this.netId = reader.ReadNetworkId();
			this.authorityState = reader.ReadBoolean();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.WritePackedUInt32((uint)this.connectionId);
			writer.Write(this.netId);
			writer.Write(this.authorityState);
		}

		public int connectionId;

		public NetworkInstanceId netId;

		public bool authorityState;
	}
}
