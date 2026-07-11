using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.TitleStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct FileMetadataInternal : IDisposable
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

		public uint FileSizeBytes
		{
			get
			{
				uint @default = Helper.GetDefault<uint>();
				Helper.TryMarshalGet<uint>(this.m_FileSizeBytes, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<uint>(ref this.m_FileSizeBytes, value);
			}
		}

		public string MD5Hash
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_MD5Hash, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_MD5Hash, value);
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

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		private uint m_FileSizeBytes;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_MD5Hash;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Filename;
	}
}
