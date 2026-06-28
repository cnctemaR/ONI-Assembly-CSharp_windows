using System;

namespace UnityEngine.Networking.NetworkSystem
{
	public class ErrorMessage : MessageBase
	{
		public override void Deserialize(NetworkReader reader)
		{
			this.errorCode = (int)reader.ReadUInt16();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write((ushort)this.errorCode);
		}

		public int errorCode;
	}
}
