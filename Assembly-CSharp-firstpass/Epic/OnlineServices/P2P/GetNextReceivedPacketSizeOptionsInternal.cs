using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.P2P
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetNextReceivedPacketSizeOptionsInternal : IDisposable
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

		private IntPtr m_RequestedChannel;
	}
}
