using System;

namespace UnityEngine.Networking.NetworkSystem
{
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class IntegerMessage : MessageBase
	{
		public IntegerMessage()
		{
		}

		public IntegerMessage(int v)
		{
			this.value = v;
		}

		public override void Deserialize(NetworkReader reader)
		{
			this.value = (int)reader.ReadPackedUInt32();
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.WritePackedUInt32((uint)this.value);
		}

		public int value;
	}
}
