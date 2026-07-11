using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryEntitlementsOptionsInternal : IDisposable
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

		public string[] EntitlementNames
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_EntitlementNames, out @default, this.m_EntitlementNameCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_EntitlementNames, value, out this.m_EntitlementNameCount);
			}
		}

		public bool IncludeRedeemed
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_IncludeRedeemed, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_IncludeRedeemed, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_EntitlementNames);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_EntitlementNames;

		private uint m_EntitlementNameCount;

		private int m_IncludeRedeemed;
	}
}
