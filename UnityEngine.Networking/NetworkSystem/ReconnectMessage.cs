using System;

namespace UnityEngine.Networking.NetworkSystem
{
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class ReconnectMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
			this.oldConnectionId = (int)reader.ReadPackedUInt32();
			this.playerControllerId = (short)reader.ReadPackedUInt32();
			this.netId = reader.ReadNetworkId();
			this.msgData = reader.ReadBytesAndSize();
			this.msgSize = this.msgData.Length;
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.WritePackedUInt32((uint)this.oldConnectionId);
			writer.WritePackedUInt32((uint)this.playerControllerId);
			writer.Write(this.netId);
			writer.WriteBytesAndSize(this.msgData, this.msgSize);
		}

		public int oldConnectionId;

		public short playerControllerId;

		public NetworkInstanceId netId;

		public int msgSize;

		public byte[] msgData;
	}
}
