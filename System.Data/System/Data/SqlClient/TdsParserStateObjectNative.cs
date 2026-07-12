using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
	internal class TdsParserStateObjectNative : TdsParserStateObject
	{
		public TdsParserStateObjectNative(TdsParser parser)
			: base(parser)
		{
		}

		internal TdsParserStateObjectNative(TdsParser parser, TdsParserStateObject physicalConnection, bool async)
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
				return IntPtr.Zero;
			}
		}

		protected override void CreateSessionHandle(TdsParserStateObject physicalConnection, bool async)
		{
			TdsParserStateObjectNative tdsParserStateObjectNative = physicalConnection as TdsParserStateObjectNative;
			SNINativeMethodWrapper.ConsumerInfo consumerInfo = this.CreateConsumerInfo(async);
			this._sessionHandle = new SNIHandle(consumerInfo, tdsParserStateObjectNative.Handle);
		}

		private SNINativeMethodWrapper.ConsumerInfo CreateConsumerInfo(bool async)
		{
			SNINativeMethodWrapper.ConsumerInfo consumerInfo = default(SNINativeMethodWrapper.ConsumerInfo);
			consumerInfo.defaultBufferSize = this._outBuff.Length;
			if (async)
			{
				consumerInfo.readDelegate = SNILoadHandle.SingletonInstance.ReadAsyncCallbackDispatcher;
				consumerInfo.writeDelegate = SNILoadHandle.SingletonInstance.WriteAsyncCallbackDispatcher;
				this._gcHandle = GCHandle.Alloc(this, GCHandleType.Normal);
				consumerInfo.key = (IntPtr)this._gcHandle;
			}
			return consumerInfo;
		}

		internal override void CreatePhysicalSNIHandle(string serverName, bool ignoreSniOpenTimeout, long timerExpire, out byte[] instanceName, ref byte[] spnBuffer, bool flushCache, bool async, bool fParallel, bool isIntegratedSecurity)
		{
			spnBuffer = null;
			if (isIntegratedSecurity)
			{
				spnBuffer = new byte[SNINativeMethodWrapper.SniMaxComposedSpnLength];
			}
			SNINativeMethodWrapper.ConsumerInfo consumerInfo = this.CreateConsumerInfo(async);
			long num;
			if (9223372036854775807L == timerExpire)
			{
				num = 2147483647L;
			}
			else
			{
				num = ADP.TimerRemainingMilliseconds(timerExpire);
				if (num > 2147483647L)
				{
					num = 2147483647L;
				}
				else if (0L > num)
				{
					num = 0L;
				}
			}
			this._sessionHandle = new SNIHandle(consumerInfo, serverName, spnBuffer, ignoreSniOpenTimeout, checked((int)num), out instanceName, flushCache, !async, fParallel);
		}

		protected override uint SNIPacketGetData(object packet, byte[] _inBuff, ref uint dataSize)
		{
			return SNINativeMethodWrapper.SNIPacketGetData((IntPtr)packet, _inBuff, ref dataSize);
		}

		protected override bool CheckPacket(object packet, TaskCompletionSource<object> source)
		{
			IntPtr intPtr = (IntPtr)packet;
			return IntPtr.Zero == intPtr || (IntPtr.Zero != intPtr && source != null);
		}

		public void ReadAsyncCallback(IntPtr key, IntPtr packet, uint error)
		{
			this.ReadAsyncCallback(key, packet, error);
		}

		public void WriteAsyncCallback(IntPtr key, IntPtr packet, uint sniError)
		{
			this.WriteAsyncCallback(key, packet, sniError);
		}

		protected override void RemovePacketFromPendingList(object ptr)
		{
			IntPtr intPtr = (IntPtr)ptr;
			object writePacketLockObject = this._writePacketLockObject;
			lock (writePacketLockObject)
			{
				SNIPacket snipacket;
				if (this._pendingWritePackets.TryGetValue(intPtr, out snipacket))
				{
					this._pendingWritePackets.Remove(intPtr);
					this._writePacketCache.Add(snipacket);
				}
			}
		}

		internal override void Dispose()
		{
			SafeHandle sniPacket = this._sniPacket;
			SafeHandle sessionHandle = this._sessionHandle;
			SafeHandle sniAsyncAttnPacket = this._sniAsyncAttnPacket;
			this._sniPacket = null;
			this._sessionHandle = null;
			this._sniAsyncAttnPacket = null;
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

		protected override void FreeGcHandle(int remaining, bool release)
		{
			if ((remaining == 0 || release) && this._gcHandle.IsAllocated)
			{
				this._gcHandle.Free();
			}
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
			IntPtr zero = IntPtr.Zero;
			error = SNINativeMethodWrapper.SNIReadSyncOverAsync(handle, ref zero, base.GetTimeoutRemaining());
			return zero;
		}

		internal override bool IsPacketEmpty(object readPacket)
		{
			return IntPtr.Zero == (IntPtr)readPacket;
		}

		internal override void ReleasePacket(object syncReadPacket)
		{
			SNINativeMethodWrapper.SNIPacketRelease((IntPtr)syncReadPacket);
		}

		internal override uint CheckConnection()
		{
			SNIHandle handle = this.Handle;
			if (handle != null)
			{
				return SNINativeMethodWrapper.SNICheckConnection(handle);
			}
			return 0U;
		}

		internal override object ReadAsync(out uint error, ref object handle)
		{
			IntPtr zero = IntPtr.Zero;
			error = SNINativeMethodWrapper.SNIReadAsync((SNIHandle)handle, ref zero);
			return zero;
		}

		internal override object CreateAndSetAttentionPacket()
		{
			SNIPacket snipacket = new SNIPacket(this.Handle);
			this._sniAsyncAttnPacket = snipacket;
			this.SetPacketData(snipacket, SQL.AttentionHeader, 8);
			return snipacket;
		}

		internal override uint WritePacket(object packet, bool sync)
		{
			return SNINativeMethodWrapper.SNIWritePacket(this.Handle, (SNIPacket)packet, sync);
		}

		internal override object AddPacketToPendingList(object packetToAdd)
		{
			SNIPacket snipacket = (SNIPacket)packetToAdd;
			this._sniPacket = null;
			IntPtr intPtr = snipacket.DangerousGetHandle();
			object writePacketLockObject = this._writePacketLockObject;
			lock (writePacketLockObject)
			{
				this._pendingWritePackets.Add(intPtr, snipacket);
			}
			return intPtr;
		}

		internal override bool IsValidPacket(object packetPointer)
		{
			return (IntPtr)packetPointer != IntPtr.Zero;
		}

		internal override object GetResetWritePacket()
		{
			if (this._sniPacket != null)
			{
				SNINativeMethodWrapper.SNIPacketReset(this.Handle, SNINativeMethodWrapper.IOType.WRITE, this._sniPacket, SNINativeMethodWrapper.ConsumerNumber.SNI_Consumer_SNI);
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
			SNINativeMethodWrapper.SNIPacketSetData((SNIPacket)packet, buffer, bytesUsed);
		}

		internal override uint SniGetConnectionId(ref Guid clientConnectionId)
		{
			return SNINativeMethodWrapper.SniGetConnectionId(this.Handle, ref clientConnectionId);
		}

		internal override uint DisabeSsl()
		{
			return SNINativeMethodWrapper.SNIRemoveProvider(this.Handle, SNINativeMethodWrapper.ProviderEnum.SSL_PROV);
		}

		internal override uint EnableMars(ref uint info)
		{
			return SNINativeMethodWrapper.SNIAddProvider(this.Handle, SNINativeMethodWrapper.ProviderEnum.SMUX_PROV, ref info);
		}

		internal override uint EnableSsl(ref uint info)
		{
			return SNINativeMethodWrapper.SNIAddProvider(this.Handle, SNINativeMethodWrapper.ProviderEnum.SSL_PROV, ref info);
		}

		internal override uint SetConnectionBufferSize(ref uint unsignedPacketSize)
		{
			return SNINativeMethodWrapper.SNISetInfo(this.Handle, SNINativeMethodWrapper.QTypes.SNI_QUERY_CONN_BUFSIZE, ref unsignedPacketSize);
		}

		internal override uint GenerateSspiClientContext(byte[] receivedBuff, uint receivedLength, ref byte[] sendBuff, ref uint sendLength, byte[] _sniSpnBuffer)
		{
			return SNINativeMethodWrapper.SNISecGenClientContext(this.Handle, receivedBuff, receivedLength, sendBuff, ref sendLength, _sniSpnBuffer);
		}

		internal override uint WaitForSSLHandShakeToComplete()
		{
			return SNINativeMethodWrapper.SNIWaitForSSLHandshakeToComplete(this.Handle, base.GetTimeoutRemaining());
		}

		internal override void DisposePacketCache()
		{
			object writePacketLockObject = this._writePacketLockObject;
			lock (writePacketLockObject)
			{
				this._writePacketCache.Dispose();
			}
		}

		private SNIHandle _sessionHandle;

		private SNIPacket _sniPacket;

		internal SNIPacket _sniAsyncAttnPacket;

		private readonly TdsParserStateObjectNative.WritePacketCache _writePacketCache = new TdsParserStateObjectNative.WritePacketCache();

		private GCHandle _gcHandle;

		private Dictionary<IntPtr, SNIPacket> _pendingWritePackets = new Dictionary<IntPtr, SNIPacket>();

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
}
