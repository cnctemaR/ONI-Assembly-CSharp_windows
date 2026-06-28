using System;

namespace UnityEngine.Networking
{
	public class NetworkMessage
	{
		public static string Dump(byte[] payload, int sz)
		{
			string text = "[";
			for (int i = 0; i < sz; i++)
			{
				text = text + payload[i] + " ";
			}
			return text + "]";
		}

		public TMsg ReadMessage<TMsg>() where TMsg : MessageBase, new()
		{
			TMsg tmsg = new TMsg();
			tmsg.Deserialize(this.reader);
			return tmsg;
		}

		public void ReadMessage<TMsg>(TMsg msg) where TMsg : MessageBase
		{
			msg.Deserialize(this.reader);
		}

		public const int MaxMessageSize = 65535;

		public short msgType;

		public NetworkConnection conn;

		public NetworkReader reader;

		public int channelId;
	}
}
