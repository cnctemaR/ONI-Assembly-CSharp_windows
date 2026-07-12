using System;
using System.Collections.Generic;

namespace System.Data.SqlClient
{
	internal sealed class WritePacketCache : IDisposable
	{
		public WritePacketCache()
		{
			this._disposed = false;
			this._packets = new Stack<SNIPacket>();
		}

		public SNIPacket Take(SNIHandle sniHandle)
		{
			SNIPacket snipacket;
			if (this._packets.Count > 0)
			{
				snipacket = this._packets.Pop();
				SNINativeMethodWrapper.SNIPacketReset(sniHandle, SNINativeMethodWrapper.IOType.WRITE, snipacket, SNINativeMethodWrapper.ConsumerNumber.SNI_Consumer_SNI);
			}
			else
			{
				snipacket = new SNIPacket(sniHandle);
			}
			return snipacket;
		}

		public void Add(SNIPacket packet)
		{
			if (!this._disposed)
			{
				this._packets.Push(packet);
				return;
			}
			packet.Dispose();
		}

		public void Clear()
		{
			while (this._packets.Count > 0)
			{
				this._packets.Pop().Dispose();
			}
		}

		public void Dispose()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				this.Clear();
			}
		}

		private bool _disposed;

		private Stack<SNIPacket> _packets;
	}
}
