using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ItemOwnershipInternal : IDisposable
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

		public string Id
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Id, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Id, value);
			}
		}

		public OwnershipStatus OwnershipStatus
		{
			get
			{
				OwnershipStatus @default = Helper.GetDefault<OwnershipStatus>();
				Helper.TryMarshalGet<OwnershipStatus>(this.m_OwnershipStatus, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<OwnershipStatus>(ref this.m_OwnershipStatus, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Id;

		private OwnershipStatus m_OwnershipStatus;
	}
}
