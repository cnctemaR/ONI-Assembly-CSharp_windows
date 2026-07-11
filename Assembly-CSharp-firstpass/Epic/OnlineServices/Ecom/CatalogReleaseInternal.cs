using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CatalogReleaseInternal : IDisposable
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

		public string[] CompatibleAppIds
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_CompatibleAppIds, out @default, this.m_CompatibleAppIdCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CompatibleAppIds, value, out this.m_CompatibleAppIdCount);
			}
		}

		public string[] CompatiblePlatforms
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_CompatiblePlatforms, out @default, this.m_CompatiblePlatformCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CompatiblePlatforms, value, out this.m_CompatiblePlatformCount);
			}
		}

		public string ReleaseNote
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ReleaseNote, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ReleaseNote, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_CompatibleAppIds);
			Helper.TryMarshalDispose(ref this.m_CompatiblePlatforms);
		}

		private int m_ApiVersion;

		private uint m_CompatibleAppIdCount;

		private IntPtr m_CompatibleAppIds;

		private uint m_CompatiblePlatformCount;

		private IntPtr m_CompatiblePlatforms;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ReleaseNote;
	}
}
