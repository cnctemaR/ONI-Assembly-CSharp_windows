using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct InfoInternal : IDisposable
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

		public Status Status
		{
			get
			{
				Status @default = Helper.GetDefault<Status>();
				Helper.TryMarshalGet<Status>(this.m_Status, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<Status>(ref this.m_Status, value);
			}
		}

		public EpicAccountId UserId
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId>(this.m_UserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_UserId, value);
			}
		}

		public string ProductId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ProductId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ProductId, value);
			}
		}

		public string ProductVersion
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ProductVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ProductVersion, value);
			}
		}

		public string Platform
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Platform, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Platform, value);
			}
		}

		public string RichText
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_RichText, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_RichText, value);
			}
		}

		public DataRecordInternal[] Records
		{
			get
			{
				DataRecordInternal[] @default = Helper.GetDefault<DataRecordInternal[]>();
				Helper.TryMarshalGet<DataRecordInternal>(this.m_Records, out @default, this.m_RecordsCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<DataRecordInternal>(ref this.m_Records, value, out this.m_RecordsCount);
			}
		}

		public string ProductName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_ProductName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_ProductName, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Records);
		}

		private int m_ApiVersion;

		private Status m_Status;

		private IntPtr m_UserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Platform;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RichText;

		private int m_RecordsCount;

		private IntPtr m_Records;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductName;
	}
}
