using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.PlayerDataStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ReadFileOptionsInternal : IDisposable, IInitializable
	{
		public void Initialize()
		{
			this.m_ReadFileDataCallback = new OnReadFileDataCallbackInternal(PlayerDataStorageInterface.OnReadFileData);
			this.m_FileTransferProgressCallback = new OnFileTransferProgressCallbackInternal(PlayerDataStorageInterface.OnFileTransferProgress);
		}

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

		public string Filename
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Filename, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Filename, value);
			}
		}

		public uint ReadChunkLengthBytes
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_ReadChunkLengthBytes, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_ReadChunkLengthBytes, value);
			}
		}

		public OnReadFileDataCallbackInternal ReadFileDataCallback
		{
			get
			{
				return this.m_ReadFileDataCallback;
			}
		}

		public OnFileTransferProgressCallbackInternal FileTransferProgressCallback
		{
			get
			{
				return this.m_FileTransferProgressCallback;
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Filename;

		private uint m_ReadChunkLengthBytes;

		private OnReadFileDataCallbackInternal m_ReadFileDataCallback;

		private OnFileTransferProgressCallbackInternal m_FileTransferProgressCallback;
	}
}
