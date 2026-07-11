using System;
using System.Collections.Generic;

namespace System.Data.SqlClient.SNI
{
	internal class SNIMarsConnection
	{
		public Guid ConnectionId
		{
			get
			{
				return this._connectionId;
			}
		}

		public SNIMarsConnection(SNIHandle lowerHandle)
		{
			this._lowerHandle = lowerHandle;
			this._lowerHandle.SetAsyncCallbacks(new SNIAsyncCallback(this.HandleReceiveComplete), new SNIAsyncCallback(this.HandleSendComplete));
		}

		public SNIMarsHandle CreateMarsSession(object callbackObject, bool async)
		{
			SNIMarsHandle snimarsHandle2;
			lock (this)
			{
				ushort nextSessionId = this._nextSessionId;
				this._nextSessionId = nextSessionId + 1;
				ushort num = nextSessionId;
				SNIMarsHandle snimarsHandle = new SNIMarsHandle(this, num, callbackObject, async);
				this._sessions.Add((int)num, snimarsHandle);
				snimarsHandle2 = snimarsHandle;
			}
			return snimarsHandle2;
		}

		public uint StartReceive()
		{
			SNIPacket snipacket = null;
			if (this.ReceiveAsync(ref snipacket) == 997U)
			{
				return 997U;
			}
			return SNICommon.ReportSNIError(SNIProviders.SMUX_PROV, 0U, 19U, string.Empty);
		}

		public uint Send(SNIPacket packet)
		{
			uint num;
			lock (this)
			{
				num = this._lowerHandle.Send(packet);
			}
			return num;
		}

		public uint SendAsync(SNIPacket packet, SNIAsyncCallback callback)
		{
			uint num;
			lock (this)
			{
				num = this._lowerHandle.SendAsync(packet, callback);
			}
			return num;
		}

		public uint ReceiveAsync(ref SNIPacket packet)
		{
			uint num;
			lock (this)
			{
				num = this._lowerHandle.ReceiveAsync(ref packet);
			}
			return num;
		}

		public uint CheckConnection()
		{
			uint num;
			lock (this)
			{
				num = this._lowerHandle.CheckConnection();
			}
			return num;
		}

		public void HandleReceiveError(SNIPacket packet)
		{
			foreach (SNIMarsHandle snimarsHandle in this._sessions.Values)
			{
				snimarsHandle.HandleReceiveError(packet);
			}
		}

		public void HandleSendComplete(SNIPacket packet, uint sniErrorCode)
		{
			packet.InvokeCompletionCallback(sniErrorCode);
		}

		public void HandleReceiveComplete(SNIPacket packet, uint sniErrorCode)
		{
			SNISMUXHeader snismuxheader = null;
			SNIPacket snipacket = null;
			SNIMarsHandle snimarsHandle = null;
			if (sniErrorCode != 0U)
			{
				SNIMarsConnection snimarsConnection = this;
				lock (snimarsConnection)
				{
					this.HandleReceiveError(packet);
					return;
				}
			}
			for (;;)
			{
				SNIMarsConnection snimarsConnection = this;
				lock (snimarsConnection)
				{
					if (this._currentHeaderByteCount != 16)
					{
						snismuxheader = null;
						snipacket = null;
						snimarsHandle = null;
						while (this._currentHeaderByteCount != 16)
						{
							int num = packet.TakeData(this._headerBytes, this._currentHeaderByteCount, 16 - this._currentHeaderByteCount);
							this._currentHeaderByteCount += num;
							if (num == 0)
							{
								sniErrorCode = this.ReceiveAsync(ref packet);
								if (sniErrorCode == 997U)
								{
									return;
								}
								this.HandleReceiveError(packet);
								return;
							}
						}
						this._currentHeader = new SNISMUXHeader
						{
							SMID = this._headerBytes[0],
							flags = this._headerBytes[1],
							sessionId = BitConverter.ToUInt16(this._headerBytes, 2),
							length = BitConverter.ToUInt32(this._headerBytes, 4) - 16U,
							sequenceNumber = BitConverter.ToUInt32(this._headerBytes, 8),
							highwater = BitConverter.ToUInt32(this._headerBytes, 12)
						};
						this._dataBytesLeft = (int)this._currentHeader.length;
						this._currentPacket = new SNIPacket(null);
						this._currentPacket.Allocate((int)this._currentHeader.length);
					}
					snismuxheader = this._currentHeader;
					snipacket = this._currentPacket;
					if (this._currentHeader.flags == 8 && this._dataBytesLeft > 0)
					{
						int num2 = packet.TakeData(this._currentPacket, this._dataBytesLeft);
						this._dataBytesLeft -= num2;
						if (this._dataBytesLeft > 0)
						{
							sniErrorCode = this.ReceiveAsync(ref packet);
							if (sniErrorCode == 997U)
							{
								break;
							}
							this.HandleReceiveError(packet);
							break;
						}
					}
					this._currentHeaderByteCount = 0;
					if (!this._sessions.ContainsKey((int)this._currentHeader.sessionId))
					{
						SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.SMUX_PROV, 0U, 5U, string.Empty);
						this.HandleReceiveError(packet);
						this._lowerHandle.Dispose();
						this._lowerHandle = null;
						break;
					}
					if (this._currentHeader.flags == 4)
					{
						this._sessions.Remove((int)this._currentHeader.sessionId);
					}
					else
					{
						snimarsHandle = this._sessions[(int)this._currentHeader.sessionId];
					}
				}
				if (snismuxheader.flags == 8)
				{
					snimarsHandle.HandleReceiveComplete(snipacket, snismuxheader);
				}
				if (this._currentHeader.flags == 2)
				{
					try
					{
						snimarsHandle.HandleAck(snismuxheader.highwater);
					}
					catch (Exception ex)
					{
						SNICommon.ReportSNIError(SNIProviders.SMUX_PROV, 35U, ex);
					}
				}
				snimarsConnection = this;
				lock (snimarsConnection)
				{
					if (packet.DataLeft != 0)
					{
						continue;
					}
					sniErrorCode = this.ReceiveAsync(ref packet);
					if (sniErrorCode != 997U)
					{
						this.HandleReceiveError(packet);
					}
				}
				break;
			}
		}

		public uint EnableSsl(uint options)
		{
			return this._lowerHandle.EnableSsl(options);
		}

		public void DisableSsl()
		{
			this._lowerHandle.DisableSsl();
		}

		private readonly Guid _connectionId = Guid.NewGuid();

		private readonly Dictionary<int, SNIMarsHandle> _sessions = new Dictionary<int, SNIMarsHandle>();

		private readonly byte[] _headerBytes = new byte[16];

		private SNIHandle _lowerHandle;

		private ushort _nextSessionId;

		private int _currentHeaderByteCount;

		private int _dataBytesLeft;

		private SNISMUXHeader _currentHeader;

		private SNIPacket _currentPacket;
	}
}
