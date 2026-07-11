using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryOwnershipTokenOptionsInternal : IDisposable
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

		public EpicAccountId LocalUserId
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId>(this.m_LocalUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_LocalUserId, value);
			}
		}

		public string[] CatalogItemIds
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_CatalogItemIds, out @default, this.m_CatalogItemIdCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CatalogItemIds, value, out this.m_CatalogItemIdCount);
			}
		}

		public string CatalogNamespace
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_CatalogNamespace, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CatalogNamespace, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_CatalogItemIds);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_CatalogItemIds;

		private uint m_CatalogItemIdCount;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CatalogNamespace;
	}
}
