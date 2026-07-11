using System;

namespace UnityEngine.Networking.NetworkSystem
{
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class PeerListMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
			this.oldServerConnectionId = (int)reader.ReadPackedUInt32();
			int num = (int)reader.ReadUInt16();
			this.peers = new PeerInfoMessage[num];
			for (int i = 0; i < this.peers.Length; i++)
			{
				PeerInfoMessage peerInfoMessage = new PeerInfoMessage();
				peerInfoMessage.Deserialize(reader);
				this.peers[i] = peerInfoMessage;
			}
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.WritePackedUInt32((uint)this.oldServerConnectionId);
			writer.Write((ushort)this.peers.Length);
			for (int i = 0; i < this.peers.Length; i++)
			{
				this.peers[i].Serialize(writer);
			}
		}

		public PeerInfoMessage[] peers;

		public int oldServerConnectionId;
	}
}
