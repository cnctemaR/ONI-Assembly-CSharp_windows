using System;

namespace UnityEngine.Networking.NetworkSystem
{
	internal class ObjectDestroyMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
			this.netId = reader.ReadNetworkId();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(this.netId);
		}

		public NetworkInstanceId netId;
	}
}
