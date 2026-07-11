using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PresenceModificationSetDataOptionsInternal : IDisposable
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

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_Records);
		}

		private int m_ApiVersion;

		private int m_RecordsCount;

		private IntPtr m_Records;
	}
}
