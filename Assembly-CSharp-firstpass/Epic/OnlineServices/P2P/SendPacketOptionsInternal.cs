using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SendPacketOptionsInternal : IDisposable
	{
		public int ApiVersion
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ApiVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ApiVersion, value);
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_LocalUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_LocalUserId, value);
			}
		}

		public ProductUserId RemoteUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_RemoteUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_RemoteUserId, value);
			}
		}

		public SocketIdInternal? SocketId
		{
			get
			{
				SocketIdInternal? @default = Helper.GetDefault<SocketIdInternal?>();
				Helper.TryMarshalGet<SocketIdInternal>(this.m_SocketId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<SocketIdInternal>(ref this.m_SocketId, value);
			}
		}

		public byte Channel
		{
			get
			{
				byte @default = Helper.GetDefault<byte>();
				Helper.TryMarshalGet<byte>(this.m_Channel, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<byte>(ref this.m_Channel, value);
			}
		}

		public byte[] Data
		{
			get
			{
				byte[] @default = Helper.GetDefault<byte[]>();
				Helper.TryMarshalGet<byte>(this.m_Data, out @default, this.m_DataLengthBytes);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<byte>(ref this.m_Data, value, out this.m_DataLengthBytes);
			}
		}

		public bool AllowDelayedDelivery
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_AllowDelayedDelivery, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_AllowDelayedDelivery, value);
			}
		}

		public PacketReliability Reliability
		{
			get
			{
				PacketReliability @default = Helper.GetDefault<PacketReliability>();
				Helper.TryMarshalGet<PacketReliability>(this.m_Reliability, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<PacketReliability>(ref this.m_Reliability, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_SocketId);
			Helper.TryMarshalDispose(ref this.m_Data);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_RemoteUserId;

		private IntPtr m_SocketId;

		private byte m_Channel;

		private uint m_DataLengthBytes;

		private IntPtr m_Data;

		private int m_AllowDelayedDelivery;

		private PacketReliability m_Reliability;
	}
}
