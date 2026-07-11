using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.TitleStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ReadFileDataCallbackInfoInternal : ICallbackInfo
	{
		public object ClientData
		{
			get
			{
				object @default = Helper.GetDefault<object>();
				Helper.TryMarshalGet(this.m_ClientData, out @default);
				return @default;
			}
		}

		public IntPtr ClientDataAddress
		{
			get
			{
				return this.m_ClientData;
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
		}

		public string Filename
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Filename, out @default);
				return @default;
			}
		}

		public uint TotalFileSizeBytes
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_TotalFileSizeBytes, out @default);
				return @default;
			}
		}

		public bool IsLastChunk
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_IsLastChunk, out @default);
				return @default;
			}
		}

		public byte[] DataChunk
		{
			get
			{
				byte[] @default = Helper.GetDefault<byte[]>();
				Helper.TryMarshalGet<byte>(this.m_DataChunk, out @default, this.m_DataChunkLengthBytes);
				return @default;
			}
		}

		private IntPtr m_ClientData;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Filename;

		private uint m_TotalFileSizeBytes;

		private int m_IsLastChunk;

		private uint m_DataChunkLengthBytes;

		private IntPtr m_DataChunk;
	}
}
