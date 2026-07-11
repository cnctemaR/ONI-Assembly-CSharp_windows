using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ReceivePacketOptionsInternal : IDisposable
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

		public uint MaxDataSizeBytes
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_MaxDataSizeBytes, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_MaxDataSizeBytes, value);
			}
		}

		public byte? RequestedChannel
		{
			get
			{
				byte? @default = Helper.GetDefault<byte?>();
				Helper.TryMarshalGet<byte>(this.m_RequestedChannel, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<byte>(ref this.m_RequestedChannel, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_RequestedChannel);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private uint m_MaxDataSizeBytes;

		private IntPtr m_RequestedChannel;
	}
}
