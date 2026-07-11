using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CheckoutOptionsInternal : IDisposable
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

		public string OverrideCatalogNamespace
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_OverrideCatalogNamespace, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_OverrideCatalogNamespace, value);
			}
		}

		public CheckoutEntryInternal[] Entries
		{
			get
			{
				CheckoutEntryInternal[] @default = Helper.GetDefault<CheckoutEntryInternal[]>();
				Helper.TryMarshalGet<CheckoutEntryInternal>(this.m_Entries, out @default, this.m_EntryCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<CheckoutEntryInternal>(ref this.m_Entries, value, out this.m_EntryCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Entries);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_OverrideCatalogNamespace;

		private uint m_EntryCount;

		private IntPtr m_Entries;
	}
}
