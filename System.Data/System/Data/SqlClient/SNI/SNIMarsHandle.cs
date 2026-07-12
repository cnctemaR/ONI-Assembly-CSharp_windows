using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Data.SqlClient.SNI
{
	internal class SNIMarsHandle : SNIHandle
	{
		public override Guid ConnectionId
		{
			get
			{
				return this._connectionId;
			}
		}

		public override uint Status
		{
			get
			{
				return this._status;
			}
		}

		public override void Dispose()
		{
			try
			{
				this.SendControlPacket(SNISMUXFlags.SMUX_FIN);
			}
			catch (Exception ex)
			{
				SNICommon.ReportSNIError(SNIProviders.SMUX_PROV, 35U, ex);
				throw;
			}
		}

		public SNIMarsHandle(SNIMarsConnection connection, ushort sessionId, object callbackObject, bool async)
		{
			this._sessionId = sessionId;
			this._connection = connection;
			this._callbackObject = callbackObject;
			this.SendControlPacket(SNISMUXFlags.SMUX_SYN);
			this._status = 0U;
		}

		private void SendControlPacket(SNISMUXFlags flags)
		{
			byte[] array = null;
			lock (this)
			{
				this.GetSMUXHeaderBytes(0, (byte)flags, ref array);
			}
			SNIPacket snipacket = new SNIPacket();
			snipacket.SetData(array, 16);
			this._connection.Send(snipacket);
		}

		private void GetSMUXHeaderBytes(int length, byte flags, ref byte[] headerBytes)
		{
			headerBytes = new byte[16];
			this._currentHeader.SMID = 83;
			this._currentHeader.flags = flags;
			this._currentHeader.sessionId = this._sessionId;
			this._currentHeader.length = (uint)(16 + length);
			SNISMUXHeader currentHeader = this._currentHeader;
			uint num;
			if (flags != 4 && flags != 2)
			{
				uint sequenceNumber = this._sequenceNumber;
				this._sequenceNumber = sequenceNumber + 1U;
				num = sequenceNumber;
			}
			else
			{
				num = this._sequenceNumber - 1U;
			}
			currentHeader.sequenceNumber = num;
			this._currentHeader.highwater = this._receiveHighwater;
			this._receiveHighwaterLastAck = this._currentHeader.highwater;
			BitConverter.GetBytes((short)this._currentHeader.SMID).CopyTo(headerBytes, 0);
			BitConverter.GetBytes((short)this._currentHeader.flags).CopyTo(headerBytes, 1);
			BitConverter.GetBytes(this._currentHeader.sessionId).CopyTo(headerBytes, 2);
			BitConverter.GetBytes(this._currentHeader.length).CopyTo(headerBytes, 4);
			BitConverter.GetBytes(this._currentHeader.sequenceNumber).CopyTo(headerBytes, 8);
			BitConverter.GetBytes(this._currentHeader.highwater).CopyTo(headerBytes, 12);
		}

		private SNIPacket GetSMUXEncapsulatedPacket(SNIPacket packet)
		{
			uint sequenceNumber = this._sequenceNumber;
			byte[] array = null;
			this.GetSMUXHeaderBytes(packet.Length, 8, ref array);
			SNIPacket snipacket = new SNIPacket(16 + packet.Length);
			snipacket.Description = string.Format("({0}) SMUX packet {1}", (packet.Description == null) ? "" : packet.Description, sequenceNumber);
			snipacket.AppendData(array, 16);
			snipacket.AppendPacket(packet);
			return snipacket;
		}

		public override uint Send(SNIPacket packet)
		{
			for (;;)
			{
				SNIMarsHandle snimarsHandle = this;
				lock (snimarsHandle)
				{
					if (this._sequenceNumber < this._sendHighwater)
					{
						break;
					}
				}
				this._ackEvent.Wait();
				snimarsHandle = this;
				lock (snimarsHandle)
				{
					this._ackEvent.Reset();
					continue;
				}
				break;
			}
			return this._connection.Send(this.GetSMUXEncapsulatedPacket(packet));
		}

		private uint InternalSendAsync(SNIPacket packet, SNIAsyncCallback callback)
		{
			uint num;
			lock (this)
			{
				if (this._sequenceNumber >= this._sendHighwater)
				{
					num = 1048576U;
				}
				else
				{
					SNIPacket smuxencapsulatedPacket = this.GetSMUXEncapsulatedPacket(packet);
					if (callback != null)
					{
						smuxencapsulatedPacket.SetCompletionCallback(callback);
					}
					else
					{
						smuxencapsulatedPacket.SetCompletionCallback(new SNIAsyncCallback(this.HandleSendComplete));
					}
					num = this._connection.SendAsync(smuxencapsulatedPacket, callback);
				}
			}
			return num;
		}

		private uint SendPendingPackets()
		{
			for (;;)
			{
				lock (this)
				{
					if (this._sequenceNumber < this._sendHighwater)
					{
						if (this._sendPacketQueue.Count != 0)
						{
							SNIMarsQueuedPacket snimarsQueuedPacket = this._sendPacketQueue.Peek();
							uint num = this.InternalSendAsync(snimarsQueuedPacket.Packet, snimarsQueuedPacket.Callback);
							if (num != 0U && num != 997U)
							{
								return num;
							}
							this._sendPacketQueue.Dequeue();
							continue;
						}
						else
						{
							this._ackEvent.Set();
						}
					}
				}
				break;
			}
			return 0U;
		}

		public override uint SendAsync(SNIPacket packet, bool disposePacketAfterSendAsync, SNIAsyncCallback callback = null)
		{
			lock (this)
			{
				this._sendPacketQueue.Enqueue(new SNIMarsQueuedPacket(packet, (callback != null) ? callback : new SNIAsyncCallback(this.HandleSendComplete)));
			}
			this.SendPendingPackets();
			return 997U;
		}

		public override uint ReceiveAsync(ref SNIPacket packet)
		{
			Queue<SNIPacket> receivedPacketQueue = this._receivedPacketQueue;
			lock (receivedPacketQueue)
			{
				int count = this._receivedPacketQueue.Count;
				if (this._connectionError != null)
				{
					return SNICommon.ReportSNIError(this._connectionError);
				}
				if (count == 0)
				{
					this._asyncReceives++;
					return 997U;
				}
				packet = this._receivedPacketQueue.Dequeue();
				if (count == 1)
				{
					this._packetEvent.Reset();
				}
			}
			lock (this)
			{
				this._receiveHighwater += 1U;
			}
			this.SendAckIfNecessary();
			return 0U;
		}

		public void HandleReceiveError(SNIPacket packet)
		{
			Queue<SNIPacket> receivedPacketQueue = this._receivedPacketQueue;
			lock (receivedPacketQueue)
			{
				this._connectionError = SNILoadHandle.SingletonInstance.LastError;
				this._packetEvent.Set();
			}
			((TdsParserStateObject)this._callbackObject).ReadAsyncCallback<SNIPacket>(packet, 1U);
		}

		public void HandleSendComplete(SNIPacket packet, uint sniErrorCode)
		{
			lock (this)
			{
				((TdsParserStateObject)this._callbackObject).WriteAsyncCallback<SNIPacket>(packet, sniErrorCode);
			}
		}

		public void HandleAck(uint highwater)
		{
			lock (this)
			{
				if (this._sendHighwater != highwater)
				{
					this._sendHighwater = highwater;
					this.SendPendingPackets();
				}
			}
		}

		public void HandleReceiveComplete(SNIPacket packet, SNISMUXHeader header)
		{
			SNIMarsHandle snimarsHandle = this;
			lock (snimarsHandle)
			{
				if (this._sendHighwater != header.highwater)
				{
					this.HandleAck(header.highwater);
				}
				Queue<SNIPacket> receivedPacketQueue = this._receivedPacketQueue;
				lock (receivedPacketQueue)
				{
					if (this._asyncReceives == 0)
					{
						this._receivedPacketQueue.Enqueue(packet);
						this._packetEvent.Set();
						return;
					}
					this._asyncReceives--;
					((TdsParserStateObject)this._callbackObject).ReadAsyncCallback<SNIPacket>(packet, 0U);
				}
			}
			snimarsHandle = this;
			lock (snimarsHandle)
			{
				this._receiveHighwater += 1U;
			}
			this.SendAckIfNecessary();
		}

		private void SendAckIfNecessary()
		{
			uint receiveHighwater;
			uint receiveHighwaterLastAck;
			lock (this)
			{
				receiveHighwater = this._receiveHighwater;
				receiveHighwaterLastAck = this._receiveHighwaterLastAck;
			}
			if (receiveHighwater - receiveHighwaterLastAck > 2U)
			{
				this.SendControlPacket(SNISMUXFlags.SMUX_ACK);
			}
		}

		public override uint Receive(out SNIPacket packet, int timeoutInMilliseconds)
		{
			packet = null;
			uint num = 997U;
			for (;;)
			{
				Queue<SNIPacket> receivedPacketQueue = this._receivedPacketQueue;
				lock (receivedPacketQueue)
				{
					if (this._connectionError != null)
					{
						return SNICommon.ReportSNIError(this._connectionError);
					}
					int count = this._receivedPacketQueue.Count;
					if (count > 0)
					{
						packet = this._receivedPacketQueue.Dequeue();
						if (count == 1)
						{
							this._packetEvent.Reset();
						}
						num = 0U;
					}
				}
				if (num == 0U)
				{
					break;
				}
				if (!this._packetEvent.Wait(timeoutInMilliseconds))
				{
					goto Block_4;
				}
			}
			lock (this)
			{
				this._receiveHighwater += 1U;
			}
			this.SendAckIfNecessary();
			return num;
			Block_4:
			SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.SMUX_PROV, 0U, 11U, string.Empty);
			return 258U;
		}

		public override uint CheckConnection()
		{
			return this._connection.CheckConnection();
		}

		public override void SetAsyncCallbacks(SNIAsyncCallback receiveCallback, SNIAsyncCallback sendCallback)
		{
		}

		public override void SetBufferSize(int bufferSize)
		{
		}

		public override uint EnableSsl(uint options)
		{
			return this._connection.EnableSsl(options);
		}

		public override void DisableSsl()
		{
			this._connection.DisableSsl();
		}

		private const uint ACK_THRESHOLD = 2U;

		private readonly SNIMarsConnection _connection;

		private readonly uint _status = uint.MaxValue;

		private readonly Queue<SNIPacket> _receivedPacketQueue = new Queue<SNIPacket>();

		private readonly Queue<SNIMarsQueuedPacket> _sendPacketQueue = new Queue<SNIMarsQueuedPacket>();

		private readonly object _callbackObject;

		private readonly Guid _connectionId = Guid.NewGuid();

		private readonly ushort _sessionId;

		private readonly ManualResetEventSlim _packetEvent = new ManualResetEventSlim(false);

		private readonly ManualResetEventSlim _ackEvent = new ManualResetEventSlim(false);

		private readonly SNISMUXHeader _currentHeader = new SNISMUXHeader();

		private uint _sendHighwater = 4U;

		private int _asyncReceives;

		private uint _receiveHighwater = 4U;

		private uint _receiveHighwaterLastAck = 4U;

		private uint _sequenceNumber;

		private SNIError _connectionError;
	}
}
