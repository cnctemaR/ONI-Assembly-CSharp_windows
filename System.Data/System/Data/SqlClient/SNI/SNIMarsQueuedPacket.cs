using System;

namespace System.Data.SqlClient.SNI
{
	internal class SNIMarsQueuedPacket
	{
		public SNIMarsQueuedPacket(SNIPacket packet, SNIAsyncCallback callback)
		{
			this._packet = packet;
			this._callback = callback;
		}

		public SNIPacket Packet
		{
			get
			{
				return this._packet;
			}
			set
			{
				this._packet = value;
			}
		}

		public SNIAsyncCallback Callback
		{
			get
			{
				return this._callback;
			}
			set
			{
				this._callback = value;
			}
		}

		private SNIPacket _packet;

		private SNIAsyncCallback _callback;
	}
}
