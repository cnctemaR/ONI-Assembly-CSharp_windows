using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
	internal class TdsParserStateObjectManaged : TdsParserStateObject
	{
		public TdsParserStateObjectManaged(TdsParser parser)
			: base(parser)
		{
		}

		internal TdsParserStateObjectManaged(TdsParser parser, TdsParserStateObject physicalConnection, bool async)
			: base(parser, physicalConnection, async)
		{
		}

		internal SNIHandle Handle
		{
			get
			{
				return this._sessionHandle;
			}
		}

		internal override uint Status
		{
			get
			{
				if (this._sessionHandle == null)
				{
					return uint.MaxValue;
				}
				return this._sessionHandle.Status;
			}
		}

		internal override object SessionHandle
		{
			get
			{
				return this._sessionHandle;
			}
		}

		protected override object EmptyReadPacket
		{
			get
			{
				return null;
			}
		}

		protected override bool CheckPacket(object packet, TaskCompletionSource<object> source)
		{
			SNIPacket snipacket = packet as SNIPacket;
			return snipacket.IsInvalid || (!snipacket.IsInvalid && source != null);
		}

		protected override void CreateSessionHandle(TdsParserStateObject physicalConnection, bool async)
		{
			TdsParserStateObjectManaged tdsParserStateObjectManaged = physicalConnection as TdsParserStateObjectManaged;
			this._sessionHandle = tdsParserStateObjectManaged.CreateMarsSession(this, async);
		}

		internal SNIMarsHandle CreateMarsSession(object callbackObject, bool async)
		{
			return this._marsConnection.CreateMarsSession(callbackObject, async);
		}

		protected override uint SNIPacketGetData(object packet, byte[] _inBuff, ref uint dataSize)
		{
			return SNIProxy.Singleton.PacketGetData(packet as SNIPacket, _inBuff, ref dataSize);
		}

		internal override void CreatePhysicalSNIHandle(string serverName, bool ignoreSniOpenTimeout, long timerExpire, out byte[] instanceName, ref byte[] spnBuffer, bool flushCache, bool async, bool parallel, bool isIntegratedSecurity)
		{
			this._sessionHandle = SNIProxy.Singleton.CreateConnectionHandle(this, serverName, ignoreSniOpenTimeout, timerExpire, out instanceName, ref spnBuffer, flushCache, async, parallel, isIntegratedSecurity);
			if (this._sessionHandle == null)
			{
				this._parser.ProcessSNIError(this);
				return;
			}
			if (async)
			{
				SNIAsyncCallback sniasyncCallback = new SNIAsyncCallback(this.ReadAsyncCallback);
				SNIAsyncCallback sniasyncCallback2 = new SNIAsyncCallback(this.WriteAsyncCallback);
				this._sessionHandle.SetAsyncCallbacks(sniasyncCallback, sniasyncCallback2);
			}
		}

		internal void ReadAsyncCallback(SNIPacket packet, uint error)
		{
			base.ReadAsyncCallback<SNIPacket>(IntPtr.Zero, packet, error);
		}

		internal void WriteAsyncCallback(SNIPacket packet, uint sniError)
		{
			base.WriteAsyncCallback<SNIPacket>(IntPtr.Zero, packet, sniError);
		}

		protected override void RemovePacketFromPendingList(object packet)
		{
		}

		internal override void Dispose()
		{
			SNIPacket sniPacket = this._sniPacket;
			SNIHandle sessionHandle = this._sessionHandle;
			SNIPacket sniAsyncAttnPacket = this._sniAsyncAttnPacket;
			this._sniPacket = null;
			this._sessionHandle = null;
			this._sniAsyncAttnPacket = null;
			this._marsConnection = null;
			base.DisposeCounters();
			if (sessionHandle != null || sniPacket != null)
			{
				if (sniPacket != null)
				{
					sniPacket.Dispose();
				}
				if (sniAsyncAttnPacket != null)
				{
					sniAsyncAttnPacket.Dispose();
				}
				if (sessionHandle != null)
				{
					sessionHandle.Dispose();
					base.DecrementPendingCallbacks(true);
				}
			}
			this.DisposePacketCache();
		}

		internal override void DisposePacketCache()
		{
			object writePacketLockObject = this._writePacketLockObject;
			lock (writePacketLockObject)
			{
				this._writePacketCache.Dispose();
			}
		}

		protected override void FreeGcHandle(int remaining, bool release)
		{
		}

		internal override bool IsFailedHandle()
		{
			return this._sessionHandle.Status > 0U;
		}

		internal override object ReadSyncOverAsync(int timeoutRemaining, out uint error)
		{
			SNIHandle handle = this.Handle;
			if (handle == null)
			{
				throw ADP.ClosedConnectionError();
			}
			SNIPacket snipacket = null;
			error = SNIProxy.Singleton.ReadSyncOverAsync(handle, out snipacket, timeoutRemaining);
			return snipacket;
		}

		internal override bool IsPacketEmpty(object packet)
		{
			return packet == null;
		}

		internal override void ReleasePacket(object syncReadPacket)
		{
			((SNIPacket)syncReadPacket).Dispose();
		}

		internal override uint CheckConnection()
		{
			SNIHandle handle = this.Handle;
			if (handle != null)
			{
				return SNIProxy.Singleton.CheckConnection(handle);
			}
			return 0U;
		}

		internal override object ReadAsync(out uint error, ref object handle)
		{
			SNIPacket snipacket;
			error = SNIProxy.Singleton.ReadAsync((SNIHandle)handle, out snipacket);
			return snipacket;
		}

		internal override object CreateAndSetAttentionPacket()
		{
			if (this._sniAsyncAttnPacket == null)
			{
				SNIPacket snipacket = new SNIPacket();
				this.SetPacketData(snipacket, SQL.AttentionHeader, 8);
				this._sniAsyncAttnPacket = snipacket;
			}
			return this._sniAsyncAttnPacket;
		}

		internal override uint WritePacket(object packet, bool sync)
		{
			return SNIProxy.Singleton.WritePacket(this.Handle, (SNIPacket)packet, sync);
		}

		internal override object AddPacketToPendingList(object packet)
		{
			return packet;
		}

		internal override bool IsValidPacket(object packetPointer)
		{
			return (SNIPacket)packetPointer != null && !((SNIPacket)packetPointer).IsInvalid;
		}

		internal override object GetResetWritePacket()
		{
			if (this._sniPacket != null)
			{
				this._sniPacket.Reset();
			}
			else
			{
				object writePacketLockObject = this._writePacketLockObject;
				lock (writePacketLockObject)
				{
					this._sniPacket = this._writePacketCache.Take(this.Handle);
				}
			}
			return this._sniPacket;
		}

		internal override void ClearAllWritePackets()
		{
			if (this._sniPacket != null)
			{
				this._sniPacket.Dispose();
				this._sniPacket = null;
			}
			object writePacketLockObject = this._writePacketLockObject;
			lock (writePacketLockObject)
			{
				this._writePacketCache.Clear();
			}
		}

		internal override void SetPacketData(object packet, byte[] buffer, int bytesUsed)
		{
			SNIProxy.Singleton.PacketSetData((SNIPacket)packet, buffer, bytesUsed);
		}

		internal override uint SniGetConnectionId(ref Guid clientConnectionId)
		{
			return SNIProxy.Singleton.GetConnectionId(this.Handle, ref clientConnectionId);
		}

		internal override uint DisabeSsl()
		{
			return SNIProxy.Singleton.DisableSsl(this.Handle);
		}

		internal override uint EnableMars(ref uint info)
		{
			this._marsConnection = new SNIMarsConnection(this.Handle);
			if (this._marsConnection.StartReceive() == 997U)
			{
				return 0U;
			}
			return 1U;
		}

		internal override uint EnableSsl(ref uint info)
		{
			return SNIProxy.Singleton.EnableSsl(this.Handle, info);
		}

		internal override uint SetConnectionBufferSize(ref uint unsignedPacketSize)
		{
			return SNIProxy.Singleton.SetConnectionBufferSize(this.Handle, unsignedPacketSize);
		}

		internal override uint GenerateSspiClientContext(byte[] receivedBuff, uint receivedLength, ref byte[] sendBuff, ref uint sendLength, byte[] _sniSpnBuffer)
		{
			SNIProxy.Singleton.GenSspiClientContext(this.sspiClientContextStatus, receivedBuff, ref sendBuff, _sniSpnBuffer);
			sendLength = (uint)((sendBuff != null) ? sendBuff.Length : 0);
			return 0U;
		}

		internal override uint WaitForSSLHandShakeToComplete()
		{
			return 0U;
		}

		private SNIMarsConnection _marsConnection;

		private SNIHandle _sessionHandle;

		private SNIPacket _sniPacket;

		internal SNIPacket _sniAsyncAttnPacket;

		private readonly Dictionary<SNIPacket, SNIPacket> _pendingWritePackets = new Dictionary<SNIPacket, SNIPacket>();

		private readonly TdsParserStateObjectManaged.WritePacketCache _writePacketCache = new TdsParserStateObjectManaged.WritePacketCache();

		internal SspiClientContextStatus sspiClientContextStatus = new SspiClientContextStatus();

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
					snipacket.Reset();
				}
				else
				{
					snipacket = new SNIPacket();
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
}
